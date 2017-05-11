/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */
'use strict';

import * as path from 'path';

import { workspace, Disposable, ExtensionContext, window } from 'vscode';
import { LanguageClient, LanguageClientOptions, SettingMonitor, ServerOptions, TransportKind } from 'vscode-languageclient';

export function activate(context: ExtensionContext) {
    window.setStatusBarMessage("Coco/R Generic Language Server started", 2000); // hide after 2s
    
    // Options to control the language client
    let clientOptions: LanguageClientOptions = {
        // Register the server for plaintext documents named *.txt
        documentSelector: [{
            language: 'plaintext',
            pattern: '**/*.txt'
        }],
        synchronize: {
            // Synchronize the setting section 'CocoR' to the server
            configurationSection: 'CocoR',
            // Notify the server about file changes to '.clientrc files contained in the workspace
            fileEvents: workspace.createFileSystemWatcher('**/.clientrc')
        }
    }

    // The server is implemented as a process with console StdIn/StdOut streams
    let serverDll = context.asAbsolutePath('../sample/SampleServer/bin/Debug/netcoreapp1.1/win7-x64/SampleServer.dll');
    let run = { command: "dotnet.exe", args: [serverDll] };

    // Create the language client and start the client.
    let disposable = new LanguageClient('Coco/R Generic', run, clientOptions).start();

    // Push the disposable to the context's subscriptions so that the
    // client can be deactivated on extension deactivation
    context.subscriptions.push(disposable);
}
