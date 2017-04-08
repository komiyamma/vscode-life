# 元のスクリプトファイル
my $strSrcFullPath  = shift @ARGV;
my ($strSrcDirectory) = $strSrcFullPath =~ /^(.+)\\.+?/;
chdir $strSrcDirectory;

# 実行ファイル
my $strDstFullPath = $strSrcFullPath;
   $strDstFullPath =~ s/\.pl/\.exe/i;

# 一端 par で固める。
my $strPerlPackerCommand = "pp -C -o $strSrcFullPath.exe $strSrcFullPath";
print $strPerlPackerCommand."\n";
`$strPerlPackerCommand`;

# リソースを変更し、abc.pl に対応する abc.exe の名前のものへ。
my $strResHackerCommand = "res_hacker -modify $strSrcFullPath.exe, $strDstFullPath, ".'C:\usr\perl\site\bin\par.res , , ,';
print $strResHackerCommand."\n";
`$strResHackerCommand`;

print "\[$strSrcFullPath\] -> \[$strDstFullPath\] ...compile complete!!\n";

# 不要な par は削除
unlink "$strSrcFullPath.exe";

