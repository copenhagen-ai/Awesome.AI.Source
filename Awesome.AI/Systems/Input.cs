using Awesome.AI.Common;

namespace Awesome.AI.Systems
{
    public class InputTool
    {
        public string val_a;
        public string val_b;
    }

    public class InputIdea
    {
        public string val_a;
        public string val_b;
    }

    public class InputLearn
    {
        object input;
        private InputLearn() { }
        public static InputLearn Create(string type, object o)
        {
            InputLearn _in = new InputLearn();
            switch (type)
            {
                case "STRING": _in.input = (string)o; break;
                case "D_STRING": _in.input = (string[,])o; break;
                //case "UNIT": _in.input = (UNIT)o; break;
                //case "D_UNIT": _in.input = (UNIT[,])o; break;
                default:
                    throw new Exception("Create");
            }
            return _in;
        }
        public object Get(string type)
        {
            switch (type)
            {
                case "STRING": return (string)input;
                case "D_STRING": return (string[,])input;
                default:
                    throw new Exception("Get");
            }
        }
    }
    public class Combinations
    {
        object input;
        private Combinations() { }
        public static Combinations Create(string type, object o)
        {
            Combinations com = new Combinations();
            switch (type)
            {
                case "A_STRING": com.input = (string[])o; break;
                case "L_D_STRING": com.input = (List<string[,]>)o; break;
                case "A_UNIT": com.input = (UNIT[])o; break;
                case "L_D_UNIT": com.input = (List<UNIT[,]>)o; break;
                default:
                    throw new Exception("Create");
            }
            return com;
        }
        public object Get(string type)
        {
            switch (type)
            {
                case "A_STRING": return (string[])input;
                case "L_D_STRING": return (List<string[,]>)input;
                case "A_UNIT": return (UNIT[])input;
                case "L_D_UNIT": return (List<UNIT[,]>)input;
                default:
                    throw new Exception("Get");
            }
        }
    }
}