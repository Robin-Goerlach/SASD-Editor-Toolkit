# SASD Editor Toolkit – Feature-Analyse vergleichbarer Produkte

**Dokumenttyp:** Feature-Analyse / Vorlagenanalyse  
**Projektidee:** Moderner Nachbau eines Editor-Baukastens im Geist der Borland Turbo Editor Toolbox  
**Arbeitsname:** SASD Editor Toolkit / SASD EditorKit  
**Stand:** 2026-05-22  
**Sprache:** Deutsch  
**Ziel:** Sammlung und Bewertung der Funktionen relevanter Editor-Toolkits und Editor-Komponenten als Grundlage für eine spätere SASD-Roadmap.

---

## 1. Ziel dieses Dokuments

Dieses Dokument sammelt die Features der Produkte und Komponenten, die im Gespräch als mögliche Vorbilder für einen modernen Nachbau der **Turbo Pascal Editor Toolbox** angesprochen wurden.

Die Idee ist **nicht**, ein historisches Produkt 1:1 zu kopieren. Sinnvoller ist ein eigenständiges modernes SASD-Projekt, das die damalige Stärke übernimmt:

> Ein wiederverwendbarer Editor-Baukasten, mit dem eigene Editoren oder Anwendungen mit eingebautem Editor gebaut werden können.

Das Dokument soll als Arbeitsgrundlage dienen für:

- ein späteres Lastenheft,
- ein Pflichtenheft,
- eine technische Roadmap,
- eine Architekturentscheidung,
- eine Feature-Priorisierung,
- und eine erste MVP-Abgrenzung.

---

## 2. Betrachtete Produkte und Komponenten

Folgende Produkte bzw. Komponenten wurden berücksichtigt:

| Produkt / Komponente | Kategorie | Hauptnutzen als Vorlage |
|---|---|---|
| Borland Turbo Editor Toolbox | Historische Editor-Toolbox | Grundidee: Editor-Baukasten, Command Dispatcher, Text-/Window-Strukturen |
| FIRST-ED | Einfacher Demoeditor der Toolbox | Minimalbeispiel für einen schnell erzeugbaren Editor |
| MicroStar | Umfangreicher Demoeditor der Toolbox | Beispiel für Menüsystem, Command Set, Statusanzeige, Pop-ups, Dirty-Bit |
| Scintilla | Native Editor-Komponente | Robuste Code-Editor-Komponente mit Syntax-Styling, Markern, Completion |
| SciTE | Editor auf Basis von Scintilla | Demonstriert, wie aus einer Komponente ein vollständiger Editor wird |
| AvalonEdit | WPF/.NET Editor-Komponente | Naheliegendste Vorlage für C#/.NET/WPF |
| CodeMirror 6 | Webbasierte Editor-Komponente | Moderne State/View/Extension-Architektur |
| Monaco Editor | Webbasierte VS-Code-Editor-Komponente | Sehr umfangreicher moderner Code-Editor mit Modell/View/Provider-Konzept |
| SynEdit / TurboPack SynEdit | Delphi/C++Builder Editor-Control | Historisch und technisch nah an Borland/Delphi/Pascal |
| GtkSourceView | GTK Source-Editing-Widget | Gute Vorlage für Linux/GTK-Editorfunktionen |
| Ace Editor | Webbasierter JavaScript-Code-Editor | Gute Vorlage für leicht einbettbare Web-Editoren |

---

## 3. Historische Ausgangsbasis: Borland Turbo Editor Toolbox

### 3.1 Grundidee

Die Turbo Editor Toolbox war ein Baukasten zum Erstellen eigener Texteditoren und zum Einbetten von Editorfunktionen in eigene Turbo-Pascal-Anwendungen.

Wesentliche Ziele:

- einfache Editoren bauen,
- komplexe Editoren bauen,
- Editorfunktionen in eigene Anwendungen einbauen,
- High-Level- und Low-Level-Prozeduren bereitstellen,
- Text auf verschiedenen Ebenen bearbeiten,
- Bildschirmdarstellung und Fensterverwaltung unterstützen,
- anpassbare Kommandostruktur ermöglichen.

### 3.2 Zentrale Featurebereiche

Die historische Toolbox unterstützte bzw. behandelte unter anderem:

- Zeichenbearbeitung,
- Wörter und Zeilen,
- Gruppen von Zeilen,
- Textstreams,
- Fenster auf Textstreams,
- Blöcke von Zeilen,
- Dateien,
- Screen Displays,
- Cursorbewegung,
- Textlöschung,
- Word-Processing-Kommandos,
- Window-Kommandos,
- Block-Kommandos,
- File-Kommandos,
- Exit-Kommandos,
- Command Dispatcher,
- Command Processor,
- UserCommand-Hook,
- UserError-Hook,
- Statuszeilen-Anpassung,
- Prompting für Find/Replace,
- Hintergrundaufgaben über Scheduler/UserTask,
- Dirty-Bit zur Änderungserkennung,
- Overlaying für Speicherknappheit,
- Einbetten des Editors in eigene Programme.

### 3.3 Textmodell und Datenstrukturen

Das Handbuch diskutiert mehrere Textpuffer-Strategien:

- Array-of-Lines,
- Fixed Buffer,
- Linked List.

Die Toolbox verwendet im historischen Design einen Linked-List-Ansatz für Zeilen. Das war damals sinnvoll, weil Speicher knapp war und Einfügen/Löschen von Zeilen effizienter werden sollte.

Für einen modernen Nachbau ist diese Idee konzeptionell wertvoll, aber nicht zwingend als konkrete Implementierung zu übernehmen. Moderne Alternativen wären:

- `List<TextLine>` für ein einfaches MVP,
- Gap Buffer,
- Piece Table,
- Rope,
- immutable Snapshots,
- virtualisierte Textmodelle für sehr große Dateien.

### 3.4 FIRST-ED als Vorbild

FIRST-ED ist ein minimalistischer Demoeditor. Sein Wert liegt nicht in vielen Funktionen, sondern darin, dass er zeigt:

- wie schnell ein Editor mit der Toolbox erzeugt werden kann,
- welche Minimalmodule nötig sind,
- wie Hooks bereitgestellt werden,
- wie der Standard-Editorloop genutzt wird,
- wie Anpassungen später ergänzt werden können.

Für SASD wäre FIRST-ED das Vorbild für:

> eine sehr kleine Demoanwendung, die beweist, dass der Editor-Kern funktioniert.

Mögliche moderne Entsprechung:

- `Sasd.EditorKit.WinForms.DemoFirstEditor`
- oder `Sasd.EditorKit.Wpf.DemoFirstEditor`

MVP-Funktionen:

- Datei öffnen,
- Datei speichern,
- Text bearbeiten,
- Cursor bewegen,
- Statuszeile,
- Dirty-Anzeige,
- Tastatursteuerung,
- minimale Menüleiste.

### 3.5 MicroStar als Vorbild

MicroStar ist der umfangreichere Demoeditor der Toolbox. Wichtige Featureideen:

- Pulldown-Menüsystem,
- größerer Command Set,
- Command Dispatcher,
- Pop-up-Fenster,
- Hintergrunddruck,
- anpassbare Fehlerbehandlung,
- anpassbare Statusanzeige,
- Dirty-Bit,
- mehr Struktur im UI.

Für SASD wäre MicroStar das Vorbild für:

> eine größere Demoanwendung, die zeigt, wie man aus dem EditorKit einen vollständigen Editor baut.

Mögliche moderne Entsprechung:

- `Sasd.EditorKit.DemoMicroEditor`
- `Sasd.EditorKit.SampleApp`
- `SASD MiniEdit`

Mögliche Funktionen:

- Menüsystem,
- Toolbar,
- Kontextmenüs,
- mehrere Dokumente,
- Split View,
- Suche/Ersetzen,
- Blockoperationen,
- Markdown- oder Code-Modus,
- Statuszeile,
- Konfigurationsdialog,
- Exportfunktionen,
- optionale Syntaxhervorhebung.

---

## 4. Scintilla

### 4.1 Einordnung

Scintilla ist eine freie Source-Code-Editor-Komponente. Sie ist keine komplette IDE, sondern eine wiederverwendbare Editor-Komponente, die in andere Anwendungen eingebettet werden kann.

Sie ist als Vorlage besonders wertvoll, weil sie genau wie die Turbo Editor Toolbox eher eine technische Komponente als ein Endanwenderprodukt ist.

### 4.2 Zentrale Features

Scintilla bietet unter anderem:

- native Editor-Komponente,
- Source-Code-Bearbeitung,
- Syntax Styling,
- Fehlerindikatoren,
- Code Completion,
- Call Tips,
- Selektionsrand / Margin,
- Marker im Rand,
- Breakpoint-ähnliche Marker,
- aktuelle Zeile / Debugger-Markierungen,
- freie Styling-Möglichkeiten,
- verschiedene Vordergrund- und Hintergrundfarben,
- unterschiedliche Fonts,
- Bold/Italic-Stile,
- proportionale Fonts,
- Trennung von Quelltext und mechanisch erzeugtem Styling,
- Nutzung auf Win32, GTK und macOS,
- komplette Quellcode-Verfügbarkeit,
- Einsatz in freien und kommerziellen Projekten laut Projektbeschreibung.

### 4.3 Relevante Featureideen für SASD

Für SASD besonders interessant:

- klare Trennung zwischen Textinhalt und visueller Darstellung,
- Styling darf nicht automatisch den Textinhalt verändern,
- Marker-System für Fehler, Warnungen, Breakpoints und Suchtreffer,
- Completion und Call Tips als generische Schnittstellen,
- Margins/Gutters für Zeilennummern, Marker, Folding,
- robuste native Performance,
- Source-Code-Fokus,
- Einbettbarkeit in andere Anwendungen.

### 4.4 Was SASD übernehmen sollte

Für das SASD Editor Toolkit wären folgende Konzepte wertvoll:

- `ITextStyler` oder `ISyntaxHighlighter`,
- `IMarkerService`,
- `IGutterRenderer`,
- `ICompletionProvider`,
- `ICallTipProvider`,
- `IErrorIndicatorProvider`,
- klare Regel: Styling ist berechenbar und gehört nicht dauerhaft in den Textpuffer,
- Unterstützung mehrerer Frontends.

### 4.5 Was SASD nicht übernehmen sollte

Nicht blind übernehmen:

- direkte native C++-Architektur, wenn das SASD-Projekt in C# entstehen soll,
- sehr große API-Oberfläche direkt zu Beginn,
- zu frühe Optimierung auf professionelle IDE-Features.

---

## 5. SciTE

### 5.1 Einordnung

SciTE ist ein Editor auf Basis von Scintilla. Ursprünglich war SciTE ein Demonstrationseditor für Scintilla, ist aber zu einem allgemein nutzbaren Editor mit Build-/Run-Funktionen geworden.

Für SASD ist SciTE nicht primär wegen seines UI-Designs interessant, sondern weil es zeigt:

> Eine gute Editor-Komponente kann durch eine schlanke Anwendung zu einem brauchbaren Editorprodukt werden.

### 5.2 Zentrale Features

Typische SciTE-Featureideen:

- Editor auf Scintilla-Basis,
- Syntax Highlighting,
- einfache Konfiguration,
- Build- und Run-Funktionen,
- Quelltextbearbeitung,
- Einsatz für Test- und Demonstrationsprogramme,
- Sprach-/API-Konfigurationen,
- Zusatzkonfigurationsdateien,
- leichtgewichtige Editoranwendung.

Je nach Installation und Konfiguration kommen häufig hinzu:

- Code Folding,
- Completion,
- Call Tips,
- Ausgabefenster,
- Skript-/Build-Ausführung,
- Sprachprofile,
- Tastaturkürzel,
- Suchfunktionen.

### 5.3 Relevante Featureideen für SASD

Für SASD besonders interessant:

- Demoeditor als ernsthaft benutzbares Werkzeug,
- schlankes Produkt statt überladener IDE,
- Textkonfigurationsdateien,
- Build-/Run-Kommandos als optionales Modul,
- einfache Spracheinstellungen,
- Editor plus Output-Panel,
- Projekt- oder Dateityp-spezifische Einstellungen.

### 5.4 Mögliche SASD-Umsetzung

Ein späterer SASD-Demoeditor könnte enthalten:

- Texteditorbereich,
- Output-Bereich,
- konfigurierbare externe Tools,
- Build-/Run-Kommandos,
- Dateityp-Profile,
- einfache Projektdatei,
- konfigurierbare Keybindings,
- einfache portable Installation.

---

## 6. AvalonEdit

### 6.1 Einordnung

AvalonEdit ist eine WPF-basierte Texteditor-Komponente aus dem SharpDevelop-Umfeld und wird unter anderem in ILSpy und weiteren .NET-Projekten verwendet.

Für SASD ist AvalonEdit die naheliegendste Vorlage, wenn das Projekt mit C#/.NET und WPF gedacht wird.

### 6.2 Zentrale Features und Eigenschaften

AvalonEdit bietet bzw. steht für:

- WPF-basierte Editor-Komponente,
- .NET-Integration,
- Nutzung als NuGet-Paket,
- MIT-Lizenz,
- Einsatz in realen .NET-Projekten,
- Texteditor-Control,
- Syntax Highlighting,
- Dokumentmodell,
- Rendering im WPF-Kontext,
- Einbettbarkeit in eigene Desktopanwendungen,
- Erweiterbarkeit über .NET-Code,
- gute Eignung für Tools, Viewer und IDE-artige Anwendungen.

Viele Anwendungen nutzen AvalonEdit als Baustein für:

- XAML-Editoren,
- Log-Viewer,
- Code-Editoren,
- Decompiler-Ansichten,
- Skripteditoren,
- Konfigurationseditoren,
- Markdown- oder Texteditoren.

### 6.3 Relevante Featureideen für SASD

Für SASD besonders interessant:

- C#/.NET-orientierte Architektur,
- einbettbares Editor-Control,
- geeignet für WPF,
- realistische Vorlage für ein SASD-Desktoptool,
- Syntax-Highlighting als separater Mechanismus,
- Nutzung in professionellen Tools,
- überschaubarer als Monaco,
- näher an SASD-Windows-Desktopprojekten als CodeMirror/Ace.

### 6.4 Mögliche SASD-Ableitung

Wenn SASD einen eigenen Editor bauen will, kann AvalonEdit als Referenz dienen für:

- `Sasd.EditorKit.Wpf`,
- Textansicht,
- Rendering,
- Caret,
- Selection,
- Folding,
- Syntax-Highlighting-Integration,
- Scrollverhalten,
- Copy/Paste,
- Editor-Control als UI-Komponente.

### 6.5 Strategische Einschätzung

AvalonEdit ist als Vorlage besonders stark für:

- SASD Prompt Manager,
- SASD Log Viewer,
- SASD Markdown Editor,
- SASD Config Editor,
- kleine IDE-artige SASD-Tools,
- Windows-Desktop-Werkzeuge.

Falls es um ein kurzfristig produktives SASD-Tool geht, könnte man auch überlegen, AvalonEdit zunächst zu verwenden, statt sofort alles selbst zu schreiben. Falls das Ziel aber ein eigenständiges Lern-/Grundlagenprojekt ist, ist ein eigener kleiner Editor-Kern trotzdem sinnvoll.

---

## 7. CodeMirror 6

### 7.1 Einordnung

CodeMirror 6 ist eine moderne Code-Editor-Komponente für Webanwendungen. Sie ist besonders interessant wegen ihrer klaren Architektur:

- Editor State,
- Editor View,
- Transaktionen,
- Extensions,
- Modularity.

Für SASD ist CodeMirror 6 weniger als direkte C#-Vorlage wichtig, sondern als Architekturvorbild.

### 7.2 Zentrale Features

CodeMirror bietet unter anderem:

- Webbasierte Editor-Komponente,
- umfangreiche Editing-Funktionen,
- reiche Programmierschnittstelle,
- Accessibility-Unterstützung,
- Keyboard-only-Nutzung,
- Screenreader-Unterstützung,
- Mobile Support,
- Bidirectional Text Support,
- Syntax Highlighting,
- Line Numbers / Gutters,
- Autocompletion,
- Code Folding,
- Search/Replace,
- RegExp-Suche,
- Replace-Funktionalität,
- Parsing / Syntax Trees,
- Extension Interface,
- Modularity,
- Performance auf großen Dokumenten und langen Zeilen,
- automatische Klammerergänzung,
- Linting,
- State/View-Trennung,
- Transaktionsmodell,
- sichtbarer Viewport statt vollständigem DOM-Rendering großer Dokumente.

### 7.3 Relevante Featureideen für SASD

Für SASD besonders wichtig:

- State/View-Trennung,
- Transaktionen statt direkter DOM- oder Textmanipulation,
- Features als Erweiterungen,
- testbarer Kern,
- klare öffentliche API,
- Modularität,
- große Dokumente nur teilweise rendern,
- Accessibility nicht nachträglich, sondern als Grundprinzip,
- mobile und bidirektionale Texte zumindest architektonisch bedenken.

### 7.4 Mögliche SASD-Ableitung

Ein modernes SASD EditorKit könnte ähnliche Konzepte nutzen:

```text
EditorState
EditorDocument
EditorTransaction
EditorSelection
EditorViewState
EditorCommand
EditorExtension
```

Vorteil:

- Änderungen sind nachvollziehbar,
- Undo/Redo wird sauberer,
- Tests werden einfacher,
- UI bleibt austauschbar,
- Features können später modular ergänzt werden.

### 7.5 Mögliche SASD-Features nach CodeMirror-Vorbild

- Extension-System,
- State-Objekt,
- Transaktionslogik,
- Decorations,
- Diagnostics,
- Linting,
- Keymaps,
- Syntaxpakete,
- ViewPlugins,
- UpdateListener,
- multiple Selections,
- Gutter-Extensions,
- optionaler Readonly-Modus,
- dynamische Konfiguration.

---

## 8. Monaco Editor

### 8.1 Einordnung

Monaco Editor ist der browserbasierte Code-Editor, der aus VS-Code-Quellen erzeugt wird bzw. die Editorbasis von VS Code im Browserkontext bereitstellt.

Für SASD ist Monaco die Vorlage für moderne High-End-Editorfunktionen. Gleichzeitig ist Monaco für einen einfachen SASD-Nachbau sehr groß und komplex.

### 8.2 Zentrale Features und Konzepte

Monaco bietet bzw. verwendet:

- browserbasierter Code-Editor,
- VS-Code-nahe Editorfunktionen,
- Modelle als zentrale Textrepräsentation,
- URI-basierte Modellidentität,
- Editor als sichtbare View auf ein Modell,
- View State,
- Actions und Commands,
- Providers für intelligente Editorfunktionen,
- Completion Provider,
- Hover Provider,
- Spracheigenschaften,
- JSON/TypeScript-artige IntelliSense-Konzepte,
- API-Dokumentation,
- Playground,
- ESM-Build,
- AMD-Build aus Kompatibilitätsgründen,
- Lokalisierung,
- Web Worker für schwere Sprachdienste,
- Disposable-Konzept für Ressourcenfreigabe,
- MIT-Lizenz.

### 8.3 Typische Funktionsbereiche

Aus Monaco/VS-Code-Sicht relevante Featurebereiche:

- Syntax Highlighting,
- IntelliSense,
- Code Completion,
- Hover Information,
- Diagnostics,
- Marker,
- Multiple Models,
- Multiple Editors,
- Commands,
- Actions,
- Context Menus,
- Minimap,
- Folding,
- Bracket Matching,
- Find/Replace,
- Go to Definition,
- Rename,
- Language Services,
- JSON Schema-Unterstützung,
- TypeScript/JavaScript-Sprachdienste,
- Worker-basierte Hintergrundanalyse.

Nicht alle Features sind automatisch ohne zusätzliche Konfiguration in jeder Einbettung verfügbar. Monaco ist mächtig, erfordert aber ein sauberes Setup.

### 8.4 Relevante Featureideen für SASD

Für SASD besonders interessant:

- Modell/View-Trennung,
- URI-basierte Dokumentidentität,
- Provider-Modell,
- Commands/Actions,
- Ressourcenfreigabe über Disposable,
- Language Services als austauschbare Anbieter,
- Background Worker als spätere Ausbaustufe,
- Playground/Sample-Ansatz als Dokumentations- und Testhilfe.

### 8.5 Strategische Einschätzung

Monaco ist als direkte Vorlage sehr mächtig, aber für ein SASD-MVP zu groß. Sinnvoll wäre:

- nicht Monaco nachbauen,
- sondern ausgewählte Architekturideen übernehmen,
- insbesondere Model/View/Provider/Command,
- eine kleine, verständliche Version bauen,
- High-End-Features erst später planen.

---

## 9. SynEdit / TurboPack SynEdit

### 9.1 Einordnung

SynEdit ist ein Syntax-Highlighting-Edit-Control für Delphi und C++Builder. Es ist nicht auf den Windows Common Controls aufgebaut und gehört historisch in die Borland-/Delphi-Welt.

Für SASD ist SynEdit interessant, weil es dem Borland-Umfeld konzeptionell näher steht als moderne reine Webeditoren.

### 9.2 Zentrale Features und Eigenschaften

SynEdit bietet bzw. steht für:

- Syntax Highlighting Edit Control,
- Delphi-Unterstützung,
- C++Builder-Unterstützung,
- Design-Time- und Runtime-Packages,
- Win32- und Win64-Unterstützung,
- Einbettung in Delphi/C++Builder-Anwendungen,
- Quellcodebasierte Verteilung,
- eigene Editor-Komponente statt Windows-Standardcontrol,
- Highlighter-Struktur,
- historisch IDE-nahe Nutzung.

Je nach Version/Fork/Integration sind typisch:

- Syntaxhervorhebung,
- Zeilennummern,
- Gutter,
- Caret/Selection,
- Undo/Redo,
- Code Folding,
- Suchfunktionen,
- Tastenkürzel,
- Editor-Events,
- Integration in RAD Studio,
- Design-Time-Konfiguration.

### 9.3 Relevante Featureideen für SASD

Für SASD besonders interessant:

- Editor-Control als UI-Komponente,
- Komponentenmodell,
- Design-Time/Runtime-Gedanke,
- Syntax-Highlighter als austauschbare Komponenten,
- Nähe zu Borland-Tradition,
- geeignetes Vergleichsobjekt für „Turbo Editor Toolbox in modern“.

### 9.4 Mögliche SASD-Ableitung

Für SASD könnte daraus entstehen:

- `Sasd.EditorKit.WinForms.EditorControl`,
- `Sasd.EditorKit.Wpf.EditorControl`,
- Highlighter-Komponenten,
- Gutter-Komponenten,
- Editor-Events,
- Commands,
- Design-Time-freundliche Properties,
- spätere Toolbox-Verwendung in mehreren SASD-Anwendungen.

---

## 10. GtkSourceView

### 10.1 Einordnung

GtkSourceView ist eine GNOME/GTK-Bibliothek, die das Standard-GTK-TextView-Widget um typische Source-Code-Editorfunktionen erweitert.

Für SASD ist GtkSourceView interessant, wenn Linux/GTK oder plattformübergreifende Desktopüberlegungen eine Rolle spielen.

### 10.2 Zentrale Features

GtkSourceView unterstützt laut Projektbeschreibung unter anderem:

- Erweiterung von GtkTextView,
- multiline text editing,
- Syntax Highlighting,
- File Loading,
- File Saving,
- Search and Replace,
- Code Completion,
- Snippets,
- Vim Emulation,
- Printing,
- Line Numbers,
- typische Source-Code-Editorfunktionen.

Aus API- und Klassensicht relevant:

- Source Buffer,
- Source View,
- Anzeige von Zeilennummern,
- rechter Rand,
- aktuelle Zeile hervorheben,
- Einrückungseinstellungen,
- Home/End-Verhalten,
- Line Marks.

### 10.3 Relevante Featureideen für SASD

Für SASD besonders interessant:

- bestehendes TextView erweitern statt alles neu erfinden,
- klare Trennung von Buffer und View,
- SourceBuffer-Konzept,
- Completion-System,
- Snippet-System,
- Vim-Emulation als optionales Profil,
- Druckfunktion,
- Line Marks,
- gute Linux-Orientierung.

### 10.4 Mögliche SASD-Ableitung

Für das SASD Editor Toolkit:

- `EditorBuffer`,
- `EditorView`,
- `LineMark`,
- `RightMargin`,
- `CurrentLineHighlight`,
- `IndentationSettings`,
- `KeyboardBehaviorProfile`,
- `SnippetService`,
- `CompletionService`,
- `PrintService`.

---

## 11. Ace Editor

### 11.1 Einordnung

Ace ist ein embeddable Code Editor in JavaScript. Er kann einfach in Webseiten und JavaScript-Anwendungen eingebettet werden und war/ist eng mit Cloud9 IDE verbunden.

Für SASD ist Ace besonders relevant, wenn später eine Webvariante des Editor Toolkits oder ein browserbasierter SASD-Konfigurations-/Prompt-/Markdown-Editor entstehen soll.

### 11.2 Zentrale Features

Ace bietet unter anderem:

- einbettbarer Codeeditor für Webanwendungen,
- JavaScript-Implementierung,
- Performance auf Niveau nativer Editoren laut Projektbeschreibung,
- einfache Einbettung,
- Syntax Highlighting für viele Sprachen,
- viele Themes,
- Import von TextMate/Sublime-Sprach- und Theme-Dateien,
- automatic indent/outdent,
- optionale Command Line,
- große Dokumente,
- vollständig anpassbare Keybindings,
- Vim- und Emacs-Modus,
- Search/Replace mit regulären Ausdrücken,
- Matching Parentheses,
- Soft Tabs / Real Tabs,
- Anzeige versteckter Zeichen,
- Drag & Drop von Text,
- Line Wrapping,
- Code Folding,
- Multiple Cursors,
- Multiple Selections,
- Live Syntax Checker,
- Cut/Copy/Paste,
- BSD-Lizenz laut Projektseite,
- GitHub-Quellcode.

### 11.3 Relevante Featureideen für SASD

Für SASD besonders interessant:

- einfache Einbettung,
- klare Web-Komponente,
- viele Sprachen und Themes,
- mehrere Keybinding-Profile,
- Vim/Emacs-Kompatibilität,
- Multiple Cursor/Selection,
- gute Standardfeatures für Quelltext,
- leichtgewichtiger als Monaco,
- gute Vorlage für eine spätere Web-Editor-Komponente.

### 11.4 Mögliche SASD-Ableitung

Für eine spätere Web-Variante:

- `Sasd.EditorKit.Web`,
- browserbasierte Prompt- und Markdown-Bearbeitung,
- Theme-Schnittstelle,
- Language Mode-Schnittstelle,
- Keybinding-Profile,
- RegExp-Suche,
- Foldable Regions,
- Live Diagnostics,
- Multiple Selection.

---

## 12. Vergleichsmatrix der wichtigsten Featurebereiche

### 12.1 Zweck und Produktart

| Featurebereich | Turbo Editor Toolbox | Scintilla | SciTE | AvalonEdit | CodeMirror 6 | Monaco | SynEdit | GtkSourceView | Ace |
|---|---|---|---|---|---|---|---|---|---|
| Editor-Baukasten | Stark | Stark | Mittel | Stark | Stark | Stark | Stark | Stark | Stark |
| Fertiger Editor | Teilweise durch FIRST-ED/MicroStar | Nein | Ja | Nein | Nein | Nein | Nein | Nein | Nein |
| Einbettbare Komponente | Ja | Ja | Nein/eher App | Ja | Ja | Ja | Ja | Ja | Ja |
| Lern-/Demonstrationswert | Sehr hoch | Hoch | Hoch | Hoch | Hoch | Mittel | Hoch | Mittel | Hoch |
| IDE-artige Basis | Mittel | Hoch | Mittel | Mittel-Hoch | Hoch | Sehr hoch | Mittel | Mittel | Hoch |

### 12.2 Plattform und Technologie

| Produkt | Haupttechnologie | Typische Plattform |
|---|---|---|
| Turbo Editor Toolbox | Turbo Pascal | MS-DOS / IBM PC |
| Scintilla | C++ | Windows, GTK/Linux, macOS |
| SciTE | C++ / Scintilla | Windows, Linux, macOS teils kommerziell |
| AvalonEdit | C# / WPF | Windows Desktop |
| CodeMirror 6 | JavaScript / TypeScript | Web |
| Monaco | TypeScript / JavaScript | Web |
| SynEdit | Delphi / Pascal | Windows / Delphi / C++Builder |
| GtkSourceView | C / GTK | Linux/GNOME, GTK-Umfeld |
| Ace | JavaScript | Web |

### 12.3 Kernfeatures

| Feature | Turbo Editor Toolbox | Scintilla | SciTE | AvalonEdit | CodeMirror 6 | Monaco | SynEdit | GtkSourceView | Ace |
|---|---|---|---|---|---|---|---|---|---|
| Text bearbeiten | Ja | Ja | Ja | Ja | Ja | Ja | Ja | Ja | Ja |
| Datei öffnen/speichern | Ja | Komponente unterstützt indirekt | Ja | App muss es integrieren | App muss es integrieren | App muss es integrieren | App muss es integrieren | Ja | App muss es integrieren |
| Cursorsteuerung | Ja | Ja | Ja | Ja | Ja | Ja | Ja | Ja | Ja |
| Selektion | Ja | Ja | Ja | Ja | Ja | Ja | Ja | Ja | Ja |
| Blockoperationen | Ja | Ja | Ja | Ja | Ja | Ja | Ja | Ja | Ja |
| Copy/Paste | Ja | Ja | Ja | Ja | Ja | Ja | Ja | Ja | Ja |
| Undo/Redo | Grundsätzlich erwartbar/Toolbox-Kontext prüfen | Ja | Ja | Ja | Ja | Ja | Ja | Ja | Ja |
| Dirty-Anzeige | Ja | Architekturthema | Ja | App-seitig | State-seitig | Model-seitig | App-seitig | Buffer-seitig | App-seitig |
| Search/Replace | Ja/Prompting erwähnt | Ja | Ja | Ja/typisch | Ja | Ja | Ja | Ja | Ja |
| RegExp-Suche | Historisch eher nein/unklar | Ja/abhängig | Ja | optional | Ja | Ja | optional | Ja/abhängig | Ja |

### 12.4 Code-Editor-Funktionen

| Feature | Scintilla | SciTE | AvalonEdit | CodeMirror 6 | Monaco | SynEdit | GtkSourceView | Ace |
|---|---|---|---|---|---|---|---|---|
| Syntax Highlighting | Ja | Ja | Ja | Ja | Ja | Ja | Ja | Ja |
| Code Folding | Ja | Ja | Ja/üblich | Ja | Ja | Ja/typisch | Ja/teils | Ja |
| Code Completion | Ja | Ja/konfigurierbar | möglich | Ja | Ja | möglich | Ja | Ja |
| Call Tips / Hover | Ja | Ja | möglich | per Extension | Ja | möglich | Completion/Info-System | möglich |
| Diagnostics / Fehleranzeigen | Ja | Ja | möglich | Linting | Marker/Diagnostics | möglich | möglich | Live Syntax Checker |
| Line Numbers | Ja/Margin | Ja | Ja | Ja | Ja | Ja | Ja | Ja |
| Gutter / Margin | Ja | Ja | Ja | Ja | Ja | Ja | Ja | Ja |
| Marker | Ja | Ja | Ja | Ja | Ja | Ja | Ja | Ja |
| Breakpoint-ähnliche Marker | Ja | Ja | möglich | möglich | möglich | möglich | möglich | möglich |
| Minimap | nicht Fokus | nicht Kern | nein/optional | nicht Kern | Ja | nein/optional | nein | nein/optional |
| Multiple Cursor | nicht historisch | abhängig/version | nicht Hauptfokus | möglich | Ja | eher nein/optional | eher nein | Ja |

### 12.5 Architekturideen

| Architekturidee | Besonders gute Vorlage |
|---|---|
| Command Dispatcher / Command Processor | Turbo Editor Toolbox |
| Hooks für Erweiterbarkeit | Turbo Editor Toolbox |
| Demoeditor als Proof of Concept | FIRST-ED, SciTE |
| Umfangreicher Demoeditor | MicroStar, SciTE |
| Native robuste Editor-Komponente | Scintilla |
| Trennung Textinhalt vs. Styling | Scintilla |
| C#/.NET/WPF-Integration | AvalonEdit |
| State/View/Transaction-Modell | CodeMirror 6 |
| Provider-Modell für intelligente Features | Monaco |
| Model/View/URI-Konzept | Monaco |
| Pascal-/Borland-Komponentendenken | SynEdit |
| Buffer/View-Konzept für GTK/Linux | GtkSourceView |
| Web-Einbettung und Keybinding-Profile | Ace |

---

## 13. Featurekatalog für ein mögliches SASD Editor Toolkit

Dieser Abschnitt übersetzt die Produktanalyse in mögliche SASD-Features.

### 13.1 Editor Core

Pflicht für ein echtes Toolkit:

- EditorDocument,
- EditorBuffer,
- TextLine,
- CaretPosition,
- SelectionRange,
- Multiple Selection optional später,
- TextChange,
- EditorTransaction,
- UndoStack,
- RedoStack,
- Dirty State,
- Document Metadata,
- Encoding-Unterstützung,
- Zeilenenden-Unterstützung,
- Tabs/Spaces-Modell,
- ReadOnly-Modus,
- Newline-Normalisierung,
- große Dateien zumindest konzeptionell vorbereiten.

### 13.2 Textpuffer und Datenstruktur

Mögliche Stufen:

#### V1: Einfaches Modell

- `List<string>` oder `List<TextLine>`,
- leicht verständlich,
- gut testbar,
- ausreichend für kleine bis mittlere Dateien,
- ideal für Lern- und MVP-Phase.

#### V2: Besseres Editiermodell

- Piece Table,
- Gap Buffer,
- Rope,
- Snapshot-Modell.

#### V3: Große Dateien

- Lazy Loading,
- Paging,
- Virtualisierung,
- Memory Mapping,
- Streaming Search,
- Indexing.

### 13.3 Commands

Pflicht:

- `IEditorCommand`,
- `CanExecute`,
- `Execute`,
- Command Context,
- Command Dispatcher,
- Keybinding Registry,
- Command History,
- Command Groups.

Standardcommands:

- MoveLeft,
- MoveRight,
- MoveUp,
- MoveDown,
- MoveWordLeft,
- MoveWordRight,
- MoveLineStart,
- MoveLineEnd,
- MoveDocumentStart,
- MoveDocumentEnd,
- InsertCharacter,
- InsertText,
- InsertNewLine,
- DeleteCharacter,
- Backspace,
- DeleteLine,
- DuplicateLine,
- Cut,
- Copy,
- Paste,
- SelectAll,
- Undo,
- Redo,
- Find,
- Replace,
- Save,
- SaveAs,
- Open,
- CloseDocument.

### 13.4 Keybindings

Mögliche Funktionen:

- Standard-Windows-Keybindings,
- Turbo/WordStar-Profil als historisches Gimmick,
- Vim-Profil später,
- Emacs-Profil später,
- frei konfigurierbare Tastaturkürzel,
- Konflikterkennung,
- Import/Export als JSON,
- Keybinding-Hilfe im UI.

### 13.5 Suche und Ersetzen

Pflicht:

- einfache Suche,
- Groß-/Kleinschreibung beachten,
- ganzes Wort,
- rückwärts suchen,
- nächster/vorheriger Treffer,
- Ersetzen,
- Alle ersetzen,
- Suche in Auswahl.

Später:

- RegExp,
- mehrzeilige Suche,
- Suche in Dateien,
- Suchtreffer-Marker im Gutter,
- Highlight aller Treffer,
- Suchhistorie,
- sichere Vorschau vor Massenersetzung.

### 13.6 Selektion und Blöcke

Pflicht:

- Zeichen-/Bereichsauswahl,
- Zeilenauswahl,
- Blockoperationen,
- Copy/Cut/Paste,
- Drag & Drop optional.

Später:

- Spalten-/Blockselektion,
- mehrere Selektionen,
- mehrere Cursor,
- Auswahl speichern/wiederherstellen,
- Auswahl transformieren.

### 13.7 Fenster und Views

In Anlehnung an die Turbo Editor Toolbox:

- mehrere Views auf dasselbe Dokument,
- eigene Cursorposition je View,
- eigene Scrollposition je View,
- Split View,
- horizontales/vertikales Splitten,
- zwei Dokumente vergleichen,
- unsichtbare Textpuffer für interne Operationen.

### 13.8 Status und Dirty State

Pflicht:

- Dokument geändert / ungespeichert,
- Dateipfad,
- Zeile/Spalte,
- Encoding,
- Zeilenende,
- Einfügemodus/Überschreibmodus,
- ReadOnly-Status,
- Sprache/Modus,
- Fehler-/Warnungsanzahl.

### 13.9 Syntax Highlighting

Stufen:

#### V1

- kein Syntax Highlighting oder sehr einfache Regeln.

#### V2

- regelbasierte Highlighter,
- JSON/XML/Markdown/C#/SQL/Bash als erste Modi.

#### V3

- echte Parser-/Tree-Sitter-artige Struktur,
- semantische Hervorhebung,
- Theme-System.

### 13.10 Diagnostics und Marker

Sinnvolle Features:

- Warnungen,
- Fehler,
- Infohinweise,
- Suchtreffer,
- Bookmarks,
- Breakpoint-Marker,
- aktuelle Zeile,
- geänderte Zeilen,
- ungespeicherte Änderungen,
- Linter-Ergebnisse,
- Security-Hinweise.

### 13.11 Completion und Call Tips

Mögliche Stufen:

#### V1

- keine Completion.

#### V2

- statische Wortliste,
- Vorschläge aus Dokument,
- Snippets.

#### V3

- Provider-Schnittstelle,
- sprachspezifische Completion,
- Hover/Tooltips,
- Call Tips,
- LSP-Anbindung.

### 13.12 Folding

Mögliche Features:

- manuelles Folding,
- Folding nach Einrückung,
- Folding nach Markdown-Überschriften,
- Folding nach Klammern,
- Folding nach Regionen,
- gespeicherter Folding-State.

### 13.13 Snippets

Sinnvolle Funktionen:

- Snippet-Katalog,
- Platzhalter,
- Tab-Stops,
- Variablen,
- Projektsnippets,
- Benutzersnippets,
- Import/Export.

### 13.14 Themes und Darstellung

Mögliche Funktionen:

- Light/Dark Theme,
- SASD Theme,
- Schriftart,
- Schriftgröße,
- Zeilenhöhe,
- Tab-Größe,
- sichtbare Whitespaces,
- sichtbare Zeilenenden,
- aktueller Zeilenhintergrund,
- Auswahlfarbe,
- Gutter-Farbe,
- Marker-Farben,
- High-Contrast-Modus.

### 13.15 Accessibility

Sollte früh berücksichtigt werden:

- Tastaturbedienbarkeit,
- ausreichender Kontrast,
- Screenreader-Kompatibilität,
- sichtbarer Fokus,
- skalierbare Schrift,
- keine reine Farbcodierung für wichtige Zustände,
- reduzierte Animation,
- klare Statusmeldungen.

### 13.16 Datei- und Encoding-Funktionen

Sinnvolle Funktionen:

- UTF-8,
- UTF-8 mit BOM,
- UTF-16,
- Windows-1252 optional,
- Encoding-Erkennung,
- Zeilenenden CRLF/LF,
- Datei neu laden,
- externe Änderungen erkennen,
- Schreibschutz erkennen,
- automatische Backups,
- Atomic Save,
- Save As,
- Export.

### 13.17 Sicherheit und Datenschutz

Für SASD wichtig:

- keine stillen Cloud-Uploads,
- lokale Verarbeitung als Standard,
- sichere Behandlung sensibler Texte,
- keine Logausgabe von kompletten Dokumentinhalten,
- Secret Scanner optional,
- Warnung bei API Keys/Passwörtern,
- sicherer Clipboard-Umgang optional,
- Crash Reports ohne sensible Inhalte,
- klare Datenablage in `%LocalAppData%` oder projektbezogen.

### 13.18 Dokumentation und Lernwert

Da SASD-Projekte oft auch Lern-/Referenzcharakter haben:

- Architektur-Dokumentation,
- kommentierter Code,
- XML-Dokumentationskommentare,
- README,
- Lastenheft,
- Pflichtenheft,
- Roadmap,
- ADRs,
- API-Dokumentation,
- Beispielanwendungen,
- Tutorial „Build your first editor“,
- Tutorial „Add a new command“,
- Tutorial „Add syntax highlighting“,
- Tutorial „Embed editor in your app“.

---

## 14. Mögliche SASD-Roadmap

### Phase 0 – Projektgrundlage

- Repository anlegen,
- README,
- Lizenzentscheidung,
- Lastenheft,
- Pflichtenheft,
- Architekturüberblick,
- Roadmap,
- erste ADRs,
- Solution-Struktur.

### Phase 1 – Minimaler Editor Core

- Textdokument,
- Zeilenmodell,
- Cursorposition,
- Insert/Delete,
- NewLine,
- einfache Tests,
- keine UI-Abhängigkeit.

### Phase 2 – Commandsystem

- Command Interface,
- Command Dispatcher,
- Keybinding-Modell,
- Standardcommands,
- Unit Tests.

### Phase 3 – Datei und Dirty State

- Datei öffnen,
- Datei speichern,
- Encoding-Grundlage,
- Dirty State,
- Save/Save As,
- externe Änderung später vorbereiten.

### Phase 4 – WinForms- oder WPF-Demo-FIRST-ED

- einfache Desktop-App,
- Editoranzeige,
- Tastatureingabe,
- Menü,
- Statuszeile,
- Datei öffnen/speichern.

### Phase 5 – Suche, Ersetzen, Blockoperationen

- Find,
- Replace,
- SelectAll,
- Copy/Cut/Paste,
- DeleteLine,
- Move/Copy Block,
- UI-Dialoge.

### Phase 6 – Undo/Redo

- TextChange-Modell,
- UndoStack,
- RedoStack,
- Gruppierung von Änderungen,
- Tests.

### Phase 7 – MicroStar-ähnliche Demo-App

- Menüs,
- Toolbar,
- Kontextmenüs,
- Pop-up-Dialoge,
- mehrere Dokumente,
- Split View,
- Statusanzeige,
- Konfiguration.

### Phase 8 – Syntax Highlighting V1

- einfache regelbasierte Highlighter,
- Markdown,
- JSON,
- XML,
- C#,
- SQL,
- Bash.

### Phase 9 – Marker, Gutter, Line Numbers

- Zeilennummern,
- Gutter,
- Bookmarks,
- Suchtreffer,
- Warnungen,
- aktuelle Zeile.

### Phase 10 – Completion/Snippets V1

- Wortvorschläge,
- Snippet-Katalog,
- statische Provider-Schnittstelle.

### Phase 11 – Professionalisierung

- API-Dokumentation,
- NuGet-Paket,
- Beispielprojekte,
- Performance-Tests,
- Accessibility-Review,
- Security-Review.

---

## 15. Priorisierung für ein realistisches MVP

Für ein SASD-MVP sollte man bewusst klein starten.

### Muss für MVP

- Editor Core ohne UI-Abhängigkeit,
- Textdokument,
- Cursor,
- Einfügen/Löschen,
- Newline,
- Datei öffnen/speichern,
- Dirty State,
- Commandsystem,
- einfache WinForms- oder WPF-Demo,
- Unit Tests,
- README,
- Architektur-Dokumentation.

### Sollte früh kommen

- Undo/Redo,
- Suche,
- Ersetzen,
- Copy/Cut/Paste,
- Auswahl,
- Statuszeile,
- Keybindings,
- einfache Konfiguration.

### Kann später kommen

- Syntax Highlighting,
- Folding,
- Completion,
- Snippets,
- mehrere Cursor,
- große Dateien,
- LSP,
- Minimap,
- Web-Version,
- Vim/Emacs-Modus,
- mehrere Frontends.

### Nicht im MVP

- VS-Code-Konkurrenz,
- komplette IDE,
- komplexe Sprachserver,
- perfekter Large-File-Support,
- Plugin-Marketplace,
- Cloud-Synchronisation,
- kollaboratives Echtzeit-Editing.

---

## 16. Empfehlung für SASD

### 16.1 Beste Vorlagen je Ziel

| Ziel | Beste Vorlage |
|---|---|
| Historische Grundidee | Turbo Editor Toolbox |
| Minimaler Demoeditor | FIRST-ED |
| Größerer Demoeditor | MicroStar / SciTE |
| Native robuste Editor-Komponente | Scintilla |
| C#/.NET-Desktop | AvalonEdit |
| Moderne Architektur | CodeMirror 6 |
| High-End-Codefeatures | Monaco |
| Borland-/Pascal-Nähe | SynEdit |
| Linux/GTK | GtkSourceView |
| Web-Einbettung | Ace |

### 16.2 Empfohlener SASD-Ansatz

Der beste Weg wäre ein eigenständiger, moderner Editor-Baukasten:

```text
Sasd.EditorKit.Core
Sasd.EditorKit.Application
Sasd.EditorKit.Rendering
Sasd.EditorKit.WinForms
Sasd.EditorKit.Wpf
Sasd.EditorKit.Tests
Sasd.EditorKit.Samples
```

Starten sollte man mit:

- Core,
- Commands,
- Dateioperationen,
- Tests,
- einfache Demo-App.

Später können Rendering, Syntax Highlighting und intelligente Features wachsen.

### 16.3 Strategische Bewertung

Ein eigenes SASD Editor Toolkit ist sinnvoll, wenn das Ziel ist:

- Lernen,
- eigene Toolbasis,
- wiederverwendbare SASD-Komponente,
- Prompt Manager / Log Viewer / Markdown Editor / Config Editor unterstützen,
- langfristige Architekturkompetenz aufbauen.

Es ist weniger sinnvoll, wenn das Ziel nur ist:

- schnell einen Editor in einer App zu haben.

Dann wäre die Verwendung von AvalonEdit, ScintillaNET, Monaco, CodeMirror oder Ace vermutlich schneller.

---

## 17. Rechtliche und praktische Hinweise

Wichtig:

- keine Namen, Logos, Texte oder Quellcodes historischer Produkte kopieren,
- nur Konzepte und allgemeine Architekturideen übernehmen,
- Lizenzbedingungen jeder Bibliothek prüfen,
- bei Codeübernahme Lizenzkompatibilität prüfen,
- für SASD-eigenen Code eine eigene klare Lizenzentscheidung treffen,
- Dokumentation sauber formulieren: „inspiriert von klassischen Editor-Toolbox-Konzepten“, nicht „Kopie von Borland“.

---

## 18. Quellen und Recherchehinweise

Die Analyse basiert auf dem bereitgestellten Handbuch zur **Turbo Editor Toolbox Version 1.0** sowie auf offiziellen Projektseiten bzw. Projekt-Repositories der betrachteten modernen Komponenten.

### Lokale/hochgeladene Quelle

- Borland Turbo Editor Toolbox Version 1.0, Owner's Handbook, 1985.

### Öffentliche Projektquellen

- Scintilla / SciTE: https://scintilla.org/
- SciTE: https://scintilla.org/SciTE.html
- AvalonEdit GitHub: https://github.com/icsharpcode/AvalonEdit
- AvalonEdit Homepage: https://avalonedit.net/
- CodeMirror: https://codemirror.net/
- CodeMirror Reference Manual: https://codemirror.net/docs/ref/
- Monaco Editor: https://microsoft.github.io/monaco-editor/
- Monaco Editor GitHub: https://github.com/microsoft/monaco-editor
- TurboPack SynEdit: https://github.com/TurboPack/SynEdit
- GtkSourceView GitHub: https://github.com/GNOME/gtksourceview
- GtkSourceView GNOME/GitLab: https://gitlab.gnome.org/GNOME/gtksourceview
- Ace Editor: https://ace.c9.io/
- Ace Editor GitHub: https://github.com/ajaxorg/ace

---

## 19. Kurzfazit

Die Turbo Editor Toolbox war ihrer Zeit voraus, weil sie nicht nur ein Editor war, sondern ein Editor-Baukasten. Genau diese Idee ist heute noch wertvoll.

Ein modernes SASD Editor Toolkit sollte nicht versuchen, sofort ein VS-Code-Konkurrent zu werden. Der sinnvollere Weg ist:

1. kleiner, sauberer Editor Core,
2. gutes Commandsystem,
3. UI-unabhängiges Textmodell,
4. einfache Desktop-Demo,
5. danach Suche, Undo/Redo, Blockoperationen,
6. später Syntax Highlighting, Completion, Folding und intelligente Provider.

Als Vorlagenmix ist besonders sinnvoll:

- **Turbo Editor Toolbox** für die Grundidee,
- **AvalonEdit** für C#/.NET/WPF,
- **Scintilla** für robuste Editor-Komponenten,
- **CodeMirror 6** für moderne Architektur,
- **Monaco** für Provider-/Model-Ideen,
- **SynEdit** für Borland-/Pascal-Tradition,
- **GtkSourceView** für Buffer/View und Linux/GTK,
- **Ace** für Web-Einbettung und leichtgewichtige Browserintegration.

Damit hätte SASD eine sehr gute Grundlage für eigene Werkzeuge wie Prompt Manager, Log Viewer, Markdown Editor, Konfigurationseditor und spätere IDE-nahe Anwendungen.
