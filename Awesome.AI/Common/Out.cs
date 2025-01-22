using Awesome.AI.Common;
using Awesome.AI.Core;

namespace Awesome.AI.Web.AI.Common
{
    public class Out
    {
        TheMind mind;
        private Out() { }
        public Out(TheMind mind)
        {
            this.mind = mind;
        }

        public string cycles { get; set; }
        public string cycles_total { get; set; }
        public string momentum { get; set; }

        public string pain { get; set; }
        public string position { get; set; }
        public string ratio_yes { get; set; }
        public string ratio_no { get; set; }
        public string the_choise { get; set; }
        public string epochs { get; set; }
        public string runtime { get; set; }
        public string occu { get; set; }
        public string location { get; set; }
        public string state { get; set; }

        public UNIT common_unit { get; set; }
        public string common_hub { get; set; }

        public void Set()
        {
            cycles = $"{mind.cycles}";
            cycles_total = $"{mind.cycles_all}";
            momentum = $"{mind.parms._mech.dir.d_momentum}";

            pain = $"{mind.pain}";
            position = $"{mind.parms._mech.dir.d_pos_x}";
            ratio_yes = $"{mind.parms._mech.dir.ratio.CountYes()}";
            ratio_no = $"{mind.parms._mech.dir.ratio.CountNo()}";
            the_choise = $"{(mind.parms._mech.dir.Choise.IsNo() ? "NO" : "YES")}";
            epochs = $"{mind.epochs}";
            runtime = $"{mind.parms.runtime}";
            occu = $"{mind._internal.Occu}";
            location = $"{mind.loc.LocationFinal}";
            state = mind.loc.State > 0 ? "making a decision" : "just thinking";

            //List<UNIT> all = mind.mem.UNITS_ALL();

            common_unit = mind.process.most_common_unit;
            //UNIT common = all.Where(x=>x.root == common_word).FirstOrDefault();
            //if (common.IsNull())
                //throw new Exception();
            common_hub = common_unit.HUB.GetSubject();
        }
    }
}
