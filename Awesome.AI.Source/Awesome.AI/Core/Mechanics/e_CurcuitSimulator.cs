using Awesome.AI.Common;
using Awesome.AI.Core.Mechanics;
using Awesome.AI.Interfaces;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core.Electrical
{
    public class e_CircuitSimulator : IMechanics
    {
        MECHANICS type;
        public MechParams mp { get; set; }
        public MechHelper mh { get; set; }

        private TheMind mind;
        private e_CircuitSimulator() { }

        public e_CircuitSimulator(TheMind mind, MECHANICS type)
        {
            this.mind = mind;
            this.type = type;

            this.mh = new MechHelper() { };
            this.mp = new MechParams() { };

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

        public double POS_XY
        {
            get
            {
                throw new NotImplementedException("CircuitSimulator, POS_XY");
            }
        }

        public void Peek(UNIT curr)
        {
            if (curr.IsNull())
                throw new Exception("CircuitSimulator, Current NULL");

            if (curr.IsIDLE())
                throw new Exception("CircuitSimulator, Current IDLE");

            Calc(curr, true, -1);

            mp.peek_norm = mind.calc.Normalize(mp.peek_pp_elec, mp.vv_out_low_peek, mp.vv_out_high_peek, 0.0d, 100.0d);
        }

        public void Calc(UNIT curr, bool peek, int cycles)
        {
            DeltaTime();

            double inductance = 0.0d;

            switch (type)
            {
                case MECHANICS.CIRCUIT_1_LOW:
                    //mp.voltageMax = 5.0d;                                           // Max voltage scaling
                    mp.dampingFactor = 0.2d;                                        // Scaling for resistor/damping
                    mp.batteryVoltage = CONST.MAX * CONST.BASE_REDUCTION * 0.05d;    // Constant voltage source
                    mp.variableResistance = curr.Variable * 0.1d;                   // Dynamic resistance
                    mp.inductance = 1.0d;                                           // Inductor as electrical inertia
                    inductance = mp.inductance;
                    break;
                default: throw new Exception("e_CircuitSimulator, Unsupported type");
            }

            // Calculate voltages
            double vBattery = ApplyBattery(mp, type);
            double vResistor = ApplyResistor(mp, type);
            double vLoss = ApplyResistiveLoss(mp, type);

            double netVoltage = vBattery + vResistor + vLoss;

            // Inductor: L * di/dt = V_net => di = V_net / L * dt
            double deltaCurrent = (netVoltage / inductance) * mp.dt;

            if (peek)
            {
                mp.peek_pp_elec = mp.currentCurrent + deltaCurrent;
            }
            else
            {
                //mp.previousCurrent = mp.currentCurrent;
                //mp.currentCurrent += deltaCurrent;
                //mp.previousFluxLinkage = mp.currentFluxLinkage;
                //mp.currentFluxLinkage = mp.inductance * mp.currentCurrent;

                // Integrate current to get charge
                //mp.cumulativeCharge += mp.currentCurrent * mp.dt;



                mp.previousCurrent = mp.currentCurrent; // update for next step
                mp.currentCurrent += deltaCurrent * mp.dt;

                mp.pp_elec_prev = mp.pp_elec_curr;
                mp.dp_elec_prev = mp.dp_elec_curr;
                mp.pp_elec_curr = inductance * mp.currentCurrent; // p = L * I
                mp.dp_elec_curr = inductance * (mp.currentCurrent - mp.previousCurrent); // Δp = L * ΔI

            }

            if (double.IsNaN(mp.previousCurrent) || double.IsNaN(mp.currentCurrent)/* || double.IsNaN(mp.cumulativeCharge)*/)
                throw new Exception("NAN in CircuitSimulator");
        }

        public void DeltaTime()
        {
            double delta = mind.mech_high.mp.dv_100;
            double mod = delta > 0.0d ? delta / 100.0d : 1.0d;
            mp.dt = 0.0005d * mod;
        }

        // ---------------- Electrical Functions ----------------

        public double ApplyBattery(MechParams mp, MECHANICS type)
        {
            switch (type)
            {
                case MECHANICS.CIRCUIT_1_LOW:
                    return -mp.batteryVoltage * mp.dampingFactor;
                default: throw new Exception("CircuitSimulator, ApplyBattery");
            }
        }

        public double ApplyResistor(MechParams mp, MECHANICS type)
        {
            switch (type)
            {
                case MECHANICS.CIRCUIT_1_LOW:
                    return mp.variableResistance * mp.dampingFactor;
                default: throw new Exception("CircuitSimulator, ApplyResistor");
            }
        }

        public double ApplyResistiveLoss(MechParams mp, MECHANICS type)
        {
            // Resistive damping proportional to current
            double damping = mh.Friction(mind, mind.unit_current.credits, 0.1d);
            return damping * mp.currentCurrent * mp.dampingFactor;
        }

        // --------------------------------------------------------

        public void Calculate(PATTERN match, int cycles)
        {
            PATTERN pattern = PATTERN.NONE;

            if (mind.z_current != "z_noise")
                return;

            if (pattern != match)
                return;

            if (cycles == 1)
                mh.ResetCircuit(mind, mp);

            Calc(mind.unit_current, false, cycles);

            mh.ExtremesCircuit(mp);
            mh.NormalizeCircuit(mind, mp);
            mh.ConvertCircuit(mp);
        }
    }
}