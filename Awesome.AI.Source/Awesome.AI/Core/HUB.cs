using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core
{
    public class HUB
    {
        /*
         * maybe HUB should be renamed GROUP
         * */

        public string hub_guid { get; set; }
        public string subject { get; set; }
        public int max_num_units { get; set; }
        public List<UNIT> units { get; set; }        
        public TONE tone { get; set; }

        private HUB(string hub_guid, string subject, List<UNIT> units, TONE ton, int max_num_units)
        {
            //CreateNet(is_accord, neurons, learningrate, momentum);

            this.hub_guid = hub_guid;
            this.subject = subject;
            this.units = units;
            this.tone = ton;
            this.max_num_units = max_num_units;
        }

        private HUB()
        {
        }

        public static HUB Create(string hub_guid, string subject, List<UNIT> units, TONE ton, int max_num_units)
        {
            HUB h = new HUB(hub_guid, subject, units, ton, max_num_units);
            return h;
        }

        public void AddUnit(UNIT u)
        {
            units ??= new List<UNIT>();

            units.Add(u);
        }


        public bool IsIDLE()
        {
            return subject == "IDLE";
        }        
    }
}
