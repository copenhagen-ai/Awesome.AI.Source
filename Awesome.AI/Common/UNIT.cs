using Awesome.AI.Core;
using Awesome.AI.Interfaces;
using Awesome.AI.Systems.Externals;
using static Awesome.AI.Helpers.Enums;

namespace Awesome.AI.Common
{
    public class UNIT
    {
        /*
         * maybe later there will be other types, like: SOUND, IMAGE, VIDEO
         * */

        public Ticket ticket = new Ticket("NOTICKET");
        private TYPE type { get; set; }
        public string root { get; set; }
        public string root_val { get; set; }
        public double max_nrg { get; set; }
        public double energy { get; set; }
        public int visited { get; set; }

        TheMind mind;
        private UNIT() { }
        public UNIT(TheMind mind)
        {
            this.mind = mind;
        }

        public double index_conv
        {
            get
            {
                double orig = index_orig;
                double res = mind.convert.Process(orig, root, type);

                return res;
            }
        }

        private double dex_orig = -1.0d;
        public double index_orig
        {
            get { return dex_orig; }
            set { dex_orig = value; }
        }

        double _f = -1d;
        public double Variable
        {
            get 
            {
                if (_f != -1d)
                    return _f;

                IBaseVariable _base = mind.parms.GetBaseForce();
                _f = _base.Variable(this);
                return _f;
            }
        }

        public bool IsValid
        {
            get
            {
                switch (mind.parms.validation)
                {
                    case VALIDATION.BOTH:
                        return mind._internal.Valid(this) && mind._external.Valid(this);
                    case VALIDATION.EXTERNAL:
                        return mind._external.Valid(this);
                    case VALIDATION.INTERNAL:
                        return mind._internal.Valid(this);
                    default:
                        throw new Exception();
                }
            }
        }

        public double LengthFromZero
        {
            get
            {
                double min = ZUNIT.zero_dist < this.LowAtZero ? ZUNIT.zero_dist : this.LowAtZero;
                double max = ZUNIT.zero_dist > this.LowAtZero ? ZUNIT.zero_dist : this.LowAtZero;
                return max - min;
            }
        }
        
        public bool IsHighPass
        {
            get { return mind.filters.HighPass(this); }
        }

        public bool IsEnergy
        {
            get { return mind.filters.Energy(this); }
        }

        UNIT next = null;
        public UNIT Next
        {
            get
            {
                if (!next.IsNull())
                    return next;

                List<UNIT> units = mind.mem.UNITS_VAL();
                units = units.OrderByDescending(x => x.Variable).ToList();
                units = units.Where(x => !x.IsSPECIAL()).ToList();
                units = units.Where(x => x.IsHighPass).ToList();
                units = units.Where(x => x.Variable < this.Variable).ToList();

                next = units.FirstOrDefault();
                if (next.IsNull())
                    next = new UNIT() { type = TYPE.NOTNEXTPREV };

                return next;
            }
        }

        UNIT prev = null;
        public UNIT Prev
        {
            get
            {
                if (!prev.IsNull())
                    return prev;

                List<UNIT> units = mind.mem.UNITS_VAL();
                units = units.OrderByDescending(x => x.Variable).ToList();
                units = units.Where(x => !x.IsSPECIAL()).ToList();
                units = units.Where(x => x.IsHighPass).ToList();
                units = units.Where(x => x.Variable > this.Variable).ToList();

                prev = units.LastOrDefault();
                if (prev.IsNull())
                    prev = new UNIT() { type = TYPE.NOTNEXTPREV };

                return prev;
            }
        }

        /*
         * used to be Distance
         * used in WorkWithWheelAndContest
         * */
        public double LowAtZero
        {
            get
            {
                double max = mind.convert.p_Max;
                double min = mind.convert.p_Min;

                double res = index_conv;
                res = mind.calc.NormalizeRange(res, 0.0d, 100.0d, 0.0d, mind.parms.max_index);

                return res;
            }
        }

        /*
         * used to be Mass
         * used in WorksWithHill
         * */
        public double HighAtZero
        {
            get
            {
                double max = mind.convert.p_Max;
                double min = mind.convert.p_Min;

                double res = max - index_conv;
                res = mind.calc.NormalizeRange(res, 0.0d, 100.0d, 0.0d, mind.parms.max_index);

                return res;
            }
        }

        public double LowAtZeroReciprocal
        {
            get
            {
                double max = mind.convert.p_Max;
                double min = mind.convert.p_Min;

                double res = mind.calc.NormalizeRange(index_conv, 0.0d, 100.0d, 0.0d, mind.parms.scale);
                res = res < 0.25d ? 0.25d : res;
                res = mind.calc.Reciprocal(res);
                res = res > 2.0d ? 2.0d : res;
                res = mind.calc.NormalizeRange(res, 0.0d, 2.0d, 0.0d, mind.parms.max_index);

                return res;
            }
        }

        public double HighAtZeroReciprocal
        {
            get
            {
                double max = mind.convert.p_Max;
                double min = mind.convert.p_Min;

                double res = mind.calc.NormalizeRange(max - index_conv, 0.0d, 100.0d, 0.0d, mind.parms.scale);
                res = res < 0.25d ? 0.25d : res;
                res = mind.calc.Reciprocal(res);
                res = res > 2.0d ? 2.0d : res;
                res = mind.calc.NormalizeRange(res, 0.0d, 2.0d, 0.0d, mind.parms.max_index);

                return res;
            }
        }

        private HUB hub = null;
        public HUB HUB
        {
            get
            {
                if (hub != null)
                    return hub;

                hub = mind.mem.HUBS_ALL().Where(x => x.units.Contains(this)).FirstOrDefault();
                if (hub == null)
                    return HUB.Create("IDLE", new List<UNIT>(), mind.parms.is_accord, new int?[] { 1, 1, 1 }, 0.0d, 0.0d);

                return hub;
            }
        }

        private List<UNIT> rel = null;
        public List<UNIT> REL
        {
            get
            {
                if (rel != null)
                    return rel;

                rel = HUB.units;

                return rel;
            }
        }


        public static UNIT Create(TheMind mind, double index_orig, string root, string root_val, string ticket, TYPE t)
        {
            UNIT _w = new UNIT() { mind = mind, index_orig = index_orig, root = root, root_val = root_val, type = t };

            if (ticket != "")
                _w.ticket = new Ticket(ticket);

            _w.max_nrg = mind.parms.max_nrg;
            _w.energy = mind.parms.max_nrg;

            return _w;
        }

        public static UNIT IDLE_UNIT(TheMind mind)
        {
            return UNIT.Create(mind, -1d, "XXXX", "XXXX", "", TYPE.IDLE);
        }

        public bool IsUNIT()
        {
            return
                type == TYPE.JUSTAUNIT;
        }
        public bool IsIDLE()
        {
            return
                type == TYPE.IDLE;
        }
        public bool IsLEARNING()
        {
            return
                type == TYPE.LEARNING;
        }
        public bool IsPERSUE()
        {
            return
                type == TYPE.PERSUE;
        }
        public bool IsNEXTPREV()
        {
            return
                type != TYPE.NOTNEXTPREV;
        }
        public bool IsLEARNINGorPERSUE()
        {
            return
                type == TYPE.LEARNING ||
                type == TYPE.PERSUE;
        }
        public bool IsSPECIAL()
        {
            return
                type == TYPE.IDLE ||
                type == TYPE.LEARNING ||
                type == TYPE.PERSUE;
        }        
    }
}
