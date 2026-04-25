using Awesome.AI.Core;

namespace Awesome.AI.Source.Awesome.AI.Common
{
    public static class MyNormalize
    {
        public static double Norm0(this double _x, TheMind mind, double _l, double _h)
        {
            double res = mind.calc.Normalize(_x, _l, _h, -1.0d, 1.0d);

            return res;
        }

        public static double Norm1(this double _x, TheMind mind, double _l = -1d, double _h = -1d)
        {
            double res = mind.calc.Normalize(_x, _l, _h, 0.0d, 1.0d);

            return res;
        }

        public static double Norm100(this double _x, TheMind mind, double _l, double _h)
        {
            double res = mind.calc.Normalize(_x, _l, _h, 0.0d, 100.0d);

            return res;
        }



        public static double Norm0DV(this double _x, TheMind mind)
        {
            double _l = mind.mech.ms.dv_sym_low;
            double _h = mind.mech.ms.dv_sym_high;
            _l = _l == _h ? _l - 0.1d : _l;

            double res = mind.calc.Normalize(_x, _l, _h, -1.0d, 1.0d);

            return res;
        }

        public static double Norm1DV(this double _x, TheMind mind)
        {
            double _l = mind.mech.ms.dv_sym_low;
            double _h = mind.mech.ms.dv_sym_high;
            _l = _l == _h ? _l - 0.1d : _l;

            double res = mind.calc.Normalize(_x, _l, _h, 0.0d, 1.0d);

            return res;
        }

        public static double Norm100DV(this double _x, TheMind mind)
        {
            double _l = mind.mech.ms.dv_sym_low;
            double _h = mind.mech.ms.dv_sym_high;
            _l = _l == _h ? _l - 0.1d : _l;

            double res = mind.calc.Normalize(_x, _l, _h, 0.0d, 100.0d);

            return res;
        }



        public static double Norm0VV(this double _x, TheMind mind)
        {
            double _l = mind.mech.ms.vv_sym_low;
            double _h = mind.mech.ms.vv_sym_high;
            _l = _l == _h ? _l - 0.1d : _l;

            double res = mind.calc.Normalize(_x, _l, _h, -1.0d, 1.0d);

            return res;
        }

        public static double Norm1VV(this double _x, TheMind mind)
        {
            double _l = mind.mech.ms.vv_sym_low;
            double _h = mind.mech.ms.vv_sym_high;
            _l = _l == _h ? _l - 0.1d : _l;

            double res = mind.calc.Normalize(_x, _l, _h, 0.0d, 1.0d);

            return res;
        }

        public static double Norm100VV(this double _x, TheMind mind)
        {
            double _l = mind.mech.ms.vv_sym_low;
            double _h = mind.mech.ms.vv_sym_high;
            _l = _l == _h ? _l - 0.1d : _l;

            double res = mind.calc.Normalize(_x, _l, _h, 0.0d, 100.0d);

            return res;
        }



        public static double Norm0FNET(this double _x, TheMind mind)
        {
            double _l = mind.mech.ms.fnet_sym_low;
            double _h = mind.mech.ms.fnet_sym_high;
            _l = _l == _h ? _l - 0.1d : _l;

            double res = mind.calc.Normalize(_x, _l, _h, -1.0d, 1.0d);

            return res;
        }

        public static double Norm1FNET(this double _x, TheMind mind)
        {
            double _l = mind.mech.ms.fnet_sym_low;
            double _h = mind.mech.ms.fnet_sym_high;
            _l = _l == _h ? _l - 0.1d : _l;

            double res = mind.calc.Normalize(_x, _l, _h, 0.0d, 1.0d);

            return res;
        }

        public static double Norm100FNET(this double _x, TheMind mind)
        {
            double _l = mind.mech.ms.fnet_sym_low;
            double _h = mind.mech.ms.fnet_sym_high;
            _l = _l == _h ? _l - 0.1d : _l;

            double res = mind.calc.Normalize(_x, _l, _h, 0.0d, 100.0d);

            return res;
        }



        public static double Norm0MOM(this double _x, TheMind mind)
        {
            double _l = mind.mech.ms.mom_sym_low;
            double _h = mind.mech.ms.mom_sym_high;
            _l = _l == _h ? _l - 0.1d : _l;

            double res = mind.calc.Normalize(_x, _l, _h, -1.0d, 1.0d);

            return res;
        }

        public static double Norm1MOM(this double _x, TheMind mind)
        {
            double _l = mind.mech.ms.mom_sym_low;
            double _h = mind.mech.ms.mom_sym_high;
            _l = _l == _h ? _l - 0.1d : _l;

            double res = mind.calc.Normalize(_x, _l, _h, 0.0d, 1.0d);

            return res;
        }

        public static double Norm100MOM(this double _x, TheMind mind)
        {
            double _l = mind.mech.ms.mom_sym_low;
            double _h = mind.mech.ms.mom_sym_high;
            _l = _l == _h ? _l - 0.1d : _l;

            double res = mind.calc.Normalize(_x, _l, _h, 0.0d, 100.0d);

            return res;
        }



        public static double Norm0ACC(this double _x, TheMind mind)
        {
            double _l = mind.mech.ms.acc_sym_low;
            double _h = mind.mech.ms.acc_sym_high;
            _l = _l == _h ? _l - 0.1d : _l;

            double res = mind.calc.Normalize(_x, _l, _h, -1.0d, 1.0d);

            return res;
        }

        public static double Norm1ACC(this double _x, TheMind mind)
        {
            double _l = mind.mech.ms.acc_sym_low;
            double _h = mind.mech.ms.acc_sym_high;
            _l = _l == _h ? _l - 0.1d : _l;

            double res = mind.calc.Normalize(_x, _l, _h, 0.0d, 1.0d);

            return res;
        }

        public static double Norm100ACC(this double _x, TheMind mind)
        {
            double _l = mind.mech.ms.acc_sym_low;
            double _h = mind.mech.ms.acc_sym_high;
            _l = _l == _h ? _l - 0.1d : _l;

            double res = mind.calc.Normalize(_x, _l, _h, 0.0d, 100.0d);

            return res;
        }



        public static double Norm0KE(this double _x, TheMind mind)
        {
            double _l = mind.mech.ms.ke_sym_low;
            double _h = mind.mech.ms.ke_sym_high;
            _l = _l == _h ? _l - 0.1d : _l;

            double res = mind.calc.Normalize(_x, _l, _h, -1.0d, 1.0d);

            return res;
        }

        public static double Norm1KE(this double _x, TheMind mind)
        {
            double _l = mind.mech.ms.ke_sym_low;
            double _h = mind.mech.ms.ke_sym_high;
            _l = _l == _h ? _l - 0.1d : _l;

            double res = mind.calc.Normalize(_x, _l, _h, 0.0d, 1.0d);

            return res;
        }

        public static double Norm100KE(this double _x, TheMind mind)
        {
            double _l = mind.mech.ms.ke_sym_low;
            double _h = mind.mech.ms.ke_sym_high;
            _l = _l == _h ? _l - 0.1d : _l;

            double res = mind.calc.Normalize(_x, _l, _h, 0.0d, 100.0d);

            return res;
        }
    }
}
