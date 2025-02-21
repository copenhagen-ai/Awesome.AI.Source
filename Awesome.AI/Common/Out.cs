using Awesome.AI.Common;
using Awesome.AI.Core;
using Awesome.AI.Helpers;
using Microsoft.CodeAnalysis.Elfie.Model.Strings;
using static Awesome.AI.Helpers.Enums;

namespace Awesome.AI.Web.AI.Common
{
    public class Out
    {
        private TheMind mind;
        private Out() { }
        public Out(TheMind mind)
        {
            this.mind = mind;
        }

        public string cycles { get; set; }
        public string cycles_total { get; set; }
        public string momentum { get; set; }
        public string deltaMom { get; set; }

        public string pain { get; set; }
        public string position { get; set; }
        public string ratio_yes { get; set; }
        public string ratio_no { get; set; }
        public string going_down { get; set; }
        public string epochs { get; set; }
        public string runtime { get; set; }
        public string occu { get; set; }
        public string location { get; set; }
        public string loc_state { get; set; }
        public string chat_state { get; set; }
        public string chat_answer { get; set; }
        public string chat_subject { get; set; }
        public string common_hub { get; set; }

        //public string chat_index { get; set; }
        public UNIT common_unit { get; set; }

        public async Task<string> GetAnswer()
        {
            chat_answer = "";

            int count = 0;
            while((chat_answer == null || chat_answer == "") && count++ < 60)
                await Task.Delay(1000);

            return count >= 59 ? ":COMEAGAIN" : chat_answer;
        }

        public void Set()
        {
            cycles = $"{mind.cycles}";
            cycles_total = $"{mind.cycles_all}";
            momentum = $"{mind.mech.momentum.ToString("E3")}";
            deltaMom = $"{mind.mech.deltaMom.ToString("E3")}";

            pain = $"{mind.pain}";
            if (mind._mech == MECHANICS.HILL)
                position = $"{mind.mech.POS_XY}";
            if (mind._mech == MECHANICS.CONTEST)
                position = $"{mind.pos.Pos}";
            if (mind._mech == MECHANICS.GRAVITY)
                position = $"{mind.pos.Pos}";
            ratio_yes = $"{mind.dir.Count(HARDDOWN.YES)}";
            ratio_no = $"{mind.dir.Count(HARDDOWN.NO)}";
            going_down = $"{(mind.dir.DownHard.IsNo() ? "NO" : "YES")}";
            epochs = $"{mind.epochs}";
            runtime = $"{Constants.RUNTIME}";
            occu = $"{mind._internal.Occu}";
            location = $"{mind.loc.LocationFinal}";
            loc_state = mind.loc.LocationState > 0 ? "making a decision" : "just thinking";
            chat_state = mind.chatans.ChatState > 0 ? "thinking" : "just thinking";

            if (mind.chatans.Answer != "") {
                chat_answer = $"{mind.chatans.Answer}";
                mind.chatans.Answer = "";
            }

            if(mind.chatask.Subject != "") {
                chat_subject = $"{mind.chatask.Subject}";
                mind.chatask.Subject = "";
            }
            //chat_index= $"{mind.chatask.Index}";

            //mind.chatask.Index = "";

            common_unit = mind.process.most_common_unit;

            if (common_unit == null)
                return;
                       
            common_hub = common_unit.HUB.GetSubject();
        }
    }
}
