# ���̃X�N���v�g�t�@�C��
my $strSrcFullPath  = shift @ARGV;
my ($strSrcDirectory) = $strSrcFullPath =~ /^(.+)\\.+?/;
chdir $strSrcDirectory;

# ���s�t�@�C��
my $strDstFullPath = $strSrcFullPath;
   $strDstFullPath =~ s/\.pl/\.exe/i;

# ��[ par �Ōł߂�B
my $strPerlPackerCommand = "pp -C -o $strSrcFullPath.exe $strSrcFullPath";
print $strPerlPackerCommand."\n";
`$strPerlPackerCommand`;

# ���\�[�X��ύX���Aabc.pl �ɑΉ����� abc.exe �̖��O�̂��̂ցB
my $strResHackerCommand = "res_hacker -modify $strSrcFullPath.exe, $strDstFullPath, ".'C:\usr\perl\site\bin\par.res , , ,';
print $strResHackerCommand."\n";
`$strResHackerCommand`;

print "\[$strSrcFullPath\] -> \[$strDstFullPath\] ...compile complete!!\n";

# �s�v�� par �͍폜
unlink "$strSrcFullPath.exe";

