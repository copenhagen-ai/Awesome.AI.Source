namespace Awesome.AI.Common
{
    public class GeneralHelper
    {
        public static bool IsDebug()
        {
            bool debug = false;
#if DEBUG
            debug = true;
#endif
            return debug;
        }
    }
}
