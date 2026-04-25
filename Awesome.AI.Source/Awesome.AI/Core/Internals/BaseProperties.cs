using Awesome.AI.Core.Mechanics;
using Awesome.AI.Source.Awesome.AI.Common;
using Awesome.AI.Variables;

namespace Awesome.AI.Core.Internals
{
    public class BaseProperties
    {
        MechSymbolicOut ms { get; set; }

        public BaseProperties(MechSymbolicOut ms)
        {
            this.ms = ms;

            curr_dir[CONST.AXES[0]] = 0.0d;
            curr_dir[CONST.AXES[1]] = 0.0d;
            curr_dir[CONST.AXES[2]] = 0.0d;
            curr_dir[CONST.AXES[3]] = 0.0d;
            curr_dir[CONST.AXES[4]] = 0.0d;
            curr_dir[CONST.AXES[5]] = 0.0d;            
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
        public Dictionary<string, double> curr_dir = new Dictionary<string, double>();
        
        public double DirPrev(string ax)
        {
            if (prev_dir.ContainsKey(ax))
                return prev_dir[ax];
            return -1d;
        }

        public double Step(TheMind mind, string axis_tmp)
        {
            switch (axis_tmp)
            {
                case "will": return Will().Norm100DV(mind);
                case "attention": return Attention().Norm100VV(mind);
                case "commitment": return Commitment().Norm100MOM(mind);
                case "adaptation": return Adaptation().Norm100ACC(mind);
                case "activation": return Activation().Norm100KE(mind);
                case "influence": return Influence().Norm100FNET(mind);
                default: throw new Exception("UnitSpaceSoup, Step");
            }
        }

        public double Direction(TheMind mind, string ax, bool set_prev)
        {
            if (set_prev)
                prev_dir[ax] = curr_dir[ax];

            switch (ax)
            {
                //case "will": return mind.down.Dir;
                case "will": curr_dir[ax] = ms.dv_sym_curr > ms.dv_sym_prev ? 1.0d : -1.0d; break;
                case "attention": curr_dir[ax] = ms.vv_sym_curr > ms.vv_sym_prev ? 1.0d : -1.0d; break;
                case "commitment": curr_dir[ax] = ms.mom_sym_curr > ms.mom_sym_prev ? 1.0d : -1.0d; break;
                case "adaption": curr_dir[ax] = ms.acc_sym_curr > ms.acc_sym_prev ? 1.0d : -1.0d; break;
                case "activation": curr_dir[ax] = ms.ke_sym_curr > ms.ke_sym_prev ? 1.0d : -1.0d; break;
                case "influence": curr_dir[ax] = ms.fnet_sym_curr > ms.fnet_sym_prev ? 1.0d : -1.0d; break;
                default: throw new Exception("UnitSpaceSoup, Direction");
            }

            double cont = mind.down.Continue ? 1d : -1d;
            curr_dir[ax] = curr_dir[ax] * cont;

            return curr_dir[ax];
        }
    }
}
