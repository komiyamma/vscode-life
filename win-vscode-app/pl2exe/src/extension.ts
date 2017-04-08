'use strict';
// The module 'vscode' contains the VS Code extensibility API Import the module
// and reference it with the alias vscode in your code below
import * as vscode from 'vscode';

let process_exec = require("child_process").exec;

// this method is called when your extension is activated your extension is
// activated the very first time the command is executed
export function activate(context : vscode.ExtensionContext) {

    // Use the console to output diagnostic information (console.log) and errors
    // (console.error) This line of code will only be executed once when your
    // extension is activated
    console.log('Congratulations, your extension "pl2exe" is now active!');

    // The command has been defined in the package.json file Now provide the
    // implementation of the command with  registerCommand The commandId parameter
    // must match the command field in package.json
    let disposable = vscode.commands.registerCommand('extension.pl2exe', () => {

        var wpp: WinPARPacker = new WinPARPacker();
        wpp.Compile();

        // The code you place here will be executed every time your command is executed
        // Display a message box to the user
        console.log('pl2exe!');
    });

    context.subscriptions.push(disposable);
}

class WinPARPacker {

    private static command = "C:/usr/perl/site/bin/WinPARPacker.pl";
    private static interpreter = "perl.exe";

    constructor() {
    }
    
    private GetCommandString(): string {
        // ファイル名の取得
        var filename = vscode.window.activeTextEditor.document.fileName;
        var command = WinPARPacker.interpreter + ' ' + WinPARPacker.command + ' ' +filename;
        return command;
    }

    /**
     * 外部コマンドを実行する。
     * @param command : 実行するコマンドの文字列
     */
    public Compile() {
        process_exec(this.GetCommandString(), this.OutputDataRecieved );
    }

    /**
     * execしたアウトプットの受信
     * @param error : エラーメッセージ
     * @param stdout : 標準出力メッセージ
     * @param stderr : 標準エラーメッセージ
     */
    private OutputDataRecieved (error: string, stdout: string, stderr: string) {
        console.log('stdout: ' + stdout);
        console.log('stderr: ' + stderr);
        if (error !== null) {
            console.log('exec error: ' + error);
        } else {
            WinPARPacker.OutputWindow(stdout);
        }
    }

    /**
     * 出力ウィンドウに文字列を出す。
     * @param channnel_name : チャンネルの名前
     */
    static OutputWindow(message: string) {
        vscode.window.showInformationMessage("compile complete!!")
        var output = vscode.window.createOutputChannel("PAR::Packer");
        output.show();
        output.clear();
        output.append(message)
    }
}

// this method is called when your extension is deactivated
export function deactivate() {}