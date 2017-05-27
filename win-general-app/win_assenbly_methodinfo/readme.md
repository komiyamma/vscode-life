# ClearScriptのための実行ファイル

## .NET のクラスをTypeScript風の宣言にする

- .NET Framework 4.7 その性質上、4系で最も新しい.NET Frameworkを必要なようにしている。
   (新しいものは古いものも読めるが、古いと新しいアセンブリは読めないため)
- 検索対象は、「.NET Framework 4系のフォルダ」と「カレントフォルダ」
- PowerShellだと「`」などがいたずらずるので、cmd.exeの方が良い。

```
WinAssemblyMethodInfoForDTS [NameSpace] [Class] [Option]

-- 「名前空間 System」の「Console」クラスをTypeScript風の宣言に
WinAssemblyMethodInfoForDTS System Console

-- 上と同じだが、「string」「number」「boolean」以外の型は「any」にする
WinAssemblyMethodInfoForDTS System Console any

-- 名前空間が「ない」、名前空間を「問わない」、もしくは名前空間が「よくわからない」状態で、「Form」クラスをTypeScritp風の宣言に
WinAssemblyMethodInfoForDTS any Form

-- 上と同様だが、「string」「number」「boolean」以外の型は「any」にする
WinAssemblyMethodInfoForDTS any Form any

-- １つのGenericパラメータがあるList
WinAssemblyMethodInfoForDTS System.Collections.Generic List`1

-- ２つのGenericパラメータがあるDictionary
WinAssemblyMethodInfoForDTS System.Collections.Generic Dictionary`2


```

