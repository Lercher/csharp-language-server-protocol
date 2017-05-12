using System;
using System.Collections.Concurrent;
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
            Tracer.Deactivate(new int[] { 1, 2 });
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
        public TextDocumentHandler(ILanguageServer router)
        {
            _router = router;
        }


        private readonly DocumentSelector _documentSelector = new DocumentSelector(
            new DocumentFilter() {
                Pattern = "**/*.txt",
                Language = "plaintext"
            },
            new DocumentFilter() {
                Pattern = "**/*.wfmodel",
                Language = "plaintext"
            }
        );

        public TextDocumentSyncOptions Options { get; } = new TextDocumentSyncOptions() {
            WillSaveWaitUntil = false,
            WillSave = true,
            Change = TextDocumentSyncKind.Incremental,
            Save = new SaveOptions() {
                IncludeText = false // TODO: try false
            },
            OpenClose = true
        };

        public TextDocumentAttributes GetTextDocumentAttributes(Uri uri)
        {
            _router.LogMessage("GetTextDocumentAttributes " + uri.ToString());
            return new TextDocumentAttributes(uri, "plaintext");
        }



        #region "Open/Close/Change/Save Notifications of ITextDocumentSyncHandler"

        public void SetCapability(SynchronizationCapability capability)
        {
            _SynchronizationCapability = capability;
        }
        private SynchronizationCapability _SynchronizationCapability;


        // --------------- Change ------------------------------------------
        public async Task Handle(DidChangeTextDocumentParams notification)
        {
            ParseUnitFor(notification.TextDocument).ApplyChanges(notification.ContentChanges);
            await Task.CompletedTask;
        }

        TextDocumentChangeRegistrationOptions IRegistration<TextDocumentChangeRegistrationOptions>.GetRegistrationOptions()
        {
            return new TextDocumentChangeRegistrationOptions() {
                DocumentSelector = _documentSelector,
                SyncKind = Options.Change
            };
        }

        // --------------- Open ------------------------------------------

        public async Task Handle(DidOpenTextDocumentParams notification)
        {
            ParseUnitFor(notification.TextDocument).ApplyChanges(notification.TextDocument.Text);
            await Task.CompletedTask;
        }

        TextDocumentRegistrationOptions IRegistration<TextDocumentRegistrationOptions>.GetRegistrationOptions()
        {
            return new TextDocumentRegistrationOptions() {
                DocumentSelector = _documentSelector,
            };
        }


        // --------------- Close ------------------------------------------

        public Task Handle(DidCloseTextDocumentParams notification)
        {
            _ParseUnits.TryRemove(notification.TextDocument.Uri, out var _);
            return Task.CompletedTask;
        }

        // --------------- Save ------------------------------------------

        public Task Handle(DidSaveTextDocumentParams notification)
        {
            return Task.CompletedTask;
        }

        TextDocumentSaveRegistrationOptions IRegistration<TextDocumentSaveRegistrationOptions>.GetRegistrationOptions()
        {
            return new TextDocumentSaveRegistrationOptions() {
                DocumentSelector = _documentSelector,
                IncludeText = Options.Save.IncludeText
            };
        }
#endregion


        #region "Hover"
        public Task<Hover> Handle(TextDocumentPositionParams request, CancellationToken token)
        {
            var pu = ParseUnitFor(request.TextDocument);
            _router.LogMessage("Hover over " + request.Position.Describe());
            var alt = pu.parser.LookingAt(request.Position);
            return Task.FromResult(pu.CreateHover(alt));
        }

        public void SetCapability(HoverCapability capability)
        {
            _HoverCapability = capability;
        }
        private HoverCapability _HoverCapability;

        #endregion


        #region "Parse Units"
        private readonly ConcurrentDictionary<Uri, ParseUnit> _ParseUnits = new ConcurrentDictionary<Uri, ParseUnit>();

        private ParseUnit ParseUnitFor(TextDocumentIdentifier tdi)
        {
            return _ParseUnits.GetOrAdd(tdi.Uri, u => new ParseUnit(_router, u));
        }        
        #endregion
    }

}