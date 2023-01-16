Per utilizzare il modello Intranet, sarà necessario abilitare l'autenticazione 
di Windows e disabilitare l'autenticazione anonima.

Per istruzioni dettagliate (incluso istruzioni per IIS 6.0), visitare
http://go.microsoft.com/fwlink/?LinkID=213745

IIS 7
1. Aprire Gestione IIS e passare al proprio sito Web.
2. In Visualizzazione funzionalità, fare doppio clic su Autenticazione.
3. Nella pagina Autenticazione, selezionare Autenticazione di Windows. Se 
   Autenticazione di Windows non è disponibile, accertarsi che l'autenticazione 
   di Windows sia installata nel server.
        Per abilitare l'autenticazione di Windows:
 a) In Pannello di controllo, aprire "Programmi e funzionalità".
 b) Selezionare "Attivazione o disattivazione delle funzionalità Windows".
 c) Andare a Internet Information Services | Servizi Web | Sicurezza
    e accertarsi che il nodo Autenticazione di Windows sia selezionato.
4. Nel riquadro Azioni, fare clic su Abilita per utilizzare l'autenticazione di 
   Windows.
5. Nella pagina Autenticazione, selezionare Autenticazione anonima.
6. Nel riquadro Azioni, fare clic su Disabilita per disabilitare 
   l'autenticazione anonima.

IIS Express
1. In Visual Studio, fare clic con il pulsante destro del mouse sul progetto e 
   selezionare Usa IIS Express.
2. In Esplora soluzioni, fare clic sul progetto per selezionarlo.
3. Se il riquadro Proprietà non è aperto, aprirlo (F4).
4. Nel riquadro Proprietà del progetto:
 a) Impostare "Autenticazione anonima" su "Disabilitata".
 b) Impostare "Autenticazione di Windows" su "Abilitata".

È possibile installare IIS Express mediante Installazione guidata piattaforma 
Web Microsoft:
    Per Visual Studio: http://go.microsoft.com/fwlink/?LinkID=214802
    Per Visual Web Developer: http://go.microsoft.com/fwlink/?LinkID=214800
