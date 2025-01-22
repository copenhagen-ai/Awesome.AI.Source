using Awesome.AI.Common;
using Awesome.AI.Core;
using Awesome.AI.Helpers;
using static Awesome.AI.Helpers.Enums;

namespace Awesome.AI.CoreHelpers
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

        private List<string> should_decision = new List<string>()
        {
            //Constants.decision_u1,//MAKEDECISION
            Constants.should_decision_u1,//YES
            Constants.should_decision_u1,//YES
            Constants.should_decision_u1,//YES
            Constants.should_decision_u1,//YES
            Constants.should_decision_u1,//YES
            Constants.should_decision_u1,//YES
            Constants.should_decision_u1,//YES
            Constants.should_decision_u1,//YES
                                    
            //Constants.should_decision_u2,//NO
        };

        private List<string> what_decision = new List<string>()
        {
            //Constants.decision_u1,//MAKEDECISION
            Constants.what_decision_u1,//KITCHEN
            Constants.what_decision_u1,//KITCHEN
            Constants.what_decision_u1,//KITCHEN
            Constants.what_decision_u1,//KITCHEN
            Constants.what_decision_u2,//BEDROOM
            Constants.what_decision_u2,//BEDROOM
            Constants.what_decision_u2,//BEDROOM
            Constants.what_decision_u2,//BEDROOM
            Constants.what_decision_u3,//LIVINGROOM
            Constants.what_decision_u3,//LIVINGROOM
            Constants.what_decision_u3,//LIVINGROOM
            Constants.what_decision_u3,//LIVINGROOM
        };

        private List<UNIT> units { get; set; }
        private List<HUB> hubs { get; set; }
        
        public List<UNIT> learning = new List<UNIT>();

        TheMind mind;
        private Memory() { }
        
        public Memory(TheMind mind, int u_count) 
        {
            this.mind = mind;

            units = new List<UNIT>();
            hubs = new List<HUB>();

            List<string> commen = mind.mindtype == MINDS.ROBERTA ? roberta : andrew;
            List<string> should_decision = this.should_decision;
            List<string> what_decision = this.what_decision;

            UnitsCommon(u_count, commen, TYPE.JUSTAUNIT);
            HubsCommon(u_count, commen);

            int count1 = 1;
            int count2 = 1;

            count1 = UnitsDecide(should_decision, TYPE.DECISION, count1);
            count2 = HubsDecide(Constants.subject_decision[0], should_decision, TYPE.DECISION, count2);

            count1 = UnitsDecide(what_decision, TYPE.DECISION, count1);
            count2 = HubsDecide(Constants.subject_decision[1], what_decision, TYPE.DECISION, count2);

            /*
             * setup for learning
             * */
            //learning
            //ca 25000 cycles
            //learning.Add(UNIT.Create(24.13f, 61.0f, "pi next:PI_Next", TYPE.UNIT));

            //ca 10000 cycles
            //learning.Add(UNIT.Create(23.54f, 60.5f, "pi is:PI_Is", TYPE.UNIT));

            //ca 50000 cycles
            //learning.Add(UNIT.Create(61.13f, 61.0f, "tictac:TicTacToe", TYPE.UNIT));

            //ca 25000 cycles
            //learning.Add(UNIT.Create(Calc.RandomDouble(49.0d, 51.0d), "CALCONv20a", "ME1", "SPECIAL", TYPE.JUSTAUNIT));
            ////learning.Add(WORD.Create(24.14f, 61.1f, "CALCONv20b:ME1", 1.0d, TYPE.WORD, DIRECTION.NONE));//second key

            //HUB _new17 = HUB.Create("LEARNING", Memory.learning, Params.is_accord, neurons, 0.25d, 0.9d);

            //HUBS_ADD(_new17);

        }

        private int _c = -1;
        private void Reset()
        {
            if (_c == mind.cycles_all)
                return;

            if (mind.cycles_all % 10000 == 0)
            {
                units = new List<UNIT>();
                
                foreach (HUB h in hubs)
                    units = units.Concat(h.units).ToList();

                units = units.OrderBy(x => x.Index).ToList();
            }
            
            _c = mind.cycles_all;
        }

        public List<UNIT> UNITS_ALL()
        {
            Reset();

            return units;
        }

        public List<UNIT> UNITS_VAL()
        {
            Reset();

            List<UNIT> res = units.Where(x => x.IsValid).ToList();

            return res;
        }

        public List<HUB> HUBS_ALL()
        {
            return hubs.ToList();
        }

        public HUB HUBS_DEX(int index)
        {
            if (index < 0 || index >= hubs.Count)
                throw new Exception();

            HUB _h = hubs[index];
            
            return _h;
        }

        public HUB HUBS_RND()
        {
            int[] rand = mind.rand.MyRandomInt(1, hubs.Count() - 1);
            
            HUB _h = hubs[rand[0]];
            
            return _h;
        }

        public HUB HUBS_SUB(string subject)
        {
            if (subject == null)
                throw new ArgumentNullException();

            HUB _h = hubs.Where(x=>x.GetSubject() == subject).FirstOrDefault();
            
            return _h;
        }

        public void HUBS_ADD(HUB h)
        {
            if (h == null)
                throw new ArgumentNullException();

            hubs.Add(h);
            hubs = hubs.OrderBy(x=>x.GetSubject()).ToList();
        }

        public void Randomize(List<UNIT> units)
        {
            MyRandom rand = mind.rand;

            int count = units.Count;
            double[] doubles = rand.MyRandomDouble(count);

            int _i = 0;
            foreach (UNIT _u in units)
            {
                double _rand = doubles[_i] * 100.0d;
                _rand = _rand.Convert(mind);

                _u.Index = _rand;
                _i++;
            }
        }

        public void UnitsCommon(int u_count, List<string> list, TYPE type)
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
                for(int i = 1; i <= u_count; i++)
                    ticket.Add(i);

                ticket.Shuffle<int>();

                for (int i = 1; i <= u_count; i++)
                {
                    double rand = random.NextDouble() * 100.0d;
                    rand = rand.Convert(mind);

                    units.Add(
                        UNIT.Create(
                            mind,
                            rand,//value
                            "_" + s + i,//root
                            null,
                            "" + s + ticket[i - 1],//ticket
                            type
                        ));
                }
            }
        }

        public int UnitsDecide(List<string> list, TYPE type, int count)
        {
            //XElement xdoc;
            //if (mind.parms.setup_tags == TAGSETUP.PRIME)
            //    xdoc = XElement.Load(PathSetup.MyPath(mind.settings));
            //else
            //    throw new Exception();

            Random random = new Random();
                        
            foreach (string s in list)
            {
                double rand = random.NextDouble() * 100.0d;
                rand = rand.Convert(mind);

                units.Add(
                    UNIT.Create(
                        mind,
                        rand,//value
                        "_" + type.ToString().ToLower() + count,//root
                        s,
                        "SPECIAL",//ticket
                        type
                    ));
                          
                count++;
            }

            return count;
        }

        public void HubsCommon(int u_count, List<string> list)
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
                    UNIT _un = units.Where(x => x.root == "_" + s + i).FirstOrDefault();
                    _u.Add(_un);
                }

                HUB _h = HUB.Create(s, _u);

                HUBS_ADD(_h);
            }
        }

        public int HubsDecide(string subject, List<string> list, TYPE type, int count)
        {
            //XElement xdoc;
            //if (mind.parms.setup_tags == TAGSETUP.PRIME)
            //    xdoc = XElement.Load(PathSetup.MyPath(mind.settings));
            //else
            //    throw new Exception();

            List<UNIT> _u = new List<UNIT>();

            foreach (string s in list)
            {
                UNIT _un = units.Where(x => x.root == "_" + type.ToString().ToLower() + count).FirstOrDefault();
                _u.Add(_un);

                count++;
            }
            
            HUB _h = HUB.Create(subject, _u);
            
            HUBS_ADD(_h);

            return count;
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
