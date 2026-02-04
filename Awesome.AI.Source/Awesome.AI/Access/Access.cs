using Awesome.AI.Common;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Access
{
    internal class AccessApi
    {        
    }

    public class ApiRequestSetup
    {
        public List<MINDS> bots { get; set; }
    }

    public class ApiRequestUpdate
    {
        public MINDS mindtype { get; set; }
        public CASE _case { get; set; }
        public VALUE _value { get; set; }
    }

    public class ApiRequestAnswer
    {
        public MINDS mindtype { get; set; }
    }

    //public class ApiBot
    //{
    //    public MECHANICS mech { get; set; }
    //    public MINDS mindtype { get; set; }

    //    public Dictionary<string, string> long_deci { get; set; }
    //}

    public class ApiInstance
    {
        public Out _out { get; set; }
        public Stats stats { get; set; }
        public MINDS mindtype { get; set; }
    }
}
