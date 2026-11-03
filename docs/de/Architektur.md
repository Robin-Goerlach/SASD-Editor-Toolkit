# Architektur – SASD Editor Toolkit

[English](../en/Architecture.md) · [Dokumentationsindex](README.md) · [Pflichtenheft](Pflichtenheft.md) · [Roadmap](ROADMAP.md)

**Status:** Architekturgrundlage für das eigenständige Produkt; Ist-/Soll-Abgleich, keine Release-Freigabe.  
**Stand:** 03.11.2026, geprüft gegen \`main\` (\`820dc9f\`).  
**Normative Grundlage:** \`docs/adr/\`, \`docs/spec/\`, Lastenheft und Pflichtenheft. Bei Widersprüchen gilt die Autoritätsreihenfolge aus \`AGENTS.md\`.

> **Produktgrenze:** Das SASD Editor Toolkit ist ein **eigenständiger Editor-Baukasten** mit einem C#/.NET-Referenzkern. Die SASD Editor Toolbox ist **weder Voraussetzung noch Quellcodebasis** für diese Architektur. Eine denkbare spätere Produktintegration ist ausdrücklich **nicht Bestandteil** dieser Entwurfsgrundlage. Das eigenständige SASD UI Toolkit ist ebenfalls keine Pflichtabhängigkeit.

## 1. Zielbild und Entwurfsprinzipien

Das Toolkit stellt wiederverwendbare Textbearbeitung zur Verfügung, ohne den fachlichen Zustand an WinForms oder eine konkrete Anwendung zu binden. Die erste sichtbare Anwendung ist **Modern FIRST-ED**; sie dient als Integrationsnachweis, nicht als alleinige Produktform.

**Leitplanken:**

1. **Ein fachlicher Textzustand:** \`TextDocument\` und \`ITextBuffer\` sind maßgeblich. Der UI-Adapter führt keine zweite Dokumentkopie.
2. **Dokument und Ansicht trennen:** Mehrere Ansichten können langfristig dasselbe Dokument mit unabhängigem Caret-, Auswahl- und Scrollzustand darstellen.
3. **Befehlsorientierte Bearbeitung:** Eingaben aus Tastatur, Menü oder API sollen über stabile Command-IDs zu denselben Operationen führen.
4. **Sichere Datenverarbeitung:** Text, Pfade, Konfigurationsdaten und Erweiterungen gelten als nicht vertrauenswürdig. Ein Abbruch darf keine unerwarteten Datenverluste erzeugen.
5. **Pragmatische Modularität:** Zunächst zwei Produkt-Assemblies (Core und WinForms), ein Sample und Tests. Zusätzliche Assemblies nur für belegte Trennungsbedarfe.
6. **Portierbares Verhalten, keine erzwungene Einheits-ABI:** Sprachneutrale Spezifikationen und Testvektoren; spätere Java- oder C++-Implementierungen verwenden ihre natürlichen Speicher- und Laufzeitmodelle.
7. **Verständlichkeit vor Optimierung:** Invarianten, Zustandsübergänge und Verträge klar dokumentieren; Optimierung nach Messung.

## 2. Kontext und Bausteine

\`\`\`mermaid
flowchart TD
  User[Anwender] --> Host[Host / Modern FIRST-ED]
  Host --> UI[WinForms-Adapter]
  Host --> Storage[Plattformneutrale Datei-/Streamdienste]
  UI --> Core[Editor Core]
  Storage --> Core
  Core --> Spec[Sprachneutrale Verträge]
\`\`\`

Die Pfeile zu den Spezifikationen bedeuten **Konformität**, keine Laufzeitabhängigkeit. Die Datei-/Streamdienste gehören gegenwärtig zum Core-Projekt; die Host-Anwendung steuert Dateiauswahl und Bestätigungen.

### 2.1 Physische Projekte

| Baustein | Aufgabe | Stand |
|---|---|---|
| \`Sasd.EditorToolkit.Core\` (\`net10.0\`) | Dokumente, Textpuffer, Views/Workspace, Befehle, Suche, Undo, Speicherung, Einstellungen | **Implementiert als M1-Grundlage** |
| \`Sasd.EditorToolkit.WinForms\` (\`net10.0-windows\`) | \`SasdEditorView\`, \`EditorSurface\`, Rendering und Eingabeverarbeitung | **Implementierter erster Adapter**, noch keine vollständige Abnahme |
| \`Sample.FirstEd.WinForms\` | Menüs, Toolbar, Suchdialog, Datei-/Schließdialoge und Status | **Lauffähiges Referenzsample**, M1-Funktionsumfang |
| \`Sasd.EditorToolkit.Core.Tests\` | xUnit-Tests für Core und mehrere Randfälle | **Vorhanden** |
| WPF, Web, Java, C++, Python | Alternative Adapter oder eigenständige Portierungen | **Nicht implementiert und nicht für M1 freigegeben** |

**Verbotene Abhängigkeiten:** Core → WinForms/WPF/Web; Core → Sample; Produktassembly → Testassembly; zyklische Abhängigkeiten. Der Host darf Core und Adapter referenzieren. Eine spätere native UI darf nicht unbemerkt Speicher- oder Threading-Regeln des C#-Core nach außen lecken.

### 2.2 Logische Verantwortlichkeiten

| Bereich | Eigentümer | Keine Verantwortung für |
|---|---|---|
| Textmodell / \`ITextBuffer\` | Core | Pixel, Fokus, Dateiauswahldialoge |
| Dokument / Undo / Dirty | Core | MessageBox, Autosave-Verzeichniswahl |
| ViewState / Workspace | Core | WinForms-Controls und Layoutcontainer |
| Command-Registry / Dispatcher | Core | Tastatur-Scancodes und Menütexte |
| Suchdienst | Core | Suchdialog oder Benutzerbenachrichtigung |
| Encoding / Stream / Datei | Core-Infrastruktur | Bestätigung durch Benutzer, Zugriffsrichtlinie des Hosts |
| Rendering / Eingabe | WinForms-Adapter | Eigene Text- oder Undo-Wahrheit |
| Workflow / Dialoge | Host/Sample | Bibliotheksinterne Geschäftslogik |

**Wichtig:** \`Application Services\` sind zunächst eine **logische Schicht innerhalb des Core**, keine zusätzliche Projektdatei. Eine explizite Aufspaltung verlangt messbare Vorteile und eine ADR.

## 3. Struktur des tatsächlichen M1-Codes

- \`TextDocument\` besitzt \`ITextBuffer\`, Metadaten und \`UndoManager\`; \`IsDirty\` vergleicht derzeit den vollständigen Text mit einem gespeicherten Snapshot.
- \`LineTextBuffer\` speichert logische Zeilen samt \`LineEndingKind\`. Seine Implementierung setzt beim Ändern die komplette Textdarstellung neu zusammen. Das ist **keine** performante Large-File-Strategie.
- \`EditorViewState\` hält Caret, Auswahl, Scroll- und einige Bearbeitungsoptionen. \`EditorWorkspace\` kennt mehrere Dokumente/Views; das FIRST-ED-Sample demonstriert derzeit **keine** vollständig integrierte Split-View-Bedienung.
- \`EditorCommandRegistry\` registriert einen kleinen Satz realer Commands; \`EditorCommandDispatcher\` prüft die Ausführbarkeit und ruft den passenden Handler auf. \`Search.Replace\` ist **noch kein** implementierter Standardcommand.
- \`TextSearchService\` arbeitet mit einem vollständigen Textsnapshot und bietet aktuell literale Suche, Richtung, \`StringComparison\` und Wrap. Abbruchfähiges Ersetzen und Regex sind **nicht** Teil des heutigen Codes.
- \`FileDocumentStorage\` unterstützt Streams, BOM-Erkennung, Encoding und einen temporären Dateipfad beim atomaren Speichern. Es liest momentan alle Bytes in den Speicher, erkennt Binärverdacht primär an NUL-Bytes und prüft externe Dateikonflikte noch nicht vollständig.
- \`EditorSurface\` rendert selbst, verarbeitet einen begrenzten Tastensatz und besitzt einen ViewState. Die im Pflichtenheft beschriebenen serialisierten Tastaturprofile, ein allgemeines Host-Prompt-System und erweiterte Accessibility sind **Zielarchitektur**.
- \`EditorSettings\` enthält einige Anfangswerte; die JSON-Schemata unter \`docs/spec/\` sind noch nicht als vollständige Konfigurationsinfrastruktur verfügbar.

Die Übersetzung des Pflichtenhefts enthält langfristige Zielverträge. **Konzeptionelle C#-Beispiele im Pflichtenheft sind kein Nachweis, dass diese Interfaces bereits im Repository existieren.**

## 4. Text, Positionen und Änderungen

\`\`\`mermaid
sequenceDiagram
  participant H as Host / Adapter
  participant D as Command Dispatcher
  participant C as Core Command
  participant Doc as TextDocument
  participant Buf as ITextBuffer
  H->>D: Command-ID + Kontext
  D->>C: CanExecute / ExecuteAsync
  C->>Doc: Insert / Delete / Replace
  Doc->>Buf: atomare Änderung
  Buf-->>Doc: TextChangeSet + Changed
  Doc-->>H: neuer Dokumentzustand
  H->>H: Anzeige aktualisieren
\`\`\`

**Verbindliche fachliche Regeln:**

- Ein leeres Dokument besitzt mindestens eine ansprechbare leere logische Zeile.
- \`TextPosition\` verwendet nullbasierte Zeilen und Spalten; \`TextRange\` ist halb-offen.
- In .NET sind die aktuellen Spalten UTF-16-Code-Unit-Indizes. Die Spezifikation muss für Portierungen ausdrücklich zwischen *Code Units*, *Unicode Scalars*, *Graphemen* und *visuellen Spalten* unterscheiden.
- Textänderungen müssen entweder als **vollständig erfolgreich** oder als **nicht angewendet** beobachtbar sein; keine partiell mutierten Dokumente nach Fehlern.
- Das Textmodell bewahrt CRLF, LF, CR und einen fehlenden Abschlusszeilenumbruch, soweit der gewählte Speicherpfad es unterstützt.
- Ein Buffer-Change erzeugt eine neue Version. Ansichten aktualisieren ihre Darstellung anhand von Dokumentänderungen, ohne den Text zu kopieren.
- Carets und Selektionen dürfen bei Benutzeroperationen Unicode-Surrogatpaare und zusammengehörige Graphemfolgen nicht absichtlich zerreißen. Die aktuelle Implementierung erfüllt dies noch nicht umfassend; Tests sind verbindlich nachzuziehen.

**Zu lösende Vertragslücke:** \`TextDocument.Buffer\` ist öffentlich direkt mutierbar, obwohl die Dokumentmethoden zusätzlich Undo registrieren. Für M2/M3 ist eine eindeutige **Mutationsgrenze** erforderlich (z. B. nur über Dokumentoperationen bzw. intern kontrollierte Transaktionen), sonst können Änderungen die Undo-Historie umgehen. Die endgültige öffentliche API gehört in eine eigene ADR, nicht in einen stillschweigenden Umbau.

## 5. Undo, Dirty State und Mehransichtsfähigkeit

**Heute:** \`UndoManager\` verwaltet Stacks von \`TextChangeSet\`; \`TextDocument\` führt einfache inverse Operationen aus. Das ist eine nützliche M1-Basis, aber noch kein vollwertiges Transaktionsmodell mit Gruppierung, Speicherbudget und stabilen Ankern.

**Ziel:** Jede fachliche Änderung läuft durch einen dokumentbezogenen Transaktionspfad. Undo/Redo ist unabhängig von der Zahl der Views. Der Savepoint definiert den zuletzt erfolgreich gespeicherten Inhalt; eine Änderung nur in der Darstellung setzt das Dokument nicht auf „dirty“. Views besitzen ausschließlich ihre lokalen Zustände.

**Lebensdauerregel:** Ein Dokument darf nicht verworfen werden, solange es aktive Views oder eine Host-Referenz besitzt. Das Schließen des letzten Views führt bei geändertem Inhalt zu einer Hostentscheidung, nicht zu einer Core-\`MessageBox\`.

**Reihenfolge der Härtung:** Mutationsgrenze → konsistente Undo-Transaktionen → Anchor-/Selection-Transformation → Mehransichtstests → Performance. Diese Reihenfolge ist ein Architekturvorschlag, keine bereits freigegebene Implementierung.

## 6. Befehle, Hostdienste und Fehlerschnittstellen

Der Command-Pfad soll lauten:

\`Eingabe oder API → Command-ID → Context → CanExecute → Handler → dokumentbezogene Änderung → Resultat → Hostdarstellung\`.

- Command-IDs bleiben lesbare und sprachunabhängige Strings (\`docs/spec/Command-Ids.md\`).
- **Ist-Zustand:** Registry, Dispatcher und \`CommandResult\` existieren. Unbekannte oder nicht ausführbare Befehle liefern derzeit \`NotHandled\`; eine allgemeine Interceptor-Pipeline, Versionierung von Hostkommandos und JSON-Tastaturprofile sind noch offen.
- **Ziel:** Strukturierte Ergebnisse mit stabilen Fehlercodes, ohne lokalisierte Meldungstexte als Maschinenvertrag. UI-Texte werden vom Host lokalisiert. Cancellation darf nicht nach bereits vollständig durchgeführten Mutationen so erscheinen, als wäre nichts geschehen.
- **Sicherheitsgrenze:** Ein Command ist **keine** Erlaubnis für Dateisystemzugriff, Netzwerk, Shell-Ausführung oder fremde Prozesse. Makros/Plugins brauchen eine separate Sicherheits-ADR.
- **Hostdienste nach Bedarf:** Clipboard, Datei- und Ordnerauswahl, Bestätigungen, Fehler-/Statusdarstellung, optional Logging und Autosave. Nicht jede geplante Schnittstelle sofort als Assembly oder Service Provider implementieren.

## 7. Speichern, Integrität und Fehlerfälle

| Schritt | Verbindliches Ziel | M1-Beobachtung |
|---|---|---|
| Öffnen | Encoding/BOM ermitteln, Binärverdacht melden, bei Fehler altes Dokument bewahren | Encoding/BOM und Ergebnisflag vorhanden; komplette Bytepufferung; kein vollständiger Host-Binärdialog |
| Bearbeiten | Nur explizite Mutation setzt Dirty/Undo | Einfacher Undo-Pfad vorhanden, direkte Buffer-Mutation noch möglich |
| Speichern | temporär im Zielordner schreiben, flush, atomar ersetzen soweit unterstützt, erst danach \`MarkSaved\` | Temporärdatei mit \`File.Move(overwrite: true)\` vorhanden; Garantie hängt vom Dateisystem ab |
| Konflikt | Änderungen durch andere Prozesse erkennen und Hostentscheidung einholen | Fingerprint/Conflict-Workflow noch ausstehend |
| Abbruch | Ursprungsdatei und Dokumentzustand schützen | Einzelfälle zu testen, besonders bei Fehler während Replacement |
| Wiederherstellung | explizite, geschützte Autosave-/Recovery-Snapshots | späterer Meilenstein |

**Entwurfsanforderung:** Der Host entscheidet über zulässige Pfade und bestätigt Datenverlust- oder Binärkonflikte. Bibliotheksdienste werfen keine UI-Dialoge auf. Temporärdateien müssen bei Fehlern bestmöglich bereinigt werden. Symlinks, Berechtigungen, konkurrierende Schreiber und Recovery-Verzeichnisrechte sind vor einer produktiven Freigabe explizit zu testen.

## 8. Adapter, UI und Accessibility

\`SasdEditorView\` bindet ein Dokument an \`EditorSurface\`; diese zeichnet aktuell Textzeilen, Zeilennummern, aktuelle Zeile, Auswahlhintergrund und Caret. Der Host hält Menüs und Dialoge.

**Dauerhafte Trennung:** Core entscheidet *was* ausgewählt oder bearbeitet ist; der Adapter entscheidet *wie* es gezeichnet, fokussiert und per IME oder Pointer bedient wird. Pixelmaße, Fonts und DPI gehören nicht in \`TextDocument\`.

Für die nächste Ausbaustufe: dokumentierte View-Anbindung, Berechnung sichtbarer Bereiche, konfigurierbare Farb-/Kontrastwerte, korrektes Redraw bei Änderungen, Unicode-/IME-Eingabe, Tastaturnavigation, Screenreader-Informationen und fokussicheres Caret. Keine zweite Textpufferimplementierung im Control.

## 9. Mehrsprachigkeit und Portabilität

Eine gemeinsame native C++-Bibliothek mit .NET-/Java-/Python-Adaptern wäre zum jetzigen Zeitpunkt **verfrüht**. Sie würde eine ABI, Ownership-Regeln, Fehlerkonventionen, Threads und eine Unicode-Konvertierungsschicht erzwingen, bevor ein stabiler fachlicher Vertrag vorliegt.

Stattdessen:

- \`docs/spec/\` definiert beobachtbare Semantik, Command-IDs, Eingabeformate und Conformance-Vektoren.
- C#/.NET bleibt die erste vollständige Referenz; WinForms ist nur ihr erster UI-Adapter.
- Spätere Java-/C++-Implementierungen dürfen idiomatische Garbage-Collection- oder RAII/Ownership-Modelle verwenden und wählen ihr internes Encoding selbst.
- Wo eine FFI/ABI später tatsächlich notwendig ist, erhält sie eine **explizite schmale, versionierte C-ABI** mit Ownership-, Lifespan-, Fehler- und Encoding-Regeln; das wird in einer gesonderten ADR entschieden.
- WPF/Web werden als Adapter beziehungsweise Bridges bewertet, nicht automatisch als weitere Core-Implementierungen.

## 10. Betriebssicherheit, Ressourcen und Observability

**Bedrohungsmodell:** bösartige oder beschädigte Dateien, exotische Unicode-Folgen, nicht beschreibbare Verzeichnisse, Dateiänderung zwischen Lesen/Schreiben, große Texte, ungewöhnliche Eingabesequenzen und möglicherweise untrusted Hosts.

**Verpflichtende Grenzen:** keine Ausführung von Dokumentinhalt; keine Secrets im Logging; kein verdeckter Netzwerkzugriff; keine stillschweigende Überschreibung erkannter Konflikte; nachvollziehbare Fehler und sichere Defaults.

**Leistungsstrategie:** M1 ist bewusst korrektheits- und lernorientiert. \`LineTextBuffer.GetText()\`, \`Length\`, Offset-Abbildungen und Mutationen sind derzeit teilweise vollständig text-/zeilenbasiert. \`TextDocument.IsDirty\` vergleicht Textsnapshots. Vor einem Large-File-Versprechen sind Lasttests, Benchmarks und ggf. Pufferindizes/Piece Table/Rope hinter \`ITextBuffer\` nötig. Keine Optimierung durch Umgehen von Undo- oder Event-Invarianten.

**Logging:** technisch hilfreiche Events ohne Dokumentinhalt; Pfade je Hostregel redigieren; Metriken wie Ladedauer/Größe nur bewusst und datensparsam erheben.

## 11. Qualitätsnachweis und Architektur-Gates

| Gate | Nachweis | Stand |
|---|---|---|
| Core ohne UI-Framework | Projekt-/Dependency-Check; später mechanischer Architekturtest | Grenze im vorhandenen Projekt sichtbar; automatischer Gate ausstehend |
| Deterministische Kernfunktionen | xUnit-Core-Tests | vorhanden |
| Gesamte Windows-Solution | \`scripts/verify-all.ps1\` und \`.github/workflows/ci.yml\` | vorhanden; tatsächliches CI-Ergebnis separat prüfen |
| Linux-/macOS-Core | \`scripts/verify-core.sh\` | Skript vorhanden |
| Datei-Integrität | Tests zu BOM, Zeilenenden, Fehler- und Dateispeicherung | teilweise vorhanden, Konflikt-/Crash-Tests fehlen |
| Unicode-Grapheme und IME | Core-Vektoren + manuelle Windows-Tests | unvollständig |
| Command-IDs / JSON-Profile | Schema-/Eindeutigkeits- und Conformance-Tests | vorgesehen |
| Release/API/NuGet | Pack-, ABI/API-, Consumer- und Lizenzprüfung | für M3 vorgesehen |

Ein grüner Build ist **kein** Beleg für Vollständigkeit, Accessibility oder Produktabnahme.

## 12. Architekturentscheidungen und gezielte nächste Arbeiten

Bereits akzeptiert: ADR-0001 bis ADR-0005 (C#-Referenz, sprachneutrale Konzepte, Repositorystrategie, M1-Scope, Unicode/Indexierung). Neue substanzielle öffentliche Verträge benötigen eine eigene Entscheidung.

| Priorität | Entscheidung bzw. Arbeit | Zeitpunkt |
|---|---|---|
| P1 | M1c \`Search.Replace\`, Auswahl-Editing und eindeutiges Caret-Verhalten fertigstellen | vor M2 |
| P1 | Core-/UI-Abhängigkeitsgrenzen und Command-ID-Eindeutigkeit mechanisch prüfen | nach M1c |
| P2 | Eigentums- und Mutationsmodell (\`TextDocument\` vs. \`ITextBuffer\`) als ADR festlegen | vor komplexem Undo |
| P2 | Savepoint, Undo-Transaktionen und Positionstracking spezifizieren | vor Mehransichtsabnahme |
| P2 | Hostentscheidung für Binärverdacht, Dateikonflikte und CloseGuard konkretisieren | vor V1 |
| P3 | Tastaturprofile, strukturiertes Error-Modell, Settings-Serialisierung | M2 |
| P3 | Performance-/Large-File-Strategie und Accessibility nach Metriken entscheiden | M3 |
| später | WPF/Web/Java/C++/FFI nur nach Bedarf und separatem Entwurfsentscheid | nach stabiler Referenz |

**Nicht in diesem Dokument beschlossen:** Neues Modul-/Paket-Splitting, eine C++-Interop-ABI, neue Laufzeitabhängigkeiten oder vorgezogene Erweiterungen. Diese Arbeit ist Architektur- und Dokumentationspflege, keine Änderung des Produktcodes.

## 13. Navigations- und Quellenverzeichnis

- [Lastenheft](Lastenheft.md) und [Pflichtenheft](Pflichtenheft.md): verbindliche Produkt- und Zielanforderungen.
- [Roadmap](ROADMAP.md): Meilensteine und Nachweisstand.
- [Architekturentscheidungen](../adr/): akzeptierte technische Grundsatzentscheidungen.
- [Editor Model](../spec/Editor-Model.md), [Text Buffer Behavior](../spec/Text-Buffer-Behavior.md), [Command IDs](../spec/Command-Ids.md): normative, sprachneutrale Verträge.
- [Entwicklerhandbuch](Entwicklerhandbuch.md), [Testhandbuch](Testhandbuch.md), [Benutzerhandbuch](Benutzerhandbuch.md): Umsetzung, Prüfung, Bedienung.
- [Historische Nachverfolgung](Historical-Traceability.md): Herkunft, Kategorien und Prüflücken.
