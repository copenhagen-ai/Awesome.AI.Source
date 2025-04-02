using Awesome.AI.Core;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Interfaces
{
    public interface IMechanics
    {
        double m_out_high { get; set; }
        double m_out_low { get; set; }
        double d_out_high { get; set; }
        double d_out_low { get; set; }
        double posx_high { get; set; }
        double posx_low { get; set; }
        double p_curr { get; set; }
        double p_prev { get; set; }
        double p_delta { get; set; }
        double p_delta_prev { get; set; }

        FUZZYDOWN FuzzyMom { get; }
        HARDDOWN HardMom { get; }

        double POS_XY { get; }

        double HighestVar { get; }
        double LowestVar { get; }

        //these are thought patterns
        //void CalcPatternOld(MECHVERSION version);//do calculations
        void CalcPattern1(MECHVERSION version, int cycles);//mood general
        void CalcPattern2(MECHVERSION version, int cycles);//good mood
        void CalcPattern3(MECHVERSION version, int cycles);//bad mood

        double Variable(UNIT c);//force, mass, distance, acceleration etc
    }
}