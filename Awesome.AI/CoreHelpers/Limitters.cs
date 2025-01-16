//using Awesome.AI.Core;
//using static Awesome.AI.Helpers.Enums;

//namespace Awesome.AI.CoreHelpers
//{
//    public class Limitters
//    {
//        public double learn_result { get; set; } = 0.0d;
//        public double limit_result { get; set; } = 0.0d;
//        private double min { get; set; } = 1000.0d;
//        private double max { get; set; } = -1000.0d;

//        TheMind mind;
//        private Limitters() { }
//        public Limitters(TheMind mind)
//        {
//            this.mind = mind;
//        }

//        public double Limit(bool is_static/*, bool process*/)//aka Elastic
//        {
//            /*
//             * thoughts on LIMIT..
//             * maybe its a "fake it till you make it", situation??
//             * maybe this is actually the core of the project. and the task is to make this limit as smooth as possible
//             * maybe making it layered somehow, use chance, fuzzy?
//             * maybe a gradient of somesort
//             * maybe this does actually make the "whoosh" effect over time/cycles
//             * 
//             * how to make it move like a spring?
//             * 
//             * there are many ways to implement the Limit
//             * */

//            //if (is_static && !process)
//            //    return mind.parms.base_friction / 2;

//            //if (!is_static && !process)
//            //    return limit_result;

//            switch (mind.parms.typelimit)
//            {
//                case TYPELIMIT.SIMPLE:
//                    return is_static ? mind.parms.base_friction : Simple();
//                case TYPELIMIT.SIGMOID:
//                    return is_static ? mind.parms.base_friction : Sigmoid();
//                case TYPELIMIT.CHANCE:
//                    return is_static ? mind.parms.base_friction : Chance();
//                default:
//                    throw new Exception();
//            }
//        }


//        private double GetIndex()
//        {
//            bool say_no = mind.parms._mech.dir.SayNo();

//            double tip = 2.0d - mind.parms.lim_correction;
//            double learn = !say_no ? -LearningRate(mind.parms.lim_learningrate, mind.parms.lim_correction) : LearningRate(mind.parms.lim_learningrate, tip);

//            mind.parms.lim_bias += learn;

//            double mom = mind.parms._mech.momentum;
//            double mom_min = mind.parms._mech.out_low;
//            double mom_max = mind.parms._mech.out_high;

//            /*
//             * we can just use hardcoded 10 and 0, since calculations are contained in this function
//             * works for all MECHANICS (pos is normalized, so works for HILL too)
//             * */
//            if (mind.parms.lim_bias < -10.0d) mind.parms.lim_bias = -10.0d;
//            if (mind.parms.lim_bias > 0.0d) mind.parms.lim_bias = 0.0d;

//            double pos = mind.calc.NormalizeRange(mom, mom_min, mom_max, 0.0d, 10.0d);

//            //higher bias less no(contest)
//            double index = pos + mind.parms.lim_bias;
//            min = index < min ? index : min;
//            max = index > max ? index : max;

//            return index;
//        }

//        public double Simple()//bias, hard limit
//        {
//            /*
//             * for a long time this was the solution used
//             * bias: some value, close to 0.0
//             * */
//            double bias = 0.12345;
//            bool ok = mind.parms._mech.momentum < bias;

//            return ok ? 1.0d : 0.0d;
//        }

//        public double Sigmoid()//sigmoid, limitter
//        {
//            double index = GetIndex();

//            double _x = mind.calc.NormalizeRange(index, min - 0.1d, max + 0.1d, -6.0d, 6.0d);
//            double _y = 1.0d - mind.calc.Logistic(_x);

//            if (double.IsNaN(_y))
//                throw new Exception();

//            limit_result = _y;

//            //double res = mind.calc.NormalizeRange(_y, 0.0d, 1.0d, mind.parms.base_friction / 2, mind.parms.base_friction);

//            //limit_result = res;

//            return limit_result;
//        }

//        public double Chance()//sigmoid, elastic
//        {
//            double index = GetIndex();

//            double _x = mind.calc.NormalizeRange(index, min - 0.1d, max + 0.1d, -6.0d, 6.0d);
//            double _y = 1.0d - mind.calc.Logistic(_x);

//            if (double.IsNaN(_y))
//                throw new Exception();

//            bool go_up = mind.calc.Chance0to1(_y, false);

//            return go_up ? 1.0d : 0.0d;
//        }

//        public double LearningRate(double learningrate, double correction) //making learningrate variable
//        {
//            learn_result = learningrate * correction;

//            if (learn_result > 1.0d)
//                learn_result = 1.0d;
//            if (learn_result < 0.0d)
//                learn_result = 0.0d;

//            return learn_result;
//        }













//        /*private List<bool> l_dir_prev { get; set; } = new List<bool>();
//        public double d_elastic { get; set; }
//        private bool b_dir_prev { get; set; }
//        /*private double Elastic0()
//        {
//            bool go_up;
//            go_up = momentum <= 0.0d ? false : true;

//            return go_up ? 1.0d : 0.0d;
//        }

//        public double Elastic1()
//        {
//            //IMechanics mech = Params.GetMechanics();

//            double index = GetIndex();

//            d_elastic = Calc.NormalizeRange(index, min - 0.1d, max + 0.1d, -1.0d, 1.0d);
//            d_elastic = d_elastic < 0.0d ? -d_elastic + 1.0d : 1.0d;
//            d_elastic = d_elastic * d_elastic;//pow 2

//            bool go_up;
//            go_up = momentum <= 0.0d ? false : true;

//            return go_up ? 1.0d : 0.0d;
//        }/**/

//        /*private double Elastic3()
//        {
//            IMechanics mech = Params.GetMechanics();

//            bool go_up;
//            if (momentum <= 0.0d)
//                go_up = false;
//            else
//            {
//                double _out = Calc.NormalizeRange(momentum, mech.low_out, mech.high_out, -4.0d, 4.0d);
//                double logi = 1.0d - Calc.Logistic(_out);
//                double percentage = Calc.NormalizeRange(logi, 0.5d, 1.0d, 10.0d, 95.0d);
//                bool flip = Calc.MyChance0to100(percentage, true);
//                go_up = flip;
//            }

//            return !go_up ? 0.0d : 1.0d;
//        }

//        private double Elastic4()
//        {
//            int top = 100;
//            int count = l_dir_prev.Where(x => x == true).Count();
//            double percentage = Calc.ToPercent((double)count, top);

//            bool flip = Calc.Chance0to1(percentage / 100.0d, false);
//            bool go_up = momentum > 0.0d && (flip || b_dir_prev);
//            b_dir_prev = go_up;

//            l_dir_prev.Add(go_up);
//            if (l_dir_prev.Count > top)
//                l_dir_prev.RemoveAt(0);

//            return !go_up ? 0.0d : 1.0d;
//        }/**/

//    }
//}
