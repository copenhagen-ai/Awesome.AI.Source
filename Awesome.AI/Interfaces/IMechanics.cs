using Awesome.AI.Core;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Interfaces
{
    public interface IMechanics
    {
        double posx_high { get; set; }
        double posx_low { get; set; }

        double peek_momentum { get; set; }
        double peek_norm { get; set; }
        double p_100 { get; set; }
        double d_100 { get; set; }
        double p_90 { get; set; }
        double d_90 { get; set; }
        double p_curr { get; set; }
        double p_prev { get; set; }
        double d_curr { get; set; }
        double d_prev { get; set; }

        FUZZYDOWN FuzzyMom { get; }
        HARDDOWN HardMom { get; }

        double POS_XY { get; }

        //these are thought patterns
        void CalcPattern1(PATTERN pattern, int cycles);//mood general
        void CalcPattern2(PATTERN pattern, int cycles);//good mood
        void CalcPattern3(PATTERN pattern, int cycles);//bad mood

        void Peek(UNIT c);
        void Normalize();
    }
}