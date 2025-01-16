using System.Reflection;
using static Awesome.AI.Helpers.Enums;

namespace Awesome.AI.Web.Helpers
{
    public class PathSetup
    {
        public static string Root
        {
            get
            {
                string nd = Path.DirectorySeparatorChar.ToString();
                string application = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                string directory = Directory.GetParent(application).Parent.Parent.FullName;

#if DEBUG
                string rootPath = directory + nd;
#else
                string rootPath = application + nd;
#endif

                return rootPath;
            }
        }

        public static string MyPath(MINDS mindtype)
        {
            string path = "";
            string setting = mindtype == MINDS.ROBERTA ? "roberta" : "andrew";

            path = Root + "Awesome.AI\\Data\\setup_" + setting + ".xml";

            //if (mindtype == MINDS.STANDARD)
            //    path = Root + "Awesome.AI\\Data\\setup_2.xml";

            return path;
        }
    }
}