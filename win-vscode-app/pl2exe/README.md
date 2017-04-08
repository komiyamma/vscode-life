# pl2exe README

はじめての拡張機能の作成テスト。現在vscode編集中の「***.pl」ファイルを、「***.exe」とする。  
とはいっても、単純に c:/usr/perl/site/bin/ の中に個人的に入れている、pl2exe.plを実行しているだけ。

## Requirements

PAR::Packerが入っていること  
- *Strberry Perl* なら最新でも入りやすいが、シェア的に難あり  
- *Active Perlだと5.16* ぐらいのやや古めのパッケージでないと、入れるのに苦労する。  
```powershell
ppm install Getopt::ArgvFile
ppm install Module::ScanDeps
ppm install PAR
cpan install PAR::Dist
cpan install Win32::ShellQuote
ppm install Win32::Exe
cpan isntall PAR::Packer
```

WinPARPacker.pl : 引数に現在編集中の.plテキストのフルパスが渡る。
- このファイルは *perl* フォルダの *site\bin* に入れる。

## Extension Settings

なし

## Known Issues

Windowsでしか動作しない。  
最初の版。execをnodeから持ってきているけれども、vscode内のあるのかな？

## Release Notes

### 1.0.0

-----------------------------------------------------------------------------------------------------------


