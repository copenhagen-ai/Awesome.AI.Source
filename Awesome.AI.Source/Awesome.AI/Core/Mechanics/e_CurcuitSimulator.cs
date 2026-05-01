using Awesome.AI.Common;
using Awesome.AI.Core.Internals;
using Awesome.AI.Core.Mechanics;
using Awesome.AI.Core.Spaces;
using Awesome.AI.Interfaces;
using Awesome.AI.Source.Awesome.AI.Common;
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

        public double Step(double inputVoltage, double gain1, double gain2, double dt)
        {
            double rEffetive = rFixed * (0.1 + gain1 * gain2);
            Current = (inputVoltage - CapacitorVoltage) / rEffetive;
            DeltaCurrent = Current - previousCurrent;
            CapacitorVoltage += (Current / Capacitance) * dt;
            previousCurrent = Current;

            // Add slight nonlinearity (smooth saturation)
            CapacitorVoltage = Math.Tanh(CapacitorVoltage);

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

        public e_CircuitSimulator(TheMind mind, MECHANICS type, PROPS mprops)
        {
            this.mind = mind;
            this.type = type;

            this.ms = new MechSymbolicOut() { };
            this.mh = new MechHelper() { };
            this.mp = new MechParams() { };

            //this.mp.mprops = new ModProperties(mind, mprops);
            this.mp.eprops = new BaseProperties(ms);

            mp.posxy = CONST.STARTXY;

            //mp.peek_vv_out_high = -1000.0d;
            //mp.peek_vv_out_low = 1000.0d;
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

        public double PosXY()
        {
            double meter = mh.PosXY(mind, mp);

            return meter;
        }

        //public void Peek(UNIT curr)
        //{
        //    if (curr.IsNull())
        //        throw new Exception("CircuitSimulator, Current NULL");

        //    if (curr.IsIDLE())
        //        throw new Exception("CircuitSimulator, Current IDLE");

        //    Calc(curr, true, -1);

        //    double adj = mp.peek_vv_out_low == mp.peek_vv_out_high ? 0.1d : 0.0d;

        //    mp.peek_vv_norm = mind.calc.Normalize(mp.peek_cc_elec, mp.peek_vv_out_low - adj, mp.peek_vv_out_high, 0.0d, 100.0d);
        //}

        //public double Dir(string ax)
        //{
        //    throw new NotImplementedException("e_CircuitSimulator, Dir");
        //}

        //public double Mean()
        //{
        //    throw new NotImplementedException("e_CircuitSimulator, Dir");
        //}

        public void Calc(UNIT curr, int cycles)
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

                    mp.damp = 1.0d;             // Scaling for resistor/damping
                    mp.inertia_lim = 0.0015d;
                    mp.dt = 0.01;

                    break;
                default: throw new Exception("e_CircuitSimulator, Calc 1");
            }
            
            switch (type)
            {
                case MECHANICS.CIRCUIT_1_LOW:
                    // Calculate voltages
                    double vBattery = -(CONST.MAX * CONST.BASE_SCALE * 0.05d) * mp.damp;
                    double vResistor = (curr.Variable * 0.1d) * mp.damp;
                    double vLoss = mp.cc_elec_curr * Damping(mind) * mp.damp;
                    double netVoltage = vBattery + vResistor + vLoss;

                    // Inductor: L * di/dt = V_net => di = V_net / L * dt
                    mp.deltaCurrent = (netVoltage / mp.inductance) * mp.dt;

                    mp.dc_elec_prev = mp.dc_elec_curr;
                    mp.dc_elec_curr = mp.deltaCurrent;
                    mp.cc_elec_prev = mp.cc_elec_curr;
                    mp.cc_elec_curr += mp.deltaCurrent;

                    break;
                case MECHANICS.CIRCUIT_2_LOW:
                    double voltage = register.Average() + (mind.rand.MyRandomDouble(1)[0] - 0.5) * 0.01; ;
                    double dCurrent = 0.0d;
                        
                    double g1 = curr.Variable.Norm1VV(mind);
                    double g2 = Damping(mind);

                    double gain1 = g1 * mp.damp;
                    double gain2 = g2 * mp.damp;

                    foreach (var stage in circuit)
                    {
                        //stage.Resistance = 1000 + 500 * Math.Sin(2 * Math.PI * time * 0.1);
                        voltage = stage.Step(voltage, g1, g2, mp.dt);
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

            if (double.IsNaN(mp.cc_elec_curr) || double.IsNaN(mp.cc_elec_prev) || double.IsNaN(mp.dc_elec_curr) || double.IsNaN(mp.dc_elec_prev))
                throw new Exception("e_CircuitSimulator, Calc 4");
        }

        public void DeltaTime()
        {
            mp.dt = 0.001d;
        }

        public double Damping(TheMind mind)
        {
            /*
             * friction coeficient
             * should friction be calculated from position???
             * */

            MyCalc calc = mind.calc;

            double credits = CONST.MAX_CREDIT - mind.unit_current.credits;
            double friction = calc.Logistic(credits - ((double)CONST.MAX_CREDIT / 2.0d));

            return friction;
        }

        public void Calculate(PATTERN match, int cycles)
        {
            PATTERN pattern = PATTERN.NONE;

            if (pattern != match)
                return;

            if (cycles == 1)
                mh.ResetCircuit(mind, mp);

            Calc(mind.unit_current/*, false*/, cycles);

            mh.ExtremesCircuit(mp);
            mh.NormalizeCircuit(mind, mp);
            ms.Convert(mp, this.type);
        }
    }
}