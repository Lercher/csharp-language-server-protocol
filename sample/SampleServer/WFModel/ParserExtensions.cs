using System;
using System.Linq;
using System.Collections.Generic;
using Lsp.Models;

namespace SampleServer
{
    public static class ParserExtensions
    {
        public static Range ToRange(this CocoRCore.Diagnostic e)
        {
            var pos = new Position(e.line1 - 1, e.col1 - 1);
            var range = new Range(pos, pos);
            return range;
        }

        public static CocoRCore.Alternative LookingAt(this CocoRCore.ParserBase parser, Position p)
        {
            var qy = from a in parser.tokens where a.t.IsAt(p) select a;
            return qy.FirstOrDefault();
        }

        public static bool IsAt(this CocoRCore.Token t, Position p)
        {
            return t.position.line <= p.Line + 1 && p.Line + 1 <= t.endPosition.line
                && t.position.col <= p.Character + 1 && p.Character + 1 <= t.endPosition.col
            ;
        }

        public static Location ToLocation(this CocoRCore.Token t, Uri inFile)
        {
            return new Location() { Range = t.ToRange(), Uri = inFile };
        }

        public static Position ToPosition(this CocoRCore.Position p) => new Position(p.line - 1, p.col - 1);

        public static Range ToRange(this CocoRCore.Token t)
        {
            var start = t.position.ToPosition();
            var end = t.endPosition.ToPosition();
            return new Range(start, end);
        }

        // https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet
        public static IEnumerable<string> DescribeFor(this CocoRCore.Alternative a, CocoRCore.ParserBase parser)
        {
            yield return a.t.DescribeFor(parser);
            if (!string.IsNullOrEmpty(a.declares)) yield return string.Format("declares a{1} `{0}` symbol", a.declares, isVowel(a.declares) ? "n" : "");
            if (!string.IsNullOrEmpty(a.declared)) yield return string.Format("references a{1} `{0}` symbol", a.declared, isVowel(a.declared) ? "n" : "");

            var altKWs = new List<string>();
            for(var i = 0; i < a.alt.Length; i++)
            {
                if (a.alt[i])
                {
                    var kind = parser.NameOf(i);
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

        public static string DescribeFor(this CocoRCore.Token t, CocoRCore.ParserBase parser)
        {
            var kind = parser.NameOf(t.kind);
            if (kind.StartsWith("\""))
                return "Keyword **" + t.val + "**";
            return string.Format("[{2}] **{3}**", t.line, t.col, kind, t.val);
        }
    }
}
