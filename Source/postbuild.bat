rem ============================================================================
rem  RCDWeb - evento di post-build (opzionale)
rem
rem  Copia gli assembly compilati nella cartella "lib" di un'applicazione host.
rem  Impostare la variabile HOST_LIB_PATH sulla cartella desiderata; se non e'
rem  definita non viene copiato nulla e la build prosegue comunque.
rem
rem  Se la copia non serve, commentare l'evento PostBuildEvent in RCDWeb.csproj
rem  oppure eliminare questo file.
rem ============================================================================

if not defined HOST_LIB_PATH goto :nessunhost

if not exist "%HOST_LIB_PATH%" (
    echo [postbuild] cartella host non trovata: %HOST_LIB_PATH%
    goto :eof
)

xcopy "$(TargetDir)RCDWeb.dll" "%HOST_LIB_PATH%" /y/s
xcopy "$(TargetDir)RCDWeb.pdb" "%HOST_LIB_PATH%" /y/s
goto :eof

:nessunhost
echo [postbuild] HOST_LIB_PATH non definita: nessuna copia eseguita.
