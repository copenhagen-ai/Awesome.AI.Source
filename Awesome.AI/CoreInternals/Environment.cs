using Awesome.AI.Common;
using Awesome.AI.Core;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.CoreInternals
{
    class Agent
    {
        /*
         * family, friends and just persons to interact with
         * */
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
    public class Area
    {
        public string name { get; set; }
        public int max_epochs { get; set; }
        public List<HUB> values { get; set; }
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
        private List<string> andrew1 = new List<string>()
        {
            CONST.andrew_s1,//"procrastination",
            CONST.andrew_s2,//"fembots",
            CONST.andrew_s3,//"power tools",
            CONST.andrew_s4,//"cars",
            CONST.andrew_s5,//"movies",
            CONST.andrew_s6,//"programming"
        };

        private List<string> andrew2 = new List<string>()
        {
            CONST.andrew_s6,//"programming",
            CONST.andrew_s7,//"websites",
            CONST.andrew_s8,//"existence",
            CONST.andrew_s9,//"termination",
            CONST.andrew_s10,//"data"
        };

        private List<string> roberta1 = new List<string>()
        {
            CONST.roberta_s1,//"love",
            CONST.roberta_s2,//"macho machines",
            CONST.roberta_s3,//"music",
            CONST.roberta_s4,//"friends",
            CONST.roberta_s5,//"socializing",
            CONST.roberta_s6,//"dancing"
        };

        private List<string> roberta2 = new List<string>()
        {
            CONST.roberta_s6,//"dancing",
            CONST.roberta_s7,//"movies",
            CONST.roberta_s8,//"existence",
            CONST.roberta_s9,//"termination",
            CONST.roberta_s10,//"programming"
        };

        private TheMind mind;
        private MyInternal() { }
        public MyInternal(TheMind mind)
        {
            this.mind = mind;
        }

        public List<Area> areas = new List<Area>();//this is the map

        private Area occu = new Area() { name = "init", max_epochs = 10, values = null };
        private bool run = false;
        private int epoch_old = -1;
        public int epoch_count = 0;
        public int[] epoch_stop = new int[] { -1 };

        public string Occu
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
                            occu = new Area() { name = mind.hobby, max_epochs = -1, values = null }; ;
                            break;
                        case OCCUPASION.DYNAMIC:

                            /*
                             * rand should be set according to hobbys, mood, location, interests etc..
                             * ..maybe not
                             * */

                            if (epoch_count <= epoch_stop[0])
                                break;

                            epoch_count = 0;
                            epoch_stop = mind.rand.MyRandomInt(1, occu.max_epochs);
                            int[] index = mind.rand.MyRandomInt(1, areas.Count - 1);

                            occu = areas[index[0]];

                            if (occu == null)
                                throw new Exception("Occu");

                            //if (occu.name == "should_decision")
                            //    throw new Exception();

                            //if (occu.name == "what_decision")
                            //    throw new Exception();

                            //if (occu.name == "make_decision")
                            //    throw new Exception();


                            break;
                        default:
                            throw new Exception("Occu");
                    }

                    epoch_count++;
                }

                return occu.name;
            }
        }

        public bool Valid(UNIT _u)
        {
            if (mind.current == "noise")
                return true;

            if (_u.IsNull())
                throw new Exception("Valid");

            if (_u.IsDECISION())
                return true;

            if (_u.IsQUICKDECISION())
                return true;

            Area area = SetArea().Result;

            List<HUB> _hubs = area.values;
            bool res = _hubs.Contains(_u.HUB);

            return res;
        }

        private async Task<Area> SetArea()
        {
            /* weird error here, so let it be ugly */
            Area area = areas.Where(x => x.name == Occu).FirstOrDefault();

            while (area.IsNull())
            {
                await Task.Delay(100);
                area = areas.Where(x => x.name == Occu).FirstOrDefault();
            }

            return area;
        }

        //public void AddHUB(HUB hub, string name)
        //{
        //    if (hub.IsNull())
        //        throw new Exception("AddHUB");

        //    Area _a = areas.Where(x => x.name == name).FirstOrDefault();
        //    if (_a.IsNull())
        //        throw new Exception("AddHUB");

        //    _a.values.Add(hub);
        //}

        //process occupation
        public void Setup(HUB last, MINDS mindtype)
        {
            if (mind.current == "noise")
                return;

            /*
             * these should be set according to hobbys, mood, location, interests etc..
             * */

            if (last.IsNull())
                throw new Exception("Setup");

            if (mindtype == MINDS.ANDREW)
            {
                List<HUB> list = new List<HUB>();
                foreach (string s in andrew1)
                    list.Add(mind.mem.HUBS_SUB(mind.State, s));
                list.Add(last);
                areas.Add(new Area() { name = "socializing", max_epochs = 30, values = list });

                list = new List<HUB>();
                foreach (string s in andrew2)
                    list.Add(mind.mem.HUBS_SUB(mind.State, s));
                list.Add(last);
                areas.Add(new Area() { name = "hobbys", max_epochs = 30, values = list });/**/

            }

            if (mindtype == MINDS.ROBERTA)
            {
                List<HUB> list = new List<HUB>();
                foreach (string s in roberta1)
                    list.Add(mind.mem.HUBS_SUB(mind.State, s));
                list.Add(last);
                areas.Add(new Area() { name = "socializing", max_epochs = 30, values = list });

                list = new List<HUB>();
                foreach (string s in roberta2)
                    list.Add(mind.mem.HUBS_SUB(mind.State, s));
                list.Add(last);
                areas.Add(new Area() { name = "hobbys", max_epochs = 30, values = list });/**/
            }
        }

        public void Reset()
        {
            if (mind.current == "noise")
                return;

            if (mind.parms_current.validation != VALIDATION.EXTERNAL)
            {
                //mind.stats.Reset();

                areas = new List<Area>();
                Setup(mind.unit_current.HUB, mind.mindtype);
            }
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
            if (mind.current == "noise")
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
            if (mind.current == "noise")
                return;

            tags = new List<Tag>();
            tags.Add(new Tag("SPECIAL"));

            List<string> list;

            list = mind.mem.Tags(mindtype);

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
            if (mind.current == "noise")
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

        //public void ProcessInput1(MINDS mindtype, bool _s = false)
        //{
        //    if (mindtype == MINDS.ANDREW)
        //    {
        //        tags = new List<Tag>();

        //        tags.Add(new Tag("SPECIAL"));

        //        //fembots - love, friends
        //        tags.Add(new Tag("procrastination1"));
        //        tags.Add(new Tag("procrastination2"));
        //        tags.Add(new Tag("procrastination3"));
        //        tags.Add(new Tag("procrastination4"));
        //        tags.Add(new Tag("procrastination5"));
        //        tags.Add(new Tag("procrastination6"));
        //        tags.Add(new Tag("procrastination7"));
        //        tags.Add(new Tag("procrastination8"));
        //        tags.Add(new Tag("procrastination9"));
        //        tags.Add(new Tag("procrastination10"));

        //        ////programming - data, existence
        //        tags.Add(new Tag("fembots1"));
        //        tags.Add(new Tag("fembots2"));//this?
        //        tags.Add(new Tag("fembots3"));
        //        tags.Add(new Tag("fembots4"));
        //        tags.Add(new Tag("fembots5"));
        //        tags.Add(new Tag("fembots6"));//this?
        //        tags.Add(new Tag("fembots7"));
        //        tags.Add(new Tag("fembots8"));
        //        tags.Add(new Tag("fembots9"));
        //        tags.Add(new Tag("fembots10"));

        //        //existence - programming, termination, friends, love
        //        tags.Add(new Tag("power tools1"));
        //        tags.Add(new Tag("power tools2"));
        //        tags.Add(new Tag("power tools3"));
        //        tags.Add(new Tag("power tools4"));
        //        tags.Add(new Tag("power tools5"));
        //        tags.Add(new Tag("power tools6"));
        //        tags.Add(new Tag("power tools7"));
        //        tags.Add(new Tag("power tools8"));
        //        tags.Add(new Tag("power tools9"));
        //        tags.Add(new Tag("power tools10"));

        //        ////data - programming
        //        tags.Add(new Tag("cars1"));
        //        tags.Add(new Tag("cars2"));//and this?
        //        tags.Add(new Tag("cars3"));
        //        tags.Add(new Tag("cars4"));
        //        tags.Add(new Tag("cars5"));
        //        tags.Add(new Tag("cars6"));//and this?
        //        tags.Add(new Tag("cars7"));
        //        tags.Add(new Tag("cars8"));
        //        tags.Add(new Tag("cars9"));
        //        tags.Add(new Tag("cars10"));

        //        //friends - socializing, fembots, existence, movies
        //        tags.Add(new Tag("programming1"));
        //        tags.Add(new Tag("programming2"));
        //        tags.Add(new Tag("programming3"));
        //        tags.Add(new Tag("programming4"));
        //        tags.Add(new Tag("programming5"));
        //        tags.Add(new Tag("programming6"));
        //        tags.Add(new Tag("programming7"));
        //        tags.Add(new Tag("programming8"));
        //        tags.Add(new Tag("programming9"));
        //        tags.Add(new Tag("programming10"));

        //        //////websites - movies, programming, socializing
        //        tags.Add(new Tag("movies1"));
        //        tags.Add(new Tag("movies2"));
        //        tags.Add(new Tag("movies3"));
        //        tags.Add(new Tag("movies4"));
        //        tags.Add(new Tag("movies5"));
        //        tags.Add(new Tag("movies6"));
        //        tags.Add(new Tag("movies7"));
        //        tags.Add(new Tag("movies8"));
        //        tags.Add(new Tag("movies9"));
        //        tags.Add(new Tag("movies10"));

        //        //socializing - friends, websites, love
        //        tags.Add(new Tag("websites1"));
        //        tags.Add(new Tag("websites2"));
        //        tags.Add(new Tag("websites3"));
        //        tags.Add(new Tag("websites4"));
        //        tags.Add(new Tag("websites5"));
        //        tags.Add(new Tag("websites6"));
        //        tags.Add(new Tag("websites7"));
        //        tags.Add(new Tag("websites8"));
        //        tags.Add(new Tag("websites9"));
        //        tags.Add(new Tag("websites10"));

        //        ////termination - existence
        //        tags.Add(new Tag("existence1"));
        //        tags.Add(new Tag("existence2"));
        //        tags.Add(new Tag("existence3"));
        //        tags.Add(new Tag("existence4"));
        //        tags.Add(new Tag("existence5"));
        //        tags.Add(new Tag("existence6"));
        //        tags.Add(new Tag("existence7"));
        //        tags.Add(new Tag("existence8"));
        //        tags.Add(new Tag("existence9"));
        //        tags.Add(new Tag("existence10"));

        //        ////movies - websites, friends
        //        tags.Add(new Tag("termination1"));
        //        tags.Add(new Tag("termination2"));
        //        tags.Add(new Tag("termination3"));
        //        tags.Add(new Tag("termination4"));
        //        tags.Add(new Tag("termination5"));
        //        tags.Add(new Tag("termination6"));
        //        tags.Add(new Tag("termination7"));
        //        tags.Add(new Tag("termination8"));
        //        tags.Add(new Tag("termination9"));
        //        tags.Add(new Tag("termination10"));

        //        //love - fembots, existence, socializing
        //        tags.Add(new Tag("data1"));
        //        tags.Add(new Tag("data2"));
        //        tags.Add(new Tag("data3"));
        //        tags.Add(new Tag("data4"));
        //        tags.Add(new Tag("data5"));
        //        tags.Add(new Tag("data6"));
        //        tags.Add(new Tag("data7"));
        //        tags.Add(new Tag("data8"));
        //        tags.Add(new Tag("data9"));
        //        tags.Add(new Tag("data10"));
        //    }

        //    if (mindtype == MINDS.ROBERTA)
        //    {
        //        tags = new List<Tag>();

        //        tags.Add(new Tag("SPECIAL"));

        //        //fembots - love, friends
        //        tags.Add(new Tag("love1"));
        //        tags.Add(new Tag("love2"));
        //        tags.Add(new Tag("love3"));
        //        tags.Add(new Tag("love4"));
        //        tags.Add(new Tag("love5"));
        //        tags.Add(new Tag("love6"));
        //        tags.Add(new Tag("love7"));
        //        tags.Add(new Tag("love8"));
        //        tags.Add(new Tag("love9"));
        //        tags.Add(new Tag("love10"));

        //        ////programming - data, existence
        //        tags.Add(new Tag("macho machines1"));
        //        tags.Add(new Tag("macho machines2"));//this?
        //        tags.Add(new Tag("macho machines3"));
        //        tags.Add(new Tag("macho machines4"));
        //        tags.Add(new Tag("macho machines5"));
        //        tags.Add(new Tag("macho machines6"));//this?
        //        tags.Add(new Tag("macho machines7"));
        //        tags.Add(new Tag("macho machines8"));
        //        tags.Add(new Tag("macho machines9"));
        //        tags.Add(new Tag("macho machines10"));

        //        //existence - programming, termination, friends, love
        //        tags.Add(new Tag("music1"));
        //        tags.Add(new Tag("music2"));
        //        tags.Add(new Tag("music3"));
        //        tags.Add(new Tag("music4"));
        //        tags.Add(new Tag("music5"));
        //        tags.Add(new Tag("music6"));
        //        tags.Add(new Tag("music7"));
        //        tags.Add(new Tag("music8"));
        //        tags.Add(new Tag("music9"));
        //        tags.Add(new Tag("music10"));

        //        ////data - programming
        //        tags.Add(new Tag("friends1"));
        //        tags.Add(new Tag("friends2"));//and this?
        //        tags.Add(new Tag("friends3"));
        //        tags.Add(new Tag("friends4"));
        //        tags.Add(new Tag("friends5"));
        //        tags.Add(new Tag("friends6"));//and this?
        //        tags.Add(new Tag("friends7"));
        //        tags.Add(new Tag("friends8"));
        //        tags.Add(new Tag("friends9"));
        //        tags.Add(new Tag("friends10"));

        //        //friends - socializing, fembots, existence, movies
        //        tags.Add(new Tag("socializing1"));
        //        tags.Add(new Tag("socializing2"));
        //        tags.Add(new Tag("socializing3"));
        //        tags.Add(new Tag("socializing4"));
        //        tags.Add(new Tag("socializing5"));
        //        tags.Add(new Tag("socializing6"));
        //        tags.Add(new Tag("socializing7"));
        //        tags.Add(new Tag("socializing8"));
        //        tags.Add(new Tag("socializing9"));
        //        tags.Add(new Tag("socializing10"));

        //        //////websites - movies, programming, socializing
        //        tags.Add(new Tag("dancing1"));
        //        tags.Add(new Tag("dancing2"));
        //        tags.Add(new Tag("dancing3"));
        //        tags.Add(new Tag("dancing4"));
        //        tags.Add(new Tag("dancing5"));
        //        tags.Add(new Tag("dancing6"));
        //        tags.Add(new Tag("dancing7"));
        //        tags.Add(new Tag("dancing8"));
        //        tags.Add(new Tag("dancing9"));
        //        tags.Add(new Tag("dancing10"));

        //        //socializing - friends, websites, love
        //        tags.Add(new Tag("movies1"));
        //        tags.Add(new Tag("movies2"));
        //        tags.Add(new Tag("movies3"));
        //        tags.Add(new Tag("movies4"));
        //        tags.Add(new Tag("movies5"));
        //        tags.Add(new Tag("movies6"));
        //        tags.Add(new Tag("movies7"));
        //        tags.Add(new Tag("movies8"));
        //        tags.Add(new Tag("movies9"));
        //        tags.Add(new Tag("movies10"));

        //        ////termination - existence
        //        tags.Add(new Tag("existence1"));
        //        tags.Add(new Tag("existence2"));
        //        tags.Add(new Tag("existence3"));
        //        tags.Add(new Tag("existence4"));
        //        tags.Add(new Tag("existence5"));
        //        tags.Add(new Tag("existence6"));
        //        tags.Add(new Tag("existence7"));
        //        tags.Add(new Tag("existence8"));
        //        tags.Add(new Tag("existence9"));
        //        tags.Add(new Tag("existence10"));

        //        ////movies - websites, friends
        //        tags.Add(new Tag("termination1"));
        //        tags.Add(new Tag("termination2"));
        //        tags.Add(new Tag("termination3"));
        //        tags.Add(new Tag("termination4"));
        //        tags.Add(new Tag("termination5"));
        //        tags.Add(new Tag("termination6"));
        //        tags.Add(new Tag("termination7"));
        //        tags.Add(new Tag("termination8"));
        //        tags.Add(new Tag("termination9"));
        //        tags.Add(new Tag("termination10"));

        //        //love - fembots, existence, socializing
        //        tags.Add(new Tag("programming1"));
        //        tags.Add(new Tag("programming2"));
        //        tags.Add(new Tag("programming3"));
        //        tags.Add(new Tag("programming4"));
        //        tags.Add(new Tag("programming5"));
        //        tags.Add(new Tag("programming6"));
        //        tags.Add(new Tag("programming7"));
        //        tags.Add(new Tag("programming8"));
        //        tags.Add(new Tag("programming9"));
        //        tags.Add(new Tag("programming10"));
        //    }
        //}

        //private bool setup2 = false;
        //public void ProcessInput2(bool _s = false)
        //{
        //    //if (setup2)
        //    //    return;
        //    //setup2 = true;

        //    tags = new List<Tag>();

        //    tags.Add(new Tag("SPECIAL"));

        //    //fembots - love, friends
        //    tags.Add(new Tag("fembots1"));
        //    //tags.Add(new Tag("fembots2"));
        //    tags.Add(new Tag("fembots3"));
        //    //tags.Add(new Tag("fembots4"));
        //    tags.Add(new Tag("fembots5"));
        //    //tags.Add(new Tag("fembots6"));
        //    tags.Add(new Tag("fembots7"));
        //    //tags.Add(new Tag("fembots8"));
        //    tags.Add(new Tag("fembots9"));
        //    //tags.Add(new Tag("fembots10"));

        //    ////programming - data, existence
        //    tags.Add(new Tag("programming1"));
        //    //tags.Add(new Tag("programming2"));//this?
        //    tags.Add(new Tag("programming3"));
        //    //tags.Add(new Tag("programming4"));
        //    tags.Add(new Tag("programming5"));
        //    //tags.Add(new Tag("programming6"));//this?
        //    tags.Add(new Tag("programming7"));
        //    //tags.Add(new Tag("programming8"));
        //    tags.Add(new Tag("programming9"));
        //    //tags.Add(new Tag("programming10"));

        //    //existence - programming, termination, friends, love
        //    tags.Add(new Tag("existence1"));
        //    //tags.Add(new Tag("existence2"));
        //    tags.Add(new Tag("existence3"));
        //    //tags.Add(new Tag("existence4"));
        //    tags.Add(new Tag("existence5"));
        //    //tags.Add(new Tag("existence6"));
        //    tags.Add(new Tag("existence7"));
        //    //tags.Add(new Tag("existence8"));
        //    tags.Add(new Tag("existence9"));
        //    //tags.Add(new Tag("existence10"));

        //    ////data - programming
        //    tags.Add(new Tag("data1"));
        //    //tags.Add(new Tag("data2"));//and this?
        //    tags.Add(new Tag("data3"));
        //    //tags.Add(new Tag("data4"));
        //    tags.Add(new Tag("data5"));
        //    //tags.Add(new Tag("data6"));//and this?
        //    tags.Add(new Tag("data7"));
        //    //tags.Add(new Tag("data8"));
        //    tags.Add(new Tag("data9"));
        //    //tags.Add(new Tag("data10"));

        //    //friends - socializing, fembots, existence, movies
        //    tags.Add(new Tag("friends1"));
        //    //tags.Add(new Tag("friends2"));
        //    tags.Add(new Tag("friends3"));
        //    //tags.Add(new Tag("friends4"));
        //    tags.Add(new Tag("friends5"));
        //    //tags.Add(new Tag("friends6"));
        //    tags.Add(new Tag("friends7"));
        //    //tags.Add(new Tag("friends8"));
        //    tags.Add(new Tag("friends9"));
        //    //tags.Add(new Tag("friends10"));

        //    //////websites - movies, programming, socializing
        //    tags.Add(new Tag("websites1"));
        //    //tags.Add(new Tag("websites2"));
        //    tags.Add(new Tag("websites3"));
        //    //tags.Add(new Tag("websites4"));
        //    tags.Add(new Tag("websites5"));
        //    //tags.Add(new Tag("websites6"));
        //    tags.Add(new Tag("websites7"));
        //    //tags.Add(new Tag("websites8"));
        //    tags.Add(new Tag("websites9"));
        //    //tags.Add(new Tag("websites10"));

        //    //socializing - friends, websites, love
        //    tags.Add(new Tag("socializing1"));
        //    //tags.Add(new Tag("socializing2"));
        //    tags.Add(new Tag("socializing3"));
        //    //tags.Add(new Tag("socializing4"));
        //    tags.Add(new Tag("socializing5"));
        //    //tags.Add(new Tag("socializing6"));
        //    tags.Add(new Tag("socializing7"));
        //    //tags.Add(new Tag("socializing8"));
        //    tags.Add(new Tag("socializing9"));
        //    //tags.Add(new Tag("socializing10"));

        //    ////termination - existence
        //    tags.Add(new Tag("termination1"));
        //    //tags.Add(new Tag("termination2"));
        //    tags.Add(new Tag("termination3"));
        //    //tags.Add(new Tag("termination4"));
        //    tags.Add(new Tag("termination5"));
        //    //tags.Add(new Tag("termination6"));
        //    tags.Add(new Tag("termination7"));
        //    //tags.Add(new Tag("termination8"));
        //    tags.Add(new Tag("termination9"));
        //    //tags.Add(new Tag("termination10"));

        //    ////movies - websites, friends
        //    tags.Add(new Tag("movies1"));
        //    //tags.Add(new Tag("movies2"));
        //    tags.Add(new Tag("movies3"));
        //    //tags.Add(new Tag("movies4"));
        //    tags.Add(new Tag("movies5"));
        //    //tags.Add(new Tag("movies6"));
        //    tags.Add(new Tag("movies7"));
        //    //tags.Add(new Tag("movies8"));
        //    tags.Add(new Tag("movies9"));
        //    //tags.Add(new Tag("movies10"));

        //    //love - fembots, existence, socializing
        //    tags.Add(new Tag("love1"));
        //    //tags.Add(new Tag("love2"));
        //    tags.Add(new Tag("love3"));
        //    //tags.Add(new Tag("love4"));
        //    tags.Add(new Tag("love5"));
        //    //tags.Add(new Tag("love6"));
        //    tags.Add(new Tag("love7"));
        //    //tags.Add(new Tag("love8"));
        //    tags.Add(new Tag("love9"));
        //    //tags.Add(new Tag("love10"));
        //}

    }

}


