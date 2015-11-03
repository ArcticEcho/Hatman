using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ChatExchangeDotNet;

namespace Hatman.Commands
{
    class Face : ICommand
    {
        private readonly Regex ptn = new Regex(@"(?i)face", Extensions.RegOpts);
        private readonly string[] faces = new[]
        {
            "( ͡° ͜ʖ ͡°)", "¯\\\\_(ツ)_/¯", "༼ つ ◕_◕ ༽つ", "(ง ͠° ͟ل͜ ͡°)ง",
            "ಠ_ಠ", "( ͡°╭͜ʖ╮͡° )", "(ノಠ益ಠ)ノ彡┻━┻", "( ͡° ʖ̯ ͡°)",
            "[̲̅$̲̅(̲̅ ͡° ͜ʖ ͡°̲̅)̲̅$̲̅]", "﴾͡๏̯͡๏﴿ O'RLY?", "༼ つ  ͡° ͜ʖ ͡° ༽つ",
            "(ಥ﹏ಥ)", "| (• ◡•)| (❍ᴥ❍ʋ)", "ლ(ಠ益ಠლ)",
            "(╯°□°)╯︵ ʞooqǝɔɐɟ", "( ͡ᵔ ͜ʖ ͡ᵔ )", "(╯°□°)╯︵ ┻━┻",
            "༼ つ ಥ_ಥ ༽つ", "ᕙ(⇀‸↼‶)ᕗ", "ಠ╭╮ಠ", "ヽ༼ຈل͜ຈ༽ﾉ", "◉_◉",
            "ᕦ(ò_óˇ)ᕤ", "(≧ω≦)", "~(˘▾˘~)", "ᄽὁȍ ̪ őὀᄿ", "┬──┬ ノ( ゜-゜ノ)",
            "( ಠ ͜ʖರೃ)", "┌( ಠ_ಠ)┘", "(ง°ل͜°)ง", "(°ロ°)☝", "(~˘▾˘)~",
            "☜(˚▽˚)☞", "(ಥ_ಥ)", "ლ,ᔑ•ﺪ͟͠•ᔐ.ლ", "(ʘᗩʘ')", "(⊙ω⊙)",
            "⚆ _ ⚆", "°Д°", "ب_ب", "☉_☉", "ಠ~ಠ", "ರ_ರ", "ಠoಠ", "◔ ⌣ ◔",
            "¬_¬", "⊜_⊜", ":)", ":(", ":D", "D:", ":p", ":P", ":o", ":O",
            ":0", ":]", ":[", ":x", ":3", "e.e", "o.o", "O.O", "O.o", "e_e",
            "o_o", "O_O", "O_o", "-_-", "'_'", ">_>", "<_<", "\\o", "o/", "\\o/",
            "http://i.stack.imgur.com/fNpod.jpg",
            "http://i.stack.imgur.com/2X6XE.jpg",
            "http://i.stack.imgur.com/S35ZH.jpg",
            "http://i.stack.imgur.com/Y7Jw8.png",
            "http://i.stack.imgur.com/Oduwh.jpg"
        };

        public Regex CommandPattern
        {
            get
            {
                return ptn;
            }
        }

        public string Description
        {
            get
            {
                return "Posts a random face.";
            }
        }

        public string Usage
        {
            get
            {
                return "[...]face[...]";
            }
        }

        public void ProcessMessage(Message msg, ref Room rm)
        {
            rm.PostReplyFast(msg, faces.PickRandom());
        }
    }
}
