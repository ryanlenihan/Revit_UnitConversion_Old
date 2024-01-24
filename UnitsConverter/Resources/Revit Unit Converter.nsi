;NSIS Modern User Interface
;Welcome/Finish Page Example Script
;Written by Joost Verburg

;--------------------------------
;Include Modern UI

  !include "MUI2.nsh"

;--------------------------------
;General

  ;Define name of the product
  !define PRODUCT "Revit Unit Conversion Tool"
  
  ;Name and file
  
  ;Define the main name of the installer
  Name "${PRODUCT}"
  
  OutFile "${PRODUCT} Setup.exe"
  Unicode True

  ;Default installation folder
  InstallDir "$APPDATA\Autodesk\Revit\Addins\"

  ;Get installation folder from registry if available
  InstallDirRegKey HKCU "Software\${PRODUCT}" ""

  ;Request application privileges for Windows Vista
  RequestExecutionLevel user

;--------------------------------
;Interface Settings

  !define MUI_ABORTWARNING
  
  ;Use optional a custom picture for the 'Welcome' and 'Finish' page:
  !define MUI_HEADERIMAGE_RIGHT
  !define MUI_WELCOMEFINISHPAGE_BITMAP "${NSISDIR}\Docs\Modern UI\images\rau.bmp"  # for the Installer
  !define MUI_UNWELCOMEFINISHPAGE_BITMAP "${NSISDIR}\Docs\Modern UI\images\rau.bmp"  # for the later created UnInstaller

;--------------------------------
;Pages

  !insertmacro MUI_PAGE_WELCOME
  !insertmacro MUI_PAGE_LICENSE "${NSISDIR}\Docs\Modern UI\License.txt"
  !insertmacro MUI_PAGE_COMPONENTS
  ;!insertmacro MUI_PAGE_DIRECTORY
  !insertmacro MUI_PAGE_INSTFILES
  !insertmacro MUI_PAGE_FINISH

  !insertmacro MUI_UNPAGE_WELCOME
  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES
  !insertmacro MUI_UNPAGE_FINISH

;--------------------------------
;Languages

  !insertmacro MUI_LANGUAGE "English"

;--------------------------------
;Installer Sections

Section "Revit 2021" Revit2021

  SectionIn RO # Just means if in component mode this is locked

  ;Set output path to the installation directory.
  SetOutPath $INSTDIR\2021

  ;Put the following file in the SetOutPath
  File ".\2021\UnitsConverter.dll"
  File ".\2021\UnitsConverter.addin"

  ;Store installation folder in registry
  WriteRegStr HKLM "Software\${PRODUCT}" "" $INSTDIR\2021

  ;Registry information for add/remove programs
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}" "DisplayName" "${PRODUCT}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}" "UninstallString" '"$INSTDIR\${PRODUCT}_uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}" "NoRepair" 1

  ;Create uninstaller and Main component
  
  ;Create uninstaller
  WriteUninstaller "${PRODUCT}_uninstaller.exe"

SectionEnd

Section "Revit 2020" Revit2020

  ; Save something else optional to the installation directory.
  SetOutPath $INSTDIR\2020

  ;Put the following file in the SetOutPath
  File ".\2020\UnitsConverter.dll"
  File ".\2020\UnitsConverter.addin"


SectionEnd

Section "Revit 2019" Revit2019

  ; Save something else optional to the installation directory.
  SetOutPath $INSTDIR\2019

  ;Put the following file in the SetOutPath
  File ".\2019\UnitsConverter.dll"
  File ".\2019\UnitsConverter.addin"


SectionEnd

Section "Revit 2018" Revit2018

  ; Save something else optional to the installation directory.
  SetOutPath $INSTDIR\2018

  ;Put the following file in the SetOutPath
  File ".\2018\UnitsConverter.dll"
  File ".\2018\UnitsConverter.addin"


SectionEnd

Section "Revit 2017" Revit2017

  ; Save something else optional to the installation directory.
  SetOutPath $INSTDIR\2017

  ;Put the following file in the SetOutPath
  File ".\2017\UnitsConverter.dll"
  File ".\2017\UnitsConverter.addin"


SectionEnd

;--------------------------------
;Descriptions

  ;Language strings
  LangString DESC_Revit2021 ${LANG_ENGLISH} "Revit 2021 metric and imperial conversion component. Includes both current file and batch processor."
  LangString DESC_Revit2020 ${LANG_ENGLISH} "Revit 2020 metric and imperial conversion component. Includes both current file and batch processor."
  LangString DESC_Revit2019 ${LANG_ENGLISH} "Revit 2019 metric and imperial conversion component. Includes both current file and batch processor."
  LangString DESC_Revit2018 ${LANG_ENGLISH} "Revit 2018 metric and imperial conversion component. Includes both current file and batch processor."
  LangString DESC_Revit2017 ${LANG_ENGLISH} "Revit 2017 metric and imperial conversion component. Includes both current file and batch processor."

  ;Assign language strings to sections
  !insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
    !insertmacro MUI_DESCRIPTION_TEXT ${Revit2021} $(DESC_Revit2021)
	!insertmacro MUI_DESCRIPTION_TEXT ${Revit2020} $(DESC_Revit2020)
	!insertmacro MUI_DESCRIPTION_TEXT ${Revit2019} $(DESC_Revit2019)
	!insertmacro MUI_DESCRIPTION_TEXT ${Revit2018} $(DESC_Revit2018)
	!insertmacro MUI_DESCRIPTION_TEXT ${Revit2017} $(DESC_Revit2017)
  !insertmacro MUI_FUNCTION_DESCRIPTION_END

;--------------------------------
;Uninstaller Section

Section "Uninstall"

  ;Remove all registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT}"
  DeleteRegKey HKLM "Software\${PRODUCT}"

  ;Delete the installation directory + all files in it
  ;Add 'RMDir /r "$INSTDIR\folder\*.*"' for every folder you have added additionaly
  Delete "$INSTDIR\2021\UnitsConverter.dll"
  Delete "$INSTDIR\2021\UnitsConverter.addin"
  Delete "$INSTDIR\2020\UnitsConverter.dll"
  Delete "$INSTDIR\2020\UnitsConverter.addin"
  Delete "$INSTDIR\2019\UnitsConverter.dll"
  Delete "$INSTDIR\2019\UnitsConverter.addin"
  Delete "$INSTDIR\2018\UnitsConverter.dll"
  Delete "$INSTDIR\2018\UnitsConverter.addin"
  Delete "$INSTDIR\2017\UnitsConverter.dll"
  Delete "$INSTDIR\2017\UnitsConverter.addin"

  ;Delete Start Menu Shortcuts
  Delete "$SMPROGRAMS\${PRODUCT}\*.*"
  RmDir  "$SMPROGRAMS\${PRODUCT}"

SectionEnd