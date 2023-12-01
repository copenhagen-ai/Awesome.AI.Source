using Awesome.AI.Common;
using Awesome.AI.CoreHelpers;
using Awesome.AI.Helpers;
using Awesome.AI.Interfaces;
using Awesome.AI.Systems.Externals;
using Awesome.AI.Web.AI.Common;
using Awesome.AI.Web.Helpers;
using static Awesome.AI.Helpers.Enums;
using static Google.Protobuf.WellKnownTypes.Field.Types;

namespace Awesome.AI.Core
{
    /*
     * Awesome-AI - Apache License Disclaimer
     * Copyright (C) 2023 Joakim Jacobsen <jjacobsen@copenhagen-ai.net>
     * https://www.copenhagen-ai.net
     * 
     * This software is provided by the Awesome-AI project "as is" and 
     * any expressed or implied warranties, including, but not limited to, 
     * the implied warranties of merchantability and fitness for a particular 
     * purpose are disclaimed. In no event shall the Awesome-AI project 
     * or its contributors be liable for any direct, indirect, incidental, 
     * special, exemplary, or consequential damages (including, but not 
     * limited to, procurement of substitute goods or services; loss of use, 
     * data, or profits; or business interruption) however caused and on any 
     * theory of liability, whether in contract, strict liability, or tort 
     * (including negligence or otherwise) arising in any way out of the use 
     * of this software, even if advised of the possibility of such damage.
     * 
     * This disclaimer is in accordance with the Apache License, Version 2.0. 
     * For more details about the Apache License, please refer to the LICENSE 
     * file included with this software or visit the Apache Software Foundation's 
     * website: https://www.apache.org/licenses/LICENSE-2.0.html
     * */


    /*
     * Notes:
     * Maybe its not about creating a replica of the brain, but building upon the basics ie. TheMatrix, Mechanics etc
     * Maybe the question should be: "What can be done with this setup"
     * */
    public class TheMind
    {
        public TheMatrix matrix;
        public Core core;
        public TheCurve curve;
        public Memory mem;
        public Params parms;
        public Calc calc;
        public Process process;
        public Filters filters;
        public Out _out;
        public MyInternal _internal;
        public MyExternal _external;
        public Common.Common common;
        public Common.Convert convert;
        
        public HUB curr_hub;
        public UNIT curr_unit;
        public UNIT theanswer;

        public MECHANICS mech;
        public THECHOISE goodbye = THECHOISE.NO;

        private bool do_process = false;
                
        public int epochs = 1;
        public int cycles = 0; // Go TRON!
        public int cycles_all = 0;
        public int correct_thinking = 0;
        public int not_correct_thinking = 0;
        public int _near_death = 0;
        public double pain = 0.0f;
        public int valid_units = 0;

        public bool theme_on = false;
        public string theme = "none";
        public string theme_old = "";
        public string hobby = "socializing";
        
        public List<KeyValuePair<string, int>> themes_stat = new List<KeyValuePair<string, int>>();
        public Stats stats = new Stats();
        
        public TheMind(MECHANICS mech)
        {
            try
            {
                Console.BackgroundColor = ConsoleColor.DarkGray;

                this.mech = mech;

                parms = new Params(this, mech);
                common = new Common.Common(this);
                matrix = new TheMatrix(this);
                calc = new Calc(this);
                process = new Process(this);
                _internal = new MyInternal(this);
                _external = new MyExternal(this);
                filters = new Filters(this);
                convert = new Common.Convert(this);
                core = new Core(this);
                curve = new TheCurve(this);
                _out = new Out(this);

                mem = new Memory(this);
                                    
                curr_unit = mem.UNITS_ALL().Where(x => x.root == "_love10").FirstOrDefault();
                curr_hub = curr_unit.HUB;
                        
                PreRun(true);
            
                theanswer = UNIT.Create(this, -1.0d, "I dont Know", "null", "SPECIAL", TYPE.JUSTAUNIT);//set it to "It does not", and the program terminates
            
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

            List<Tuple<string, bool, double>> units_mass = new List<Tuple<string, bool, double>>();
            foreach (UNIT u in list.OrderBy(x => x.HighAtZero).ToList())
                units_mass.Add(new Tuple<string, bool, double>(u.root, u.IsValid, u.HighAtZero));

            Dictionary<string, double> units_dist = new Dictionary<string, double>();
            foreach (UNIT u in list.OrderBy(x => x.LengthFromZero).ToList())
                units_dist.Add(u.root, u.LengthFromZero);

            Dictionary<string, double> units_perc = new Dictionary<string, double>();
            foreach (UNIT u in list.OrderBy(x => parms.GetBaseForce().ToPercent(x)).ToList())
                units_perc.Add(u.root, parms.GetBaseForce().ToPercent(u));

            List<UNIT> list1 = list.OrderBy(x => x.index_conv).ToList();
            List<UNIT> list2 = list.OrderBy(x => x.Variable).ToList();
            List<UNIT> list3 = list.Where(x=>filters.HighPass(x)).OrderBy(x => x.Variable).ToList();

            valid_units = units_force.Count;

            ;
        }

        public void StatClear()
        {
            epochs = 1;
            stats = new Stats();
            themes_stat = new List<KeyValuePair<string, int>>();
            themes_stat.Add(new KeyValuePair<string, int>("none", 1));
        }        

        public bool run = true;
        public bool ok = true;
        public void Run(object sender, MicroLibrary.MicroTimerEventArgs timerEventArgs)
        {
            try
            {                
                HUB _h = mem.HUBS_RND();

                Lists();

                if (!ok)
                    return;
                if (!run)
                    return;
                run = false;

                if (do_process)
                    epochs++;
                if (epochs > 150)
                    epochs = 1;

                bool _pro = do_process;
                do_process = false;
                
                PreRun(_pro);

                if (!Core(_pro))//the basics
                    ok = false;

                TheSoup();//find new curr_unit/curr_hub

                Process(_pro);
                Systems(_pro);
                Output(_pro);

                run = true;
            }
            catch (Exception _e) 
            {
                string msg = "run - " + _e.Message + "\n";
                msg += _e.StackTrace;
                XmlHelper.WriteError(msg);
            }
        }

        private void PreRun(bool _pro)
        {
            if (!_pro)
                return;

            convert.Reset();
            common.Reset();

            _internal.Reset();
            _external.Reset();

            process.Stream();            
        }

        private bool Core(bool _pro)
        {
            /*
             * This is the algorithm for producing thought/making the choise
             * - maybe Core() + TheSoup() could be made into at neural net all by it self, since "almost all" it does is choosing up or down 
             * */

            cycles++;
            cycles_all++;


            IMechanics mech = parms.GetMechanics(MECHANICS.NONE);

            core.UpdateEnergy();
            core.AnswerQuestion();
            
            if (curr_unit.IsIDLE())
                return true;

            mech.CALC();
            mech.XPOS();

            if (curr_hub.IsIDLE())
                core.SetTheme(_pro);            
            
            if (!curve.OK(out pain))
                return false;
            return true;
        }

        private void TheSoup() 
        {
            curr_hub = curr_unit.HUB;
            curr_unit = matrix.NextUnit(curr_unit, parms._mech.dir);
        }

        private void Process(bool _pro)
        {
            process.History(this);
            process.SetCommonUnit(this);
            process.ProcessCommonUnit(this, _pro);
            process.CommonHubs(this);
            process.SetPercent(_pro);
        }

        private void Systems(bool _pro)
        {
            
        }

        public string output_topic = "";
        public string output_sub_th = "";
        private void Output(bool _pro)
        {
            if (curr_unit.root.Split(':')[0] == "subject")
                return;

            output_topic = process.StreamTop().HUB.GetSubject();

            output_sub_th = "currently subprocessing:\t" + curr_unit.HUB.GetSubject() + "\n" +
                            "- actual:\t\t\t" + curr_unit.root + "";

            if (_pro)
                _out.Set();
            
            if (_pro)
                cycles = 0;           
        }

        private async void ProcessPass()
        {
            while (true)
            {
                do_process = true;
                await Task.Delay(1000);
            }
        }                
    }
}
