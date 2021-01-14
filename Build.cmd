cd /D "%~dp0"
powershell -noexit -executionpolicy bypass -File .\Build-Msi.ps1
pause