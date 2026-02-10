using Awesome.AI.Common;
using Awesome.AI.Core;
using Awesome.AI.Core.Spaces;
using Awesome.AI.CoreInternals;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.CoreSystems
{
    class SimpleAgent
    {
        /*
         * family, friends and just persons to interact with
         * */

        private bool Property { get; set; }

        private TheMind mind;
        private SimpleAgent() { }
        public SimpleAgent(TheMind mind)
        {
            this.mind = mind;
        }

        public double SimulateDirection()
        {
            //bool samp = CONST.SAMPLE20.RandomSample(mind);

            double rand = mind.rand.MyRandomDouble(1)[0];

            double down = rand <= 0.5d ? -1.0d : 1.0d;

            return down;
        }

        public bool SimulateYes()
        {
            //bool samp = CONST.SAMPLE20.RandomSample(mind);

            double dir = SimulateDirection();

            bool down = dir <= 0.0d;

            return down;
        }

        public void SetProperty(bool _b)
        {
            Property = _b;
        }
    }

    class TimeLine
    {
        /*
         * youth, adolecence, events, learning..
         * for creating memories and setting up the system 
         * */
    }


    /*
     * rooms at home, work and other
     * ie. at work you mostly think about work and so on
     * */
    public class Occupasion
    {
        public string name { get; set; }
        public int max_epochs { get; set; }
        public List<string> values { get; set; }
    }

    public class Ticket
    {
        public string t_name { get; set; }

        public Ticket(string _n)
        {
            t_name = _n;
        }
    }

    public class MyInternal// aka MapMind
    {
        private TheMind mind;
        private MyInternal() { }
        public MyInternal(TheMind mind)
        {
            this.mind = mind;
        }

        public List<Occupasion> occus = new List<Occupasion>();//this is the map

        private Occupasion occu = new Occupasion() { name = "init", max_epochs = 10, values = null };
        private bool run = false;
        private int epoch_old = -1;
        public int epoch_count = 0;
        public int epoch_stop = -1;


        public Occupasion Occu
        {
            get
            {
                /*
                 * run is only true once per cycle
                 * */

                run = mind.epochs != epoch_old;
                epoch_old = mind.epochs;

                if (run)
                {
                    switch (mind.parms_current.occupasion)
                    {
                        case OCCUPASION.FIXED:
                            Lookup lookup = new Lookup();
                            MINDS mindtype = mind.mindtype;
                            List<string> list = lookup.GetOCCU(mindtype, 0, out string _o);
                            occu = new Occupasion() { name = _o, max_epochs = 30, values = list };
                            break;
                        case OCCUPASION.DYNAMIC:

                            /*
                             * rand should be set according to hobbys, mood, location, interests etc..
                             * ..maybe not
                             * */

                            if (epoch_count <= epoch_stop)
                                break;

                            epoch_count = 0;
                            epoch_stop = mind.rand.MyRandomInt(1, occu.max_epochs)[0];
                            int index = mind.rand.MyRandomInt(1, occus.Count - 1)[0];

                            occu = occus[index];

                            break;
                        default:
                            throw new Exception("Occu");
                    }

                    epoch_count++;
                }

                return occu;
            }
        }

        public bool Valid(UNIT _u)
        {
            if (mind.z_current == "z_noise")
                return true;

            if (!Filters.FilterUnit(mind, FILTERUNIT.NONE, FILTERTYPE.THREE, _u))
                return true;

            try
            {
                string name = Occu.name;
                string sub = mind.hub.GetSubject(_u) ?? "";

                if (name == "")
                    return false;

                if (sub == "")
                    return false;

                Occupasion occu = occus.Where(x => x.name == name).First();
                List<string> _hubs = occu.values;
                bool res = _hubs.Contains(sub);

                return res;
            }
            catch
            {
                return false;
            }
        }

        //process occupation
        public void Setup()
        {
            /*
             * these should be set according to hobbys, mood, location, interests etc..
             * */

            occus = new List<Occupasion>();

            Lookup lookup = new Lookup();
            MINDS[] minds = { MINDS.ROBERTA, MINDS.ANDREW };
            int occu_count = lookup.occupasions.Length;
            
            foreach (MINDS mind in minds)
            {
                for (int i = 0; i < occu_count; i++)
                {
                    List<string> list = lookup.GetOCCU(mind, i, out string occu);
                    occus.Add(new Occupasion() { name = occu, max_epochs = 30, values = list });                
                }
            }
        }

        public void Reset()
        {
            if (mind.z_current == "z_noise")
                return;

            if (mind.STATE == STATE.QUICKDECISION)
                return;

            if (mind.parms_current.validation != VALIDATION.EXTERNAL)
                Setup();
        }
    }


    public class MyExternal// aka MapWorld
    {
        public class Tag
        {
            public string t_name { get; set; }

            public Tag(string _n)
            {
                t_name = _n;
            }
        }

        public List<Tag> tags = new List<Tag>();//this is the map

        private TheMind mind;
        private MyExternal() { }
        public MyExternal(TheMind mind)
        {
            this.mind = mind;
        }

        public bool Valid(UNIT _u)
        {
            if (mind.z_current == "z_noise")
                return true;

            if (_u.ticket.IsNull())
                throw new Exception("Valid");

            if (_u.IsDECISION())
                return true;

            if (_u.IsQUICKDECISION())
                return true;

            tags = tags.Where(x => x != null).ToList();

            double scale = 0.0d;

            //bool t_name = _u.ticket.t_name == "SPECIAL";
            bool tags_hit = tags.Where(x => x.t_name == _u.ticket.t_name).Any();

            bool hit = /*t_name || */tags_hit;// || focus;

            return hit;
        }

        //setup input
        private void Setup(MINDS mindtype, bool onlyeven)
        {
            tags = new List<Tag>();
            tags.Add(new Tag("SPECIAL"));

            List<string> list;

            list = mind.memory.Tags(mindtype);

            foreach (string s in list)
            {
                for (int i = 1; i <= CONST.NUMBER_OF_UNITS; i++)
                {
                    if (onlyeven && i % 2 == 0)
                        continue;

                    tags.Add(new Tag("" + s + i));
                }
            }
        }

        public void Reset()
        {
            if (mind.z_current == "z_noise")
                return;

            if (mind.parms_current.validation != VALIDATION.INTERNAL)
            {
                //mind.stats.Reset();

                tags = new List<Tag>();
                switch (mind.parms_current.tags)
                {
                    case TAGS.ALL: Setup(mind.mindtype, false); break;
                    case TAGS.EVEN: Setup(mind.mindtype, true); break;
                    default: throw new Exception("Reset");
                }
            }
        }
    }
}


