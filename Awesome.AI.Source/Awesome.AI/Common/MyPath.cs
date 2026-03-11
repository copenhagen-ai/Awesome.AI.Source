using Awesome.AI.Access;
using System.Reflection;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Common
{
    public class MyPath
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

        public static string Path(MINDS mindtype)
        {
            string root = Root;

            string setting = 
                mindtype == MINDS.ROBERTA ? "roberta" :
                mindtype == MINDS.ANDREW ? "andrew" :
                "basic";

            string path = root + "Awesome.AI\\Data\\setup_" + setting + ".xml";

            return path;
        }

        public static string PathSetting(MINDS mindtype)
        {
            string root = Root;
            string setting = 
                mindtype == MINDS.ROBERTA ? "_r" :
                mindtype == MINDS.ANDREW ? "_a" :
                "";
            string data = GenHelper.IsDebug() ? "Data" : "DataFiles";
            string path = root + data + "\\settings" + setting + ".xml";

            return path;
        }
    }
}