using Awesome.AI.Common;
using Awesome.AI.CoreInternals;
using Awesome.AI.CoreSystems;
using Awesome.AI.Interfaces;
using Awesome.AI.Variables;
using Awesome.AI.Web.AI.Common;
using Awesome.AI.Web.Helpers;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Core
{
    /*
     * Notes:
     * Maybe its not about creating a replica of the brain, but building upon the basics ie. TheMatrix, Mechanics etc
     * Maybe the question should be: "What can be done with this setup"
     * */

    public class TheMind
    {
        public TheMatrix matrix;
        public Core core;
        public Memory mem;
        public QuickDecision _quick;
        public LongDecision _long;
        public Params parms;
        public Calc calc;
        public MyRandom rand;
        public Process process;
        public Filters filters;
        public Out _out;
        public MyInternal _internal;
        public MyExternal _external;
        public Direction dir;
        public Position pos;
        public MyQubit quantum;

        public HUB curr_hub;
        public UNIT curr_unit;
        public UNIT theanswer;

        public IMechanics mech = null;

        public MINDS mindtype;
        public MECHANICS _mech;
        public HARDDOWN goodbye = HARDDOWN.NO;

        private bool do_process = false;
                
        public int epochs = 1;
        public int cycles = 0; // Go TRON!
        public int cycles_all = 0;
        public int correct_thinking = 0;
        public int not_correct_thinking = 0;
        public int _near_death = 0;
        public double user_var = 0.0d;
        public int valid_units = 0;

        public bool chat_answer { get; set; }
        public bool chat_asked { get; set; }

        private Dictionary<string, string> long_deci {  get; set; }
        
        //public bool theme_on = false;
        //public string theme = "none";
        //public string theme_old = "";
        public string hobby = "socializing";

        public List<KeyValuePair<string, int>> themes_stat = new List<KeyValuePair<string, int>>();
        public Stats stats = new Stats();
        
        public TheMind(MECHANICS m, MINDS mindtype, Dictionary<string, string> long_deci)
        {
            try
            {
                this._mech = m;
                this.mindtype = mindtype;
                this.long_deci = long_deci;
                
                parms = new Params(this);
                matrix = new TheMatrix(this);
                calc = new Calc(this);
                rand = new MyRandom(this);
                process = new Process(this);
                _internal = new MyInternal(this);
                _external = new MyExternal(this);
                filters = new Filters(this);
                core = new Core(this);
                _out = new Out(this);
                _long = new LongDecision(this, this.long_deci);
                _quick = new QuickDecision(this);
                dir = new Direction(this);
                pos = new Position(this);
                quantum = new MyQubit();
                mem = new Memory(this, Constants.NUMBER_OF_UNITS);

                mech = parms.GetMechanics(_mech);
                parms.UpdateLowCut();

                //if (mindtype == MINDS.STANDARD)
                //    curr_unit = mem.UNITS_ALL().Where(x => x.root == "_love1").FirstOrDefault();
                if (mindtype == MINDS.ANDREW)
                    curr_unit = mem.UNITS_ALL().Where(x => x.root == "_fembots1").FirstOrDefault();
                if (mindtype == MINDS.ROBERTA)
                    curr_unit = mem.UNITS_ALL().Where(x => x.root == "_macho machines1").FirstOrDefault();

                curr_hub = curr_unit.HUB;

                PreRun(true);
                PostRun(true);

                theanswer = UNIT.Create(this, -1.0d, "I dont Know", "null", "SPECIAL", UNITTYPE.JUSTAUNIT, LONGTYPE.NONE);//set it to "It does not", and the program terminates
            
                ProcessPass();
                        
                Lists();
            }
            catch (Exception _e)
            {
                string msg = "themind - " + _e.Message + "\n";
                msg += _e.StackTrace;
                XmlHelper.WriteError(msg);                
            }
        }
        
        private void Lists()
        {
            List<UNIT> list = mem.UNITS_VAL();

            List<Tuple<string, bool, double>> units_force = new List<Tuple<string, bool, double>>();
            foreach (UNIT u in list.OrderBy(x => x.Variable).ToList())
                units_force.Add(new Tuple<string, bool, double>(u.root, u.IsValid, u.Variable));

            //List<Tuple<string, bool, double>> units_mass = new List<Tuple<string, bool, double>>();
            //foreach (UNIT u in list.OrderBy(x => x.HighAtZero).ToList())
            //    units_mass.Add(new Tuple<string, bool, double>(u.root, u.IsValid, u.HighAtZero));

            List<UNIT> list1 = list.OrderBy(x => x.Index).ToList();
            List<UNIT> list2 = list.OrderBy(x => x.Variable).ToList();
            List<UNIT> list3 = list.Where(x => filters.LowCut(x)).OrderBy(x => x.Variable).ToList();

            valid_units = units_force.Count;

            ;
        }
        
        public bool run = true;
        public bool ok = true;
        public void Run(object sender, MicroLibrary.MicroTimerEventArgs timerEventArgs)
        {
            try
            {                
                if (!ok)
                    return;
                
                if (!run)
                    return;

                run = false;

                Lists();

                if (do_process)
                    epochs++;

                bool _pro = do_process;
                do_process = false;
                
                //Randomize(_pro);
                PreRun(_pro);

                if (!Core(_pro))//the basics
                    ok = false;

                TheSoup();//find new curr_unit/curr_hub
                PostRun(_pro);
                Process(_pro);
                Systems(_pro);
                //Output(_pro);

                if (_pro) _out.Set();
                if (_pro) cycles = 0;
            }
            catch (Exception _e) 
            {
                string msg = "run - " + _e.Message + "\n";
                msg += _e.StackTrace;
                XmlHelper.WriteError(msg);
            }
            finally
            {
                run = true;
            }
        }

        private void PreRun(bool _pro)
        {
            //rand.SaveMomentum(mech.momentum);
            rand.SaveMomentum(mech.deltaMom);
            _quick.Run(_pro, curr_unit);
            
            //if (_pro)
            //    common.Reset();            
        }

        private void PostRun(bool _pro)
        {
            if (!_pro)
                return;

            _internal.Reset();
            _external.Reset();
        }

        private bool Core(bool _pro)
        {
            /*
             * This is the algorithm for producing thought/making the choise
             * - maybe Core() + TheSoup() could be made into at neural net all by it self, since "almost all" it does is choosing up or down 
             * */

            cycles++;
            cycles_all++;

            core.UpdateCredit();
            core.AnswerQuestion();
            
            if (curr_unit.IsIDLE())
                return true;

            mech.CalcPatternOld(parms.version);//mood old
            mech.CalcPattern1(parms.version , cycles);//mood general
            mech.CalcPattern2(parms.version, cycles);//mood good
            mech.CalcPattern3(parms.version, cycles);//mood bad

            dir.Update();
            pos.Update(_pro);//Enums.POSITION.NEW

            //if (curr_hub.IsIDLE())
            //    core.SetTheme(_pro);

            if (!core.OK(out user_var))
                return false;
            return true;
        }

        private void TheSoup() 
        {
            curr_hub = curr_unit.HUB;
            curr_unit = matrix.NextUnit();
        }

        private void Process(bool _pro)
        {
            process.History();
            process.Common();
            process.Stats(_pro);
        }

        private void Systems(bool _pro)
        {
            if (parms.state == STATE.QUICKDECISION)
                return;

            foreach(var kv in this.long_deci)
                _long.Decide(_pro, kv.Key);            
        }

        private async void ProcessPass()
        {
            while (true)
            {
                do_process = true;
                await Task.Delay(2000);
            }
        }                
    }
}
