/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */
'use strict';

import * as path from 'path';

import { workspace, Disposable, ExtensionContext, window } from 'vscode';
import { LanguageClient, LanguageClientOptions, SettingMonitor, ServerOptions, TransportKind } from 'vscode-languageclient';

export function activate(context: ExtensionContext) {

    // The server is implemented in node
    let serverDll = context.asAbsolutePath('../sample/SampleServer/bin/Debug/netcoreapp1.1/win7-x64/SampleServer.dll');
    window.setStatusBarMessage("starting " + serverDll + " ...");
    
    // The debug options for the server
    let runArgs = [serverDll];

    // If the extension is launched in debug mode then the debug server options are used
    // Otherwise the run options are used
    let serverOptions: ServerOptions = {
        run : { command: "dotnet.exe", args: runArgs },
        debug: { command: "dotnet.exe", args: runArgs }
    };

    // Options to control the language client
    let clientOptions: LanguageClientOptions = {
        // Register the server for plain text documents
        documentSelector: [{
            language: 'plaintext',
            pattern: '**/*'
        }],
        synchronize: {
            // Synchronize the setting section 'CocoR' to the server
            configurationSection: 'CocoR',
            // Notify the server about file changes to '.clientrc files contain in the workspace
            fileEvents: workspace.createFileSystemWatcher('**/.clientrc')
        }
    }

    // Create the language client and start the client.
    let disposable = new LanguageClient('CocoR', 'Coco/R Language Server', serverOptions, clientOptions).start();
    window.setStatusBarMessage(serverDll + " started", 5000); // hide after 5s

    // Push the disposable to the context's subscriptions so that the
    // client can be deactivated on extension deactivation
    context.subscriptions.push(disposable);
}
