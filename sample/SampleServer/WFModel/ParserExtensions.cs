using System;
using System.Collections.Generic;
using System.Text;
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

        public static Range ToRange(this Token t)
        {
            var start = new Position(t.line - 1, t.col - 1);
            var end = new Position(t.line - 1, t.col - 1 + t.val.Length);
            return new Range(start, end);
        }

        public static IEnumerable<string> Describe(this Alternative a)
        {
            yield return a.t.Describe();
            if (!string.IsNullOrEmpty(a.declares)) yield return string.Format("declares a{1} `{0}` symbol", a.declares, isVowel(a.declares) ? "n" : "");
            if (!string.IsNullOrEmpty(a.declared)) yield return string.Format("references a{1} `{0}` symbol", a.declared, isVowel(a.declared) ? "n" : "");
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
