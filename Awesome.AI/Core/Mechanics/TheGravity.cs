using Awesome.AI.Common;
using Awesome.AI.Interfaces;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core.Mechanics
{
    public class _TheGravity : IMechanics
    {
        /*
         * >> THE HACK <<
         * -  gaspedal/wheel analogy in docs
         * */

        public double momentum { get; set; }
        public double deltaMom { get; set; }
        private double deltaMomPrev {  get; set; }
        public double Fsta { get; set; }
        public double Fdyn { get; set; }
        public double m_out_high { get; set; }
        public double m_out_low { get; set; }
        public double d_out_high { get; set; }
        public double d_out_low { get; set; }
        public double posx_high { get; set; }
        public double posx_low { get; set; }
        
        private TheMind mind;
        private _TheGravity() { }
        public _TheGravity(TheMind mind, Params parms)
        {
            this.mind = mind;

            posxy = Constants.STARTXY;//10;

            m_out_high = -1.0E20d;
            m_out_low = 1.0E20d;
            d_out_high = -1.0E20d;
            d_out_low = 1.0E20d;
            posx_high = -1.0E20d;
            posx_low = 1.0E20d;
        }

        public FUZZYDOWN FuzzyMom
        {
            get { return deltaMom.ToFuzzy(mind); }
        }

        public HARDDOWN HardMom
        {
            get { return deltaMom.ToDownZero(mind); }
            //get { return deltaMom.ToDownPrev(deltaMomPrev, mind); }
        }

        public double HighestVar
        {
            get { return Variable(UNIT.GetHigh); }
        }

        public double LowestVar
        {
            get { return Variable(UNIT.GetLow); }
        }

        private double posxy { get; set; }
        public double POS_XY
        {
            get 
            {
                return -1d;
            }
        }

        public double Variable(UNIT curr)
        {
            /*
             * I guess this is a changeable function, for now it is just the one I know to calculate force
             * */

            if (curr.IsNull())
                throw new Exception("Variable");

            if (curr.IsIDLE())
                throw new Exception("Variable");

            double dist = mind.calc.NormalizeRange(curr.LowAtZero, 0.0d, 100.0d, 0.0d, 50000000000.0d);
            double mass_M = Vars.zero_mass;
            double mass_m = mind.parms.mass;

            //Gravitational Constant (G)
            double G = Constants.GRAV_CONST;

            // FORMEL: ~F = (G * (m * M) / r^2) * ~r
            double grav = G * ((mass_m * mass_M) / (dist * dist));

            return grav;
        }

        public void Calculate()
        {
            /*
             * still experimental..
             * I know its not using a black hole, but it should be the same principle outside the event horizon???
             * */

            double mod = Modifier(mind.curr_unit.credits, mind.parms.shift);
            double m = mind.parms.mass;
            double dt = mind.parms.delta_time;    //delta time
            double Fnet = mind.curr_unit.Variable * mod;

            //F=m*a
            //a=dv/dt
            //F=(m*dv)/dt
            //F*dt=m*dv
            //dv=(F*dt)/m

            double deltaVel = (Fnet * dt) / m;            //delta velocity

            //momentum: p = m * v
            deltaMomPrev = deltaMom;
            deltaMom = (m) * deltaVel;
            momentum += deltaMom;

            if (momentum <= m_out_low) m_out_low = momentum;
            if (momentum > m_out_high) m_out_high = momentum;

            if (deltaMom <= d_out_low) d_out_low = deltaMom;
            if (deltaMom > d_out_high) d_out_high = deltaMom;

            if (double.IsNaN(deltaVel))
                throw new Exception("Calculate");
        }

        public double Modifier(double credits, double shift)
        {
            Calc calc = mind.calc;

            double _c = 10.0d - credits;
            double x = 5.0d - _c + shift;
            double mod = calc.Logistic(x);

            return mod < 0.5 ? -1d : 1d;
        }

        /////*
        //// * car left
        //// * */
        //public double ApplyStatic()
        //{
        //    double force = mind.common.HighestForce().Variable;
        //    //double limit = lim.Limit(true);

        //    double F = force * mind.calc.ForceLimit(true, 0.0d, 0.0d);                       //force, left
        //    double m = Vars.zero_mass;
        //    double dt = mind.parms.delta_time;                                               //delta time

        //    //F=m*a
        //    //a=dv/dt
        //    //F=(m*dv)/dt
        //    //F*dt=m*dv
        //    //dv=(F*dt)/m
        //    double dv = (F * dt) / m;                                                        //delta velocity

        //    //velocity -= dv;

        //    return dv;
        //}

        ///*
        // * car right
        // * */
        //public double ApplyDynamic()
        //{
        //    UNIT curr_unit_th = mind.curr_unit;

        //    if (curr_unit_th.IsNull())
        //        throw new Exception();

        //    //bool first_run = false;
        //    //if (mind.cycles_all <= mind.parms.first_run)
        //    //    first_run = true;

        //    double max = mind.common.HighestForce().Variable;
        //    double force = max - curr_unit_th.Variable * mind.calc.ForceLimit(false, curr_unit_th.credits, mind.parms.shift);
        //    //double limit = first_run ? 0.5d : lim.Limit(false);

        //    double F = force;                       //force, right
        //    double m = mind.parms.mass;
        //    double dt = mind.parms.delta_time;      //delta time

        //    //F=m*a
        //    //a=dv/dt
        //    //F=(m*dv)/dt
        //    //F*dt=m*dv
        //    //dv=(F*dt)/m
        //    double dv = (F * dt) / m;               //delta velocity

        //    //if (goodbye.IsNo())
        //    //if (goodbye.IsNo() && momentum < 0.0d)
        //    //velocity += dv;

        //    return dv;
        //}/**/

        //NewtonForce
        //public double Variable(UNIT curr)
        //{
        //    /*
        //     * I guess this is a changeable function, for now it is just the one I know to calculate force
        //     * */

        //    if (curr.IsNull())
        //        throw new Exception();

        //    if (curr.IsIDLE())
        //        throw new Exception();

        //    double dist = curr.LowAtZero;
        //    dist = dist == 0.0d ? 1.0E-50 : dist;// jajajaa
        //    double mass_m = Vars.zero_mass;
        //    double mass_M = mind.parms.mass;

        //    //Gravitational Constant (G)
        //    double G = 6.674E-11d;

        //    // FORMEL: ~F = (G * (m * M) / r^2) * ~r
        //    double grav = G * ((mass_m * mass_M) / (dist * dist));

        //    return grav;
        //}

        //public void Calculate()
        //{
        //    bool reset = velocity >= 0.0d; //maybe 0.666 * max_velocity

        //    //if(reset)
        //    //    velocity = 0.0d;

        //    //if (reset)  //car left
        //    Fsta = ApplyStatic();

        //    //car right
        //    if (mind.goodbye.IsNo())
        //        Fdyn = ApplyDynamic();

        //    //momentum: p = m * v
        //    momentum = (mind.parms.mass * 2) * velocity;

        //    if (momentum <= out_low) out_low = momentum;
        //    if (momentum > out_high) out_high = momentum;

        //    if (double.IsNaN(velocity))
        //        throw new Exception();
        //}

        ///*
        // * car left
        // * */
        //public double ApplyStatic()
        //{
        //    double force = mind.common.HighestForce().Variable;
        //    //double limit = lim.Limit(true);

        //    double F = force;                       //force, left
        //    double m = mind.parms.mass;
        //    double dt = DeltaT();                   //delta time
        //    double dv = DeltaV(F, m, dt) * 0.5d;// limit;   //delta velocity

        //    velocity -= dv;

        //    return dv;
        //}

        ///*
        // * car right
        // * */
        //public double ApplyDynamic()
        //{
        //    UNIT curr_unit_th = mind.curr_unit;

        //    if (curr_unit_th.IsNull())
        //        throw new Exception();

        //    bool first_run = false;
        //    if (mind.cycles_all <= mind.parms.first_run)
        //        first_run = true;

        //    double max = mind.common.HighestForce().Variable;
        //    double force = max - curr_unit_th.Variable;
        //    //double limit = first_run ? 0.5d : lim.Limit(false);

        //    double F = force;                       //force, right
        //    double m = mind.parms.mass;
        //    double dt = DeltaT();                   //delta time
        //    double dv = DeltaV(F, m, dt) * 0.5d;// limit;   //delta velocity

        //    //if (goodbye.IsNo())
        //    //if (goodbye.IsNo() && momentum < 0.0d)
        //        velocity += dv;

        //    return dv;
        //}/**/

        //private double DeltaV(double F, double m, double dt)
        //{
        //    //F=m*a
        //    //a=dv/dt
        //    //F=(m*dv)/dt
        //    //F*dt=m*dv
        //    //dv=(F*dt)/m
        //    double dv = (F * dt) / m;
        //    return dv;
        //}

        //private double DeltaT()
        //{
        //    //most of the time this is true

        //    double x = mind.parms.micro_sec;
        //    double dt = x / 1000000.0d;
        //    return dt;
        //}

        //private void Reset(TheMind mind)
        //{
        //    if (mind.cycles_all % 25000 != 0)
        //        return;

        //    out_low *= 0.5d;
        //    out_high *= 0.5d;

        //    posx_low *= 0.5d;
        //    posx_high *= 0.5d;

        //    min *= 0.5d;
        //    max *= 0.5d;
        //}
    }
}

