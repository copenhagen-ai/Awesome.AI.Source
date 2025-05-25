namespace Awesome.AI.Common
{
    public struct MyVector2D
    {
        public double xx { get; set; }
        public double yy { get; set; }

        public double theta_in_radians { get; set; }
        public double theta_in_degrees { get { return ToDegrees(this); } }
        public double magnitude { get; set; }

        public MyVector2D()
        {
        }

        public MyVector2D(double? x, double? y, double? mag, double? rad)
        {
            xx = x == null ? double.NaN : (double)x;
            yy = y == null ? double.NaN : (double)y;
            magnitude = mag == null ? double.NaN : (double)mag;
            theta_in_radians = rad == null ? double.NaN : (double)rad;
        }


        public MyVector2D Add(MyVector2D v1, MyVector2D v2)
        {
            return new MyVector2D(v1.xx + v2.xx, v1.yy + v2.yy, null, null);
        }

        public MyVector2D Sub(MyVector2D v1, MyVector2D v2)
        {
            return new MyVector2D(v1.xx - v2.xx, v1.yy - v2.yy, null, null);
        }

        public MyVector2D Mul(MyVector2D v1, double scalar)
        {
            return new MyVector2D(v1.xx * scalar, v1.yy * scalar, null, null);
        }

        public MyVector2D Div(MyVector2D v1, double scalar)
        {
            if (scalar == 0)
                throw new DivideByZeroException("Cannot divide by zero.");
            
            return new MyVector2D(v1.xx / scalar, v1.yy / scalar, null, null);
        }

        public double Dot(MyVector2D v1, MyVector2D v2)
        {
            return v1.xx * v2.xx + v1.yy * v2.yy;
        }

        public MyVector2D Normalize()
        {
            if (this.magnitude == 0)
                throw new DivideByZeroException("Cannot divide by zero.");

            double magnitude = this.magnitude;
            
            double _x = xx / magnitude;
            double _y = yy / magnitude;

            return new MyVector2D(_x, _y, null, null);
        }

        public double ToRadians(double angle)
        {
            double res = angle * (Math.PI / 180.0d);

            return res;
        }

        public double ToDegrees(MyVector2D v1)
        {
            double res = v1.theta_in_radians * (180.0d / Math.PI);

            return res;
        }
        
        public MyVector2D ToPolar(MyVector2D v1)
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

        public MyVector2D ToCart(MyVector2D v1)
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