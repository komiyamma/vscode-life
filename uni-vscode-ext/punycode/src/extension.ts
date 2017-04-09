'use strict';
// The module 'vscode' contains the VS Code extensibility API
// Import the module and reference it with the alias vscode in your code below
import * as vscode from 'vscode';
let punycode = require('punycode');

// this method is called when your extension is activated
// your extension is activated the very first time the command is executed
export function activate(context: vscode.ExtensionContext) {

    // Use the console to output diagnostic information (console.log) and errors (console.error)
    // This line of code will only be executed once when your extension is activated
    console.log('Congratulations, your extension "punycode" is now active!');

    // The command has been defined in the package.json file
    // Now provide the implementation of the command with  registerCommand
    // The commandId parameter must match the command field in package.json
    let disposable = vscode.commands.registerCommand('extension.punycode', () => {
        // The code you place here will be executed every time your command is executed

        let editor = vscode.window.activeTextEditor;
        let selection = editor.selection
        var sel = editor.document.getText(selection);

        if (!sel) {
            OutputWindow("文字列を選択した状態で使ってください。")
        } else{
            let conved_text = GetConvertedPunyCode(sel);
            OutputWindow(conved_text);
        }
        // Display a message box to the user
        vscode.window.showInformationMessage('ConvertPunyCode!');
    });

    context.subscriptions.push(disposable);
}

// this method is called when your extension is deactivated
export function deactivate() {
}

/**
 * テキストを PunyCode⇔国際化ドメイン へと相互変換する。
 * @param sel : テキスト文字列
 */
function GetConvertedPunyCode(sel: string) {
    let ret :string;

    try {
        // 現在選択対象がすでにPunycodeなら
        if ( sel.indexOf("xn--") != -1 ) {
            // ホスト名相当する部分を国際化ドメインへ
            ret = punycode.toUnicode(sel);
        
        } else {
            // ホスト名相当する部分をPunycodeへ
            ret = punycode.toASCII(sel);
        }

        return ret

    } catch (e) {
        // とりあえずエラー出力ウィンドウへ
        OutputWindow(e);

	    return ret;
    }
}

/**
 * 対象のメッセージを出力ウィンドウへと表示する
 * @param message : 表示対象のメッセージ
 */
var outputChannel = vscode.window.createOutputChannel("ConvertPunyCode");
function OutputWindow(message: string) {
    outputChannel.show();
    outputChannel.clear();
    outputChannel.append(message + "\n");
}
