using Awesome.AI.Common;
using Awesome.AI.Helpers;
using Awesome.AI.Interfaces;
using static Awesome.AI.Helpers.Enums;

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

            m_out_high = -1000.0d;
            m_out_low = 1000.0d;
            d_out_high = -1000.0d;
            d_out_low = 1000.0d;
            posx_high = -1000.0d;
            posx_low = 1000.0d;
        }

        public SOFTCHOICE FuzzyMom
        {
            get { return deltaMom.ToFuzzy(mind); }
        }

        public HARDCHOICE TheChoice
        {
            get { return deltaMom.ToChoise(mind); }
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
                //its a hack, yes its cheating..
                double boost = mind.parms.boost;

                //posxy = 10.0d + (boost * momentum);//dosnt seem right
                posxy = 10.0d + (boost * deltaMom);//dosnt seem right
                //posx = posx + (boost * velocity);
                //posx = 10.0d + (boost * momentum);

                if (posxy < Constants.LOWXY)
                    posxy = Constants.LOWXY;
                if (posxy > Constants.HIGHXY)
                    posxy = Constants.HIGHXY;

                if (posxy <= posx_low) posx_low = posxy;
                if (posxy > posx_high) posx_high = posxy;

                return posxy;
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

            double dist = mind.calc.NormalizeRange(curr.LowAtZero, 0.0d, 100.0d, 100000.0d, 1000100.0d);//dist mercury -> sun
            double mass_M = Vars.zero_mass;
            double mass_m = mind.parms.mass;

            //Gravitational Constant (G)
            double G = Constants.GRAV_CONST;

            // FORMEL: ~F = (G * (m * M) / r^2) * ~r
            double grav = G * ((mass_m * mass_M) / (dist * dist));

            return grav;
        }

        private List<double> cred = new List<double>();
        private List<double> lim = new List<double>();
        private double velocity = 0;
        public void Calculate()
        {
            /*
             * still experimental..
             * I know its not using a black hole, but it should be the same principle outside the event horizon???
             * */

            double max = HighestVar;
            double sta_lim = Limitter(true, 0.0d, 0.0d);
            double dyn_lim = Limitter(false, mind.curr_unit.credits, mind.parms.shift);
            double sta_force = 10E-10 * max;
            double dyn_force = 10E-10 * (max - mind.curr_unit.Variable);

            double Fnet = (sta_force * sta_lim) - (dyn_force * dyn_lim);
            double m = mind.parms.mass;
            double dt = mind.parms.delta_time;      //delta time

            //F=m*a
            //a=dv/dt
            //F=(m*dv)/dt
            //F*dt=m*dv
            //dv=(F*dt)/m
            double dv = (Fnet * dt) / m;            //delta velocity

            velocity = dv;

            //momentum: p = m * v
            momentum = (m) * velocity;

            if (momentum <= m_out_low) m_out_low = momentum;
            if (momentum > m_out_high) m_out_high = momentum;

            if (deltaMom <= d_out_low) d_out_low = deltaMom;
            if (deltaMom > d_out_high) d_out_high = deltaMom;

            if (double.IsNaN(velocity))
                throw new Exception("Calculate");

            cred.Add(mind.curr_unit.credits);
            lim.Add(dyn_lim);

            if (cred.Count > 50)
                cred.RemoveAt(0);

            if (lim.Count > 50)
                lim.RemoveAt(0);





            ////car left
            //Fsta = ApplyStatic();

            ////car right
            //Fdyn = ApplyDynamic();

            //double Fnet = mind.goodbye.IsNo() ? -Fsta + Fdyn : -Fsta;
            //double m = mind.parms.mass;
            //double dt = mind.parms.delta_time;                             //delta time, 1sec/500cyc

            ////F=m*a
            ////a=dv/dt
            ////F=(m*dv)/dt
            ////F*dt=m*dv
            ////dv=(F*dt)/m
            //double dv = (Fnet * dt) / m;

            //velocity += dv;

            ////momentum: p = m * v
            //momentum = (m) * velocity;

            //if (momentum <= out_low) out_low = momentum;
            //if (momentum > out_high) out_high = momentum;

            //if (double.IsNaN(velocity))
            //    throw new Exception();







            ////car left
            //Fsta = ApplyStatic();

            ////car right
            //if (mind.goodbye.IsNo())
            //    Fdyn = ApplyDynamic();

            ////momentum: p = m * v
            //momentum = (mind.parms.mass * 2) * velocity;

            //if (momentum <= out_low) out_low = momentum;
            //if (momentum > out_high) out_high = momentum;

            //if (double.IsNaN(velocity))
            //    throw new Exception();
        }

        public double Limitter(bool is_static, double credits, double shift)
        {
            /*
             * friction coeficient
             * should friction be calculated from position???
             * */

            if (is_static)
                return Constants.BASE_FRICTION;

            Calc calc = mind.calc;

            double _c = 10.0d - credits;
            double x = 5.0d - _c + shift;
            double friction = calc.Logistic(x);

            return friction;
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

