# ClearScriptのための実行ファイル

## .NET のクラスをTypeScript風の宣言にする

- .NET Framework 4.7 その性質上、4系で最も新しい.NET Frameworkを必要なようにしている。
   (新しいものは古いものも読めるが、古いと新しいアセンブリは読めないため)
- 検索対象は、「.NET Framework 4系のフォルダ」と「カレントフォルダ」
- PowerShellだと「`」などが邪魔するので、cmd.exeの方が良い。
- PowerShellでやる場合は'List`1'や'Dictionary`2'といったように、シングルクォーテーションで囲むと良い。
```
WinAssemblyToTypeScriptDeclare [NameSpace] [Class] -deep:0-2 -complex

-- 「名前空間 System」の「Console」クラスをTypeScript風の宣言に
WinAssemblyToTypeScriptDeclare System Console

-- 上と同じだが、「string」「number」「boolean」以外の型は「any」にする
WinAssemblyToTypeScriptDeclare System Console -deep:0

-- 名前空間が「ない」、名前空間を「問わない」、もしくは名前空間が「よくわからない」状態で、「Form」クラスをTypeScritp風の宣言に
WinAssemblyToTypeScriptDeclare any Form

-- 上と同様だが、「string」「number」「boolean」以外の型は「any」にする
WinAssemblyToTypeScriptDeclare any Form -deep:0

-- １つのGenericパラメータがあるList
WinAssemblyToTypeScriptDeclare System.Collections.Generic List`1

-- ２つのGenericパラメータがあるDictionary
WinAssemblyToTypeScriptDeclare System.Collections.Generic Dictionary`2

-- TypeScriptの文法を超える複雑な型でもあえて受け入れる
WinAssemblyToTypeScriptDeclare System.Collections.Generic Dictionary`2 -complex

-- ２深まで型を分析する。
WinAssemblyToTypeScriptDeclare System.Collections.Generic Dictionary`2 -deep:2


```

