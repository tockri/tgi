@echo off

rem TGI2をデプロイする

pushd ..\..

set dstroot=deploy\deploy-%date:~0,4%%date:~5,2%%date:~8,2%
del /S /Q %dstroot%

set binsrc=src\TGI2\bin\x86\Debug
set bindst=%dstroot%\bin
set wwwsrc=src\Client\www
set wwwdst=%dstroot%\bin\www
set filessrc=src\deploy\files
set filesdst=%dstroot%
mkdir %dstroot%
mkdir %bindst%
mkdir %wwwdst%

xcopy /D%dopt% /S /I /Y /EXCLUDE:src\deploy\deploy.exclude %binsrc%\* %bindst%\
xcopy /D%dopt% /S /I /Y %wwwsrc%\* %wwwdst%\
xcopy /D%dopt% /S /I /Y %filessrc%\* %filesdst%\

echo ..\..\%dstroot%\install.batを実行するとC:\TGI2以下にインストールします。
start %dstroot%

pause

popd

