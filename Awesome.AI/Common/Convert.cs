using Awesome.AI.Core;
using static Awesome.AI.Helpers.Enums;

namespace Awesome.AI.Common
{
    public class Convert
    {
        TheMind mind;
        private Convert() { }
        public Convert(TheMind mind)
        {
            this.mind = mind;
        }

        public double p_Min { get { return 0.1d + 1.5d; } }

        public double p_Max { get { return 99.9d - 1.5d; } }

        public double Process(double _x, string root, TYPE type)
        {
            double _l = Low();
            double _h = Max();
            double min = p_Min;
            double max = p_Max;

            //double _zero = 0.00000000000000000000000000000000000000000000000001d;
            //double _max = 99.99999999999999999999999999999999999999999999999999d;
            double _idle = 50.0d;

            switch (type)
            {
                //case TYPE.ZERO: return _zero;
                //case TYPE.MAX: return _max;
                case TYPE.IDLE: return _idle;
                default:
                    double res = mind.calc.NormalizeRange(_x, _l, _h, min, max);

                    return res;
            }
        }
        
        private double maxx = -1000.0d;
        public double Max()
        {
            if (maxx != -1000.0d)
                return maxx;
            
            //only valid UNITs
            List<UNIT> _u = mind.mem.UNITS_VAL();
            UNIT max = _u.OrderByDescending(x => x.index_orig).FirstOrDefault();
            //UNIT max = _u.OrderByDescending(x => x.index_calc).FirstOrDefault();

            maxx = max.index_orig;
            //maxx = max.index_calc;

            return maxx;
        }

        private double lowx = 1000.0d;
        public double Low()
        {
            if (lowx != 1000.0d)
                return lowx;

            //only valid UNITs
            List<UNIT> _u = mind.mem.UNITS_VAL();
            UNIT low = _u.OrderByDescending(x => x.index_orig).LastOrDefault();
            //UNIT low = _u.OrderByDescending(x => x.index_calc).LastOrDefault();

            lowx = low.index_orig;
            //lowx = low.index_calc;

            return lowx;
        }

        public void Reset() 
        {
            maxx = -1000.0d;
            lowx = 1000.0d;
        }
    }
}
