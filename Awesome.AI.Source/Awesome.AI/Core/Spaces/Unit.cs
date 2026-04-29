using Awesome.AI.Common;
using Awesome.AI.Core.Internals;
using Awesome.AI.CoreSystems;
using Awesome.AI.Interfaces;
using Awesome.AI.Source.Awesome.AI.Common;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core.Spaces
{
    public class UNIT
    {
        /*
         * maybe later there will be other types, like: SOUND, IMAGE, VIDEO
         * */

        public Ticket ticket { get; set; }
        private UNITTYPE unit_type { get; set; }
        public LONGTYPE ld_type { get; set; }
        public DateTime created { get; set; }
        public string guid { get; set; }
        public double credits { get; set; }
        public double reward { get; set; }        
        public double trace { get; set; }
                
        private TheMind mind;
        private UNIT() { }
        public UNIT(TheMind mind)
        {
            this.mind = mind;
        }

        private GPTVector2D vect = new GPTVector2D();
                
        public double UIget(string ax)
        {
            if (IsIDLE())
                return 50.0d;

            switch (ax)
            {
                case "will": return vect.xx;
                case "attention": return vect.yy;
                default: throw new Exception("Unit, UIget");
            }
        }

        public void UIset(string ax, double val)
        {
            switch (ax)
            {
                case "will": vect.xx = val; break;
                case "attention": vect.yy = val; break;
                default: throw new Exception("Unit, UIset");
            }            
        }

        private double h_index { get; set; }
        public double HI
        {
            get => IsIDLE() ? 50.0 : h_index;
            set { h_index = value; }
        }

        private string data { get; set; }//data
        public string Data
        {
            get
            {
                if (data != "DATA" && data != CONST.QUICK)
                    return data;

                switch (data)
                {
                    case "DATA":
                        string sub = mind.hub.GetSubject(this);

                        if (sub == "init")
                            return "";

                        double _i = mind.mech.ms.dv_sym_100.Norm0DV(mind);
                        string idx = $"{_i.Index(mind)}";

                        Lookup lookup = new Lookup();

                        string _data = lookup.GetDATA(mind, idx, sub);

                        return _data;
                    case "QUICK":
                        double index = UIget("will");

                        return index < 50.0d ? CONST.Q_YES : CONST.Q_NO;
                    default:
                        throw new Exception("Unit, Data");
                }
            }
            set { data = value; }
        }

        public string Root
        {
            get
            {
                List<UNIT> list = mind.hub.UnitsPerOccupasionc().OrderBy(x => x.created).ToList();

                if (!list.Any())
                    return "";

                int idx = list.IndexOf(this) + 1;
                string res = "_" + mind.hub.GetSubject(this) + idx;

                return res;
            }
        }

        public double Variable
        {
            get
            {
                IMechanics mech = mind.mech;
                double res = 0.0d;
                switch (mech.type)
                {
                    case MECHANICS.CIRCUIT_1_LOW:
                        throw new Exception("UNIT, Variable");
                    case MECHANICS.CIRCUIT_2_LOW:
                        res = UIget("will").LowZero(); break;
                    case MECHANICS.TUGOFWAR_LOW:
                        res = UIget("will").HighZero(); break;
                    case MECHANICS.BALLONHILL_LOW:
                        res = UIget("will").LowZero(); break;
                    default: throw new Exception("UNIT, Variable");
                }

                switch (CONST.transfer)
                {
                    case TRANSFER.NONE: break;
                    case TRANSFER.LOGISTIC: res = mind.calc.Logistic(res * 0.1d - 5.0d) * 100.0d; break;
                    case TRANSFER.OTHER: throw new NotImplementedException("Unit, Variable 1");
                    default : throw new Exception("Unit, Variable 2");
                }

                return res;
            }
        }

        public bool IsValid
        {
            get
            {
                switch (mind.bot.validation)
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

        public static UNIT Create(TheMind mind, string h_guid, double[] index, string data, string ticket, UNITTYPE ut, LONGTYPE lt)
        {
            //make sure some time has gone before creating a new unit
            if (ut != UNITTYPE.QDECISION)
                "hello world!".BusyWait(1, mind);

            DateTime create = DateTime.Now;
            Random rand = new Random();
            Lookup lookup = new Lookup();
            
            UNIT _w = new UNIT() { mind = mind, created = create, guid = h_guid, data = data, unit_type = ut, ld_type = lt };

            for (int i = 0; i < CONST.AXIS_MAX; i++)
                _w.UIset(CONST.AXES[i], index[i]);

            ticket = ticket != "" ? ticket : "NOTICKET";
            _w.ticket = new Ticket(ticket);

            double _r1 = rand.NextDouble();

            _w.trace = 1.0d;
            _w.credits = CONST.MAX_CREDIT;
            _w.h_index = _r1 * CONST.MAX_HUBSPACE;

            return _w;
        }

        public static UNIT CreateIdle(TheMind mind)
        {
            return Create(mind, "GUID", [-1d, -1d], "IDLE", "NONE", UNITTYPE.IDLE, LONGTYPE.NONE);
        }

        public static UNIT CreateQuick(TheMind mind, string name, double[] dex)
        {
            return Create(mind, "GUID", dex, name, "NONE", UNITTYPE.QDECISION, LONGTYPE.NONE);
        }

        public GPTVector2D ToVector() 
        {
            return new GPTVector2D(UIget(CONST.AXES[0]), UIget(CONST.AXES[1]), null, null);
        }

        public void Update(GPTVector2D v_near)
        {
            UpdateTRA();
            UpdateREW();
            UpdateHUB();
            UpdateUNT(v_near);
        }

        private void UpdateTRA()
        {
            if (IsDECISION())
                return;

            List<UNIT> units = mind.hub.UnitsPerOccupasionc();

            foreach (UNIT unit in units)
            {
                if (unit.guid == this.guid)
                    unit.trace = CONST.DECAY * unit.trace + 1.0d;
                else
                    unit.trace = CONST.DECAY * unit.trace + 0.0d;                    
            }
        }

        private void UpdateREW()
        {
            if (!mind.reward)
                return;

            if (IsDECISION())
                return;

            reward += 1.0 * trace;

            List<UNIT> units = mind.hub.UnitsPerOccupasionc();

            double max = units.Max(x => x.reward);
            
            foreach (UNIT unit in units)
            {
                if (unit.guid == this.guid)
                    continue;

                double reward_norm = mind.calc.Normalize(reward, 0.0d, max, 0.0d, 1.0d);

                if (reward_norm < CONST.EPSILON1)
                    mind.access.UNITS_REM(unit);                
            }
        }

        private void UpdateHUB()
        {
            if (IsDECISION())
                return;

            mind.reward = false;

            List<UNIT> units = mind.hub.UnitsPerOccupasionc();

            string sub = mind.hub.GetSubject(this);

            double index = mind.hub.GetIndex(sub);

            if (index == -1)
                return;

            double max = units.Max(x => x.reward);

            double reward_norm = mind.calc.Normalize(reward, 0.0d, max, 0.0d, 1.0d);

            double effective_gamma = CONST.GAMMA * (1.0d + reward_norm);

            HI += HI < index ? effective_gamma : -effective_gamma;

            mind.hub.AdjustWeights(sub, effective_gamma * 0.1d);
        }

        private void UpdateUNT(GPTVector2D v_near)
        {
            if (mind.STATE == STATE.QUICKDECISION)
                return;

            if (Add2D(v_near))
                return;

            Adjust();
        }

        private bool Add2D(GPTVector2D v_near)
        {
            int axis_count = CONST.AXES.Length;
            Lookup lookup = new Lookup();
            int count_units = mind.access.UNITS_ALL().Count;
            int count_hubs = lookup.CountHUBS(mind.mindtype);

            //double avg_area = (100.0 * 100.0) / count;
            //double avg_radius = Math.Sqrt(avg_area / Math.PI);

            //if (dist < avg_radius)
            //    return false;

            if (count_units > (CONST.MAX_UNITS * count_hubs))
                return false;

            double[] xx = [-1d, -1d];
            double[] yy = [-1d, -1d];
            
            xx[0] = Math.Clamp(v_near.xx - CONST.ALPHA, CONST.MIN, CONST.MAX);
            xx[1] = Math.Clamp(v_near.xx + CONST.ALPHA, CONST.MIN, CONST.MAX);
            
            yy[0] = Math.Clamp(v_near.yy - CONST.ALPHA, CONST.MIN, CONST.MAX);
            yy[1] = Math.Clamp(v_near.yy + CONST.ALPHA, CONST.MIN, CONST.MAX);
            
            double[][] axis = new double[axis_count][];
            
            axis[0] = xx;
            axis[1] = yy;
            
            mind.access.UNITS_ADD(this, axis, axis_count);

            return true;
        }
                
        private void Adjust()
        {
            double rnd = mind.rand.MyRandomDouble(10)[5];

            GPTVector2D func = new GPTVector2D();
            GPTVector2D vec = ToVector();
            GPTVector2D dir = mind.down.FlipUnit(vec);
            dir = func.Mul(dir, rnd * CONST.ETA);
            GPTVector2D _new = func.Add(vec, dir);

            if (_new.xx <= CONST.MIN) _new.xx = CONST.MIN;
            if (_new.yy >= CONST.MAX) _new.yy = CONST.MAX;

            switch (_new.xx)
            {
                case < CONST.MIN + CONST.MIN: mind.access.UNITS_REM(this); break;
                case > CONST.MAX - CONST.MIN: mind.access.UNITS_REM(this); break;
                default: UIset("will", _new.xx); return;
            }

            switch (_new.yy)
            {
                case < CONST.MIN + CONST.MIN: mind.access.UNITS_REM(this); break;
                case > CONST.MAX - CONST.MIN: mind.access.UNITS_REM(this); break;
                default: UIset("attention", _new.yy); return;
            }
        }

        //private void Remove(double near)
        //{
        //    double low = near - CONST.ALPHA;
        //    double high = near + CONST.ALPHA;

        //    mind.access.UNITS_REM(this, low, high);
        //}

        public bool IsUNIT() => unit_type == UNITTYPE.JUSTAUNIT;

        public bool IsIDLE() => unit_type == UNITTYPE.IDLE;

        public bool IsDECISION() => unit_type == UNITTYPE.LDECISION;

        public bool IsQDECISION() => unit_type == UNITTYPE.QDECISION;
    }
}
