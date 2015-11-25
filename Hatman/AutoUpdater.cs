using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using ServiceStack;
using ServiceStack.Text;

namespace Hatman
{
    class AutoUpdater
    {
        private const string apiUrl = "https://ci.appveyor.com/api/";
        private readonly string tkn;
        private readonly string curVer;
        private string newVerExePath;



        public AutoUpdater(string appveyorApiToken)
        {
            if (string.IsNullOrWhiteSpace(appveyorApiToken))
            {
                throw new ArgumentException("appveyorApiToken");
            }

            tkn = appveyorApiToken;

            var projJson = Encoding.UTF8.GetString(Get("projects/ArcticEcho/hatman"));
            curVer = DynamicJson.Deserialize(projJson).build.version;
        }



        public bool Update(out string oldVersion, out string newVersion, out string updateMessage)
        {
            var projJson = Encoding.UTF8.GetString(Get("projects/ArcticEcho/hatman"));
            var remoteVer = DynamicJson.Deserialize(projJson).build.version;

            if (remoteVer == curVer)
            {
                oldVersion = null;
                newVersion = null;
                updateMessage = null;

                return false;
            }

            var jobId = DynamicJson.Deserialize(projJson).build.jobs[0].jobId;
            var artifRes = Encoding.UTF8.GetString(Get("buildjobs/" + jobId + "/artifacts"));
            var artifJson = JsonSerializer.DeserializeFromString<Dictionary<string, object>[]>(artifRes);

            foreach (var file in artifJson)
            {
                var newFile = remoteVer + " - " + file["name"];
                RemoveOldFile(file["name"]);
                GetFile("buildjobs/" + jobId + "/artifacts/" + file["fileName"], newFile);

                if (file["name"].EndsWith(".exe"))
                {
                    newVerExePath = Path.Combine(Directory.GetCurrentDirectory(), newFile);
                }
            }

            oldVersion = curVer;
            newVersion = remoteVer;
            updateMessage = DynamicJson.Deserialize(projJson).build.message;

            return true;
        }

        public bool StartNewVersion()
        {
            if (string.IsNullOrWhiteSpace(newVerExePath))
            {
                string x, y, z;

                if (!Update(out x, out y, out z))
                {
                    return false;
                }
            }

            Process.Start(newVerExePath);

            return true;
        }



        private byte[] Get(string url, bool file = false)
        {
            var req = (HttpWebRequest)WebRequest.Create(apiUrl + url);

            req.Method = "get";
            req.Headers.Add("Authorization", "Bearer " + tkn);
            if (!file)
            {
                req.ContentType = "application/json";
            }

            var res = (HttpWebResponse)req.GetResponse();
            using (var strm = res.GetResponseStream())
            {
                return strm.ReadFully();
            }
        }

        private void GetFile(string url, string filepath)
        {
            var bytes = Get(url, true);
            using (var file = new FileStream(filepath, FileMode.Create))
            {
                file.Write(bytes, 0, bytes.Length);
            }
        }

        private void RemoveOldFile(string filename)
        {
            var files = Directory.EnumerateFiles(Directory.GetCurrentDirectory());
            var fn = Path.GetFileNameWithoutExtension(filename).Replace(".", @"\.");

            foreach (var file in files)
            {
                var oldFn = Path.GetFileNameWithoutExtension(file);
                if (Regex.IsMatch(oldFn, @"\d+\.\d+\.\d+\.\d+ - " + fn) &&
                    !fn.StartsWith(curVer))
                {
                    File.Delete(file);
                }
            }
        }
    }
}
