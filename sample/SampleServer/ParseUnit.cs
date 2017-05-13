using System;
using System.Collections.Generic;
using System.Linq;
using Lsp;
using Lsp.Models;
using Lsp.Protocol;

using SampleServer.WFModel; // use this Parser/Scanner/etc.

namespace SampleServer
{
    public class ParseUnit : Lsp.Models.TextDocumentIdentifier
    {
        public Parser parser { get; private set; }
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
                var maxLine1 = LinesOf(sourcecode).Count();

                var b = System.Text.Encoding.UTF8.GetBytes(sourcecode.ToString());
                var sb = new System.Text.StringBuilder();

                using (var sw = new System.IO.StringWriter(sb))
                using (var w = new CollectingTextWriter(sw, maxLine1))
                using (var s = new System.IO.MemoryStream(b))
                {
                    var scanner = new Scanner(s, true); // it's BOM free but UTF8
                    parser = new Parser(scanner);
                    parser.errors.errorStream = w;
                    parser.Parse();
                    sw.WriteLine("\n{0:n0} error(s) detected in {1}", parser.errors.count, Uri); // sw (sic!) as we don't want this to be an error
                    PublishDiagnostics(w.list);
                }
                JsonRpc.Tracer.Do(3, (tw) => tw.WriteLine(sb.ToString()));
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

        public LocationOrLocations CreateDefinitionLocation(Position position)
        {
            var alt = parser.LookingAt(position);
            if (alt?.declaration == null)
                return new LocationOrLocations();
            else
                return new LocationOrLocations(alt.declaration.ToLocation(Uri));
        }

        public LocationContainer CreateReferenceLocations(Position position)
        {
            var alt = parser.LookingAt(position);
            if (alt != null)
            {
                if (alt.declaration != null)
                    return CreateReferenceLocations(alt.declaration);
                if (!string.IsNullOrEmpty(alt.declares))
                    return CreateReferenceLocations(alt.t);
            }
            return new LocationContainer();
        }

        private LocationContainer CreateReferenceLocations(Token declaration)
        {
            var decl = declaration.ToLocation(Uri);
            var qy = 
                from alt in parser.tokens
                where alt.declaration == declaration
                select alt.t.ToLocation(Uri);
            // vscode sorts this by linenumber, so we can use Append here
            return new LocationContainer(qy.Append(decl)); 
        }

        private RecentCompletionRequest _RecentCompletionRequest = null;
        public CompletionList CreateCompletionList(Position position)
        {
            var alt = parser.LookingAt(position);
            lock (this)
            {
                if (RecentCompletionRequest.WasSuchARequestRecently(ref _RecentCompletionRequest, alt?.t))
                    return new CompletionList(Enumerable.Empty<CompletionItem>());
            }
            return new CompletionList(CreateCompletionList(alt));
        }

        private IEnumerable<CompletionItem> CreateCompletionList(Alternative a)
        {
            if (a == null) yield break;

            for (var i = 0; i < a.alt.Length; i++)
            {
                if (a.alt[i])
                {
                    var kind = Parser.tName[i];
                    var st = a.st[i];
                    if (st == null)
                    {
                        if (kind.StartsWith("\""))
                        { 
                            var kw = kind.Substring(1, kind.Length - 2);
                            yield return CreateCompletionItem("{0} - {1}", kw, CompletionItemKind.Keyword, null);
                        }
                        else
                        {
                            yield return CreateCompletionItem("({0}) structure", kind, CompletionItemKind.Text, null);
                        }
                    }
                    else
                    {
                        foreach(var t in st.items)
                            yield return CreateCompletionItem("{0} - {2} symbol", t.val, CompletionItemKind.Reference, st.name);
                    }
                }
            }
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
            var alt = parser.LookingAt(position);
            var dict = new Dictionary<Uri, IEnumerable<TextEdit>>();
            dict[Uri] = RenameSymbol(alt, newName);
            return new WorkspaceEdit() { Changes = dict };
        }

        private IEnumerable<TextEdit> RenameSymbol(Alternative alt, string newName)
        {
            if (alt == null) yield break;
            var decl = alt.declaration ?? alt.t;
            yield return new TextEdit() { NewText = newName, Range = decl.ToRange() };
            var qy = 
                from a in parser.tokens
                where a.declaration == decl
                select a.t.ToRange() into r
                select new TextEdit() { NewText = newName, Range = r };
            foreach (var te in qy)
                yield return te;
        }

        private class RecentCompletionRequest
        {
            public readonly Token t;
            public readonly DateTime when = DateTime.Now;

            private RecentCompletionRequest(Token t) => this.t = t;

            public static bool WasSuchARequestRecently(ref RecentCompletionRequest recent, Token t)
            {
                if (t == null) return false;
                try
                {
                    if (recent == null) return false;
                    if (t.charPos != recent.t.charPos) return false;
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
