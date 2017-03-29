# VSCodeサイトへのFTP
my $filename = $ARGV[0];
if ($filename =~ /\bVS_/i) {
	print "upload start...\n";
	$cmd = sprintf('cmd /C C:\usr\nextftp\NEXTFTP.EXE $Host13 -local="C:\usr\web\VSCode" -upload=%s -quit -nosound -minimize', $filename );
	print $cmd . "\n";
	print `$cmd`;

	print "upload complete!\n";
} else {
	print "error";
}
