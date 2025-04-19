using Awesome.AI.Core;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Interfaces
{
    public interface IMechanics
    {
        double m_out_high_c { get; set; }
        double m_out_low_c { get; set; }
        double m_out_high_n { get; set; }
        double m_out_low_n { get; set; }
        double d_out_high { get; set; }
        double d_out_low { get; set; }
        double posx_high { get; set; }
        double posx_low { get; set; }

        double n_momentum { get; set; }
        double p_curr { get; set; }
        double p_prev { get; set; }
        double p_delta { get; set; }
        double p_delta_prev { get; set; }

        FUZZYDOWN FuzzyMom { get; }
        HARDDOWN HardMom { get; }

        double POS_XY { get; }

        //these are thought patterns
        void CalcPattern1(PATTERN pattern, int cycles);//mood general
        void CalcPattern2(PATTERN pattern, int cycles);//good mood
        void CalcPattern3(PATTERN pattern, int cycles);//bad mood

        void Momentum(UNIT c);
    }
}