using Awesome.AI.Common;
using Awesome.AI.Helpers;
using Awesome.AI.Interfaces;
using System;

namespace Awesome.AI.Core.Physics
{
    public class WorksWithHill : IBaseVariable
    {
        TheMind mind;
        private WorksWithHill() { }
        public WorksWithHill(TheMind mind)
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

        //Weight
        public double Variable(UNIT _c)
        {
            /*
             * This is a changeable function.
             * 
             * Weight
             * W = m * g
             * */
            if (_c.IsNull())
                throw new Exception();

            if (_c.IsIDLE())
                throw new Exception();// return Params.idle_val;

            double earth_gravity = ZUNIT.zero_gravity;
            double mass = mind.parms.mass;
            double percent = mind.calc.NormalizeRange(_c.HighAtZero, 0.0d, mind.parms.max_index, 0.0d, 1.0d);

            double res = (mass * earth_gravity) * percent;
            
            return res;
        }        
    }
}

