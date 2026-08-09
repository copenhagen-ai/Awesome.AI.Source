namespace Awesome.AI.Common
{
    public struct GPTVector6D
    {
        public double xx { get; set; }
        public double yy { get; set; }
        public double zz { get; set; }
        public double ww { get; set; }
        public double vv { get; set; }
        public double uu { get; set; }

        public double magnitude
        {
            get
            {
                return Math.Sqrt(
                    xx * xx +
                    yy * yy +
                    zz * zz +
                    ww * ww +
                    vv * vv +
                    uu * uu);
            }
        }

        public GPTVector6D(
            double x,
            double y,
            double z,
            double w,
            double v,
            double u)
        {
            xx = x;
            yy = y;
            zz = z;
            ww = w;
            vv = v;
            uu = u;
        }

        public GPTVector6D Add(GPTVector6D v1, GPTVector6D v2)
        {
            return new GPTVector6D(
                v1.xx + v2.xx,
                v1.yy + v2.yy,
                v1.zz + v2.zz,
                v1.ww + v2.ww,
                v1.vv + v2.vv,
                v1.uu + v2.uu);
        }

        public GPTVector6D Sub(GPTVector6D v1, GPTVector6D v2)
        {
            return new GPTVector6D(
                v1.xx - v2.xx,
                v1.yy - v2.yy,
                v1.zz - v2.zz,
                v1.ww - v2.ww,
                v1.vv - v2.vv,
                v1.uu - v2.uu);
        }

        public GPTVector6D Mul(GPTVector6D vector, double scalar)
        {
            return new GPTVector6D(
                vector.xx * scalar,
                vector.yy * scalar,
                vector.zz * scalar,
                vector.ww * scalar,
                vector.vv * scalar,
                vector.uu * scalar);
        }

        public GPTVector6D Div(GPTVector6D vector, double scalar)
        {
            if (scalar == 0)
                throw new DivideByZeroException("Cannot divide by zero.");

            return new GPTVector6D(
                vector.xx / scalar,
                vector.yy / scalar,
                vector.zz / scalar,
                vector.ww / scalar,
                vector.vv / scalar,
                vector.uu / scalar);
        }

        public double Dot(GPTVector6D v1, GPTVector6D v2)
        {
            return
                v1.xx * v2.xx +
                v1.yy * v2.yy +
                v1.zz * v2.zz +
                v1.ww * v2.ww +
                v1.vv * v2.vv +
                v1.uu * v2.uu;
        }

        public GPTVector6D Unit()
        {
            double len = magnitude;

            if (len == 0)
                throw new DivideByZeroException(
                    "Cannot normalize a zero vector.");

            return new GPTVector6D(
                xx / len,
                yy / len,
                zz / len,
                ww / len,
                vv / len,
                uu / len);
        }

        public GPTVector6D Reverse()
        {
            return new GPTVector6D(
                100.0d - xx,
                100.0d - yy,
                100.0d - zz,
                100.0d - ww,
                100.0d - vv,
                100.0d - uu);
        }

        public GPTVector6D ReverseUnit()
        {
            return new GPTVector6D(
                -xx,
                -yy,
                -zz,
                -ww,
                -vv,
                -uu);
        }

        public double DistanceTo(GPTVector6D other)
        {
            double dx = xx - other.xx;
            double dy = yy - other.yy;
            double dz = zz - other.zz;
            double dw = ww - other.ww;
            double dv = vv - other.vv;
            double du = uu - other.uu;

            return Math.Sqrt(
                dx * dx +
                dy * dy +
                dz * dz +
                dw * dw +
                dv * dv +
                du * du);
        }
    }
}
