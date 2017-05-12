using System;
using System.Linq;
using System.Collections.Generic;
using Lsp.Models;

namespace SampleServer.WFModel
{
    public static class ParserExtensions
    {
        public static Alternative LookingAt(this Parser parser, Position p)
        {
            foreach (var t in parser.tokens)
            {
                if (t.t.IsAt(p))
                    return t;
            }
            return null;
        }

        public static bool IsAt(this Token t, Position p)
        {
            return t.line == p.Line + 1
                && t.col <= p.Character + 1 
                && p.Character + 1 <= t.col + t.val.Length
            ;
        }

        public static Location ToLocation(this Token t, Uri inFile)
        {
            return new Location() { Range = t.ToRange(), Uri = inFile };
        }

        public static Range ToRange(this Token t)
        {
            var start = new Position(t.line - 1, t.col - 1);
            var end = new Position(t.line - 1, t.col - 1 + t.val.Length);
            return new Range(start, end);
        }

        // https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet
        public static IEnumerable<string> Describe(this Alternative a)
        {
            yield return a.t.Describe();
            if (!string.IsNullOrEmpty(a.declares)) yield return string.Format("declares a{1} `{0}` symbol", a.declares, isVowel(a.declares) ? "n" : "");
            if (!string.IsNullOrEmpty(a.declared)) yield return string.Format("references a{1} `{0}` symbol", a.declared, isVowel(a.declared) ? "n" : "");

            var altKWs = new List<string>();
            for(var i = 0; i < a.alt.Length; i++)
            {
                if (a.alt[i])
                {
                    var kind = Parser.tName[i];
                    var st = a.st[i];
                    if (st == null)
                        altKWs.Add(kind.Replace('"', '`'));
                    else
                    {
                        var qy = from t in st.items select t.val;
                        var ast = string.Format("**Valid `{0}`**\n\n{1}", st.name, string.Join("  \n  ", qy.ToArray()));
                        yield return ast;
                    }
                }
            }
            if (altKWs.Any())
            {
                var akw = string.Format("**Valid items**\n\n{0}", string.Join("  \n  ", altKWs.ToArray()));
                yield return akw;
            }

        }

        private static bool isVowel(string s)
        {          
            if (string.IsNullOrWhiteSpace(s))
                return false;
            var c = char.ToLowerInvariant(s[0]);
            return (c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u');
        }


        public static string Describe(this Position p)
        {
            return string.Format("({0},{1})", p.Line, p.Character);
        }

        public static string Describe(this Token t)
        {
            var kind = Parser.tName[t.kind];
            if (kind.StartsWith("\""))
                return "Keyword **" + t.val + "**";
            return string.Format("[{2}] **{3}**", t.line, t.col, kind, t.val);
        }
    }
}
