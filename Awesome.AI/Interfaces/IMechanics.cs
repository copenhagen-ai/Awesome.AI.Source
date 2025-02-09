using Awesome.AI.Common;

namespace Awesome.AI.Interfaces
{
    public interface IMechanics
    {
        double Fsta { get; set; }
        double Fdyn { get; set; }

        double out_high { get; set; }
        double out_low { get; set; }
        double posx_high { get; set; }
        double posx_low { get; set; }
        double momentum { get; set; }
                
        double POS_XY { get; }

        void Calculate();//do calculations
        double Variable(UNIT c);//the force, mass, distance, acceleration etc
    }
}


















/*using Awesome.AI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Awesome.AI.Helpers.Enums;

namespace Awesome.AI.Interfaces
{
    public abstract class Mechanics<T> where T : new()
    {
        public static double momentum = -1000.0d;
        private static T inst = default(T);
        public abstract double ApplyForceStatic();
        public abstract double ApplyForceDynamic(UNIT curr_unit_th, THECHOISE goodbye);
        public T Inst()
        {
            if (inst.IsNull())
                inst = new T();
            return inst;
        }
    }
}/**/