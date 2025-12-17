using Awesome.AI.Common;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core.Mechanics
{
    public class MechHelper
    {
        public void ResetCircuit(TheMind mind, MechParams mp)
        {
            if (!CONST.SAMPLE20.RandomSample(mind))
                return;

            mp.posxy = CONST.STARTXY;

            mp.peek_max = -1000.0d;
            mp.peek_min = 1000.0d;
            mp.pp_elec_max = -1000.0d;
            mp.pp_elec_min = 1000.0d;
            mp.dp_elec_max = -1000.0d;
            mp.dp_elec_min = 1000.0d;

            mp.vv_out_high_peek = -1000.0d;
            mp.vv_out_low_peek = 1000.0d;
            mp.vv_out_high = -1000.0d;
            mp.vv_out_low = 1000.0d;
            mp.dv_out_high = -1000.0d;
            mp.dv_out_low = 1000.0d;
            mp.posx_high = -1000.0d;
            mp.posx_low = 1000.0d;
        }

        public void ResetNoise(TheMind mind, MechParams mp)
        {
            //could be done in other ways also
            if (!CONST.SAMPLE20.RandomSample(mind))
                return;

            mp.posxy = CONST.STARTXY;

            mp.vv_out_high_peek = -1000.0d;
            mp.vv_out_low_peek = 1000.0d;
            mp.vv_out_high = -1000.0d;
            mp.vv_out_low = 1000.0d;
            mp.dv_out_high = -1000.0d;
            mp.dv_out_low = 1000.0d;
            mp.posx_high = -1000.0d;
            mp.posx_low = 1000.0d;
        }

        public void Reset(MechParams mp)
        {
            if (mp.pattern_prev == mp.pattern_curr)
                return;

            mp.pattern_prev = mp.pattern_curr;

            mp.pos_x = CONST.STARTXY;
            mp.velocity = 0.0d;
            mp.vv_curr = 0.0d;
            mp.dv_curr = 0.0d;
            mp.vv_prev = 0.0d;

            //m_out_high = -1000.0d;
            //m_out_low = 1000.0d;
            //d_out_high = -1000.0d;
            //d_out_low = 1000.0d;
            mp.posx_high = -1E10d;
            mp.posx_low = 1E10d;
        }

        public void ConvertCircuit(MechParams mp)
        {
            mp.peek_velocity = mp.peek_pp_elec;
            mp.vv_out_low_peek = mp.peek_min;
            mp.vv_out_high_peek = mp.peek_max;

            mp.vv_curr = mp.currentCurrent;
            mp.dv_curr = mp.deltaCurrent;

            mp.vv_out_low = mp.pp_elec_min;
            mp.vv_out_high = mp.pp_elec_max;
            mp.dv_out_low = mp.dp_elec_min;
            mp.dv_out_high = mp.dp_elec_max;


            mp.vv_100 = mp.pp_elec_100;
            mp.dv_100 = mp.dp_elec_100;

            mp.vv_90 = mp.pp_elec_90;
            mp.dv_90 = mp.dp_elec_90;
        }





        public void NormalizeCircuit(TheMind mind, MechParams mp)
        {
            // Adjust for degenerate ranges
            double currentAdj = mp.pp_elec_min == mp.pp_elec_max ? 0.1d : 0.0d;
            double chargeAdj = mp.dp_elec_min == mp.dp_elec_max ? 0.1d : 0.0d;

            // Normalize current (0-100%) and charge (0-100%) for UI or scaling purposes
            mp.pp_elec_100 = mind.calc.Normalize(mp.currentCurrent, mp.pp_elec_min - currentAdj, mp.pp_elec_max, 0.0d, 100.0d);
            mp.dp_elec_100 = mind.calc.Normalize(mp.deltaCurrent, mp.dp_elec_min - chargeAdj, mp.dp_elec_max, 0.0d, 100.0d);

            // Optional 10-90% normalized range
            mp.pp_elec_90 = mind.calc.Normalize(mp.currentCurrent, mp.pp_elec_min - currentAdj, mp.pp_elec_max, 10.0d, 90.0d);
            mp.dp_elec_90 = mind.calc.Normalize(mp.deltaCurrent, mp.dp_elec_min - chargeAdj, mp.dp_elec_max, 10.0d, 90.0d);
        }

        public void ExtremesCircuit(MechParams mp)
        {
            // Update flux linkage extremes
            if (mp.peek_pp_elec <= mp.peek_min) mp.peek_min = mp.peek_pp_elec;
            if (mp.peek_pp_elec > mp.peek_max) mp.peek_max = mp.peek_pp_elec;

            // Update current extremes
            if (mp.currentCurrent <= mp.pp_elec_min) mp.pp_elec_min = mp.currentCurrent;
            if (mp.currentCurrent > mp.pp_elec_max) mp.pp_elec_max = mp.currentCurrent;

            // Update cumulative charge extremes
            if (mp.deltaCurrent <= mp.dp_elec_min) mp.dp_elec_min = mp.deltaCurrent;
            if (mp.deltaCurrent > mp.dp_elec_max) mp.dp_elec_max = mp.deltaCurrent;
        }

        public void NormalizeNoise(TheMind mind, MechParams mp)
        {
            double adj1 = mp.vv_out_low == mp.vv_out_high ? 0.1d : 0.0d;
            double adj2 = mp.dv_out_low == mp.dv_out_high ? 0.1d : 0.0d;

            if (adj1 == 0.1d || adj2 == 0.1d)
                ;

            mp.vv_100 = mind.calc.Normalize(mp.vv_curr, mp.vv_out_low - adj1, mp.vv_out_high, 0.0d, 100.0d);
            mp.dv_100 = mind.calc.Normalize(mp.dv_curr, mp.dv_out_low - adj2, mp.dv_out_high, 0.0d, 100.0d);

            mp.vv_90 = mind.calc.Normalize(mp.vv_curr, mp.vv_out_low - adj1, mp.vv_out_high, 10.0d, 90.0d);
            mp.dv_90 = mind.calc.Normalize(mp.dv_curr, mp.dv_out_low - adj2, mp.dv_out_high, 10.0d, 90.0d);
        }

        public void ExtremesNoise(MechParams mp)
        {
            if (mp.peek_velocity <= mp.vv_out_low_peek) mp.vv_out_low_peek = mp.peek_velocity;
            if (mp.peek_velocity > mp.vv_out_high_peek) mp.vv_out_high_peek = mp.peek_velocity;

            if (mp.vv_curr <= mp.vv_out_low) mp.vv_out_low = mp.vv_curr;
            if (mp.vv_curr > mp.vv_out_high) mp.vv_out_high = mp.vv_curr;

            if (mp.dv_curr <= mp.dv_out_low) mp.dv_out_low = mp.dv_curr;
            if (mp.dv_curr > mp.dv_out_high) mp.dv_out_high = mp.dv_curr;
        }

        public void Normalize(TheMind mind, MechParams mp)
        {
            double adj1 = mp.vv_out_low == mp.vv_out_high ? 0.1d : 0.0d;
            double adj2 = mp.dv_out_low == mp.dv_out_high ? 0.1d : 0.0d;

            if (adj1 == 0.1d || adj2 == 0.1d)
                ;

            mp.vv_100 = mind.calc.Normalize(mp.vv_curr, mp.vv_out_low - adj1, mp.vv_out_high, 0.0d, 100.0d);
            mp.dv_100 = mind.calc.Normalize(mp.dv_curr, mp.dv_out_low - adj2, mp.dv_out_high, 0.0d, 100.0d);

            mp.vv_90 = mind.calc.Normalize(mp.vv_curr, mp.vv_out_low - adj1, mp.vv_out_high, 10.0d, 90.0d);
            mp.dv_90 = mind.calc.Normalize(mp.dv_curr, mp.dv_out_low - adj2, mp.dv_out_high, 10.0d, 90.0d);
        }

        public void Extremes(MechParams mp)
        {
            if (mp.vv_curr <= mp.vv_out_low) mp.vv_out_low = mp.vv_curr;
            if (mp.vv_curr > mp.vv_out_high) mp.vv_out_high = mp.vv_curr;

            if (mp.dv_curr <= mp.dv_out_low) mp.dv_out_low = mp.dv_curr;
            if (mp.dv_curr > mp.dv_out_high) mp.dv_out_high = mp.dv_curr;
        }

        public double PosXY(TheMind mind, MechParams mp)
        {
            double x_meter = mp.pos_x;

            if (x_meter <= 0.1d && !mind.goodbye)
                x_meter = CONST.VERY_LOW;
            
            if (x_meter < CONST.LOWXY) x_meter = CONST.LOWXY;
            if (x_meter > CONST.HIGHXY) x_meter = CONST.HIGHXY;

            if (x_meter <= mp.posx_low) mp.posx_low = x_meter;
            if (x_meter > mp.posx_high) mp.posx_high = x_meter;

            return x_meter;
        }

        public double Friction(TheMind mind, double credits, double damp)
        {
            /*
             * friction coeficient
             * should friction be calculated from position???
             * */

            Calc calc = mind.calc;

            double _c = 10.0d - credits;
            double x = 5.0d - _c;
            double friction = calc.Logistic(x);

            return friction * damp;
        }

        public double GetRandomNoise(TheMind mind, double noiseAmplitude)
        {
            double will_prop = mind.down.WillProp;

            return will_prop * noiseAmplitude;// Random value in range [-amplitude, amplitude]
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
