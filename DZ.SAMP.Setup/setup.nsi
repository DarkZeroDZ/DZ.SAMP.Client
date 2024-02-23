;--------------------------------
;Include Modern UI

  !include "MUI.nsh"

;--------------------------------
;Include Font Stuff


;--------------------------------
;General

!define VERSION "0.3.7 R5"

Name "San Andreas Multiplayer ${VERSION}"
OutFile "sa-mp-${VERSION}-install.exe"
AutoCloseWindow true
DirText "Please select your Grand Theft Auto: San Andreas directory:"
InstallDir "$PROGRAMFILES\Rockstar Games\GTA San Andreas\"
InstallDirRegKey HKLM "Software\Rockstar Games\GTA San Andreas\Installation" ExePath

;--------------------------------
;Interface Settings

  !define MUI_ABORTWARNING

;--------------------------------
;Pages


  !define MUI_WELCOMEPAGE_TITLE "Welcome!"
  !define MUI_FINISHPAGE_TITLE "Installation Complete."

  !insertmacro MUI_PAGE_LICENSE "Resources\samp-license.txt"
  !insertmacro MUI_PAGE_DIRECTORY
  !insertmacro MUI_PAGE_INSTFILES
  !insertmacro MUI_PAGE_FINISH

  !insertmacro MUI_UNPAGE_INSTFILES
  !insertmacro MUI_UNPAGE_FINISH

;--------------------------------
;Languages
 
  !insertmacro MUI_LANGUAGE "English"

;--------------------------------
;Functions

Function .onVerifyInstDir
	IfFileExists $INSTDIR\gta_sa.exe GoodGood
		Abort
	GoodGood:
FunctionEnd

;--------------------------------
;Installer Sections

Section "" 
	SetOutPath $INSTDIR
	File ..\DZ.SAMP.Client\bin\release\sa-multiplayer.exe
	File Resources\*

	SetOutPath $INSTDIR\SAMP
	File Resources\SAMP\*

	SetOutPath $INSTDIR\de
	File ..\DZ.SAMP.Client\bin\release\de\*

	SetOutPath $SYSDIR
	File "c:\windows\system32\d3dx9_25.dll"

        SetOutPath "$FONTS"
        File Resources\gtaweap3.ttf

	SetOutPath $INSTDIR
	WriteUninstaller SAMPUninstall.exe
	
	CreateDirectory "$SMPROGRAMS\San Andreas Multiplayer"
	CreateShortcut "$SMPROGRAMS\San Andreas Multiplayer\San Andreas Multiplayer.lnk" "$INSTDIR\sa-multiplayer.exe" "" "$INSTDIR\sa-multiplayer.exe" 0
	CreateShortcut "$SMPROGRAMS\San Andreas Multiplayer\Uninstall.lnk" "$INSTDIR\SAMPUninstall.exe"
	CreateShortCut "$DESKTOP\San Andreas Multiplayer.lnk" "$INSTDIR\sa-multiplayer.exe"
SectionEnd

Section "Uninstall"
	Delete $INSTDIR\samp.exe
	Delete $INSTDIR\sa-multiplayer.exe
	Delete $INSTDIR\samp.dll
	Delete $INSTDIR\samp.saa
	Delete $INSTDIR\samp_debug.exe
	Delete $INSTDIR\SAMPUninstall.exe
	Delete $INSTDIR\bass.dll
	Delete $INSTDIR\gtaweap3.tff
	Delete $INSTDIR\mouse.png
	Delete $INSTDIR\rcon.exe
	Delete $INSTDIR\sampaux3.ttf
	Delete $INSTDIR\sampgui.png
	Delete $INSTDIR\samp-license.txt

        Delete $INSTDIR\SAMP\*
        RMDir $INSTDIR\SAMP

        Delete $INSTDIR\de\*
        RMDir $INSTDIR\de
	
	Delete "$SMPROGRAMS\San Andreas Multiplayer\San Andreas Multiplayer.lnk"
	Delete "$SMPROGRAMS\San Andreas Multiplayer\Uninstall.lnk"
	RMDir "$SMPROGRAMS\San Andreas Multiplayer"
SectionEnd
