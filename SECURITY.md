# Politica di sicurezza

## Segnalare una vulnerabilità

**Non aprire una issue pubblica.** Inviare una segnalazione privata al maintainer del repository
tramite la funzione *Security → Report a vulnerability* di GitHub.

Includere:

- descrizione della vulnerabilità e impatto stimato
- passi di riproduzione minimi e riproducibili
- versione/commit interessato
- eventuale proposta di correzione

Tempi di risposta attesi: conferma entro 7 giorni, valutazione entro 30 giorni.

## Regole per i contributori

Questo repository è pubblico. **Non introdurre mai** nei file tracciati:

| Categoria | Esempi |
|---|---|
| Credenziali | password, `apiKey`, token, connection string con `User ID=`/`$1=[removed] |
| Segreti infrastrutturali | hostname interni, indirizzi IP privati, nomi di dominio aziendali, path UNC di rete |
| Dati personali | email reali, nomi, matricole, codici fiscali |
| Materiale non ridistribuibile | binari commerciali o closed source (vedi `THIRD-PARTY.md`) |

I test di regressione in `tests/RCDWeb.Tests` verificano automaticamente queste regole e
falliscono se vengono reintrodotti valori vietati.

## Configurazione sicura in esercizio

1. **Stringhe di connessione**: preferire `Integrated Security=SSPI` con l'identità del pool IIS.
   Se serve autenticazione SQL, fornire la password tramite variabile d'ambiente o file di
   override non versionato.
2. **ELMAH**: mantenere `allowRemoteAccess="0"`, `elmah.mvc.requiresAuthentication="true"` e
   `elmah.mvc.allowedRoles` limitato a un ruolo amministrativo. Il log errori può contenere
   stringhe di connessione e dati di dominio.
3. **`customErrors`**: mantenere `RemoteOnly`. `Off` espone stack trace.
4. **Autenticazione**: `Web.config` è predisposto per l'autenticazione di Windows. Se si modifica
   la modalità di autenticazione, verificare che le pagine dell'area `RCD` e `elmah` restino
   protette.
5. **Upload allegati**: validare estensione, tipo MIME e dimensione; non fidarsi del nome file
   fornito dal client (rischio path traversal).
6. **Permessi file system**: l'identità del pool necessita di scrittura **solo** sulle cartelle
   di upload e sulla cartella temporanea dei documenti generati.
7. **HTTPS**: terminare TLS a livello di IIS e forzare il redirect da HTTP.
8. **Dipendenze**: mantenere aggiornati i pacchetti NuGet; le librerie legacy incluse hanno
   vulnerabilità note (vedi `THIRD-PARTY.md`).

## Esposizione storica (già sanata nel working tree)

Versioni precedenti di questo repository hanno contenuto in chiaro credenziali di database,
credenziali di web service e host interni. Chiunque abbia clonato o forkato il repository prima
della sanizzazione **possiede ancora quei valori**.

Pertanto:

- le credenziali esposte in passato devono essere considerate **compromesse** e **ruotate**
  indipendentemente da qualsiasi riscrittura della storia git;
- la riscrittura della storia non raggiunge i fork e i cloni già esistenti;
- i valori effettivi non sono riportati in questo documento né altrove nel repository.

Vedi la sezione `user_actions_required` in `docs/WORKSTATE.json` per l'elenco delle azioni
richieste all'operatore.

## Versioni supportate

Il progetto è in fase di recupero. Solo il branch `master` corrente è supportato.
