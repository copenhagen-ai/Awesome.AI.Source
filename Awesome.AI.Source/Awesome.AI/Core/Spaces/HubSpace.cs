using Awesome.AI.CoreInternals;
using Awesome.AI.Variables;
using System.Security;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core.Spaces
{
    public class HubSpace
    {
        /*
         * these are groups of UNITs
         * */

        public Dictionary<string, double> hubs_all { get; set; }

        private TheMind mind;
        public HubSpace(TheMind mind)
        {
            this.mind = mind;
            this.hubs_all = new Dictionary<string, double>();

            MINDS mindtype = mind.mindtype;
            Lookup lookup = new Lookup();
            string[] occus = lookup.occupasions;

            Random rand = new Random();
            foreach (string occ in occus)
            {
                List<string> hubs = lookup.GetHUBS(mindtype, occ);
                foreach (string hub in hubs)
                {
                    if (hubs_all.ContainsKey(hub))
                        continue;

                    double _r = rand.NextDouble();
                    hubs_all.Add(hub, _r);
                }
            }
        }

        public void AdjustWeights(string sub, double _val)
        {
            try
            {
                string occu = mind._internal.Occu.name;
                MINDS mindtype = mind.mindtype;

                Lookup lookup = new Lookup();
                List<string> hubs = lookup.GetHUBS(mindtype, occu);

                double _val1 = _val;
                double _val2 = _val * 0.1d;

                foreach (string hub in hubs) 
                    hubs_all[hub] -= _val2;

                foreach (string hub in hubs)
                    hubs_all[hub] = hubs_all[hub] < 0.0d ? 0.0d : hubs_all[hub]; 
                
                hubs_all[sub] += _val1;                
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public double GetIndex(string sub_get)
        {
            try
            {
                string occu = mind._internal.Occu.name;
                MINDS mindtype = mind.mindtype;

                Lookup lookup = new Lookup();
                List<string> hubs = lookup.GetHUBS(mindtype, occu);

                var hubs_curr = new Dictionary<string, double>();
                hubs.ForEach(x => hubs_curr.Add(x, hubs_all[x]));

                // no ordering, should be based on semantics
                var weights = hubs_curr.ToList();
                var sum = hubs_curr.Sum(x => x.Value);

                double res = 0.0d;
                double count = 0.0d;
                for (int i = 0; i < weights.Count(); i++)
                {
                    var weight = weights[i].Value;
                    var area = (weight / sum) * 100.0d;

                    count += area;

                    var sub = GetSubject(count);

                    if (sub_get == sub)
                        break;

                    var weight_next = weights[i + 1].Value;
                    var area_next = (weight_next / sum) * 100.0d;

                    res = count + area_next / 2;
                }

                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private bool LongDecision(UNIT unit, out string sub)
        {
            sub = "";
            if (unit.IsDECISION())
            {
                string data = unit.Data;

                // "SHOULD_A" "SHOULD_B" "SHOULD_C"
                if (data.StartsWith(CONST.lng_should))
                    sub = CONST.LSUB_SHOULD; 

                // "WHAT_KITCHEN" "WHAT_BEDROOM" "WHAT_LIVINGROOM" "WHAT_im busy right now.." "WHAT_not right now.." "WHAT_talk later.."                
                if (data.StartsWith(CONST.lng_what))
                    sub = CONST.LSUB_WHAT; 
            }

            return sub != "";
        }

        public string GetSubject(UNIT unit)
        {
            try
            {
                if (LongDecision(unit, out string sub))
                    return sub;

                string occu = mind._internal.Occu.name;
                MINDS mindtype = mind.mindtype;

                Lookup lookup = new Lookup();
                List<string> hubs = lookup.GetHUBS(mindtype, occu);

                var hubs_curr = new Dictionary<string, double>();
                hubs.ForEach(x => hubs_curr.Add(x, hubs_all[x]));

                // no ordering, should be based on semantics
                var weights = hubs_curr.ToList();
                var sum = weights.Sum(x => x.Value);

                double count = 0.0d;
                for (int i = 0; i < weights.Count(); i++)
                {
                    var weight = weights[i].Value;
                    var area = (weight / sum) * 100.0d;

                    count += area;

                    sub = weights[i].Key;

                    if (count >= unit.HubIndex)
                        break;
                }

                return sub;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string GetSubject(double hub_index)
        {
            try
            {
                string occu = mind._internal.Occu.name;
                MINDS mindtype = mind.mindtype;

                Lookup lookup = new Lookup();
                List<string> hubs = lookup.GetHUBS(mindtype, occu);

                var hubs_curr = new Dictionary<string, double>();
                hubs.ForEach(x => hubs_curr.Add(x, hubs_all[x]));

                // no ordering, should be based on semantics
                var weights = hubs_curr.ToList();
                var sum = weights.Sum(x => x.Value);

                string sub = "";
                double count = 0.0d;
                for (int i = 0; i < weights.Count(); i++)
                {
                    var weight = weights[i].Value;
                    var area = (weight / sum) * 100.0d;

                    count += area;

                    sub = weights[i].Key;
                 
                    if (count >= hub_index)
                        break;
                }

                return sub;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<UNIT> GetUnits()
        {
            try
            {
                string occu = mind._internal.Occu.name;
                
                var units = mind.access.UNITS_ALL();
                
                List<UNIT> res = new List<UNIT>();

                foreach (var unit in units)
                {
                    var max = Max(unit.register);
                    if (max == occu && !res.Contains(unit))
                        res.Add(unit);
                }            
            
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string Max(Dictionary<string, int> dict)
        {
            string res = "";
            int max = -1;

            foreach (var kvp in dict)
            {
                if (kvp.Value > max)
                {
                    max = kvp.Value;
                    res = kvp.Key;
                }
            }

            return res;
        }
    }
}
