@echo off
for /f %%f in ('dir /a:-hd /b') do xcopy /y Champion.cs %%f\%%f\Champion.cs