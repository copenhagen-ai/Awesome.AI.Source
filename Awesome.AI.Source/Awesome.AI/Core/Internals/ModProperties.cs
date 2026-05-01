using Awesome.AI.Source.Awesome.AI.Common;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core.Internals
{
    public class ModProperties
    {
        public class MyModifiers
        {
            public double Mod(TheMind mind, double value, string prop)
            {
                double res = 0.0d;

                if (prop == "base")
                    res = value;
                else
                {
                    switch (prop)
                    {
                        case CONST.prop2_temperament:
                            PATTERN patt = mind.mech.mp.pattern_curr;
                            if (patt == PATTERN.MOODGENERAL) res = mind.calc.Normalize(value, 0.0d, 100.0d, 10.0d, 90.0d);
                            if (patt == PATTERN.MOODGOOD) res = mind.calc.Normalize(value, 0.0d, 100.0d, 60.0d, 90.0d);
                            if (patt == PATTERN.MOODBAD) res = mind.calc.Normalize(value, 0.0d, 100.0d, 10.0d, 40.0d);
                            break;
                        case CONST.prop2_brain: throw new Exception("ModProperties, Mod 1");
                        case CONST.prop3_brain: throw new Exception("ModProperties, Mod 2");
                        case CONST.prop2_comm: throw new Exception("ModProperties, Mod 3");
                        case CONST.prop3_comm: throw new Exception("ModProperties, Mod 4");
                        case CONST.prop4_comm: throw new Exception("ModProperties, Mod 5");
                        default: throw new Exception("ModProperties, Mod 6");
                    }
                }

                return res;                
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

                foreach (var v in _data)
                    res *= this[v.Key.Item1, key2];

                return res * val;
            }
        }

        private MyModifiers Mods { get; set; }
        private MyMatrix Matrix { get; set; }
        public Dictionary<string, double> PropsIn { get; set; }
        public Dictionary<string, double> PropsOut { get; set; }
        
        private TheMind mind;
        private ModProperties() { }
        public ModProperties(TheMind mind, PROPS props)
        {
            this.mind = mind;

            GetProps(props, out Dictionary<string, double> attr, out Dictionary<(string, string), double> matr);

            PropsOut = new Dictionary<string, double>();
            PropsIn = attr;
            Mods = new MyModifiers();
            Matrix = new MyMatrix();

            foreach (var kv in attr)
                PropsOut.Add(kv.Key, 0.0d);

            foreach (var kv in matr)
                Matrix[kv.Key.Item1, kv.Key.Item2] = kv.Value;
        }

        public void Update()
        {
            PROPS props = mind.bot.props;

            GetBase(props, out double _norm);

            foreach (var kv in PropsIn)
            {
                string prop = kv.Key;
                double res = 0.0d;

                res = Mods.Mod(mind, _norm, prop);
                //res = Matrix.Run(res, prop);
                PropsOut[prop] = res;
            }
        }

        private void GetProps(PROPS props, out Dictionary<string, double> attr, out Dictionary<(string, string), double> matr)
        {
            switch (props)
            {
                case PROPS.TEMPERAMENT:
                    //will, mood
                    attr = new Dictionary<string, double> { { "base", double.NaN }, { CONST.prop2_temperament, 2.0 } };

                    matr = new Dictionary<(string, string), double>();
                    matr.Add(("base", CONST.prop1_temperament), 1.0d);
                    break;

                case PROPS.BRAINWAVE:
                    //conflict, readyness
                    attr = new Dictionary<string, double> { { "base", double.NaN }, { CONST.prop2_brain, 2.0 }, { CONST.prop3_brain, 1.2 } };

                    matr = new Dictionary<(string, string), double>();
                    matr.Add(("base", CONST.prop3_brain), 1.0d);
                    matr.Add((CONST.prop2_brain, CONST.prop3_brain), 1.0d);
                    break;

                case PROPS.COMMUNICATION:
                    //opinion, temporality, abstraction
                    attr = new Dictionary<string, double> { { "base", double.NaN }, { CONST.prop2_comm, 2.0 }, { CONST.prop3_comm, 0.5 }, { CONST.prop4_comm, 1.2 } };

                    matr = new Dictionary<(string, string), double>();
                    matr.Add(("base", CONST.prop3_comm), 0.65d);
                    matr.Add((CONST.prop2_comm, CONST.prop3_comm), 0.45d);
                    matr.Add((CONST.prop4_comm, CONST.prop2_comm), 0.35d);

                    break;

                default: throw new NotImplementedException("Properties, GetProps");
            }
        }

        private void GetBase(PROPS props, out double _norm)
        {
            switch (props)
            {
                case PROPS.TEMPERAMENT: _norm = mind.mech.mp.eprops.Will().Norm100VV(mind); break;
                //case PROPS.BRAINWAVE: _norm = mind.mech.mp.eprops.Will().Norm0(mind, 0.0d, 100.0d); break;
                //case PROPS.COMMUNICATION: _norm = mind.mech.mp.eprops.Will().Norm0(mind, 0.0d, 100.0d); break;
                default: throw new NotImplementedException("Properties, GetProps");
            }
        }
    }
}
