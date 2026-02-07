using Awesome.AI.Common;
using Awesome.AI.Core;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.CoreInternals
{
    public class Memory
    {
        private List<string> location_should_decision = new List<string>()
        {
            //Constants.decision_u1,//MAKEDECISION
            CONST.location_should,//YES
            CONST.location_should,//YES
            CONST.location_should,//YES
            CONST.location_should,//YES
            CONST.location_should,//YES
            CONST.location_should,//YES
            CONST.location_should,//YES
            CONST.location_should,//YES
            CONST.location_should,//YES
            CONST.location_should,//YES
            CONST.location_should,//YES
            CONST.location_should,//YES
            CONST.location_should,//YES
            CONST.location_should,//YES
            CONST.location_should,//YES
            CONST.location_should,//YES
            CONST.location_should,//YES
            CONST.location_should,//YES
            CONST.location_should,//YES
            CONST.location_should,//YES
                                    
            //Constants.should_decision_u2,//NO
        };

        private List<string> location_what_decision = new List<string>()
        {
            //Constants.decision_u1,//MAKEDECISION
            CONST.location_what_u1,//KITCHEN
            CONST.location_what_u1,//KITCHEN
            CONST.location_what_u1,//KITCHEN
            CONST.location_what_u1,//KITCHEN
            CONST.location_what_u1,//KITCHEN
            CONST.location_what_u1,//KITCHEN
            CONST.location_what_u1,//KITCHEN
            CONST.location_what_u2,//BEDROOM
            CONST.location_what_u2,//BEDROOM
            CONST.location_what_u2,//BEDROOM
            CONST.location_what_u2,//BEDROOM
            CONST.location_what_u2,//BEDROOM
            CONST.location_what_u2,//BEDROOM
            CONST.location_what_u2,//BEDROOM
            CONST.location_what_u3,//LIVINGROOM
            CONST.location_what_u3,//LIVINGROOM
            CONST.location_what_u3,//LIVINGROOM
            CONST.location_what_u3,//LIVINGROOM
            CONST.location_what_u3,//LIVINGROOM
            CONST.location_what_u3,//LIVINGROOM
            CONST.location_what_u3,//LIVINGROOM
        };

        private List<string> answer_should_decision = new List<string>()
        {
            //Constants.decision_u1,//MAKEDECISION
            CONST.answer_should,//YES
            CONST.answer_should,//YES
            CONST.answer_should,//YES
            CONST.answer_should,//YES
            CONST.answer_should,//YES
            CONST.answer_should,//YES
            CONST.answer_should,//YES
            CONST.answer_should,//YES
            CONST.answer_should,//NO
            CONST.answer_should,//NO
                                    
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
            CONST.ask_should,//YES
            CONST.ask_should,//YES
            CONST.ask_should,//YES
            CONST.ask_should,//YES
            CONST.ask_should,//YES
            CONST.ask_should,//NO
            CONST.ask_should,//NO
            CONST.ask_should,//NO
            CONST.ask_should,//NO
            CONST.ask_should,//NO
                                    
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
        
        public List<UNIT> learning = new List<UNIT>();

        private TheMind mind;
        private Memory() { }

        public Memory(TheMind mind)
        {
            this.mind = mind;

            units_running = new List<UNIT>();
            units_decision = new List<UNIT>();
            
            List<string> commen = Tags(mind.mindtype);
            //List<string> long_decision_should = this.location_should_decision;
            //List<string> long_decision_what = this.location_what_decision;
            //List<string> answer_should_decision = this.answer_should_decision;
            //List<string> answer_what_decision = this.answer_what_decision;
            //List<string> ask_should_decision = this.ask_should_decision;

            Common(CONST.NUMBER_OF_UNITS, commen, UNITTYPE.JUSTAUNIT, LONGTYPE.NONE, TONE.RANDOM);

            int count1 = 1;

            TONE tone;
            tone = mind._mech == MECHANICS.GRAVITY_HIGH ? TONE.RANDOM : TONE.RANDOM;
            count1 = Decide(STATE.JUSTRUNNING, CONST.DECI_SUBJECT_A, location_should_decision, UNITTYPE.LDECISION, LONGTYPE.LOCATION, count1, tone);
            
            tone = mind._mech == MECHANICS.GRAVITY_HIGH ? TONE.RANDOM : TONE.HIGH;
            count1 = Decide(STATE.JUSTRUNNING, CONST.DECI_SUBJECT_B, location_what_decision, UNITTYPE.LDECISION, LONGTYPE.LOCATION, count1, tone);
            
            tone = mind._mech == MECHANICS.GRAVITY_HIGH ? TONE.RANDOM : TONE.RANDOM;
            count1 = Decide(STATE.JUSTRUNNING, CONST.DECI_SUBJECT_A, answer_should_decision, UNITTYPE.LDECISION, LONGTYPE.ANSWER, count1, tone);
            
            tone = mind._mech == MECHANICS.GRAVITY_HIGH ? TONE.RANDOM : TONE.LOW;
            count1 = Decide(STATE.JUSTRUNNING, CONST.DECI_SUBJECT_B, answer_what_decision, UNITTYPE.LDECISION, LONGTYPE.ANSWER, count1, tone);
            
            tone = mind._mech == MECHANICS.GRAVITY_HIGH ? TONE.RANDOM : TONE.MID;
            count1 = Decide(STATE.JUSTRUNNING, CONST.DECI_SUBJECT_A, ask_should_decision, UNITTYPE.LDECISION, LONGTYPE.ASK, count1, tone);
            
            //Dictionary<string, int[]> dict = mind.mindtype == MINDS.ROBERTA ? CONST.DECISIONS_R : CONST.DECISIONS_A;
            //foreach (var kv in dict)
            {
                tone = mind._mech == MECHANICS.GRAVITY_HIGH ? TONE.RANDOM : TONE.RANDOM;
                Quick(5 /*kv.Value[1]*/, CONST.DECI_SUBJECT_C, "WHISTLE"/*kv.Key*/, UNITTYPE.QDECISION, LONGTYPE.NONE, tone);
            }
        }

        public List<UNIT> UNITS_ALL(ORDER order = ORDER.NONE)
        {
            if (mind.STATE == STATE.JUSTRUNNING && !units_running.Any())
                throw new Exception("Memory, UNITS_VAL 1");

            if (mind.STATE == STATE.QUICKDECISION && !units_decision.Any())
                throw new Exception("Memory, UNITS_VAL 2");

            switch (mind.STATE)
            {
                case STATE.JUSTRUNNING:
                    if (order == ORDER.BYINDEX)
                        return units_running.OrderBy(x=>x.UnitIndex).ToList();
                    if (order == ORDER.BYVARIABLE)
                        return units_running.OrderBy(x=>x.Variable).ToList();
                    return units_running;
                case STATE.QUICKDECISION: return units_decision;
                default: throw new NotImplementedException();
            }
        }

        public UNIT UNIT_GUID(string guid)
        {
            UNIT res;

            if (mind.STATE == STATE.JUSTRUNNING && !units_running.Any())
                throw new Exception("Memory, UNITS_VAL 1");

            if (mind.STATE == STATE.QUICKDECISION && !units_decision.Any())
                throw new Exception("Memory, UNITS_VAL 2");

            switch (mind.STATE)
            {
                case STATE.JUSTRUNNING: res = units_running.Where(x => x.guid == guid).First(); break;
                default: throw new NotImplementedException();
            }

            return res;
        }

        public List<UNIT> UNITS_VAL()
        {
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

            string ticket = "" + unit.HUB.Subject + rand;
            string guid = "" + unit.guid;

            UNIT _u = UNIT.Create(mind, guid, idx, "DATA", ticket, UNITTYPE.JUSTAUNIT, LONGTYPE.NONE);

            units_running.Add(_u);
        }

        public void UNITS_REM(UNIT unit, double low, double high)
        {
            List<UNIT> list = UNITS_ALL().Where(x => x.Variable > low && x.Variable < high).ToList();
            list = list.Where(x => x.created < unit.created).ToList();

            foreach (UNIT _u in list) 
                units_running.Remove(_u);            
        }
        
        public void QDRESETU() => units_decision = new List<UNIT>();
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

        public void Common(/*int num_regions, */int num_units, List<string> list, UNITTYPE utype, LONGTYPE ltype, TONE tone)
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
            }
        }

        public int Decide(STATE state/*, int max_units*/, string subject, List<string> units, UNITTYPE utype, LONGTYPE ltype, int count, TONE tone)
        {
            //XElement xdoc;
            //if (mind.parms.setup_tags == TAGSETUP.PRIME)
            //    xdoc = XElement.Load(PathSetup.MyPath(mind.settings));
            //else
            //    throw new Exception();

            List<UNIT> _u = new List<UNIT>();

            Random random = new Random();


            int _count = 0;
            foreach (string s in units)
            {
                string guid = Guid.NewGuid().ToString();
                double rand = mind.cycles < CONST.FIRST_RUN ?
                random.NextDouble() :
                mind.rand.MyRandomDouble(units.Count())[_count];

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

            return count;
        }

        public void Quick(/*int max_units, */int num_units, string subject, string name, UNITTYPE utype, LONGTYPE ltype, TONE tone)
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
        }        
    }
}
