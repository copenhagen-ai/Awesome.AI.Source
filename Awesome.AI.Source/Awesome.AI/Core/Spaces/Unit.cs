using Awesome.AI.Common;
using Awesome.AI.CoreInternals;
using Awesome.AI.CoreSystems;
using Awesome.AI.Interfaces;
using Awesome.AI.Variables;
using System.Numerics;
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
        public string guid { get; set; }//name
        public double credits { get; set; }
        public Dictionary<string, int> register { get; set; }        
        public double reward { get; set; }
        public double reward_norm { get; set; }
        public double trace { get; set; }
                
        private TheMind mind;
        private UNIT() { }
        public UNIT(TheMind mind)
        {
            this.mind = mind;
        }

        public double GetUI(string ax)
        {
            return u_index[ax];            
        }

        private Dictionary<string, double> u_index { get; set; }
        public double UI
        {
            get
            {
                if (IsIDLE())
                    return 50.0d;

                string ax = mind.soup.Axis;
                return u_index[ax];
            }
            set
            {
                string ax = mind.soup.Axis;
                u_index[ax] = value;
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
                if (data != "DATA")
                    return data;

                string sub = mind.hub.GetSubject(this);

                if (sub == "init")
                    return "";

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
                List<UNIT> list = mind.hub.GetUnits().OrderBy(x => x.created).ToList();

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
                IMechanics mech = mind.mech["z_noise"];
                double res = 0.0d;
                switch (mech.type)
                {
                    case MECHANICS.CIRCUIT_1_LOW:
                        throw new Exception("UNIT, Variable");
                    case MECHANICS.CIRCUIT_2_LOW:
                        throw new Exception("UNIT, Variable");
                    case MECHANICS.TUGOFWAR_LOW:
                        res = UI.LowZero(); break;
                    case MECHANICS.BALLONHILL_LOW:
                        res = UI.LowZero(); break;
                    default: throw new Exception("UNIT, Variable");
                }

                switch (CONST.transfer)
                {
                    case TRANSFER.NONE: break;
                    case TRANSFER.LOGISTIC: res = mind.calc.Logistic(res * 0.1d - 5.0d) * 100.0d; break;
                    default : throw new Exception("Unit, Variable");
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
            if (mind.environment == ENV.SERVER)
                "hello world!".BusyWaitOut(10);

            if (mind.environment == ENV.LOCAL)
                "hello world!".BusyWaitXml(2, mind.epochs);

            DateTime create = DateTime.Now;
            Random rand = new Random();
            Lookup lookup = new Lookup();
            List<string> occus = new List<string>();

            UNIT _w = new UNIT() { mind = mind, created = create, guid = h_guid, data = data, unit_type = ut, ld_type = lt };

            _w.u_index = new Dictionary<string, double>();
            int i = 0;
            foreach (string s in mind.soup.axis)
                _w.u_index[s] = index[i++];

            ticket = ticket != "" ? ticket : "NOTICKET";
            _w.ticket = new Ticket(ticket);

            double _r1 = rand.NextDouble();
            long _r2 = rand.NextInt64(lookup.occupasions.Length);

            _w.credits = CONST.MAX_CREDIT;
            _w.h_index = _r1 * CONST.MAX_HUBSPACE;

            occus = lookup.occupasions.ToList();
            _w.register = new Dictionary<string, int>();
            occus.ForEach(x => _w.register.Add(x, (int)_r2));

            return _w;
        }

        public static UNIT CreateIdle(TheMind mind)
        {
            return Create(mind, "GUID", [-1d], "IDLE", "NONE", UNITTYPE.IDLE, LONGTYPE.NONE);
        }

        private int _do { get; set; }
        public void Update(GPTVector2D v_near, double dist)
        {
            //if (mind.cycles_all < CONST.FIRST_RUN + 1)
            //    return;

            UpdateTRA();
            UpdateREW();
            UpdateAFF();
            UpdateHUB();
            UpdateUNT(v_near, dist);

            _do++;
            if (_do > 100)
                _do = 0;
        }

        private string last_affinity { get; set; }
        private void UpdateTRA()
        {
            //if (_do > 0)
            //    return;

            //if (IsDECISION())
            //    return;

            List<UNIT> units = mind.hub.GetUnits();

            foreach (UNIT unit in units)
            {
                if (unit.IsDECISION())
                    continue;

                if (unit.guid == this.guid)
                    trace = CONST.DECAY * trace + 1.0d;
                else
                    trace = CONST.DECAY * trace + 0.0d;
            }
        }

        private void UpdateREW()
        {
            //if (_do > 0)
            //    return;

            //if (IsDECISION())
            //    return;

            if (!mind.reward)
                return;

            mind.reward = false;

            List<UNIT> units = mind.hub.GetUnits();

            double max = 0.0d;

            foreach (UNIT unit in units)
            {
                if (unit.IsDECISION())
                    continue;

                reward += 1.0 * trace;

                max = reward > max ? reward : max;
            }
            
            List<UNIT> rem = new List<UNIT>();

            foreach (UNIT unit in units)
            {
                if (unit.IsDECISION())
                    continue;

                reward_norm = mind.calc.Normalize(reward, 0.0d, max, 0.0d, 1.0d);

                if (reward_norm < CONST.EPSILON)
                    rem.Add(unit);
            }

            //foreach (UNIT unit in rem)
            //    mind.access.UNITS_REM(unit);
        }

        private void UpdateAFF()
        {
            if (_do > 0)
                return;

            if (IsDECISION())
                return;

            string af_occo = mind._internal.Occu.name;

            if (af_occo == "")
                return;

            if (af_occo == "init")
                return;

            register[af_occo]++;

            int count = register.Sum(x => x.Value);

            if (count > 1000)//promil
                register[last_affinity]--;

            last_affinity = af_occo;

            return;
        }

        private void UpdateHUB()
        {
            if (_do > 0 && !mind.reward)
                return;

            if (IsDECISION())
                return;

            Lookup lookup = new Lookup();

            MINDS mindtype = mind.mindtype;

            int max = register.Values.Max();
            string af_occu = register.First(x => x.Value == max).Key;
            string occu = mind._internal.Occu.name;
            string sub = mind.hub.GetSubject(this);

            if (af_occu != occu)
                return;

            double index = mind.hub.GetIndex(sub);

            double effective_gamma = CONST.GAMMA * (1.0d + reward_norm);

            HI += HI < index ? effective_gamma : -effective_gamma;

            mind.hub.AdjustWeights(sub, effective_gamma * 0.1d);
        }

        private void UpdateUNT(GPTVector2D v_near, double dist)
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
            
            if (Add2D(v_near, dist))
                return;

            //Remove(near);
            Adjust(dist);
        }

        //private bool Add1D(double[] near, double dist)
        //{
        //    int count = mind.hub.GetUnits().Count;
        //    double avg = 100.0d / count;

        //    if (dist < avg)
        //        return false;

        //    double low = Math.Clamp(near[0] - CONST.ALPHA, CONST.MIN, CONST.MAX);
        //    double high = Math.Clamp(near[0] + CONST.ALPHA, CONST.MIN, CONST.MAX);

        //    mind.access.UNITS_ADD(this, low, high);

        //    return true;
        //}

        private bool Add2D(GPTVector2D v_near, double dist)
        {
            int count = mind.hub.GetUnits().Count;

            double total_area = 100.0 * 100.0;
            double avg_area = total_area / count;

            double avg_radius = Math.Sqrt(avg_area / Math.PI);

            if (dist < avg_radius)
                return false;

            double[] xx = [-1d, -1d];
            double[] yy = [-1d, -1d];
            int axis_count = mind.soup.axis.Length;
            for (int i = 0; i < axis_count; i++)
            {
                switch (i)
                {
                    case 0:
                        xx[0] = Math.Clamp(v_near.xx - CONST.ALPHA, CONST.MIN, CONST.MAX);
                        xx[1] = Math.Clamp(v_near.xx + CONST.ALPHA, CONST.MIN, CONST.MAX);
                        break;
                    case 1: 
                        yy[0] = Math.Clamp(v_near.yy - CONST.ALPHA, CONST.MIN, CONST.MAX);
                        yy[1] = Math.Clamp(v_near.yy + CONST.ALPHA, CONST.MIN, CONST.MAX);
                        break;
                    default: throw new Exception("Unit, Add2D");
                }
            }

            double[][] axis = new double[axis_count][];
            
            if (axis_count == 1) {
                axis[0] = xx;
            }

            if (axis_count == 2) {
                axis[0] = xx;
                axis[1] = yy;
            }

            mind.access.UNITS_ADD(this, axis, axis_count);
            
            return true;
        }

        private void Adjust(double dist)
        {
            if (dist < CONST.ALPHA)
                return;

            double rand = mind.rand.MyRandomDouble(10)[5];
            double dir = mind.down.Dir;

            UI += rand * CONST.ETA * dir;

            if (UI <= CONST.MIN) UI = CONST.MIN;
            if (UI >= CONST.MAX) UI = CONST.MAX;
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

        public bool IsQUICKDECISION() => unit_type == UNITTYPE.QDECISION;
    }
}
