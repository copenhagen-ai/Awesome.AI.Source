using Awesome.AI.Source.Awesome.AI.Common;

namespace Awesome.AI.Core
{
    public class HUB_SPACE
    {
        /*
         * these are groups of UNITs
         * */

        public string u_guid { get; set; }

        private TheMind mind;
        private HUB_SPACE(TheMind mind, string u_guid)
        {
            this.mind = mind;
            this.u_guid = u_guid;            
        }

        public string Subject 
        {
            get
            {
                /*
                 * these are named axis
                 * */

                UNIT unit = mind.mem.UNIT_GUID(u_guid);

                Lookup lookup = new Lookup();

                string occu = mind._internal.Occu.name;

                string hub = lookup.GetHUB(mind.mindtype, occu, unit.HubIndex);

                return hub;
            }
        }

        public List<UNIT> Units
        {
            get
            {
                UNIT unit = mind.mem.UNIT_GUID(u_guid);

                Lookup lookup = new Lookup();

                string occu = mind._internal.Occu.name;

                List<UNIT> units = lookup.GetUNITS(mind, mind.mindtype, occu, unit.HubIndex);

                return units;
            }
        }

        public static HUB_SPACE Create(TheMind mind, string u_guid)
        {
            HUB_SPACE h = new HUB_SPACE(mind, u_guid);
            return h;
        }        
    }
}
