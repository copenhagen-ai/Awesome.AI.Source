using Awesome.AI.CoreInternals;
using Awesome.AI.Interfaces;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core
{
    public class UNIT
    {
        /*
         * maybe later there will be other types, like: SOUND, IMAGE, VIDEO
         * */

        public Ticket ticket = new Ticket("NOTICKET");
        private UNITTYPE unit_type { get; set; }
        public LONGTYPE long_deci_type { get; set; }
        public string root { get; set; }//name
        public string data { get; set; }//data
        public double credits { get; set; }

        private TheMind mind;
        private UNIT() { }
        public UNIT(TheMind mind)
        {
            this.mind = mind;
        }

        private double dex = -1.0d;
        public double Index
        {
            get { return dex; }
            set { dex = value; }
        }

        double _f = -1d;
        public double Variable
        {
            get
            {
                if (root == "_decision42")
                    ;

                if (_f != -1d)
                    return _f;

                IMechanics _mech = mind.mech[mind.current];
                _f = _mech.Variable(this);
                return _f;
            }
        }

        public bool IsValid
        {
            get
            {
                switch (mind.parms[mind.current].validation)
                {
                    case VALIDATION.BOTH:
                        return mind._internal.Valid(this) && mind._external.Valid(this);
                    case VALIDATION.EXTERNAL:
                        return mind._external.Valid(this);
                    case VALIDATION.INTERNAL:
                        return mind._internal.Valid(this);
                    default:
                        throw new Exception("IsValid");
                }
            }
        }

        public bool IsLowCut
        {
            get { return mind.filters.LowCut(this); }
        }

        public bool CreditOK
        {
            get { return mind.filters.Credits(this); }
        }

        /*
         * used to be Distance
         * used in WorkWithWheelAndContest
         * */
        public double LowAtZero
        {
            get
            {
                double res = Index;
                //res = mind.calc.NormalizeRange(res, 0.0d, 100.0d, 0.0d, 100.0d);

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
                double res = 100.0d - Index;
                //res = mind.calc.NormalizeRange(res, 0.0d, 100.0d, 0.0d, 100.0d);

                return res;
            }
        }

        private HUB hub = null;
        public HUB HUB
        {
            get
            {
                //if (hub != null)
                //    return hub;

                if (IsIDLE())
                    return HUB.Create("IDLE", new List<UNIT>(), TONE.RANDOM);

                hub = mind.mem.HUBS_ALL(mind.parms[mind.current].state).Where(x => x.units.Contains(this)).FirstOrDefault();

                if (hub == null)
                    return HUB.Create("IDLE", new List<UNIT>(), TONE.RANDOM);

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

        public static UNIT Create(TheMind mind, double index, string root, string data, string ticket, UNITTYPE ut, LONGTYPE lt)
        {
            UNIT _w = new UNIT() { mind = mind, Index = index, root = root, data = data, unit_type = ut, long_deci_type = lt };

            if (ticket != "")
                _w.ticket = new Ticket(ticket);

            _w.credits = Constants.MAX_CREDIT;

            return _w;
        }

        public static UNIT GetHigh
        {
            get
            {
                return Create(null, Constants.MAX, "MAX", "DATA", "TICKET", UNITTYPE.MAX, LONGTYPE.NONE);
            }
        }
        public static UNIT GetLow
        {
            get
            {
                return Create(null, Constants.MIN, "MIN", "DATA", "TICKET", UNITTYPE.MIN, LONGTYPE.NONE);
            }
        }

        public static UNIT IDLE_UNIT(TheMind mind)
        {
            return Create(mind, -1d, "XXXX", "XXXX", "", UNITTYPE.IDLE, LONGTYPE.NONE);
        }

        public bool IsUNIT() => unit_type == UNITTYPE.JUSTAUNIT;

        public bool IsIDLE() => unit_type == UNITTYPE.IDLE;

        public bool IsDECISION() => unit_type == UNITTYPE.DECISION;

        public bool IsQUICKDECISION() => unit_type == UNITTYPE.QDECISION;

        

        //UNIT next = null;
        //public UNIT Next
        //{
        //    get
        //    {
        //        if (!next.IsNull())
        //            return next;

        //        List<UNIT> units = mind.mem.UNITS_VAL();
        //        units = units.OrderByDescending(x => x.Variable).ToList();
        //        units = units.Where(x => !x.IsSPECIAL()).ToList();
        //        units = units.Where(x => !x.IsLowCut).ToList();
        //        units = units.Where(x => x.Variable < this.Variable).ToList();

        //        next = units.FirstOrDefault();
        //        if (next.IsNull())
        //            next = new UNIT() { type = TYPE.NOTNEXTPREV };

        //        return next;
        //    }
        //}

        //UNIT prev = null;
        //public UNIT Prev
        //{
        //    get
        //    {
        //        if (!prev.IsNull())
        //            return prev;

        //        List<UNIT> units = mind.mem.UNITS_VAL();
        //        units = units.OrderByDescending(x => x.Variable).ToList();
        //        units = units.Where(x => !x.IsSPECIAL()).ToList();
        //        units = units.Where(x => !x.IsLowCut).ToList();
        //        units = units.Where(x => x.Variable > this.Variable).ToList();

        //        prev = units.LastOrDefault();
        //        if (prev.IsNull())
        //            prev = new UNIT() { type = TYPE.NOTNEXTPREV };

        //        return prev;
        //    }
        //}

        //public bool IsLEARNING()
        //{
        //    return
        //        type == TYPE.LEARNING;
        //}

        //public bool IsPERSUE()
        //{
        //    return
        //        type == TYPE.PERSUE;
        //}

        //public bool IsNEXTPREV()
        //{
        //    return
        //        type != TYPE.NOTNEXTPREV;
        //}

        //public bool IsLEARNINGorPERSUE()
        //{
        //    return
        //        type == TYPE.LEARNING ||
        //        type == TYPE.PERSUE;
        //}

        //public bool IsSPECIAL()
        //{
        //    return
        //        type == TYPE.IDLE ||
        //        type == TYPE.LEARNING ||
        //        type == TYPE.PERSUE;
        //}

    }
}
