# Fase 4: contenuti da approvare e zone grigie

**Data:** 2026-10-02
**Stato:** analisi del decider, nessuna decisione presa. Ogni voce diventa una
ruling quando la decidi.
**Riferimenti:** `docs/SIM-REQ.md` (§16 dopo R-050, §18 dopo R-051 e R-052, AC-04
dopo R-053), `docs/SIM-ECON.md`, `docs/SIM-STATE.md`.

Il file contiene due elenchi:

- **Contenuti (K-01…K-11).** Definiscono quale mondo si simula: goods, mestieri,
  recipes e simili. Per R-050 li approvi tu prima del piano che li usa.
- **Zone grigie (Z-01…Z-27).** Sono punti in cui la specifica è ambigua,
  contraddittoria o incompleta. Le decide il decider, salvo dove è indicato che
  tocca a te. Tra parentesi c'è il vecchio codice (B1…B12, C1…C7) usato nel
  report di oggi.

Per ogni voce trovi: **cosa è**, **dove serve** (sottosistema della fase 4,
nell'ordine di R-051: production → needs → market → labour and income →
demography → mobility → events), **dipende da**, **da essa dipende** e il
**consiglio**.

---

## Da dove partire

Alcune voci servono già al primo punto della fase 4, cioè generator e
production. Le altre servono solo quando arriva il loro sottosistema.

| Quando | Voci |
|---|---|
| Prima di scrivere il piano | K-01, K-02, K-03, K-04, K-05, K-06, K-07, K-09; Z-01, Z-02, Z-06, Z-07, Z-15, Z-26 |
| Prima del sottosistema needs | Z-09 (parte needs) |
| Prima del sottosistema market | K-08 (travel cost), Z-09, Z-10, Z-11, Z-12, Z-13, Z-14, Z-17 |
| Prima di labour and income | Z-16, Z-18 |
| Prima di demography | Z-20, Z-24 |
| Prima di mobility | Z-22, Z-25 |
| Prima di events | K-10, K-11, Z-23 |
| Trasversali, quando capita | Z-03, Z-04, Z-05, Z-19, Z-21, Z-27 |

**Consiglio generale: dividere la fase 4 in più piani in sequenza**, uno per
sottosistema o per gruppo di sottosistemi. Per R-050 i contenuti vanno
approvati prima del piano che li usa. Con un piano unico dovresti approvare
subito anche gli eventi (K-10) e i dungeon (K-11), che serviranno mesi dopo e
che a quel punto saranno decisi senza averli visti in un mondo che gira. La
divisione mantiene anche i piani corti, ed è per questo che esiste il
moment one.

---

## Parte 1: contenuti da approvare

### K-01 — Goods: lista e attributi

- **Cosa è.** FR-P-04 propone 7 goods, *"proposed, pending confirmation"*:
  Grain, Bread, Ore, Timber, Tools, Wool, Cloth, in tre filiere (Grain→Bread,
  Ore+Timber→Tools, Wool→Cloth). P-09 fissa il numero, 7, come *Decided*. Per
  ogni good FR-P-01 chiede unit weight, value reference, perishability, role e
  need tier.
- **Dove serve.** Dal primo punto: il generator scrive gli stock iniziali e la
  colonna `goods int[]` del cohort ha una cella per good.
- **Dipende da.** Z-07 (rapporto tra role e tier), K-11 (materiali dei
  dungeon, che sarebbero un ottavo good).
- **Da essa dipende.** K-02, K-03, K-04, K-05, K-06; P-10 (prezzo di
  riferimento); Z-09.
- **Consiglio.** Confermare i 7, con questi attributi:

| Good | Role | Tier | Perishability | Peso / valore |
|---|---|---|---|---|
| Grain | Input (vedi K-06) | — | media | pesante, economico |
| Bread | Subsistence | 0 | alta | pesante, economico |
| Ore | Input | — | nulla | molto pesante, economico |
| Timber | Input (anche per la costruzione) | — | bassa | pesante, economico |
| Tools | Durable | — | usura (P-53) | medio, caro |
| Wool | Input | — | bassa | leggero, medio |
| Cloth | vedi Z-07 | 1 | bassa | leggero, caro |

  I numeri (pesi, valori, perishability) sono parametri: entrano in §16 come
  Provisional (R-050). Tu approvi la lista, i ruoli e i tier.

### K-02 — Mestieri

- **Cosa è.** FR-J-01 dice che un mestiere è un record di dati: attività,
  building richiesto, produttività, settled o itinerant, wealth tendency,
  social rank. Una lista non esiste. SIM-STATE stima *"roughly 9 rows per
  settlement at Phase 4"*, e su 9 cohort si basa P-17.
- **Dove serve.** Dal primo punto che costruisce cohort: i cohort sono
  indicizzati per mestiere (FR-J-02) e R-037 scatta lì.
- **Dipende da.** K-01, K-03, Z-06 (chi assume chi).
- **Da essa dipende.** La tabella dei cohort, la larghezza dell'allocation
  vector (FR-J-03), la misura di P-17 e P-01, R-037.
- **Consiglio.** 10 mestieri dall'inizio della fase 4:
  - **7 produttori**, uno per good: contadino, fornaio, minatore, boscaiolo,
    fabbro, pastore, tessitore.
  - **Caravanner** (FR-J-04), che fornisce capacità di trasporto.
  - **Merchant stanziale** (FR-C-10), che compra al prezzo d'origine e
    rivende a destinazione.
  - **Bracciante**: non possiede nulla e vende lavoro. Senza questa figura
    FR-C-05 (*"hire labour from cohorts that own neither"*) non ha chi vende
    lavoro; vedi Z-06.

  Itinerant merchant (FR-J-06) e adventurer (FR-J-11) entrano con mobility ed
  events. Con 10–12 mestieri le righe per settlement superano le 9 stimate, e
  P-17 va rimisurato con il numero vero, come chiede già §19.

### K-03 — Recipes e varianti crude

- **Cosa è.** FR-P-02: input, output, lavoro, building. FR-P-20: rapporto con i
  durable (richiesto, migliorato da, indipendente). FR-P-21: ogni recipe che
  richiede un durable ha una variante crude (P-52).
- **Dove serve.** Sottosistema production.
- **Dipende da.** K-01, K-04, K-05.
- **Da essa dipende.** Production, domanda di Tools (FR-P-23), allocation
  vector.
- **Consiglio.**

| Recipe | Input → output | Building | Durable | Crude |
|---|---|---|---|---|
| Grano | terra + lavoro → Grain (è una crop, K-04) | — | migliorato da Tools | — |
| Pane | Grain → Bread | Mulino | indipendente | sì, senza mulino (macina a mano) |
| Minerale | lavoro → Ore | Miniera (sito ore) | richiede Tools | sì |
| Legname | lavoro → Timber (sito forest) | — | migliorato da Tools | — |
| Attrezzi | Ore + Timber → Tools | Forgia | richiede Tools | sì |
| Lana | terra + lavoro → Wool | — | indipendente | — |
| Panno | Wool → Cloth | Telaio | migliorato da Tools | — |

  La variante crude del pane evita che un villaggio con grano e senza mulino
  muoia di fame (vedi K-06). A rigore FR-P-21 chiede la variante crude solo per
  le recipe che *richiedono* un durable, e il mulino è un building, non un
  durable: la variante del pane è una scelta di contenuto, non un obbligo.

### K-04 — Crops

- **Cosa è.** FR-P-10: una crop è un record con data di semina, durata di
  maturazione, data di raccolto e resa per unità di terra e di lavoro.
- **Dove serve.** Production: passi datati di semina e raccolto.
- **Dipende da.** K-01, K-08 (il clima sposta le date).
- **Da essa dipende.** L'allocation adjustment avviene alla semina
  (DEC-047): con una sola crop scatta una volta l'anno. Vedi Z-05.
- **Consiglio.** Nella fase 4 una sola crop, il grano. La lana è una
  produzione continua modulata dal clima (FR-P-08 cita *livestock growth*). Una
  seconda crop si aggiunge quando serve differenziare le campagne.

### K-05 — Building types e site attributes

- **Cosa è.** FR-P-17: i building sono stock dei cohort, costruiti con timber e
  lavoro. FR-P-19: un building può richiedere un attributo di sito (ore,
  water, forest, dungeon in SIM-STATE); senza attributi di sito i settlement
  convergono sulla stessa economia e il commercio sparisce.
- **Dove serve.** Production (capacità nella fase Labour) e il generator, che
  distribuisce i siti.
- **Dipende da.** K-03.
- **Da essa dipende.** Capacità di lavoro (fase 4 di SIM-ECON), construction,
  endowment iniziale (FR-P-22, P-54), vantaggio comparato tra settlement.
- **Consiglio.** Quattro building:
  - Mulino, che richiede il sito water;
  - Miniera, che richiede il sito ore;
  - Forgia;
  - Telaio.

  Il legname richiede il sito forest ma nessun building. La capacità di ogni
  building (posti di lavoro) è un parametro Provisional.

### K-06 — Tabella dei bisogni (struttura)

- **Cosa è.** SIM-ECON definisce il bisogno *"per good per person per day, by
  tier"* e ammette: *"The tier table has a shape and no numbers"*. Non prevede
  sostituzioni tra goods.
- **Dove serve.** Needs; ma anche il generator, che deve creare stock iniziali
  sufficienti.
- **Dipende da.** K-01, Z-07.
- **Da essa dipende.** Hunger (mortalità in demography), domanda di mercato,
  fragility.
- **Consiglio.**
  - Tier 0: Bread soltanto.
  - Tier 1: Cloth.
  - Tier 2: nessun good nella lista base. Il ramo del codice per il tier 2 si
    prova con un good di test nella suite.
  - Il grano non si mangia direttamente: la variante crude del pane (K-03)
    copre il caso senza mulino.

  Le quantità sono parametri Provisional (R-050). Tu approvi quale good sta in
  quale tier.

### K-07 — Fasce d'età

- **Cosa è.** SIM-STATE Open 1: *"Confirm the age band count for cohorts"*. È
  un campo del cohort (`ageDistribution int[]`) e ne moltiplica la larghezza.
- **Dove serve.** Il primo punto che costruisce i cohort (production), anche se
  la demografia arriva dopo.
- **Dipende da.** Niente.
- **Da essa dipende.** Offerta di lavoro, demography (FR-D-03 *"per age
  band"*), dimensione dello stato (NFR-07).
- **Consiglio.** Tre fasce: bambini, età da lavoro, anziani. Solo la fascia da
  lavoro offre lavoro. Si può allargare dopo con una migrazione di formato,
  come prevede R-029.

### K-08 — Curve climatiche

- **Cosa è.** FR-P-08 prevede due curve settimanali da 52 voci, `cold` e
  `warm`, che modulano travel cost, mortality, livestock growth, timber yield e
  construction labour. Il testo non dice se le due curve valgono per tutti e
  cinque i modificatori o se ogni modificatore ne ha due proprie. P-14 dice
  *"52 weekly entries, 2 curves"*, che suggerisce la prima lettura.
- **Dove serve.** Production (legname, lana, costruzione), demography
  (mortalità), market e mobility (travel cost).
- **Dipende da.** Niente.
- **Da essa dipende.** Date delle crop (FR-P-11), rese stagionali.
- **Consiglio.** Una coppia di curve per modificatore, cioè 5 × 2 × 52 voci, in
  una tabella di dati: un'unica coppia non può rendere un inverno che ferma i
  cantieri ma non fa morire di più. Si scrivono solo quelle che il
  sottosistema in corso usa. Preparo io curve lisce di partenza e tu le
  approvi. Il chiarimento va in FR-P-08 e P-14 con una ruling.

### K-09 — Contenuto del generator

- **Cosa è.** FR-G-01 dice cosa il generator produce (*"terrain, settlements,
  trade graph, kingdoms, initial populations and stocks"*), ma non con quali
  regole. Sono già decisi P-02 (2.000 settlement), P-03, P-04, P-12 (mix) e
  FR-M-04 (topologia). Mancano:
  - quanti regni e come si distribuiscono;
  - quali terreni ci sono e quale costo di trasporto ha ciascuno;
  - come si distribuiscono i siti;
  - popolazione iniziale per tipo di settlement;
  - stock e currency iniziali;
  - dove sta il buried stock (FR-J-14, P-38);
  - a quale regno appartiene ogni settlement.
- **Dove serve.** Il primo punto della fase 4.
- **Dipende da.** K-01, K-02, K-05, K-07; Z-01, Z-06.
- **Da essa dipende.** Tutto il resto.
- **Consiglio.**
  - Numero di regni come P-number Provisional (indicativamente 6–10).
  - Cinque terreni: pianura, collina, foresta, montagna, acqua. Ognuno ha un
    costo di trasporto.
  - Siti assegnati in base al terreno: forest in foresta, ore in collina e
    montagna, water lungo i corsi d'acqua.
  - Popolazioni e stock iniziali come parametri Provisional, tarati in modo
    che al tick 0 ogni settlement copra il proprio tier 0 per una stagione.

  Le regole qualitative le approvi tu, i numeri vanno in §16.

### K-10 — Classi di eventi

- **Cosa è.** FR-E-01 prevede eventi esogeni (flood, drought, earthquake,
  frost), endogeni (famine, revolt, insolvency, village abandonment,
  succession war, kingdom collapse) e contagiosi (plague). Per ogni classe
  servono trigger o frequenza, bersagli ammessi ed effetto attraverso
  l'economia (FR-E-06). SIM-STATE li elenca tra i dati statici.
- **Dove serve.** Il sottosistema events, l'ultimo.
- **Dipende da.** Z-02 (stato S/I/R, fragility), Z-03 (range di legitimacy).
- **Da essa dipende.** AC-11, AC-04 (infection prevalence, legitimacy).
- **Consiglio.** Rimandarne l'approvazione al piano di events (vedi il
  consiglio generale). Per ogni classe ti preparo una scheda con trigger,
  bersagli ed effetto.

### K-11 — Contratti, dungeon e materiali rari

- **Cosa è.**
  - FR-J-16 prevede tre tipi di contratto: escort, bounty, route clearing.
  - FR-K-11 dice che il dungeon ha uno stock rigenerante di *"rare
    materials"*, e FR-K-15 ammette recipes con materiali che vengono solo dai
    dungeon. Un materiale raro però sarebbe un ottavo good, mentre P-09 è
    *Decided* a 7.
  - FR-K-16 prevede recipes recuperabili nei dungeon.
- **Dove serve.** Events.
- **Dipende da.** K-01, K-03.
- **Da essa dipende.** Il freno del loop povertà–banditismo (FR-M-14), gli
  adventurer (FR-J-11).
- **Consiglio.** Nella fase 4 i tre contratti di FR-J-16 e nessun materiale
  esclusivo dei dungeon (FR-K-15 dice *"may"*). Lo stock di un dungeon è Ore.
  I materiali rari entrano con un ampliamento dei contenuti, che riapre P-09
  con una tua decisione.

---

## Parte 2: zone grigie

### Primo punto e trasversali

#### Z-01 — A quale tabella appartiene un `EntityId` (B4)

- **Cosa è.** SIM-STATE Open 5 e R-025: un `EntityId` è indice più generation,
  ma non dice di quale tabella. DEC-083 dà per scontato che entità diverse
  abbiano chiavi diverse, e questo vale tra tabelle solo se gli indici
  condividono uno spazio unico.
- **Dove serve.** Il primo punto che crea i settlement. Per R-025 deve
  risolverlo prima di chiudersi.
- **Dipende da.** Niente.
- **Da essa dipende.** Generator, chronicle (`entities`, `location`), canali
  dell'RNG (DEC-081, DEC-083), ogni colonna di tipo `id`.
- **Consiglio.** Indice per tabella, con il tipo di tabella nei bit alti della
  chiave impacchettata di DEC-081. Le righe restano dense, la chiave distingue
  settlement 7 da agent 7, e il chronicle sa a quale tabella punta. Si fa con
  una ruling che modifica DEC-081 e SIM-STATE.

#### Z-02 — Stato implicito che SIM-STATE non elenca (B3)

- **Cosa è.** SIM-STATE §Rule dice che ciò che non è elencato deve essere
  derivabile, altrimenti è un buco di determinismo. Mancano:
  - il deficit di fame (SIM-ECON §Demand);
  - i remainder carry per cohort e good (SIM-ECON §Spoilage);
  - l'impegno di semina;
  - il ruolo e il dwell della migrazione (FR-D-09, FR-D-11);
  - S/I/R (FR-E-05);
  - il discontent (tier 1 non soddisfatto);
  - le fragility metrics (FR-E-03);
  - il contatore del tetto annuo di FR-P-15;
  - le ruins (FR-W-10);
  - l'appartenenza settlement→regno;
  - i migranti in transito;
  - i goods in transito aggregati (vedi Z-12).
- **Dove serve.** Ogni sottosistema, al momento in cui aggiunge il campo.
- **Dipende da.** K-01…K-07.
- **Da essa dipende.** Hash, save (R-029, R-041), AC-01, AC-02.
- **Consiglio.** Una ruling sola che aggiunge subito a SIM-STATE tutti i campi
  dell'elenco, ognuno con tipo, range e il sottosistema che lo introduce. È
  meglio di una SPEC-QUESTIONS per campo, che fermerebbe ogni punto.

#### Z-03 — Range dichiarati e A-01 (C2)

- **Cosa è.** A-11 chiede che ogni valore fixed-point stia *"inside its
  declared range"*, ma i range sono dichiarati solo per `climateIndex` e per
  l'allocation vector. Mancano danger (A-12), legitimacy, wealthSpread e
  taxRate. A-01 confronta la somma dei cohort con la popolazione del
  settlement, che però non è un campo: se la popolazione si calcola come somma,
  A-01 è sempre vero.
- **Dove serve.** Ogni campo fixed che viene aggiunto. AC-01 è il gate.
- **Dipende da.** Z-02.
- **Da essa dipende.** AC-01, i limiti provvisori di AC-04 (R-053).
- **Consiglio.** Ogni campo fixed dichiara il suo range in SIM-STATE nella
  ruling che lo aggiunge (Z-02). Per A-01: la popolazione resta derivata e A-01
  si sostituisce con un controllo che possa fallire, cioè che ogni persona che
  entra o esce da un settlement (nascite, morti, migranti partiti e arrivati)
  compaia nei count dei cohort. Togliere A-01 indebolirebbe la metà
  popolazione di AC-01: tocca a te (riserva 2).

#### Z-04 — Remainder accumulator mancanti (C1)

- **Cosa è.** SIM-ECON elenca *"durable wear, building decay, interest on small
  debts, transit loss"* e dice che la regola è generale. Restano senza
  accumulator dichiarato: nascite, morti, migrazione, contagio, tasse,
  commissione del cambiavalute, flusso commerciale (k × differenza troncata) e
  decadimento della fame. Non è detto dove finisce il carry quando una riga si
  svuota o cambia.
- **Dove serve.** Ogni sottosistema.
- **Dipende da.** Z-02 (il carry è stato).
- **Da essa dipende.** AC-04 (gli stock piccoli che non cambiano mai), AC-01.
- **Consiglio.** Una ruling che estende la lista di SIM-ECON a "ogni tasso
  applicato a un intero" e stabilisce che il carry resta alla riga e si
  azzera quando il count della riga arriva a zero. Il carry è una frazione e
  non una quantità conservata, quindi azzerarlo non viola nessun invariante.

#### Z-05 — Semantica dell'allocation vector (B9)

- **Cosa è.**
  - FR-P-14 dice che il vettore copre terra e lavoro; FR-J-03 che le quote dei
    mestieri si muovono con lo stesso vettore.
  - Non è detto come una quota fixed-point diventa un numero intero di
    persone, in quale wealth band finisce chi cambia mestiere, cosa succede ai
    suoi soldi e beni, né come A-05 resta esattamente uguale a uno.
  - Sulla cadenza c'è un contrasto: FR-P-15 dice *"Non-agricultural activities
    adjust … at settlement cadence"*, mentre il titolo di DEC-047 dice
    *"once per sowing"*.
- **Dove serve.** Production.
- **Dipende da.** K-02, K-04, Z-20.
- **Da essa dipende.** Production, labour, AC-01 (popolazione), A-05.
- **Consiglio.**
  - Conversione in interi con il metodo del resto maggiore, a parità in ordine
    di id.
  - Chi cambia mestiere resta nella sua wealth band e porta con sé currency e
    goods pro capite.
  - A-05 si tiene esatto mettendo l'arrotondamento sull'ultima voce.
  - Cadenza: la terra agricola si sposta alla semina (DEC-047), le attività non
    agricole a cadenza di settlement (FR-P-15). Va verificato che il testo di
    DEC-047 sia compatibile.

#### Z-06 — Proprietà della terra e chi assume (B5)

- **Cosa è.** FR-C-05: *"Cohorts owning land or buildings hire labour from
  cohorts that own neither"*. FR-C-29: *"Land is not owned by cohorts"*.
  SIM-ECON dice che il raccolto va *"to the growing cohort's holdings"*, mentre
  FR-P-10 dice *"to settlement stock"* (che per FR-C-02 è la somma dei cohort).
- **Dove serve.** Production (raccolto) e labour and income.
- **Dipende da.** K-02 (bracciante).
- **Da essa dipende.** Labour market, salari, distribuzione della ricchezza.
- **Consiglio.** Riscrivere FR-C-05 come *"cohorts working land or owning
  buildings"*. La terra resta del settlement (FR-C-29). Il mestiere che la
  lavora (contadino, pastore) possiede l'output e assume braccianti. Il
  raccolto va al cohort del contadino, e FR-P-10 si legge attraverso FR-C-02.

#### Z-07 — Role e tier dei goods non coincidono (nuova)

- **Cosa è.** FR-P-03 ha quattro role: Subsistence (non rinviabile), Input,
  Durable, Luxury (comprato solo con quello che avanza). SIM-ECON ha tre tier:
  0 non rinviabile, 1 rinviabile, 2 comprato con una quota di quello che
  avanza. Subsistence corrisponde al tier 0 e Luxury al tier 2, ma **al tier 1
  non corrisponde nessun role**. Un good di base rinviabile come il Cloth non
  ha un role adatto.
- **Dove serve.** Needs, e prima ancora K-01.
- **Dipende da.** Niente.
- **Da essa dipende.** K-01, K-06, domanda di mercato.
- **Consiglio.** Role e tier sono due assi indipendenti. Il role dice come il
  good si comporta nella produzione e nel tempo (consumato, input, durable); il
  tier dice quanto è urgente il bisogno. Si fondono Subsistence e Luxury in un
  role "Consumable", e la rinviabilità la decide il tier. Si fa con una ruling
  su FR-P-03.

#### Z-26 — Ruling dovute che cadono sul primo punto

- **Cosa è.** Non è una zona grigia della specifica ma un vincolo di
  dimensionamento. Il primo punto che aggiunge una tabella porta insieme
  R-041, R-043, R-029, R-034/R-036 (ri-pin), R-025 (Z-01) e, se crea cohort,
  R-037. In più R-046 (prima esecuzione degli agenti) e R-050 (P-number).
- **Consiglio.** Un primo punto più stretto possibile: generator dei soli
  settlement e regni, senza cohort. Così R-037 cade sul secondo punto e il
  primo resta leggibile.

### Needs e market

#### Z-09 — Chiave del razionamento e ripartizione (B10)

- **Cosa è.** *"Descending ability to pay"* (SIM-ECON) non è definita.
  `wealthSpread` *"drives the closed-form affordability fraction"*
  (SIM-STATE), ma quella frazione non è scritta da nessuna parte. Mancano
  anche la ripartizione del pagamento tra più venditori e il budget tra più
  goods dello stesso tier.
- **Dove serve.** Needs e market.
- **Dipende da.** K-06, Z-20.
- **Da essa dipende.** Fame, prezzi, AC-04.
- **Consiglio.**
  - Ability to pay = currency pro capite del cohort, calcolata una volta per
    update e ordinata (SIM-ECON lo chiede già, per i costi).
  - La frazione di affordability usa una distribuzione della ricchezza dentro
    il cohort con larghezza `wealthSpread`. La formula chiusa la scrive il
    decider in SIM-ECON.
  - Venditori pagati in proporzione all'offerta, con il resto assegnato in
    ordine di id.
  - Budget di tier ripartito in proporzione al valore del bisogno non
    soddisfatto.

#### Z-10 — Il price è memorizzato o calcolato (B6)

- **Cosa è.** FR-W-01 mette i *prices* nello stato. FR-C-04.6 li ricalcola alla
  fine del clearing *"for the next update"*. SIM-ECON li calcola nella fase 2
  dallo stock corrente. SIM-STATE non ha un campo price. Le due letture danno
  numeri diversi, perché tra la fine del clearing e la fase 2 successiva lo
  stock cambia con spoilage, export e arrivals.
- **Dove serve.** Market.
- **Dipende da.** Niente.
- **Da essa dipende.** Export, knowledge table, AC-04 (prezzi).
- **Consiglio.** Il price non è stato: si calcola nella fase 2 dallo stock
  corrente, come dice SIM-ECON, e vale per tutto l'update. Ciò che i vicini
  vedono è la voce della knowledge table, che invece è stato. FR-C-04.6 e
  FR-W-01 vanno allineati con una ruling.

#### Z-11 — Errore della knowledge table (B7, in parte risolta)

- **Cosa è.** L'export usa il prezzo di destinazione, ma per FR-I-06a nessuna
  decisione legge un prezzo remoto vero. R-052 ha già messo FR-I-05…06a nel
  market. Resta P-36: *"small, non-zero"* non è un valore.
- **Dove serve.** Market (export).
- **Dipende da.** Z-10.
- **Da essa dipende.** AC-33, flussi commerciali.
- **Consiglio.** P-36 diventa Provisional, con un valore in punti base, e la
  forma dell'errore (FR-I-06) si fissa quando la si costruisce.

#### Z-12 — Invariante dei goods e goods in transito (B2)

- **Cosa è.** A-04: *"Goods leaving a settlement arrive, spoil, or are lost in
  transit, with the loss recorded"*. Non c'è un campo che registri le perdite
  né un'equazione. Il tempo di viaggio del flusso aggregato non è definito, e
  il significato di `flowAccumulator` è aperto (SIM-STATE Open 2). I sampled
  transients non sono memorizzati.
- **Dove serve.** Market (export e arrivals).
- **Dipende da.** Z-02, Z-13.
- **Da essa dipende.** La metà goods di AC-01.
- **Consiglio.**
  - Il flusso in transito è stato: per link, per good e per tick di arrivo.
    Il tempo di viaggio si ricava da `transportCost`.
  - A-04 diventa un'equazione: per ogni good, variazione dello stock mondiale
    = produzione − consumo − spoilage − perdite in transito, con le perdite in
    un contatore per link.
  - `flowAccumulator` resta solo come media mobile del volume (FR-M-10).

#### Z-13 — "Per tick" in sistemi a cadenza settimanale (B11)

- **Cosa è.** FR-D-04 (*"Migration per tick is capped"*), FR-D-10, FR-I-02 e
  SIM-ECON (*"landing this tick"*) parlano di tick, ma questi sistemi girano
  ogni 7 tick. Letto alla lettera, ciò che atterra nei giorni senza update non
  viene mai raccolto.
- **Dove serve.** Market (arrivals), mobility, events (information).
- **Dipende da.** Niente.
- **Da essa dipende.** Z-12, AC-04 (migration flow per tick).
- **Consiglio.** Per i sistemi a cadenza di settlement "per tick" significa
  "per update". La fase 1 raccoglie tutto ciò che è arrivato dall'update
  precedente. AC-04 misura il migration flow per tick dividendo per 7. È un
  chiarimento di testo, lo fa il decider.

#### Z-14 — Gli attori del commercio (B12)

- **Cosa è.** FR-C-10: *"A merchant cohort in the destination settlement pays
  the origin price"*. Non è detto quale mestiere sia, chi paga il salario dei
  caravanner (FR-J-05), dalle holdings di quale cohort partono i goods, né in
  quale cohort entrano i migranti.
- **Dove serve.** Market, mobility.
- **Dipende da.** K-02, Z-15.
- **Da essa dipende.** Conservazione della currency, salari dei caravanner.
- **Consiglio.**
  - Il merchant stanziale (K-02) della destinazione compra.
  - I goods partono dai cohort con eccedenza, in proporzione all'eccedenza.
  - Il merchant paga il caravanner per unità trasportata.
  - I migranti entrano nello stesso mestiere se esiste nella destinazione,
    altrimenti nei braccianti.

#### Z-15 — L'invariante della currency non è univoco (B1)

- **Cosa è.**
  - A-02 conta *"transient contents, plus buried stock"*; AC-20 non conta il
    buried stock; AC-35 e FR-J-15 lo trattano come esterno alla circolazione.
  - L'escrow delle gilde (AC-54) non compare in A-02.
  - A-02 non conta la currency degli agent, mentre AC-25 dice *"every
    holder's coins"*.
  - Non è definito quando passa di mano la currency del commercio.
- **Dove serve.** Dal primo punto, perché il generator piazza currency e
  buried stock.
- **Dipende da.** Niente.
- **Da essa dipende.** La metà currency di AC-01, AC-20, AC-25, AC-35, AC-54,
  Z-14, Z-16.
- **Consiglio.** A-02 è la forma completa: cohort + tesori + contenuto dei
  transient + buried stock + escrow + agent (da fase 5) = `currencyTotal`.
  AC-20 e AC-35 vanno allineati con una ruling che non indebolisce nessuno dei
  due. La currency del commercio passa di mano alla partenza: il merchant di
  destinazione paga i cohort d'origine, e i goods in transito sono suoi.

#### Z-17 — Il salario prima di labour and income (nuova, da R-051)

- **Cosa è.** R-051 mette il market prima di labour and income, ma la fase 2 di
  SIM-ECON calcola il salario insieme ai prezzi, e la domanda di mercato ne
  dipende.
- **Dove serve.** Market.
- **Dipende da.** K-09.
- **Da essa dipende.** Il gate del market.
- **Consiglio.** Finché labour non c'è, il salario è quello scritto dal
  generator e non cambia. È stato generato, non uno stub, nello spirito di
  R-051. Va nel register del punto del market.

### Labour and income

#### Z-16 — Taxation e spesa del regno (B8)

- **Cosa è.**
  - Base imponibile: FR-C-07 dice *"from cohort currency at the rate"*, P-20
    dice *"10% of surplus"*, FR-C-09 parla di *"surplus after needs"*.
  - Collocazione: SIM-ECON la mette nella fase 5 o 12, FR-T-04 alla cadenza
    del regno.
  - Spesa del regno: *"not yet placed"* (SIM-ECON, voce aperta 4), ma
    FR-C-08 vieta la tassazione senza spesa.
  - Il convoy richiede di spezzare la currency nei quattro tagli, e nessuna
    regola dice come.
- **Dove serve.** Labour and income.
- **Dipende da.** Z-15, K-09 (regni).
- **Da essa dipende.** Tax pressure (FR-C-09), fragility, convoy (FR-C-19), il
  freno garrison di FR-M-14.
- **Consiglio.**
  - Base: la currency del cohort sopra una riserva pari al suo bisogno di un
    anno; così "surplus" ha una misura.
  - Raccolta a cadenza di regno, nella fase 12 dell'update del settlement che
    cade in quel tick.
  - Spesa a cadenza di regno, come salari (garrison, lavori) pagati ai cohort
    dei settlement del regno, in proporzione alla popolazione.
  - Convoy con algoritmo goloso dal taglio più grande, sui conteggi del tesoro.

#### Z-18 — Conio e metallo (nuova)

- **Cosa è.** FR-C-21: il conio consuma lo stock di metallo del regno. Non è
  detto da dove venga il metallo (è Ore?) né se nella fase 4 si conia.
- **Dove serve.** Labour and income.
- **Dipende da.** K-01.
- **Da essa dipende.** A-03, AC-20.
- **Consiglio.** Nessun conio nella fase 4: `currencyTotal` resta costante dal
  generator, e `metalStock` è inizializzato e mai consumato. Il conio entra con
  una ruling quando serve, e lì si decide se il metallo è Ore.

### Demography

#### Z-20 — Le tre wealth band (C6)

- **Cosa è.** FR-W-09 e DEC-054: tre band in CI per rompere il codice che
  presume una riga per mestiere. Non è definito come la popolazione iniziale si
  divide tra le band né cosa sposta qualcuno da una band all'altra. Le sequenze
  pinnate di R-034 e R-036 cambiano con la configurazione: serve un pin per
  configurazione?
- **Dove serve.** Il primo punto con i cohort (R-037).
- **Dipende da.** K-02 (wealth tendency).
- **Da essa dipende.** Z-05, Z-09, AC-28, R-037.
- **Consiglio.**
  - Il generator divide in base alla wealth tendency del mestiere.
  - Nella fase 4 nessun movimento tra band, salvo che con il cambio di
    mestiere (Z-05).
  - Un pin per configurazione, perché sono mondi diversi.

#### Z-24 — Verifiche senza criterio (C5)

- **Cosa è.** SIM-LOOPS:143 chiede di verificare in fase 4 *"that the ceiling
  binds before starvation does"*, senza un AC. AC-47 parla di *"a declared
  number of ticks"*, che non è dichiarato. AC-48 parla di *"a declared
  density"*, anch'essa non dichiarata.
- **Dove serve.** Demography e mobility.
- **Dipende da.** R-050.
- **Da essa dipende.** Le chiusure di demography e mobility.
- **Consiglio.** I due valori diventano P-number Provisional. La verifica di
  SIM-LOOPS diventa parte del criterio del punto che chiude mobility.

### Mobility ed events

#### Z-22 — Gli esiti degli adventurer senza host (nuova, importante)

- **Cosa è.** FR-J-17: *"The host resolves that and reports the outcome through
  the command queue."* Nelle run headless (AC-04, sweep, CI) non c'è un host,
  quindi i contratti non si completano mai, il danger non scende per quella
  via, e il freno di FR-M-14 (*"Neither is optional"*) resta solo alla spesa
  per i presidi.
- **Dove serve.** Mobility ed events.
- **Dipende da.** Z-27.
- **Da essa dipende.** AC-04, il loop povertà–banditismo, R-042 e R-036 (il
  primo command kind).
- **Consiglio.** In assenza di host il simulatore risolve il viaggio con una
  draw (NFR-03) su probabilità che dipendono da rank e danger; un host presente
  lo sostituisce. Va scritto in FR-J-17 con una ruling.

#### Z-23 — Retention del chronicle (C3)

- **Cosa è.** SIM-STATE Open 3: in 60 anni le voci si accumulano e niente le
  scarta. Pesa su NFR-07, sul costo dell'hash (R-043) e su AC-08.
- **Dove serve.** Events (che scrive la maggior parte delle voci) e ogni run
  lunga di AC-04.
- **Dipende da.** Niente.
- **Da essa dipende.** NFR-07, R-043, DEC-088 (righe liberate dentro il
  count).
- **Consiglio.** Retention per importanza ed età: le voci a bassa importanza
  scadono dopo N anni (P-number); le altre restano. La compattazione è in
  ordine di id, come chiede DEC-085. Attenzione: una voce che è causa di
  un'altra non può scadere prima di lei, altrimenti AC-11 perde la catena.

#### Z-25 — Limiti strutturali per AC-04 (nuova, da R-053)

- **Cosa è.** R-053 ricava i limiti provvisori dalla specifica, ma non ha
  verificato che esistano per tutte le variabili.
- **Dove serve.** La chiusura di ogni sottosistema.
- **Dipende da.** Z-03, Z-13.
- **Da essa dipende.** Il gate della fase 4.
- **Consiglio.**
  - Migration flow: limitato dai tetti di FR-D-04 e FR-D-10.
  - Urban share: compreso tra 0 e 1.
  - Infection prevalence: non supera la popolazione.
  - Legitimacy: il range dichiarato in Z-03.

  Gli ultimi tre sono limiti deboli: AC-04 su quelle variabili prova poco
  finché la fase 6 non mette i limiti osservati. Va detto nel register.

### Varie

#### Z-19 — Misura di NFR-05 (C4)

- **Cosa è.** NFR-05 va misurato *"at target scale"*, ma DEC-041 include i 300
  hot agent, che arrivano in fase 5. Non è detto da quando la soglia viene
  imposta in CI.
- **Dove serve.** La chiusura della fase 4 (P-17, P-01).
- **Consiglio.** In fase 4 si misura senza agent e si registra la misura. In
  CI nessuna soglia, perché i runner condivisi la farebbero fallire per il
  carico (come in D-101). La soglia piena si verifica in fase 5.

#### Z-21 — Difetti di testo minori

- SIM-ECON:58: *"phase 5 or phase 12"* indica le fasi dell'update, mentre
  altrove "Phase 4" è una fase di consegna. Basta scriverle diversamente.
- SIM-LOOPS:140: *"Met for R1, R2, R4, R5, R6, R7"* omette R8–R11.
- La tabella delle fasi di SIM-ECON è spezzata: le righe 8–12 stanno sotto il
  paragrafo sulla fase 7, senza intestazione.

**Consiglio.** Una ruling di pulizia, che non cambia nessun contenuto.

#### Z-27 — Command kind nella fase 4 (C7)

- **Cosa è.** R-052 mette §13 in fase 7, quindi nella fase 4 non c'è nessun
  command kind. Restano sospesi R-042, R-036 (pin con il comando vero) e le
  domande di D-106.
- **Dove serve.** Nessun punto della fase 4, se Z-22 si risolve senza comandi.
- **Consiglio.** Confermare che nella fase 4 non c'è nessun command kind. Le
  ruling sospese restano dovute alla fase 7. Va confermato da te, perché
  tocca il perimetro.

---

## Cosa non è stato verificato

- I requisiti delle sezioni 5–12 sono stati letti dalla prima riga di ciascuno.
  §9–§12 non sono stati letti per intero.
- Il testo completo di DEC-047 (Z-05) non è stato letto, solo il titolo.
- SIM-OBS e SIM-LOOPS non sono stati letti per intero. Le voci che li toccano
  (Z-24, Z-25) si basano sulle righe citate.
- I consigli su K-01…K-11 sono proposte di contenuto, non derivano dalla
  specifica. Sono il punto di partenza per la tua approvazione.
- Z-08 non esiste: la numerazione è saltata durante la stesura e non è stata
  ricompattata, per non cambiare i riferimenti incrociati.
