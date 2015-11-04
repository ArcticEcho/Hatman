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



        public bool Update()
        {
            var projJson = Encoding.UTF8.GetString(Get("projects/ArcticEcho/hatman"));
            var remoteVer = DynamicJson.Deserialize(projJson).build.version;

            if (remoteVer == curVer)
            {
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

            return true;
        }

        public bool StartNewVersion()
        {
            if (string.IsNullOrWhiteSpace(newVerExePath))
            {
                if (!Update())
                {
                    return false;
                }
            }

            Process.Start(newVerExePath);

            return true;
        }



        private byte[] Get(string url)
        {
            var req = (HttpWebRequest)WebRequest.Create(apiUrl + url);

            req.Method = "get";
            req.Headers.Add("Authorization", "Bearer " + tkn);
            req.ContentType = "application/json";

            var res = (HttpWebResponse)req.GetResponse();
            using (var strm = res.GetResponseStream())
            {
                return strm.ReadFully();
            }
        }

        private void GetFile(string url, string filepath)
        {
            var bytes = Get(url);
            using (var file = new FileStream(filepath, FileMode.Create))
            {
                file.Write(bytes, 0, bytes.Length);
            }
        }

        private void RemoveOldFile(string filename)
        {
            var files = Directory.EnumerateFiles(Directory.GetCurrentDirectory());

            foreach (var file in files)
            {
                var fn = Path.GetFileNameWithoutExtension(file);
                if (Regex.IsMatch(fn, @"\d+\.\d+\.\d+\.\d+ - " + filename.Replace(".", @"\.")) &&
                    !fn.StartsWith(curVer))
                {
                    File.Delete(file);
                }
            }
        }
    }
}
