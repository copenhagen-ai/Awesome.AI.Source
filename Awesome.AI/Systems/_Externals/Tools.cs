using Awesome.AI.Common;
using Awesome.AI.Core;
using Awesome.AI.Systems.Input;
using static Awesome.AI.Helpers.Enums;

namespace Awesome.AI.Systems.Externals
{
    /*
     * this class is not used yet
     * */
    class Tools
    {
        public class Calculater
        {
            TheMind mind;
            private Calculater() { }
            public Calculater(TheMind mind)
            {
                this.mind = mind;
            }

            public UNIT Use(InputTool _in, string _operator)
            {
                switch (_operator)
                {
                    case "add":
                        return UNIT.Create(mind, 50.0d, "" + (int.Parse(_in.val_a) + int.Parse(_in.val_b)), "null", "", TYPE.JUSTAUNIT);
                    case "sub":
                        return UNIT.Create(mind, 50.0d, "" + (int.Parse(_in.val_a) - int.Parse(_in.val_b)), "null", "", TYPE.JUSTAUNIT);
                    default:
                        return UNIT.Create(mind, 50.0d, "none", "null", "", TYPE.JUSTAUNIT);
                }                
            }
        }

        public class Dictionary {
            class W
            {
                public string root;
                public string stem;
            }

            private static List<string> noun = new List<string>()
            {
                "short", "test", "notation", "curve", "feature", "connection", "tree", "algorithm", "error", "exception", "zero", "one", "dataset", "input"
            };
            private static List<W> verb = new List<W>()
            {
                //regular verbs
                new W(){ root = "terminate", stem = "terminat" }, new W(){root = "reboot", stem = "reboot" }, new W(){ root = "code", stem = "cod" }, new W(){ root = "Google", stem = "Googl" },
                //for sentences
                new W(){ root = "meet", stem = "meet" },
                new W(){ root = "exchange", stem = "exchang" },
                new W(){ root = "seek", stem = "seek" },
                new W(){ root = "run", stem = "runn" },
                new W(){ root = "be", stem = "be" },
                new W(){ root = "update", stem = "updat" },
                new W(){ root = "surf", stem = "surf" },
                new W(){ root = "check", stem = "check" }
            };
            private static List<string> proper = new List<string>()
            {
                "Google", "robo heaven", "movie A.I", "movie Tron", "movie Terminator", "Lieutenant Data", "Roy Batty", "R2D2", "T-1000", "Rotten Tomatos", "IMDB", "Wikipedia"
            };

            private static string Noun(string root)
            {
                string[] arr = root.Split(' ');
                string res = "";
                foreach (string str in arr)
                {
                    res += noun.Contains(str) ? str + "s " : str + " ";
                }
                return res == "" ? "xxx" : res.Trim();
            }

            private static string Verb(string root)
            {
                string[] arr = root.Split(' ');
                string res = "";
                foreach (string str in arr)
                {
                    W w = verb.Select(x => x).Where(x => x.root == root).FirstOrDefault();
                    res += w != null ? w.stem + "ing " : str + " ";
                }
                return res == "" ? "xxx" : res.Trim();
            }

            private static string Proper(string root)
            {
                if (proper.Contains(root))
                    return root + "'s";
                return "xxx";
            }

            private static string Sentence(string root)
            {
                string[] arr = root.Split(' ');
                string res = "";
                foreach(string str in arr)
                {
                    W w = verb.Select(x => x).Where(x => x.root == str).FirstOrDefault();
                    res += w != null ? w.stem + "ing " : str + " ";
                }

                return res == "" ? "xxx" : res.Trim();
            }

            public static string Use(string root, string classA, string classB, int form)
            {
                return form == 0 ? root :
                form == 1 && classB == "n" ? Noun(root) :
                form == 2 && (classB == "v" || classB == "x2") ? Verb(root) :
                form == 3 && (classB == "x1" || classB == "x2" || classB == "x3") ? Proper(root) :
                form == 4 && classB == "s" ? Sentence(root) :
                "xxx";
            }
        }

        public class Languange
        {
            public static string NotMaybe(string res, string notmaybe)
            {
                res =
                    notmaybe == "MAYBE" ? res.Replace("A:[not, maybe]", "maybe").Replace("B:[not, maybe]", "") :
                    notmaybe == "NO" ? res.Replace("A:[not, maybe]", "").Replace("B:[not, maybe]", "not") :
                    notmaybe == "YES" ? res.Replace("A:[not, maybe]", "").Replace("B:[not, maybe]", "") :
                    notmaybe == "none" ? res.Replace("A:[not, maybe]", "").Replace("B:[not, maybe]", "") :
                    res;

                return res;
            }

            public static string Dont(string type_t, bool pinion)
            {
                return (type_t == "like" || type_t == "want") && pinion ? "dont" : "";
            }

            public static string AtTheTotobetogoto(string type_t, int form_rnd, string classA, string classB, bool is_vowel)
            {
                string athetotobetogoto =
                (type_t == "is" || type_t == "answer" || type_t == "question" || type_t == "like" || type_t == "want") && form_rnd == 0 && classB == "x3" ? "the" :
                (type_t == "is" || type_t == "answer" || type_t == "question" || type_t == "like" || type_t == "want") && form_rnd == 0 && classB == "n" ? "a" :
                (type_t == "is" || type_t == "answer" || type_t == "question" || type_t == "like" || type_t == "want") && form_rnd == 0 && classB == "s" ? "to" :
                (type_t == "is" || type_t == "answer" || type_t == "question" || type_t == "like" || type_t == "want") && form_rnd == 0 && classB == "v" ? "to" :
                (type_t == "like" || type_t == "want") && form_rnd == 0 && classB == "a" ? "to be" :
                (type_t == "like" || type_t == "want") && form_rnd == 0 && classA == "m" && classB == "x1" ? "to go to" :
                (type_t == "like" || type_t == "want") && form_rnd == 2 && classB == "x2" ? "to be" :
                (type_t == "like" || type_t == "want") && form_rnd == 4 && classB == "s" ? "to be" :
                "";
                athetotobetogoto = athetotobetogoto == "a" && is_vowel ? "an" : athetotobetogoto;

                return athetotobetogoto;
            }

            public static string The(int form_rnd, string classB)
            {
                return form_rnd == 3 && classB == "x3" ? "the" : "";
            }

            public static string Going(string type_t, int form_rnd, string classB)
            {
                return (type_t == "is" || type_t == "answer" || type_t == "question") && (form_rnd == 0) && classB == "v" ? "going" :
                (type_t == "is" || type_t == "answer" || type_t == "question") && (form_rnd == 0) && classB == "s" ? "going" :
                "";
            }

        }
        public class NewsPaper
        {
            TheMind mind;
            private NewsPaper() { }
            public NewsPaper(TheMind mind)
            {
                this.mind = mind;
            }

            public UNIT Read(string _section)
            {
                bool hit = mind.calc.RandomInt(2) == 0 ? true : false;
                string news = hit ? "new news" : "no news";
                
                return UNIT.Create(mind, 50.0d, news, "null", "", TYPE.JUSTAUNIT);                
            }
        }
    }
}
