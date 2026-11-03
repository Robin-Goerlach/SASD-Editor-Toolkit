# Historische Nachverfolgung – SASD Editor Toolkit

[English](../en/Historical-Traceability.md) · [Lastenheft](Lastenheft.md) · [Pflichtenheft](Pflichtenheft.md) · [Architektur](Architektur.md)

**Stand:** 03.11.2026. Dieses Dokument verknüpft die historische *Inspiration* mit den Anforderungen und der eigenständigen modernen Architektur. Es ist **keine** Behauptung vollständiger Funktionsgleichheit mit der ursprünglichen Turbo Editor Toolbox und keine Abnahme.

## 1. Zweck und Rechts-/Produktgrenze

SASD Editor Toolkit entsteht als **eigenständiger Baukasten** für Textbearbeitung. Das Handbuch zur Turbo Editor Toolbox Version 1.0 (1985) dient als historische Referenz für Konzepte, Befehlskategorien und Bedienideen, **nicht** als Quellcode- oder Designvorlage zur wortgetreuen Übernahme.

- Keine Kopie von Pascal-Quellcode, Scans, Logos, Originalgrafiken oder langen Handbuchpassagen.
- Keine offizielle Zugehörigkeit zu Borland behaupten.
- Die Produktnamen „FIRST-ED“ und „MicroStar“ kennzeichnen historische Referenzideen bzw. Samples, nicht garantierte binäre oder UI-Kompatibilität.
- Eigenständige Anforderungen und Implementierungsentscheidungen verbleiben bei SASD.
- Diese Traceability ist keine Rechtsberatung; Lizenz-/Markenprüfung bleibt ein eigener Freigabepunkt.

**Entscheidend:** Das andere Projekt **SASD Editor Toolbox** und eine spätere Migration davon sind *nicht* Teil dieser Nachverfolgung. Das Toolkit wird hier ausschließlich nach seiner ursprünglichen eigenständigen Zielsetzung beurteilt.

## 2. Welche Quellen gelten?

| Quelle | Rolle | Verbindlichkeit |
|---|---|---|
| Historisches Handbuch *Turbo Editor Toolbox v1.0 – Owner's Handbook* | historischer Funktions-/Terminologiehintergrund | Referenz, nicht zur Kopie freigegeben |
| [Lastenheft](Lastenheft.md), insbesondere Kapitel 25–27 | Anforderungskategorien, historische Kapitel, Routineninventur | fachliche Ausgangsanforderungen |
| [Pflichtenheft](Pflichtenheft.md), insbesondere Kapitel 38–45 | 208 Anforderungszuordnungen, 123 Routinen und 46 globale Zustände | technische Zielzuordnung |
| \`docs/adr/\` | angenommene Architekturentscheidungen | verbindlich gemäß AGENTS |
| \`docs/spec/\` | plattformunabhängiges beobachtbares Verhalten | Vertragsgrundlage |
| Quellcode und Tests auf \`main\` | was aktuell tatsächlich existiert | Implementierungsnachweis, **keine** automatisch bestandene Abnahme |
| \`docs/ai/CURRENT-STATE.md\` / \`NEXT-WORK.md\` | datierter Zustand und Entwicklungsplanung | Orientierung, nicht Releasefreigabe |

Die im Pflichtenheft als „Umsetzung“ bezeichneten Spalten beschreiben häufig die **vorgesehene Realisierung**, nicht den Nachweis einer bereits fertiggestellten Funktion.

## 3. Historische Kapitel und moderne Zielbereiche

| Historisches Kapitel | Konzept | Moderne Verantwortlichkeit |
|---|---|---|
| Einleitung | Editoren als Baukasten | unabhängiges Produkt, Core-API, Host |
| 1 | Zeichen, Zeilen, Editorbegriffe | \`TextDocument\`, \`ITextBuffer\`, View |
| 2 | Editorentypen und Speicherarten | bewusste Produkt-/Performancegrenzen |
| 3 | Zeilenarray, fester Buffer, verkettete Liste | \`ITextBuffer\`, \`LineTextBuffer\`, spätere Engines |
| 4 | Einfacher Editor | FIRST-ED-Referenzsample |
| 5 | FIRST-ED, Initialisierung, Hooks | Hostkomposition, Standardcommands |
| 6 | UserCommand und Präfixbefehle | Registry, Dispatcher, spätere Tastaturprofile |
| 7 | MicroStar und Menüs | spätere Menüs, Host-UI, optionale Demo |
| 8 | Popups, Fehler, Status, Dirty, Druck | Hostdienste, Core-Ergebnis, optionaler Printdienst |
| 9 | Text- und Fensterzustände | Dokument, \`EditorViewState\`, Workspace |
| 10 | Eingabeschleife und Hintergrundarbeit | asynchrone Tasks, Cancellation, Host-Eventloop |
| 11 | Bildschirm, Farbcodes, Updates | Adapter-Rendering und Invalidierung |
| 12 | Editierbefehle, Dateien, Fenster, Blöcke | Commands, Text/Storage/Search/Views |
| 13 | Overlaytechnik | **nicht übernommen**; .NET-Assembly/Runtime |
| 14 | Einbettung in Anwendungen | Core-API, WinForms-Control, künftige Adapter |
| 15 | Modul-/Dateiübersicht | modulare Verantwortung statt Pascal-Includes |
| 16 | Konstanten | typisierte Modelle, Optionen, Status |
| 17 | Typen | Dokument/Position/Buffer/View |
| 18 | globale Variablen | gekapselte Zustände statt Prozessglobaler |
| 19 | Prozeduren/Funktionen | Inventar mit eigener moderner Zuordnung |

Die historische Zuordnung dient zur **Vollständigkeitskontrolle**. Sie bedeutet nicht, dass jede historische Datenstruktur identisch in C# existieren muss.

## 4. Abdeckungsmatrix nach Fachbereich

| Bereich | Historische Idee | SASD-Ziel | M1-Beobachtung | Weiteres Vorgehen |
|---|---|---|---|---|
| Text/Buffer | Zeilen, Puffer, Textstrom | UI-unabhängiger Textkern | \`LineTextBuffer\`, \`TextDocument\` vorhanden | Unicode und Performance härten |
| Cursor/Navigation | Links, rechts, Wort, Seite, Anfang/Ende | durch Command-IDs ausführbar | Basisnavigation vorhanden | Unicode-/Wort-/Seitenfälle ergänzen |
| Bearbeitung | Einfügen, Löschen, Umformung | atomare Commands | Insert, Delete, NewLine vorhanden | Auswahl-/Blockoperationen M1c/M2 |
| Undo/Dirty | Undo und Änderungsflag | Transaktionen, Savepoint | einfache History, Textsnapshot | Transaktionen/Sicherheit M2 |
| Suche/Replace | Find, Replace, Optionen | Core-Suche/-Ersetzung | Find vorhanden | Replace und Selection M1c |
| Datei | Lesen, Schreiben, Bestätigungen | sichere Streams/Files, Host-Prompts | Encoding/BOM und temporäres Save vorhanden | Konflikte, Binärwarnung, Recovery |
| Ansichten | verknüpfte Fenster | unabhängige Views eines Dokuments | Workspace-/Viewmodell vorhanden | vollständige Host-/Split-Tests |
| Menüs/Tasten | UserCommand, Ctrl-K/O/Q | Registry und serialisierbare Profile | Registry; begrenzte Tastensteuerung | Profile/Chords M2 |
| Status/Fehler | UserStatusLine, UserError | hostaustauschbare Dienste | Hoststatus, \`CommandResult\` | strukturierte Fehler/Presenter |
| Hintergrund | UserTask, Scheduler | Task/Cancellation | kein eigener DOS-Scheduler | Hostdienst bei echtem Bedarf |
| Darstellung | Bildschirm, Farben, Row Updates | WinForms-Renderer, später Adapter | eigene EditorSurface | DPI, IME, Barrierefreiheit |
| Historische Technik | Pointer, Overlay, BIOS | **bewusst nicht übernehmen** | kein DOS-/BIOS-Design | Clean-Room-Grenzen beibehalten |

**Statusregel:** „vorhanden“ bedeutet Code existiert. Nur ein konkreter Test/Abnahmebeleg kann „bestanden“ begründen.

## 5. Historische Hooks: fachliche Entsprechung

| Historische Schnittstelle | Moderne Richtung | Status |
|---|---|---|
| \`UserCommand\` | Command-Registry und später Interceptors/Key-Sequences | **teilweise** |
| \`UserError\` | strukturierte Core-Fehler und Host-Presenter | **geplant**, erste CommandResult-Basis |
| \`UserStatusLine\` | Hoststatus ohne Core-UI-Abhängigkeit | **teilweise** im FIRST-ED-Host |
| \`UserReplace\` | Replace-Strategie und optionaler Prompt | **offen** |
| \`UserTask\` | hostgesteuerte Tasks/Cancellation | **später** |
| \`MakeWindow\`, \`RestoreWindow\` | Dialog-/Overlayservice im Adapter/Host | **teilweise** Hostdialoge |
| \`PulldownMenu\`, \`InitMainMenu\` | Menü auf Command-IDs | **teilweise** |
| \`SecondChar\` | Keyboard-Sequence-Resolver | **offen** |
| \`PrintNext\` | optionaler Printservice | **später** |
| \`SpellingCheck\` | externer Sprachdienst | **später, optional** |

Das vollständige Inventar weiterer Hooks, historischer Module, Konstanten und Zustände ist in den Kapiteln 26–27 des Lastenhefts und 40–44 des Pflichtenhefts enthalten. **Dieses Dokument kopiert die dortigen 123 Routinen nicht erneut**, damit nicht drei voneinander abweichende Inventarstände gepflegt werden.

## 6. IDs, Verifikation und Abnahme

- **208** formale Produkt-, Funktions- und Qualitätsanforderungen erscheinen in Kapitel 38 des Pflichtenhefts.
- **15** Use Cases (\`UC-01\` bis \`UC-15\`) haben eine vorgesehene technische Realisierung.
- **15** Akzeptanzkriterien (\`AK-001\` bis \`AK-015\`) besitzen vorgeschlagene Testverfahren.
- **123** historische Routinen sind im Lastenheft inventarisiert und im Pflichtenheft auf moderne Komponenten oder bewusste Nichtübernahme abgebildet.
- **46** historische globale Zustände besitzen eine moderne Verantwortlichkeitszuordnung im Pflichtenheft.

Die Zählungen beziehen sich auf **Dokumentationsinventare**, nicht auf 208 bestandene Tests, 123 vorhandene C#-Methoden oder freigegebene Version 1.0.

**Prüfprozess pro historischer Fähigkeit:** Quellkapitel → Anforderungs-ID → Zielkomponente/Command-ID → ADR/Spec → konkreter Code → automatisierter Test → manueller Nachweis (falls UI) → Meilensteinabnahme.

## 7. Nichtübernahmen und Erweiterungen

**Bewusst nicht portiert:** Videospeicher-Manipulation, Assembler-Row-Updates, BIOS, feste 80×25-Darstellung als Produktgrenze, DOS-Scheduler/Polling, globaler Typeahead-Ringpuffer, globale Pointerlisten, Overlays/Chain-Dateien und 255-Zeichen-Zeilenlimit.

**Bewusst später:** komplette historische Tastensequenzen, erweiterte MicroStar-Funktionen, Druck/Spelling, Makros und zusätzliche UI-Adapter. Bei Makros/Skripten gilt eine zusätzliche Sicherheitsentscheidung; historische Kompatibilität **rechtfertigt keine automatische Ausführung**.

**Moderne Erweiterungen:** Unicode, sichere Dateizustände, Portierbarkeit, Testbarkeit, lokalisierbare UI und zusätzliche Bearbeitungsdienste sind eigenständige SASD-Anforderungen.

## 8. Pflege-Regeln

Bei einem neuen historisch inspirierten Slice:

1. Anforderungs-ID aus Lastenheft/Pflichtenheft nennen.
2. Moderne Semantik in \`docs/spec/\` beschreiben; historische Originaltechnik nicht kopieren.
3. Code und Testbeleg verlinken, sobald vorhanden.
4. Status nur mit Nachweis ändern; „geplant“ darf nicht automatisch zu „bestanden“ werden.
5. Falls ein historischer Name nicht 1:1 übernommen wird, die moderne Zuordnung begründen.
6. Deutsche und englische Dokumente gemeinsam aktualisieren.

Für formale Tests siehe [Testhandbuch](Testhandbuch.md); für die M1-/M2-Grenze siehe [Roadmap](ROADMAP.md).
