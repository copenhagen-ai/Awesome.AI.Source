using Awesome.AI.Common;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core.Mechanics
{
    public class MechHelper
    {
        public void ResetNoise(TheMind mind, MechParams mp)
        {
            //could be done in other ways also
            if (!CONST.SAMPLE200.RandomSample(mind))
                return;

            mp.posxy = CONST.STARTXY;

            mp.m_out_high_p = -1000.0d;
            mp.m_out_low_p = 1000.0d;
            mp.m_out_high = -1000.0d;
            mp.m_out_low = 1000.0d;
            mp.d_out_high = -1000.0d;
            mp.d_out_low = 1000.0d;
            mp.posx_high = -1000.0d;
            mp.posx_low = 1000.0d;
        }

        public void Reset(MechParams mp)
        {
            if (mp.pattern_prev == mp.pattern_curr)
                return;

            mp.pattern_prev = mp.pattern_curr;

            mp.position_x = CONST.STARTXY;
            mp.velocity = 0.0d;
            mp.p_curr = 0.0d;
            mp.d_curr = 0.0d;
            mp.p_prev = 0.0d;

            //m_out_high = -1000.0d;
            //m_out_low = 1000.0d;
            //d_out_high = -1000.0d;
            //d_out_low = 1000.0d;
            mp.posx_high = -1E10d;
            mp.posx_low = 1E10d;
        }

        public void NormalizeNoise(TheMind mind, MechParams mp)
        {
            mp.p_100 = mind.calc.Normalize(mp.p_curr, mp.m_out_low - 0.1d, mp.m_out_high, 0.0d, 100.0d);
            mp.d_100 = mind.calc.Normalize(mp.d_curr, mp.d_out_low - 0.1d, mp.d_out_high, 0.0d, 100.0d);

            mp.p_90 = mind.calc.Normalize(mp.p_curr, mp.m_out_low - 0.1d, mp.m_out_high, 10.0d, 90.0d);
            mp.d_90 = mind.calc.Normalize(mp.d_curr, mp.d_out_low - 0.1d, mp.d_out_high, 10.0d, 90.0d);
        }

        public void Normalize(TheMind mind, MechParams mp)
        {
            if (mp.p_curr > mp.m_out_high) mp.m_out_high = mp.p_curr;
            if (mp.p_curr < mp.m_out_low) mp.m_out_low = mp.p_curr;

            if (mp.d_curr > mp.d_out_high) mp.d_out_high = mp.d_curr;
            if (mp.d_curr < mp.d_out_low) mp.d_out_low = mp.d_curr;

            mp.p_100 = mind.calc.Normalize(mp.p_curr, mp.m_out_low, mp.m_out_high, 0.0d, 100.0d);
            mp.d_100 = mind.calc.Normalize(mp.d_curr, mp.d_out_low, mp.d_out_high, 0.0d, 100.0d);

            mp.p_90 = mind.calc.Normalize(mp.p_curr, mp.m_out_low, mp.m_out_high, 10.0d, 90.0d);
            mp.d_90 = mind.calc.Normalize(mp.d_curr, mp.d_out_low, mp.d_out_high, 10.0d, 90.0d);
        }

        public void UpdateNoise(MechParams mp)
        {
            if (mp.peek_momentum <= mp.m_out_low_p) mp.m_out_low_p = mp.peek_momentum;
            if (mp.peek_momentum > mp.m_out_high_p) mp.m_out_high_p = mp.peek_momentum;

            if (mp.p_curr <= mp.m_out_low) mp.m_out_low = mp.p_curr;
            if (mp.p_curr > mp.m_out_high) mp.m_out_high = mp.p_curr;

            if (mp.d_curr <= mp.d_out_low) mp.d_out_low = mp.d_curr;
            if (mp.d_curr > mp.d_out_high) mp.d_out_high = mp.d_curr;
        }

        public void Update(MechParams mp)
        {
            if (mp.p_curr <= mp.m_out_low) mp.m_out_low = mp.p_curr;
            if (mp.p_curr > mp.m_out_high) mp.m_out_high = mp.p_curr;

            if (mp.d_curr <= mp.d_out_low) mp.d_out_low = mp.d_curr;
            if (mp.d_curr > mp.d_out_high) mp.d_out_high = mp.d_curr;
        }

        public double PosXY(TheMind mind, MechParams mp)
        {
            double x_meter = mp.position_x;

            if (x_meter <= 0.1d && mind.goodbye.IsNo)
                x_meter = CONST.VERY_LOW;
            
            if (x_meter < CONST.LOWXY) x_meter = CONST.LOWXY;
            if (x_meter > CONST.HIGHXY) x_meter = CONST.HIGHXY;

            if (x_meter <= mp.posx_low) mp.posx_low = x_meter;
            if (x_meter > mp.posx_high) mp.posx_high = x_meter;

            return x_meter;
        }

        public double Friction(TheMind mind, double credits, double shift)
        {
            /*
             * friction coeficient
             * should friction be calculated from position???
             * */

            Calc calc = mind.calc;

            double _c = 10.0d - credits;
            double x = 5.0d - _c + shift;
            double friction = calc.Logistic(x);

            return friction;
        }

        public double GetRandomNoise(TheMind mind, double noiseAmplitude)
        {
            UNIT curr_unit = mind.unit_noise;

            if (curr_unit == null)
                throw new Exception("MechHelper, GetRandomNoise");

            double _var = curr_unit.Variable;

            double rand = mind.calc.Normalize(_var, 0.0d, 100.0d, -1.0d, 1.0d);

            return rand * noiseAmplitude;// Random value in range [-amplitude, amplitude]
        }

        public double Sine(PATTERN pattern, double t, double omega, double gen1, double gen2, double good1, double good2, double bad1, double bad2)
        {
            switch (pattern)
            {
                case PATTERN.MOODGENERAL: return gen1 + (Math.Sin(omega * t) + 1.0d) / 2.0d * gen2;
                case PATTERN.MOODGOOD: return good1 + (Math.Sin(omega * t) + 1.0d) / 2.0d * good2;
                case PATTERN.MOODBAD: return bad1 + (Math.Sin(omega * t) + 1.0d) / 2.0d * bad2;
                default: throw new Exception("MechHelper, Sine");
            }
        }
    }
}
