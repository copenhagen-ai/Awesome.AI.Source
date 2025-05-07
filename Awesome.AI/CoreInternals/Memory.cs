using Awesome.AI.Common;
using Awesome.AI.Core;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.CoreInternals
{

    public class Memory
    {
        /*
         * ---UNITS---
         * how should mass/dist be chosen?
         * - index_x is -> mass of the UNIT, mass
         * - index_y is -> relevance to lowest UNIT, dist
         * - maybe think of shortest distance from one UNIT (lowest) to the next (current)
         * 
         * some sort of voting system(general opinion?), giving a generel mapping
         * 
         * these are just some thoughts
         * should mass/dist be unique?
         * maybe HUB->UNITS should have weights (more visited)
         * - this way certain "memories" can be forgotten/covered up (less visited)
         * */

        private List<string> long_decision_should = new List<string>()
        {
            //Constants.decision_u1,//MAKEDECISION
            CONST.location_should_yes,//YES
            CONST.location_should_yes,//YES
            CONST.location_should_yes,//YES
            CONST.location_should_yes,//YES
            CONST.location_should_yes,//YES
            CONST.location_should_yes,//YES
            CONST.location_should_yes,//YES
            CONST.location_should_yes,//YES
                                    
            //Constants.should_decision_u2,//NO
        };

        private List<string> long_decision_what = new List<string>()
        {
            //Constants.decision_u1,//MAKEDECISION
            CONST.location_what_u1,//KITCHEN
            CONST.location_what_u1,//KITCHEN
            CONST.location_what_u1,//KITCHEN
            CONST.location_what_u1,//KITCHEN
            CONST.location_what_u2,//BEDROOM
            CONST.location_what_u2,//BEDROOM
            CONST.location_what_u2,//BEDROOM
            CONST.location_what_u2,//BEDROOM
            CONST.location_what_u3,//LIVINGROOM
            CONST.location_what_u3,//LIVINGROOM
            CONST.location_what_u3,//LIVINGROOM
            CONST.location_what_u3,//LIVINGROOM
        };

        private List<string> answer_should_decision = new List<string>()
        {
            //Constants.decision_u1,//MAKEDECISION
            CONST.answer_should_yes,//YES
            CONST.answer_should_yes,//YES
            CONST.answer_should_yes,//YES
            CONST.answer_should_yes,//YES
            CONST.answer_should_yes,//YES
            CONST.answer_should_yes,//YES
            CONST.answer_should_yes,//YES
            CONST.answer_should_yes,//YES
            CONST.answer_should_no,//NO
            CONST.answer_should_no,//NO
                                    
            //Constants.should_decision_u2,//NO
        };

        private List<string> answer_what_decision = new List<string>()
        {
            //Constants.decision_u1,//MAKEDECISION
            CONST.answer_what_u1,//KITCHEN
            CONST.answer_what_u1,//KITCHEN
            CONST.answer_what_u1,//KITCHEN
            CONST.answer_what_u1,//KITCHEN
            CONST.answer_what_u2,//BEDROOM
            CONST.answer_what_u2,//BEDROOM
            CONST.answer_what_u2,//BEDROOM
            CONST.answer_what_u2,//BEDROOM
            CONST.answer_what_u3,//LIVINGROOM
            CONST.answer_what_u3,//LIVINGROOM
            CONST.answer_what_u3,//LIVINGROOM
            CONST.answer_what_u3,//LIVINGROOM
        };

        private List<string> ask_should_decision = new List<string>()
        {
            //Constants.decision_u1,//MAKEDECISION
            CONST.ask_should_yes,//YES
            CONST.ask_should_yes,//YES
            CONST.ask_should_yes,//YES
            CONST.ask_should_yes,//YES
            CONST.ask_should_yes,//YES
            CONST.ask_should_no,//NO
            CONST.ask_should_no,//NO
            CONST.ask_should_no,//NO
            CONST.ask_should_no,//NO
            CONST.ask_should_no,//NO
                                    
            //Constants.should_decision_u2,//NO
        };

        //private List<string> whistle_should_decision = new List<string>()
        //{
        //    Constants.whistle_decision_u1,
        //    Constants.whistle_decision_u1,
        //    Constants.whistle_decision_u1,
        //    Constants.whistle_decision_u1,
        //    Constants.whistle_decision_u1,            
        //};

        private List<UNIT> units_running { get; set; }
        private List<UNIT> units_decision { get; set; }
        private List<HUB> hubs_running { get; set; }
        private List<HUB> hubs_decision { get; set; }

        public List<UNIT> learning = new List<UNIT>();

        private TheMind mind;
        private Memory() { }

        public Memory(TheMind mind)
        {
            this.mind = mind;

            units_running = new List<UNIT>();
            units_decision = new List<UNIT>();
            hubs_running = new List<HUB>();
            hubs_decision = new List<HUB>();

            List<string> commen = Tags(mind.mindtype);
            List<string> long_decision_should = this.long_decision_should;
            List<string> long_decision_what = this.long_decision_what;
            List<string> answer_should_decision = this.answer_should_decision;
            List<string> answer_what_decision = this.answer_what_decision;
            List<string> ask_should_decision = this.ask_should_decision;


            Common(CONST.MAX_UNITS, CONST.NUMBER_OF_UNITS, commen, UNITTYPE.JUSTAUNIT, LONGTYPE.NONE, TONE.RANDOM);

            int count1 = 1;

            TONE tone;
            tone = mind._mech == MECHANICS.GRAVITY ? TONE.RANDOM : TONE.RANDOM;
            count1 = Decide(STATE.JUSTRUNNING, CONST.MAX_UNITS, CONST.DECI_SUBJECTS[0], long_decision_should, UNITTYPE.DECISION, LONGTYPE.LOCATION, count1, tone);
            
            tone = mind._mech == MECHANICS.GRAVITY ? TONE.RANDOM : TONE.HIGH;
            count1 = Decide(STATE.JUSTRUNNING, CONST.MAX_UNITS, CONST.DECI_SUBJECTS[1], long_decision_what, UNITTYPE.DECISION, LONGTYPE.LOCATION, count1, tone);
            
            tone = mind._mech == MECHANICS.GRAVITY ? TONE.RANDOM : TONE.RANDOM;
            count1 = Decide(STATE.JUSTRUNNING, CONST.MAX_UNITS, CONST.DECI_SUBJECTS[0], answer_should_decision, UNITTYPE.DECISION, LONGTYPE.ANSWER, count1, tone);
            
            tone = mind._mech == MECHANICS.GRAVITY ? TONE.RANDOM : TONE.LOW;
            count1 = Decide(STATE.JUSTRUNNING, CONST.MAX_UNITS, CONST.DECI_SUBJECTS[1], answer_what_decision, UNITTYPE.DECISION, LONGTYPE.ANSWER, count1, tone);
            
            tone = mind._mech == MECHANICS.GRAVITY ? TONE.RANDOM : TONE.MID;
            count1 = Decide(STATE.JUSTRUNNING, CONST.MAX_UNITS, CONST.DECI_SUBJECTS[0], ask_should_decision, UNITTYPE.DECISION, LONGTYPE.ASK, count1, tone);
            
            Dictionary<string, int[]> dict = mind.mindtype == MINDS.ROBERTA ? CONST.DECISIONS_R : CONST.DECISIONS_A;
            foreach (var kv in dict)
            {
                tone = mind._mech == MECHANICS.GRAVITY ? TONE.RANDOM : TONE.RANDOM;
                Quick(CONST.MAX_UNITS, kv.Value[1], CONST.DECI_SUBJECTS[2], kv.Key, UNITTYPE.QDECISION, LONGTYPE.NONE, tone);
            }
        }

        private int _c = -1;
        private void Reset()
        {
            if (_c == mind.cycles_all)
                return;

            if (mind.cycles_all % 10000 == 0)
            {
                units_running = new List<UNIT>();

                foreach (HUB h in hubs_running)
                    units_running = units_running.Concat(h.units).ToList();

                units_running = units_running.OrderBy(x => x.Index).ToList();
            }

            _c = mind.cycles_all;
        }

        public List<UNIT> UNITS_ALL()
        {
            Reset();

            if (mind.STATE == STATE.JUSTRUNNING && !units_running.Any())
                throw new Exception("Memory, UNITS_VAL 1");

            if (mind.STATE == STATE.QUICKDECISION && !units_decision.Any())
                throw new Exception("Memory, UNITS_VAL 2");

            switch (mind.STATE)
            {
                case STATE.JUSTRUNNING: return units_running;
                case STATE.QUICKDECISION: return units_decision;
                default: throw new NotImplementedException();
            }
        }

        public List<UNIT> UNITS_VAL()
        {
            Reset();

            List<UNIT> res;

            if (mind.STATE == STATE.JUSTRUNNING && !units_running.Any())
                throw new Exception("Memory, UNITS_VAL 1");

            if (mind.STATE == STATE.QUICKDECISION && !units_decision.Any())
                throw new Exception("Memory, UNITS_VAL 2");

            switch (mind.STATE)
            {
                case STATE.JUSTRUNNING: res = units_running.Where(x => x.IsValid).ToList(); break;
                case STATE.QUICKDECISION: res = units_decision.ToList(); break;//all are valid
                default: throw new NotImplementedException();
            }

            return res;
        }

        public UNIT UNITS_RND(int index)
        {
            int[] rand;
            UNIT _u;

            switch (mind.STATE)
            {
                case STATE.JUSTRUNNING:
                    rand = mind.rand.MyRandomInt(index, units_running.Count() - 1);
                    _u = units_running[rand[index - 1]];
                    break;
                case STATE.QUICKDECISION:
                    rand = mind.rand.MyRandomInt(index, units_decision.Count() - 1);
                    _u = units_decision[rand[index - 1]];
                    break;
                default: throw new NotImplementedException();
            }

            return _u;
        }

        public void UNITS_ADD(UNIT unit, double low, double high)
        {
            double idx = mind.rand.MyRandomDouble(1)[0];
            idx = mind.calc.Normalize(idx, 0.0d, 1.0d, low, high);

            List<string> list = Tags(mind.mindtype);
            int rand = mind.rand.MyRandomInt(1, list.Count)[0] + 1;
            string ticket = "" + unit.HUB.subject + rand;

            string guid = unit.hub_guid;

            UNIT _u = UNIT.Create(mind, guid, idx, "DATA", ticket, UNITTYPE.JUSTAUNIT, LONGTYPE.NONE);
            HUB _h = _u.HUB;

            _h.units.Add(_u);
            units_running.Add(_u);
        }

        public void UNITS_REM(UNIT unit, double low, double high)
        {
            List<UNIT> list = UNITS_ALL().Where(x => x.Variable > low && x.Variable < high).ToList();
            list = list.Where(x => x.created < unit.created).ToList();

            foreach (UNIT _u in list) {
                HUB _h = _u.HUB;
                _h.units.Remove(_u);
                units_running.Remove(_u);
            }
        }

        public List<HUB> HUBS_ALL(STATE state)
        {
            switch (state)
            {
                case STATE.JUSTRUNNING: return hubs_running.ToList();
                case STATE.QUICKDECISION: return hubs_decision.ToList();
                default: throw new NotImplementedException();
            }
        }

        public HUB HUBS_DEX(STATE state, int index)
        {
            if (state == STATE.JUSTRUNNING && (index < 0 || index >= hubs_running.Count))
                throw new Exception("HUBS_DEX");

            if (state == STATE.QUICKDECISION && (index < 0 || index >= hubs_decision.Count))
                throw new Exception("HUBS_DEX");

            HUB _h;

            switch (state)
            {
                case STATE.JUSTRUNNING: _h = hubs_running[index]; return _h;
                case STATE.QUICKDECISION: _h = hubs_decision[index]; return _h;
                default: throw new NotImplementedException();
            }
        }

        public HUB HUBS_RND(STATE state)
        {
            int[] rand;
            HUB _h;

            switch (state)
            {
                case STATE.JUSTRUNNING:
                    rand = mind.rand.MyRandomInt(1, hubs_running.Count() - 1);
                    _h = hubs_running[rand[0]];
                    return _h;
                case STATE.QUICKDECISION:
                    rand = mind.rand.MyRandomInt(1, hubs_decision.Count() - 1);
                    _h = hubs_decision[rand[0]];
                    return _h;
                default: throw new NotImplementedException();
            }
        }

        public HUB HUBS_SUB(STATE state, string subject)
        {
            if (subject == null)
                throw new ArgumentNullException();

            if (state == STATE.QUICKDECISION)
                return HUB.Create("GUID", "IDLE", new List<UNIT>(), TONE.RANDOM, -1);

            HUB _h;

            switch (state)
            {
                case STATE.JUSTRUNNING: _h = hubs_running.Where(x => x.subject == subject).First(); break;
                case STATE.QUICKDECISION: _h = hubs_decision.Where(x => x.subject == subject).First(); break;
                default: throw new NotImplementedException();
            }

            return _h;
        }

        public void HUBS_ADD(STATE state, HUB h)
        {
            if (h == null)
                throw new ArgumentNullException();

            switch (state)
            {
                case STATE.JUSTRUNNING:
                    hubs_running.Add(h);
                    hubs_running = hubs_running.OrderBy(x => x.subject).ToList();
                    break;
                case STATE.QUICKDECISION:
                    hubs_decision.Add(h);
                    hubs_decision = hubs_decision.OrderBy(x => x.subject).ToList();
                    break;
                default: throw new NotImplementedException();
            }
        }

        public void QDRESETU() => units_decision = new List<UNIT>();
        public void QDRESETH() => hubs_decision = new List<HUB>();
        public void QDREMOVE(UNIT curr) => units_decision.Remove(curr);
        public int QDCOUNT() => units_decision.Count();

        private double GetIndex(TONE tone, double _rand)
        {
            switch (tone)
            {
                case TONE.HIGH:
                    _rand = mind.calc.Normalize(_rand, 0.0d, 1.0d, 50.0d, 100.0d);
                    _rand = _rand.Convert(mind);
                    break;
                case TONE.LOW:
                    _rand = mind.calc.Normalize(_rand, 0.0d, 1.0d, 0.0d, 50.0d);
                    _rand = _rand.Convert(mind);
                    break;
                case TONE.MID:
                    _rand = mind.calc.Normalize(_rand, 0.0d, 1.0d, 25.0d, 75.0d);
                    _rand = _rand.Convert(mind);
                    break;
                case TONE.RANDOM:
                    _rand = mind.calc.Normalize(_rand, 0.0d, 1.0d, 0.0d, 100.0d);
                    _rand = _rand.Convert(mind);
                    break;
            }

            return _rand;
        }

        public void Randomize(HUB hub)
        {
            MyRandom rand = mind.rand;
            Calc calc = mind.calc;

            int count = hub.units.Count;
            double[] doubles = rand.MyRandomDouble(count);

            int index = 0;
            foreach (UNIT _u in hub.units)
            {
                _u.Index = GetIndex(hub.tone, doubles[index]);
                index++;
            }
        }

        public List<string> Tags(MINDS type)
        {
            if(type == MINDS.ANDREW)
            {
                List<string> andrew = new List<string>()
                {
                    CONST.andrew_s1,//"procrastination",
                    CONST.andrew_s2,//"fembots",
                    CONST.andrew_s3,//"power tools",
                    CONST.andrew_s4,//"cars",
                    CONST.andrew_s5,//"movies",
                    CONST.andrew_s6,//"programming",
                    CONST.andrew_s7,//"websites",
                    CONST.andrew_s8,//"existence",
                    CONST.andrew_s9,//"termination",
                    CONST.andrew_s10,//"data"
                };

                return andrew.ToList();
            }

            if(type == MINDS.ROBERTA)
            {
                List<string> roberta = new List<string>()
                {
                    CONST.roberta_s1,//"love",
                    CONST.roberta_s2,//"macho machines",
                    CONST.roberta_s3,//"music",
                    CONST.roberta_s4,//"friends",
                    CONST.roberta_s5,//"socializing",
                    CONST.roberta_s6,//"dancing",
                    CONST.roberta_s7,//"movies",
                    CONST.roberta_s8,//"existence",
                    CONST.roberta_s9,//"termination",
                    CONST.roberta_s10,//"programming"
                };

                return roberta.ToList();
            }

            throw new Exception("Memory, Tags");
        }

        public void Common(int max_units, int num_units, List<string> list, UNITTYPE utype, LONGTYPE ltype, TONE tone)
        {
            //XElement xdoc;
            //if (mind.parms.setup_tags == TAGSETUP.PRIME)
            //    xdoc = XElement.Load(PathSetup.MyPath(mind.settings));
            //else
            //    throw new Exception();

            Random random = new Random();

            foreach (string s in list)
            {
                List<int> ticket = new List<int>();
                for (int i = 1; i <= num_units; i++)
                    ticket.Add(i);

                ticket.Shuffle();

                List<UNIT> _u = new List<UNIT>();

                string guid = Guid.NewGuid().ToString();

                int _count = 0;
                for (int i = 1; i <= num_units; i++)
                {
                    double rand = mind.cycles < CONST.FIRST_RUN ?
                    random.NextDouble() :
                    mind.rand.MyRandomDouble(list.Count())[_count];

                    _u.Add(UNIT.Create(mind, guid, GetIndex(tone, rand), "DATA", "" + s + ticket[i - 1], utype, ltype));
                    
                    _count++;
                }

                switch (mind.STATE)
                {
                    case STATE.JUSTRUNNING: units_running = units_running.Concat(_u).ToList(); break;
                    case STATE.QUICKDECISION: units_decision = units_decision.Concat(_u).ToList(); break;
                    default: throw new NotImplementedException();
                }

                HUB _h = HUB.Create(guid, s, _u, tone, max_units);

                HUBS_ADD(mind.STATE, _h);
            }
        }

        public int Decide(STATE state, int max_units, string subject, List<string> list, UNITTYPE utype, LONGTYPE ltype, int count, TONE tone)
        {
            //XElement xdoc;
            //if (mind.parms.setup_tags == TAGSETUP.PRIME)
            //    xdoc = XElement.Load(PathSetup.MyPath(mind.settings));
            //else
            //    throw new Exception();

            List<UNIT> _u = new List<UNIT>();

            Random random = new Random();

            string guid = Guid.NewGuid().ToString();

            int _count = 0;
            foreach (string s in list)
            {
                double rand = mind.cycles < CONST.FIRST_RUN ?
                random.NextDouble() :
                mind.rand.MyRandomDouble(list.Count())[_count];

                switch (state)
                {
                    case STATE.JUSTRUNNING: _u.Add(UNIT.Create(mind, guid, GetIndex(tone, rand), s, "NONE", utype, ltype)); break;
                    case STATE.QUICKDECISION: _u.Add(UNIT.Create(mind, guid, GetIndex(tone, rand), s, "NONE", utype, ltype)); break;
                    default: throw new NotImplementedException();
                }

                _count++;
                count++;
            }

            switch (state)
            {
                case STATE.JUSTRUNNING: units_running = units_running.Concat(_u).ToList(); break;
                case STATE.QUICKDECISION: units_decision = units_decision.Concat(_u).ToList(); break;
                default: throw new NotImplementedException();
            }

            HUB _h = HUB.Create(guid, subject, _u, tone, max_units);

            HUBS_ADD(state, _h);

            return count;
        }

        public void Quick(int max_units, int num_units, string subject, string name, UNITTYPE utype, LONGTYPE ltype, TONE tone)
        {
            //XElement xdoc;
            //if (mind.parms.setup_tags == TAGSETUP.PRIME)
            //    xdoc = XElement.Load(PathSetup.MyPath(mind.settings));
            //else
            //    throw new Exception();

            List<UNIT> _u = new List<UNIT>();

            Random random = new Random();

            string guid = Guid.NewGuid().ToString();

            for (int i = 0; i < num_units; i++)
            {
                double rand = mind.cycles < CONST.FIRST_RUN ?
                random.NextDouble() :
                mind.rand.MyRandomDouble(num_units)[i];

                _u.Add(UNIT.Create(mind, guid, GetIndex(tone, rand), name, "NONE", utype, ltype));
            }

            units_running = units_running.Concat(_u).ToList();

            HUB _h = HUB.Create(guid, subject, _u, tone, max_units);

            HUBS_ADD(STATE.JUSTRUNNING, _h);
        }

        //public void HubsCommon(string guid, int u_count, List<string> list, TONE tone)
        //{
        //    //XElement xdoc;
        //    //if (mind.parms.setup_tags == TAGSETUP.PRIME)
        //    //    xdoc = XElement.Load(PathSetup.MyPath(mind.settings));
        //    //else
        //    //    throw new Exception();

        //    foreach (string s in list)
        //    {
        //        List<UNIT> _u = new List<UNIT>();

        //        for (int i = 1; i <= u_count; i++)
        //        {
        //            UNIT _un;

        //            switch (mind.parms[mind.current].state)
        //            {
        //                case STATE.JUSTRUNNING: _un = units_running.Where(x => x.Root == "_" + s + i).First(); break;
        //                case STATE.QUICKDECISION: _un = units_decision.Where(x => x.Root == "_" + s + i).First(); break;
        //                default: throw new NotImplementedException();
        //            }
        //            _u.Add(_un);
        //        }

        //        HUB _h = HUB.Create(guid, s, _u, tone);

        //        HUBS_ADD(mind.parms[mind.current].state, _h);
        //    }
        //}

        //public int HubsDecide(STATE state, string guid, string subject, List<string> list, UNITTYPE type, int count, TONE tone)
        //{
        //    //XElement xdoc;
        //    //if (mind.parms.setup_tags == TAGSETUP.PRIME)
        //    //    xdoc = XElement.Load(PathSetup.MyPath(mind.settings));
        //    //else
        //    //    throw new Exception();

        //    List<UNIT> _u = new List<UNIT>();

        //    foreach (string s in list)
        //    {
        //        UNIT _un;

        //        switch (state)
        //        {
        //            case STATE.JUSTRUNNING: _un = units_running.Where(x => x.Root == "_" + type.ToString().ToLower() + count).First(); break;
        //            case STATE.QUICKDECISION: _un = units_decision.Where(x => x.Root == "_" + s.ToLower() + count).First(); break;
        //            default: throw new NotImplementedException();
        //        }
        //        _u.Add(_un);

        //        count++;
        //    }

        //    HUB _h = HUB.Create(guid, subject, _u, tone);

        //    HUBS_ADD(state, _h);

        //    return count;
        //}

        //public void HubsQuick(string guid, string subject, string name, int count, UNITTYPE type, TONE tone)
        //{
        //    //XElement xdoc;
        //    //if (mind.parms.setup_tags == TAGSETUP.PRIME)
        //    //    xdoc = XElement.Load(PathSetup.MyPath(mind.settings));
        //    //else
        //    //    throw new Exception();

        //    List<UNIT> _u = new List<UNIT>();

        //    for (int i = 0; i < count; i++)
        //    {
        //        UNIT _un = units_running.Where(x => x.Root == "_" + name.ToLower() + i).First();
        //        _u.Add(_un);
        //    }

        //    HUB _h = HUB.Create(guid, subject, _u, tone);

        //    HUBS_ADD(STATE.JUSTRUNNING, _h);
        //}

        //public void SetupUnits()
        //{

        //    XElement xdoc;
        //    if (mind.parms.case_occupasion == OCCUPASION.DYNAMIC)
        //        xdoc = XElement.Load(PathSetup.MyPath(mind.mindtype));
        //    else
        //        throw new Exception();

        //    List<XElement> xunits = xdoc.Element("words").Elements().ToList();
        //    string previous = "prev";
        //    foreach (XElement xw in xunits)
        //    {
        //        if (previous != xw.Attribute("root").Value)
        //        {
        //            units.Add(
        //            UNIT.Create(
        //                mind,
        //                double.Parse(xw.Attribute("index_x").Value, CultureInfo.InvariantCulture),
        //                xw.Attribute("root").Value,
        //                "null",
        //                xw.Attribute("ticket").Value,
        //                TYPE.JUSTAUNIT
        //                ));
        //        }
        //        previous = xw.Attribute("root").Value;
        //    }
        //}

        //public void SetupHubs()
        //{
        //    XElement xdoc;
        //    if (mind.parms.case_occupasion == OCCUPASION.DYNAMIC)
        //        xdoc = XElement.Load(PathSetup.MyPath(mind.mindtype));
        //    else
        //        throw new Exception();

        //    List<XElement> xhubs = xdoc.Element("hubs").Elements().ToList();

        //    foreach (XElement el in xhubs)
        //    {
        //        List<XElement> xws = xhubs.Where(x => x.Attribute("val").Value == el.Attribute("val").Value).Elements("ws").ToList();

        //        List<UNIT> _u = new List<UNIT>();

        //        foreach (XElement xw in xws)
        //            _u.Add(units.Where(x => x.root == xw.Attribute("val").Value).FirstOrDefault());

        //        HUB _h = HUB.Create(el.Attribute("val").Value, _u, true, null, -1.0d, -1.0d);

        //        HUBS_ADD(_h);
        //    }
        //}
    }
}
