using System.Reflection;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Common
{
    public class Path
    {
        public static string Root
        {
            get
            {
                string nd = System.IO.Path.DirectorySeparatorChar.ToString();
                string assembly = Assembly.GetEntryAssembly().Location.ToString();

                string app_dir = System.IO.Path.GetDirectoryName(assembly);
                string app_dir_parent = Directory.GetParent(app_dir).Parent.Parent.FullName;

#if DEBUG
                string rootPath = app_dir_parent + nd;
#else
                string rootPath = app_dir + nd;
#endif

                return rootPath;
            }
        }

        public static string MyPath(MINDS mindtype)
        {
            string root = Root;
            
            string setting = mindtype == MINDS.ROBERTA ? "roberta" : "andrew";

            string path = root + "Awesome.AI\\Data\\setup_" + setting + ".xml";

            //if (mindtype == MINDS.STANDARD)
            //    path = Root + "Awesome.AI\\Data\\setup_2.xml";

            return path;
        }
    }
}