using Awesome.AI.Core.Mechanics;

namespace Awesome.AI.Core.Internals
{
    public class BaseProperties
    {
        MechSymbolicOut ms { get; set; }

        public BaseProperties(MechSymbolicOut ms)
        {
            this.ms = ms;
        }

        // Maps mechanics to emergent cognition
        public double Will() => ms.dv_sym_curr;

        public double Attention() => ms.vv_sym_curr;

        public double Commitment() => ms.mom_sym_curr;

        public double Adaptation() => ms.acc_sym_curr;

        public double Activation() => ms.ke_sym_curr;

        public double Influence() => ms.fnet_sym_curr;

        // Example: creativity as curvature (simplified)
        public double Creativity()
        {
            if (ms.vv_sym_curr == 0 || ms.vv_sym_prev == 0)
                return 0;

            return Math.Abs(ms.vv_sym_curr - ms.vv_sym_prev);
        }

        public Dictionary<string, double> prev_dir = new Dictionary<string, double>();
        double dir = 0.0d;
        public double Dir(string ax, bool set_prev)
        {
            if (set_prev)
                prev_dir[ax] = dir;

            switch (ax)
            {
                case "attention": dir = ms.vv_sym_curr > ms.vv_sym_prev ? 1.0d : -1.0d; break;
                case "commitment": dir = ms.mom_sym_curr > ms.mom_sym_prev ? 1.0d : -1.0d; break;
                case "adaption": dir = ms.acc_sym_curr > ms.acc_sym_prev ? 1.0d : -1.0d; break;
                case "activation": dir = ms.ke_sym_curr > ms.ke_sym_prev ? 1.0d : -1.0d; break;
                case "influence": dir = ms.fnet_sym_curr > ms.fnet_sym_prev ? 1.0d : -1.0d; break;
                default: throw new Exception("EmProperties, Dir");
            }

            return dir;
        }

        public double DirPrev(string ax)
        {
            if (prev_dir.ContainsKey(ax))
                return prev_dir[ax];
            return -1d;
        }
    }
}
