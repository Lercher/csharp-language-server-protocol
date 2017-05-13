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
* Hit F5, notice another vscode instance appearing, 
as it executes `Code.exe --debugBrkPluginHost=8680 --extensionDevelopmentPath=<your-Path>\vscode-testextension`
* Optionally close vscode's welcome screen after checking the "Don't show me again" box
* Hit Ctrl-Shift-U to open the output window
* Select `Coco/R Generic` from the drop down box, otherwise you won't see any logging
* Now open and modify the sample file `sample\SampleServer\WFModel\SampleWfModel.txt`
* Notice some logging


## Evaluating Implemented Language Server Features

Change to the open vscode instance first and load 
the sample file `sample\SampleServer\WFModel\SampleWfModel.txt`.

See [Language Server Protocol](https://github.com/Microsoft/language-server-protocol/blob/master/protocol.md) 
for the protocol definition.


### Hover

Point the mouse cursor to a word in the "code". 
Notice a markdown formatted tooltip with some basic 
information concerning the word under the mouse cursor.


### Diagnostics

Notice a diagnostic messages in vscode's lower left status bar. 
Click on it to display the detailed list.
Click on a diagnostic line to get to the error's position. 
Try to correct the error (Hint: Try `Ctrl-Space`).


### Go to Definition

Find the line 
````
List WeitereGeschäftsführer as KundePrivat
````
and click on `KundePrivat`. Then press `F12` - `Go to definition`.
Notice that the Cursor will jump to `KundePrivat` in the line 
````
Class KundePrivat {Privatkunde bzw. Firmeninhaber/Geschäftsführer}
````

### Find all References

Find the line (somewhere around line 164)
````
Property Aufenthaltsdauer_unbegrenzt Mimics NeinJa
````
and click on `NeinJa`. Then press `Shift-F12` - `Find all references`.
Notice the embedded view with 11 references of the symbol on the right. 
One of these lines, in our example the last one, contains the definition 
of the symbol as described in the previous section.


### Code Completion, aka Intellisense

Place the cursor inside some word after an `as` or `Mimics` keyword, inside 
a keyword such as `Property` in the example file. Press `Ctrl-Space` and notice 
a short list of variants for this word. Now move the cursor to the start of 
such a word and press `Ctrl-Space` again. Notice that a much longer list of 
completion items appears and that each line is marked with "`- Keyword`" for 
terminal symbols, "`- structure`" for non-terminal symbols and 
"`- ` *x* ` symbol`", where *x* is the name of a symbol table defined in 
the *.atg file. In the example grammar `sample\SampleServer\WFModel\WFModel.atg` 
there are these
````
SYMBOLTABLES
	types.
	enumtypes.
````


Please consider a word of *warning*: Coco/R is not very good at providing completion, 
if there are syntactical errors in the DSL and this is *the* common case, when 
writing a document from scratch.

### Rename Symbol

Find a ... `Mimicas NeinJa` ... line, e.g.
````
Property NeinJa Mimics NeinJa
````
and click on `NeinJa`, which is a symbol of type `enumtypes` in the example grammar. 
Then first hit `Shift-F12` to see a list of all occurrences of the symbol and then
`F2` and enter `NeinOderJa` in the small input box. Notice that all symbols `NeinJa` 
in the enum position, but not in the variable position, changed their name to the 
new one. However, vscode does not update the "all references" list automatically, 
but if you close and re-open it, all is OK again.


### (CodeLens)

Not yet implemented. Commands will probably only output as info lines to the Output window.

[API for vscode](https://code.visualstudio.com/docs/extensionAPI/language-support#_codelens-show-actionable-context-information-within-source-code)

[CodeLens Class](http://vshaxe.github.io/vscode-extern/vscode/CodeLens.html)

[vscode Complex commands](https://code.visualstudio.com/docs/extensionAPI/vscode-api-commands)  
  `vscode.previewHtml` - Render the HTML of the resource in an editor view.

  `uri` Uri of the resource to preview.  
  `column` (optional) Column in which to preview.  
  `label` (optional) An human readable string that is used as title for the preview.

[vscode.previewHTML Sample](https://github.com/Microsoft/vscode-extension-samples/tree/master/previewhtml-sample)

[A TextDocumentContentProvider](https://github.com/Microsoft/vscode-extension-samples/tree/master/contentprovider-sample)
