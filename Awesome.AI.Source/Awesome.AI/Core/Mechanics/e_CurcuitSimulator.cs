using Awesome.AI.Common;
using Awesome.AI.Core.Mechanics;
using Awesome.AI.Interfaces;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core.Electrical
{
    public class RCStage
    {
        public double Capacitance;
        public double rFixed;

        public double CapacitorVoltage;
        public double Current;
        public double DeltaCurrent;

        private double previousCurrent;

        public RCStage(double r, double c)
        {
            Capacitance = c;
            rFixed = r;
            CapacitorVoltage = 0;
            previousCurrent = 0;
        }

        public double Step(double inputVoltage, double weight1, double weight2, double dt)
        {
            double rEffetive = rFixed * weight1 * weight2;
            Current = (inputVoltage - CapacitorVoltage) / rEffetive;
            DeltaCurrent = Current - previousCurrent;
            CapacitorVoltage += (Current / Capacitance) * dt;
            previousCurrent = Current;

            //Clamp or saturate voltage
            //CapacitorVoltage = Math.Max(-1.0, Math.Min(1.0, CapacitorVoltage));

            //Or, for smooth saturation:
            //CapacitorVoltage = Math.Tanh(CapacitorVoltage);

            if (double.IsNaN(CapacitorVoltage) || double.IsInfinity(CapacitorVoltage))
                throw new Exception("MechHelper, Step");

            return CapacitorVoltage;
        }
    }

    public class FeedbackRegister
    {
        private double[] buffer;
        private int index = 0;

        public FeedbackRegister(int size)
        {
            buffer = new double[size];
        }

        public double Average()
        {
            double sum = 0;
            foreach (var v in buffer)
                sum += v;
            return sum / buffer.Length;
        }

        public void Push(double value)
        {
            buffer[index] = value;
            index = (index + 1) % buffer.Length;
        }
    }

    public class e_CircuitSimulator : IMechanics
    {
        public MECHANICS type { get; set; }
        public MechSymbolicOut ms { get; set; }
        public MechParams mp { get; set; }
        public MechHelper mh { get; set; }

        // Feedback register (memory)
        FeedbackRegister register = new FeedbackRegister(64);
        // RC circuit
        public RCStage[] circuit = 
        {
            new RCStage(1000, 0.0005),
            new RCStage(2200, 0.001),
            new RCStage(4700, 0.002)
        };

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

            //double[] vol = mind.rand.MyRandomDouble(64);
            // Seed the system with tiny initial noise
            //for (int i = 0; i < 64; i++)
            //    register.Push((vol[i] - 0.5d > 0 ? 1 : -1) * 0.0001);

            for (int i = 0; i < 64; i++)
                register.Push((i % 2 == 0 ? 1 : -1) * 0.0001);
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

            double adj = mp.peek_min == mp.peek_max ? 0.1d : 0.0d;

            mp.peek_norm = mind.calc.Normalize(mp.peek_cc_elec, mp.peek_min - adj, mp.peek_max, 0.0d, 100.0d);
        }

        public void Calc(UNIT curr, bool peek, int cycles)
        {
            DeltaTime();

            switch (type)
            {
                case MECHANICS.CIRCUIT_1_LOW:
                    /*
                     * Tug-Of-War
                     * */

                    mp.damp = 0.2d;             // Scaling for resistor/damping
                    mp.inertia_lim = 0.0015d;
                    mp.inductance = 1.0d;       // Inductor as electrical inertia
                                        
                    break;
                case MECHANICS.CIRCUIT_2_LOW:
                    /*
                     * Pink Noise
                     * */

                    mp.damp = 80.0d;             // Scaling for resistor/damping
                    mp.inertia_lim = 0.0015d;
                    mp.dt = 0.01;

                    break;
                default: throw new Exception("e_CircuitSimulator, Calc 1");
            }

            if (peek) 
            {
                mp.peek_cc_elec = mp.cc_elec_curr + mp.deltaCurrent;
            }
            else 
            {
                switch (type)
                {
                    case MECHANICS.CIRCUIT_1_LOW:
                        // Calculate voltages
                        double vBattery = -(CONST.MAX * CONST.BASE_REDUCTION * 0.05d) * mp.damp;
                        double vResistor = (curr.Variable * 0.1d) * mp.damp;
                        double vLoss = mp.cc_elec_curr * mh.Friction(mind) * mp.damp;
                        double netVoltage = vBattery + vResistor + vLoss;

                        // Inductor: L * di/dt = V_net => di = V_net / L * dt
                        mp.deltaCurrent = (netVoltage / mp.inductance) * mp.dt;

                        mp.dc_elec_prev = mp.dc_elec_curr;
                        mp.dc_elec_curr = mp.deltaCurrent;
                        mp.cc_elec_prev = mp.cc_elec_curr;
                        mp.cc_elec_curr += mp.deltaCurrent;

                        break;
                    case MECHANICS.CIRCUIT_2_LOW:
                        double voltage = register.Average();
                        double dCurrent = 0.0d;
                        
                        double w1 = curr.Variable.Norm1(mind, 0.0d, 100.0d);
                        double w2 = mh.Friction(mind);

                        double weight1 = w1 * mp.damp;
                        double weight2 = w2 * mp.damp;

                        foreach (var stage in circuit)
                        {
                            //stage.Resistance = 1000 + 500 * Math.Sin(2 * Math.PI * time * 0.1);
                            voltage = stage.Step(voltage, weight1, weight2, mp.dt);
                            dCurrent += stage.DeltaCurrent;

                            if (double.IsNaN(dCurrent) || double.IsInfinity(dCurrent))
                                throw new Exception("e_CircuitSimulator, Calc 2");
                        }

                        register.Push(voltage);

                        mp.dc_elec_prev = mp.dc_elec_curr;
                        mp.dc_elec_curr = dCurrent;
                        mp.cc_elec_prev = mp.cc_elec_curr;
                        mp.cc_elec_curr += dCurrent;

                        break;
                    default : throw new Exception("e_CircuitSimulator, Calc 3");
                }

            }

            if (double.IsNaN(mp.cc_elec_curr) || double.IsNaN(mp.cc_elec_prev) || double.IsNaN(mp.dc_elec_curr) || double.IsNaN(mp.dc_elec_prev))
                throw new Exception("e_CircuitSimulator, Calc 4");
        }

        public void DeltaTime()
        {
            double delta = mind.mech_high.ms.dv_sym_100;
            double mod = delta > 0.0d ? delta / 100.0d : 1.0d;
            mp.dt = 0.0005d * mod;
        }

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
            ms.Convert(mp, this.type);
        }
    }
}