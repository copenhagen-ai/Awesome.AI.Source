using Awesome.AI.Core;
using Awesome.AI.CoreInternals;

namespace Awesome.AI.Core.Spaces
{
    public class HubSpace
    {
        /*
         * these are groups of UNITs
         * */

        public string u_guid { get; set; }

        private TheMind mind;
        private HubSpace(TheMind mind, string u_guid)
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

                UNIT unit = mind.access.UNIT_GUID(u_guid);

                Lookup lookup = new Lookup();

                string occu = mind._internal.Occu.name;

                string hub = lookup.GetSUB(mind.mindtype, occu, unit.HubIndex);

                return hub;
            }
        }

        public List<UNIT> Units
        {
            get
            {
                UNIT unit = mind.access.UNIT_GUID(u_guid);

                Lookup lookup = new Lookup();

                string occu = mind._internal.Occu.name;

                List<UNIT> units = lookup.GetUNITS(mind, mind.mindtype, occu, unit.HubIndex);

                return units;
            }
        }

        public static HubSpace Create(TheMind mind, string u_guid)
        {
            HubSpace h = new HubSpace(mind, u_guid);
            return h;
        }
    }
}
