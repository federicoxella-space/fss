# Proposta di decisioni — Povertà, cibo e raccolto

**Documento:** PROPOSAL-POVERTY-SEED
**Stato:** Revisione 10. Il decider ha risolto O-5, O-6, O-15, PD-15 e PD-16 (R-054 … R-058); restano al decider O-7, O-9, O-12 e O-14 (PD-17). Le decisioni dell'utente sono marcate Deciso; tutto ciò che resta è affidato al decider, salvo il contenuto statico, che per R-050 resta all'utente. Nulla di quanto segue è ancora nella spec.
**Data:** 2026-10-04
**Base:** `main` @ `54f3180`, il cui SIM-REQ coincide con quello caricato il 2026-10-02. Riferimenti incrociati a `2026-10-02-fase-4-da-decidere.md` (K-xx, Z-xx).
**Lingua:** motivazioni in italiano; i testi da inserire nella spec sono in inglese, come il corpus, pronti da trascrivere.

**Revisioni.**
- *2–6:* conferme puntuali; indigenti e braccianti come righe distinte; curve di ricchezza; fase 8a; riallineamento in fase 12.
- *7:* recepisce la revisione dell'implementatore (istruzioni non eseguibili e conflitti di contenuto) e i sei punti confermati: chi assume (FR-C-05, Z-06), riserva sulla terra assegnata, niente cancello del credito, freno del nuovo anello lavoro–povertà, curva di ricchezza anche nel razionamento (Z-09), wealth tendency (Z-20). Introduce le classi di beni con grado di lavorazione e valore nutrizionale (PD-14), e propone la somma dei bisogni (PD-15) e il piatto secondo la ricchezza (PD-16). La produzione domestica (orto, animali, raccolta) è lasciata al decider (PD-17).
- *8:* l'utente affida al decider tutto ciò che resta: PD-15, PD-16, PD-17, la forma dei ruoli di FR-P-03, la regola di conservazione nelle ricette e i punti aperti non di contenuto. Il contenuto statico resta all'utente (R-050).
- *9:* recepisce la seconda revisione dell'implementatore. Il tier esce dal record del bene anche in FR-P-01, DEC-015, AC-56 e DEC-080; AC-56 protegge i bisogni, non i beni (decisione dell'utente, riserva 2 di R-001). Il grado di lavorazione è uno per bene. AC-09 classifica i consumabili per il tier del bisogno più urgente che soddisfano, con una riga nuova per il tier 1 (decisione dell'utente, riserva 2). La farina entra tra i beni di base e P-09 passa a 8 (decisione dell'utente). Il tier 0 ha un solo bisogno, sul cibo, controllato al caricamento (FR-P-03b); il deficit di fame è in unità di nutrimento. La riserva di semina si calcola sull'allocazione corrente, non su quella della semina successiva. Il deficit di fame è per membro, e quando dei membri cambiano riga la riga che li riceve ne prende la media pesata. FR-C-04: i passi 5 e 6 restano. O-5 e O-6 si decidono nello stesso lotto, e la proposta entra in `docs/` solo con B12b scritto. Minori: posizione del paragrafo di X-8, A-16 sommato anche sulle colture, FR-P-03b sotto Needs in §18, aggiunta a OBS-3 autorizzata dall'utente.
- *10:* il decider risolve O-5, O-6, O-15, PD-15 e PD-16 (R-054 … R-058) e ne deposita qui il testo. B12b è scritto. La proposta è trascritta in `docs/` da R-059, con le differenze di forma che R-059 elenca; da qui `docs/` è l'autorità e questo documento è la storia della decisione.

---

## Di cosa tratta

Il documento raccoglie le decisioni prese in sessione di design su come la povertà, il cibo e la carestia esistono nel simulatore:

1. **Chi è povero.** Una riga di indigenti in cui si entra per impoverimento e da cui si esce riprendendosi; una curva per mestiere dice quanti membri di una coorte stanno sotto una soglia.
2. **Chi lavora a giornata.** Il bracciante è un mestiere scelto per il salario, che segue i picchi di semina e raccolto.
3. **Il cibo.** I beni appartengono a classi estendibili, hanno un grado di lavorazione e un valore nutrizionale; i bisogni si definiscono sulle classi, non sui singoli beni.
4. **Il seme.** La semina consuma un bene di semina; se è cibo, la riserva è protetta finché la fame non diventa disperazione.
5. **Il lavoro agricolo e il raccolto in campo.** Il lavoro limita semina e mietitura, il raccolto dura una finestra, la coltura ha una condizione, il campo senza coorte diventa di chi lo miete.

Il punto di partenza è che una sola fascia di ricchezza non esprime "chi ha meno" dentro lo stesso mestiere (OBS-3), e che nella spec attuale il grano della semina nasce dal nulla (FR-P-10), quindi una carestia non può durare più di un anno.

## Come leggerlo

Ogni decisione ha uno **stato**:

- **Deciso**: scelto o confermato esplicitamente in sessione.
- **Al decider**: scelta affidata al decider. Il documento ne dà l'analisi e, dove c'è, il testo suggerito, marcato **[DECIDER]**: il decider lo adotta, lo modifica o lo scarta.

Le decisioni sono numerate PD-1 … PD-17. I numeri DEC suggeriti partono da DEC-090, i requisiti nuovi da FR-J-19 e FR-P-03a, i parametri da P-66, gli invarianti da A-16. I punti marcati **[OPEN]** dipendono da questioni aperte, elencate in fondo.

**Rapporto con il file del decider.** Questa proposta chiude Z-06, Z-07 e la parte di Z-09 sulla frazione di accessibilità, cambia il consiglio di K-06 (il grano diventa cibo di grado 0) e di K-01 (la lista dei beni cresce), ed è coerente con Z-04, Z-05 e Z-20.

---

## Decisioni

### PD-1 — La povertà è una riga: gli indigenti

**Stato:** Deciso.

Ogni insediamento ha una riga di **indigenti** (*destitute*): un record di mestiere di servizio, senza edificio, fuori dal vettore di allocazione, che vende lavoro sotto FR-C-05. Vive sotto la chiave `(mestiere, wealthBand)`; in produzione la fascia è una (P-30 = 1). In CI chi diventa indigente va nella riga indigenti della propria fascia, e chi esce va nella stessa fascia del mestiere di destinazione, come per ogni cambio di mestiere (Z-05). Il generatore popola la riga al tick 0 leggendo le curve di ricchezza (PD-10).

**Motivazione.** Con una riga per mestiere il razionamento colpisce mestieri interi, e "chi ha meno" non esiste. Fasce fisse farebbero della povertà un'etichetta del generatore. Una riga in cui si entra quando non ci si sfama più al prezzo locale fa della povertà un esito del mercato. È distinta dal mestiere bracciante (PD-9) perché chi lavora a giornata per scelta non è povero, e una riga sola costringerebbe la regola d'uscita a espellerlo.

**Costo.** Si perde l'origine di chi entra. Due righe in più per insediamento: 11 in produzione, 33 in CI. La riga è raramente vuota, ma nessuna regola lo garantisce: il budget di DEC-041 va misurato con la riga piena.

### PD-2 — Si diventa indigenti per impoverimento

**Stato:** Deciso.

- Si entra solo per impoverimento. Chi entra lascia alla coorte edifici, durevoli, cibo, riserva di semina e debiti, e porta con sé solo valuta. Gli indigenti non possiedono edifici né durevoli, quindi FR-C-24 è vero per costruzione.
- **Nessun cancello del credito.** Il prestito della fase 3 entra già nella valuta, quindi nella ricchezza `w`. Un cancello misurato sulla coorte intera presupporrebbe che il membro povero possa dare in garanzia gli edifici del membro ricco: lo stesso errore di aggregazione che questa proposta vuole togliere.
- **Ricchezza e soglia.**
  - `w = (valuta + valore al prezzo locale del cibo posseduto, riserva esclusa) / count`
  - `T = costo, al prezzo locale, del nutrimento minimo di tier 0 per persona al giorno × P-68 giorni`
- La quota di membri sotto `T`, e la quota di ricchezza che detengono, si leggono dalla curva del mestiere al rapporto `T / w`. Chi entra porta la valuta in proporzione alla ricchezza della coda bassa.
- Il resto frazionario passa all'aggiornamento successivo in un campo della coorte, che si azzera quando la coorte si svuota (Z-04).

**Motivazione.** Fuori dal vettore, perché il reddito basso farebbe estinguere la riga per allocazione (DEC-061). Lasciare debiti e garanzie alla coorte tiene insieme ciò che il default di FR-C-27 deve poter pignorare.

**Costo.** FR-J-03 acquista un'eccezione. La soglia dipende dalle quantità dei bisogni, che sono contenuto di K-06.

### PD-3 — Gli indigenti escono colmando gli scarti del vettore

**Stato:** Deciso.

- Escono i membri la cui ricchezza supera `T × (1 + P-69)`, nella quota letta dalla curva del record indigente, con la valuta della coda alta.
- Destinazione: prima i mestieri la cui quota reale è sotto quella del vettore, in proporzione allo scarto; il resto in proporzione alle quote del vettore. La quota reale si calcola sulla popolazione esclusi gli indigenti. Il mestiere bracciante è una destinazione come le altre. La ripartizione tra destinazioni usa il metodo dei resti maggiori, a parità in ordine di id, senza stato.
- Quando: al successivo aggiustamento del mestiere di destinazione, cioè alla data di semina per le attività agricole e alla cadenza dell'insediamento per le altre.
- Il vettore è l'obiettivo e non si muove per le uscite: P-19 non entra in gioco.

**Motivazione.** In campagna le occasioni di ripresa arrivano meno spesso che in città, e deve vedersi. Colmare prima gli scarti ripopola i mestieri che la carestia ha svuotato.

**Costo.** Le asimmetrie della cadenza e del salario si sommano, e uno strascico rurale va attribuito alla fonte giusta. Le curve non hanno memoria: chi esce può ricadere sotto `T` all'aggiornamento dopo, e senza cancello del credito lo frena solo l'isteresi P-69 (O-12).

### PD-4 — La semina consuma un bene di semina

**Stato:** Deciso.

- Il record della coltura indica un **bene di semina** e il fabbisogno per unità di terra. Per cereali e patate è il bene raccolto stesso; per colture come il cavolo è un bene distinto, non cibo, prodotto dalla coltura come seconda resa.
- La semina consuma il bene di semina dalle scorte della coorte. Il lavoro di semina è quello dei membri più quello assunto. La terra senza seme o senza braccia resta incolta.
- **La riserva si calcola sulla terra assegnata:** `fabbisogno per unità di terra × terra che il vettore corrente assegna alla coltura della coorte`. Il vettore si muove solo alla semina, prima che la semina consumi il seme, quindi la riserva non supera mai il seme che la semina potrà usare.
- La riserva è esclusa da ogni uso diverso dalla semina: consumo proprio, offerta, input delle ricette.
- Il bene importato è anche seme: il commercio è la via d'uscita da una carestia pluriennale.

**Motivazione.** Senza seme un raccolto perso costa un anno, mai di più. Mangiare il seme è un prestito al tasso del rapporto di resa, e sposta la carestia dai compratori ai contadini per più anni. In una prima versione la riserva era calcolata sul lavoro, per liberare il seme di chi emigra; ma così poteva superare il seme davvero necessario. Il caso di chi emigra passa alla regola sui migranti (O-5, risolto da R-054: la riserva resta alla riga).

**Costo.** La durata delle carestie dipende dal rapporto di resa, contenuto statico. Nella spec: **planting good** e **seed reserve**, per non confondersi con `worldSeed` e P-65.

### PD-5 — Il seme commestibile è protetto fino alla disperazione

**Stato:** Deciso.

- La regola vale quando il bene di semina è cibo. Con le classi di PD-14 il grano è cibo di grado 0, quindi il seme liberato sfama chi lo possiede.
- Il deficit di fame è un campo di stato della coorte, per membro, in unità di nutrimento: cresce di quanto manca al bisogno di tier 0 e decade di P-70 per aggiornamento quando il bisogno è soddisfatto.
- Quando dei membri passano da una riga all'altra (ingresso e uscita dagli indigenti, riallineamento, migrazione), la riga che li riceve prende la media del proprio deficit e di quello della riga d'origine, pesata sui membri; la riga d'origine non cambia. Chi entra tra gli indigenti è probabilmente più affamato della media della propria coorte, ma la curva non lo dice: è un'approssimazione.
- Sotto la soglia P-66 la riserva non si tocca; sopra soglia si libera solo quanto basta a riportare il deficit a P-66.
- Il seme liberato va solo al consumo proprio. La quota che può entrare nell'offerta è P-67, zero in produzione e P-71 in CI.

**Motivazione.** Nessuno mangia il seme se non è disperato. La soglia produce entrambe le carestie, quella breve sui compratori e quella lunga sui contadini.

**Costo.** P-66 va confrontato con l'inizio della mortalità da fame. Vendere il seme liberato riaprirebbe la vendita forzata.

### PD-6 — Il lavoro limita la coltura alla semina e al raccolto

**Stato:** Deciso.

- Il lavoro non moltiplica la resa: la limita in due date, in superficie. Semina: `min(terra assegnata, bene di semina / fabbisogno, lavoro di semina / lavoro per unità di terra)`. Raccolto: a ogni aggiornamento della finestra, `superficie mietuta = min(superficie in campo, lavoro di raccolto / lavoro per unità di terra)`, e il raccolto è `superficie mietuta × resa × condizione`.
- Il lavoro è quello dei membri più quello assunto.
- La data di raccolto resta e apre una finestra di N aggiornamenti. Durante la finestra non si perde nulla; alla chiusura, quello che non è stato mietuto è perso.
- Il raccolto va alla coorte che ha seminato.

**Motivazione.** Senza braccia non si semina né si raccoglie, e contare il lavoro anche nella resa lo conterebbe due volte. Raccogliere tardi non costa finché la finestra è aperta, ma si rischia di non finire. I picchi di domanda danno ai braccianti la loro stagione.

**Costo.** Trebbiatura e trasporto, che dipendono dalla quantità, non sono modellati. La fase 4 deve vedere la domanda di lavoro degli stadi agricoli attivi.

### PD-7 — Tre stadi e una condizione da 0 a 2

**Stato:** Deciso. La regola economica per la cura piena è al decider: O-9.

- Stadi: semina, crescita, raccolto. La crescita dichiara due livelli di lavoro, cura normale e cura piena.
- La condizione, fixed-point su 0–20000, moltiplica la resa: scende verso 0 sotto la cura normale, vale 10000 alla cura normale, sale verso 20000 fino alla cura piena. Gli eventi che colpiscono la coltura la moltiplicano.
- In produzione i due livelli valgono 0 e la condizione resta a 10000; una configurazione CI li accende.

**Motivazione.** Con il campo presente fin dall'inizio, aggiungere cura ed effetti degli eventi è un cambio di dati e non una migrazione dei salvataggi.

**Costo.** Con la cura piena la stessa terra rende fino al doppio: B1 e B3a vanno ricontrollati prima di accenderla.

### PD-8 — Il campo senza coorte è di chi lo miete

**Stato:** Deciso.

Se la coorte che ha seminato si svuota, la coltura resta come campo libero fino alla chiusura della finestra. In fase 4, dopo che il mercato del lavoro ha chiuso, il lavoro rimasto senza impiego viene assegnato ai campi liberi; in fase 5 miete con il raccolto ordinario. Ogni coorte tiene il prodotto in proporzione al lavoro messo, con il metodo dei resti maggiori. Un anticipo su quel raccolto si cancella con FR-C-27.

**Motivazione.** Un campo maturo che marcisce accanto a un villaggio affamato sembrerebbe un errore. Il lavoro inattivo esiste già, e in crisi è soprattutto degli indigenti.

**Costo.** Un passo in più dopo il mercato del lavoro.

### PD-9 — Il bracciante è un mestiere scelto, con un tasso proprio

**Stato:** Deciso.

- Il bracciante è un mestiere di servizio dentro il vettore, senza edificio, con rendimento atteso pari al salario. Corrisponde al bracciante di K-02.
- Ogni attività può dichiarare nel record un tasso di aggiustamento e un tetto propri; senza, valgono P-19 e il tetto annuale. Il bracciante ha un tasso più rapido e nessun tetto annuale.
- Quando l'offerta di lavoro supera la domanda, ogni coorte che vende lavoro è assunta in proporzione al lavoro offerto.

**Motivazione.** Il lavoro a giornata è pagato nello stesso aggiornamento, quindi non ha il ritardo che genera la ragnatela delle colture. Con lo smorzamento delle colture non seguirebbe un raccolto di poche settimane.

**Costo.** P-19 non è più l'unico tasso. Un aggiornamento di ritardo tra salario e arrivi resta, quindi il tasso va tenuto sotto 1. Quando il salario non conviene, la quota tende a zero (DEC-061).

### PD-10 — La ricchezza dentro una riga è una curva per mestiere

**Stato:** Deciso.

- Il record del mestiere porta una curva della quota di membri sotto un dato rapporto con la ricchezza media, e la curva compagna della quota di ricchezza che detengono, ciascuna in al più 32 punti.
- Uno strumento offline le genera da una lognormale con il σ del record; sono contenuto statico e si possono modificare. Il core le interpola in aritmetica intera (NFR-02, come le curve climatiche di DEC-037).
- **La curva serve a due cose.** All'ingresso e all'uscita degli indigenti, applicata alla ricchezza `w`. Nel razionamento, applicata alla valuta pro capite, dà la quota dei membri di una coorte che riesce a pagare: è la frazione di accessibilità che Z-09 attribuiva a `wealthSpread` senza scriverla.
- `wealthSpread` esce dallo stato.
- La **wealth tendency** di FR-J-01 è la ricchezza media iniziale per membro, usata dal generatore; in CI guida anche la divisione della popolazione iniziale tra le fasce (Z-20). σ e le curve ne descrivono la dispersione.

**Motivazione.** Una riga uniforme attraverserebbe una soglia tutta insieme. La lognormale non ha pavimento, quindi la povertà compare per gradi. La curva scritta a mano tiene la forma fuori dall'architettura.

**Costo.** La curva non ha memoria. Usare la stessa forma per la ricchezza e per la sola valuta è un'approssimazione. Le curve non sono calibrate su dati storici (O-8).

### PD-11 — La domanda è limitata dalla valuta, che include il prestito

**Stato:** Deciso.

La domanda è limitata dalla valuta, che include quanto preso a prestito in fase 3. La linea di credito non si aggiunge una seconda volta, né nella formula di §Demand né nella cella della fase 7.

### PD-12 — Ingresso e uscita degli indigenti in fase 8a

**Stato:** Deciso.

- Ingresso e uscita girano in una fase 8a, dopo rimborso e default. Le uscite verso attività agricole avvengono solo nell'aggiornamento che contiene una data di semina.
- Il vettore si muove dove si muove oggi; il riallineamento delle quote di FR-J-03 avviene in fase 12, solo per le attività il cui aggiustamento è caduto nell'aggiornamento corrente.

**Motivazione.** Il default della fase 8 è la via d'ingresso più ovvia. Il riallineamento deve venire dopo le uscite, altrimenti colmerebbe gli scarti con membri solventi prima degli indigenti.

**Costo.** Tra la fase 5 e la fase 12 le quote reali sono disallineate dal vettore appena mosso, e nessuna regola letta in quell'intervallo deve presumere che coincidano.

### PD-13 — Il nuovo anello lavoro–povertà e i suoi freni

**Stato:** Deciso. Il secondo freno dipende da O-5 e O-6, affidati al decider. Il decider li risolve nella stessa sessione, e la proposta entra in `docs/` solo con B12b scritto, così il gate della fase 1 non si riapre (decisione dell'utente). O-5 e O-6 sono risolti da R-054 e R-055, e B12b è scritto in SIM-LOOPS.

- **R12.** Gli indigenti vendono lavoro, il salario scende, altre coorti cadono sotto soglia, gli indigenti aumentano.
- **B12a.** Con il salario basso il vettore riduce la quota dei braccianti, e l'offerta di lavoro cala. Non agisce quando quella quota è già vicina a zero.
- **B12b.** Gli indigenti emigrano. È il freno vero, ma richiede la regola sui migranti (O-5) e il collegamento tra indigenti e "displaced population" (O-6).

**Motivazione.** Il gate della fase 1 vuole un freno nominato per ogni anello rinforzante.

### PD-14 — Classi di beni, grado di lavorazione e valore nutrizionale

**Stato:** Deciso. La regola di conservazione nelle ricette e la nuova forma dei ruoli di FR-P-03 sono al decider: risolte da R-056.

- Una **classe** è un record di dati con un nome e un'eventuale classe madre: cibo, bevande, ingredienti, materie prime, e sottoclassi come "cereali" sotto "cibo". Si aggiungono, si modificano e si annidano senza codice, e viaggiano con `ruleVersion`.
- Ogni **bene** dichiara le classi a cui appartiene, anche più di una, con un **valore** per unità in ciascuna: per le classi alimentari è il valore nutrizionale. Dichiara anche il **grado di lavorazione**: il grano grado 0, il pane grado 1. La farina è un ingrediente di grado 1, non un cibo.
- Un **bisogno** è definito su una classe: classe, grado minimo, tier di urgenza, quantità in unità della classe per persona al giorno. Un bisogno su una classe è soddisfatto anche dai beni delle sottoclassi.
- Il tier 0 ha un solo bisogno, sulla classe cibo, e un controllo al caricamento lo impone. Il deficit di fame, la soglia `T` e P-66 si misurano quindi in unità di nutrimento. Un secondo bisogno di tier 0 richiederebbe una ruling.
- Il **tier** resta l'urgenza del bisogno (SIM-ECON) e non è una proprietà del bene. Il grado minimo collega le due scale: la sopravvivenza può chiedere "cibo, grado almeno 0", il tier 1 "cibo, grado almeno 1".
- I ruoli di FR-P-03 descrivono solo il comportamento nella produzione e nel tempo: consumabile, input, durevole. L'urgenza passa ai bisogni. Questo chiude Z-07. *(Al decider)*
- Nelle classi alimentari nessuna ricetta produce più valore di quanto ne consumi: la lavorazione non crea cibo. *(Al decider)*

**Motivazione.** Con i bisogni definiti per bene, ogni cibo nuovo sarebbe un bisogno in più: tutti dovrebbero mangiare pane *e* patate. Le classi rendono il cibo un insieme di alternative, e il grano diventa cibo, quindi il seme liberato sfama. Senza la regola di conservazione, in carestia basterebbe cuocere tutto il grano per avere più da mangiare.

**Costo.** La domanda della fase 7 diventa una scelta tra beni della stessa classe. Il contenuto statico cresce: classi, appartenenze, valori, gradi, bisogni (K-01, K-06), tutti da approvare per R-050.

### PD-15 — I bisogni della stessa classe si sommano

**Stato:** Deciso dal decider: R-057, con una motivazione diversa. Le due opzioni differiscono nella quantità, non nel grado: sommando, un anno normale mangia il minimo più la quota del tier 1, e la carenza toglie prima la quota, che è rinviabile. La regola vale per ogni coppia di bisogni: ciascuno è una quantità a sé, e un'unità consumata ne soddisfa uno solo.

Due bisogni sulla stessa classe si sommano: per sopravvivere serve una quantità di cibo di qualunque grado, e per il tier 1 serve una quantità *in più* di cibo di grado almeno 1.

**Motivazione.** Tiene separati il minimo per sopravvivere e il pasto normale. In carestia si scende al minimo e si stringe la cinghia prima di morire: la fame resta una stagione, come vuole SIM-ECON. L'alternativa, una razione unica migliorata di grado, fa coincidere il pasto normale con il minimo vitale: chi perde potere d'acquisto passa subito dal mangiare bene al morire, e in carestia tutta la domanda di pane si riversa sul grano proprio quando scarseggia.

**Costo.** Negli anni buoni il piatto è misto, un po' di grado basso e un po' di grado alto. Le quantità di K-06 vanno tarate perché il totale sia plausibile.

### PD-16 — Il piatto segue la ricchezza

**Stato:** Deciso dal decider: R-058, con tre correzioni. Il miglioramento di grado viene dopo i bisogni di tier 0 e 1, sia nella spesa sia nel razionamento. La domanda razionata ripiega su qualunque bene che soddisfi lo stesso bisogno, non solo sul grado inferiore. L'alternanza fra cibi dello stesso grado è scritta come costo.

- **Prima le proprie scorte**, dal grado più basso: il pane resta per il bisogno che lo richiede.
- **Poi la sopravvivenza**, con il cibo più economico per unità di nutrimento.
- **Poi il miglioramento.** Con quello che resta, la coorte sostituisce unità di grado basso con unità di grado più alto finché il budget lo consente. Con due gradi è una formula chiusa, con più gradi un procedimento goloso, calcolato una volta per coorte e per aggiornamento.
- **La curva divide la coorte.** La quota dei membri che può permettersi ogni grado si legge dalla curva di ricchezza: dentro lo stesso mestiere i più poveri mangiano grano e i più agiati pane.
- **Ripiego dentro l'aggiornamento.** I mercati di una classe si liquidano in ordine di grado decrescente, e la domanda non soddisfatta per razionamento scende al grado inferiore nello stesso aggiornamento.

**Punti forti.** Il piatto rende leggibile la ricchezza (OBS-3): in carestia il pane sparisce prima dalle case dei poveri. La domanda di grano diventa più elastica, e i prezzi hanno più gradazioni (OBS-1). Usare la curva invece della coorte intera evita che una piccola variazione di prezzo sposti tutta la domanda da un bene all'altro.

**Punti critici.** Ogni cibo in più è un mercato in più da razionare, nella fase più costosa: il numero di cibi va deciso dopo una misura. Il ripiego per grado impone un ordine fra i mercati di una classe, che oggi non esiste. Una coorte ricca compra comunque il cibo più economico per la parte di sopravvivenza: il "piatto del ricco" è dato dal miglioramento, non dalla sopravvivenza.

### PD-17 — Produzione domestica: orto, animali, raccolta

**Stato:** Al decider. La scelta tra includerla in questo lotto o nel successivo è del decider; il documento non ne dà il testo di spec.

**Cosa è.** Il cibo che non passa dal mercato: l'orto di casa, i piccoli animali come le galline, la raccolta di bacche e radici nel bosco.

**Perché conta.** Con la produzione domestica il piatto dei poveri dipende anche da *dove* vivono. I poveri di campagna hanno cuscinetti, quelli di città dipendono dal mercato. La carestia diventa una sequenza leggibile di strategie: meno pane, l'orto, le galline, il bosco, il seme, l'emigrazione.

**Forma consigliata.** Un solo meccanismo generico: un record di **produzione domestica** che dichiara da dove prende la terra (quota per famiglia, sito forestale, nessuna), da dove prende il lavoro, cosa produce, in quale stagione e se attinge a uno stock comune. Orto, pollaio e raccolta sono tre righe di dati.

**Problemi da risolvere.**
- **Terra.** L'orto richiede una quota della terra per famiglia, fuori dal vettore. Tocca B1 e A-16. In città la terra per persona è poca, e niente orti.
- **Lavoro.** Il lavoro domestico non è sul mercato. Le opzioni: le fasce d'età non lavorative (bambini, anziani), una quota fissa del lavoro dei membri con un P-number, o solo il lavoro inattivo come per la spigolatura.
- **Animali.** Sono capitale che si mangia, come il seme: serve uno stock per coorte e una regola di protezione simile a P-66. Mangiano scarti o grano, e in carestia competono con le persone.
- **Bosco.** È un bene comune che si rigenera: in carestia tutti raccolgono, lo stock si esaurisce e la resa crolla. È un freno naturale, ma è stato nuovo per insediamento.
- **Stagioni e deperibilità.** Ortaggi e bacche marciscono in fretta; l'abbondanza estiva non si accumula e la primavera prima del raccolto diventa più dura.
- **Equilibrio.** Se orto e bosco rendono troppo, la povertà diventa comoda: la raccolta va tenuta poco nutriente rispetto al lavoro, stagionale ed esauribile.

---

## Modifiche alla documentazione

I testi tra le righe `>` sono da inserire così come sono. Le parti marcate **[DECIDER]** erano testo suggerito o segnaposto per una scelta del decider: PD-15, PD-16, O-5, O-6, la forma di FR-P-03, FR-P-03c e la regola di conservazione in DEC-098. Sono state sostituite dal testo deciso in R-054 … R-058, marcato con il numero della ruling.

### SIM-DEC

**Dove vanno le nuove decisioni.** In coda alle sezioni:
- *Occupations and transients*: DEC-090, DEC-091, DEC-096, DEC-097.
- *Climate and agriculture*: DEC-092, DEC-093, DEC-094, DEC-095.
- *Economy*: DEC-098, DEC-099.

**Nuove decisioni.**

> ### DEC-090 — Destitution is a row, entered by impoverishment
>
> Every settlement holds a destitute row: a service mestiere with no building, outside the allocation vector, entered only by impoverishment. The destitute own no buildings and no durables. The generator populates the row at tick 0 from the wealth curves.
>
> **Rationale.** With one row per mestiere the rationing order of DEC-043 reaches whole trades, so a famine falls evenly on the rich and the poor of the same mestiere and nobody is the one who has least. Fixed wealth bands would make poverty a label written by the generator. A row entered when members can no longer feed themselves at the local price makes poverty a market outcome, and gives FR-C-24 a population that cannot borrow by construction. It is kept apart from day labour by choice, so that recovery can be read from wealth without expelling those who labour by choice. Entry is not gated on the cohort's credit: a poor member cannot pledge a rich member's buildings, and what the cohort borrows in phase 3 already counts in its wealth.
>
> **Cost.** FR-J-03 gains an exception. An entrant's former mestiere is lost. The row is seldom empty, and the budget of DEC-041 is measured with it full.

> ### DEC-091 — The destitute leave to fill the vector's gaps, at the cadence of their destination
>
> Destitute members above the entry threshold by P-69 leave at the next adjustment of their destination: a crop's sowing date for agricultural activities, settlement cadence for the others. They go first to mestieri whose actual share, among the population outside the destitute row, is below the allocation vector's, in proportion to the gap, and the rest in proportion to the vector. The vector is a target and does not move for them.
>
> **Rationale.** The countryside offers fewer chances to recover than the town, and the asymmetry should show. Filling gaps first repopulates the trades a famine emptied. Since the vector does not move, P-19 is not involved and no cobweb can form.
>
> **Cost.** Two asymmetries add: the destination's cadence and the local wage. Wealth curves have no memory, so a member who leaves can fall back below the threshold; only the margin P-69 holds it.

> ### DEC-092 — Sowing consumes a planting good, protected until desperation when it is food
>
> A crop record names its planting good and the amount needed per unit of land. For grain the planting good is the harvest itself; a crop whose seed is not food names a separate good it yields alongside its harvest. Sowing consumes the planting good from the sowing cohort's holdings. The seed reserve is derived, not stored: the amount per unit of land times the land the current allocation vector gives the cohort's crop, and nothing but sowing draws on it. When the planting good is food, the reserve is released once the cohort's hunger deficit exceeds P-66, only as far as needed to hold the deficit there, and only to own consumption; P-67 is the share that may reach the market, zero in the shipping configuration.
>
> **Rationale.** Without seed a lost harvest costs one year, since the next sowing is whole regardless. Eating seed borrows against next year at the crop's yield ratio, and the choice is distributive: keeping seed pushes the famine onto those who buy, eating it drags it onto those who grow over several years. A threshold produces both. Imported grain is also seed, so trade becomes the way out of a famine that outlasts a year. Bounding the reserve by allocated land keeps it from exceeding what the next sowing can use.
>
> **Cost.** Famine length depends on each crop's yield ratio and on where P-66 sits against the onset of hunger mortality. Farmers gain a path into destitution that DEC-043 excluded. Letting P-67 above zero reopens forced sale.

> ### DEC-093 — Labour bounds a crop at sowing and at harvest, over a window
>
> Labour is a capacity at two dates, not a yield multiplier, and both are measured in area. Labour is the cohort's own and what it hires. Sowing labour bounds the land sown. The harvest date opens a window of N updates, N carried by the crop, and harvest labour bounds the area reaped in each update of it. Nothing is lost while the window is open; what is still standing when it closes is lost. The harvest goes to the cohort that sowed.
>
> **Rationale.** Without hands nothing is sown or reaped, and counting labour again in the yield would count it twice. Reaping depends on the plants sown, not on what each one yields. A window keeps a single week's labour from deciding a year's harvest: reaping late costs nothing until the window closes, and the risk is not finishing. The demand peaks give day labourers their season through the wage.
>
> **Cost.** Threshing and carting scale with quantity and are not modelled, so a doubled harvest needs no more hands. A standing crop becomes state per cohort per crop. The labour market of the settlement update must see the demand of every crop stage active in the update.

> ### DEC-094 — A standing crop carries a condition from neglect to full care
>
> A crop's life has three stages, sowing, growth and harvest, each declaring labour per unit of land. Growth declares two levels, normal care and full care. A standing crop carries a condition on 0–20000 that multiplies its yield: below normal care it falls towards 0, at normal care it is 10000, towards full care it rises to 20000. Exogenous events that strike a standing crop multiply its condition. Both levels are zero in the shipping configuration, where the condition stays at 10000, and non-zero in a CI configuration.
>
> **Rationale.** Tending the crop and the events that strike it between sowing and harvest both need somewhere to act. Holding that field from the first day, at a degenerate value, keeps adding them a data change rather than a change of state layout and a save migration. Two levels keep any labour spent on a crop with a zero requirement from reading as extra care.
>
> **Cost.** A field that does nothing in the shipping configuration, and a CI configuration to keep the code that moves it from rotting. Full care doubles what the same land can yield, which loosens B1 and B3a: both assume a fixed yield per unit of land, and both are rechecked before care is enabled.

> ### DEC-095 — A crop left without its cohort belongs to whoever reaps it
>
> When the cohort that sowed a crop is empty, the crop stands as an open field until its harvest window closes. Labour left unhired when the labour market clears is assigned to it and reaps it with the harvest, and each cohort keeps the harvest in proportion to the labour it put in.
>
> **Rationale.** A ripe field rotting beside a hungry village reads as a fault. Idle labour already exists after the labour market clears, so an open field needs no wage, no payer and no new market. In a crisis the idle are mostly destitute, so the gleaning reaches them without naming them.
>
> **Cost.** One more step after the labour market. An advance on the crop is written off, since its collateral became everyone's.

> ### DEC-096 — Day labour is a chosen mestiere that follows the wage at its own rate
>
> Day labourer is an ordinary service mestiere inside the allocation vector, with no building, whose expected return is the wage. An activity's record may carry its own adjustment rate and cap, P-19 and the yearly cap of FR-P-15 applying where it carries none; day labour carries a faster rate and no yearly cap. Where labour offered exceeds labour demanded, every cohort selling labour is hired in proportion to what it offers.
>
> **Rationale.** The cobweb comes from the delay between sowing and harvest. Day labour is paid in the update it is hired, so it needs none of the damping crops need, and with that damping its share could not follow a harvest lasting a few weeks. A rate in the record avoids an exception wired to one mestiere. Proportional hiring favours neither those who labour by choice nor the destitute.
>
> **Cost.** P-19 is no longer the vector's only rate. One update still separates the wage from the arrivals it draws, so the rate is tuned below one. When the wage does not justify day labour the vector drives its share towards zero, as DEC-061 expects of any unremunerative mestiere.

> ### DEC-097 — Wealth within a row is an authored curve, generated from a lognormal
>
> Each mestiere record carries a wealth curve, the share of members below a given ratio to the row's mean, and its companion curve, the share of wealth those members hold, each sampled at up to 32 points. An offline tool generates both from a lognormal with the record's σ; they are static content and may be edited. The core interpolates them in integer arithmetic. The same curves read wealth for entry to and exit from destitution, and currency for the share of a cohort able to pay when supply is rationed.
>
> **Rationale.** A row is uniform and would cross any threshold all at once; the curve says how many members lie below it. A lognormal has no floor, so poverty appears by degrees. Pareto describes the rich tail and has a floor at the bottom like a uniform. Authored curves, as for climate under DEC-037, keep transcendental functions out of the core under NFR-02 and keep the shape out of the architecture.
>
> **Cost.** The curve has no memory: after each crossing the row spreads again around its new mean, so persistence lives in the destitute row. Reading wealth and currency through one shape is an approximation. Two curves and a σ per mestiere to approve, an offline tool, and no historical data to calibrate them against yet.

> ### DEC-098 — Needs are met by classes of goods, by grade
>
> Goods belong to classes, data records that may nest under a parent class. A good carries a value per unit in each class it belongs to, nutrition for food, and one processing grade. A need names a class, a minimum grade, a tier and a quantity in the class's units per person per day, and any good of the class or its subclasses at or above the grade meets it. Tier is the urgency of the need, not a property of the good. Processing creates no food: no recipe yields more food value than its inputs embody, a good outside the food class carrying the least food value consumed to make it. *(R-056)*
>
> **Rationale.** With needs defined per good, every new food would be one more need: everyone would have to eat bread and also potatoes. Classes make food a set of alternatives, and the minimum grade keeps urgency and processing on separate scales while letting them meet: survival can accept grain, a basic need can ask for bread. Grain becomes food, so a released seed reserve feeds the cohort that holds it. Without the conservation rule, baking would create food, and a famine could be cured in the oven. The rule runs along the chain rather than recipe by recipe, because flour belongs to no food class: milling consumes food, baking yields it, and only the two together say whether food was made. *(R-056)*
>
> **Cost.** Demand becomes a choice among goods of a class. Static content grows: classes, memberships, values, grades and needs, all approved under R-050. A recipe table can fail to load for a food value set too high several recipes away from the error. *(Ultima frase: R-056)*

> ### DEC-099 — The plate follows the purse
>
> Needs add: each is a quantity of its own, and a unit consumed meets one need. *(R-057)* A cohort meets its needs tier by tier from its own holdings first, lowest grade first, so that a better good is kept for the need that asks for it, and buys what remains of each need at the lowest price per unit of value among the goods that meet it. With the currency left once tiers 0 and 1 are met, and before tier 2 takes its fraction, members replace units of a lower grade with units of a higher grade that meet the same need, as far as they can pay; the share of members able to pay for each grade is read from the wealth curve (DEC-097). In rationing, demand that meets a need comes before demand that only raises its grade. Demand rationed out of one good falls, within the update, to the other goods that meet the same need, cheapest per unit of value first. *(R-058)*
>
> **Rationale.** Adding needs keeps the survival minimum apart from the ordinary meal. An ordinary year eats the minimum and the tier 1 share, so a shortfall first takes the share, which is deferrable, and only then the minimum: a famine tightens belts before it kills, and stays a season. One ration made better by grade would leave no such margin, and every unit short would be hunger. *(R-057)* The plate then shows wealth: within one mestiere the poorer eat grain and the better-off bread, and in a famine bread leaves the poorer tables first. Reading the better grade through the wealth curve rather than the cohort's mean keeps a small price move from switching a whole cohort between goods. Ranking it after every need keeps the rationing rule of SIM-ECON: nobody eats better while another cannot eat and could pay. *(R-058)*
>
> **Cost.** Every food is one more market to ration in the most expensive phase, so the number of foods is set after a measurement. The markets of a class acquire an order, derived from grade, price and good id, and demand falling between them may cross them more than once in an update. In ordinary years the plate is mixed, and the quantities of the needs table are tuned so that the total is plausible. A need's demand goes whole to its cheapest good, so two foods of one grade at nearly equal prices can take turns from one update to the next. *(R-058)*

**Decisioni da modificare.**

- **DEC-038** — Nel testo, sostituire "and yield per land and labour. Harvest delivers the full amount into settlement stock at once." con:
  > and, per DEC-092 to DEC-094, a planting good, labour per stage and yield per unit of land. The harvest date opens a window of updates over which the crop is reaped.

  Sostituire l'intero costo con:
  > **Cost.** Grain output responds to a shock only at the next harvest. An event meant to change food supply within the year acts on a standing crop through its condition (DEC-094), or on stock, transport, or population.

- **DEC-043** — Nella motivazione, sostituire l'ultima frase con:
  > It also gives famine a legible shape: labourers, artisans, and townsfolk feel it before farmers, and farmers feel it when they start eating their seed reserve (DEC-092).

- **DEC-047** — Nel testo, aggiungere: "An activity may carry its own rate and cap (DEC-096)." La contraddizione sulla cadenza resta nei difetti (X-3).

- **DEC-076** — Sostituire il costo con:
  > **Cost.** No rent, no great estates. A peasant is dispossessed only by losing what working the land needs: seed, durables, buildings (DEC-090, DEC-092). Rent and tenancy stay outside the model until the cohort key gains its second axis.

- **DEC-054** — Nessuna modifica. P-30 resta 1.

- **DEC-015** — Nel testo, sostituire "Unit weight, reference value, perishability, role, need tier." con:
  > Unit weight, reference value, perishability, role, classes with a value in each, processing grade.

- **DEC-080** — Nel testo, sostituire "Validation at load proves nothing in tier 0 or tier 1 depends on one." con:
  > Validation at load proves every tier 0 and tier 1 need can be met without one.

### SIM-REQ

**§6 Goods and production.**

- **FR-P-01** — Sostituire con:
  > **FR-P-01** — A good is a data record: unit weight, value reference, perishability, role, the classes it belongs to with a value per unit in each, and its processing grade (FR-P-03a). Adding a good requires no code change.

- **FR-P-03** — Sostituire con: *(R-056)*
  > **FR-P-03** — Roles describe how a good behaves in production and over time:
  >
  > | Role | Behaviour |
  > |---|---|
  > | Consumable | consumed to meet the needs that accept it (FR-P-03b) |
  > | Input | consumed by the recipes that take it (FR-P-02) |
  > | Durable | not consumed, decays slowly, raises productivity |
  >
  > A good's record says whether it is durable. A good that is not takes the consumable role where a need accepts it and the input role where a recipe takes it, and may take both, as grain does. How urgently a good is wanted is not a property of the good but of the needs it meets (FR-P-03b).

- Nuovi requisiti dopo FR-P-03:
  > **FR-P-03a** — A goods class is a data record with a name and an optional parent class. A good belongs to one or more classes and carries a value per unit in each, nutrition for food, and one processing grade. Classes and memberships are added or changed without code change.

  > **FR-P-03b** — A need names a class, a minimum grade, a tier and a quantity in the class's units per person per day. Any good of the class or of its subclasses, at or above the minimum grade, meets it in proportion to its value. Tier 0 holds exactly one need, on the food class, and validation at load refuses a needs table that holds another. Needs add: each is a quantity of its own, and a unit consumed meets one need only. *(Ultima frase: R-057)*

  > **FR-P-03c** — Processing creates no food. Every good carries an embodied food value: a good of the food class, the class the tier 0 need names, carries its value there; another good that recipes produce carries the least food value that any recipe producing it consumes per unit, each input counted at its own embodied food value; any other good carries zero. No recipe yields more food value than its inputs embody, and validation at load refuses a recipe table that breaks this or that passes food value around a cycle. *(R-056)*

- **FR-P-04** — Sostituire con: *(Decisione dell'utente: la farina entra subito, e la catena del pane passa da essa.)*
  > **FR-P-04** — Baseline goods set *(proposed, pending confirmation, with classes, values and grades under FR-P-03a)*: Grain, Flour, Bread, Ore, Timber, Tools, Wool, Cloth. Three chains: Grain→Flour→Bread, Ore+Timber→Tools, Wool→Cloth.

  La lista resta contenuto da approvare (K-01).

- **FR-P-10** — Sostituire con:
  > **FR-P-10** — Crops are not produced continuously. A crop is a data record carrying sowing date, maturation length, harvest date, harvest window length in updates, its planting good and the amount needed per unit of land, yield per unit of land, and labour per unit of land for sowing, for harvest, and for growth at normal care and at full care.

- Nuovi requisiti dopo FR-P-10:
  > **FR-P-10a** — Sowing consumes the planting good from the sowing cohort's holdings. Land sown is the least of the land allocated, the land the planting good held can sow, and the land the cohort's own and hired sowing labour can sow. Land without seed or hands lies fallow.

  > **FR-P-10b** — The harvest date opens the crop's window. Each update reaps the lesser of the area still standing and the area the cohort's own and hired harvest labour can reap, and delivers area reaped × yield per unit of land × condition to the holdings of the cohort that sowed. Nothing is lost while the window is open; what is still standing when it closes is lost.

  > **FR-P-10c** — A standing crop carries a condition on 0–20000, set to 10000 at sowing. Growth labour below normal care lowers it towards 0; growth labour between normal and full care raises it towards 20000. Exogenous events striking a standing crop multiply it. Both care levels are zero in the shipping configuration; CI runs a configuration where they are not.

  > **FR-P-10d** — A cohort's seed reserve is the planting good needed per unit of land times the land the current allocation vector gives its crop. Only sowing draws on it: own consumption, market supply, recipe inputs and emigrants (FR-D-04a) leave it untouched. When the planting good is food and the cohort's hunger deficit exceeds P-66, the reserve is released to own consumption only as far as needed to bring the deficit back to P-66. A share P-67 of what is released may become supply.

  > **FR-P-10e** — A standing crop whose cohort is empty is an open field until its harvest window closes. Labour left unhired once the labour market of FR-C-05 clears is assigned to it and reaps it under FR-P-10b, and each cohort keeps the harvest in proportion to the labour it supplied. An advance against it is written off under FR-C-27.

- **FR-P-15** — Sostituire con:
  > **FR-P-15** — At each crop's sowing date the settlement computes expected return per activity as expected yield × current price − input cost − wage cost, then moves the allocation toward the highest by the activity's adjustment rate, capped per year by the activity's cap. An activity whose record carries neither uses P-19 and the yearly cap. Non-agricultural activities adjust the same way at settlement cadence; the expected return of day labour is the wage.

**§5 Population and demography.**

- Nuovo requisito dopo FR-D-03:
  > **FR-D-03a** — Unmet tier 0 need accumulates as a hunger deficit per member of each cohort, in units of nutrition, and decays by P-70 per update when the need is met. When members move from one row to another, the receiving row's deficit becomes the average of its own and that of the row they leave, weighted by count; the row they leave keeps its own. The death rate's response to hunger in FR-D-03 reads this deficit.

- Nuovo requisito dopo FR-D-04: *(R-054; la prima frase è di R-055)*
  > **FR-D-04a** — Emigrants are drawn from a settlement's destitute row first, and from its other rows in proportion to their counts for the rest. Members leaving a row take its mean share per member: all of its currency, and of its goods and durables outside the seed reserve as much as P-72 weight per member allows, highest value per unit of weight first, ties by good id. Buildings never move, and what emigrants do not take stays with the row. On the road, emigrants are a scheduled transient carrying their count, the key of the row they left, their hunger deficit, their currency as one integer and their goods.

**§6, sottosezione Occupations.**

- **FR-J-01** — Sostituire "wealth tendency" con "wealth tendency, the mean wealth per member the generator gives it," e aggiungere alla fine: "It also carries a wealth curve under FR-J-22, and may carry its own adjustment rate and cap under FR-P-15."

- **FR-J-03** — Sostituire con:
  > **FR-J-03** — The allocation vector is a target, and the share of population in each mestiere is realigned to it whenever the mestiere's activity adjusts. The destitute row of FR-J-19 is the one exception: it is outside the vector, entered only by impoverishment, and left under FR-J-20 before realignment runs.

- Nuovi requisiti:
  > **FR-J-19** — Every settlement holds a destitute row: a service mestiere with no building, selling labour under FR-C-05, outside the allocation vector. With w a cohort's wealth per member, counting currency and food held at the local price with the seed reserve excluded, and T the local cost of the tier 0 need per person per day times P-68, the share of members below T and the share of wealth they hold are read from the cohort's wealth curve at T / w. Those members enter the destitute row of their wealth band, taking their share of the cohort's currency and nothing else. The destitute own no buildings and no durables.

  > **FR-J-20** — Destitute members whose wealth exceeds T × (1 + P-69), in the share read from the destitute row's wealth curve, leave at the next adjustment of their destination under FR-P-15: a crop's sowing date for agricultural activities, settlement cadence for the others. They go first to mestieri whose actual share, among the population outside the destitute row, is below the allocation vector's, in proportion to the gap, and the rest in proportion to the vector, staying in their wealth band and taking their share of the row's currency. The vector does not move for them.

  > **FR-J-21** — Day labourer is a service mestiere with no building inside the allocation vector, whose expected return is the wage. Its record carries an adjustment rate faster than P-19 and no yearly cap.

  > **FR-J-22** — A mestiere record carries a wealth curve, the share of members below a ratio to mean wealth, and its companion curve of the share of wealth they hold, each at up to 32 points, generated offline from a lognormal with the record's σ and editable as static content. The core interpolates them in integer arithmetic. They are read for destitution under FR-J-19 and FR-J-20, and for the share of a cohort able to pay under FR-C-04.

**§7 Money and income.**

- **FR-C-03** — Sostituire con:
  > **FR-C-03** — Consumption order is fixed: a cohort meets its needs from its own holdings first, lowest grade first, then buys the remainder at market.

- **FR-C-04** — Sostituire i passi 1–4 con quelli che seguono; i passi 5 e 6 restano invariati:
  > 1. Cohorts consume from their own holdings, the seed reserve excepted under FR-P-10d.
  > 2. Unmet need becomes demand for the goods that meet it, chosen under DEC-099, capped by the cohort's currency, which includes what it borrowed this update. *(R-058)*
  > 3. Holdings above the cohort's own needs and its seed reserve become supply.
  > 4. If supply is short, it is allocated in descending order of ability to pay, the currency per member, demand that meets a need before demand that only raises its grade; the share of a cohort able to pay is read from its wealth curve under FR-J-22. Demand rationed out of one good falls to the other goods that meet the same need, within the update. *(R-058, dalla prima virgola dopo "member")*

- **FR-C-05** — Sostituire con:
  > **FR-C-05** — Labour is traded like a good that cannot be stored: its price is set by the same bounded function of supply against demand, and it expires each update. Cohorts working land allocated to them or a standing crop, or owning buildings, hire labour from cohorts that do neither, and pay wages in currency. Where labour offered exceeds labour demanded, every cohort selling labour is hired in proportion to what it offers.

- **FR-C-25** — Sostituire con:
  > **FR-C-25** — Payment advanced against future output is a loan collateralised by that output: the cohort's standing crop under FR-P-10b. It is held as its own balance, apart from the cohort's other debt. A crop lost before harvest, or left to an open field under FR-P-10e, leaves the advance to be written off under FR-C-27.

**§8 Markets and trade.**

- **FR-M-13** — Aggiungere alla fine: *(R-055)*
  > Destitute members who emigrate under FR-D-04a are famine refugees: each raises the danger of the link it travels by P-40.

**§14 World generation.**

- **FR-G-01** — Aggiungere alla fine:
  > Initial populations include each settlement's destitute row, read from the wealth curves, and in a configuration with several wealth bands the generator divides each mestiere's population among them by its wealth tendency.

**§16 Parameters.** Nuove righe:

| # | Parametro | Valore | Stato |
|---|---|---|---|
| P-66 | Hunger deficit above which a food planting good is released | TBD by sweep | Open |
| P-67 | Share of released planting good that may become supply, shipping | 0 | Decided |
| P-68 | Days of tier 0 need a member's wealth must cover to stay out of destitution | TBD by sweep | Open |
| P-69 | Exit margin for the destitute, as a fraction of T | TBD by sweep | Open |
| P-70 | Hunger deficit decay per update when the tier 0 need is met | TBD by sweep | Open |
| P-71 | Share of released planting good that may become supply, CI | TBD | Open |
| P-72 | Weight an emigrant carries, per member (FR-D-04a; R-054) | TBD by sweep | Open |

Riga da modificare: **P-09**, Goods count, da 7 a **8**, stato Decided (decisione dell'utente: la farina, FR-P-04).

P-19 resta come valore predefinito per le attività senza un tasso proprio.

**Contenuto statico da approvare** prima del piano che lo usa (§16, R-050): i record dei mestieri indigente e bracciante, con tasso e tetto del secondo; per ogni mestiere σ, le due curve e la wealth tendency; le classi di beni, le appartenenze, i valori nutrizionali e i gradi; la tabella dei bisogni per classe, grado minimo e tier (K-06); il bene farina, le ricette Grain→Flour e Flour→Bread e l'edificio che ciascuna richiede, e il grado del pane ora che la catena ha un passo in più; per ogni coltura bene di semina e fabbisogno, resa, lavoro per stadio e livello di cura, finestra.

**§17 Acceptance criteria.** Nessun criterio aggiunto per ora: O-7.

- **AC-56** — Sostituire "Static validation proves no tier 0 or tier 1 good depends transitively on a dungeon-only material." con: *(Decisione dell'utente, riserva 2 di R-001: con il tier sui bisogni, il criterio protegge i bisogni e non i singoli beni, così i cibi pregiati possono usare materiali di dungeon come vuole la motivazione di DEC-080.)*
  > Static validation proves that every tier 0 and tier 1 need can be met by at least one good, at or above the need's minimum grade, that depends transitively on no dungeon-only material.

- **AC-09** — Sostituire la tabella con: *(Decisione dell'utente, riserva 2 di R-001: senza i ruoli Subsistence e Luxury, i consumabili si classificano per il bisogno più urgente che soddisfano; la riga del tier 1 è nuova, e la sua intenzione è dell'utente.)*
  > | Good | X, first price move | Y, return to band |
  > |---|---|---|
  > | Consumable, tier 0 | short, within two settlement updates | long, up to the next harvest |
  > | Consumable, tier 1 | medium | within the season |
  > | Consumable, tier 2 | medium, bounded by travel | short |
  > | Input | medium | within the season |
  > | Durable | long | long |
  >
  > A consumable takes the row of the most urgent need it can meet.

**§18 Delivery phases**, tabella dei sottosistemi della fase 4 (R-052):
- *Production*: aggiungere FR-P-03a, FR-P-03c, FR-P-10a … FR-P-10e, FR-J-21, FR-J-22.
- *Needs*: aggiungere FR-P-03b, FR-D-03a.
- *Labour and income*: aggiungere "8a" alle fasi, e FR-J-19, FR-J-20.
- *Mobility*: aggiungere FR-D-04a (R-054).

**§19 Open items** — Le misure di P-17, P-63 e P-64 vanno rifatte con 11 righe per insediamento in produzione e 33 in CI, con 8 beni (P-09) e con il numero di mercati alimentari fissato dal contenuto.

### SIM-ECON

- **Fase 4** — Sostituire la cella "Does" con:
  > Labour market clears. Workers are assigned to activities up to building capacity, and to every crop stage active in this update; where labour is in excess, sellers are hired in proportion to what they offer. Labour still unhired is assigned to open fields (FR-P-10e); the rest is unemployment. Wages are paid.

- **Fase 5** — Aggiungere alla cella "Does":
  > Sowing consumes the planting good. Harvest reaps what labour allows from the area still standing, open fields included. Recipes never draw on a seed reserve.

  Aggiungere FR-P-10a, FR-P-10b, FR-P-10c, FR-P-10e agli "Implements".

- **Fase 6** — Sostituire con:
  > Each cohort meets its needs from its own holdings, by tier, lowest grade first, leaving its seed reserve untouched unless its hunger deficit exceeds P-66.

  Aggiungere FR-P-10d agli "Implements".

- **Fase 7** — Sostituire la cella "Does" con:
  > Unmet need becomes demand, capped by currency, which includes what phase 3 lent. Surplus, the seed reserve excluded, becomes supply. Short supply is allocated by ability to pay, the share of a cohort able to pay read from its wealth curve. Currency moves buyer to seller.

  Spostare il paragrafo che segue la riga della fase 7 ("Phase 7 dominates the cost of the update …") subito dopo la riga della fase 12, prima del paragrafo "Phases 3 and 8 are the two halves of credit", così la tabella non è più spezzata (X-8).

- **Fase 8a, nuova**, tra la fase 8 e la fase 9:
  > | 8a | Destitution | Cohort members whose wealth falls below the threshold enter the destitute row. Destitute members above the exit threshold leave for their destinations: for agricultural activities only in an update holding a crop's sowing date. | FR-J-19, FR-J-20 |

- **Fase 11** — Aggiungere alla cella "Does": *(R-054, R-055)*
  > Emigrants are drawn from the destitute row first and leave with their share under FR-D-04a, as migrants in transit; destitute emigrants raise danger on the link they take (FR-M-13).

  Aggiungere FR-M-13 agli "Implements".

- **Fase 12** — Aggiungere alla cella "Does":
  > Mestiere shares are realigned to the allocation vector for the activities that adjusted in this update.

  Aggiungere FR-J-03 agli "Implements".

- **Dated and annual steps**:
  - Sowing: "Commits land, labour, and the planting good to a crop for the season".
  - Harvest: "Opens the harvest window; each update reaps what harvest labour allows".
  - Allocation adjustment: sostituire "occupational shares move with it" con "each activity moves at its own rate (FR-P-15); occupational shares are realigned in phase 12, after the destitute leave in phase 8a".

- **Demand** — Sostituire la prima riga e la formula con:
  > A need is defined per goods class, minimum grade and tier, per person per day (FR-P-03b).
  >
  > ```
  > need        = perCapitaNeed(class, grade, tier) × count
  > unmet       = max(0, need − value of own holdings meeting it)
  > affordable  = currency / price per unit of value, currency including what phase 3 lent
  > demand      = min(unmet, affordable)
  > ```

  Nella tabella dei tier, aggiungere sotto: "Tiers belong to needs, not goods."

  Nel paragrafo **Rationing**, sostituire "within a tier, never across tiers" con "within a tier, never across tiers, and the share of a cohort able to pay is read from its wealth curve".

  Nel paragrafo **Hunger**, aggiungere: "The deficit is cohort state (FR-D-03a), and the seed reserve reads it (FR-P-10d)."

  Aggiungere il paragrafo: *(R-057, R-058)*
  > **Choice within a class.** Needs add (FR-P-03b). A cohort meets them tier by tier from its own holdings, lowest grade first, and buys what remains of each at the lowest price per unit of value among the goods that meet it. With the currency left once tiers 0 and 1 are met, and before tier 2 takes its fraction, members replace units of a lower grade with units of a higher grade that meet the same need, the share of members able to pay for each grade read from the wealth curve. In rationing, demand that meets a need comes before demand that only raises a grade. Demand rationed out of one good falls, within the update, to the other goods that meet the same need, cheapest per unit of value first; the order in which a class's markets clear is derived from grade, price and good id.

- **Spoilage and integer rounding** — Aggiungere alla regola generale:
  > The carry stays with its row and is cleared when the row's count reaches zero. Splits made within one update, such as gleaning shares or the destinations of the destitute, use largest remainders with ties by id and leave no carry.

- **What the sequence does not yet cover**, punto 2 — Aggiungere: "The seed reserve of FR-P-10d is a forced reserve, not a choice: it holds next season's seed and nothing beyond it." Punto 5 — Sostituire con: "**Per-capita need values.** The needs table has a shape per class, grade and tier and no numbers."

### SIM-STATE

- **Cohort** — Togliere la riga `wealthSpread`. Nuove righe:

| Field | Type | Notes |
|---|---|---|
| hungerDeficit | fixed | per member, FR-D-03a; read by mortality and by the seed reserve; averaged by count when members arrive |
| transitionRemainder | fixed | carry of the share entering or leaving destitution; cleared when the row empties |
| sownLand | int[] | per crop; set at sowing, FR-P-10a |
| landInField | int[] | per crop; falls through the harvest window, FR-P-10b |
| condition | fixed[] | per crop, 0–20000, FR-P-10c |
| advance | int64 | owed against the standing crop, FR-C-25; not included in debt |

- **Cohort**, intestazione: sostituire "Roughly 9 rows per settlement at Phase 4" con "Roughly 11 rows per settlement at Phase 4, the destitute and day-labourer rows included".

- **Static data, not state** — Aggiungere "goods classes, needs table, wealth curves".

- **Transient** — Nella riga `kind`, aggiungere `migrants` all'elenco. Sotto la tabella, prima del paragrafo sui transient campionati: *(R-054)*
  > Migrants are a scheduled transient. Beside the fields above they carry a count, the key of the row they left and its hunger deficit (FR-D-03a); their currency is one integer in smallest units, as a cohort's (FR-C-13), not four denominations. A-02 counts it and A-04 their goods.

- **Invarianti** — Nuovi:

| # | Assert |
|---|---|
| A-16 | For every cohort and crop, landInField ≤ sownLand; for every settlement, sownLand summed over its cohorts and crops ≤ land |
| A-17 | Destitute rows hold no buildings and no durables |

### SIM-LOOPS

- **R2** — In B2, sostituire "Allocation moves a capped fraction per year toward the best return." con:
  > Allocation moves a capped fraction per year toward the best return, for activities that carry no rate of their own. Day labour carries its own and is held by its wage instead.

- **R5 — Famine** — Aggiungere dopo B5b:
  > Hunger → seed eaten → less land sown → smaller harvest → deeper hunger, for years.
  > **B5c — Seed protection.** The reserve is untouched until the hunger deficit exceeds P-66, and is released only to hold it there.
  > **B5d — Imported seed.** Grain imported over the trade graph is also seed.

  Sostituire la riga *FR-D-03, FR-D-05.* con:
  > *FR-D-03, FR-D-05, FR-P-10a, FR-P-10d. Tuned by P-66 against the onset of hunger mortality.*

- **R7** — In B7, sostituire "so the loop cannot reach the people who grow the food" con:
  > so the loop reaches the people who grow the food only once they eat their seed reserve (B5c).

- **Nuovo anello, dopo R7:**
  > ### R12 — Destitute labour
  > More destitute → more labour offered → lower wage → more members below the threshold → more destitute.
  > **B12a — Day labour leaves.** A lower wage drives the day-labourer share down under its own rate, and labour offered falls.
  > **B12b — The destitute emigrate.** Emigration draws on the destitute row first, so those a falling wage pushes below the threshold are the first to leave, and the labour they offered leaves with them. They leave as refugees and raise danger on the links they take, which feeds R4. *(R-054, R-055)*
  > *FR-J-19, FR-J-21, FR-C-05, FR-D-04a, FR-M-13, DEC-090, DEC-096.*
  >
  > B12a fails once the day-labourer share is near zero. B12b acts only where a neighbour is more attractive by more than the hysteresis band of FR-D-09. In a region poor throughout nobody leaves, and the loop stops at the lower clamp of the wage (DEC-045), which bounds it without braking it, and at hunger (R5). *(R-055: sostituisce "B12b is the brake that holds")*

- **Balancing loops that stand alone** — Aggiungere:
  > | Day labour against wage | a high wage draws day labourers, which lowers it | FR-J-21, FR-C-05 |

- **Gate check** — Sostituire "Met for R1, R2, R4, R5, R6, R7" con "Met for R1, R2, R4, R5, R6, R7, R12". B12b è scritto nello stesso lotto (PD-13).

- **Open** — Aggiungere:
  > 4. Full care under DEC-094 raises yield per unit of land up to twice. B1 and B3a assume it fixed; recheck both before care is enabled.

- **R4** — *(R-055)* Aggiungere, dopo la riga dell'anello:
  > Poverty is the destitute row (FR-J-19). The displaced people are those of it who emigrate (FR-D-04a, B12b), and each raises danger on the link it takes.

  Sostituire la riga *FR-J-16, FR-C-08, FR-M-13, FR-M-14. Tuned by P-40 and P-41.* con:
  > *FR-J-16, FR-C-08, FR-M-13, FR-M-14, FR-D-04a. Tuned by P-40 and P-41.*

### SIM-OBS

- **OBS-3**, "Served by" — Aggiungere FR-P-03a, FR-P-03b, FR-J-19 a FR-J-22, FR-P-10a, FR-P-10d, FR-P-10e, FR-D-03a, DEC-090, DEC-091, DEC-092, DEC-095, DEC-097, DEC-098, e DEC-099 se adottata. *(L'aggiunta è dell'utente: non cambia l'osservabile, ma tocca SIM-OBS, che per la riserva 3 di R-001 il decider non modifica da solo.)*

---

## Punti aperti

| # | Questione | A chi | Note |
|---|---|---|---|
| O-5 | Cosa portano con sé i migranti | Decider | **Chiuso da R-054.** La quota media della riga: tutta la valuta, beni e durevoli fuori dalla riserva fino a P-72 di peso per membro; in transito sono un transient programmato, contato da A-02. Il candidato (vendita prima della partenza) è scartato |
| O-6 | Gli indigenti che emigrano sono la "displaced population" di FR-M-13? | Decider | **Chiuso da R-055.** Sì: l'emigrazione pesca prima dagli indigenti, che in viaggio sono famine refugees e alzano il pericolo del link di P-40 ciascuno. R4 ha la sua sorgente |
| O-7 | Criteri di accettazione | Decider | Candidati: una carestia con seme mangiato dura più di un anno; la ripresa rurale è più lenta di quella urbana; con P-67 = 0 il seme liberato non compare mai nell'offerta; un campo libero viene mietuto da chi è senza impiego; la quota dei braccianti sale durante la finestra di raccolto; dentro un mestiere i membri più poveri mangiano grado più basso dei più ricchi |
| O-9 | Regola economica per la cura oltre il normale | Decider | Candidato: la coorte assume lavoro di crescita finché il grano in più, al prezzo atteso, supera il salario |
| O-12 | Ricaduta immediata di chi esce dagli indigenti | Decider | Senza cancello del credito la frena solo P-69. Se le misure mostrano un ping-pong, un tempo minimo di permanenza come per la migrazione |
| O-13 | Somma dei bisogni e piatto secondo la ricchezza (PD-15, PD-16) | Decider | **Chiuso da R-057 e R-058.** Testo in DEC-099, FR-P-03b, FR-C-04 e SIM-ECON §Demand |
| O-14 | Produzione domestica (PD-17) | Decider | In questo lotto o nel successivo; analisi in PD-17 |
| O-15 | Ruoli di FR-P-03 e conservazione nelle ricette (PD-14) | Decider | **Chiuso da R-056.** Il record dice solo se il bene è durevole; consumabile e input derivano da bisogni e ricette, e il grano è entrambi. La conservazione corre lungo la catena: ogni bene porta il valore alimentare incorporato, il minimo che una ricetta consuma per produrlo. Resta all'utente in quali unità misura una sottoclasse (FR-P-03a) |
| O-8 | Dati storici di riferimento | Utente | Rese, forma delle distribuzioni di ricchezza e valori nutrizionali citati in sessione non sono verificati; servono a tarare il contenuto statico |
| O-11 | Colture e cibi oltre il grano | Utente | Contenuto statico (K-01, K-04, R-050); con PD-14 entrano come nuovi beni della classe cibo |

Il **contenuto statico** elencato sotto §16 (mestieri indigente e bracciante, curve e σ, classi, valori, gradi, colture) resta all'utente per R-050. I **valori numerici** marcati TBD, comprese le quantità della tabella dei bisogni, seguono R-050: li propone l'implementatore, li numera il decider.

Chiusi: O-1, O-2, O-3, O-4 (revisioni 4–5), O-10 (il grano è cibo di grado 0).

## Difetti trovati, indipendenti da queste decisioni

| # | Difetto | Stato |
|---|---|---|
| X-1 | Il deficit di fame non è nello stato | Risolto da FR-D-03a |
| X-2 | Limite della domanda diverso tra FR-C-04 e SIM-ECON | Risolto da PD-11, anche nella cella della fase 7 |
| X-3 | DEC-047 "once per sowing" contro FR-P-15 | Aperto; coincide con Z-05 |
| X-4 | `wealthSpread` senza definizione | Risolto da PD-10 |
| X-5 | Nessuna regola su cosa portano i migranti | Risolto da FR-D-04a (R-054) |
| X-6 | Effetti di siccità e gelo sul raccolto non specificati | FR-P-10c offre l'aggancio; il resto è K-10 |
| X-7 | FR-C-25 nomina un campo che non esiste | Risolto dal campo `advance` |
| X-8 | Tabella delle fasi spezzata in SIM-ECON | Risolto nelle modifiche alla fase 7 |
| X-9 | `main` indietro rispetto al SIM-REQ caricato | Risolto: `54f3180` |
| X-10 | FR-C-05 parla di terra posseduta | Risolto dal nuovo FR-C-05; coincide con Z-06 |
