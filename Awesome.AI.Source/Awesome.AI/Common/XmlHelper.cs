using System.Xml.Linq;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Common
{
    public class XmlHelper
    {
        private static readonly object fileLock = new object();

        public static void WriteHello(string name, DateTime date)
        {
            try
            {
                lock (fileLock)
                {
                    var xdoc = XElement.Load(MyPath.PathIP(MINDS.ROBERTA));
                    var group = xdoc.Elements("out");

                    foreach (XElement elem in group.Descendants())
                    {
                        if (elem.Name == "hello" && elem.Attribute("name").Value == "HELLO")
                        {
                            XElement element = new XElement("world",
                                new XAttribute("name", "" + name),
                                new XAttribute("date", "" + date)
                            );

                            elem.Add(element);

                            break;
                        }
                    }

                    xdoc.Save(MyPath.PathIP(MINDS.ROBERTA));
                }
            }
            catch (Exception _e) { }
        }

        public static void ClearHello(DateTime now)
        {
            try
            {
                lock (fileLock)
                {
                    string path = MyPath.PathIP(MINDS.ROBERTA);
                    var xdoc = XElement.Load(path);
                    var group = xdoc.Elements("out");

                    foreach (XElement elem in group.Descendants())
                    {
                        if (elem.Name == "hello" && elem.Attribute("name").Value == "HELLO")
                        {
                            var toRemove = elem.Elements()
                                .Where(p => DateTime.TryParse(p.Attribute("date")?.Value, out DateTime d) && d.ToUniversalTime() < now.ToUniversalTime())
                                .ToList();

                            foreach (var el in toRemove)
                                el.Remove();

                            break;
                        }
                    }

                    xdoc.Save(path);
                }
            }
            catch (Exception _e) { }
        }
    }
}
