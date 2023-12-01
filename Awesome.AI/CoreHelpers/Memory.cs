using Awesome.AI.Common;
using Awesome.AI.Core;
using Awesome.AI.Web.Helpers;
using System.Globalization;
using System.Xml.Linq;
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

        private List<HUB> hubs { get; set; }        
        
        public List<UNIT> learning = new List<UNIT>();

        TheMind mind;
        private Memory() { }
        
        public Memory(TheMind mind) 
        {
            this.mind = mind;
            hubs = new List<HUB>();
            Setup();            

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
        private List<UNIT> all = null;
        private void Reset()
        {
            if (_c == mind.cycles_all)
                return;

            if (mind.cycles_all % 10000 == 0)
                all = null;

            _c = mind.cycles_all;
        }

        public List<UNIT> UNITS_ALL()
        {
            Reset();

            if (!all.IsNull())
                return all;

            all = new List<UNIT>();
            List<HUB> hubs = HUBS_ALL();
            foreach (HUB h in hubs)
                all = all.Concat(h.units).ToList();

            all = all.OrderBy(x => x.index_orig).ToList();

            return all;
        }

        public List<UNIT> UNITS_VAL()
        {
            Reset();

            if (all.IsNull())
                UNITS_ALL();

            List<UNIT> res = all.Where(x => x.IsValid).ToList();

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
            int rand = mind.calc.MyRandom(hubs.Count() - 1);
            HUB _h = hubs.ToList()[rand];
            return _h;
        }

        public HUB HUBS_SUB(string subject)
        {
            if (subject == null)
                throw new ArgumentNullException();

            HUB _h = hubs.Where(x=>x.GetSubject() == subject).FirstOrDefault();
            if (_h == null)
                return null;
            return _h;
        }

        public void HUBS_ADD(HUB h)
        {
            if (h == null)
                throw new ArgumentNullException();

            hubs.Add(h);
            hubs = hubs.OrderBy(x=>x.GetSubject()).ToList();
        }

        private void Setup()
        {
            List<UNIT> all_units = new List<UNIT>();
            SetupUnits(all_units);
            SetupHubs(all_units);
        }

        public void SetupUnits(List<UNIT> all_units)
        {
            

            XElement xdoc;
            if (mind.parms.setup_tags == TAGSETUP.PRIME)
                xdoc = XElement.Load(PathSetup.MyPath);
            else
                throw new Exception();

            List<XElement> xunits = xdoc.Element("words").Elements().ToList();
            string previous = "prev";
            foreach (XElement xw in xunits)
            {
                if (previous != xw.Attribute("root").Value)
                {
                    all_units.Add(
                    UNIT.Create(
                        mind,
                        double.Parse(xw.Attribute("index_x").Value, CultureInfo.InvariantCulture),
                        xw.Attribute("root").Value,
                        "null",
                        xw.Attribute("ticket").Value,
                        TYPE.JUSTAUNIT
                        ));
                }
                previous = xw.Attribute("root").Value;
            }
        }

        public void SetupHubs(List<UNIT> all_units) 
        {
            XElement xdoc;
            if (mind.parms.setup_tags == TAGSETUP.PRIME)
                xdoc = XElement.Load(PathSetup.MyPath);
            else
                throw new Exception();

            List<XElement> xhubs = xdoc.Element("hubs").Elements().ToList();
            foreach (XElement el in xhubs)
            {
                List<XElement> xws = xhubs.Where(x => x.Attribute("val").Value == el.Attribute("val").Value).Elements("ws").ToList();
                List<UNIT> units = new List<UNIT>();
                foreach (XElement xw in xws)
                    units.Add(all_units.Where(x => x.root == xw.Attribute("val").Value).FirstOrDefault());

                HUB h = HUB.Create(el.Attribute("val").Value, units, true, null, -1.0d, -1.0d);

                HUBS_ADD(h);
            }
        }        
    }
}
