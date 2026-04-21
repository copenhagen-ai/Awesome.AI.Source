using Awesome.AI.Common;
using Awesome.AI.Core.Mechanics;
using static Awesome.AI.Variables.Enums;

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
        public double DirPrev(string ax)
        {
            if (prev_dir.ContainsKey(ax))
                return prev_dir[ax];
            return -1d;
        }

        //public double Dir(string ax, bool set_prev)
        //{
        //    if (set_prev)
        //        prev_dir[ax] = dir;

        //    switch (ax)
        //    {
        //        case "will": dir = ms.dv_sym_curr > ms.dv_sym_prev ? 1.0d : -1.0d; break;
        //        case "attention": dir = ms.vv_sym_curr > ms.vv_sym_prev ? 1.0d : -1.0d; break;
        //        case "commitment": dir = ms.mom_sym_curr > ms.mom_sym_prev ? 1.0d : -1.0d; break;
        //        case "adaption": dir = ms.acc_sym_curr > ms.acc_sym_prev ? 1.0d : -1.0d; break;
        //        case "activation": dir = ms.ke_sym_curr > ms.ke_sym_prev ? 1.0d : -1.0d; break;
        //        case "influence": dir = ms.fnet_sym_curr > ms.fnet_sym_prev ? 1.0d : -1.0d; break;
        //        default: throw new Exception("EmProperties, Dir");
        //    }

        //    return dir;
        //}

        public double Step(TheMind mind, string axis_tmp)
        {
            switch (axis_tmp)
            {
                case "will": return Will().Norm100(mind);
                case "attention": return Attention().Norm100(mind);
                case "commitment": return Commitment().Norm100(mind);
                case "adaptation": return Adaptation().Norm100(mind);
                case "activation": return Activation().Norm100(mind);
                case "influence": return Influence().Norm100(mind);
                default: throw new Exception("UnitSpaceSoup, Step");
            }
        }

        public double Direction(TheMind mind, string ax, bool set_prev)
        {
            if (set_prev)
                prev_dir[ax] = dir;

            switch (ax)
            {
                case "will": return mind.down.Dir;
                case "attention": dir = ms.vv_sym_curr > ms.vv_sym_prev ? 1.0d : -1.0d; break;
                case "commitment": dir = ms.mom_sym_curr > ms.mom_sym_prev ? 1.0d : -1.0d; break;
                case "adaption": dir = ms.acc_sym_curr > ms.acc_sym_prev ? 1.0d : -1.0d; break;
                case "activation": dir = ms.ke_sym_curr > ms.ke_sym_prev ? 1.0d : -1.0d; break;
                case "influence": dir = ms.fnet_sym_curr > ms.fnet_sym_prev ? 1.0d : -1.0d; break;
                default: throw new Exception("UnitSpaceSoup, Direction");
            }

            return dir;
        }
    }
}
