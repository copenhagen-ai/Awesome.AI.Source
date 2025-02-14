﻿using Awesome.AI.Common;
using Awesome.AI.CoreHelpers;
using Awesome.AI.Interfaces;
using static Awesome.AI.Helpers.Enums;

namespace Awesome.AI.Core
{
    public class TheMatrix
    {
        /*
         * as it is now, it is very simple just up or down
         * 
         * expansion 1:
         * - still using up or down approach
         * - implement a theme system, and say GiveMeYourTheme and follow(follow by mood, interest, environment etc?)
         * - topic: most common HUB in related
         * 
         * expansion 2:
         * - still using up or down approach
         * - the closer to zero Calc.PythDist(UNIT.CreateMIN(), unit) is, the further up we want to jump
         * - maybe a limit or fuzzy: if Calc.PythDist(UNIT.CreateMIN(), unit) < 20.0 -> its critical -> jump far (similar to Meters.SayNO())
         * 
         * */

        private TheMind mind;
        private TheMatrix() { }
        public TheMatrix(TheMind mind)
        {
            this.mind = mind;
        }

        public UNIT NextUnit(UNIT curr, Direction dir)
        {
            if (curr == null)
                throw new Exception("NextUnit");
            if (dir == null)
                throw new Exception("Variable");

            if (!curr.IsIDLE())
            {
                UNIT w_act = Unit(curr, dir);

                if (w_act != null)
                    return w_act;                
            }
            
            /*
             * with more HUBS and UNITS added, this buffer wil be used less often
             * */
            UNIT _u = Buffer();
            return _u;
        }

        /*
         * priority 1
         * */
        private UNIT Unit(UNIT curr, Direction dir)
        {
            if (curr == null)
                throw new ArgumentNullException();
            
            List<UNIT> units = mind.mem.UNITS_VAL();

            units = units.Where(x =>
                   mind.filters.UnitIsValid(x)                   //comment to turn off
                && mind.filters.Direction(x, curr, dir)       //comment to turn off
                && !mind.filters.LowCut(x)                      //comment to turn off
                && mind.filters.Credits(x)                        //comment to turn off
                //&& mind.filters.Theme(x)                         //comment to turn off
                //&& Filters.Elastic2(dir)                  //comment to turn off
                //&& Filters.Ideal(x)                       //comment to turn off
                //&& Filters.Neighbor(x)
                ).ToList();

            if (units == null)
                throw new Exception("Unit");

            UNIT _u = Jump(curr, dir, units);
            //_u = Filters.Neighbor(_u, units);

            return _u;
        }

        private UNIT Jump(UNIT curr, Direction dir, List<UNIT> units)
        {
            if (curr == null)
                throw new ArgumentNullException();
            if (units == null)
                throw new ArgumentNullException();

            double near = NearEnergy(dir);

            units = units.OrderByDescending(x => x.Variable).ToList();

            UNIT above = units.Where(x => x.Variable < near).FirstOrDefault();
            UNIT below = units.Where(x => x.Variable >= near).LastOrDefault();

            if (above.IsNull() && below.IsNull())
                return null;

            if (above.IsNull())
                return below;

            if (below.IsNull())
                return above;

            UNIT res = near - above.Variable < below.Variable - near ? above : below;

            return res;
        }

        private double NearEnergy(Direction dir)
        {
            IMechanics mech = mind.mech;
            double f_h = mind.common.HighestForce().Variable;
            double f_l = mind.common.LowestForce().Variable;

            //double _v = mech.momentum;
            //double v_h = mech.m_out_high;
            //double v_l = mech.m_out_low;

            double _v = mech.deltaMom;
            double v_h = mech.d_out_high;
            double v_l = mech.d_out_low;

            double nrg = mind.calc.NormalizeRange(_v, v_l, v_h, f_l, f_h);

            return nrg;
        }

        private double NearPercent(Direction dir)
        {
            IMechanics mech = mind.mech;

            //double _v = mech._momentum;
            //double v_h = mech.m_out_high;
            //double v_l = mech.m_out_low;

            double _v = mech.deltaMom;
            double v_h = mech.d_out_high;
            double v_l = mech.d_out_low;

            double pct = mind.calc.NormalizeRange(_v, v_l, v_h, 0.0d, 100.0d);

            return pct;
        }

        /*
         * priority 2
         * */
        private UNIT Buffer()
        {
            /*
             * not sure if the buffer should be here(because of the random in topic an here)
             * but maybe it wont have such a big part later on, when more HUBS and UNITS are added
             * */
                        
            List<UNIT> units = mind.mem.UNITS_VAL().Where(x => 
                                mind.filters.UnitIsValid(x) 
                                && mind.filters.Credits(x) 
                                && !mind.filters.LowCut(x)
                                ).OrderByDescending(x => x.Variable).ToList();
            
            int[] rand = mind.rand.MyRandomInt(1,units.Count - 1);
            UNIT __u = units.Any() ? units[rand[0]] : UNIT.IDLE_UNIT(mind);
            return __u;
        }

        /*private UNIT JumpA(UNIT curr, List<UNIT> units, bool dir_up)
        {
            if (curr == null)
                throw new ArgumentNullException();
            if (units == null)
                throw new ArgumentNullException();

            units = units.OrderByDescending(x => Newton.NewtonForce(x)).ToList();

            UNIT _u = dir_up ? 
                units.Where(x => Newton.NewtonForce(x) < Newton.NewtonForce(curr)).FirstOrDefault() : 
                units.Where(x => Newton.NewtonForce(x) > Newton.NewtonForce(curr)).LastOrDefault();

            return _u;
        }/**/

        /*private UNIT JumpB(UNIT curr, List<UNIT> units, bool dir_up)
        {
            if (curr == null)
                throw new ArgumentNullException();
            if (units == null)
                throw new ArgumentNullException();

            units = units.OrderBy(x => Calc.DistPyth(curr, x)).ToList();

            UNIT _u = dir_up ?
                units.Where(x => Newton.NewtonForce(x) < Newton.NewtonForce(curr)).FirstOrDefault() :
                units.Where(x => Newton.NewtonForce(x) > Newton.NewtonForce(curr)).FirstOrDefault();

            return _u;
        }/**/
                
        /*private UNIT JumpC(UNIT curr, List<UNIT> units, bool dir_up)
        {
            if (curr == null)
                throw new ArgumentNullException();
            if (units == null)
                throw new ArgumentNullException();

            units = units.OrderByDescending(x => Newton.NewtonForce(curr, x)).ToList();

            UNIT _u = dir_up ?
                units.Where(x => Newton.NewtonForce(x) < Newton.NewtonForce(curr)).FirstOrDefault() :
                units.Where(x => Newton.NewtonForce(x) > Newton.NewtonForce(curr)).LastOrDefault();

            return _u;
        }/**/

        /*private UNIT JumpD(UNIT curr, Direction dir, List<UNIT> units)
        {
            if (curr == null)
                throw new ArgumentNullException();
            if (units == null)
                throw new ArgumentNullException();

            double near = NearEnergy3(dir);

            units = units.OrderByDescending(x => x.Variable).ToList();

            UNIT above = units.Where(x => x.Variable < near).FirstOrDefault();
            
            if (above.IsNull())
                return null;

            UNIT res = above;

            return res;
        }/**/

        /*private UNIT JumpF(UNIT curr, Direction dir, List<UNIT> units)
        {
            if (curr == null)
                throw new ArgumentNullException();
            if (units == null)
                throw new ArgumentNullException();

            double near = NearEnergy3(dir);

            units = units.OrderByDescending(x => x.Variable).ToList();

            UNIT above = units.Where(x => x.Variable < near).FirstOrDefault();
            UNIT below = units.Where(x => x.Variable > curr.Variable).LastOrDefault();//free fall???
            //UNIT below = units.Where(x => Newton.NewtonForce(x) >= near).LastOrDefault();

            if (above.IsNull() && below.IsNull())
                return null;

            if (dir.TheChoise(false).IsNo() && above.IsNull())
                return null;

            if (dir.TheChoise(false).IsYes() && below.IsNull())
                return null;

            UNIT res = dir.TheChoise(false).IsNo() ? above : below;

            return res;
        }/**/

        /*private UNIT JumpG(UNIT curr, List<UNIT> units, bool dir_up)
        {
            if (curr == null)
                throw new ArgumentNullException();
            if (units == null)
                throw new ArgumentNullException();

            double near = NearEnergy(dir_up);
            
            units = units.OrderByDescending(x => Newton.NewtonForce(curr, x)).ToList();

            UNIT above = units.Where(x => Newton.NewtonForce(x) < Newton.NewtonForce(curr) && Newton.NewtonForce(x) < near).FirstOrDefault();
            UNIT below = units.Where(x => Newton.NewtonForce(x) > Newton.NewtonForce(curr) && Newton.NewtonForce(x) > near).LastOrDefault();

            return dir_up && above != null ? above : below;
        }/**/

        /*private UNIT JumpH(UNIT curr, List<UNIT> units, bool dir_up)
        {
            if (curr == null)
                throw new ArgumentNullException();
            if (units == null)
                throw new ArgumentNullException();

            double per = NearPercent(dir_up);
            double act = per * Math.Sqrt(100.0d * 100.0d + 100.0d * 100.0d) / 100.0d;

            units = units.OrderBy(x => Calc.DistPyth(curr, x)).ToList();

            UNIT _u = dir_up ?
                units.Where(x => Calc.DistPyth(curr, x) > act).FirstOrDefault() :
                units.Where(x => Calc.DistPyth(curr, x) < act).LastOrDefault();

            return _u;
        }/**/

        /*private UNIT JumpI(UNIT curr, List<UNIT> units, bool dir_up)
        {
            if (curr == null)
                throw new ArgumentNullException();
            if (units == null)
                throw new ArgumentNullException();

            double per = NearPercent(dir_up);
            double _base = Calc.RoundDown(5, (int)per - 5);

            if (_base < 0.0d)
                _base = 0.0d;

            int area = 1;
            List<UNIT> _u1 = new List<UNIT>();
            while(_u1.Count  < 5)
            {
                _u1 = Memory.UNITS().Where(x => Calc.DistPyth(curr, x) <= area).OrderByDescending(x => Newton.NewtonForce(x)).ToList();
                area++;
            }

            if (!_u1.Any())
                return null;

            List<UNIT> _u2 = units.OrderBy(x => Newton.NewtonForce(x)).ToList();
            int rand = _u1.Count != 0 ? Calc.MyRandom(_u1.Count - 1) : 0;
            double _in = Calc.NormalizeRange(Math.Exp(rand), 0.0d, Math.Exp(_u1.Count - 1), 0.0d, (double)(_u1.Count - 1));
            
            UNIT _u = dir_up ?
                _u1[(int)_in] :
                _u2.Where(x => Newton.NewtonForce(x) > Newton.NewtonForce(curr)).LastOrDefault();

            return _u;
        }/**/

        /*private double _NearEnergy1(Direction dir)
        {
            //double f_h = Newton.NewtonForce(UNIT.CreateMIN());
            //double f_h = Statics.HighestForceD() - Statics.LowestForceD();
            //double f_l = Statics.LowestForceD();
            double f_h = Statics.HighestForce().Variable;

            double _v = dir.d_momentum;
            double v_h = Params.pos_x_low;
            double v_l = Params.pos_x_high;

            if (dir.is_momentum)
            {
                _v = Params.GetMechanics().dir.d_momentum;
                v_h = Params.GetMechanics().out_high;
                v_l = Params.GetMechanics().out_low;
            }
                       
            double percent = Calc.NormalizeRange(_v, v_l, v_h, 0.0d, 100.0d);
            double res = f_h * (percent / 100.0d);               //calc "best guess" force


            return res;
        }/**/

        /*private double _NearEnergy2(Direction dir)
        {
            IMechanics mech = Params.GetMechanics();
            double f_h = Statics.HighestForce().Variable;
            double f_l = Statics.LowestForce().Variable;

            double _v = mech.dir.d_momentum;
            double v_h = mech.out_high;
            double v_l = mech.out_low;

            double norm = Calc.NormalizeRange(_v, v_l, v_h, 0.0d, 100.0d);

            //if(dir.is_momentum && Params.output == Enums.OUTPUT.XXX)
            //{
            //    double index = Calc.NormalizeRange(_v, v_l, v_h, 0.0d, 1.0d);
            //    double _a = 1.0d;// - index;
            //    _v = Math.Pow(_v, -2);
            //    _v = _a * _v;

            //    v_l = Math.Pow(v_l, -2);
            //    v_l = _a * v_l;

            //    v_h = Math.Pow(v_h, -2);
            //    v_h = _a * v_h;
            //}

            double res = -1;
            double percent;

            switch (Params.mech)
            {
                case MECHANICS.WHEEL:
                case MECHANICS.CONTEST:
                case MECHANICS.HILL:
                    double f_dif = f_h - f_l;
                    percent = norm;
                    res = f_l + (f_dif * (percent / 100.0d));//calc "best guess" force
                    break;
                default:
                    percent = norm;
                    res = f_h * (percent / 100.0d);//calc "best guess" force
                    break;
            }

            return res;
        }/**/

        /*private double NearPercent(bool dir_up)
        {
            //double f_h = Newton.NewtonForce(UNIT.CreateMIN());
            //double f_l = Statics.LowestForceD();
            double _m = Params.GetMechanics().dir.d_momentum;
            double m_h = Params.GetMechanics().out_high;
            double m_l = Params.GetMechanics().out_low;

            double range = m_h + -m_l;                                  //find range
            double momentum = dir_up ? -m_l + _m : -(m_l + -_m);        //absolutice momentum
            double percent = (momentum * 100.0d) / range;               //calc range percentage
            //double res      = f_h * (percent / 100.0d);               //calc "best guess" force

            return percent;
        }/**/


        /*public UNIT NextUnit(UNIT curr_unit, Direction dir)
        {
            if (curr_unit == null)
                throw new ArgumentNullException();

            UNIT new_unit = GetUnit(curr_unit, dir);

            return new_unit;
        }/**/
    }
}
