using Awesome.AI.Common;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core.Spaces
{
    public class USSetup
    {
        private int count_loc_what = 0;
        public string GetLOCWHAT
        {
            get
            {
                if (count_loc_what > 2)
                    count_loc_what = 0;

                switch (count_loc_what)
                {
                    case 0: return CONST.LDAT_LOC_WHAT_u1;
                    case 1: return CONST.LDAT_LOC_WHAT_u2;
                    case 2: return CONST.LDAT_LOC_WHAT_u3;
                    default: throw new Exception("GetWHAT, USSetup");
                }

                count_loc_what++;
            }
        }

        private int count_ans_what = 0;
        public string GetANSWHAT
        {
            get
            {
                if (count_ans_what > 2)
                    count_ans_what = 0;

                switch (count_ans_what)
                {
                    case 0: return CONST.LDAT_ANS_WHAT_u1;
                    case 1: return CONST.LDAT_ANS_WHAT_u2;
                    case 2: return CONST.LDAT_ANS_WHAT_u3;
                    default: throw new Exception("GetANSWHAT, USSetup");
                }

                count_ans_what++;
            }
        }

        public List<UNIT> units_running { get; set; }
        public List<UNIT> units_decision { get; set; }

        public List<UNIT> learning = new List<UNIT>();

        private TheMind mind;
        private USSetup() { }

        public USSetup(TheMind mind)
        {
            this.mind = mind;

            units_running = new List<UNIT>();
            units_decision = new List<UNIT>();            
        }        

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
            if (type == MINDS.ANDREW)
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

            if (type == MINDS.ROBERTA)
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

            if (type == MINDS.BASIC)
            {
                List<string> basic = new List<string>()
                {
                    CONST.basic_s1,//"love",
                    CONST.basic_s2,//"macho machines",
                    CONST.basic_s3,//"music",
                    CONST.basic_s4,//"friends",
                    CONST.basic_s5,//"socializing",
                    CONST.basic_s6,//"dancing",
                    CONST.basic_s7,//"movies",
                    CONST.basic_s8,//"existence",
                    CONST.basic_s9,//"termination",
                    CONST.basic_s10,//"programming"
                };

                return basic.ToList();
            }

            throw new Exception("Memory, Tags");
        }

        public void Common(/*int num_regions, */int num_units, List<string> list, UNITTYPE utype, LONGTYPE ltype, TONE tone)
        {
            Random random = new Random();

            foreach (string s in list)
            {
                List<int> ticket = new List<int>();
                for (int i = 1; i <= num_units; i++)
                    ticket.Add(i);

                ticket.Shuffle();

                List<UNIT> _u = new List<UNIT>();

                string guid = Guid.NewGuid().ToString();
                                
                for (int i = 1; i <= num_units; i++)
                {
                    double rand1 = mind.cycles < CONST.FIRST_RUN ?
                    random.NextDouble() :
                    mind.rand.MyRandomDouble(list.Count())[10];

                    double rand2 = mind.cycles < CONST.FIRST_RUN ?
                    random.NextDouble() :
                    mind.rand.MyRandomDouble(list.Count())[20];

                    double rand3 = mind.cycles < CONST.FIRST_RUN ?
                    random.NextDouble() :
                    mind.rand.MyRandomDouble(list.Count())[30];

                    double rand4 = mind.cycles < CONST.FIRST_RUN ?
                    random.NextDouble() :
                    mind.rand.MyRandomDouble(list.Count())[40];

                    double rand5 = mind.cycles < CONST.FIRST_RUN ?
                    random.NextDouble() :
                    mind.rand.MyRandomDouble(list.Count())[50];

                    double rand6 = mind.cycles < CONST.FIRST_RUN ?
                    random.NextDouble() :
                    mind.rand.MyRandomDouble(list.Count())[60];

                    double[] arr = [GetIndex(tone, rand1), GetIndex(tone, rand2), GetIndex(tone, rand3), GetIndex(tone, rand4), GetIndex(tone, rand5), GetIndex(tone, rand6)];

                    _u.Add(UNIT.Create(mind, guid, arr, "DATA", "" + s + ticket[i - 1], utype, ltype));
                }

                switch (mind.STATE)
                {
                    case STATE.JUSTRUNNING: units_running = units_running.Concat(_u).ToList(); break;
                    case STATE.QUICKDECISION: units_decision = units_decision.Concat(_u).ToList(); break;
                    default: throw new NotImplementedException();
                }
            }
        }

        public void Decide(STATE state, string subject, string s, UNITTYPE utype, LONGTYPE ltype, TONE tone)
        {
            List<UNIT> _u = new List<UNIT>();

            Random random = new Random();

            string guid = Guid.NewGuid().ToString();

            double rand1 = mind.rand.MyRandomDouble(100)[10];//->100

            double rand2 = mind.rand.MyRandomDouble(100)[20];//->200

            double rand3 = mind.rand.MyRandomDouble(100)[30];//->200

            double rand4 = mind.rand.MyRandomDouble(100)[40];//->200

            double rand5 = mind.rand.MyRandomDouble(100)[50];//->200

            double rand6 = mind.rand.MyRandomDouble(100)[60];//->200

            double[] arr = [GetIndex(tone, rand1), GetIndex(tone, rand2), GetIndex(tone, rand3), GetIndex(tone, rand4), GetIndex(tone, rand5), GetIndex(tone, rand6)];

            switch (state)
            {
                case STATE.JUSTRUNNING: 
                    _u.Add(UNIT.Create(mind, guid, arr, s, "NONE", utype, ltype)); break;
                case STATE.QUICKDECISION: 
                    _u.Add(UNIT.Create(mind, guid, arr, s, "NONE", utype, ltype)); break;
                default: throw new NotImplementedException();
            }
            
            switch (state)
            {
                case STATE.JUSTRUNNING: units_running = units_running.Concat(_u).ToList(); break;
                case STATE.QUICKDECISION: units_decision = units_decision.Concat(_u).ToList(); break;
                default: throw new NotImplementedException();
            }
        }

    }
}