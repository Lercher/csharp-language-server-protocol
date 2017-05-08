using System;
using System.Diagnostics;
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

            await new TaskCompletionSource<object>().Task;
        }
    }

    class TextDocumentHandler : ITextDocumentSyncHandler
    {
        private readonly ILanguageServer _router;

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
            _router.LogMessage("DidOpenTextDocumentParams");
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