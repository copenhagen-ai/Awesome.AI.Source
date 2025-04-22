using Awesome.AI.Common;
using Awesome.AI.CoreInternals;
using Awesome.AI.Variables;
using Microsoft.VisualBasic;
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
        public DateTime created {  get; set; }
        public string hub_guid { get; set; }//name
        public string data { get; set; }//data
        public double credits { get; set; }
        //public string root { get; set; }//name

        private TheMind mind;
        private UNIT() { }
        public UNIT(TheMind mind)
        {
            this.mind = mind;
        }

        public string Root
        {
            get
            {
                HUB hub = HUB;
                List<UNIT> list = hub.units.OrderBy(x => x.created).ToList();
                int idx = list.IndexOf(this) + 1;
                string res = "_" + hub.subject + idx;

                return res;
            }
        }

        private double dex = -1.0d;
        public double Index
        {
            get { return dex; }
            set { dex = value; }
        }

        public double Variable
        {
            get
            {
                bool high_at_zero = mind.parms[mind.current].high_at_zero;

                return high_at_zero ? HighAtZero : LowAtZero;
            }
        }

        public double LowAtZero
        {
            get { return Index; }
        }

        public double HighAtZero
        {
            get { return 100.0d - Index; }
        }

        public static UNIT GetHigh
        {
            get { return Create(null, "GUID", CONST.MAX, "MAX", "NONE", UNITTYPE.MAX, LONGTYPE.NONE); }
        }

        public static UNIT GetLow
        {
            get { return Create(null, "GUID", CONST.MIN, "MIN", "NONE", UNITTYPE.MIN, LONGTYPE.NONE); }
        }

        public bool IsLowCut
        {
            get { return mind.filters.LowCut(this); }
        }

        public bool CreditOK
        {
            get { return mind.filters.Credits(this); }
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

        public HUB HUB
        {
            get
            {
                if (IsIDLE())
                    return HUB.Create("GUID", "IDLE", new List<UNIT>(), TONE.RANDOM, -1);

                STATE state = mind.State;
                List<HUB> list = mind.mem.HUBS_ALL(state);

                //FirstOrDefault is fine
                return list.Where(x => x.hub_guid == this.hub_guid).FirstOrDefault();                
            }
        }

        public List<UNIT> REL
        {
            get
            {
                List<UNIT> rel = HUB.units;

                return rel;
            }
        }

        public static UNIT Create(TheMind mind, string h_guid, double index, string data, string ticket, UNITTYPE ut, LONGTYPE lt)
        {
            //make sure some time has gone before creating a new unit
            "hello world!".BusyWait(10);

            DateTime create = DateTime.Now;

            UNIT _w = new UNIT() { mind = mind, created = create, hub_guid = h_guid, Index = index, data = data, unit_type = ut, long_deci_type = lt };

            if (ticket != "")
                _w.ticket = new Ticket(ticket);

            _w.credits = CONST.MAX_CREDIT;

            return _w;
        }

        public void Update(double sign, double near, double dist)
        {
            /*
             * it is difficult determinating if the does as supposed, but the logic seems correct
             * */

            if (!CONST.ACTIVATOR.RandomSample(mind))
                return;

            if (Add(near, dist))
                return;

            Remove(near);
            Adjust(sign, dist);
        }

        private bool Add(double near, double dist)
        {
            int count = HUB.units.Count;
            int max = HUB.max_num_units;
            double avg = 100.0d / count;

            if (count > max)
                return false;

            if (dist < avg)
                return false;

            double low = near - CONST.ALPHA <= CONST.MIN ? CONST.MIN : near - CONST.ALPHA;
            double high = near + CONST.ALPHA >= CONST.MAX ? CONST.MAX : near + CONST.ALPHA;

            mind.mem.UNITS_ADD(this, low, high);
            
            return true;
        }

        private void Remove(double near)
        {
            double low = near - CONST.ALPHA;
            double high = near + CONST.ALPHA;

            mind.mem.UNITS_REM(this, low, high);
        }

        private void Adjust(double sign, double dist)
        {
            if (dist < CONST.ALPHA)
                return;

            double rand = mind.rand.MyRandomDouble(10)[5];

            Index += rand * CONST.ETA * sign;

            if (Index <= CONST.MIN)
                Index = CONST.MIN;

            if (Index >= CONST.MAX)
                Index = CONST.MAX;
        }

        public static UNIT IDLE_UNIT(TheMind mind)
        {
            return Create(mind, "GUID", -1d, "DATA", "NONE", UNITTYPE.IDLE, LONGTYPE.NONE);
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
