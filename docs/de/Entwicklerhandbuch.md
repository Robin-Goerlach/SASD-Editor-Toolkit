# Entwicklerhandbuch – SASD Editor Toolkit

[English](../en/Developer-Guide.md) · [Architektur](Architektur.md) · [Testhandbuch](Testhandbuch.md) · [Roadmap](ROADMAP.md)

**Zielgruppe:** Entwickler der eigenständigen Core-Bibliothek, WinForms-Adapter, Samples und künftiger Hostanwendungen.  
**Stand:** 03.11.2026. Dieses Handbuch beschreibt zuerst den **tatsächlich vorhandenen M1-Code**; spätere Verträge werden als geplant bezeichnet.

## 1. Arbeitsumgebung

| Zweck | Voraussetzung |
|---|---|
| Core-Entwicklung unter Windows/Linux/macOS | .NET SDK 10.x, Git |
| Vollständige WinForms-Solution | Windows mit .NET SDK 10.x und Windows-Desktop-Support |
| Tests | xUnit über \`dotnet test\` |
| Versionsvorgabe | \`global.json\`: SDK 10.0.100 mit \`latestFeature\`-Roll-forward |
| IDE | Visual Studio oder C#-fähige IDE/Editorumgebung |

**Klonen und Orientieren:**

\`\`\`bash
git clone https://github.com/Robin-Goerlach/SASD-Editor-Toolkit.git
cd SASD-Editor-Toolkit
git status --short --branch
git log -5 --oneline
dotnet --info
\`\`\`

**Unter Windows die vollständige Solution prüfen:**

\`\`\`powershell
powershell -ExecutionPolicy Bypass -File .\scripts\verify-all.ps1
\`\`\`

**Unter Debian/WSL, Linux oder macOS den Core prüfen:**

\`\`\`bash
./scripts/verify-core.sh
\`\`\`

Unter Linux können Core und Core-Tests entwickelt werden; daraus folgt **keine** erfolgreiche Kompilierung des WinForms-Samples. Die Windows-CI ist dafür der zusätzliche Nachweis. Wenn SDK, NuGet-Feeds oder Buildwerkzeuge nicht verfügbar sind, diese Einschränkung dokumentieren und keine Testerfolge behaupten.

## 2. Repository lesen statt Architektur erraten

\`\`\`text
src/Sasd.EditorToolkit.Core/
  Documents/    TextDocument und Metadaten
  Text/         ITextBuffer, LineTextBuffer, Positionen, Ranges
  Editing/      EditorViewState, TextSelection
  Commands/     Command-IDs, Registry, Dispatcher, Handler
  Search/       UI-unabhängige Literalsuche
  Storage/      Streams, Encoding und Dateispeicherung
  Undo/         einfache Undo-/Redo-Historie
  Workspaces/   Dokumente und Views
  Settings/     initiale Einstellungen
src/Sasd.EditorToolkit.WinForms/
  SasdEditorView.cs / EditorSurface.cs
samples/Sasd.EditorToolkit.Sample.FirstEd.WinForms/
  MainForm.cs – Hostabläufe und Dialoge
tests/Sasd.EditorToolkit.Core.Tests/
docs/adr/   akzeptierte Entscheidungen
docs/spec/  sprachneutrale Verhaltensverträge
docs/ai/    Arbeitsstand, Codex-Regeln und nächste Aufgaben
\`\`\`

**Lese-Reihenfolge für eine Änderung:** \`AGENTS.md\` und lokale AGENTS → relevante ADR/Spec → betroffene Klassen/Tests → Roadmap/NEXT-WORK. Das Pflichtenheft ist die **Zielumsetzung**, nicht eine Liste bereits vorhandener APIs.

## 3. Minimaler Headless-Einstieg

Die folgende Verwendung basiert auf real vorhandenen M1-Typen:

\`\`\`csharp
using Sasd.EditorToolkit.Commands;
using Sasd.EditorToolkit.Documents;
using Sasd.EditorToolkit.Editing;
using Sasd.EditorToolkit.Text;

var document = new TextDocument(new LineTextBuffer("Hello"));
document.MarkSaved();

var view = new EditorViewState();
view.MoveCaret(new TextPosition(0, 5));

var registry = new EditorCommandRegistry();
registry.RegisterM1Defaults();
var dispatcher = new EditorCommandDispatcher(registry);

var result = await dispatcher.ExecuteAsync(
    EditorCommandIds.InsertText,
    new EditorCommandContext(document, view, "!"));

if (!result.Handled)
{
    throw new InvalidOperationException(result.Message);
}

Console.WriteLine(document.Buffer.GetText()); // Hello!
Console.WriteLine(document.IsDirty);           // True
document.Undo();
Console.WriteLine(document.Buffer.GetText()); // Hello
\`\`\`

**Wichtig:** Der Host erzeugt Dispatcher und Kontext explizit; keine globale Editorinstanz wird vorausgesetzt. Die \`EditorViewState\`-Instanz ist unabhängig vom Dokument. Einige im Pflichtenheft entworfene Interfaces wie \`ICommandInterceptor\` oder \`ITextBufferFactory\` sind noch nicht als produktive Implementierungen verfügbar.

## 4. Wie eine Änderung durch die Schichten läuft

1. Eine Eingabe wird im Host/Adapter in eine Command-ID und einen Parameter übersetzt.
2. Der Dispatcher holt den Command aus der Registry und ruft \`CanExecute\` auf.
3. Ein mutierender Command verändert den Text **über \`TextDocument\`** und aktualisiert Caret/Selection.
4. \`TextDocument\` registriert einen \`TextChangeSet\` beim \`UndoManager\`.
5. Der Buffer meldet \`Changed\`; Adapter aktualisieren ihre Darstellung.
6. Die Sample-Anwendung aktualisiert Status/Dirty-Anzeige.

Nicht direkt über \`document.Buffer.Replace(...)\` verändern, wenn Undo- und Dirty-Verhalten Teil der Aufgabe sind: Die derzeit öffentliche Buffer-Mutabilität ist eine bekannte Architekturgrenze. Neue Codepfade müssen sie **nicht weiter ausnutzen**; eine zukünftige Einschränkung der API benötigt eine ADR.

## 5. Neue Core-Funktion oder Command entwickeln

**Vorgehen:**

1. Einen beobachtbaren Vertrag definieren: Eingaben, Ergebnis, Fehler, Unicode-/Zeilenendengrenzen, Undo und Abbruch.
2. Vorhandene Command-IDs und Dienste durchsuchen; keine parallele Zustandsmaschine.
3. Testfälle zuerst oder parallel entwerfen – insbesondere leeres Dokument, Bereichsgrenzen, Unicode und Fehler.
4. Reine Fachlogik nach \`Core\`; UI-spezifische Eingabe/Zeichnung nach WinForms; Dialogentscheidung in den Host.
5. Einen bestehenden Command erweitern oder eine eigene \`IEditorCommand\`-Implementierung ergänzen.
6. Registry und gegebenenfalls \`EditorCommandIds\` aktualisieren; neue öffentlich stabile IDs auch in \`docs/spec/Command-Ids.md\` aufnehmen.
7. XML-Dokumentation und Kommentare für Invarianten ergänzen; keine rein syntaktischen Erklärungen.
8. Core-Tests, Windows-Integration und Dokumentation prüfen; Changelog ergänzen.

**Befehle dürfen keine Berechtigung annehmen.** Ein registrierter Command darf nicht automatisch fremde Dateien löschen, Makros ausführen oder Netzwerkverbindungen aufbauen. Erweiterungen mit eigener Sicherheitsgrenze benötigen eine ADR.

## 6. Einen Host oder UI-Adapter anbinden

Der derzeitige WinForms-Adapter bietet \`SasdEditorView.Document\`, \`SasdEditorView.ViewState\`, \`CommandDispatcher\` und \`ExecuteCommandAsync\`. Das FIRST-ED-Sample zeigt die Bindung sowie das Bereitstellen von Menüs, Datei- und Suchdialogen.

Eine neue Hostintegration sollte:

- dieselbe \`TextDocument\`-Instanz wiederverwenden, keine Textkopie zum zweiten „Quellzustand“ machen;
- pro Editoransicht einen getrennten ViewState besitzen;
- Buffer-\`Changed\` beobachten und den betroffenen visuellen Bereich aktualisieren;
- Datei-Auswahl, Save/Discard/Cancel und Fehlerpräsentation hostseitig implementieren;
- \`IsDirty\` und Dateimetadaten für Status/CloseGuard verwenden;
- UI-Threadregeln des Frameworks einhalten;
- Steuerzeichen, IME, Surrogatpaare und Multi-Line-Paste ausdrücklich testen.

**Noch keine Zusage:** Vollständige Split-View-Hostintegration, Tastaturprofil-Loader, WPF-Control oder Web-Adapter. Entsprechende Beispiele aus dem Pflichtenheft sind Zielentwürfe.

## 7. Datei- und Encodingbehandlung

\`FileDocumentStorage\` implementiert \`IDocumentStorage\` für Streams sowie zusätzliche Pfadmethoden.

\`\`\`csharp
using Sasd.EditorToolkit.Storage;

var storage = new FileDocumentStorage();
var loaded = await storage.LoadFileAsync("example.txt");

if (loaded.BinaryContentSuspected)
{
    // Der Host muss eine bewusste Entscheidung treffen.
    // Die M1-Bibliothek erzwingt hier noch keinen Dialog.
}

loaded.Document.Insert(new(0, 0), "Note: ");
await storage.SaveFileAsync(loaded.Document);
\`\`\`

**Speicherregeln:** Encoding/BOM nicht unbedacht verändern; nicht nach einem Fehler \`MarkSaved\` aufrufen; Dateipfade vom Host autorisieren lassen; Konflikte mit externen Schreibern vor Veröffentlichung absichern; Recovery/Autosave nur mit expliziter Schutz- und Datenschutzentscheidung ergänzen.

**Grenzen heute:** Dateiinhalt wird zum Laden vollständig gepuffert; der Default-Binärverdacht ist noch begrenzt; großskalige Streaming- und Konfliktbehandlung sind nicht fertig. Temporärdatei plus \`File.Move(overwrite: true)\` ist nur im Rahmen der jeweiligen Dateisystemsemantik atomar.

## 8. Unicode, Suche, Selektion und Performance

- Aktuelle \`TextPosition.Column\` entspricht UTF-16-Indizes im C#-Modell. Für Caret-Bewegungen sind später Graphemgrenzen sicherzustellen.
- \`TextSearchService\` unterstützt bisher Literalsuche mit Richtung, Wrap und \`StringComparison\`. Eine allgemeine Regex-Suche oder vollständige Replace-Engine ist noch nicht vorhanden.
- \`FindTextCommand\` setzt Auswahl und Caret im Core, nicht in der Oberfläche.
- \`LineTextBuffer\` arbeitet bei Änderungen über vollständigen Text-Neuaufbau; \`Length\` und Offset-Abbildung sind nicht für sehr große Dokumente optimiert.
- Benchmarks erst mit reproduzierbaren Testdateien, Warm-up und definierter Hardware vergleichen; keine erfundenen Performancegarantien.
- Abbruch und Fehler dürfen kein halb editiertes Dokument hinterlassen.

## 9. Dokumentieren, prüfen und committen

- Verhaltensänderung → \`docs/spec/\` plus Tests.
- Grundsatzentscheidung → neue oder geänderte \`docs/adr/\`.
- Benutzerfunktion → Benutzerhandbuch, DE/EN-Verweise und \`CHANGELOG.md\`.
- Architekturänderung → beide Fassungen von \`Architektur.md\` / \`Architecture.md\`.
- Arbeit in mehreren Schichten oder öffentliche API → Ausführungsplan nach \`docs/en/PLANS.md\`.
- Historisch inspirierte Funktion → \`Historische-Nachverfolgung.md\` und Pflichtenheft-IDs prüfen.

Vor Abschluss: \`dotnet test\`, \`git diff --check\`, Git-Status, CI-Nachweis. Keine Fremdänderungen verwerfen, keine Secrets/Benutzerdateien committen und keinen Force-Push. Direkte Arbeit an \`main\` gilt nur im aktuellen Single-Developer-Workflow laut AGENTS.

## 10. Häufige Fehler und sinnvolle Reaktion

| Beobachtung | Wahrscheinlicher Ansatz |
|---|---|
| Core auf Linux grün, WinForms nicht geprüft | Vollständige Windows-CI kontrollieren; Core-Ergebnis nicht als Gesamtprüfung ausgeben |
| \`NotHandled\` bei Command | Command registriert? \`CanExecute\` erfüllt? Parameter korrekt? |
| Suchen liefert keine Fundstelle | Startposition, \`StringComparison\`, Richtung und Wrap prüfen |
| Dirty-State wirkt überraschend | Gespeicherten Textsnapshot, direkte Buffer-Mutation und Undo-Pfad prüfen |
| Zeilenenden/BOM ändern sich | Encoding aus Metadaten, \`DocumentSaveOptions\`, Testinput prüfen |
| Oberfläche aktualisiert sich nicht | Buffer-Event, View-Bindung, Invalidation und UI-Thread prüfen |
| Große Datei langsam | M1-Puffer ist nicht Large-File-fähig; reproduzierbar messen und spätere Strategie hinter Interface entwerfen |

**Nächste Implementierungsarbeiten:** die tatsächlich priorisierte Queue unter [NEXT-WORK](../ai/NEXT-WORK.md) verwenden, keine vorgezogene Featureexpansion. 
