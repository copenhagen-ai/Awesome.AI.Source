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

        private List<string> andrew = new List<string>()
        {
            Constants.andrew_s1,//"procrastination",
            Constants.andrew_s2,//"fembots",
            Constants.andrew_s3,//"power tools",
            Constants.andrew_s4,//"cars",
            Constants.andrew_s5,//"movies",
            Constants.andrew_s6,//"programming",
            Constants.andrew_s7,//"websites",
            Constants.andrew_s8,//"existence",
            Constants.andrew_s9,//"termination",
            Constants.andrew_s10//"data"
        };

        private List<string> roberta = new List<string>()
        {
            Constants.roberta_s1,//"love",
            Constants.roberta_s2,//"macho machines",
            Constants.roberta_s3,//"music",
            Constants.roberta_s4,//"friends",
            Constants.roberta_s5,//"socializing",
            Constants.roberta_s6,//"dancing",
            Constants.roberta_s7,//"movies",
            Constants.roberta_s8,//"existence",
            Constants.roberta_s9,//"termination",
            Constants.roberta_s10//"programming"
        };

        private List<string> long_decision_should = new List<string>()
        {
            //Constants.decision_u1,//MAKEDECISION
            Constants.location_should_yes,//YES
            Constants.location_should_yes,//YES
            Constants.location_should_yes,//YES
            Constants.location_should_yes,//YES
            Constants.location_should_yes,//YES
            Constants.location_should_yes,//YES
            Constants.location_should_yes,//YES
            Constants.location_should_yes,//YES
                                    
            //Constants.should_decision_u2,//NO
        };

        private List<string> long_decision_what = new List<string>()
        {
            //Constants.decision_u1,//MAKEDECISION
            Constants.location_what_u1,//KITCHEN
            Constants.location_what_u1,//KITCHEN
            Constants.location_what_u1,//KITCHEN
            Constants.location_what_u1,//KITCHEN
            Constants.location_what_u2,//BEDROOM
            Constants.location_what_u2,//BEDROOM
            Constants.location_what_u2,//BEDROOM
            Constants.location_what_u2,//BEDROOM
            Constants.location_what_u3,//LIVINGROOM
            Constants.location_what_u3,//LIVINGROOM
            Constants.location_what_u3,//LIVINGROOM
            Constants.location_what_u3,//LIVINGROOM
        };

        private List<string> answer_should_decision = new List<string>()
        {
            //Constants.decision_u1,//MAKEDECISION
            Constants.answer_should_yes,//YES
            Constants.answer_should_yes,//YES
            Constants.answer_should_yes,//YES
            Constants.answer_should_yes,//YES
            Constants.answer_should_yes,//YES
            Constants.answer_should_yes,//YES
            Constants.answer_should_yes,//YES
            Constants.answer_should_yes,//YES
            Constants.answer_should_no,//NO
            Constants.answer_should_no,//NO
                                    
            //Constants.should_decision_u2,//NO
        };

        private List<string> answer_what_decision = new List<string>()
        {
            //Constants.decision_u1,//MAKEDECISION
            Constants.answer_what_u1,//KITCHEN
            Constants.answer_what_u1,//KITCHEN
            Constants.answer_what_u1,//KITCHEN
            Constants.answer_what_u1,//KITCHEN
            Constants.answer_what_u2,//BEDROOM
            Constants.answer_what_u2,//BEDROOM
            Constants.answer_what_u2,//BEDROOM
            Constants.answer_what_u2,//BEDROOM
            Constants.answer_what_u3,//LIVINGROOM
            Constants.answer_what_u3,//LIVINGROOM
            Constants.answer_what_u3,//LIVINGROOM
            Constants.answer_what_u3,//LIVINGROOM
        };

        private List<string> ask_should_decision = new List<string>()
        {
            //Constants.decision_u1,//MAKEDECISION
            Constants.ask_should_yes,//YES
            Constants.ask_should_yes,//YES
            Constants.ask_should_yes,//YES
            Constants.ask_should_yes,//YES
            Constants.ask_should_yes,//YES
            Constants.ask_should_no,//NO
            Constants.ask_should_no,//NO
            Constants.ask_should_no,//NO
            Constants.ask_should_no,//NO
            Constants.ask_should_no,//NO
                                    
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

        public Memory(TheMind mind, int u_count)
        {
            this.mind = mind;

            units_running = new List<UNIT>();
            units_decision = new List<UNIT>();
            hubs_running = new List<HUB>();
            hubs_decision = new List<HUB>();

            List<string> commen = mind.mindtype == MINDS.ROBERTA ? roberta : andrew;
            List<string> long_decision_should = this.long_decision_should;
            List<string> long_decision_what = this.long_decision_what;
            List<string> answer_should_decision = this.answer_should_decision;
            List<string> answer_what_decision = this.answer_what_decision;
            List<string> ask_should_decision = this.ask_should_decision;

            UnitsCommon(u_count, commen, UNITTYPE.JUSTAUNIT, LONGTYPE.NONE, TONE.RANDOM);
            HubsCommon(u_count, commen, TONE.RANDOM);

            int count1 = 1;
            int count2 = 1;

            TONE tone;
            tone = mind._mech == MECHANICS.GRAVITY ? TONE.RANDOM : TONE.RANDOM;
            count1 = UnitsDecide(STATE.JUSTRUNNING, long_decision_should, UNITTYPE.DECISION, LONGTYPE.LOCATION, count1, tone);
            count2 = HubsDecide(STATE.JUSTRUNNING, Constants.deci_subject[0], long_decision_should, UNITTYPE.DECISION, count2, tone);

            tone = mind._mech == MECHANICS.GRAVITY ? TONE.RANDOM : TONE.HIGH;
            count1 = UnitsDecide(STATE.JUSTRUNNING, long_decision_what, UNITTYPE.DECISION, LONGTYPE.LOCATION, count1, tone);
            count2 = HubsDecide(STATE.JUSTRUNNING, Constants.deci_subject[1], long_decision_what, UNITTYPE.DECISION, count2, tone);

            tone = mind._mech == MECHANICS.GRAVITY ? TONE.RANDOM : TONE.RANDOM;
            count1 = UnitsDecide(STATE.JUSTRUNNING, answer_should_decision, UNITTYPE.DECISION, LONGTYPE.ANSWER, count1, tone);
            count2 = HubsDecide(STATE.JUSTRUNNING, Constants.deci_subject[0], answer_should_decision, UNITTYPE.DECISION, count2, tone);

            tone = mind._mech == MECHANICS.GRAVITY ? TONE.RANDOM : TONE.LOW;
            count1 = UnitsDecide(STATE.JUSTRUNNING, answer_what_decision, UNITTYPE.DECISION, LONGTYPE.ANSWER, count1, tone);
            count2 = HubsDecide(STATE.JUSTRUNNING, Constants.deci_subject[1], answer_what_decision, UNITTYPE.DECISION, count2, tone);

            tone = mind._mech == MECHANICS.GRAVITY ? TONE.RANDOM : TONE.MID;
            count1 = UnitsDecide(STATE.JUSTRUNNING, ask_should_decision, UNITTYPE.DECISION, LONGTYPE.ASK, count1, tone);
            count2 = HubsDecide(STATE.JUSTRUNNING, Constants.deci_subject[0], ask_should_decision, UNITTYPE.DECISION, count2, tone);

            Dictionary<string, int[]> dict = mind.mindtype == MINDS.ROBERTA ? Constants.DECISIONS_R : Constants.DECISIONS_A;
            foreach (var kv in dict)
            {
                tone = mind._mech == MECHANICS.GRAVITY ? TONE.RANDOM : TONE.RANDOM;
                UnitsQuick(kv.Key, kv.Value[1], UNITTYPE.QDECISION, LONGTYPE.NONE, tone);
                HubsQuick(Constants.deci_subject[2], kv.Key, 5, UNITTYPE.QDECISION, tone);
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

            switch (mind.parms[mind.current].state)
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

            switch (mind.parms[mind.current].state)
            {
                case STATE.JUSTRUNNING: res = units_running.Where(x => x.IsValid).ToList(); break;
                case STATE.QUICKDECISION: res = units_decision.ToList(); break;//all are valid
                default: throw new NotImplementedException();
            }

            return res;
        }

        public UNIT UNIT_RND(int index)
        {
            int[] rand;
            UNIT _u;

            switch (mind.parms[mind.current].state)
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
                return HUB.Create("IDLE", new List<UNIT>(), TONE.RANDOM);

            HUB _h;

            switch (state)
            {
                case STATE.JUSTRUNNING: _h = hubs_running.Where(x => x.GetSubject() == subject).First(); break;
                case STATE.QUICKDECISION: _h = hubs_decision.Where(x => x.GetSubject() == subject).First(); break;
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
                    hubs_running = hubs_running.OrderBy(x => x.GetSubject()).ToList();
                    break;
                case STATE.QUICKDECISION:
                    hubs_decision.Add(h);
                    hubs_decision = hubs_decision.OrderBy(x => x.GetSubject()).ToList();
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
                    _rand = mind.calc.NormalizeRange(_rand, 0.0d, 1.0d, 50.0d, 100.0d);
                    _rand = _rand.Convert(mind);
                    break;
                case TONE.LOW:
                    _rand = mind.calc.NormalizeRange(_rand, 0.0d, 1.0d, 0.0d, 50.0d);
                    _rand = _rand.Convert(mind);
                    break;
                case TONE.MID:
                    _rand = mind.calc.NormalizeRange(_rand, 0.0d, 1.0d, 25.0d, 75.0d);
                    _rand = _rand.Convert(mind);
                    break;
                case TONE.RANDOM:
                    _rand = mind.calc.NormalizeRange(_rand, 0.0d, 1.0d, 0.0d, 100.0d);
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

        public void UnitsCommon(int u_count, List<string> list, UNITTYPE utype, LONGTYPE ltype, TONE tone)
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
                for (int i = 1; i <= u_count; i++)
                    ticket.Add(i);

                ticket.Shuffle();

                int _count = 0;
                for (int i = 1; i <= u_count; i++)
                {
                    double rand = mind.cycles < Constants.FIRST_RUN ?
                    random.NextDouble() :
                    mind.rand.MyRandomDouble(list.Count())[_count];

                    switch (mind.parms[mind.current].state)
                    {
                        case STATE.JUSTRUNNING:
                            units_running.Add(UNIT.Create(mind,/*value*/ GetIndex(tone, rand),/*root*/ "_" + s + i,/*data*/  null,/*ticket*/ "" + s + ticket[i - 1], utype, ltype));
                            break;

                        case STATE.QUICKDECISION:
                            units_decision.Add(UNIT.Create(mind,/*value*/ GetIndex(tone, rand),/*root*/ "_" + s + i,/*data*/  null,/*ticket*/ "" + s + ticket[i - 1], utype, ltype));
                            break;

                        default: throw new NotImplementedException();
                    }

                    _count++;
                }
            }
        }


        public void HubsCommon(int u_count, List<string> list, TONE tone)
        {
            //XElement xdoc;
            //if (mind.parms.setup_tags == TAGSETUP.PRIME)
            //    xdoc = XElement.Load(PathSetup.MyPath(mind.settings));
            //else
            //    throw new Exception();

            foreach (string s in list)
            {
                List<UNIT> _u = new List<UNIT>();

                for (int i = 1; i <= u_count; i++)
                {
                    UNIT _un;

                    switch (mind.parms[mind.current].state)
                    {
                        case STATE.JUSTRUNNING: _un = units_running.Where(x => x.root == "_" + s + i).First(); break;
                        case STATE.QUICKDECISION: _un = units_decision.Where(x => x.root == "_" + s + i).First(); break;
                        default: throw new NotImplementedException();
                    }
                    _u.Add(_un);
                }

                HUB _h = HUB.Create(s, _u, tone);

                HUBS_ADD(mind.parms[mind.current].state, _h);
            }
        }
        public int UnitsDecide(STATE state, List<string> list, UNITTYPE utype, LONGTYPE ltype, int count, TONE tone)
        {
            //XElement xdoc;
            //if (mind.parms.setup_tags == TAGSETUP.PRIME)
            //    xdoc = XElement.Load(PathSetup.MyPath(mind.settings));
            //else
            //    throw new Exception();

            Random random = new Random();

            int _count = 0;
            foreach (string s in list)
            {
                double rand = mind.cycles < Constants.FIRST_RUN ?
                random.NextDouble() :
                mind.rand.MyRandomDouble(list.Count())[_count];

                switch (state)
                {
                    case STATE.JUSTRUNNING:
                        units_running.Add(UNIT.Create(mind,/*value*/ GetIndex(tone, rand),/*root*/ "_" + utype.ToString().ToLower() + count,/*data*/  s,/*ticket*/ "NONE", utype, ltype));
                        break;

                    case STATE.QUICKDECISION:
                        units_decision.Add(UNIT.Create(mind,/*value*/ GetIndex(tone, rand),/*root*/ "_" + s.ToLower() + count,/*data*/  s,/*ticket*/ "NONE", utype, ltype));
                        break;

                    default: throw new NotImplementedException();
                }

                _count++;
                count++;
            }

            return count;
        }

        public int HubsDecide(STATE state, string subject, List<string> list, UNITTYPE type, int count, TONE tone)
        {
            //XElement xdoc;
            //if (mind.parms.setup_tags == TAGSETUP.PRIME)
            //    xdoc = XElement.Load(PathSetup.MyPath(mind.settings));
            //else
            //    throw new Exception();

            List<UNIT> _u = new List<UNIT>();

            foreach (string s in list)
            {
                UNIT _un;

                switch (state)
                {
                    case STATE.JUSTRUNNING: _un = units_running.Where(x => x.root == "_" + type.ToString().ToLower() + count).First(); break;
                    case STATE.QUICKDECISION: _un = units_decision.Where(x => x.root == "_" + s.ToLower() + count).First(); break;
                    default: throw new NotImplementedException();
                }
                _u.Add(_un);

                count++;
            }

            HUB _h = HUB.Create(subject, _u, tone);

            HUBS_ADD(state, _h);

            return count;
        }

        public void UnitsQuick(string name, int count, UNITTYPE utype, LONGTYPE ltype, TONE tone)
        {
            //XElement xdoc;
            //if (mind.parms.setup_tags == TAGSETUP.PRIME)
            //    xdoc = XElement.Load(PathSetup.MyPath(mind.settings));
            //else
            //    throw new Exception();

            Random random = new Random();

            for (int i = 0; i < count; i++)
            {
                double rand = mind.cycles < Constants.FIRST_RUN ?
                random.NextDouble() :
                mind.rand.MyRandomDouble(count)[i];

                units_running.Add(UNIT.Create(mind,/*value*/ GetIndex(tone, rand),/*root*/ "_" + name.ToLower() + i,/*data*/ name,/*ticket*/ "NONE", utype, ltype));
            }
        }

        public void HubsQuick(string subject, string name, int count, UNITTYPE type, TONE tone)
        {
            //XElement xdoc;
            //if (mind.parms.setup_tags == TAGSETUP.PRIME)
            //    xdoc = XElement.Load(PathSetup.MyPath(mind.settings));
            //else
            //    throw new Exception();

            List<UNIT> _u = new List<UNIT>();

            for (int i = 0; i < count; i++)
            {
                UNIT _un = units_running.Where(x => x.root == "_" + name.ToLower() + i).First();
                _u.Add(_un);
            }

            HUB _h = HUB.Create(subject, _u, tone);

            HUBS_ADD(STATE.JUSTRUNNING, _h);
        }

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
