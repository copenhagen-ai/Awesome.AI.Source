using Awesome.AI.Common;
using Awesome.AI.Core;

namespace Awesome.AI.Awesome.AI.Core
{
    public class Properties
    {
        public class MyModifiers
        {
            public double Mod_A(double value, string prop)
            {
                double _base = -0.05;

                if (prop == "mood")//stronger damping for opinion
                    _base = 1.0;

                if (prop == "opinion")//stronger damping for opinion
                    _base *= 2.0;

                if (prop == "temporality")//stronger damping for temporality
                    _base *= 2.0;

                if (prop == "abstraction")//stronger damping for abstraction
                    _base *= 1.2;

                return _base * value;
            }

            public double Mod_B(double value, string prop)
            {
                double _base = -0.5;

                if (prop == "mood")//stronger damping for opinion
                    _base = 1.0;

                if (prop == "opinion")//stronger damping for opinion
                    _base = 1.0;

                if (prop == "temporality")//stronger damping for temporality
                    _base *= 1.5;

                if (prop == "abstraction")//stronger damping for abstraction
                    _base *= 0.5;

                return _base * value;
            }

            public double Run(double value, string prop)
            {
                value = Mod_A(value, prop);
                value = Mod_B(value, prop);

                return value;
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

        private MyModifiers Mods{ get; set; }
        private MyMatrix Matrix { get; set; }
        private List<string> Attributes { get; set; }
        public Dictionary<string, double> Props { get; set; }

        private TheMind mind;
        private Properties() { }
        public Properties(TheMind mind)
        {
            this.mind = mind;

            Attributes = new List<string> { "mood", "opinion", "temporality", "abstraction" };
            Props = new Dictionary<string, double>();

            Mods = new MyModifiers();
            Matrix = new MyMatrix();

            foreach (var prop in Attributes)
                Props.Add(prop, 1.0d);

            Matrix["mood", "temporality"] = 0.65d;
            Matrix["opinion", "temporality"] = 0.45d;
            Matrix["abstraction", "opinion"] = 0.35d;
        }

        public void Update()
        {
            if (mind.z_current != "z_mech")
                return;

            var _norm = mind.mech_high.mp.d_100.Zero(mind);
                        
            foreach (var type in Attributes)
            {
                double res = 0.0d;
                res = Mods.Run(_norm, type);
                res = Matrix.Run(res, type);

                Props[type] = res;
            }
        }        
    }
}
