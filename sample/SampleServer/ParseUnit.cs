﻿using System;
using System.Collections.Generic;
using System.Linq;
using Lsp;
using Lsp.Models;
using Lsp.Protocol;

namespace SampleServer
{
    public class ParseUnit : Lsp.Models.TextDocumentIdentifier
    {
        public CocoRCore.ParserBase Parser { get; private set; }
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
#if false
                    try
                    {
                        var v = sourcecode.ToString();
                        var before = v.Substring(c.Start - 10, 10).Replace("\r\n", "\\n");
                        var removed = v.Substring(c.Start, c.End - c.Start).Replace("\r\n", "\\n");
                        var after = v.Substring(c.End, 10).Replace("\r\n", "\\n");
                        _router.LogMessage(MessageType.Log, string.Format("...{0}[{1}>{3}]{2}...", before, removed, after, c.Text));
                    }
                    catch (Exception) { }
#endif
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
                var sb = new System.Text.StringBuilder();
                using (var w = new System.IO.StringWriter(sb))
                {
                    Parser = CocoRCore.Samples.WFModel.Parser.Create(scanner => scanner.Initialize(sourcecode, Uri.ToString()));
                    Parser.errors.Writer = w;
                    Parser.Parse();
                    w.WriteLine("\n{0:f} detected in {1}", Parser.errors, Uri);
                    PublishDiagnostics(Parser.errors);
                }
                JsonRpc.Tracer.Do(3, (tw) => tw.WriteLine(sb.ToString()));
            }
        }

        private void PublishDiagnostics(IEnumerable<CocoRCore.Diagnostic> errors)
        {
            var qy =
                from e in errors
                select new Diagnostic() { Source = "Coco/R", Message = e.message, Range = e.ToRange(), Severity = DiagnosticSeverity.Error };
            var Diagnostics = new Container<Diagnostic>(qy);
            var publishDiagnosticsParams = new PublishDiagnosticsParams() { Diagnostics = Diagnostics, Uri = Uri };
            _router.PublishDiagnostics(publishDiagnosticsParams);
        }

        public Hover CreateHover(Position position)
        {
            var alt = Parser.LookingAt(position);
            return CreateHover(alt);
        }

        public Hover CreateHover(CocoRCore.Alternative a)
        {
            if (a == null) return CreateHover(string.Empty);
            return CreateHover(a.DescribeFor(Parser), a.t.ToRange());
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

        public LocationOrLocations CreateDefinitionLocation(Position position)
        {
            var alt = Parser.LookingAt(position);
            if (alt?.declaration == null)
                return new LocationOrLocations();
            else
                return new LocationOrLocations(alt.declaration.ToLocation(Uri));
        }

        public LocationContainer CreateReferenceLocations(Position position)
        {
            var alt = Parser.LookingAt(position);
            if (alt != null)
            {
                if (alt.declaration != null)
                    return CreateReferenceLocations(alt.declaration);
                if (!string.IsNullOrEmpty(alt.declares))
                    return CreateReferenceLocations(alt.t);
            }
            return new LocationContainer();
        }

        private LocationContainer CreateReferenceLocations(CocoRCore.Token declaration)
        {
            var decl = declaration.ToLocation(Uri);
            var qy =
                from alt in Parser.AlternativeTokens
                where alt.declaration == declaration
                select alt.t.ToLocation(Uri);
            // vscode sorts this by linenumber, so we can use Append here
            return new LocationContainer(qy.Append(decl));
        }

        private RecentCompletionRequest _RecentCompletionRequest = null;
        public CompletionList CreateCompletionList(Position position)
        {
            var alt = Parser.LookingAt(position);
            lock (this)
            {
                if (RecentCompletionRequest.WasSuchARequestRecently(ref _RecentCompletionRequest, alt?.t))
                    return new CompletionList(Enumerable.Empty<CompletionItem>());
            }
            return new CompletionList(CreateCompletionList(alt));
        }

        private IEnumerable<CompletionItem> CreateCompletionList(CocoRCore.Alternative a)
        {
            if (a == null) yield break;

            for (var kind = 0; kind < a.alt.Length; kind++)
            {
                if (a.alt[kind])
                {
                    var kindName = Parser.NameOfTokenKind(kind);
                    if (!kindName.StartsWith("["))
                        yield return CreateCompletionItem("{0} - {1}", kindName, CompletionItemKind.Keyword, null);
                    else
                        yield return CreateCompletionItem("{0} literal", kindName, CompletionItemKind.Text, null);
                }
            }
            foreach (var st in a.symbols)
                foreach (var item in st.Items)
                    yield return CreateCompletionItem("{0} - {2} symbol", item, CompletionItemKind.Reference, st.Name);
        }


        private static CompletionItem CreateCompletionItem(string labelformat, string text, CompletionItemKind kind, object arg2)
        {
            var label = string.Format(labelformat, text, kind, arg2);
            return new CompletionItem() {
                InsertText = text,
                Label = label,
                Kind = kind
            };
        }

        public WorkspaceEdit CreateWorkspaceEdit(Position position, string newName)
        {
            var alt = Parser.LookingAt(position);
            var dict = new Dictionary<Uri, IEnumerable<TextEdit>> {
                [Uri] = RenameSymbol(alt, newName)
            };
            return new WorkspaceEdit() { Changes = dict };
        }

        private IEnumerable<TextEdit> RenameSymbol(CocoRCore.Alternative alt, string newName)
        {
            if (alt == null) yield break;
            var decl = alt.declaration ?? alt.t;
            yield return new TextEdit() { NewText = newName, Range = decl.ToRange() };
            var qy =
                from a in Parser.AlternativeTokens
                where a.declaration == decl
                select a.t.ToRange() into r
                select new TextEdit() { NewText = newName, Range = r };
            foreach (var te in qy)
                yield return te;
        }

        private class RecentCompletionRequest
        {
            public readonly CocoRCore.Token t;
            public readonly DateTime when = DateTime.Now;

            private RecentCompletionRequest(CocoRCore.Token t) => this.t = t;

            public static bool WasSuchARequestRecently(ref RecentCompletionRequest recent, CocoRCore.Token t)
            {
                if (t == null) return false;
                try
                {
                    if (recent == null) return false;
                    if (t.pos != recent.t.pos) return false;
                    var diff = DateTime.Now.Subtract(recent.when);
                    if (diff.TotalMilliseconds > 200.0) return false;
                    return true;
                }
                finally
                {
                    recent = new RecentCompletionRequest(t);
                }
            }
        }
    }
}
