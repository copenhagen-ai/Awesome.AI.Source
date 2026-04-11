using Awesome.AI.Core.Spaces;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core.Internals
{
    public class Filters
    {
        private TheMind mind;
        private Filters() { }
        public Filters(TheMind mind)
        {
            this.mind = mind;
        }

        public bool Valid(UNIT unit)
        {
            return unit.IsValid;
        }

        public bool LowCut(UNIT unit, string axis)//aka SayNo
        {
            /*
             * Lowcut filter?
             * */

            if (unit == null)
                throw new ArgumentNullException();

            if (mind.STATE == STATE.QUICKDECISION)
                return true;

            if (axis != CONST.AXES[0])
                return true;

            //because low index -> high mass
            UNIT _u = mind.access.UNITS_ALL(ORDER.BYINDEX)[CONST.LOWCUT];

            double low_cut = _u.UIget("will");
            double index = unit.UIget("will");

            return index > low_cut;
        }

        public bool Credits(UNIT unit, string axis)
        {
            if (unit == null)
                throw new ArgumentNullException();

            if (mind.STATE == STATE.QUICKDECISION)
                return true;

            if (axis != CONST.AXES[0])
                return true;

            return unit.credits > CONST.LOW_CREDIT;
        }

        //public static bool FilterUnit(TheMind mind, FILTERUNIT funit, FILTERTYPE ftype, UNIT unit = null)
        //{
        //    UNIT _u = unit;

        //    if (unit == null)
        //        _u = funit == FILTERUNIT.CURRENT ? mind.unit_current : mind.unit_actual;

        //    switch (ftype)
        //    {
        //        //case FILTERTYPE.ONE:
        //        //    if (_u.IsNull()) return false;
        //        //    if (_u.Root == "") return false;
        //        //    if (_u.IsIDLE()) return false;
        //        //    //if (_u.IsQUICKDECISION()) return false;
        //        //    if (_u.IsDECISION()) return false;
        //        //    return true;
        //        case FILTERTYPE.TWO:
        //            if (_u.IsNull()) return false;
        //            if (_u.Root == "") return false;
        //            //if (_u.IsQUICKDECISION()) return false;
        //            return true;
        //        case FILTERTYPE.THREE:
        //            if (_u.IsNull()) return false;
        //            if (_u.Root == "") return false;
        //            //if (_u.IsQUICKDECISION()) return false;
        //            if (_u.IsDECISION()) return false;
        //            return true;
        //    }

        //    throw new Exception("Extensions, FilterThinking");
        //}
    }
}
