using Awesome.AI.Core;

namespace Awesome.AI.Common
{
    public class Common
    {
        TheMind mind;
        private Common() { }
        public Common(TheMind mind)
        {
            this.mind = mind;
        }

        public UNIT highest_filter = null;
        public UNIT HighestForceHighPass()
        {
            if (highest_filter != null)
                return highest_filter;

            UNIT unit = mind.mem.UNITS_VAL().Where(x => !mind.filters.LowCut(x)).OrderByDescending(x=>x.Variable).FirstOrDefault();

            if (unit == null)
                throw new Exception();

            highest_filter = unit;
            return highest_filter;
        }

        public UNIT highest_force = null;
        public UNIT HighestForce()
        {
            if (highest_force != null)
                return highest_force;

            UNIT unit = mind.mem.UNITS_VAL().OrderByDescending(x => x.Variable).FirstOrDefault();

            if (unit == null)
                throw new Exception();
            
            highest_force = unit;
            return highest_force;
        }

        public UNIT lowest_force = null;
        public UNIT LowestForce()
        {
            if (lowest_force != null)
                return lowest_force;

            UNIT unit = mind.mem.UNITS_VAL().OrderBy(x => x.Variable).FirstOrDefault();

            if (unit == null)
                throw new Exception();

            lowest_force = unit;
            return lowest_force;
        }

        public void Reset() 
        {
            highest_filter = null;        
            highest_force = null;        
            lowest_force = null;
        }

        //public string GetMemberName<T>(Expression<Func<T>> memberExpression)
        //{
        //    MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
        //    return expressionBody.Member.Name;
        //}

        //public Type GetType(string strFullyQualifiedName)
        //{
        //    Type t = Type.GetType(strFullyQualifiedName);
        //    return t;
        //}

        //public object GetInstance(string strFullyQualifiedName)
        //{
        //    Type t = Type.GetType(strFullyQualifiedName);
        //    if (t == null)
        //        return null;
        //    return Activator.CreateInstance(t);
        //}
    }
}
