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
                foreach (var mod in mods)
                {
                    if (prop == "base")
                        _base = 1.0d;

                    else if (prop == mod.Key)
                        _base *= mod.Value;
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
        public Dictionary<string, double>[] PropsIn { get; set; }
        public Dictionary<string, double> PropsOut { get; set; }
        public List<double> Base {  get; set; }

        private TheMind mind;
        private Properties() { }
        public Properties(TheMind mind, PROPS props)
        {
            this.mind = mind;

            GetProps(props, out List<double> _base, out Dictionary<string, double>[] a, out Dictionary<(string, string), double> m);
            
            int count = a.Length;

            PropsIn = new Dictionary<string, double>[count];
            PropsOut = new Dictionary<string, double>();
            Base = new List<double>();
            Mods = new MyModifiers();
            Matrix = new MyMatrix();

            foreach (var v in _base)
                Base.Add(v);

            int _c = 0;
            foreach(var v in a)
            {
                PropsIn[_c] = new Dictionary<string, double>();
                PropsOut = new Dictionary<string, double>();
                foreach (var kv in v) {
                    PropsIn[_c].Add(kv.Key, kv.Value);
                    if (_c == 0)
                        PropsOut.Add(kv.Key, 0.0d);
                }
                _c++;
            }

            foreach (var kv in m)
                Matrix[kv.Key.Item1, kv.Key.Item2] = kv.Value;            
        }

        public void Update()
        {
            if (mind.z_current != "z_mech")
                return;

            var _norm = mind.mech_high.mp.d_100.Zero(mind);

            List<string> keys = PropsIn[0].Select(x => x.Key).ToList();

            int _c = 0;
            foreach (var props in PropsIn)
            {
                double res = 0.0d;
                foreach (var prop in keys)
                {
                    res = Mods.Mod(Base[_c], _norm, prop, props);
                    res = Matrix.Run(res, prop);
                    PropsOut[prop] = res;
                }
                _c++;
            }
        }

        private void GetProps(PROPS props, out List<double> _base, out Dictionary<string, double>[] attr, out Dictionary<(string, string), double> m)
        {
            switch (props)
            {
                case PROPS.BRAINWAVE:
                    _base = new List<double>() { 1.0, 1.0 };

                    Dictionary<string, double> dict1 = new Dictionary<string, double> { { "base", double.NaN }, { "attention", 2.0 }, { "readiness", 2.0 } };
                    Dictionary<string, double> dict2 = new Dictionary<string, double> { { "base", double.NaN }, { "attention", 1.0 }, { "readiness", 1.5 } };
                    attr = new Dictionary<string, double>[2];
                    attr[0] = dict1;
                    attr[1] = dict2;

                    m = new Dictionary<(string, string), double>();
                    m.Add(("base", "readiness"), 1.0d);
                    m.Add(("attention", "readiness"), 1.0d);
                    break;

                case PROPS.COMMUNICATION:
                    _base = new List<double>() { 1.0, 1.0 };

                    Dictionary<string, double> dict3 = new Dictionary<string, double> { { "base", double.NaN }, { "opinion", 2.0 }, { "temporality", 2.0 }, { "abstraction", 1.2 } };
                    Dictionary<string, double> dict4 = new Dictionary<string, double> { { "base", double.NaN }, { "opinion", 1.0 }, { "temporality", 1.5 }, { "abstraction", 0.5 } };
                    attr = new Dictionary<string, double> [2];
                    attr[0] = dict3;
                    attr[1] = dict4;

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
