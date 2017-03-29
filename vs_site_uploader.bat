@echo off
set %SEARCHTEXT= "VS_"
echo %1 | find %SEARCHTEXT% >NUL
if  errorlevel 1 goto END

rem $Host13というのは、NextFTPで、「ショートカット作成」をやってみると、
rem 該当の接続のショートカットが出来上がる。
@echo "upload start..."
@echo on
C:\usr\nextftp\NEXTFTP.EXE $Host13 -local="C:\usr\web\VSCode" -upload=%1 -quit -nosound -minimize
@echo off
@echo "upload complete!"

:END