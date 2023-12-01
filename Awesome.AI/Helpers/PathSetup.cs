using System.Reflection;

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

        public static string MyPath
        {
            get
            {
                string path = Root + "Awesome.AI\\Data\\setup_2.xml";

                return path;
            }
        }
    }
}
