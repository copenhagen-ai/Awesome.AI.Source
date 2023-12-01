using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awesome.AI.Common
{
    public class Stat
    {
        public string name { get; set; }
        public int count_all { get; set; }
        public double force { get; set; }
        public double conv_index { get; set; }
    }

    public class Stats
    {
        public string curr_name { get; set; }
        public int curr_value { get; set; }
        public string reset_name { get; set; }
        public int reset_value { get; set; }

        public List<Stat> list = new List<Stat>();
    }

}
