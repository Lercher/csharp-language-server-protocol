# SampleServer

The Sample Server is currently tied to a single Coco/R grammar.
It is meant to be used with the vscode extension located in `vscode-testextension`.


## Preparation and Build

* Get and install node.js V6 including npm
* Get and install Visual Studio Code (vscode)
* open a command prompt in `vscode-testextension`
* `npm install`
* `code .`
* You probably want to have a look at `src/extension.ts`
* Build the SampleServer project in LSP.sln with Visual Studio 2017


## Running and evaluating the sample

* Change to the open vscode instance
* Hit F5, notice another vscode instance appearing, as it executes `Code.exe --debugBrkPluginHost=8680 --extensionDevelopmentPath=<your-Path>\vscode-testextension`
* Optionally close vscode's welcome screen after checking the "Don't show me again" box
* Hit Ctrl-Shift-U to open the output window
* Select `Coco/R Generic` from the drop down box, otherwise you won't see any logging
* Now open and modify the sample file `sample\SampleServer\WFModel\SampleWfModel.txt`
* Notice some logging


## Evaluating Implemented Language Server Features

Change to the open vscode instance first and load the sample file `sample\SampleServer\WFModel\SampleWfModel.txt`.

See https://github.com/Microsoft/language-server-protocol/blob/master/protocol.md for the protocol definition.


### Hover

Point the mouse cursor over a word in the "code". 
Notice a markdown formatted tooltip with some basic 
information concerning the word under the mouse cursor.


## Diagnostics

Notice several diagnostic messages in vscode's lower left status bar. 
Click on it to display the detailed list.
Click on a diagnostic line to get to the error's position.
