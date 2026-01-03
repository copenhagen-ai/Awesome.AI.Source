using Awesome.AI.Core;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Common
{
    public class Out
    {
        private TheMind mind;
        public Out() { }
        public Out(TheMind mind)
        {
            this.mind = mind;
        }

        public bool ok {  get; set; }
        public string cycles { get; set; }
        public string cycles_total { get; set; }
        public string vv_curr { get; set; }
        public string dv_curr { get; set; }

        public string user_var { get; set; }
        public string position { get; set; }
        public string ratio_yes_n { get; set; }
        public string ratio_no_n { get; set; }
        public string go_down { get; set; }
        public string epochs { get; set; }
        public string runtime { get; set; }
        public string occu { get; set; }
        public string location { get; set; }
        public string loc_state { get; set; }
        public string chat_answer { get; set; }
        public string chat_subject { get; set; }
        public string whistle { get; set; }
        public string monologue_det_result { get; set; }
        public string monologue_det_subject { get; set; }
        public string monologue_det_relevance { get; set; }
        public string monologue_lat_result { get; set; }
        public string monologue_lat_subject { get; set; }
        public string monologue_lat_relevance { get; set; }
        public string mood {  get; set; }
        public bool mood_ok { get; set; }
        public double norm_mood { get; set; }
        public double norm_noise { get; set; }
        public double prop_mood { get; set; }

        public int error { get; set; }
        public bool SetLow { get; set; }
        public bool SetHigh { get; set; }



        public string common_hub_subject { get; set; }

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
        public void SetNoise()
        {
            //if (mind.z_current == "z_noise")
            //    return;

            if (mind.STATE == STATE.QUICKDECISION)
                return;

            //if (!CONST.SAMPLE200.RandomSample(mind))
            //    return;

            if (count > 1)
                count = 0;

            //mind.down.Current = "noise";
            go_down = $"{(mind.down.Dir > 0.0d ? "NO" : "YES")}";
            ratio_yes_n = $"{mind.down.Count(HARDDOWN.YES)}";
            ratio_no_n = $"{mind.down.Count(HARDDOWN.NO)}";
            error = mind.down.Error;

            SetLow = true;
        }

        public void SetMech()
        {
            //if (mind.z_current == "z_noise")
            //    return;

            if (mind.STATE == STATE.QUICKDECISION)
                return;

            if (count > 1)
                count = 0;            

            ok = mind.ok;
            cycles = $"{mind.cycles}";
            cycles_total = $"{mind.cycles_all}";
            vv_curr = $"{mind.mech_current.ms.vv_sym_curr.ToString("E3")}";
            dv_curr = $"{mind.mech_current.ms.dv_sym_curr.ToString("E3")}";

            user_var = $"{mind.user_var}";

            if (mind._mech == MECHANICS.BALLONHILL_HIGH)
                position = $"{mind.mech_current.POS_XY}";
            if (mind._mech == MECHANICS.TUGOFWAR_HIGH)
                position = $"{mind.mech_current.POS_XY}";
            //if (mind._mech == MECHANICS.GRAVITY_HIGH)
            //    position = $"{mind.mech_current.POS_XY}";

            epochs = $"{mind.epochs}";
            runtime = $"{CONST.RUNTIME}";

            occu = $"{mind._internal.Occu}";
            location = $"{mind._long.Result["location"]}";
            loc_state = mind._long.State["location"] > 0 ? "making a decision" : "just thinking";

            whistle = mind._quick.Result ? "[Whistling to my self..]" : gimmick[count];

            mood = mind.parms_current.pattern.ToString();
            mood_ok = mind.mood.ResColor == PATTERNCOLOR.GREEN;

            monologue_det_result = mind.mono1.Result;
            monologue_det_subject = mind.mono1.Subject;
            monologue_det_relevance = mind.mono1.Relevance;

            monologue_lat_result = mind.mono2.Result;
            monologue_lat_subject = mind.mono2.Subject;
            monologue_lat_relevance = mind.mono2.Relevance;

            norm_mood = mind.mood.p_90;
            norm_noise = mind.mech_noise.ms.vv_sym_90;

            string _base = mind.mindtype == MINDS.ROBERTA ? "base" : "base";
            prop_mood = mind.calc.Normalize(mind.mech_high.mp.props.PropsOut[_base], -1.0d, 1.0d, 0.0d, 100.0d);
            //down_prop_noise = mind.calc.Normalize(mind.down.Props["noise"], -1.0d, 1.0d, 0.0d, 100.0d);

            //common_unit = mind.core.most_common_unit;
            common_hub_subject = mind?.unit_actual?.HUB?.subject ?? "";

            if (mind._long.Result["answer"] != "")
            {
                chat_answer = $"{mind._long.Result["answer"]}";
                mind._long.Result["answer"] = "";
            }

            if (mind._long.Result["ask"] != "")
            {
                chat_subject = $"{mind._long.Result["ask"]}";
                mind._long.Result["ask"] = "";
            }

            count++;

            SetHigh = true;
        }
    }
}
