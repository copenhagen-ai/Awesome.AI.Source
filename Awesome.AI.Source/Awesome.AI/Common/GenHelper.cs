namespace Awesome.AI.Common
{
    public class GenHelper
    {
        private static readonly object fileLock = new object();

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
