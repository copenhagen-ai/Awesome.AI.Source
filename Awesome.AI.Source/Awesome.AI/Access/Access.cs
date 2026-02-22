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

    public class ApiInstance
    {
        public string o_json {  get; set; }
        public string s_hits_json { get; set; }
        public string s_units_json { get; set; }
        public MyOutVars o_vars { get; set; }
        public MyStats s_vars { get; set; }
        public MINDS mindtype { get; set; }
    }
}
