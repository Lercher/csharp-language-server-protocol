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
            return new Range(pos, pos);
        }


        public static bool IsAt(this CocoRCore.Token t, Position p)
        {
            var pl1 = (int) p.Line + 1;
            var pc1 = (int) p.Character + 1;
            return t.position.IsBefore(pl1, pc1) && t.endPosition.IsAfter(pl1, pc1);
        }

        public static CocoRCore.Alternative LookingAt(this CocoRCore.ParserBase parser, Position p) => parser.AlternativeTokens.FirstOrDefault(a => a.t.IsAt(p));
        public static bool IsBefore(this CocoRCore.Position pt, int line1, int col1) => pt.line < line1 || pt.line == line1 && pt.col <= col1;
        public static bool IsAfter (this CocoRCore.Position pt, int line1, int col1) => pt.line > line1 || pt.line == line1 && pt.col > col1;
        public static Location ToLocation(this CocoRCore.Token t, Uri inFile) => new Location() { Range = t.ToRange(), Uri = inFile };
        public static Position ToPosition(this CocoRCore.Position p) => new Position(p.line - 1, p.col - 1);
        public static Range ToRange(this CocoRCore.Token t) => new Range(t.position.ToPosition(), t.endPosition.ToPosition());

        // https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet
        public static IEnumerable<string> DescribeFor(this CocoRCore.Alternative a, CocoRCore.ParserBase parser)
        {
            yield return a.t.DescribeFor(parser);
            if (!string.IsNullOrEmpty(a.declares)) yield return string.Format("declares a{1} `{0}` symbol", a.declares, IsVowel(a.declares) ? "n" : "");
            if (!string.IsNullOrEmpty(a.references)) yield return string.Format("references a{1} `{0}` symbol", a.references, IsVowel(a.references) ? "n" : "");


            foreach (var st in a.symbols)
            {                
                var ast = string.Format("**Valid `{0}`**\n\n{1}", st.Name, string.Join("  \n  ", st.Items.ToArray()));
                yield return ast;
            }

            var altKWs = new List<string>();
            for (var i = 0; i < a.alt.Length; i++)
            {
                if (a.alt[i])
                {
                    var kind = parser.NameOfTokenKind(i);
                    if (kind.StartsWith("["))
                        altKWs.Add(kind);
                    else
                        altKWs.Add($"`{kind}`");
                }
            }
            if (altKWs.Any())
            {
                var akw = string.Format("**Valid items**\n\n{0}", string.Join("  \n  ", altKWs.ToArray()));
                yield return akw;
            }

        }

        private static bool IsVowel(string s)
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
            var kind = parser.NameOfTokenKind(t.kind);
            if (!kind.StartsWith("["))
                return $"Keyword **{t.valScanned}**";
            return string.Format("{2} **{3}**", t.line, t.col, kind, t.valScanned);
        }
    }
}
