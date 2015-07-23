; StormReplayUploader.nsi
;
; This script installs the StormReplayUploader 

;--------------------------------

!include "nsisXML.nsh"

!macro PowerShellExecLogMacro PSCommand
  InitPluginsDir
  ;Save command in a temp file
  Push $R1
  FileOpen $R1 $PLUGINSDIR\tempfile.ps1 w
  FileWrite $R1 "${PSCommand}"
  FileClose $R1
  Pop $R1
 
  !insertmacro PowerShellExecFileLogMacro "$PLUGINSDIR\tempfile.ps1"
!macroend

!macro PowerShellExecFileLogMacro PSFile
  !define PSExecID ${__LINE__}
  Push $R0
 
  nsExec::ExecToLog 'powershell -inputformat none -ExecutionPolicy Unrestricted -File "${PSFile}"  '
  Pop $R0 ;return value is on stack
  IntCmp $R0 0 finish_${PSExecID}
  SetErrorLevel 2
 
finish_${PSExecID}:
  Pop $R0
  !undef PSExecID
!macroend

!define PowerShellExecLog `!insertmacro PowerShellExecLogMacro`
!define PowerShellExecFileLog `!insertmacro PowerShellExecFileLogMacro`

; $DOCUMENTS

; The name of the installer
Name "StormReplay Uploader"

; The file to write
OutFile "StormReplay Uploader.exe"

; The default installation directory
InstallDir "$PROGRAMFILES\StormReplay Uploader"

; Request application privileges for Windows Vista
RequestExecutionLevel highest

;--------------------------------

; Pages
Page directory
Page instfiles

;--------------------------------

Function "FindHotsReplayFolderPath"
  ; Returns the default path to the StormReplay files on this machine
  ReadRegStr $4 HKCU "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders" Personal
  StrCpy $4 "$4\Heroes of the Storm\Accounts"
FunctionEnd

; The stuff to install
Section "Install" ;No components page, name is not important

  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ; Use current user's paths
  SetShellVarContext current
  
  ; Put file there
  File StormReplayUploader\bin\Release\AWSSDK.dll
  File StormReplayUploader\bin\Release\System.Reactive.Core.dll
  File StormReplayUploader\bin\Release\System.Reactive.Interfaces.dll
  File StormReplayUploader\bin\Release\System.Reactive.Linq.dll
  File StormReplayUploader\bin\Release\System.Reactive.PlatformServices.dll
  File StormReplayUploader\bin\Release\Topshelf.dll
  
  File StormReplayUploader\bin\Release\StormReplayUploader.exe
  File StormReplayUploader\bin\Release\StormReplayUploader.exe.config
  
  File StormReplayUploader\bin\Release\StormReplayUploader.Targets.dll
  
  File StormReplayUploader\bin\Release\Serilog.dll
  File StormReplayUploader\bin\Release\Serilog.FullNetFx.dll
  File StormReplayUploader\bin\Release\Serilog.Sinks.EventLog.dll
  
  File StormReplayUploader\bin\Release\install.ps1
  File StormReplayUploader\bin\Release\uninstall.ps1
  File StormReplayUploader\bin\Release\start.ps1
  
  ; Update configuration file
  Call "FindHotsReplayFolderPath"
  
  ; Wisou XML  Plugin
  ;nsisXML::create
  ;nsisXML::load $INSTDIR\StormReplayUploader.exe.config
  ;DetailPrint "Loaded file: $0"
  ;DetailPrint "Root is $1"
  ;nsisXML::select '/configuration/uploaderConfiguration'
  ;DetailPrint "Reference is $2"
  ;nsisXML::getAttribute "replayDirectory"
  ;nsisXML::setAttribute "replayDirectory" $4
  ;nsisXML::save $INSTDIR\StormReplayUploader.exe.config
  
  ; Joel XML  Plugin
  ${nsisXML->OpenXML} $INSTDIR\StormReplayUploader.exe.config
  ${nsisXML->SetElementAttr} "/configuration/uploaderConfiguration" "replayDirectory" $4
  ${nsisXML->CloseXML}
  
  ${PowerShellExecFileLog} "$INSTDIR\start.ps1"
  
  ; Freezes the installation
  ;nsExec::ExecToLog StormReplayUploader.exe install --localsystem --autostart
  ;nsExec::ExecToLog
SectionEnd ; end the section
