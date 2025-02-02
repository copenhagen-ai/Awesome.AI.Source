using Awesome.AI.Common;
using Awesome.AI.Core;
using Microsoft.CodeAnalysis.Elfie.Model.Strings;

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

        public string pain { get; set; }
        public string position { get; set; }
        public string ratio_yes { get; set; }
        public string ratio_no { get; set; }
        public string the_choise { get; set; }
        public string epochs { get; set; }
        public string runtime { get; set; }
        public string occu { get; set; }
        public string location { get; set; }
        public string loc_state { get; set; }
        public string chat_state { get; set; }
        public string chat_answer { get; set; }
        public string chat_subject { get; set; }
        //public string chat_index { get; set; }

        public UNIT common_unit { get; set; }
        public string common_hub { get; set; }

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
            momentum = $"{mind.parms._mech.dir.d_momentum}";

            pain = $"{mind.pain}";
            position = $"{mind.parms._mech.dir.d_pos_x}";
            ratio_yes = $"{mind.parms._mech.dir.ratio.CountYes()}";
            ratio_no = $"{mind.parms._mech.dir.ratio.CountNo()}";
            the_choise = $"{(mind.parms._mech.dir.Choise.IsNo() ? "NO" : "YES")}";
            epochs = $"{mind.epochs}";
            runtime = $"{mind.parms.runtime}";
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
            
            common_hub = common_unit.HUB.GetSubject();
        }
    }
}
