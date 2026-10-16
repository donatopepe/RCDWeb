# Fase 01 — License & Docs

Data: 2026-10-16T08:30:00Z
Variabile modificata: **documentazione e licenza del repository** (da assenti a complete).
Criteri target: C3, C4

## Baseline

```bash
test -f LICENSE    # => assente
test -f README.md  # => assente
```

Unico testo presente: `Source/readme.txt`, il readme generico del template Visual Studio
(istruzioni IIS in italiano), senza relazione con il progetto.

## Modifica

### Nuovi file

| File | Contenuto |
|---|---|
| `LICENSE` | MIT, `Copyright (c) 2014-2026 RCDWeb contributors` (autore approvato dall'utente) |
| `README.md` | Guida completa, 545 righe, **9 sezioni** come da `PLAN.md §5` |
| `SECURITY.md` | Segnalazione vulnerabilità, regole per contributori, hardening in esercizio, nota sull'esposizione storica |
| `CONTRIBUTING.md` | Regole non negoziabili, flusso di lavoro, stile, checklist PR, aree prioritarie |
| `CHANGELOG.md` | Keep a Changelog: sezione `[Unreleased]` con tutto il lavoro della Fase 0 |
| `THIRD-PARTY.md` | Licenze di tutte le dipendenze; distinzione fra incluse e non incluse; incompatibilità MIT |
| `lib/README.md` | Stato di ogni binario rimosso, dove ottenerlo, alternativa open source, comando di verifica |

### Rimosso

- `Source/readme.txt` (readme di template Visual Studio, non pertinente). Il contenuto utile
  sulle autenticazione IIS è stato assorbito in `README.md §4 Passo 7`.

### Contenuto del README (le 9 sezioni richieste)

1. **Panoramica e funzionalità** — diagramma ASCII del flusso di dominio, tabella funzionalità, 28 entità
2. **Stack tecnologico** — tabella versioni + struttura del repository
3. **Requisiti** — minimi, componenti opzionali, build su Linux/Docker
4. **Installazione passo-passo** — 9 passi: clone, restore, dipendenze binarie, creazione DB (con SQL), configurazione, build, IIS, job, verifica
5. **Configurazione** — tabella file, 5 stringhe di connessione, 18 `appSettings`, parametri DB, ELMAH, variabili d'ambiente
6. **Uso e flusso applicativo** — route, azioni di `AccountController`/`RichiestaController`/`RichiestaMetodi`, ruoli, job Quartz, modulistica
7. **Test** — come eseguire, cosa è coperto, cosa **non** è coperto
8. **Troubleshooting** — 12 sintomi frequenti con causa e soluzione
9. **Licenza, crediti e sicurezza** — licenza, origine, dipendenze non incluse, note di sicurezza, stato del progetto, contribuire

## Verifica

```bash
test -f LICENSE && wc -l LICENSE                       # => 21 righe, MIT
for s in "1. Panoramica e funzionalità" "2. Stack tecnologico" "3. Requisiti" \
         "4. Installazione passo-passo" "5. Configurazione" "6. Uso e flusso applicativo" \
         "7. Test" "8. Troubleshooting" "9. Licenza, crediti e sicurezza"; do
  grep -qF "$s" README.md || echo "MISS $s"; done      # => nessun MISS
wc -l README.md                                        # => 545
```

**Regressione C1/C2 dopo le modifiche:**

```bash
grep -rniE "$1=[removed]"' ;<]|<PATTERN_SEGRETI_REDATTI>" \
  --include=*.config --include=*.cs --include=*.cshtml --include=*.csproj --include=*.js --include=*.bat . \
  | grep -v CHANGE_ME_DB_PASSWORD | wc -l              # => 0
grep -rniE "localhost|appserver1|appserver2|example|172\.28\.|svc-rcd|rcd_user|rcd_user|<DOMINIO_AZIENDALE>|<EMAIL_PERSONALE>" \
  --include=*.config --include=*.cs --include=*.cshtml --include=*.csproj --include=*.js --include=*.bat --include=*.xml --include=*.txt . \
  | wc -l                                              # => 0
```

Esito: **PASS** — C3 e C4 verdi, nessuna regressione su C1/C2.

## Criteri aggiornati

| Criterio | Prima | Dopo |
|---|---|---|
| C1 | 🟢 green | 🟢 green (invariato) |
| C2 | 🟢 green | 🟢 green (invariato) |
| C3 | 🔴 red | 🟢 green |
| C4 | 🔴 red | 🟢 green |
| C5–C10 | 🔴 red | 🔴 red (invariati) |

Score: **2/10 → 4/10**.

## Note

- Il badge `Build` nel README punta a `.github/workflows/ci.yml`, che sarà creato in Fase 5.
  Fino ad allora il badge non ha stato: è un riferimento avanti, non un'affermazione falsa.
- `README.md §9.5` riporta in modo esplicito lo stato reale della rubrica (4/10), così la
  documentazione non afferma un funzionamento non ancora verificato.
- Le decisioni dell'utente (opzione architetturale A, autore/anno MIT, autorizzazione alla
  riscrittura della storia git) sono registrate in `docs/WORKSTATE.json → decisions` e in
  `PLAN.md §6/§8bis`.

## Prossima fase

FASE 2 — Dependencies (C7).
