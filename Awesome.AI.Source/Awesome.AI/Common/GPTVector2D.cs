namespace Awesome.AI.Common
{
    public struct GPTVector2D
    {
        public double xx { get; set; }
        public double yy { get; set; }

        public double theta_in_radians { get; set; }
        public double theta_in_degrees { get { return ToDegrees(this); } }
        public double magnitude { get; set; }

        public GPTVector2D()
        {
        }

        public GPTVector2D(double? x, double? y, double? mag, double? rad)
        {
            xx = x == null ? double.NaN : (double)x;
            yy = y == null ? double.NaN : (double)y;
            magnitude = mag == null ? double.NaN : (double)mag;
            theta_in_radians = rad == null ? double.NaN : (double)rad;
        }


        public GPTVector2D Add(GPTVector2D v1, GPTVector2D v2)
        {
            return new GPTVector2D(v1.xx + v2.xx, v1.yy + v2.yy, null, null);
        }

        public GPTVector2D Sub(GPTVector2D v1, GPTVector2D v2)
        {
            return new GPTVector2D(v1.xx - v2.xx, v1.yy - v2.yy, null, null);
        }

        public GPTVector2D Mul(GPTVector2D v1, double scalar)
        {
            return new GPTVector2D(v1.xx * scalar, v1.yy * scalar, null, null);
        }

        public GPTVector2D Div(GPTVector2D v1, double scalar)
        {
            if (scalar == 0)
                throw new DivideByZeroException("Cannot divide by zero.");

            return new GPTVector2D(v1.xx / scalar, v1.yy / scalar, null, null);
        }

        public double Dot(GPTVector2D v1, GPTVector2D v2)
        {
            return v1.xx * v2.xx + v1.yy * v2.yy;
        }

        public GPTVector2D Unit()
        {
            double len = Math.Sqrt(xx * xx + yy * yy);

            if (len == 0)
                throw new DivideByZeroException("Cannot normalize a zero vector.");

            return new GPTVector2D(xx / len, yy / len, 1.0, null);
        }

        public GPTVector2D Reverse()
        {
            return new GPTVector2D(100.0d - xx, 100.0d - yy, null, null);
        }

        public GPTVector2D ReverseUnit()
        {
            return new GPTVector2D(-xx, -yy, null, null);
        }

        public double ToRadians(double angle)
        {
            double res = angle * (Math.PI / 180.0d);

            return res;
        }

        public double ToDegrees(GPTVector2D v1)
        {
            double res = v1.theta_in_radians * (180.0d / Math.PI);

            return res;
        }

        public GPTVector2D ToPolar(GPTVector2D v1)
        {
            /*
             * r = √ ( x2 + y2 )
             * θ = tan^-1 ( y / x )
             * 
             * θ = arctan ( y / x )
             * 
             * in degrees
             * double theta1 = Math.Atan2(y, x) * 180.0 / Math.PI;
             * in radians
             * double theta2 = Math.Atan(y / x);
             * */

            double r = Math.Sqrt(v1.xx * v1.xx + v1.yy * v1.yy);
            double theta = Math.Atan2(v1.yy, v1.xx);

            v1.magnitude = r;
            v1.theta_in_radians = theta;

            return v1;
        }

        public GPTVector2D ToCart(GPTVector2D v1)
        {
            /*
             * x = r × cos( θ )
             * y = r × sin( θ )
             * */

            double x = v1.magnitude * Math.Cos(v1.theta_in_radians);
            double y = v1.magnitude * Math.Sin(v1.theta_in_radians);

            v1.xx = x;
            v1.yy = y;

            return v1;
        }
    }
}