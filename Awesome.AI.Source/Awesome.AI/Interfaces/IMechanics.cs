using Awesome.AI.Awesome.AI.Core;
using Awesome.AI.Core;
using Awesome.AI.Core.Mechanics;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Interfaces
{
    public interface IMechanics
    {
        MECHANICS type { get; set; }
        MechSymbolicOut ms { get; set; }
        MechParams mp { get; set; }

        double POS_XY { get; }

        void Peek(UNIT c);

        //these are thought patterns: general, good, bad
        void Calculate(PATTERN match, int cycles);
    }
}