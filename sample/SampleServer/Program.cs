using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JsonRpc;
using Lsp;
using Lsp.Capabilities.Client;
using Lsp.Capabilities.Server;
using Lsp.Models;
using Lsp.Protocol;
using SampleServer.WFModel;

namespace SampleServer
{    
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            //while (!System.Diagnostics.Debugger.IsAttached)
            //{
            //    await Task.Delay(100);
            //}

            Tracer.Connect(Console.Error);
            Tracer.Deactivate(new int[] {1, 2});
            var server = new LanguageServer(Console.OpenStandardInput(), Console.OpenStandardOutput());

            server.AddHandler(new TextDocumentHandler(server));

            await server.Initialize();
            var fn = System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location);
            server.LogMessage(string.Format("{0} initialized.", fn));

            await server.WasShutDown;
        }
    }

    class TextDocumentHandler : ITextDocumentSyncHandler, IHoverHandler
    {
        private readonly ILanguageServer _router;
        private string sourcecode;
        private WFModel.Scanner scanner;
        private WFModel.Parser parser;

        private readonly DocumentSelector _documentSelector = new DocumentSelector(
            new DocumentFilter() {
                Pattern = "**/*",
                Language = "plaintext"
                //Pattern = "**/*.csproj",
                //Language = "xml"
            }
        );

        private SynchronizationCapability _SynchronizationCapability;
        private HoverCapability _HoverCapability;

        public TextDocumentHandler(ILanguageServer router)
        {
            _router = router;
        }

        private static IEnumerable<string> LinesOf(string s)
        {
            using (var sr = new System.IO.StringReader(s))
                while(true)
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

        private static string ApplyChanges(string sourcecode, Container<TextDocumentContentChangeEvent> contentChanges)
        {
            if (contentChanges == null || !contentChanges.Any())
                return sourcecode;

            // special case: a single contentChange item with no Range contains the full source
            if (contentChanges.Count() == 1 && contentChanges.First().Range == null)
                return contentChanges.First().Text;

            var lines = LinesOf(sourcecode).ToArray();
            var qy =
                from item in contentChanges
                select new { 
                    item.Text, 
                    Start = (int)PosOf(item.Range?.Start, lines),
                    End = (int)PosOf(item.Range?.End, lines)
                };
            var qy2 = 
                from item in qy
                orderby item.Start descending
                select item;

            var sb = new System.Text.StringBuilder(sourcecode);
            foreach (var c in qy2)
            {
                sb.Remove(c.Start, c.End - c.Start);
                sb.Insert(c.Start, c.Text);
            }
            return sb.ToString();
        }

        void Parse(string text)
        {
            lock (this)
            {
                sourcecode = text;
                var b = System.Text.Encoding.UTF8.GetBytes(sourcecode);
                var sb = new System.Text.StringBuilder();

                using (var w = new System.IO.StringWriter(sb))
                using (var s = new System.IO.MemoryStream(b))
                {
                    scanner = new WFModel.Scanner(s, true); // it's BOM free but UTF8
                    parser = new WFModel.Parser(scanner);
                    parser.errors.errorStream = w;
                    parser.Parse();
                    w.WriteLine("\n{0:n0} error(s) detected", parser.errors.count);
                }
                _router.LogMessage(sb.ToString());
                foreach(var a in parser.tokens.Take(50))
                    foreach(var s in a.Describe())
                        _router.LogMessage(s);
            }
        }

        public TextDocumentSyncOptions Options { get; } = new TextDocumentSyncOptions() {
            WillSaveWaitUntil = false,
            WillSave = true,
            Change = TextDocumentSyncKind.Full,
            Save = new SaveOptions() {
                IncludeText = true
            },
            OpenClose = true
        };

        public async Task Handle(DidChangeTextDocumentParams notification)
        {
            _router.LogMessage("DidChangeTextDocumentParams");
            var newsource = ApplyChanges(sourcecode, notification.ContentChanges);
            Parse(newsource);
            await Task.CompletedTask;
        }

        TextDocumentChangeRegistrationOptions IRegistration<TextDocumentChangeRegistrationOptions>.GetRegistrationOptions()
        {
            return new TextDocumentChangeRegistrationOptions() {
                DocumentSelector = _documentSelector,
                SyncKind = Options.Change
            };
        }

        public void SetCapability(SynchronizationCapability capability)
        {
            _SynchronizationCapability = capability;
        }

        public async Task Handle(DidOpenTextDocumentParams notification)
        {
            _router.LogMessage("DidOpenTextDocumentParams ");
            Parse(notification.TextDocument.Text);
            await Task.CompletedTask;
        }

        TextDocumentRegistrationOptions IRegistration<TextDocumentRegistrationOptions>.GetRegistrationOptions()
        {
            return new TextDocumentRegistrationOptions() {
                DocumentSelector = _documentSelector,
            };
        }

        public Task Handle(DidCloseTextDocumentParams notification)
        {
            _router.LogMessage("DidCloseTextDocumentParams");
            return Task.CompletedTask;
        }

        public Task Handle(DidSaveTextDocumentParams notification)
        {
            _router.LogMessage("DidSaveTextDocumentParams");
            return Task.CompletedTask;
        }

        TextDocumentSaveRegistrationOptions IRegistration<TextDocumentSaveRegistrationOptions>.GetRegistrationOptions()
        {
            return new TextDocumentSaveRegistrationOptions() {
                DocumentSelector = _documentSelector,
                IncludeText = Options.Save.IncludeText
            };
        }

        public TextDocumentAttributes GetTextDocumentAttributes(Uri uri)
        {
            _router.LogMessage("GetTextDocumentAttributes " + uri.ToString());
            return new TextDocumentAttributes(uri, "plaintext");
        }

#region "Hover"
        public Task<Hover> Handle(TextDocumentPositionParams request, CancellationToken token)
        {
            if (parser == null || request == null)
                return Task.FromResult<Hover>(null);
            _router.LogMessage("Hover over " + request.Position.Describe());
            var alt = parser.LookingAt(request.Position);
            return Task.FromResult(CreateHover(alt));
        }

        private Hover CreateHover(Alternative a)
        {
            if (a == null) return CreateHover(string.Empty);
            return CreateHover(a.Describe(), a.t.ToRange());
        }

        private Hover CreateHover(string s)
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

        public void SetCapability(HoverCapability capability)
        {
            _HoverCapability = capability;
        }
# endregion
    }

}