using Awesome.AI.Common;
using Awesome.AI.CoreSystems;
using Awesome.AI.Interfaces;
using Awesome.AI.Source.Awesome.AI.Common;
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
        public LONGTYPE ld_type { get; set; }
        public DateTime created { get; set; }
        public string guid { get; set; }//name
        public double credits { get; set; }

        private double ui { get; set; }
        public double UnitIndex
        {
            get => IsIDLE() ? 50.0 : ui;
            set { ui = value; }
        }

        private double hi {  get; set; }
        public double HubIndex 
        { 
            get => IsIDLE() ? 50.0 : hi; 
            set { hi = value; } 
        }

        private TheMind mind;
        private UNIT() { }
        public UNIT(TheMind mind)
        {
            this.mind = mind;
        }

        private string data { get; set; }//data
        public string Data
        {
            get
            {
                if(data != "DATA")
                    return data;

                string sub = HUB.Subject;
                double _i = mind.mech_high.mp.props.PropsOut["base"];
                string idx = $"{_i.Index(mind)}";

                if (CONST.DECI_SUBJECT_CONTAINS(sub))
                    return "";

                Lookup lookup = new Lookup();

                string _data = lookup.GetDATA(mind, idx, sub);

                return _data;
            }
            set { data = value; }
        }

        public string Root
        {
            get
            {
                if (HUB is null)
                    return "";

                List<UNIT> list = HUB.Units.OrderBy(x => x.created).ToList();
                int idx = list.IndexOf(this) + 1;
                string res = "_" + HUB.Subject + idx;

                return res;
            }
        }

        public double Variable
        {
            get
            {
                IMechanics mech = mind.mech["z_noise"];
                switch (mech.type)
                {
                    case MECHANICS.CIRCUIT_1_LOW:
                        throw new Exception("UNIT, Variable");
                    case MECHANICS.CIRCUIT_2_LOW:
                        throw new Exception("UNIT, Variable");
                    case MECHANICS.TUGOFWAR_LOW:
                        return UnitIndex.HighZero();
                    case MECHANICS.BALLONHILL_LOW:
                        return UnitIndex.LowZero();
                    default: throw new Exception("UNIT, Variable");
                }
            } 
        }

        public bool IsValid
        {
            get
            {
                switch (mind.parms_current.validation)
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

        public HUB_SPACE HUB
        {
            get
            {
                if (mind.STATE == STATE.QUICKDECISION)
                    return null;

                if (IsQUICKDECISION())
                    return null;

                if (IsIDLE())
                    return null;

                HUB_SPACE hub = HUB_SPACE.Create(mind, this.guid);

                return hub;
            }
        }

        public static UNIT Create(TheMind mind, string h_guid, double index, string data, string ticket, UNITTYPE ut, LONGTYPE lt)
        {
            //make sure some time has gone before creating a new unit
            "hello world!".BusyWait(10);

            DateTime create = DateTime.Now;

            UNIT _w = new UNIT() { mind = mind, created = create, guid = h_guid, UnitIndex = index, data = data, unit_type = ut, ld_type = lt };

            if (ticket != "")
                _w.ticket = new Ticket(ticket);

            Random rand = new Random();

            _w.credits = CONST.MAX_CREDIT;
            _w.HubIndex = rand.NextDouble();

            return _w;
        }

        public static UNIT CreateIdle(TheMind mind)
        {
            return Create(mind, "GUID", -1d, "IDLE", "NONE", UNITTYPE.IDLE, LONGTYPE.NONE);
        }

        public void Update(double near, double map)
        {
            UpdateUS(near, map);
            UpdateHS();
        }

        private void UpdateHS()
        {
            //not implemented
            return;
        }

        private void UpdateUS(double near, double map)
        {
            /*
             * it is difficult determinating if the does as supposed, but the logic seems correct
             * i think it makes sense only noise can update unit
             * */
            if (mind.z_current != "z_noise")
                return;

            if (!CONST.ACTIVATOR.RandomSample(mind))
                return;
            
            //return;

            double dir = mind.down.Dir;
            double dist = Math.Abs(map - near);

            if (Add(near, dist))
                return;

            Remove(near);
            Adjust(dir, dist);
        }

        private bool Add(double near, double dist)
        {
            if (HUB is null)
                return true;

            int count = HUB.Units.Count;
            double avg = 100.0d / count;

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

        private void Adjust(double dir, double dist)
        {
            if (dist < CONST.ALPHA)
                return;

            double rand = mind.rand.MyRandomDouble(10)[5];

            UnitIndex += (rand * CONST.ETA * dir);

            if (UnitIndex <= CONST.MIN) UnitIndex = CONST.MIN;
            if (UnitIndex >= CONST.MAX) UnitIndex = CONST.MAX;
        }        

        public bool IsUNIT() => unit_type == UNITTYPE.JUSTAUNIT;

        public bool IsIDLE() => unit_type == UNITTYPE.IDLE;

        public bool IsDECISION() => unit_type == UNITTYPE.LDECISION;

        public bool IsQUICKDECISION() => unit_type == UNITTYPE.QDECISION;
    }
}
