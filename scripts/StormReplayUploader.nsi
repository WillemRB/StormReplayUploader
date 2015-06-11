; StormReplayUploader.nsi
;
; This script installs the StormReplayUploader 

;--------------------------------

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
  
  File StormReplayUploader\bin\Release\install.ps1
  File StormReplayUploader\bin\Release\uninstall.ps1
  
  ;${PowerShellExecFileLog} "$INSTDIR\install.ps1"
  ${PowerShellExecLog} ""
  
  nsExec::ExecToLog StormReplayUploader.exe install --localsystem --autostart
  ;nsExec::ExecToLog
SectionEnd ; end the section
