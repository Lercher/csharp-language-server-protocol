using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonRpc;
using Lsp;
using Lsp.Capabilities.Client;
using Lsp.Capabilities.Server;
using Lsp.Models;
using Lsp.Protocol;

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
            var server = new LanguageServer(Console.In, Console.Out);

            server.AddHandler(new TextDocumentHandler(server));

            await server.Initialize();
            var fn = System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location);
            server.LogMessage(string.Format("{2} initialized. Encodings: In {0}, out {1}.", Console.InputEncoding.EncodingName, Console.OutputEncoding.EncodingName, fn));

            await server.WasShutDown;
        }
    }

    class TextDocumentHandler : ITextDocumentSyncHandler
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

        private SynchronizationCapability _capability;

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
            return p.Character + PosOf(p.Line, lines, 2);
        }

        private static string ApplyChanges(string sourcecode, Container<TextDocumentContentChangeEvent> contentChanges)
        {
            var sb = new System.Text.StringBuilder(sourcecode);
            var lines = LinesOf(sourcecode).ToArray();
            var qy =
                from item in contentChanges
                orderby item.Range.Start descending
                select item;
            foreach (var c in qy)
            {
                var start = (int) PosOf(c.Range.Start, lines);
                var end = (int) PosOf(c.Range.End, lines);
                sb.Remove(start, end - start);
                sb.Insert(start, c.Text);
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
            _capability = capability;
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
    }
}