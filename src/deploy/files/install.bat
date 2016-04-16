@echo off
set dst=C:\TGI2
mkdir %dst%
xcopy /S /I /Y bin %dst%\bin
xcopy /S /I /Y data %dst%\data