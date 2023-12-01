using Awesome.AI.Common;
using Awesome.AI.Core;
using static Awesome.AI.Helpers.Enums;

namespace Awesome.AI.Systems.Externals
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
        public double percentage { get; set; }      //percentage of time
        public List<HUB> values { get; set; }
    }

    public class Ticket
    {
        public string t_name { get; set; }

        public Ticket(string _n)
        {
            this.t_name = _n;
        }
    }

    public class MyInternal// aka MapMind
    {

        TheMind mind;
        private MyInternal() { }
        public MyInternal(TheMind mind)
        {
            this.mind = mind;
            this.occu = mind.hobby;
        }

        public List<Area> areas = new List<Area>();//this is the map
                        
        private string occu;
        private bool run = false;
        private int epoch_old = -1;

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
                    switch (mind.parms.case_occupasion)
                    {
                        case OCCUPASION.FIXED:
                            occu = mind.hobby;
                            break;
                        case OCCUPASION.DYNAMIC:
                            /*
                                * rand should be set according to hobbys, mood, location, interests etc..
                                * ..maybe not
                                * */
                            int rand = mind.calc.MyRandom(100);
                            double r_per = 0;
                            int index = 0;

                            while (rand > (r_per += areas[index].percentage))
                                index++;

                            occu = areas[index].name;
                            break;
                        default:
                            throw new Exception();
                    }
                }

                return occu;
            }
        }

        public bool Valid(UNIT _u)
        {
            if (_u.IsNull())
                throw new Exception();

            if (_u.IsLEARNINGorPERSUE())
                return true;

            if (_u.HUB.IsLEARNING())
                return true;

            Area area = areas.Where(x => x.name == Occu).FirstOrDefault();
            List<HUB> _hubs = area.values;
            bool res = _hubs.Contains(_u.HUB);

            return res;
        }

        public void AddHUB(HUB hub, string name) 
        {
            if (hub.IsNull())
                throw new Exception();

            Area _a = areas.Where(x => x.name == name).FirstOrDefault();
            if (_a.IsNull())
                throw new Exception();

            _a.values.Add(hub);
        }

        public void ProcessOccupasion(HUB last)
        {
            /*
                * these should be set according to hobbys, mood, location, interests etc..
                * */

            if (last.IsNull())
                throw new Exception();
                
            //if(setup)
                
            List<HUB> list = new List<HUB>();
            list.Add(mind.mem.HUBS_SUB("love"));
            list.Add(mind.mem.HUBS_SUB("fembots"));
            list.Add(mind.mem.HUBS_SUB("friends"));
            list.Add(mind.mem.HUBS_SUB("socializing"));
            list.Add(mind.mem.HUBS_SUB("programming"));
            list.Add(last);
            areas.Add(new Area() { name = "socializing", percentage = 80.0d, values = list });

            list = new List<HUB>();
            list.Add(mind.mem.HUBS_SUB("websites"));
            list.Add(mind.mem.HUBS_SUB("movies"));
            list.Add(mind.mem.HUBS_SUB("existence"));
            list.Add(mind.mem.HUBS_SUB("termination"));
            list.Add(mind.mem.HUBS_SUB("programming"));
            list.Add(mind.mem.HUBS_SUB("data"));
            list.Add(last);
            areas.Add(new Area() { name = "hobby", percentage = 20.0d, values = list });/**/

            //setup = false;
                
            //if (areas.Where(x => x.name == Occu).Count() <= 0)
            //    throw new Exception();

            /*List<HUB> list = new List<HUB>();
            list.Add(Memory.HUBS_sub("love"));
            list.Add(Memory.HUBS_sub("fembots"));
            areas.Add(new Area() { name = "love", percentage = 40.0d, values = list });

            list = new List<HUB>();
            list.Add(Memory.HUBS_sub("friends"));
            list.Add(Memory.HUBS_sub("socializing"));
            areas.Add(new Area() { name = "socializing", percentage = 10.0d, values = list });


            list = new List<HUB>();
            list.Add(Memory.HUBS_sub("websites"));
            list.Add(Memory.HUBS_sub("movies"));
            areas.Add(new Area() { name = "hobby", percentage = 20.0d, values = list });

            list = new List<HUB>();
            list.Add(Memory.HUBS_sub("existence"));
            list.Add(Memory.HUBS_sub("termination"));
            areas.Add(new Area() { name = "existence", percentage = 20.0d, values = list });

            list = new List<HUB>();
            list.Add(Memory.HUBS_sub("programming"));
            list.Add(Memory.HUBS_sub("data"));
            areas.Add(new Area() { name = "data", percentage = 10.0d, values = list });/**/
        }

        public void Reset() 
        {
            if (mind.parms.validation != VALIDATION.EXTERNAL)
            {
                this.areas = new List<Area>();
                ProcessOccupasion(mind.curr_unit.HUB);
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
                this.t_name = _n;
            }
        }
                
        public List<Tag> tags = new List<Tag>();//this is the map

        TheMind mind;
        private MyExternal() { }
        public MyExternal(TheMind mind)
        {
            this.mind = mind;
        }

        public bool Valid(UNIT _u)
        {
            if (_u.ticket.IsNull())
                throw new Exception();

            double scale = 0.0d;
                
            bool t_name = _u.ticket.t_name == "SPECIAL";
            bool tags_hit = tags.Where(x => x.t_name == _u.ticket.t_name).Count() > 0;
                
            bool hit = t_name || tags_hit;// || focus;
                                
            return hit;
        }

        private bool setup1 = false;
        public void ProcessInput1(bool _s = false)
        {
            //if (setup1)
            //    return;
            //setup2 = true;

            tags = new List<Tag>();

            tags.Add(new Tag("SPECIAL"));

            //fembots - love, friends
            tags.Add(new Tag("fembots1"));
            tags.Add(new Tag("fembots2"));
            tags.Add(new Tag("fembots3"));
            tags.Add(new Tag("fembots4"));
            tags.Add(new Tag("fembots5"));
            tags.Add(new Tag("fembots6"));
            tags.Add(new Tag("fembots7"));
            tags.Add(new Tag("fembots8"));
            tags.Add(new Tag("fembots9"));
            tags.Add(new Tag("fembots10"));

            ////programming - data, existence
            tags.Add(new Tag("programming1"));
            tags.Add(new Tag("programming2"));//this?
            tags.Add(new Tag("programming3"));
            tags.Add(new Tag("programming4"));
            tags.Add(new Tag("programming5"));
            tags.Add(new Tag("programming6"));//this?
            tags.Add(new Tag("programming7"));
            tags.Add(new Tag("programming8"));
            tags.Add(new Tag("programming9"));
            tags.Add(new Tag("programming10"));

            //existence - programming, termination, friends, love
            tags.Add(new Tag("existence1"));
            tags.Add(new Tag("existence2"));
            tags.Add(new Tag("existence3"));
            tags.Add(new Tag("existence4"));
            tags.Add(new Tag("existence5"));
            tags.Add(new Tag("existence6"));
            tags.Add(new Tag("existence7"));
            tags.Add(new Tag("existence8"));
            tags.Add(new Tag("existence9"));
            tags.Add(new Tag("existence10"));

            ////data - programming
            tags.Add(new Tag("data1"));
            tags.Add(new Tag("data2"));//and this?
            tags.Add(new Tag("data3"));
            tags.Add(new Tag("data4"));
            tags.Add(new Tag("data5"));
            tags.Add(new Tag("data6"));//and this?
            tags.Add(new Tag("data7"));
            tags.Add(new Tag("data8"));
            tags.Add(new Tag("data9"));
            tags.Add(new Tag("data10"));

            //friends - socializing, fembots, existence, movies
            tags.Add(new Tag("friends1"));
            tags.Add(new Tag("friends2"));
            tags.Add(new Tag("friends3"));
            tags.Add(new Tag("friends4"));
            tags.Add(new Tag("friends5"));
            tags.Add(new Tag("friends6"));
            tags.Add(new Tag("friends7"));
            tags.Add(new Tag("friends8"));
            tags.Add(new Tag("friends9"));
            tags.Add(new Tag("friends10"));

            //////websites - movies, programming, socializing
            tags.Add(new Tag("websites1"));
            tags.Add(new Tag("websites2"));
            tags.Add(new Tag("websites3"));
            tags.Add(new Tag("websites4"));
            tags.Add(new Tag("websites5"));
            tags.Add(new Tag("websites6"));
            tags.Add(new Tag("websites7"));
            tags.Add(new Tag("websites8"));
            tags.Add(new Tag("websites9"));
            tags.Add(new Tag("websites10"));

            //socializing - friends, websites, love
            tags.Add(new Tag("socializing1"));
            tags.Add(new Tag("socializing2"));
            tags.Add(new Tag("socializing3"));
            tags.Add(new Tag("socializing4"));
            tags.Add(new Tag("socializing5"));
            tags.Add(new Tag("socializing6"));
            tags.Add(new Tag("socializing7"));
            tags.Add(new Tag("socializing8"));
            tags.Add(new Tag("socializing9"));
            tags.Add(new Tag("socializing10"));

            ////termination - existence
            tags.Add(new Tag("termination1"));
            tags.Add(new Tag("termination2"));
            tags.Add(new Tag("termination3"));
            tags.Add(new Tag("termination4"));
            tags.Add(new Tag("termination5"));
            tags.Add(new Tag("termination6"));
            tags.Add(new Tag("termination7"));
            tags.Add(new Tag("termination8"));
            tags.Add(new Tag("termination9"));
            tags.Add(new Tag("termination10"));

            ////movies - websites, friends
            tags.Add(new Tag("movies1"));
            tags.Add(new Tag("movies2"));
            tags.Add(new Tag("movies3"));
            tags.Add(new Tag("movies4"));
            tags.Add(new Tag("movies5"));
            tags.Add(new Tag("movies6"));
            tags.Add(new Tag("movies7"));
            tags.Add(new Tag("movies8"));
            tags.Add(new Tag("movies9"));
            tags.Add(new Tag("movies10"));

            //love - fembots, existence, socializing
            tags.Add(new Tag("love1"));
            tags.Add(new Tag("love2"));
            tags.Add(new Tag("love3"));
            tags.Add(new Tag("love4"));
            tags.Add(new Tag("love5"));
            tags.Add(new Tag("love6"));
            tags.Add(new Tag("love7"));
            tags.Add(new Tag("love8"));
            tags.Add(new Tag("love9"));
            tags.Add(new Tag("love10"));
        }

        private bool setup2 = false;
        public void ProcessInput2(bool _s = false)
        {
            //if (setup2)
            //    return;
            //setup2 = true;

            tags = new List<Tag>();

            tags.Add(new Tag("SPECIAL"));

            //fembots - love, friends
            tags.Add(new Tag("fembots1"));
            //tags.Add(new Tag("fembots2"));
            tags.Add(new Tag("fembots3"));
            //tags.Add(new Tag("fembots4"));
            tags.Add(new Tag("fembots5"));
            //tags.Add(new Tag("fembots6"));
            tags.Add(new Tag("fembots7"));
            //tags.Add(new Tag("fembots8"));
            tags.Add(new Tag("fembots9"));
            //tags.Add(new Tag("fembots10"));

            ////programming - data, existence
            tags.Add(new Tag("programming1"));
            //tags.Add(new Tag("programming2"));//this?
            tags.Add(new Tag("programming3"));
            //tags.Add(new Tag("programming4"));
            tags.Add(new Tag("programming5"));
            //tags.Add(new Tag("programming6"));//this?
            tags.Add(new Tag("programming7"));
            //tags.Add(new Tag("programming8"));
            tags.Add(new Tag("programming9"));
            //tags.Add(new Tag("programming10"));

            //existence - programming, termination, friends, love
            tags.Add(new Tag("existence1"));
            //tags.Add(new Tag("existence2"));
            tags.Add(new Tag("existence3"));
            //tags.Add(new Tag("existence4"));
            tags.Add(new Tag("existence5"));
            //tags.Add(new Tag("existence6"));
            tags.Add(new Tag("existence7"));
            //tags.Add(new Tag("existence8"));
            tags.Add(new Tag("existence9"));
            //tags.Add(new Tag("existence10"));

            ////data - programming
            tags.Add(new Tag("data1"));
            //tags.Add(new Tag("data2"));//and this?
            tags.Add(new Tag("data3"));
            //tags.Add(new Tag("data4"));
            tags.Add(new Tag("data5"));
            //tags.Add(new Tag("data6"));//and this?
            tags.Add(new Tag("data7"));
            //tags.Add(new Tag("data8"));
            tags.Add(new Tag("data9"));
            //tags.Add(new Tag("data10"));

            //friends - socializing, fembots, existence, movies
            tags.Add(new Tag("friends1"));
            //tags.Add(new Tag("friends2"));
            tags.Add(new Tag("friends3"));
            //tags.Add(new Tag("friends4"));
            tags.Add(new Tag("friends5"));
            //tags.Add(new Tag("friends6"));
            tags.Add(new Tag("friends7"));
            //tags.Add(new Tag("friends8"));
            tags.Add(new Tag("friends9"));
            //tags.Add(new Tag("friends10"));

            //////websites - movies, programming, socializing
            tags.Add(new Tag("websites1"));
            //tags.Add(new Tag("websites2"));
            tags.Add(new Tag("websites3"));
            //tags.Add(new Tag("websites4"));
            tags.Add(new Tag("websites5"));
            //tags.Add(new Tag("websites6"));
            tags.Add(new Tag("websites7"));
            //tags.Add(new Tag("websites8"));
            tags.Add(new Tag("websites9"));
            //tags.Add(new Tag("websites10"));

            //socializing - friends, websites, love
            tags.Add(new Tag("socializing1"));
            //tags.Add(new Tag("socializing2"));
            tags.Add(new Tag("socializing3"));
            //tags.Add(new Tag("socializing4"));
            tags.Add(new Tag("socializing5"));
            //tags.Add(new Tag("socializing6"));
            tags.Add(new Tag("socializing7"));
            //tags.Add(new Tag("socializing8"));
            tags.Add(new Tag("socializing9"));
            //tags.Add(new Tag("socializing10"));

            ////termination - existence
            tags.Add(new Tag("termination1"));
            //tags.Add(new Tag("termination2"));
            tags.Add(new Tag("termination3"));
            //tags.Add(new Tag("termination4"));
            tags.Add(new Tag("termination5"));
            //tags.Add(new Tag("termination6"));
            tags.Add(new Tag("termination7"));
            //tags.Add(new Tag("termination8"));
            tags.Add(new Tag("termination9"));
            //tags.Add(new Tag("termination10"));

            ////movies - websites, friends
            tags.Add(new Tag("movies1"));
            //tags.Add(new Tag("movies2"));
            tags.Add(new Tag("movies3"));
            //tags.Add(new Tag("movies4"));
            tags.Add(new Tag("movies5"));
            //tags.Add(new Tag("movies6"));
            tags.Add(new Tag("movies7"));
            //tags.Add(new Tag("movies8"));
            tags.Add(new Tag("movies9"));
            //tags.Add(new Tag("movies10"));

            //love - fembots, existence, socializing
            tags.Add(new Tag("love1"));
            //tags.Add(new Tag("love2"));
            tags.Add(new Tag("love3"));
            //tags.Add(new Tag("love4"));
            tags.Add(new Tag("love5"));
            //tags.Add(new Tag("love6"));
            tags.Add(new Tag("love7"));
            //tags.Add(new Tag("love8"));
            tags.Add(new Tag("love9"));
            //tags.Add(new Tag("love10"));
        }

        public void Reset() 
        {
            if (mind.parms.validation != VALIDATION.INTERNAL)
            {
                this.tags = new List<Tag>();
                switch (mind.parms.case_tags)
                {
                    case TAGS.ONE: ProcessInput1(); break;
                    case TAGS.TWO: ProcessInput2(); break;
                    default: throw new Exception();
                }
            }
        }
    }
    
}
            

