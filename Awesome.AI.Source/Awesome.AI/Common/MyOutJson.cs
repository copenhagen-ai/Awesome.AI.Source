using Awesome.AI.Core;
using System.Text.Json;

namespace Awesome.AI.Common
{
    public class JsonObject
    {
        public string ok { get; set; }
        public string cycles { get; set; }
        public string cycles_total { get; set; }
        public string vv_low_curr { get; set; }
        public string dv_high_curr { get; set; }

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
        public string monologue_det_result { get; set; }
        public string monologue_det_subject { get; set; }
        public string monologue_det_relevance { get; set; }
        public string monologue_lat_result { get; set; }
        public string monologue_lat_subject { get; set; }
        public string monologue_lat_relevance { get; set; }
        public string mood_pattern { get; set; }
        public string mood_green { get; set; }
        public string mood_norm { get; set; }
        public string noise_norm { get; set; }
        public string prop_mood { get; set; }

        public string error { get; set; }

        public string common_hub_subject { get; set; }        
    }

    public class MyOutJson
    {
        private TheMind mind;
        public MyOutJson() { }
        public MyOutJson(TheMind mind)
        {
            this.mind = mind;
        }
        
        //private int count = 0;

        private JsonObject obj { get; set; }
        private string json {  get; set; }
        
        private void SetObj()
        {
            obj = new JsonObject();

            obj.ok = mind.o_vars.ok;
            obj.cycles = mind.o_vars.cycles;
            obj.cycles_total = mind.o_vars.cycles_total;
            obj.vv_low_curr = mind.o_vars.vv_low_curr;
            obj.dv_high_curr = mind.o_vars.dv_high_curr;
            obj.pain_truth_something = mind.o_vars.pain_truth_something;
            obj.position = mind.o_vars.position;
            obj.ratio_yes_n = mind.o_vars.ratio_yes_n;
            obj.ratio_no_n = mind.o_vars.ratio_no_n;
            obj.go_down = mind.o_vars.go_down;
            obj.epochs = mind.o_vars.epochs;
            obj.runtime = mind.o_vars.runtime;
            obj.occu = mind.o_vars.occu;
            obj.location = mind.o_vars.location;
            obj.loc_state = mind.o_vars.loc_state;
            obj.chat_answer = mind.o_vars.chat_answer;
            obj.chat_subject = mind.o_vars.chat_subject;
            obj.whistle = mind.o_vars.whistle;
            obj.monologue_det_result = mind.o_vars.monologue_det_result;
            obj.monologue_det_subject = mind.o_vars.monologue_det_subject;
            obj.monologue_det_relevance = mind.o_vars.monologue_det_relevance;
            obj.monologue_lat_result = mind.o_vars.monologue_lat_result;
            obj.monologue_lat_subject = mind.o_vars.monologue_lat_subject;
            obj.monologue_lat_relevance = mind.o_vars.monologue_lat_relevance;
            obj.mood_pattern = mind.o_vars.mood_pattern;
            obj.mood_green = mind.o_vars.mood_green;
            obj.mood_norm = mind.o_vars.mood_norm;
            obj.noise_norm = mind.o_vars.noise_norm;
            obj.prop_mood = mind.o_vars.prop_mood;
            obj.error = mind.o_vars.error;
            obj.common_hub_subject = mind.o_vars.common_hub_subject;            
        }
        
        public string GetJson(bool _pro)
        {

            SetObj();

            json = JsonSerializer.Serialize(obj);

            return json;
        }
    }
}
