using Awesome.AI.Awesome.AI.Core;
using Awesome.AI.Core;
using Awesome.AI.Core.Mechanics;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Interfaces
{
    public interface IMechanics
    {
        MechParams mp { get; set; }

        double POS_XY { get; }

        //these are thought patterns: general, good, bad
        void CalcPattern(PATTERN pattern, PATTERN match, int cycles);

        void Peek(UNIT c);        
    }
}