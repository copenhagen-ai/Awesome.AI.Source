using Awesome.AI.Common;
using Awesome.AI.Core;
using Awesome.AI.Helpers;

namespace Awesome.AI.CoreHelpers
{
    public class Process
    {
        public UNIT most_common_unit;

        private List<UNIT> u_history = new List<UNIT>();
        private List<string> remember = new List<string>();
        private Dictionary<string, int> hits = new Dictionary<string, int>();

        private TheMind mind;
        private Process() { }
        public Process(TheMind mind)
        {
            this.mind = mind;
        }

        public void History()
        {
            if (mind.curr_unit.IsIDLE())
                return;

            if (!u_history.Any())
            {
                u_history.Add(mind.mem.UNIT_RND(1));
                u_history.Add(mind.mem.UNIT_RND(2));
                u_history.Add(mind.mem.UNIT_RND(3));
            }

            u_history.Insert(0, mind.curr_unit);
            if (u_history.Count > Constants.HIST_TOTAL)
                u_history.RemoveAt(u_history.Count - 1);
        }

        public void Common()
        {
            if (u_history.IsNullOrEmpty())
                return;

            most_common_unit = u_history
                .GroupBy(x => x)
                .OrderByDescending(x => x.Count())
                .Select(x => x.Key)
                .First();
        }

        public void Stats(bool _pro)
        {
            if (!_pro)
                return;

            if (most_common_unit.IsNull())
                return;

            List<UNIT> units = mind.mem.UNITS_ALL();
            Stats stats = new Stats();
            
            foreach (UNIT u in units)
                stats.list.Add(new Stat() { _name = u.root, _var = u.Variable, _index = u.Index });
            

            string nam = most_common_unit.root;
            Stat _s_curr = stats.list.Where(x=>x._name == nam).First();
            
            if (!hits.ContainsKey(nam))
                hits.Add(nam, 0);

            hits[nam] += 1;

            //mind.stats.list = mind.stats.list.OrderBy(x => x._var).ToList();
            stats.curr_name = nam;
            //stats.curr_value = hits[nam];
            _s_curr.hits = hits[nam];

            remember.Insert(0, nam);
            if (remember.Count > Constants.REMEMBER)
            {
                string name = remember.Last();
                Stat _s_reset = stats.list.Where(x => x._name == name).First();

                hits[name] -= 1;

                stats.reset_name = name;
                //stats.reset_value = hits[name];
                _s_reset.hits = hits[name];

                remember.RemoveAt(remember.Count - 1);
            }
                        
            mind.stats = stats;
        }

        //public void AddHistoryHUB(HUB h)
        //{
        //    if (h_history == null)
        //        throw new Exception();
            
        //    if (h == null)
        //        throw new Exception();

        //    h_history.Insert(0, h);
        //    if (h_history.Count > mind.parms.hist_total)
        //        h_history.RemoveAt(h_history.Count - 1);
        //}

        //public void AddHistoryUnít(UNIT w)
        //{
        //    if (u_history == null)
        //        throw new Exception();
            
        //    if (w == null)
        //        throw new Exception();

        //    u_history.Insert(0, w);
        //    if (u_history.Count > mind.parms.hist_total)
        //        u_history.RemoveAt(u_history.Count - 1);
        //}

        //public void CommonHubs(TheMind mind)
        //{
        //    if (mind == null)
        //        throw new Exception();

        //    for (int i = 0; i < 3; i++)
        //        most_common_hubs[i] = null;

        //    List<HUB> hubs = mind.mem.HUBS_ALL().OrderByDescending(x => x.percent).Take(3).ToList();
            
        //    most_common_hubs[0] = hubs[0];
        //    if (hubs[0].percent != hubs[1].percent) return;
        //    most_common_hubs[1] = hubs[1];
        //    if (hubs[1].percent != hubs[2].percent) return;
        //    most_common_hubs[2] = hubs[2];
        //}

        //public string ProcessCommonHubs(TheMind mind)
        //{
        //    if (mind == null)
        //        throw new ArgumentNullException();

        //    if (most_common_hubs == null)
        //        return "Ohm, Sweet Ohm";

        //    string rt = "";
        //    int counter = 0;
        //    foreach (HUB h in most_common_hubs)
        //    {
        //        if (h == null)
        //            continue;

        //        if (counter > 0)
        //            rt += " and ";

        //        string subject = "HUB:" + h.GetSubject();
        //        rt += subject;

        //        counter++;
        //    }

        //    return rt;
        //}

        //public void Stream()
        //{
        //    if (!stream.Any())
        //    {
        //        stream.Add(UNIT.Create(mind, 0.0d, "Old McDonald..", "null", "", TYPE.JUSTAUNIT/*, "none", "none"*/));
        //        stream.Add(UNIT.Create(mind, 0.0d, "Old McDonald..", "null", "", TYPE.JUSTAUNIT/*, "none", "none"*/));
        //        stream.Add(UNIT.Create(mind, 0.0d, "Old McDonald..", "null", "", TYPE.JUSTAUNIT/*, "none", "none"*/));                
        //    }

        //    if (mind == null)
        //        throw new ArgumentNullException();

        //    if (most_common_unit == null)
        //        return;

        //    UNIT c = most_common_unit;
        //    HUB[] r = most_common_hubs;

        //    if (r.Where(x => x != null && x.GetSubject() == c.HUB.GetSubject()).ToList().Count > 0)
        //    {
        //        mind.correct_thinking++;

        //        if (stream[0].root != c.root)
        //            stream.Insert(0, c);
        //        if (stream.Count > 3)
        //            stream.Remove(stream.LastOrDefault());
        //    }
        //    else
        //        mind.not_correct_thinking++;
        //}

        //public string ProcessStream(int index)
        //{
        //    if (!stream.Any())
        //    {
        //        stream.Add(UNIT.Create(mind, 0.0d, "Old McDonald..", "null", "", TYPE.JUSTAUNIT/*, "none", "none"*/));
        //        stream.Add(UNIT.Create(mind, 0.0d, "Old McDonald..", "null", "", TYPE.JUSTAUNIT/*, "none", "none"*/));
        //        stream.Add(UNIT.Create(mind, 0.0d, "Old McDonald..", "null", "", TYPE.JUSTAUNIT/*, "none", "none"*/));                
        //    }

        //    if (index >= stream.Count)
        //        return "Doh!";

        //    return stream[index].HUB.GetSubject() + " and " + stream[index].root;
        //}

        //public UNIT StreamTop()
        //{
        //    /*
        //     * stream contains most common UNITs
        //     */

        //    if (!stream.Any())
        //    {
        //        stream.Add(UNIT.Create(mind, 0.0d, "Old McDonald..", "null", "", TYPE.JUSTAUNIT/*, "none", "none"*/));
        //        stream.Add(UNIT.Create(mind, 0.0d, "Old McDonald..", "null", "", TYPE.JUSTAUNIT/*, "none", "none"*/));
        //        stream.Add(UNIT.Create(mind, 0.0d, "Old McDonald..", "null", "", TYPE.JUSTAUNIT/*, "none", "none"*/));                
        //    }

        //    if (stream == null || !stream.Any())
        //        return null;

        //    return stream[0];
        //}

        //public void SetPercent(bool _pro)
        //{
        //    if (!_pro)
        //        return;
            
        //    foreach (HUB h in mind.mem.HUBS_ALL().ToList())
        //        h.percent = (int)(h_history.Where(x => x.GetSubject() == h.GetSubject()).Count() / (double)((double)mind.parms.hist_total * 0.01d));
        //}

        /*
         * NB: EXPERIMENTAL
         * */
        //public static void PersueTopic(TheMind mind, HUB curr_hub)
        //{
        //    if (mind == null)
        //        throw new ArgumentNullException();

        //    if (curr_hub == null)
        //        throw new ArgumentNullException();

        //    string state = "";
        //    if (mind.states["ask2"] == "persue")
        //        state = "ask2";
        //    if (mind.states["ask3"] == "persue")
        //        state = "ask3";
        //    if (mind.states["present1"] == "persue")
        //        state = "present1";
        //    if (mind.states["play"] == "persue")
        //        state = "play";

        //    if (state == "")
        //        return;

        //    if (mind.topic == "")
        //        return;
        //    if (mind.ask_this_question == "" && mind.ask_this_board == null)
        //        return;
        //    if (curr_hub.GetSubject() != mind.topic)
        //        return;

        //    string subject = curr_hub.GetSubject();
        //    if (subject == mind.topic)
        //    {
        //        HUB _h = Memory.HUBS_SUB(mind.topic);
        //        Goal g = mind.goal_sys.GetGoal(mind.topic);

        //        InputLearn _in = null;

        //        if (g.tru.combi_type == "A_STRING")
        //            _in = InputLearn.Create("STRING", mind.ask_this_question);
        //        if (g.tru.combi_type == "A_UNIT")
        //            _in = InputLearn.Create("STRING", mind.ask_this_question);
        //        if (g.tru.combi_type == "L_D_STRING")
        //            _in = InputLearn.Create("D_STRING", mind.ask_this_board);

        //        mind._answer = g.Result(_h, _in);
        //        mind.states[state] = "answer";
        //    }
        //}
        
    }
}
