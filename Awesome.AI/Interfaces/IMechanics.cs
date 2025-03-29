using Awesome.AI.Core;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Interfaces
{
    public interface IMechanics
    {
        //double Fsta { get; set; }
        //double Fdyn { get; set; }

        double m_out_high { get; set; }
        double m_out_low { get; set; }
        double d_out_high { get; set; }
        double d_out_low { get; set; }
        double posx_high { get; set; }
        double posx_low { get; set; }
        double momentum { get; set; }
        double momentumPrev { get; set; }
        double deltaMom { get; set; }
        double deltaMomPrev { get; set; }

        FUZZYDOWN FuzzyMom { get; }
        HARDDOWN HardMom { get; }

        double POS_XY { get; }

        double HighestVar { get; }
        double LowestVar { get; }

        void CalculateOld();//do calculations
        void CalculateNew(int cycles);//do calculations
        double Variable(UNIT c);//the force, mass, distance, acceleration etc
    }
}