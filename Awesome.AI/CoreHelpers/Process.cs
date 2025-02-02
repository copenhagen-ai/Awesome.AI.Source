﻿using Awesome.AI.Common;
using Awesome.AI.Core;

namespace Awesome.AI.CoreHelpers
{
    public class Process
    {
        //public List<HUB> h_history = new List<HUB>();
        //public HUB[] most_common_hubs = new HUB[3];// this is statistics
        //public List<UNIT> stream = new List<UNIT>();// this is the upfront stream

        public List<UNIT> u_history = new List<UNIT>();
        public UNIT most_common_unit = null;// this is statistics

        private TheMind mind;
        private Process() { }
        public Process(TheMind mind)
        {
            this.mind = mind;
        }

        public void History(TheMind mind)
        {
            if (mind == null)
                throw new ArgumentNullException();

            //if (mind.curr_unit.IsLEARNINGorPERSUE())
            //    return;

            if (mind.curr_unit.IsIDLE())
                return;

            //if (mind.curr_unit.IsDECISION())
            //    return;

            //AddHistoryHUB(mind.curr_unit.HUB);
            //AddHistoryUnít(mind.curr_unit);

            u_history.Insert(0, mind.curr_unit);
            if (u_history.Count > mind.parms.hist_total)
                u_history.RemoveAt(u_history.Count - 1);
        }

        public void CommonUnit(TheMind mind)
        {
            if (mind == null)
                throw new Exception();

            if (u_history == null)
                throw new Exception();

            most_common_unit = u_history.GroupBy(x => x).OrderByDescending(x => x.Count()).Select(x => x.Key).FirstOrDefault();
        }

        private List<string> remember = new List<string>();
        private Dictionary<string, int> hits = new Dictionary<string, int>();
        public void Stats(TheMind mind, bool _pro)
        {
            if (mind.IsNull())
                throw new Exception();

            if (most_common_unit.IsNull())
                return;

            if (!_pro)
                return;

            Stats stats = new Stats();
            List<UNIT> units = mind.mem.UNITS_ALL();

            foreach (UNIT u in units)
                stats.list.Add(new Stat() { name = u.root, force = u.Variable, index = u.Index });

            mind.stats = stats;

            string nam = most_common_unit.root;
            
            if (!hits.ContainsKey(nam))
                hits.Add(nam, 0);

            hits[nam] += 1;

            mind.stats.list = mind.stats.list.OrderBy(x => x.force).ToList();
            mind.stats.curr_name = nam;
            mind.stats.curr_value = hits[nam];

            remember.Insert(0, nam);
            if (remember.Count > mind.parms.remember)
                remember.RemoveAt(remember.Count - 1);

            int test2 = hits.Sum(x => x.Value);

            if (remember.Count >= mind.parms.remember)
            {
                foreach (Stat _s in mind.stats.list)
                {
                    if (_s.name == remember.LastOrDefault())
                    {
                        hits[_s.name] -= 1;

                        mind.stats.reset_name = _s.name;
                        mind.stats.reset_value = hits[_s.name];
                    }
                }
            }
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
