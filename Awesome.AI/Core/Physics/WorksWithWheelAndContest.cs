using Awesome.AI.Common;
using Awesome.AI.Interfaces;

namespace Awesome.AI.Core.Physics
{
    public class WorksWithWheelAndContest : IBaseVariable
    {
        TheMind mind;
        private WorksWithWheelAndContest() { }
        public WorksWithWheelAndContest(TheMind mind)
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

            double acc = curr.HighAtZeroReciprocal / 20.0d;
            acc = acc == 0.0d ? 1.0E-50 : acc;// jajajaa

            return acc;
        }        
    }
}
