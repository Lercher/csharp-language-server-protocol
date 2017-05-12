using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lsp;
using Lsp.Models;
using Lsp.Protocol;
using SampleServer.WFModel;

namespace SampleServer
{
    class ParseUnit : Lsp.Models.TextDocumentIdentifier
    {
        public WFModel.Parser parser { get; private set; }
        private int maxLine1 = 0;
        private readonly System.Text.StringBuilder sourcecode = new System.Text.StringBuilder();
        private readonly ILanguageServer _router;


        public ParseUnit(ILanguageServer ls, Uri file) : base(file)
        {
            _router = ls;
        }

        private static IEnumerable<string> LinesOf(System.Text.StringBuilder sb)
        {
            using (var sr = new System.IO.StringReader(sb.ToString()))
                while (true)
                {
                    var line = sr.ReadLine();
                    if (line == null)
                        break;
                    yield return line;
                }
        }

        private static long PosOf(long LineNumber, string[] Lines, int WithLineEndingSize)
        {
            var pos = 0L;
            for (var i = 0; i < LineNumber && i < Lines.Length; i++)
                pos += (Lines[i].Length + WithLineEndingSize);
            return pos;
        }

        private static long PosOf(Position p, string[] lines)
        {
            if (p == null)
                return 0L;
            return p.Character + PosOf(p.Line, lines, 2);
        }

        public void ApplyChanges(string fullText)
        {
            lock (this)
            {
                sourcecode.Clear();
                sourcecode.Append(fullText);
                maxLine1 = LinesOf(sourcecode).Count();
                Parse();
            }
        }

        public void ApplyChanges(Container<TextDocumentContentChangeEvent> contentChanges)
        {
            if (contentChanges == null || !contentChanges.Any())
                return;

            // special case: a single contentChange item with no Range contains the full source
            if (contentChanges.Count() == 1 && contentChanges.First().Range == null)
            {
                ApplyChanges(contentChanges.First().Text);
                return;
            }

            lock (this)
            {
                var lines = LinesOf(sourcecode).ToArray();
                maxLine1 = lines.Length;
                var qy =
                    from item in contentChanges
                    select new
                    {
                        item.Text,
                        Start = (int)PosOf(item.Range?.Start, lines),
                        End = (int)PosOf(item.Range?.End, lines)
                    };
                var qy2 =
                    from item in qy
                    orderby item.Start descending
                    select item;

                foreach (var c in qy2)
                {
                    sourcecode.Remove(c.Start, c.End - c.Start);
                    sourcecode.Insert(c.Start, c.Text);
                }
                Parse();
            }
        }

        private void Parse()
        {
            lock (this)
            {
                var b = System.Text.Encoding.UTF8.GetBytes(sourcecode.ToString());
                var sb = new System.Text.StringBuilder();

                using (var sw = new System.IO.StringWriter(sb))
                using (var w = new CollectingTextWriter(sw, maxLine1))
                using (var s = new System.IO.MemoryStream(b))
                {
                    var scanner = new WFModel.Scanner(s, true); // it's BOM free but UTF8
                    parser = new WFModel.Parser(scanner);
                    parser.errors.errorStream = w;
                    parser.Parse();
                    sw.WriteLine("\n{0:n0} error(s) detected in {1}", parser.errors.count, Uri); // sw (sic!) as we don't want this to be an error
                    PublishDiagnostics(w.list);
                }
                _router.LogMessage(sb.ToString());
            }
        }

        private void PublishDiagnostics(IEnumerable<Diagnostic> errors)
        {
            var Diagnostics = new Container<Diagnostic>(errors);
            var publishDiagnosticsParams = new PublishDiagnosticsParams() { Diagnostics = Diagnostics, Uri = Uri };
            _router.PublishDiagnostics(publishDiagnosticsParams);

        }

        public Hover CreateHover(Position position)
        {
            var alt = parser.LookingAt(position);
            return CreateHover(alt);
        }

        public Hover CreateHover(Alternative a)
        {
            if (a == null) return CreateHover(string.Empty);
            return CreateHover(a.Describe(), a.t.ToRange());
        }

        public Hover CreateHover(string s)
        {
            return new Hover() { Contents = new MarkedStringContainer(new MarkedString(s)) };
        }

        private Hover CreateHover(IEnumerable<string> ss, Range r)
        {
            // see http://vshaxe.github.io/vscode-extern/vscode/MarkedString.html
            _router.LogMessage("from " + r.Start.Describe() + "-" + r.End.Describe());
            var qy = from s in ss select new MarkedString(s); // "markdown"
            return new Hover() { Contents = new MarkedStringContainer(qy.ToArray()), Range = r };
        }

        public LocationOrLocations CreateLocation(Position position)
        {
            var alt = parser.LookingAt(position);
            if (alt?.declaration == null)
                return new LocationOrLocations();
            else
                return new LocationOrLocations(alt.declaration.ToLocation(Uri));
        }
    }
}
