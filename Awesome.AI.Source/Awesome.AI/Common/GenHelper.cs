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

        public static void BusyWait1(string txt, int count)
        {
            //because i dont want to implement async
            for (int i = 0; i < count; i++)
                Console.WriteLine(txt);
        }

        public static void BusyWait2(string txt, int count)
        {
            //because i dont want to implement async
            for (int i = 0; i < count; i++)
                Thread.Sleep(10);
        }
    }
}
