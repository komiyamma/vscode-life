@echo off
set %SEARCHTEXT= "VS_"
echo %1 | find %SEARCHTEXT% >NUL
if  errorlevel 1 goto END

rem $Host13�Ƃ����̂́ANextFTP�ŁA�u�V���[�g�J�b�g�쐬�v������Ă݂�ƁA
rem �Y���̐ڑ��̃V���[�g�J�b�g���o���オ��B
@echo "upload start..."
@echo on
C:\usr\nextftp\NEXTFTP.EXE $Host13 -local="C:\usr\web\VSCode" -upload=%1 -quit -nosound -minimize
@echo off
@echo "upload complete!"

:END