using Awesome.AI.Common;
using Awesome.AI.Core;
using Awesome.AI.Helpers;

namespace Awesome.AI.Systems
{
    public class Location
    {
        public string LocationFinal { get; set; }
        public int LocationState { get; set; }

        private TheMind mind;
        private Location() { }

        public Location(TheMind mind, string starting)
        {
            this.mind = mind;
            LocationState = 0;
            LocationFinal = starting.Replace("WHAT", "");
        }

        public void Decide(bool _pro)
        {
            /*
             * OCCU = [ LivingroomHUB, KitchenHUB, BedroomHUB ]
             * if OCCU == DecisionHUB
             *     if USTAT == "NO"
             *         OCCU = [ LivingroomHUB, KitchenHUB, BedroomHUB ]
             *     if USTAT == "YES"
             *         OCCU = [ PosibillitiesHUB ]
             * else if OCCU == PosibillitiesHUB
             *     move to USTAT.data
             *     OCCU = [ LivingroomHUB, KitchenHUB, BedroomHUB ]
             * else
             *     if USTAT == "move location"
             *         OCCU = [ DecisionHUB ]
             *     else
             *         OCCU = OCCU
             */

            if (!_pro)
                return;

            if (mind.epochs < 5)
                return;

            if (!mind.curr_unit.IsDECISION())
                return;

            UNIT current = mind.curr_unit;
            HUB hub = current.HUB;

            List<UNIT> units = mind.mem.UNITS_ALL().Where(x => x.IsDECISION()).ToList();
            HUB _1 = mind.mem.HUBS_SUB(mind.parms.state, Constants.subject_decision[0]);
            HUB _2 = mind.mem.HUBS_SUB(mind.parms.state, Constants.subject_decision[1]);

            MyRandom rand = mind.rand;
            int[] _rand = rand.MyRandomInt(1, 5);
            if (_rand[0] == 1)
            {
                mind.mem.Randomize(_1);
                mind.mem.Randomize(_2);
            }

            if (hub.subject == "location_should_decision" && LocationState == 0)
            {
                if (current.data == "SHOULDYES")
                    LocationState++;

                if (current.data == "SHOULDNO")
                    LocationState = 0;
            }

            if (hub.subject == "location_what_decision" && LocationState == 1)
            {
                LocationFinal = mind.dir.DownHard.IsNo() ? current.data : LocationFinal;
                LocationFinal = LocationFinal.Replace("WHAT", "");
                LocationState = 0;
            }

            //if (hub.subject == "should_decision" && State == 0)
            //{
            //    if (current.data == "SHOULDYES")
            //        State++;

            //    if (current.data == "SHOULDNO")
            //        State = 0;
            //}

            //if (hub.subject == "what_decision" && State == 1)
            //{
            //    LocationTmp = current.data;
            //    State++;
            //    return "make_decision";
            //}

            //if (hub.subject == "make_decision" && State == 2)
            //{
            //    if (current.data == "MAKEYES")
            //        LocationFinal = LocationTmp.Replace("WHAT", "");

            //    if (current.data == "MAKENO")
            //        LocationTmp = "";

            //    mind._internal.epoch_stop = -1;
            //    mind._internal.epoch_count = 0;
            //    State = 0;

            //    return "";
            //}
        }

        //private UNIT Random(string subject)
        //{
        //    HUB hub = mind.mem.HUBS_SUB(subject);

        //    List<UNIT> units = hub.units.Where(x =>
        //                        mind.filters.Credits(x)
        //                        && !mind.filters.LowCut(x)
        //                        ).OrderByDescending(x => x.Variable).ToList();

        //    int rand = mind.calc.MyRandom(units.Count - 1);

        //    if (!units.Any())
        //        throw new Exception();

        //    UNIT __u = units[rand];
        //    return __u;
        //}
    }
}
