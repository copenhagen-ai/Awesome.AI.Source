using System.Reflection;

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
    }
}