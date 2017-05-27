# ClearScriptのための実行ファイル

## .NET のクラスをTypeScript風の宣言にする


- 検索対象は、「.NET Framework 4系のフォルダ」と「カレントフォルダ」
- PowerShellだと「`」などがいたずらずるので、cmd.exeの方が良い。

```
WinAssenblyMethodInfoFotDTS [NameSpace] [Class] [Option]

-- 「名前空間 System」の「Console」クラスをTypeScript風の宣言に
WinAssenblyMethodInfoFotDTS System Console

-- 上と同じだが、「string」「number」「boolean」以外の型は「any」にする
WinAssenblyMethodInfoFotDTS System Console any

-- 名前空間が「ない」、名前空間を「問わない」、もしくは名前空間が「よくわからない」状態で、「Form」クラスをTypeScritp風の宣言に
WinAssenblyMethodInfoFotDTS any Form

-- 上と同様だが、「string」「number」「boolean」以外の型は「any」にする
WinAssenblyMethodInfoFotDTS any Form any

-- １つのGenericパラメータがあるList
WinAssenblyMethodInfoFotDTS System.Collections.Generic List`1

-- ２つのGenericパラメータがあるDictionary
WinAssenblyMethodInfoFotDTS System.Collections.Generic Dictionary`2


```

