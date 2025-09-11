using Awesome.AI.Common;
using Awesome.AI.Core;

namespace Awesome.AI.Awesome.AI.Core
{
    public class Down
    {
        private TheMind mind;
        private Down() { }
        public Down(TheMind mind)
        {
            this.mind = mind;
            vector = new MyVector2D(1.0d, 0.0d, 1.0d, 0.0d);
        }

        MyVector2D vector { get; set; }

        public void SetYES()
        {
            vector = new MyVector2D(-1.0d, 0.0d, 1.0d, null);
        }

        public void SetNO()
        {
            vector = new MyVector2D(1.0d, 0.0d, 1.0d, null);
        }

        public MyVector2D Get()
        {
            return vector;
        }

        public bool IsYes()
        {
            return vector.xx < 0.0d;
        }

        public bool IsNo()
        {
            return vector.xx >= 0.0d;
        }

        public double NormX() 
        {
            double x = vector.xx;
            double res = mind.calc.Normalize(x, -1.0d, 1.0d, 0.0d, 100.0d);

            return res;
        }
    }
}
