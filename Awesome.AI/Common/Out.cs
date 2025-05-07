using Awesome.AI.Common;
using Awesome.AI.Core;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

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
        public string p_curr { get; set; }
        public string d_curr { get; set; }

        public string user_var { get; set; }
        public string position { get; set; }
        public string ratio_yes_n { get; set; }
        public string ratio_no_n { get; set; }
        public string going_down { get; set; }
        public string epochs { get; set; }
        public string runtime { get; set; }
        public string occu { get; set; }
        public string location { get; set; }
        public string loc_state { get; set; }
        public string chat_answer { get; set; }
        public string chat_subject { get; set; }
        public string whistle { get; set; }

        public string mood {  get; set; }
        public bool mood_ok { get; set; }
        public double norm_mood { get; set; }
        public double norm_noise {  get; set; }

        //public string ratio_yes_c { get; set; }
        //public string ratio_no_c { get; set; }
        //public string chat_state { get; set; }
        //public string common_hub { get; set; }
        //public string chat_index { get; set; }

        public UNIT common_unit { get; set; }
        public HUB common_hub { get; set; }

        public async Task<string> GetAnswer()
        {
            chat_answer = "";

            int count = 0;
            while((chat_answer is null or "") && count++ < 60)
                await Task.Delay(1000);

            return count >= 59 ? ":COMEAGAIN" : chat_answer;
        }

        private string[] gimmick = { "[.??]", "[??.]" };
        private int count = 0;
        public void Set()
        {
            if(mind.z_current == "z_noise")
                return;

            if (mind.STATE == STATE.QUICKDECISION)
                return;

            if (!CONST.SAMPLE200.RandomSample(mind)) 
                return;

            if (count > 1)
                count = 0;

            cycles = $"{mind.cycles}";
            cycles_total = $"{mind.cycles_all}";
            p_curr = $"{mind.mech_current.p_curr.ToString("E3")}";
            d_curr = $"{mind.mech_current.d_curr.ToString("E3")}";

            user_var = $"{mind.user_var}";
            
            if (mind._mech == MECHANICS.HILL)
                position = $"{mind.mech_current.POS_XY}";
            if (mind._mech == MECHANICS.TUGOFWAR)
                position = $"{mind.mech_current.POS_XY}";
            if (mind._mech == MECHANICS.GRAVITY)
                position = $"{mind.pos.Pos}";
            
            ratio_yes_n = $"{mind.dir.Count(HARDDOWN.YES)}";
            ratio_no_n = $"{mind.dir.Count(HARDDOWN.NO)}";
            going_down = $"{(mind.dir.DownHard.IsNo() ? "NO" : "YES")}";
            epochs = $"{mind.epochs}";
            runtime = $"{CONST.RUNTIME}";

            occu = $"{mind._internal.Occu}";
            location = $"{mind._long.Result["location"]}";
            loc_state = mind._long.State["location"] > 0 ? "making a decision" : "just thinking";
            
            whistle = mind._quick.Result ? "[Whistling to my self..]" : gimmick[count];

            mood = mind.parms_current.pattern.ToString();
            mood_ok = mind.mood.ResColor == PATTERNCOLOR.GREEN;
            
            norm_mood = mind.mood.p_90;
            norm_noise = mind.mech_noise.p_90;

            common_unit = mind.core.most_common_unit;
            common_hub = mind.core.most_common_unit.HUB;

            if (mind._long.Result["answer"] != "") {
                chat_answer = $"{mind._long.Result["answer"]}";
                mind._long.Result["answer"] = "";
            }

            if(mind._long.Result["ask"] != "") {
                chat_subject = $"{mind._long.Result["ask"]}";
                mind._long.Result["ask"] = "";
            }
                        
            count++;
        }
    }
}
