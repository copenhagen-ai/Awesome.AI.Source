using Awesome.AI.Core;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Common
{
    public class MyOutVars
    {
        private TheMind mind;
        public MyOutVars() { }
        public MyOutVars(TheMind mind)
        {
            this.mind = mind;
        }

        public bool ok {  get; set; }
        public string cycles { get; set; }
        public string cycles_total { get; set; }
        public string vv_curr { get; set; }
        public string dv_curr { get; set; }
        public string actual_us_x { get; set; }
        public string actual_us_y { get; set; }
        public string num_units { get; set; }
        public string avg_area { get; set; }
        public string avg_radius { get; set; }

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
                
        public string common_hub_subject { get; set; }

        private string[] gimmick = { "[.??]", "[??.]" };
        private int count = 0;

        public void SetNoise()
        {
            if (mind.STATE == STATE.QUICKDECISION)
                return;

            if (count > 1)
                count = 0;

            go_down = $"{(mind.down.Dir > 0.0d ? "NO" : "YES")}";
            ratio_yes_n = $"{mind.down.Count(HARDDOWN.YES)}";
            ratio_no_n = $"{mind.down.Count(HARDDOWN.NO)}";
            error = mind.down.Error;
            vv_curr = $"{mind.mech_noise.ms.dv_sym_curr.ToString("E3")}";
            norm_noise = mind.mech_noise.ms.vv_sym_90;
            actual_us_x = ("" + mind.unit_actual.GetUI("will"));
            actual_us_y = ("" + mind.unit_actual.GetUI("attention"));

            int n_units = mind.access.UNITS_ALL().Count;
            double a_area = (100.0 * 100.0) / n_units;
            double a_radius = Math.Sqrt(a_area / Math.PI);

            num_units = "" + n_units;
            avg_area = "" + a_area;
            avg_radius = "" + a_radius;
        }

        public void SetMech()
        {
            if (count > 1)
                count = 0;

            ok = mind.ok;
            
            cycles = $"{mind.cycles}";
            
            cycles_total = $"{mind.cycles_all}";
            
            dv_curr = $"{mind.mech_high.ms.dv_sym_curr.ToString("E3")}";

            user_var = $"{mind.user_var}";
            
            position = $"{mind.mech_high.POS_XY}";
            
            epochs = $"{mind.epochs}";
            
            runtime = $"{CONST.RUNTIME}";

            whistle = mind._quick.Result ? "[Whistling to my self..]" : gimmick[count];

            norm_mood = mind.mood.p_90;

            common_hub_subject = mind.hub.GetSubject(mind.unit_actual) ?? "";

            occu = $"{mind._internal.Occu.name}";
            location = $"{mind._long.Result[LONGTYPE.LOCATION]}";
            loc_state = mind._long.State[LONGTYPE.LOCATION] > 0 ? "making a decision" : "just thinking";

            mood = mind.bot.pattern.ToString();
            mood_ok = mind.mood.ResColor == PATTERNCOLOR.GREEN;

            monologue_det_result = mind.mono1.Result;
            monologue_det_subject = mind.mono1.Subject;
            monologue_det_relevance = mind.mono1.Relevance;

            monologue_lat_result = mind.mono2.Result;
            monologue_lat_subject = mind.mono2.Subject;
            monologue_lat_relevance = mind.mono2.Relevance;

            string _base = 
                mind.mindtype == MINDS.ROBERTA ? "base" :
                mind.mindtype == MINDS.ANDREW ? "base" :
                "base";

            prop_mood = mind.calc.Normalize(mind.mech_high.mp.props.PropsOut[_base], -1.0d, 1.0d, 0.0d, 100.0d);

            if (mind._long.Result[LONGTYPE.ANSWER] != "")
            {
                chat_answer = $"{mind._long.Result[LONGTYPE.ANSWER]}";
                mind._long.Result[LONGTYPE.ANSWER] = "";
            }

            if (mind._long.Result[LONGTYPE.ASK] != "")
            {
                chat_subject = $"{mind._long.Result[LONGTYPE.ASK]}";
                mind._long.Result[LONGTYPE.ASK] = "";
            }

            count++;
        }
    }
}
