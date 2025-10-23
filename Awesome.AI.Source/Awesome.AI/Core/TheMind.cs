﻿using Awesome.AI.Awesome.AI.Core;
using Awesome.AI.Common;
using Awesome.AI.CoreInternals;
using Awesome.AI.CoreSystems;
using Awesome.AI.Interfaces;
using Awesome.AI.Variables;
using System.Diagnostics;
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
        public Down down;
        public Properties props;
        public TheSoup soup;
        public Memory mem;
        public Core core;
        public QuickDecision _quick;
        public LongDecision _long;
        public MoodGenerator mood;
        public Monologue mono;
        public Calc calc;
        public MyRandom rand;
        public Filters filters;
        public Out _out;
        public MyInternal _internal;
        public MyExternal _external;
        public MyQubit quantum;
        public MyProbabilityHelper prob;

        private List<string> zzzz = new List<string>() { "z_noise", "z_mech" };

        public Dictionary<string, IMechanics> mech { get; set; }
        public Dictionary<string, Params> parms { get; set; }
        private Dictionary<string, string> long_dec {  get; set; }
                
        public Stats stats = new Stats();
        public UNIT theanswer;
                
        public MINDS mindtype;
        public MECHANICS _mech;

        public bool goodbye { get; set; }
        public bool ok { get; set; }
        public bool do_process{ get; set; }
        public bool chat_answer { get; set; }
        public bool chat_asked { get; set; }
                
        public string z_current { get; set; }
        public string hobby = "socializing";
        public int epochs = 1;
        public int cycles = 0; // Go TRON!
        public int cycles_all = 0;
        public double user_var = 0.0d;

        public STATE STATE { get; set; } = STATE.JUSTRUNNING;
        
        public UNIT unit_current { get; set; }

        public IMechanics mech_current { get { return mech[z_current]; } set { mech[z_current] = value; } }
        public IMechanics mech_high { get { return mech["z_mech"]; } set { mech["z_mech"] = value; } }
        public IMechanics mech_noise { get { return mech["z_noise"]; } set { mech["z_noise"] = value; } }

        public Params parms_current { get { return parms[z_current]; } set { parms[z_current] = value; } }
        public Params parms_high { get { return parms["z_mech"]; } set { parms["z_mech"] = value; } }
        public Params parms_noise { get { return parms["z_noise"]; } set { parms["z_noise"] = value; } }

        public TheMind(MECHANICS m, MINDS mindtype, Dictionary<string, string> long_dec)
        {
            try
            {
                this._mech = m;
                this.mindtype = mindtype;
                this.long_dec = long_dec;
                z_current = "z_mech";

                parms = new Dictionary<string, Params>();
                foreach (string s in zzzz)
                    parms[s] = new Params(this);

                mech = new Dictionary<string, IMechanics>();
                mech_high = parms_high.GetMechanics(_mech);
                mech_noise = parms_noise.GetMechanics(MECHANICS.NOISE);

                down = new Down(this);
                props = new Properties(this);
                soup = new TheSoup(this);
                calc = new Calc(this);
                rand = new MyRandom(this);
                _internal = new MyInternal(this);
                _external = new MyExternal(this);
                filters = new Filters(this);
                _out = new Out(this);
                _long = new LongDecision(this, this.long_dec);
                _quick = new QuickDecision(this);
                mood = new MoodGenerator(this);
                mono = new Monologue(this);
                quantum = new MyQubit();
                prob = new MyProbabilityHelper();
                mem = new Memory(this);

                Random random = new Random();
                int u_count = mem.UNITS_ALL().Count;
                int rand1 = random.Next(u_count);
                int rand2 = random.Next(u_count);
                int rand3 = random.Next(u_count);

                core = new Core(this, rand1, rand2, rand3);

                foreach (string s in zzzz)
                {
                    if (mindtype == MINDS.ANDREW)
                        unit_current = mem.UNITS_ALL().Where(x => x.Root == "_fembots1").First();
                    if (mindtype == MINDS.ROBERTA)
                        unit_current = mem.UNITS_ALL().Where(x => x.Root == "_macho machines1").First();
                }
                
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

                Debug.WriteLine(msg);                
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
            try
            {
                cycles++;
                cycles_all++;

                if (!ok)
                    return;
            
                //Lists();

                if (do_process)
                    epochs++;

                bool _pro = do_process;
                do_process = false;

                foreach (string s in zzzz)
                {
                    z_current = s;

                    //Randomize(_pro);
                    PreRun(z_current, _pro);

                    if (!Core(_pro))//the basics
                        ok = false;

                    CorePost(_pro);
                    Systems(_pro);
                    PostRun(_pro);                    
                }
                
                if (_pro) 
                    cycles = 0;
            }
            catch (Exception _e)
            {
                string msg = "run - " + _e.Message + "\n";
                msg += _e.StackTrace;
                
                Debug.WriteLine(msg);
                
                ok = false;
            }
        }

        private void PreRun(string current, bool _pro)
        {
            rand.SaveMomentum(current, mech_current.mp.d_curr);

            _quick.Run(unit_current);            
        }

        private void PostRun(bool _pro)
        {
            if (z_current == "z_noise")
                _out.SetNoise();

            if (z_current == "z_mech")
                _out.SetMech();

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
            
            if (unit_current.IsIDLE())
                return true;

            mech_noise.Calculate(PATTERN.NONE, cycles);
            mech_high.Calculate(PATTERN.MOODGENERAL, cycles);//mood general
            mech_high.Calculate(PATTERN.MOODGOOD, cycles);//mood good
            mech_high.Calculate(PATTERN.MOODBAD, cycles);//mood bad

            down.Update();
            props.Update();
            //pos.Update();

            //if (curr_hub.IsIDLE())
            //    core.SetTheme(_pro);

            if (!core.OK(out user_var))
                return false;
            return true;
        }

        private void CorePost(bool _pro)
        {
            soup.NextUnit();
            core.History();
            core.Common();
            core.Stats(_pro);
        }

        private void Systems(bool _pro)
        {
            if (STATE == STATE.QUICKDECISION)
                return;

            foreach(var kv in this.long_dec)
                _long.Decide(_pro, kv.Key);

            if (z_current == "z_noise")
                return;

            mood.Generate(_pro);
            mood.MoodOK(_pro);
            mono.Create(_pro);
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
