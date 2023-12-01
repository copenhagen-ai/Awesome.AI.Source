using Awesome.AI.Common;
using Awesome.AI.Helpers;
using Awesome.AI.Interfaces;
using System;

namespace Awesome.AI.Core.Physics
{
    public class WorksWithGravity : IBaseVariable
    {
        TheMind mind;
        private WorksWithGravity() { }
        public WorksWithGravity(TheMind mind)
        {
            this.mind = mind;
        }

        public double ToPercent(UNIT _u)
        {
            if (_u.IsNull())
                throw new Exception();

            double max = mind.common.HighestForce().Variable;
            double _this = _u.Variable;

            double per = _this / max * 100.0d;

            return per;
        }

        //NewtonForce
        public double Variable(UNIT curr)
        {
            /*
                * I guess this is a changeable function, for now it is just the one I know to calculate force
                * */

            if (curr.IsNull())
                throw new Exception();

            if (curr.IsIDLE())
                throw new Exception();

            double dist = curr.LowAtZero;
            dist = dist == 0.0d ? 1.0E-50 : dist;// jajajaa
            double mass_m = ZUNIT.zero_mass;
            double mass_M = mind.parms.mass;

            //Gravitational Constant (G)
            double G = 6.674E-11d;

            // FORMEL: ~F = (G * (m * M) / r^2) * ~r
            double grav = G * ((mass_m * mass_M) / (dist * dist));

            return grav;
        }        
    }
}
