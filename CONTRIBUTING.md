# Contribuire a RCDWeb

Grazie per l'interesse. Il progetto è un'applicazione legacy in fase di recupero: prima di
proporre modifiche ampie, aprire una issue per discutere l'approccio.

## Prima di iniziare

1. Leggere [`PLAN.md`](./PLAN.md) — contiene diagnosi, rubrica di valutazione e fasi di lavoro.
2. Leggere [`SECURITY.md`](./SECURITY.md) — contiene i vincoli **non negoziabili** del repository.
3. Leggere [`THIRD-PARTY.md`](./THIRD-PARTY.md) — nessuna dipendenza non ridistribuibile
   può essere aggiunta.

## Regole non negoziabili

| Regola | Verifica |
|---|---|
| Nessuna credenziale, host interno o dato personale nei file tracciati | test di regressione in `tests/RCDWeb.Tests` |
| Nessun binario commerciale o closed source | revisione manuale + `git ls-files lib/` |
| La licenza resta MIT | nessuna dipendenza incompatibile |
| Nessun segreto nei messaggi di commit, nelle issue e nelle PR | revisione manuale |

## Flusso di lavoro

```bash
git clone https://github.com/donatopepe/RCDWeb.git
cd RCDWeb
git checkout -b feature/<nome-breve>
nuget restore RCDWeb.sln
./scripts/build.sh
./scripts/test.sh
```

1. Un branch per cambiamento, nome descrittivo (`feature/…`, `fix/…`, `docs/…`).
2. Commit piccoli e coerenti; messaggio all'imperativo, in italiano o inglese, purché uniforme.
3. Nessun commit che mescoli refactoring e cambi funzionali.
4. Aggiornare `CHANGELOG.md` nella sezione `[Unreleased]`.
5. Aprire la pull request descrivendo: problema, approccio, come è stato verificato.

## Stile di codice

- Seguire `.editorconfig` (indentazione, fine riga, charset).
- C#: mantenere lo stile del file che si modifica; non riformattare file interi.
- Non toccare i file `*.generated.cs` a mano se si può rigenerarli; se è inevitabile,
  mantenere `.cshtml` e `.generated.cs` allineati.
- Le stringhe visibili all'utente sono in italiano: mantenerle coerenti.

## Cosa serve in una pull request

- [ ] Il progetto compila (`./scripts/build.sh`)
- [ ] I test passano (`./scripts/test.sh`)
- [ ] Nessun nuovo segreto o riferimento infrastrutturale
- [ ] `CHANGELOG.md` aggiornato
- [ ] La descrizione spiega **come** è stato verificato il cambiamento (comando + output)

## Aree in cui il contributo è più utile

1. Rendere compilabile il progetto (Fase 2 e 3 di `PLAN.md`).
2. Sostituire le dipendenze non ridistribuibili: in particolare `AreaCommonWeb.dll`
   (shim) e `itextsharp`/`Office.Interop.Word` (OpenXML).
3. Aumentare la copertura dei test sulla logica testabile senza IIS e senza database.
4. Eliminare il codice morto e unificare le classi duplicate.
5. Migliorare la documentazione di installazione su ambienti diversi.

## Licenza dei contributi

Inviando una pull request si accetta che il contributo sia distribuito con la licenza
[MIT](./LICENSE) del progetto.
