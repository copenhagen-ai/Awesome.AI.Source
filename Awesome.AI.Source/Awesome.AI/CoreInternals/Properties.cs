using Awesome.AI.Common;
using Awesome.AI.Core;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Awesome.AI.Core
{
    public class Properties
    {
        public class MyModifiers
        {
            public double Mod(double _base, double value, string prop, Dictionary<string, double> mods)
            {
                if (prop == "base")
                    _base = 1.0d;
                else
                {
                    foreach (var mod in mods)
                    {
                        if (mod.Key == "base")
                            continue;

                        if (mod.Key == prop)
                            _base *= mod.Value;
                    }
                }
                
                return _base * value;
            }
        }

        public class MyMatrix
        {
            private Dictionary<(string, string), double> _data = new();

            public double this[string key1, string key2]
            {
                get
                {
                    if (_data.TryGetValue((key1, key2), out var value))
                        return value;

                    return 1.0;
                }
                set => _data[(key1, key2)] = value;
            }

            public double Run(double val, string key2)
            {
                double res = 1.0d;

                foreach(var v in _data)
                    res *= this[v.Key.Item1, key2];

                return res * val;
            }
        }

        //private List<string> Attributes { get; set; }
        private MyModifiers Mods{ get; set; }
        private MyMatrix Matrix { get; set; }
        public Dictionary<string, double> PropsIn { get; set; }
        public Dictionary<string, double> PropsOut { get; set; }
        public double Base {  get; set; }

        private TheMind mind;
        private Properties() { }
        public Properties(TheMind mind, PROPS props)
        {
            this.mind = mind;

            GetProps(props, out double _base, out Dictionary<string, double> attr, out Dictionary<(string, string), double> mat);
            
            //int count = a.Length;

            PropsOut = new Dictionary<string, double>();
            PropsIn = attr;
            Base = _base;
            Mods = new MyModifiers();
            Matrix = new MyMatrix();
            
            foreach (var kv in attr) 
                PropsOut.Add(kv.Key, 0.0d);
                        
            foreach (var kv in mat)
                Matrix[kv.Key.Item1, kv.Key.Item2] = kv.Value;            
        }

        public void Update()
        {
            if (mind.z_current != "z_mech")
                return;

            var _norm = mind.mech_high.mp.d_100.Zero(mind);

            foreach (var kv in PropsIn)
            {
                string prop = kv.Key;
                double _base = Base;
                double res = 0.0d;

                res = Mods.Mod(_base, _norm, prop, PropsIn);
                res = Matrix.Run(res, prop);
                PropsOut[prop] = res;                        
            }
        }

        private void GetProps(PROPS props, out double _base, out Dictionary<string, double> attr, out Dictionary<(string, string), double> m)
        {
            switch (props)
            {
                case PROPS.BRAINWAVE:
                    _base = 1.0d;

                    attr = new Dictionary<string, double> { { "base", double.NaN }, { "attention", 2.0 }, { "readiness", 1.2 } };

                    m = new Dictionary<(string, string), double>();
                    m.Add(("base", "readiness"), 1.0d);
                    m.Add(("attention", "readiness"), 1.0d);
                    break;

                case PROPS.COMMUNICATION:
                    _base = 1.0d;

                    attr = new Dictionary<string, double> { { "base", double.NaN }, { "opinion", 2.0 }, { "temporality", 0.5 }, { "abstraction", 1.2 } };

                    m = new Dictionary<(string, string), double>();
                    m.Add(("mood", "temporality"), 0.65d);
                    m.Add(("opinion", "temporality"), 0.45d);
                    m.Add(("abstraction", "opinion"), 0.35d);

                    break;

                default: throw new NotImplementedException("Properties, GetProps");
            }
        }
    }
}
