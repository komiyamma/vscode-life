'use strict';
// The module 'vscode' contains the VS Code extensibility API Import the module
// and reference it with the alias vscode in your code below
import * as vscode from 'vscode';

const pl2exe_pl_path = "C:/usr/perl/site/bin/pl2exe.pl"

let exec = require("child_process").exec;

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

        // ファイル名の取得
        var filename = vscode.window.activeTextEditor.document.fileName;

        exec_command('perl' + ' ' + pl2exe_pl_path + ' ' +filename);

        // The code you place here will be executed every time your command is executed
        // Display a message box to the user
        console.log('pl2exe!');
    });

    context.subscriptions.push(disposable);
}

/**
 * 
 * @param command : 実行するコマンドの文字列
 */
function exec_command(command :string) {
    exec(command , function (error, stdout, stderr) {
        console.log('stdout: ' + stdout);
        console.log('stderr: ' + stderr);
        if (error !== null) {
            console.log('exec error: ' + error);
        }
    });
}

// this method is called when your extension is deactivated
export function deactivate() {}