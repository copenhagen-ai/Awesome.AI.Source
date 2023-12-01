namespace Awesome.AI.Common
{
    public class HUB
    {
        /*
         * maybe HUB should be renamed GROUP
         * */
        public string subject { get; set; }
        public double percent { get; set; }

        ///public BNet net_kutza;
        //public ActivationNetwork net_accord;
        //public BackPropagationLearning teacher;
        public List<UNIT> units;
        
        private HUB(string subject, List<UNIT> units, bool? is_accord, int?[] neurons, double? learningrate, double? momentum)
        {
            //CreateNet(is_accord, neurons, learningrate, momentum);
                        
            this.subject = subject;
            this.percent = 0.0d;
            this.units = units;
        }

        private HUB()
        {            
        }

        public static HUB Create(string subject, List<UNIT> units, bool? is_accord, int?[] neurons, double? learningrate, double? momentum)
        {
            HUB h = new HUB(subject, units, is_accord, neurons, learningrate, momentum);
            return h;
        }

        //private void CreateNet(bool? is_accord, int?[] neurons, double? learningrate, double? momentum)
        //{
        //    net_accord = null;
        //    net_kutza = null;

        //    if (neurons == null)
        //        return;

        //    if ((bool)is_accord)
        //    {
        //        IActivationFunction function = new SigmoidFunction();
        //        net_accord = new ActivationNetwork(function,
        //            inputsCount: (int)neurons[0], neuronsCount: new[] { (int)neurons[1], (int)neurons[2] });

        //        teacher = new BackPropagationLearning(net_accord)
        //        {
        //            LearningRate = (double)learningrate,
        //            Momentum = (double)momentum
        //        };
        //    }
        //    else
        //        net_kutza = new BNet(neurons, 1, (double)learningrate, (double)momentum);//neurons, slope, learningrate, momentum
        //}

        public void AddData(UNIT u)
        {
            if (u == null)
                throw new ArgumentNullException();

            if (units == null)
                units = new List<UNIT>();
            units.Add(u);
        }

        public string GetSubject()
        {
            return subject.Split(':')[0];
        }

        public bool IsIDLE()
        {
            return GetSubject() == "IDLE";
        }

        public bool IsLEARNING()
        {
            return GetSubject() == "LEARNING";
        }

        //public void Train(double[] _input, double[] _target, out double[] _out)
        //{
        //    if (net_accord == null)
        //        TrainKutza(_input, _target, out _out);
        //    else 
        //        TrainAccord(_input, _target, out _out);
        //}

        //public void Eval(int _output_nodes, double[] _input, out double[] _out)
        //{
        //    if (net_accord == null)
        //        EvalKutza(_output_nodes, _input, out _out);
        //    else
        //        EvalAccord(_input, out _out);
        //}

        //public void TrainAccord(double[] _input, double[] _target, out double[] _out)
        //{
        //    double[][] inputs = { _input };
        //    double[][] targets = { _target };
        //    teacher.Run(_input, _target);
        //    ///*error = */teacher.RunEpoch(inputs, targets);
        //    double[][] _output = inputs.Apply(net_accord.Compute);
        //    _out = _output.GetRow(0);
        //}

        //public void EvalAccord(double[] _input, out double[] _out)
        //{
        //    double[][] inputs = { _input };
        //    _out = inputs.Apply(net_accord.Compute).GetRow(0);
        //}

        //public void TrainKutza(double[] _input, double[] _target, out double[] _out)
        //{
        //    net_kutza.TrainNet(_input, _target, Params.learning_epochs, out _out);
        //}

        //public void EvalKutza(int _output_nodes, double[] _input, out double[] _out)
        //{
        //    double[] _target = new double[_output_nodes];
        //    for (int i = 0; i < _output_nodes; i++)
        //        _target[i] = .5d;
        //    net_kutza.EvaluateNet(_input, _target, out _out);
        //}
    }    
}
