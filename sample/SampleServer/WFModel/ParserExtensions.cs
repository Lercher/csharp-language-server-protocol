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
            if (a.declares != null) yield return string.Format("declares symbol in `{0}`", a.declares);
            if (a.declared != null) yield return string.Format("is declared as symbol in `{0}`", a.declared);
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
