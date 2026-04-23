using Awesome.AI.Core;
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

        public bool out_ready {  get; set; }

        public string ok {  get; set; }
        public string cycles { get; set; }
        public string cycles_total { get; set; }
        public string vv_curr { get; set; }
        public string dv_curr { get; set; }
        public string actual_us_x { get; set; }
        public string actual_us_y { get; set; }
        public string num_units { get; set; }
        public string avg_area { get; set; }
        public string avg_radius { get; set; }
        public string pain_truth_something { get; set; }
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
        public string math { get; set; }
        public string arc { get; set; }
        public string monologue_det_result { get; set; }
        public string monologue_det_subject { get; set; }
        public string monologue_det_relevance { get; set; }
        public string monologue_lat_result { get; set; }
        public string monologue_lat_subject { get; set; }
        public string monologue_lat_relevance { get; set; }
        public string mood_pattern {  get; set; }
        public string mood_green { get; set; }
        public string mood_norm { get; set; }
        public string noise_norm { get; set; }

        public string error { get; set; }
                
        public string common_hub_subject { get; set; }

        public void SetOut()
        {
            if (mind.STATE == STATE.QUICKDECISION)
                return;

            /*
             * LOW
             * */

            ok = $"{mind.ok}"; 
            cycles = $"{mind.cycles}";
            cycles_total = $"{mind.cycles_all}";
            error = $"{mind.down.Error}";
            go_down = $"{(mind.down.Dir > 0.0d ? "NO" : "YES")}";
            ratio_yes_n = $"{mind.down.Count(HARDDOWN.YES)}";
            ratio_no_n = $"{mind.down.Count(HARDDOWN.NO)}";
            vv_curr = $"{mind.mech.ms.vv_sym_curr.ToString("E3")}";
            dv_curr = $"{mind.mech.ms.dv_sym_curr.ToString("E3")}";
            noise_norm = $"{mind.mech.ms.vv_sym_90}";
            actual_us_x = $"{(mind.environment == ENV.LOCAL ? mind.unit_actual.UIget("will") : "-1")}";
            actual_us_y = $"{(mind.environment == ENV.LOCAL ? mind.unit_actual.UIget("attention") : "-1")}";

            int n_units = mind.access.UNITS_ALL().Count;
            double a_area = (100.0 * 100.0) / n_units;
            double a_radius = Math.Sqrt(a_area / Math.PI);

            num_units = $"{n_units}";
            avg_area = $"{a_area}";
            avg_radius = $"{a_radius}";

            /*
             * HIGH
             * */
        
            pain_truth_something = $"{mind.pain_truth_something}";
            position = $"{mind.mech.PosXY()}";
            epochs = $"{mind.epochs}";
            runtime = $"{mind.bot.RUNTIME}";

            whistle = $"{mind.result_whistle}";
            math = $"{mind.result_math}";
            arc = $"{mind.result_arc}";
            common_hub_subject = $"{(mind.hub.GetSubject(mind.unit_actual) ?? "")}";

            occu = $"{mind._internal.Occu.name}";
            location = $"{mind._long.Result[LONGTYPE.LOCATION]}";
            loc_state = $"{(mind._long.State[LONGTYPE.LOCATION] > 0 ? "making a decision" : "just thinking")}";

            mood_pattern = $"{mind.bot.pattern.ToString()}";
            mood_green = $"{mind.mood.res_color == PATTERNCOLOR.GREEN}";
            mood_norm = $"{mind.mood.p_90}";

            monologue_det_result = $"{mind.mono1.Result}";
            monologue_det_subject = $"{mind.mono1.Subject}";
            monologue_det_relevance = $"{mind.mono1.Relevance}";
            monologue_lat_result = $"{mind.mono2.Result}";
            monologue_lat_subject = $"{mind.mono2.Subject}";
            monologue_lat_relevance = $"{mind.mono2.Relevance}";

            string s_tmp1 = mind._long.GetResult(LONGTYPE.ANSWER);
            if (s_tmp1 != "") chat_answer = $"{s_tmp1}";

            string s_tmp2 = mind._long.GetResult(LONGTYPE.ASK);
            if (s_tmp2 != "") chat_subject = $"{s_tmp2}";

            out_ready = true;
        }
    }
}
