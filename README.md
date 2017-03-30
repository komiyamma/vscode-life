# VSCode.life のサイト構築用のデータ

## 全体の法則

VS_*** という名前規則に従っている。  
これは、アップデートツールなどで、誤爆してもFTPでアップされるのを防止するため。

 * VS_index.php  
プログラム本体。

* VS_index.html  
HTMLテンプレート。

* VS_*.html  
各ページ。HTMLテンプレートの一部となる。

* VS_site_uploader.pl  
アップロードの簡易プログラム。  
Visual Studioから呼ばれている。

* VS_jquery系  
jquery依存。左のメニューの動きの実装。

* SyntaxHighlighter系  
jquery依存。SyntaxHighlightの実装。

* LazyLoad系  
イメージ画像は実際に画面内に入ってから読み込まれる機能。  
jquery依存。LazyLoadの実装。

* HoverCard系  
jquery依存。下線等にマウスをもっていくと、  
文字位置そのままで説明噴出しが被る。





