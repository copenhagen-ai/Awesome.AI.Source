//using Awesome.AI.Awesome.AI.Core;
//using Awesome.AI.Common;
//using Awesome.AI.Core;
//using Awesome.AI.Variables;
//using static Awesome.AI.Variables.Enums;

//namespace Awesome.AI.CoreInternals
//{
//    public class Direction
//    {
//        public FUZZYDOWN GoDownFuzzy 
//        { 
//            get 
//            {
//                return mind.mech_current.d_curr.ToFuzzy(mind);
//            } 
//        }

//        public double DirectionDown { get { return mind.space.Get().xx; } }
//        public PERIODDOWN GoDownPeriod { get { return RatioNoise.ToPeriod(mind); } }

//        public List<double> RatioNoise { get; set; }

//        private TheMind mind;
//        private Direction() { }
//        public Direction(TheMind mind)
//        {
//            this.mind = mind;
//            RatioNoise = new List<double>();
//        }       

//        public void Update()
//        {
//            if (mind.z_current != "z_noise")
//                return;

//            mind.mech_current.d_curr.GoDownZero(mind);

//            RatioNoise.Add(mind.space.Get().xx);

//            if (RatioNoise.Count > CONST.LAPSES)
//                RatioNoise.RemoveAt(0);
//        }

//        public int Count(bool is_yes)
//        {
//            int count = 0;
//            switch (is_yes)
//            {
//                case true: count = RatioNoise.Where(z => z < 0.0d).Count(); break;
//                case false: count = RatioNoise.Where(z => z >= 0.0d).Count(); break;
//            }

//            return count;
//        }
//    }
//}