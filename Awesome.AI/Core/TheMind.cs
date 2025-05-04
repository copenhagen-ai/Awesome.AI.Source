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
        public TheSoup matrix;
        public Core core;
        public Memory mem;
        public QuickDecision _quick;
        public LongDecision _long;
        public MoodGenerator mood;
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

        private List<string> zzzz = new List<string>() { "z_noise", "z_mech" };

        public Dictionary<string, IMechanics> mech { get; set; }
        public Dictionary<string, Params> parms { get; set; }
        public Dictionary<string, UNIT> unit { get; set; }
        private Dictionary<string, string> long_deci {  get; set; }

        
        public Stats stats = new Stats();
        public UNIT theanswer;
                
        public MINDS mindtype;
        public MECHANICS _mech;
        public HARDDOWN goodbye = HARDDOWN.NO;

        public static bool ok { get; set; }
        public string z_current { get; set; }
        public bool do_process{ get; set; }
        public bool chat_answer { get; set; }
        public bool chat_asked { get; set; }
                
        public string hobby = "socializing";
        public int epochs = 1;
        public int cycles = 0; // Go TRON!
        public int cycles_all = 0;
        public double user_var = 0.0d;


        //public int correct_thinking = 0;
        //public int not_correct_thinking = 0;
        //public int _near_death = 0;
        //public int valid_units = 0;
        //public bool theme_on = false;
        //public string theme = "none";
        //public string theme_old = "";
        //public List<KeyValuePair<string, int>> themes_stat = new List<KeyValuePair<string, int>>();


        public STATE STATE { get; set; } = STATE.JUSTRUNNING;
        

        //these coordinates could be viewed as going a long the z-axis
        public UNIT unit_current { get { return unit[z_current]; } set { unit[z_current] = value; } }
        public UNIT unit_mechanics { get { return unit["z_mech"]; } set { unit["z_mech"] = value; } }
        public UNIT unit_noise { get { return unit["z_noise"]; } set { unit["z_noise"] = value; } }

        public IMechanics mech_current { get { return mech[z_current]; } set { mech[z_current] = value; } }
        public IMechanics mech_mechanics { get { return mech["z_mech"]; } set { mech["z_mech"] = value; } }
        public IMechanics mech_noise { get { return mech["z_noise"]; } set { mech["z_noise"] = value; } }

        public Params parms_current { get { return parms[z_current]; } set { parms[z_current] = value; } }
        public Params parms_mechanics { get { return parms["z_mech"]; } set { parms["z_mech"] = value; } }
        public Params parms_noise { get { return parms["z_noise"]; } set { parms["z_noise"] = value; } }

        public TheMind(MECHANICS m, MINDS mindtype, Dictionary<string, string> long_deci)
        {
            try
            {
                this._mech = m;
                this.mindtype = mindtype;
                this.long_deci = long_deci;
                z_current = "z_mech";

                parms = new Dictionary<string, Params>();
                foreach (string s in zzzz)
                    parms[s] = new Params(this);

                mech = new Dictionary<string, IMechanics>();
                mech_mechanics = parms_mechanics.GetMechanics(_mech);
                mech_noise = parms_noise.GetMechanics(MECHANICS.NOISE);

                matrix = new TheSoup(this);
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
                mood = new MoodGenerator(this);
                dir = new Direction(this);
                pos = new Position(this);
                quantum = new MyQubit();
                mem = new Memory(this);

                unit = new Dictionary<string, UNIT>();
                
                foreach (string s in zzzz)
                {
                    if (mindtype == MINDS.ANDREW)
                        unit[s] = mem.UNITS_ALL().Where(x => x.Root == "_fembots1").First();
                    if (mindtype == MINDS.ROBERTA)
                        unit[s] = mem.UNITS_ALL().Where(x => x.Root == "_macho machines1").First();
                }



                parms_mechanics.UpdateLowCut();
                //parms["noise"].UpdateLowCut();

                PreRun("z_noise", true);
                PostRun(true);

                theanswer = UNIT.Create(this, "GUID", -1.0d, "I dont Know", "SPECIAL", UNITTYPE.JUSTAUNIT, LONGTYPE.NONE);//set it to "It does not", and the program terminates

                ok = true;
                do_process = false;
            
                ProcessPass();
                        
                //Lists();
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
            if (z_current == "z_noise")
                return;

            List<UNIT> list = mem.UNITS_VAL();

            List<Tuple<string, bool, double>> units_force = new List<Tuple<string, bool, double>>();
            foreach (UNIT u in list.OrderBy(x => x.Variable).ToList())
                units_force.Add(new Tuple<string, bool, double>(u.Root, u.IsValid, u.Variable));

            //List<Tuple<string, bool, double>> units_mass = new List<Tuple<string, bool, double>>();
            //foreach (UNIT u in list.OrderBy(x => x.HighAtZero).ToList())
            //    units_mass.Add(new Tuple<string, bool, double>(u.root, u.IsValid, u.HighAtZero));

            List<UNIT> list1 = list.OrderBy(x => x.Index).ToList();
            List<UNIT> list2 = list.OrderBy(x => x.Variable).ToList();
            List<UNIT> list3 = list.Where(x => filters.LowCut(x)).OrderBy(x => x.Variable).ToList();

            int valid_units = units_force.Count;

            ;
        }
        
        public void Run(object sender, MicroLibrary.MicroTimerEventArgs timerEventArgs)
        {
            cycles++;
            cycles_all++;

            if (!ok)
                return;
            
            Lists();

            if (do_process)
                epochs++;

            bool _pro = do_process;
            do_process = false;

            try
            {
                foreach (string s in zzzz)
                {
                    z_current = s;

                    //Randomize(_pro);
                    PreRun(z_current, _pro);

                    if (!Core(_pro))//the basics
                        ok = false;

                    TheSoup();//find new curr_unit/curr_hub
                    PostRun(_pro);
                    Process(_pro);
                    Systems(_pro);

                    _out.Set();
                }
            }
            catch (Exception _e)
            {
                string msg = "run - " + _e.Message + "\n";
                msg += _e.StackTrace;
                XmlHelper.WriteError(msg);

                ok = false;
            }
            finally
            {
                if (_pro) 
                    cycles = 0;                
            }
        }

        private void PreRun(string current, bool _pro)
        {
            rand.SaveMomentum(current, mech_current.d_curr);

            _quick.Run(_pro, unit_current);            
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

            core.UpdateCredit();
            core.AnswerQuestion();
            
            if (unit[z_current].IsIDLE())
                return true;

            mech_noise.CalcPattern1(PATTERN.NONE, 0);
            mech_mechanics.CalcPattern1(parms_current.pattern, cycles);//mood general
            mech_mechanics.CalcPattern2(parms_current.pattern, cycles);//mood good
            mech_mechanics.CalcPattern3(parms_current.pattern, cycles);//mood bad
            
            dir.Update();
            pos.Update();

            //if (curr_hub.IsIDLE())
            //    core.SetTheme(_pro);

            if (!core.OK(out user_var))
                return false;
            return true;
        }

        private void TheSoup() 
        {
            //if (unit["noise"].IsIDLE())
            //    unit["noise"] = matrix.NextUnit(UNIT.IDLE_UNIT(this));

            //if (unit["mech"].IsIDLE())
            //    unit["mech"] = matrix.NextUnit(UNIT.IDLE_UNIT(this));

            //if (unit["noise"].IsIDLE() || unit["mech"].IsIDLE())
            //    return;

            unit_current = matrix.NextUnit(unit_current);
        }

        private void Process(bool _pro)
        {
            process.History();
            process.Common();
            process.Stats(_pro);
        }

        private void Systems(bool _pro)
        {
            if (STATE == STATE.QUICKDECISION)
                return;

            foreach(var kv in this.long_deci)
                _long.Decide(_pro, kv.Key);

            if (z_current == "z_noise")
                return;

            mood.Generate(_pro);
            mood.MoodOK(_pro);
        }

        private async void ProcessPass()
        {
            while (ok)
            {
                //if (current == "noise")
                //    continue;

                do_process = true;
                await Task.Delay(2023);
            }
        }                
    }
}
