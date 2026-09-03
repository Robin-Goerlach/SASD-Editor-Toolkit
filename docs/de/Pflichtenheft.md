# Pflichtenheft
## SASD Editor Toolkit - technische Umsetzung der modernen Turbo-Editor-Toolbox-Idee

| Merkmal | Festlegung |
|---|---|
| Dokumenttyp | Pflichtenheft / technische und organisatorische Realisierungsvorgabe |
| Projekt | SASD Editor Toolkit |
| Dokumentversion | 0.1 |
| Status | Ausführlicher Arbeitsstand zur Architektur- und Implementierungsfreigabe |
| Datum | 24.07.2026 |
| Basis | Lastenheft SASD Editor Toolkit Version 0.1 |
| Quellnachweis | `Lastenheft_SASD_Editor_Toolkit_Version_0.1.md`, SHA-256 `0d70684646c59d43f8144d51036b8636abda5a9f6df5f32b55d652b35fd167f3` |
| Zielplattform Core | .NET 10 (`net10.0`) |
| Erster UI-Adapter | WinForms (`net10.0-windows`) |
| Spätere Adapter | WPF sowie Blazor/Razor; ASPX/Web Forms als dokumentierter Kompatibilitätsweg |
| Projektsprache | Englisch für Code, öffentliche APIs und Paketnamen; Dokumentation Deutsch und Englisch |
| Repository | `SASD-Editor-Toolkit` als eigenständiges Repository |

> **Bezug zum Lastenheft:** Dieses Pflichtenheft beschreibt, **wie** die Anforderungen des Lastenhefts realisiert, geprüft, paketiert und schrittweise ausgeliefert werden. Jede formale Lastenheft-Anforderung wird in der vollständigen Traceability-Matrix in Kapitel 38 geführt. Anforderungen werden nicht stillschweigend gestrichen; spätere oder optionale Funktionen erhalten einen ausdrücklichen Meilenstein.

---

## 1. Zweck und Geltungsbereich
Dieses Dokument konkretisiert die technische Umsetzung des SASD Editor Toolkit. Es gilt für den UI-unabhängigen Editor-Kern, den ersten WinForms-Adapter, die Referenzanwendungen, Tests, Dokumentation, Paketierung sowie die vorgesehenen Erweiterungspunkte für WPF und Web.

Das Pflichtenheft legt verbindlich fest:
- die Architektur und Abhängigkeitsrichtung,
- die Repository- und Solution-Struktur,
- die fachlichen Modelle und öffentlichen Schnittstellen,
- die Umsetzung der Editorbefehle,
- Datei-, Undo-, Such-, Mehransichts- und Renderingkonzepte,
- Qualitäts-, Sicherheits-, Test- und Dokumentationsmaßnahmen,
- die Meilensteinzuordnung aller Lastenheft-Anforderungen.

Nicht Gegenstand dieses Dokuments ist die detaillierte Gestaltung einzelner Icons, Marketingtexte oder die vollständige technische Spezifikation späterer WPF- und Webadapter. Deren Integrationsverträge werden jedoch bereits verbindlich definiert.

## 2. Vollständigkeits- und Änderungsmanagement
Das Lastenheft enthält **208 formale Produkt-, Funktions- und Qualitätsanforderungen**, **15 Use Cases**, **15 Abnahmekriterien** und **7 Meilensteine**. Dieses Pflichtenheft übernimmt alle diese Elemente.

Änderungen an Anforderungen erfolgen nach folgendem Verfahren:
1. Änderungsvorschlag mit betroffenen IDs erfassen.
2. Auswirkung auf Architektur, API, Tests, Dokumentation und Meilenstein bewerten.
3. Lastenheft und Pflichtenheft gemeinsam versionieren.
4. Traceability-Matrix aktualisieren.
5. Breaking Changes zusätzlich im `CHANGELOG.md` und in einer Architecture Decision Record dokumentieren.

| Nachweistyp | Verbindlicher Speicherort |
|---|---|
| Lastenheft | `docs/de/Lastenheft.md` |
| Pflichtenheft | `docs/de/Pflichtenheft.md` |
| Architekturentscheidungen | `docs/adr/ADR-XXXX-*.md` |
| Anforderungs-Traceability | `docs/de/Historical-Traceability.md` und Anhang dieses Dokuments |
| Testnachweise | automatisierte Tests und CI-Artefakte |
| Release-Nachweise | `CHANGELOG.md`, Release Notes und NuGet-Metadaten |

## 3. Lösungsstrategie
Die Lösung wird nicht als WinForms-Anwendung mit eingebauter Editorlogik, sondern als Produktfamilie aus einem fachlichen Kern, einem Plattformadapter und Beispielhosts umgesetzt.

```text
Host / Sample / SASD Desktop Components
                |
                v
Sasd.EditorToolkit.WinForms
                |
                v
Sasd.EditorToolkit.Core
                |
                v
BCL + optionale Hostdienste
```

Kernentscheidungen:
- **Single Source of Truth:** Das `TextDocument` im Core ist alleinige fachliche Textquelle.
- **Mehransichtsfähigkeit:** Mehrere `EditorViewState`-Instanzen referenzieren dasselbe Dokument.
- **Command-first:** Menüs, Tastatur, Toolbar, Kontextmenüs und Host-API lösen dieselben Command-IDs aus.
- **Custom WinForms Rendering:** Der erste produktive Adapter erhält eine eigene Editorfläche statt den Core an `RichTextBox` zu koppeln.
- **Austauschbarer Textpuffer:** Version 0.1 verwendet einen zeilenorientierten Puffer; die Schnittstelle erlaubt ab Version 1.0 Piece-Table-/Rope-Implementierungen.
- **Keine historischen Technikaltlasten:** Keine DOS-Overlays, BIOS-Aufrufe, Pointerlisten, festen 80x25-Raster oder globalen Editorvariablen.
- **Schrittweise Auslieferung:** Modern FIRST-ED in M1, historische Funktionsbreite in M2, Stabilisierung und Large-File-Pfad in M3.

## 4. Technologiefestlegungen
| Bereich | Festlegung | Begründung |
|---|---|---|
| Programmiersprache | C# mit aktiviertem Nullable Reference Types | Sichere öffentliche API und gute Wartbarkeit |
| Core Target Framework | `net10.0` | Plattformneutraler moderner .NET-Kern |
| WinForms Target Framework | `net10.0-windows` mit `UseWindowsForms=true` | Erste verbindliche Referenzoberfläche |
| Tests | xUnit | Bereits im SASD-Umfeld verwendet, gut automatisierbar |
| Serialisierung | `System.Text.Json` | BCL-nah, keine unnötige Runtime-Abhängigkeit |
| Logging | `Microsoft.Extensions.Logging.Abstractions` oder äquivalente dünne Adaptergrenze | Integration in Desktop-, Server- und Webhosts |
| Datei-I/O | asynchrone `Stream`-/`FileStream`-APIs mit `CancellationToken` | Reaktionsfähigkeit und Headless-Nutzung |
| WinForms Rendering | GDI/TextRenderer-basierte eigene EditorSurface mit Layoutcache | Kontrollierter Zustand, mehrere Views und spätere Adapterfähigkeit |
| Build | `dotnet` CLI, deterministische Builds, zentrale `Directory.Build.props` | Reproduzierbarkeit |
| CI | GitHub Actions | Build, Tests, Pack, Dokumentations- und Architekturchecks |
| Pakete | NuGet-Pakete für Core und WinForms | Getrennte Wiederverwendung und Versionierung |

Der Core soll außer der Logging-Abstraktion keine zwingenden Drittanbieter-Runtime-Pakete benötigen. Test- und Buildwerkzeuge dürfen als Development Dependencies verwendet werden.

## 5. Repository- und Solution-Struktur
```text
SASD-Editor-Toolkit/
├── .github/
│   ├── workflows/
│   └── ISSUE_TEMPLATE/
├── artefacts/
│   ├── screenshots/
│   └── diagrams/
├── docs/
│   ├── adr/
│   ├── de/
│   └── en/
├── samples/
│   ├── Sasd.EditorToolkit.Sample.FirstEd.WinForms/
│   └── Sasd.EditorToolkit.Sample.MicroStar.WinForms/   # ab M2
├── src/
│   ├── Sasd.EditorToolkit.Core/
│   └── Sasd.EditorToolkit.WinForms/
├── tests/
│   ├── Sasd.EditorToolkit.Core.Tests/
│   ├── Sasd.EditorToolkit.WinForms.Tests/
│   ├── Sasd.EditorToolkit.Integration.Tests/
│   └── Sasd.EditorToolkit.Architecture.Tests/
├── Directory.Build.props
├── Directory.Packages.props
├── global.json
├── SASD-Editor-Toolkit.slnx
├── README.md
├── CHANGELOG.md
├── CONTRIBUTING.md
├── SECURITY.md
└── LICENSE
```

Ein separates `Abstractions`-Projekt wird in Version 0.1 bewusst nicht angelegt. Schnittstellen liegen im Core in klaren Namespaces. Erst wenn mehrere unabhängig versionierte Pakete eine echte gemeinsame Vertragsassembly benötigen, wird dies über eine ADR entschieden.

## 6. Projektverantwortlichkeiten und Abhängigkeitsregeln
| Projekt | Verantwortung | Darf referenzieren |
|---|---|---|
| `Sasd.EditorToolkit.Core` | Dokument-, Puffer-, View-State-, Command-, Suche-, Undo-, Datei- und Einstellungslogik | BCL und freigegebene Logging-Abstraktion |
| `Sasd.EditorToolkit.WinForms` | Rendering, Tastatur/Maus/IME, Clipboard, Scrollbars, Dialogadapter, Accessibility | Core und WinForms |
| `Sample.FirstEd.WinForms` | Kleine Referenzanwendung für M1 | Core und WinForms |
| `Sample.MicroStar.WinForms` | Erweiterte Menü-/Mehrfenster-/Kompatibilitätsdemo ab M2 | Core und WinForms |
| Tests | Automatisierter Funktions-, Integrations- und Architekturbeweis | jeweilige Zielprojekte und Testbibliotheken |

Verbindliche Regeln:
- Core referenziert niemals WinForms, WPF, ASP.NET oder Browsertypen.
- WinForms enthält keine zweite Dokument- oder Undo-Implementierung.
- Samples dürfen keine produktive Kernlogik enthalten.
- SASD Desktop Components konsumiert das NuGet-Paket oder eine Projekt-Referenz; keine Quellcodeduplikation.
- Data Toolbox, Numerics, Graphics und GameWorks sind keine Pflichtabhängigkeiten.
- Erweiterungen registrieren Commands und Dienste über öffentliche Verträge; sie greifen nicht auf interne Zustände zu.

## 7. Namespace- und API-Konventionen
```text
Sasd.EditorToolkit
Sasd.EditorToolkit.Text
Sasd.EditorToolkit.Documents
Sasd.EditorToolkit.Editing
Sasd.EditorToolkit.Commands
Sasd.EditorToolkit.Input
Sasd.EditorToolkit.Search
Sasd.EditorToolkit.Undo
Sasd.EditorToolkit.Storage
Sasd.EditorToolkit.Settings
Sasd.EditorToolkit.Diagnostics
Sasd.EditorToolkit.WinForms
Sasd.EditorToolkit.WinForms.Rendering
Sasd.EditorToolkit.WinForms.Input
Sasd.EditorToolkit.WinForms.Accessibility
```

Öffentliche Typen verwenden englische Namen. Öffentliche APIs erhalten XML-Dokumentation mit Zweck, Parametern, Rückgabe, Fehlerzuständen, Threading- und Cancellation-Verhalten. Interne Implementierungsdetails werden standardmäßig `internal` gehalten.

## 8. Fachliches Objektmodell
| Typ | Art | Verantwortung |
|---|---|---|
| `TextDocument` | Klasse | Besitzt Textpuffer, Metadaten, UndoManager, Marker, Dirty State und Ereignisse. |
| `DocumentId` | readonly record struct | Stabile Identität unabhängig vom Dateipfad. |
| `DocumentMetadata` | Klasse/Record | Anzeigename, Pfad, Encoding, Zeilenendungsstrategie, Zeitstempel und Dateifingerprint. |
| `ITextBuffer` | Schnittstelle | Lesen und atomare Änderung von Text ohne UI-Abhängigkeit. |
| `LineTextBuffer` | Klasse | Standardpuffer M1; Liste von Zeilen mit eigenen Zeilenendungen. |
| `TextPosition` | readonly record struct | Nullbasierte Zeile und UTF-16-Spalte; validierbar und normalisierbar. |
| `TextRange` | readonly record struct | Normalisierter halb-offener Bereich `[Start, End)`. |
| `TextSelection` | Klasse/Record | Anker, aktive Position, Richtung und Modus. |
| `TextChange` | Record | Startposition, entfernter Text, eingefügter Text und Änderungsmetadaten. |
| `TextChangeSet` | Record | Atomare Gruppe zusammengehöriger Änderungen. |
| `TextAnchor` | Klasse | Bei Änderungen nachgeführte Position mit Vorwärts-/Rückwärts-Affinität. |
| `TextMarker` | Klasse | Benannter/nummerierter Marker auf Basis eines Anchors. |
| `TextAnnotation` | Abstraktion | Erweiterbare Hervorhebung oder Metadaten für Bereich/Zeile. |
| `EditorViewState` | Klasse | Cursor, Auswahl, Scrollen, Ränder, Tabs, Modi und Anzeigeoptionen einer View. |
| `EditorWorkspace` | Klasse | Verwaltet Dokumente, Views, aktive View und Schließabläufe. |
| `EditorSession` | Klasse | Optional persistierbarer Workspace-/Layoutzustand. |

`TextDocument` und `EditorViewState` sind strikt getrennt. Ein Dokument kann ohne View und eine View niemals ohne Dokument existieren.

## 9. Textpuffer und Textrepräsentation
### 9.1 Interne Repräsentation
- Text wird intern als Unicode-.NET-Text (`string`, UTF-16) gespeichert.
- Jede logische Zeile speichert ihren Inhalt ohne Terminator sowie optional `LineEndingKind` (`None`, `CrLf`, `Lf`, `Cr`).
- Gemischte Zeilenenden bleiben beim Laden erhalten. Eine Save-Policy kann `Preserve`, `NormalizeToCrLf`, `NormalizeToLf` oder `NormalizeToCr` wählen.
- Leere Dokumente enthalten fachlich mindestens eine adressierbare leere Zeile.
- Es gibt keine feste 255-Zeichen-Grenze.
- Textpositionen sind nullbasiert. Benutzeroberflächen zeigen standardmäßig einsbasierte Zeilen- und Spaltennummern.

### 9.2 ITextBuffer-Vertrag
```csharp
public interface ITextBuffer
{
    int LineCount { get; }
    long Version { get; }
    int Length { get; }

    ReadOnlyMemory<char> GetLineText(int lineIndex);
    LineEndingKind GetLineEnding(int lineIndex);
    string GetText(TextRange range);
    TextPosition Normalize(TextPosition position);
    int GetOffset(TextPosition position);
    TextPosition GetPosition(int offset);

    TextChangeSet Insert(TextPosition position, string text);
    TextChangeSet Delete(TextRange range);
    TextChangeSet Replace(TextRange range, string text);

    event EventHandler<TextBufferChangedEventArgs>? Changed;
}
```

Änderungen sind atomar. Der Puffer erzeugt ein `TextChangeSet`, das von Undo, Marker, Views und Rendering ausgewertet wird. Direkte mutierende Listen- oder Stringreferenzen werden nicht nach außen gegeben.

### 9.3 Caret- und Unicode-Regeln
- Öffentliche Spaltenwerte sind UTF-16-Indizes, damit sie verlustfrei mit .NET-APIs interoperieren.
- Caretbewegungen verwenden gültige Text-Element-Grenzen und dürfen Surrogatpaare oder kombinierte Sequenzen nicht absichtlich teilen.
- `VisualColumnCalculator` berechnet die sichtbare Spalte unter Berücksichtigung von Tabs und optional proportionaler Schrift.
- Wortgrenzen werden über eine austauschbare `IWordBoundaryService` bestimmt; Standard ist Unicode-orientiert und konfigurierbar.

### 9.4 Large-File-Pfad
M1 verwendet `LineTextBuffer` für einfache Nachvollziehbarkeit. Ab M3 wird `PieceTableTextBuffer` als zweite Implementierung vorgesehen. Auswahl erfolgt über eine `ITextBufferFactory`, beispielsweise abhängig von Dateigröße und Hostoptionen. Öffentliche Dokument- und Command-APIs dürfen von dieser Wahl nicht abhängen.

## 10. Dokumentlebenszyklus
Der Dokumentlebenszyklus besteht aus `New`, `Load`, `Edit`, `Save`, `SaveAs`, `Reload`, `Close` und `Dispose`.

```text
New/Load
   -> Clean document + savepoint
   -> Editing changes buffer and undo stack
   -> Dirty document
   -> Save succeeds: update metadata + savepoint
   -> Close request: Save / Discard / Cancel
   -> Close and dispose when no view/reference remains
```

Beim Schließen liefert der Core keinen Dialog, sondern ein `CloseDecisionRequired`-Ergebnis. Der Host fragt den Benutzer und ruft den Ablauf mit `Save`, `Discard` oder `Cancel` erneut auf. So bleibt der Core UI-frei.

## 11. Workspace, Views und Mehrfensterfähigkeit
`EditorWorkspace` verwaltet Dokumente und Views. Eine View erhält eine eigene `ViewId`, verweist auf genau ein `TextDocument` und besitzt einen unabhängigen `EditorViewState`.

| Operation | Umsetzung |
|---|---|
| Neue Datei | Neues `TextDocument` plus erste View erzeugen. |
| Zweite View desselben Dokuments | Neuen `EditorViewState` an bestehendes Dokument binden. |
| View schließen | Nur View entfernen; Dokument bleibt erhalten, wenn weitere Views oder Hostreferenzen existieren. |
| Dokument schließen | Alle zugehörigen Views schließen; Dirty-Guard anwenden. |
| View vor/zurück | Workspace führt geordnete View-Liste und aktive View. |
| View direkt anspringen | Navigation über `ViewId`, Index oder Hostauswahl. |
| Verborgene View | ViewState bleibt im Workspace, Renderinghost ist nicht angehängt. |
| Split View | WinForms-Sample erzeugt zweite View im `SplitContainer`. |

Textänderungen werden als Dokumentereignis an alle Views verteilt. Jede View invalidiert nur die betroffenen Layoutbereiche und normalisiert Cursor und Auswahl.

## 12. Undo, Redo und Dirty State
### 12.1 Undo-Architektur
Jede mutierende Operation läuft innerhalb einer `UndoTransaction`. Die Transaktion sammelt ein oder mehrere `TextChangeSet`-Objekte und zusätzliche View-Änderungen, soweit diese fachlich rückgängig zu machen sind.

```csharp
public interface IUndoUnit
{
    string Description { get; }
    long EstimatedBytes { get; }
    void Undo(TextDocument document);
    void Redo(TextDocument document);
}
```

### 12.2 Regeln
- Mehrere direkt aufeinanderfolgende Texteingaben werden zeit- und positionsabhängig zusammengefasst.
- Einfügen, Löschen, Paste, Blockoperationen, Replace und Reformat sind undo-fähig.
- Redo wird beim Ausführen einer neuen Änderung nach Undo verworfen.
- Das Limit ist nach Operationszahl und geschätztem Speicherbudget konfigurierbar.
- Der `Savepoint` ist ein stabiler Index/Fingerprint im Undo-Verlauf. `IsDirty` ergibt sich aus aktuellem Zustand gegenüber diesem Savepoint und nicht aus einem manuell gesetzten Boolean allein.
- Hostcommands müssen mutierende Operationen über die Dokument-API ausführen, damit Undo und Dirty State vollständig bleiben.

## 13. Auswahl, Block und Zwischenablage
Die Standardauswahl ist zeichenweise und halb-offen. Der historische zeilenorientierte Blockmodus wird als `SelectionMode.LineBlock` ab M2 ergänzt.

| Funktion | Technische Umsetzung |
|---|---|
| Auswahlbeginn/-ende | `TextSelection` mit Anchor und ActivePosition |
| Auswahl erweitern | NavigationCommand mit `ExtendSelection=true` |
| Copy | Text aus normalisiertem Range an `IClipboardService` |
| Cut | Copy und atomare Delete-Transaktion |
| Paste | Unicode-Text normalisieren und atomar einfügen |
| Move | Bei gleichem Dokument Zielposition vor Delete/Insert korrekt transformieren |
| Hide/Show Block | Auswahl bleibt fachlich erhalten; Renderingoption steuert Sichtbarkeit |
| Dokumentübergreifend | Quelltext als unveränderlicher Snapshot; Zieloperation eigene Undo-Transaktion |

Der Core kennt keine Betriebssystemzwischenablage. WinForms stellt `WinFormsClipboardService`; Headless-Tests verwenden `InMemoryClipboardService`.

## 14. Command-System
### 14.1 Kernverträge
```csharp
public readonly record struct EditorCommandId(string Value);

public interface IEditorCommand
{
    EditorCommandId Id { get; }
    bool CanExecute(EditorCommandContext context);
    ValueTask<CommandResult> ExecuteAsync(
        EditorCommandContext context,
        CancellationToken cancellationToken);
}

public interface ICommandInterceptor
{
    ValueTask<CommandInterceptionResult> BeforeExecuteAsync(
        EditorCommandInvocation invocation,
        CancellationToken cancellationToken);
}
```

### 14.2 Dispatcherablauf
1. Inputadapter löst KeyGesture oder API-Command aus.
2. `KeyboardSequenceResolver` entscheidet, ob ein vollständiger Command, ein Präfixzustand oder keine Zuordnung vorliegt.
3. `EditorCommandDispatcher` baut den Kontext aus Workspace, aktiver View, Dokument und Hostdiensten.
4. Interceptors dürfen ersetzen, blockieren, als verarbeitet markieren oder weiterleiten.
5. `CanExecute` wird geprüft.
6. Command wird mit CancellationToken ausgeführt.
7. Ergebnis, Fehler, Status- und Änderungsereignisse werden veröffentlicht.

### 14.3 Tastaturprofile
`KeyboardProfile` enthält serialisierbare Zuordnungen von `KeySequence` zu `EditorCommandId`. Profile werden als JSON geladen. M1 liefert `Modern.json`; M2 liefert zusätzlich `TurboEditorCompatible.json`. Präfixsequenzen besitzen einen konfigurierbaren Timeout, werden durch Escape abgebrochen und zeigen im Host optional den erwarteten zweiten Tastendruck an.

## 15. Command-Katalog
| Command-ID | Bedeutung | Meilenstein |
|---|---|---|
| File.New | Neues Dokument | M1 |
| File.Open | Datei öffnen | M1 |
| File.Save | Speichern | M1 |
| File.SaveAs | Speichern unter | M1 |
| File.Close | Dokument/View schließen | M1 |
| App.Exit | Host-Schließablauf anstoßen | M1 |
| Edit.InsertText | Text einfügen/überschreiben | M1 |
| Edit.NewLine | Neue Zeile | M1 |
| Edit.InsertLine | Leere Zeile einfügen | M1 |
| Edit.DeleteLeft | Links löschen | M1 |
| Edit.DeleteRight | Rechts löschen/Zeilen verbinden | M1 |
| Edit.DeleteLine | Zeile löschen | M1 |
| Edit.DeleteToLineEnd | Bis Zeilenende löschen | M1 |
| Edit.DeleteWordRight | Wort rechts löschen | M1 |
| Edit.ChangeCase | Groß-/Kleinschreibung ändern | M2 |
| Edit.InsertControlCharacter | Steuerzeichen einfügen | M2 |
| Edit.Undo | Rückgängig | M1 |
| Edit.Redo | Wiederholen | M1 |
| Edit.Cut | Ausschneiden | M1 |
| Edit.Copy | Kopieren | M1 |
| Edit.Paste | Einfügen | M1 |
| Edit.SelectAll | Alles auswählen | M1 |
| Navigate.Left | Zeichen links | M1 |
| Navigate.Right | Zeichen rechts | M1 |
| Navigate.Up | Zeile hoch | M1 |
| Navigate.Down | Zeile runter | M1 |
| Navigate.WordLeft | Wort links | M1 |
| Navigate.WordRight | Wort rechts | M1 |
| Navigate.LineStart | Zeilenanfang | M1 |
| Navigate.LineEnd | Zeilenende | M1 |
| Navigate.DocumentStart | Dokumentanfang | M1 |
| Navigate.DocumentEnd | Dokumentende | M1 |
| Navigate.PageUp | Seite hoch | M1 |
| Navigate.PageDown | Seite runter | M1 |
| Navigate.GoToLine | Zu Zeile | M1 |
| Navigate.GoToColumn | Zu Spalte | M2 |
| Navigate.SelectionStart | Zum Auswahlbeginn | M2 |
| Navigate.SelectionEnd | Zum Auswahlende | M2 |
| Search.Find | Suchen | M1 |
| Search.FindNext | Nächster Treffer | M1 |
| Search.FindPrevious | Vorheriger Treffer | M2 |
| Search.Replace | Einzeln ersetzen | M1 |
| Search.ReplaceAll | Alle ersetzen | M1 |
| Marker.Set | Marker setzen | M2 |
| Marker.GoTo | Marker anspringen | M2 |
| Format.ToggleWordWrap | Word-Wrap umschalten | M2 |
| Format.ToggleAutoIndent | Auto-Indent umschalten | M2 |
| Format.ReformatParagraph | Absatz reformatieren | M2 |
| Format.CenterLine | Zeile zentrieren | M2 |
| Format.SetLeftMargin | Linken Rand setzen | M2 |
| Format.SetRightMargin | Rechten Rand setzen | M2 |
| Format.SetTabSize | Tabulatorbreite setzen | M2 |
| Edit.ToggleInsertMode | Insert/Overwrite umschalten | M1 |
| View.New | Neue View | M2 |
| View.Split | Geteilte View | M2 |
| View.Close | View schließen | M1 |
| View.Next | Nächste View | M2 |
| View.Previous | Vorherige View | M2 |
| View.GoTo | View anspringen | M2 |
| View.LinkToDocument | View an Dokument binden | M2 |
| Block.Begin | Historischen Blockbeginn setzen | M2 |
| Block.End | Historisches Blockende setzen | M2 |
| Block.Copy | Block kopieren | M2 |
| Block.Move | Block verschieben | M2 |
| Block.Delete | Block löschen | M2 |
| Block.ToggleVisibility | Blockmarkierung anzeigen/verbergen | M2 |
| Operation.Cancel | Abbrechbare Operation beenden | M1 |

Command-IDs sind öffentliche, stabile Strings. Neue Commands dürfen ergänzt werden; Umbenennungen gelten nach Version 1.0 als Breaking Change.

## 16. Bearbeitungs- und Navigationsalgorithmen
### 16.1 Einfügen und Überschreiben
- Im Insert-Modus wird Text an der Caretposition eingefügt.
- Im Overwrite-Modus ersetzt jedes eingegebene Text-Element das nächste Text-Element derselben Zeile; am Zeilenende wird eingefügt.
- Mehrzeiliger Text wird in Zeilen zerlegt, wobei die Zeilenendungsinformationen nach Paste-Policy übernommen oder normalisiert werden.
- Eine Eingabesequenz wird für Undo zusammengefasst, solange Dokument, Position, Modus und Zeitfenster kompatibel bleiben.

### 16.2 Löschen und Zeilenverbindung
- Backspace am Zeilenanfang verbindet mit der vorherigen Zeile.
- Delete am Zeilenende verbindet mit der nächsten Zeile.
- DeleteLine entfernt Inhalt und Terminator nach definierten Randfällen; ein Dokument bleibt mindestens mit einer leeren Zeile bestehen.
- DeleteWordRight verwendet `IWordBoundaryService`.

### 16.3 Navigation
- Vertikale Navigation speichert eine gewünschte visuelle X-Position, damit unterschiedliche Zeilenlängen nicht zu seitlichem Driften führen.
- PageUp/PageDown verschiebt um `VisibleLineCount - 1`, mindestens eine Zeile.
- Home/End beziehen sich auf die logische Zeile; optionale Smart-Home-Regel kann zunächst zur ersten Nicht-Leerstelle und beim zweiten Aufruf zu Spalte 0 springen.
- Auswahl-Erweiterung verändert nur die aktive Position, nicht den Anchor.

## 17. Word-Wrap, Auto-Indent, Ränder und Tabs
Das Toolkit unterscheidet zwei Konzepte:
1. **Visual Word-Wrap:** reine Darstellung; verändert den Dokumenttext nicht.
2. **Reformat Paragraph:** historische textverändernde Absatzformatierung anhand linker/rechter Ränder.

| Funktion | Umsetzung |
|---|---|
| Visual Word-Wrap | `ViewLayoutService` erzeugt visuelle Zeilenfragmente abhängig von Viewbreite und Wrapoption. |
| Auto-Indent | `IAutoIndentPolicy` bestimmt Einrückung für `NewLine`. |
| Linker/rechter Rand | Werte im `EditorViewState`; Validierung `0 <= left < right`. |
| Reformat | Ermittelt Absatzgrenzen, normalisiert Leerraum, bricht Wörter innerhalb der Ränder und erzeugt eine Undo-Transaktion. |
| Center Line | Fügt führende Leerzeichen innerhalb der Ränder ein; Tabs werden vorher gemäß Policy behandelt. |
| Tab | `TabSettings` mit Standardbreite 4; InsertTab oder MoveToNextTabStop konfigurierbar. |
| Wrapped-Annotation | Optionales `TextAnnotation` für historischen Import/Export, nicht für normales visuelles Wrapping. |


## 18. Suche, Ersetzen und Marker
### 18.1 Suchmodell
```csharp
public sealed record SearchOptions(
    bool MatchCase,
    bool WholeWord,
    SearchDirection Direction,
    SearchScope Scope,
    bool UseRegularExpressions);

public sealed record SearchMatch(TextRange Range, string Value);
public sealed record ReplaceResult(int MatchCount, int ReplacementCount, bool WasCancelled);
```

M1 implementiert Literalsuche, Vorwärtsrichtung, MatchCase, Auswahl-/Dokumentscope, Find Next, Replace und Replace All. M2 ergänzt Rückwärtssuche, WholeWord, gespeicherte Optionen und interaktive Replace-Strategien. Reguläre Ausdrücke sind M3.

### 18.2 Cancellation und Ergebnisse
Lange Such- und Ersetzoperationen prüfen regelmäßig den `CancellationToken`. „Nicht gefunden“, „abgebrochen“ und „Fehler“ sind getrennte Ergebniszustände. Die UI entscheidet über Meldungsdarstellung.

### 18.3 Marker
Marker basieren auf `TextAnchor`. Mindestens 20 nummerierte Marker werden im Kompatibilitätsprofil angeboten; intern ist die Anzahl nicht begrenzt. Marker besitzen eine Affinity, damit Einfügen an exakt derselben Position deterministisch vor oder hinter dem Marker einsortiert wird.

## 19. Datei-, Stream- und Encoding-Konzept
### 19.1 Verträge
```csharp
public interface IDocumentStorage
{
    Task<DocumentLoadResult> LoadAsync(
        Stream source,
        DocumentLoadOptions options,
        CancellationToken cancellationToken);

    Task<DocumentSaveResult> SaveAsync(
        TextDocument document,
        Stream destination,
        DocumentSaveOptions options,
        CancellationToken cancellationToken);
}

public interface IFileDocumentStorage : IDocumentStorage
{
    Task<DocumentLoadResult> LoadFileAsync(string path, DocumentLoadOptions options, CancellationToken token);
    Task<DocumentSaveResult> SaveFileAsync(TextDocument document, string path, DocumentSaveOptions options, CancellationToken token);
}
```

### 19.2 Encoding-Erkennung
- BOM-basierte Erkennung für UTF-8, UTF-16 LE/BE und UTF-32.
- Ohne BOM wird strikt gültiges UTF-8 bevorzugt.
- Ein konfigurierbares Fallback-Encoding kann der Host vorgeben; ohne Vorgabe wird keine unbegründete stille Konvertierung erzwungen.
- Binärverdacht entsteht bei NUL-Bytes, ungewöhnlich hohem Steuerzeichenanteil oder Dekodierungsfehlern. Der Host erhält `BinaryContentSuspected` und muss Öffnen ausdrücklich bestätigen oder ablehnen.

### 19.3 Atomisches Speichern
Für Dateipfade wird zunächst in eine temporäre Datei im Zielverzeichnis geschrieben, gespült und anschließend ersetzt beziehungsweise verschoben. Erst nach erfolgreichem Abschluss werden Metadaten und Savepoint aktualisiert. Bei Fehler bleibt der bestehende Dokumentinhalt unverändert und die Originaldatei soweit möglich erhalten.

### 19.4 Externe Änderungen
`DocumentFileFingerprint` speichert Pfad, Länge, LastWriteTimeUtc und optional einen schnellen Hash. Vor Überschreiben kann ein Konfliktzustand gemeldet werden. Die Entscheidung `Overwrite`, `SaveAs`, `Reload` oder `Cancel` liegt beim Host.

## 20. WinForms-Adapter
### 20.1 Komponentenaufbau
```text
SasdEditorView : UserControl
├── EditorSurface : Control
├── VScrollBar
└── HScrollBar

EditorWorkspaceControl : UserControl (Sample/optional reusable host)
├── TabControl / Document selector
└── SplitContainer(s) with SasdEditorView instances
```

`SasdEditorView` bindet genau einen `EditorViewState`. Die `EditorSurface` zeichnet Text, Auswahl, Caret, Suchtreffer, Marker, optionale Steuerzeichen und Ränder.

### 20.2 Renderingpipeline
1. Viewport und sichtbare logische Zeilen bestimmen.
2. Layoutcache anhand Dokumentversion, Viewbreite, Font, DPI, Tabs und Wrapoption prüfen.
3. Nur ungültige Zeilen/Fragmente neu layouten.
4. Hintergrund, aktuelle Zeile, Auswahl und Suchmarkierungen zeichnen.
5. Textfragmente zeichnen.
6. Caretposition berechnen und nativen Caret beziehungsweise fokussichere Zeichnung aktualisieren.
7. Scrollbarbereiche aktualisieren, ohne rekursive Layoutschleifen.

### 20.3 Eingabe
- `OnKeyDown` behandelt nichttextuelle Gesten und Command-Sequenzen.
- `OnKeyPress` beziehungsweise Textinput verarbeitet druckbare Zeichen.
- IME-Eingabe wird im WinForms-Adapter gekapselt; zusammengesetzter Text wird erst nach Commit in das Dokument geschrieben.
- Maus unterstützt Caretposition, Drag-Auswahl, Doppelklick-Wortauswahl und optional Drag-and-drop ab M2.
- Clipboard läuft ausschließlich über `IClipboardService`.

### 20.4 DPI, Fonts und Accessibility
- Layoutcaches werden bei Font-, DPI- oder Größenänderung invalidiert.
- M1 garantiert monospaced Darstellung; M2 erweitert auf proportionalen Text mit glyphgenauer Messung.
- Vollständige Tastaturbedienung ist M1-Pflicht.
- Ein eigener `AccessibleObject` exponiert Textrolle, Caret, Auswahl und Dokumentname spätestens M3.

## 21. Referenzanwendung Modern FIRST-ED
Die M1-Sampleanwendung ist kein Produktmonolith, sondern ein Integrationsnachweis.

| UI-Bereich | Inhalt |
|---|---|
| Menü File | New, Open, Save, Save As, Close, Exit |
| Menü Edit | Undo, Redo, Cut, Copy, Paste, Select All, Find, Replace |
| Menü View | Statusbar, Word-Wrap-Vorbereitung, New/Split View soweit Meilenstein |
| Menü Tools | Keyboard Profile, Settings |
| Toolbar | New, Open, Save, Undo, Redo, Cut, Copy, Paste, Find |
| Hauptbereich | Ein oder mehrere `SasdEditorView`-Controls |
| Statusbar | Dateiname, Dirty-Indikator, Zeile, Spalte, Insert/Overwrite, Encoding, Zeilenende, Profil |
| Dialoge | Open/Save, Find/Replace, Go To Line, Close Guard, Encoding/Binary Warning |

Die erweiterte MicroStar-Demo ab M2 demonstriert Pulldown-Menüs, Präfixprofile, mehrere Views, Blockdateien und austauschbare Status-/Error-Presenter.

## 22. WPF- und Web-Vorbereitung
### 22.1 WPF
Ein späteres `Sasd.EditorToolkit.Wpf` implementiert Rendering, Eingabe, Clipboard, Commands und Accessibility neu, referenziert aber ausschließlich den Core. `EditorViewState`, `TextDocument`, Commands, Suche, Undo und Dateiabläufe bleiben unverändert.

### 22.2 Blazor/Razor
Der Webadapter verwendet den Core serverseitig oder in WebAssembly, abhängig vom Host. DOM-Selektion, Browser-Clipboard, IME und Rendering liegen im Adapter. Commands werden über dieselben Command-IDs ausgelöst.

### 22.3 ASPX/Web Forms
Für bestehende Web-Forms-Anwendungen wird kein vollständiges neues serverseitiges Text-Control versprochen. Dokumentiert werden stattdessen drei Integrationswege: Core als serverseitiger Textdienst, Einbettung einer Blazor-Komponente oder JavaScript-Interop mit einem Browseradapter.

## 23. Fehler-, Meldungs- und Promptkonzept
Der Core wirft für erwartbare Benutzer- und Dateisituationen nicht ungefiltert UI-nahe Exceptions, sondern liefert strukturierte Resultate.

```csharp
public sealed record EditorError(
    string Code,
    EditorErrorCategory Category,
    string ResourceKey,
    Exception? Exception = null,
    IReadOnlyDictionary<string, object?>? Context = null);

public readonly record struct EditorResult<T>(T? Value, EditorError? Error)
{
    public bool IsSuccess => Error is null;
}
```

Hostdienste:
- `IEditorErrorPresenter` für Meldungen oder Logging,
- `IUserPromptService` für Bestätigungen und Eingaben,
- `IFileDialogService` für plattformspezifische Dateiauswahl,
- `IReplaceDecisionProvider` für interaktive Replace-Abläufe,
- `IEditorStatusSink` für Statusinformationen.
Damit werden die historischen Hooks `UserError`, `EditAskfor`, `UserReplace` und `UserStatusLine` modern abgebildet.

## 24. Hintergrundaufgaben, Autosave und Druck
Der Core implementiert keinen proprietären Scheduler. Langlaufende Operationen sind normale Tasks mit CancellationToken. Der Host bestimmt Threading und UI-Dispatcher.

| Dienst | Vertrag/Umsetzung | Meilenstein |
|---|---|---|
| Background Runner | `IBackgroundOperationRunner` für Fortschritt, Cancellation und Fehlerweitergabe | M1/M3 |
| Autosave | `IAutosaveService` erstellt Recovery-Snapshots außerhalb des Originalpfades | M3 |
| Recovery | Startprüfung auf gültige, neuere Snapshots; explizite Benutzerentscheidung | M3 |
| Background Print | `IPrintService` und inkrementeller Sample-Job | M6 |
| Modem/Upload/Backup-Beispiele | Nicht als Editorfunktion; generischer Taskvertrag deckt das Muster ab | Nicht vorgesehen |


## 25. Einstellungen und Persistenz
`EditorSettings` ist in globale Defaults, dokumentbezogene Metadaten und viewbezogene Optionen getrennt.

| Einstellungsgruppe | Beispiele |
|---|---|
| General | DefaultEncoding, NewLinePolicy, ConfirmDestructiveOperations |
| Editing | InsertMode, AutoIndent, TabSize, UseTabs, WordBoundaryMode |
| View | Font, Zoom, ShowWhitespace, ShowControlCharacters, ShowLineNumbers, HighlightCurrentLine |
| Formatting | WordWrap, LeftMargin, RightMargin |
| Undo | MaxOperations, MaxEstimatedBytes, TypingCoalescingTimeout |
| Input | KeyboardProfileName, PrefixTimeout |
| Theme | Text-, Background-, Selection-, Status-, Marker- und Annotationstyles |

Persistenz erfolgt über versioniertes JSON. Unbekannte Felder werden toleriert; ungültige Werte werden validiert, protokolliert und auf sichere Defaults zurückgesetzt. Historische feste Werte erscheinen ausschließlich in einem optionalen Retro-Profil.

## 26. Einbettungs- und Erweiterungsverträge
Ein Host kann mindestens folgende Dienste austauschen:
- Storage, File Dialogs und Close Prompts,
- Clipboard, Logging und Fehlerdarstellung,
- Statusdarstellung und Theme,
- Wortgrenzen und Auto-Indent,
- Search-/Replace-Entscheidungen,
- Autosave, Print und Background Runner,
- eigene Commands und Command Interceptors.

Syntaxhervorhebung, Markdown, Log-Viewer, Hex-Viewer und Property-Editor verwenden `TextAnnotation`, Commands und Hostdienste. Sie gehören nicht in den M1-Core und erzeugen keine zyklischen Abhängigkeiten.

## 27. Logging und Diagnostik
Logging ist optional und darf keine UI-Ausgabe erzwingen. Kategorien:
- Dokument öffnen/speichern/schließen,
- Encoding- und Binärwarnungen,
- externe Dateikonflikte,
- Commandfehler,
- Autosave/Recovery,
- Performancewarnungen und Layoutcache-Statistiken im Debugmodus.
Textinhalte werden standardmäßig nicht protokolliert. Pfade können durch den Host redigiert werden. Exceptions werden nur an technischen Grenzen geloggt und anschließend als strukturierte Fehler weitergegeben.

## 28. Sicherheits- und Rechtskonzept
- Keine automatische Ausführung von Text, Makros oder Skripten.
- Pfade werden normalisiert; der Host entscheidet über erlaubte Verzeichnisse.
- Temp- und Recovery-Dateien erhalten vorhersehbare, hostkonfigurierbare Speicherorte.
- Speichern folgt dem Prinzip „erst erfolgreich schreiben, dann Dokument als sauber markieren“.
- Plugins oder Makros benötigen später ein eigenes Berechtigungsmodell.
- Originalquellcode, Logos, Covergestaltung und gescannte Handbuchseiten werden nicht in das Produktrepository übernommen.
- Der öffentliche Name muss die unabhängige SASD-Neuimplementierung deutlich machen. „Turbo Editor Toolbox“ wird nur als historischer Bezug in der Dokumentation verwendet, vorbehaltlich Rechteprüfung.

## 29. Performance- und Ressourcenanforderungen
Die folgenden Werte sind technische Zielwerte für die Referenzhardware, die im Performance-Testprojekt dokumentiert wird. Sie konkretisieren die qualitativen Lastenheftziele und dürfen nach Messung über ADR angepasst werden, nicht stillschweigend.

| Szenario | M1-Ziel | M3-Ziel |
|---|---|---|
| Normale Eingabe bis sichtbares Update | p95 < 50 ms bei 1 MiB / 50.000 Zeilen | p95 < 30 ms bei 10 MiB / 250.000 Zeilen |
| Öffnen UTF-8 | 10 MiB in < 3 s | 100 MiB in < 8 s mit Large-File-Puffer |
| Speichern | 10 MiB in < 3 s | 100 MiB in < 8 s, atomar soweit Dateisystem unterstützt |
| Literalsuche | 10 MiB in < 2 s | 100 MiB in < 5 s |
| Scrollen | keine dauerhaft sichtbaren Vollflächen-Neuzeichnungen | inkrementelles Layout und Caching |
| Speicher | keine unbeschränkten Leaks; Schließen gibt Dokumente frei | Puffer- und Undo-Budget messbar und konfigurierbar |

UI-Performance wird mit realistischen Tastatur-, Scroll-, Resize- und Mehrview-Szenarien getestet. Cancellation muss bei Such-/Dateioperationen spätestens am nächsten definierten Checkpoint wirksam werden.

## 30. Internationalisierung und Accessibility
- Meldungen und UI-Texte kommen aus RESX-Ressourcen; Deutsch und Englisch werden gepflegt.
- Dokumenttext ist nicht kulturabhängig; Suchoptionen definieren explizit ordinal/kulturell, Standard ist ordinaler Vergleich mit optionaler Groß-/Kleinschreibung.
- Volle Tastaturbedienung für alle Kernfunktionen.
- Fokus und Auswahl müssen sichtbar sein.
- Theme-Kontraste werden dokumentiert und in der Referenzanwendung geprüft.
- WinForms-Accessibility exponiert Rolle, Name, Dokumentzustand, Caret und Auswahl; erweiterte Screenreader-Unterstützung ist M3.

## 31. Ereignismodell und Threading
| Ereignis | Absender | Inhalt |
|---|---|---|
| `TextChanging` | TextDocument/ITextBuffer | Voränderung, kann nicht durch UI-Handler mutiert werden |
| `TextChanged` | TextDocument/ITextBuffer | TextChangeSet, alte/neue Version, betroffene Zeilen |
| `DirtyStateChanged` | TextDocument | alter/neuer Zustand |
| `MetadataChanged` | TextDocument | Pfad, Encoding, Zeilenenden, Fingerprint |
| `SelectionChanged` | EditorViewState | alte/neue Auswahl |
| `CaretChanged` | EditorViewState | alte/neue Position und visuelle Spalte |
| `ViewportChanged` | EditorViewState | Scroll-/Viewportdaten |
| `ActiveViewChanged` | EditorWorkspace | alte/neue View |
| `CommandExecuted` | Dispatcher | ID, Ergebnis, Dauer |
| `StatusChanged` | StatusProvider | unveränderlicher Statussnapshot |

Coreobjekte sind nicht allgemein thread-safe. Mutationen eines Dokuments erfolgen seriell über seinen Besitzer/Hostkontext. Asynchrone I/O- und Suchoperationen dürfen im Hintergrund arbeiten, wenden Ergebnisse jedoch kontrolliert auf die aktuelle Dokumentversion an. Ein veraltetes Ergebnis wird verworfen oder als Konflikt gemeldet.

## 32. Use-Case-Realisierung
### UC-01 - Neues Dokument
| Aspekt | Festlegung |
|---|---|
| Ziel | Leeres Dokument erstellen, Text eingeben und speichern. |
| Vorbedingung | Kein Dokument erforderlich. |
| Hauptablauf | Workspace erstellt Dokument und View; Eingabe erzeugt Undo-Transaktionen; SaveAs ruft FileDocumentStorage auf. |
| Nachbedingung | Gespeicherter Clean-State und aktualisierte Metadaten. |
| Fehler-/Alternativablauf | Operation wird abgebrochen oder als strukturierter Fehler an den Host zurückgegeben; bestehender Dokumentinhalt bleibt konsistent. |

### UC-02 - Datei bearbeiten
| Aspekt | Festlegung |
|---|---|
| Ziel | Datei öffnen, verändern und sicher speichern. |
| Vorbedingung | Lesbarer Pfad/Stream. |
| Hauptablauf | Storage lädt Encoding/Zeilenenden; View wird gebunden; Änderungen; atomarer Save. |
| Nachbedingung | Datei aktualisiert oder klarer Fehler ohne Inhaltsverlust. |
| Fehler-/Alternativablauf | Operation wird abgebrochen oder als strukturierter Fehler an den Host zurückgegeben; bestehender Dokumentinhalt bleibt konsistent. |

### UC-03 - Editor einbetten
| Aspekt | Festlegung |
|---|---|
| Ziel | WinForms-Control in eine Fachanwendung integrieren und Hostdienste bereitstellen. |
| Vorbedingung | Host referenziert Core und WinForms. |
| Hauptablauf | Host registriert Dienste/Commands, erzeugt Workspace und SasdEditorView. |
| Nachbedingung | Editor funktioniert ohne duplizierte Fachlogik. |
| Fehler-/Alternativablauf | Operation wird abgebrochen oder als strukturierter Fehler an den Host zurückgegeben; bestehender Dokumentinhalt bleibt konsistent. |

### UC-04 - Mehrere Dateien vergleichen
| Aspekt | Festlegung |
|---|---|
| Ziel | Mehrere Dokumentansichten gleichzeitig verwenden. |
| Vorbedingung | Mindestens zwei Dokumente. |
| Hauptablauf | Workspace hält mehrere Dokumente/Views; Host ordnet Tabs oder Splits zu. |
| Nachbedingung | Unabhängige Inhalte und Zustände. |
| Fehler-/Alternativablauf | Operation wird abgebrochen oder als strukturierter Fehler an den Host zurückgegeben; bestehender Dokumentinhalt bleibt konsistent. |

### UC-05 - Dasselbe Dokument teilen
| Aspekt | Festlegung |
|---|---|
| Ziel | Zwei Ansichten zeigen verschiedene Stellen desselben Dokuments. |
| Vorbedingung | Ein geöffnetes Dokument. |
| Hauptablauf | Workspace erstellt zweite ViewState-Instanz auf dasselbe Dokument. |
| Nachbedingung | Gemeinsamer Inhalt, unabhängige Cursor/Scrollposition. |
| Fehler-/Alternativablauf | Operation wird abgebrochen oder als strukturierter Fehler an den Host zurückgegeben; bestehender Dokumentinhalt bleibt konsistent. |

### UC-06 - Block bearbeiten
| Aspekt | Festlegung |
|---|---|
| Ziel | Auswahl kopieren, verschieben oder löschen. |
| Vorbedingung | Nichtleere Auswahl. |
| Hauptablauf | Copy/Move/Delete über SelectionService und UndoTransaction. |
| Nachbedingung | Deterministische Auswahl und Undo/Redo. |
| Fehler-/Alternativablauf | Operation wird abgebrochen oder als strukturierter Fehler an den Host zurückgegeben; bestehender Dokumentinhalt bleibt konsistent. |

### UC-07 - Suchen und Ersetzen
| Aspekt | Festlegung |
|---|---|
| Ziel | Treffer finden, navigieren und einzeln oder gesammelt ersetzen. |
| Vorbedingung | Suchmuster. |
| Hauptablauf | SearchService liefert Treffer; Replace/ReplaceAll erzeugen atomare Änderungen. |
| Nachbedingung | Trefferzustand oder NotFound/Cancelled. |
| Fehler-/Alternativablauf | Operation wird abgebrochen oder als strukturierter Fehler an den Host zurückgegeben; bestehender Dokumentinhalt bleibt konsistent. |

### UC-08 - Undo/Redo
| Aspekt | Festlegung |
|---|---|
| Ziel | Mehrere Änderungen rückgängig machen und wiederholen. |
| Vorbedingung | Mindestens eine Änderung. |
| Hauptablauf | UndoManager wendet inverse/erneute ChangeSets an. |
| Nachbedingung | Dokument, Marker und Views konsistent. |
| Fehler-/Alternativablauf | Operation wird abgebrochen oder als strukturierter Fehler an den Host zurückgegeben; bestehender Dokumentinhalt bleibt konsistent. |

### UC-09 - Historisches Tastaturprofil
| Aspekt | Festlegung |
|---|---|
| Ziel | Turbo-/WordStar-Präfixbefehle aktivieren. |
| Vorbedingung | Kompatibilitätsprofil installiert. |
| Hauptablauf | JSON-Profil laden; SequenceResolver verarbeitet Ctrl-K/O/Q-Chords. |
| Nachbedingung | Historische Bedienung ohne Core-Neukompilierung. |
| Fehler-/Alternativablauf | Operation wird abgebrochen oder als strukturierter Fehler an den Host zurückgegeben; bestehender Dokumentinhalt bleibt konsistent. |

### UC-10 - Eigenen Befehl ergänzen
| Aspekt | Festlegung |
|---|---|
| Ziel | Host registriert anwendungsspezifischen Command für Menü, Toolbar und Taste. |
| Vorbedingung | Hostcommand implementiert. |
| Hauptablauf | Registry registriert Command und Binding; Menü/Taste verwenden gleiche ID. |
| Nachbedingung | Hostfunktion über alle Eingabekanäle erreichbar. |
| Fehler-/Alternativablauf | Operation wird abgebrochen oder als strukturierter Fehler an den Host zurückgegeben; bestehender Dokumentinhalt bleibt konsistent. |

### UC-11 - Ungespeicherte Änderungen schützen
| Aspekt | Festlegung |
|---|---|
| Ziel | Schließen oder Öffnen erzwingt Speichern/Verwerfen/Abbrechen. |
| Vorbedingung | Dokument ist dirty. |
| Hauptablauf | CloseGuard liefert DecisionRequired; Host fragt; Save/Discard/Cancel. |
| Nachbedingung | Kein unbeabsichtigter Verlust. |
| Fehler-/Alternativablauf | Operation wird abgebrochen oder als strukturierter Fehler an den Host zurückgegeben; bestehender Dokumentinhalt bleibt konsistent. |

### UC-12 - Große Datei
| Aspekt | Festlegung |
|---|---|
| Ziel | Große Datei reaktionsfähig öffnen und Vorgang abbrechen. |
| Vorbedingung | Große Datei. |
| Hauptablauf | BufferFactory und Storage streamen/laden; Fortschritt/Cancellation. |
| Nachbedingung | Reaktionsfähige UI; Abbruch hinterlässt kein halbes Dokument. |
| Fehler-/Alternativablauf | Operation wird abgebrochen oder als strukturierter Fehler an den Host zurückgegeben; bestehender Dokumentinhalt bleibt konsistent. |

### UC-13 - Autosave
| Aspekt | Festlegung |
|---|---|
| Ziel | Host speichert Wiederherstellungsstände im Hintergrund. |
| Vorbedingung | Autosave aktiviert. |
| Hauptablauf | AutosaveService erzeugt versionierten Snapshot; Recoveryprüfung beim Start. |
| Nachbedingung | Wiederherstellbarer Stand ohne Originalüberschreibung. |
| Fehler-/Alternativablauf | Operation wird abgebrochen oder als strukturierter Fehler an den Host zurückgegeben; bestehender Dokumentinhalt bleibt konsistent. |

### UC-14 - Weiteren UI-Adapter bauen
| Aspekt | Festlegung |
|---|---|
| Ziel | WPF- oder Webadapter verwendet denselben Core. |
| Vorbedingung | Core API stabil. |
| Hauptablauf | Neuer Adapter implementiert Rendering/Input/Hostdienste. |
| Nachbedingung | Keine fachliche Doppelimplementierung. |
| Fehler-/Alternativablauf | Operation wird abgebrochen oder als strukturierter Fehler an den Host zurückgegeben; bestehender Dokumentinhalt bleibt konsistent. |

### UC-15 - Desktop-Components-Showcase
| Aspekt | Festlegung |
|---|---|
| Ziel | Komponentenprojekt demonstriert Editor, Markdown- oder Log-Viewer. |
| Vorbedingung | Editorpakete verfügbar. |
| Hauptablauf | Desktop Components referenziert Pakete und baut spezialisierte Controls. |
| Nachbedingung | Keine zyklische Abhängigkeit oder Quellcodeduplikation. |
| Fehler-/Alternativablauf | Operation wird abgebrochen oder als strukturierter Fehler an den Host zurückgegeben; bestehender Dokumentinhalt bleibt konsistent. |

## 33. Testkonzept
### 33.1 Testpyramide
| Ebene | Ziel | Beispiele |
|---|---|---|
| Unit | Deterministische Corelogik | Buffer, Positionen, Commands, Undo, Suche, Marker, Settings |
| Integration | Zusammenspiel mit Streams/Dateisystem/Workspace | Encoding, atomisches Save, externe Änderungen, mehrere Views |
| WinForms | Rendering- und Inputintegration | Caret, Auswahl, Scroll, DPI, KeySequences, Clipboard |
| Architecture | Abhängigkeitsregeln | Core ohne UI-Referenzen; keine Zyklen; Samples ohne Kernlogik |
| Performance | Messbare Zielwerte | Open/Save/Search/Input/Scroll/Memory |
| Manual Exploratory | UX und Accessibility | IME, Screenreader, High DPI, Dialogabläufe |

### 33.2 Verbindliche Testfälle
| Test-ID | Testfall |
|---|---|
| T-CORE-001 | Insert/Delete über Zeilengrenzen inklusive leerem Dokument |
| T-CORE-002 | CRLF/LF/CR und gemischte Zeilenenden laden/speichern |
| T-CORE-003 | Unicode, Surrogatpaare, kombinierte Zeichen und Emoji-Caret-Stops |
| T-UNDO-001 | Undo/Redo für Typing, Paste, ReplaceAll, Blockmove und Reformat |
| T-UNDO-002 | Savepoint und Dirty State vor/nach Save und Undo zum Savepoint |
| T-VIEW-001 | Zwei Views teilen Inhalt, nicht Cursor/Scrollzustand |
| T-SEARCH-001 | Find/Replace/ReplaceAll, MatchCase, NotFound und Cancellation |
| T-FILE-001 | Fehler beim Save lässt Dokument und vorhandene Datei konsistent |
| T-FILE-002 | BOM-/UTF-8-Erkennung und Binärwarnung |
| T-CMD-001 | CommandInterceptor blockiert, ersetzt und verarbeitet Commands |
| T-INPUT-001 | Modernes und Turbo-kompatibles Tastaturprofil |
| T-UI-001 | Resize/DPI/Font invalidiert Layout korrekt |
| T-UI-002 | Clipboard mit mehrzeiligem Unicode |
| T-ARCH-001 | Core-Assembly referenziert keine UI-Frameworkassembly |
| T-PERF-001 | M1-Zieldatei erfüllt Open/Save/Search/Input-Budgets |


## 34. Build, CI, Paketierung und Dokumentation
### 34.1 Buildregeln
- `Nullable=enable`, `ImplicitUsings=enable`, deterministischer Build und SourceLink.
- Warnungen werden im CI als Fehler behandelt; begründete Ausnahmen sind zentral dokumentiert.
- Paketversion folgt Semantic Versioning.
- Öffentliche API wird über einen API-Baseline-Check überwacht.
- Releasebuild erzeugt symbol packages und XML-Dokumentation.

### 34.2 CI-Pipeline
```text
restore
-> build Debug
-> unit tests
-> architecture tests
-> integration tests
-> build Release
-> pack
-> documentation/link checks
-> optional performance smoke tests
-> publish artifacts (nur Release-Workflow)
```

### 34.3 NuGet-Pakete
| Paket | Inhalt |
|---|---|
| `Sasd.EditorToolkit.Core` | UI-unabhängige Editorbibliothek |
| `Sasd.EditorToolkit.WinForms` | WinForms-Control, Renderer und Plattformdienste |

Samples werden nicht als Runtime-Paket veröffentlicht. WPF und Web erhalten später eigene Pakete.

## 35. Meilenstein- und Lieferplanung
| Meilenstein | Ziel | Lieferumfang | Abnahme |
|---|---|---|---|
| M0 | Spezifikation und technische Basis | Lastenheft/Pflichtenheft, ADRs, Namens-/Lizenzprüfung, Solution-Skeleton, CI, Renderer-Spike | Freigegebene Architektur und grüner Skeleton-Build |
| M1 / 0.1 | Modern FIRST-ED | Core, LineTextBuffer, Basisnavigation/-editing, Datei, Undo/Redo, Suche, Commands, WinForms-Control, Demo, Tests | AK-001 bis AK-003, AK-008 bis AK-012, Kern von AK-013/014 |
| M2 / 0.5 | Historische Funktionsbreite | Word-Wrap/Reformat, Auto-Indent, Ränder, Tabs, Marker, Blocks, mehrere Views, Profile, Themes, Hooks | AK-004 bis AK-007 und vollständiges AK-013 |
| M3 / 1.0 | Stabilisierung | PieceTable-Option, Performance, Accessibility, NuGet, API-Baseline, vollständige DE/EN-Doku und Traceability | Alle AK einschließlich AK-015 |
| M4 | WPF | WPF-Adapter und Sample | Core unverändert; Adapterakzeptanztests |
| M5 | Web | Blazor/Razor und dokumentierter ASPX-Weg | Browserintegration ohne Corekopplung |
| M6 | Erweiterungen | Syntax, Markdown, Log-/Hex-Viewer, Makros, Print, Sprachdienste | Eigene Erweiterungsabnahmen |


## 36. Definition of Done
Eine Funktion ist fertig, wenn:
- die zugehörigen Lastenheft-IDs im Pull Request genannt sind,
- Implementierung und öffentliche API dokumentiert sind,
- Unit- beziehungsweise Integrationstests vorhanden und grün sind,
- Fehler-, Cancellation- und Randfälle berücksichtigt sind,
- keine unerlaubte Abhängigkeit entstanden ist,
- relevante DE/EN-Dokumentation aktualisiert ist,
- historische Traceability bei betroffenen Funktionen aktualisiert ist,
- Performance- und Security-Auswirkungen geprüft sind,
- der Code Review nach SASD Development Standard abgeschlossen ist.

Ein Meilenstein ist fertig, wenn zusätzlich alle zugeordneten Abnahmekriterien erfüllt, die NuGet-/Sample-Artefakte erzeugt und bekannte Einschränkungen in Release Notes dokumentiert sind.

## 37. Offene Entscheidungen mit vorgeschlagenem Standard
| Thema | Vorgeschlagener Standard | Entscheidungszeitpunkt |
|---|---|---|
| Öffentlicher Produktname | SASD Editor Toolkit; historischen Namen nur erläuternd verwenden | M0 |
| Lizenz | MIT für neu geschriebenen Code, vorbehaltlich Product-Owner-Freigabe | M0 |
| Renderer | Eigene WinForms EditorSurface | M0 ADR |
| M1-Puffer | LineTextBuffer | M0 ADR |
| M3-Large-File-Puffer | Piece Table | vor M3 |
| Standardencoding neue Dateien | UTF-8 ohne BOM, hostkonfigurierbar | M1 |
| Standardzeilenende | Plattform-/Hostdefault; beim Laden erhalten | M1 |
| Standardtabweite | 4 | M1 |
| Standardfont WinForms | Monospaced System-/Cascadia-Mono-Fallback, ohne Fontdatei mitzuliefern | M1 |
| Regex | M3, mit Timeout und Cancellation | M3 |
| Autosave-Pfad | Hostkonfigurierter LocalAppData-Unterordner | M3 |


## 38. Vollständige Traceability der Lastenheft-Anforderungen
Die folgende Matrix führt alle **208** formalen Anforderungen des Lastenhefts. Damit ist nachvollziehbar, wo und wann jede Anforderung realisiert und wie sie verifiziert wird.

| ID | Prio | Lastenheft-Anforderung | Pflichtenheft | Komponente | Meilenstein | Umsetzung | Nachweis |
|---|---|---|---|---|---|---|---|
| PROD-001 | MUSS | Das Produkt muss als wiederverwendbare Editor-Toolbox und nicht nur als fertige Einzelanwendung bereitgestellt werden. | Kapitel 4-8 | Repository, Paketierung und Produktarchitektur | M1 | Umsetzung durch Repository, Paketierung und Produktarchitektur gemäß Kapitel 4-8 | Architekturtest, Paket-/Repository-Review und Integrationssample |
| PROD-002 | MUSS | Die Toolbox muss einfache Editoren, komplexe Editoren und in Fachanwendungen eingebettete Textbearbeitung ermöglichen. | Kapitel 4-8 | Repository, Paketierung und Produktarchitektur | M1 | Umsetzung durch Repository, Paketierung und Produktarchitektur gemäß Kapitel 4-8 | Architekturtest, Paket-/Repository-Review und Integrationssample |
| PROD-003 | MUSS | Der fachliche Editor-Kern muss unabhängig von WinForms, WPF, ASP.NET und Browsertechniken sein. | Kapitel 4-8 | Repository, Paketierung und Produktarchitektur | M4 | SasdEditorView/UserControl mit eigener EditorSurface; separater WPF-Adapter auf unverändertem Core; separater Web-Adapter; Core bleibt DOM- und Browser-frei | Architekturtest, Paket-/Repository-Review und Integrationssample |
| PROD-004 | MUSS | WinForms ist die erste produktiv nutzbare Referenzoberfläche. | Kapitel 4-8 | Repository, Paketierung und Produktarchitektur | M1 | SasdEditorView/UserControl mit eigener EditorSurface | Architekturtest, Paket-/Repository-Review und Integrationssample |
| PROD-005 | MUSS | WPF muss später denselben Kern ohne fachliche Neuimplementierung verwenden können. | Kapitel 4-8 | Repository, Paketierung und Produktarchitektur | M4 | separater WPF-Adapter auf unverändertem Core | Architekturtest, Paket-/Repository-Review und Integrationssample |
| PROD-006 | SOLL | Der Kern muss aus ASP.NET-basierten Hosts nutzbar sein; eine moderne interaktive Webintegration soll bevorzugt über Blazor oder Razor erfolgen. | Kapitel 4-8 | Repository, Paketierung und Produktarchitektur | M5 | separater Web-Adapter; Core bleibt DOM- und Browser-frei | Architekturtest, Paket-/Repository-Review und Integrationssample |
| PROD-007 | MUSS | Das SASD Editor Toolkit wird als eigenständiges Repository, Paket und Releaseobjekt geführt. | Kapitel 4-8 | Repository, Paketierung und Produktarchitektur | M0/M1 | Umsetzung durch Repository, Paketierung und Produktarchitektur gemäß Kapitel 4-8 | Architekturtest, Paket-/Repository-Review und Integrationssample |
| PROD-008 | MUSS | SASD Desktop Components darf den Editor verwenden und demonstrieren, aber nicht Eigentümer des Editor-Kerns sein. | Kapitel 4-8 | Repository, Paketierung und Produktarchitektur | M0/M1 | Umsetzung durch Repository, Paketierung und Produktarchitektur gemäß Kapitel 4-8 | Architekturtest, Paket-/Repository-Review und Integrationssample |
| PROD-009 | MUSS | Die Editor-Toolbox muss unabhängig von Data Toolbox, Numerics, Graphics und GameWorks versionierbar bleiben. | Kapitel 4-8 | Repository, Paketierung und Produktarchitektur | M0/M1 | Umsetzung durch Repository, Paketierung und Produktarchitektur gemäß Kapitel 4-8 | Architekturtest, Paket-/Repository-Review und Integrationssample |
| PROD-010 | MUSS | Historische Fähigkeiten werden funktional modernisiert; DOS-, Turbo-Pascal-, Overlay- und Videospeichertechnik wird nicht 1:1 portiert. | Kapitel 4-8 | Repository, Paketierung und Produktarchitektur | M0/M1 | IDocumentStorage/FileDocumentStorage mit atomarem Speichern | Architekturtest, Paket-/Repository-Review und Integrationssample |
| PROD-011 | SOLL | Code, öffentliche API und Paketnamen sollen Englisch sein; Dokumentation soll Deutsch und Englisch verfügbar sein. | Kapitel 4-8 | Repository, Paketierung und Produktarchitektur | M1 | RESX-Ressourcen und zweisprachige Dokumentation | Architekturtest, Paket-/Repository-Review und Integrationssample |
| PROD-012 | MUSS | Bibliotheken, UI-Komponenten und lauffähige Beispiele müssen gemeinsam bereitgestellt werden. | Kapitel 4-8 | Repository, Paketierung und Produktarchitektur | M1 | Umsetzung durch Repository, Paketierung und Produktarchitektur gemäß Kapitel 4-8 | Architekturtest, Paket-/Repository-Review und Integrationssample |
| CORE-001 | MUSS | Text muss als Zeichen, Wörter, Zeilen, Textströme, Ansichten, Blöcke/Auswahlen und Dateien modellierbar sein. | Kapitel 9-12 | Sasd.EditorToolkit.Core / Dokument- und Textmodell | M1 | IDocumentStorage/FileDocumentStorage mit atomarem Speichern | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| CORE-002 | MUSS | Dokumentinhalt und Ansichtszustand müssen getrennt sein. | Kapitel 9-12 | Sasd.EditorToolkit.Core / Dokument- und Textmodell | M1 | Umsetzung durch Sasd.EditorToolkit.Core / Dokument- und Textmodell gemäß Kapitel 9-12 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| CORE-003 | MUSS | Mehrere Ansichten müssen dasselbe Dokument gleichzeitig anzeigen und bearbeiten können. | Kapitel 9-12 | Sasd.EditorToolkit.Core / Dokument- und Textmodell | M1 | gemeinsames TextDocument mit mehreren unabhängigen EditorViewState-Instanzen | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| CORE-004 | MUSS | Ein Dokument muss Text, optionalen Pfad, Anzeigenamen, Codierung, Zeilenenden, Dirty State, Marker und Undo/Redo verwalten können. | Kapitel 9-12 | Sasd.EditorToolkit.Core / Dokument- und Textmodell | M1 | LineEndingKind je Zeile und konfigurierbare Save-Policy; Savepoint im UndoManager und CloseGuard-Workflow; transaktionaler UndoManager mit Coalescing und Savepoint; TextAnchor-basierte MarkerCollection mit Affinity | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| CORE-005 | MUSS | Eine Ansicht muss Cursor, Auswahl, Scrollposition, Insert/Overwrite, Word-Wrap, Auto-Indent, Ränder und Tabulatorverhalten verwalten. | Kapitel 9-12 | Sasd.EditorToolkit.Core / Dokument- und Textmodell | M1 | ViewLayoutService und optionaler ReformatParagraphCommand; AutoIndentPolicy beim NewLineCommand; TabSettings und VisualColumnCalculator | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| CORE-006 | MUSS | Der Textpuffer muss hinter einer austauschbaren Abstraktion gekapselt sein. | Kapitel 9-12 | Sasd.EditorToolkit.Core / Dokument- und Textmodell | M1 | Umsetzung durch Sasd.EditorToolkit.Core / Dokument- und Textmodell gemäß Kapitel 9-12 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| CORE-007 | SOLL | Die erste Pufferimplementierung darf zeilenorientiert sein, muss aber spätere Large-File-Implementierungen erlauben. | Kapitel 9-12 | Sasd.EditorToolkit.Core / Dokument- und Textmodell | M1 | Umsetzung durch Sasd.EditorToolkit.Core / Dokument- und Textmodell gemäß Kapitel 9-12 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| CORE-008 | MUSS | Zeilen dürfen nicht auf 255 Zeichen begrenzt sein. | Kapitel 9-12 | Sasd.EditorToolkit.Core / Dokument- und Textmodell | M1 | keine feste Zeilenlängengrenze; nur Ressourcenlimits | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| CORE-009 | MUSS | Die interne Textrepräsentation muss Unicode unterstützen. | Kapitel 9-12 | Sasd.EditorToolkit.Core / Dokument- und Textmodell | M1 | UTF-16-.NET-Strings, Unicode-sichere Caret-Stops und explizite Encoding-Metadaten | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| CORE-010 | MUSS | CRLF, LF und weitere unterstützte Zeilenenden müssen erkannt, erhalten und konvertiert werden können. | Kapitel 9-12 | Sasd.EditorToolkit.Core / Dokument- und Textmodell | M1 | LineEndingKind je Zeile und konfigurierbare Save-Policy | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| CORE-011 | MUSS | Leere und noch nicht benannte Dokumente müssen unterstützt werden. | Kapitel 9-12 | Sasd.EditorToolkit.Core / Dokument- und Textmodell | M1 | Umsetzung durch Sasd.EditorToolkit.Core / Dokument- und Textmodell gemäß Kapitel 9-12 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| CORE-012 | MUSS | Textänderungen müssen als atomare, nachvollziehbare Operationen repräsentierbar sein. | Kapitel 9-12 | Sasd.EditorToolkit.Core / Dokument- und Textmodell | M1 | Umsetzung durch Sasd.EditorToolkit.Core / Dokument- und Textmodell gemäß Kapitel 9-12 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| CORE-013 | MUSS | Der Core muss ohne UI-Thread und ohne UI-Framework testbar sein. | Kapitel 9-12 | Sasd.EditorToolkit.Core / Dokument- und Textmodell | M1 | Umsetzung durch Sasd.EditorToolkit.Core / Dokument- und Textmodell gemäß Kapitel 9-12 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| CORE-014 | SOLL | Dokumentmodus und Nichtdokumentmodus sollen als konfigurierbare Bearbeitungsoption abbildbar sein. | Kapitel 9-12 | Sasd.EditorToolkit.Core / Dokument- und Textmodell | M1 | Umsetzung durch Sasd.EditorToolkit.Core / Dokument- und Textmodell gemäß Kapitel 9-12 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| CORE-015 | SOLL | Steuerzeichen müssen speicherbar und optional sichtbar darstellbar sein. | Kapitel 9-12 | Sasd.EditorToolkit.Core / Dokument- und Textmodell | M1 | IDocumentStorage/FileDocumentStorage mit atomarem Speichern | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| CORE-016 | SOLL | Zeilen und Bereiche müssen erweiterbare Annotationen für Wrap, Auswahl, Markierung und Spezialdarstellung tragen können. | Kapitel 9-12 | Sasd.EditorToolkit.Core / Dokument- und Textmodell | M1 | Umsetzung durch Sasd.EditorToolkit.Core / Dokument- und Textmodell gemäß Kapitel 9-12 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| CORE-017 | MUSS | Textpositionen und Bereiche müssen eindeutig, validierbar und bei Änderungen deterministisch behandelbar sein. | Kapitel 9-12 | Sasd.EditorToolkit.Core / Dokument- und Textmodell | M1 | Umsetzung durch Sasd.EditorToolkit.Core / Dokument- und Textmodell gemäß Kapitel 9-12 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| NAV-001 | MUSS | Cursor ein Zeichen nach links oder rechts bewegen. | Kapitel 15 | NavigationCommandHandler | M1 | Umsetzung durch NavigationCommandHandler gemäß Kapitel 15 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| NAV-002 | MUSS | Cursor eine Zeile nach oben oder unten bewegen. | Kapitel 15 | NavigationCommandHandler | M1 | Umsetzung durch NavigationCommandHandler gemäß Kapitel 15 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| NAV-003 | MUSS | Ansicht eine Zeile nach oben oder unten scrollen. | Kapitel 15 | NavigationCommandHandler | M1 | Umsetzung durch NavigationCommandHandler gemäß Kapitel 15 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| NAV-004 | MUSS | Seitenweise nach oben oder unten navigieren. | Kapitel 15 | NavigationCommandHandler | M1 | Umsetzung durch NavigationCommandHandler gemäß Kapitel 15 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| NAV-005 | MUSS | Wortweise nach links oder rechts navigieren. | Kapitel 15 | NavigationCommandHandler | M1 | Umsetzung durch NavigationCommandHandler gemäß Kapitel 15 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| NAV-006 | MUSS | Zum Zeilenanfang und Zeilenende springen; ein Umschaltbefehl zwischen beiden Positionen soll möglich sein. | Kapitel 15 | NavigationCommandHandler | M1 | LineEndingKind je Zeile und konfigurierbare Save-Policy; EditorCommandRegistry, Dispatcher und serialisierbare KeyboardProfile | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| NAV-007 | MUSS | Zum Anfang und Ende des Dokuments springen. | Kapitel 15 | NavigationCommandHandler | M1 | Umsetzung durch NavigationCommandHandler gemäß Kapitel 15 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| NAV-008 | MUSS | Zum Anfang und Ende einer Auswahl beziehungsweise eines Blocks springen. | Kapitel 15 | NavigationCommandHandler | M1 | Umsetzung durch NavigationCommandHandler gemäß Kapitel 15 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| NAV-009 | MUSS | Zu einer angegebenen Zeile oder Spalte springen. | Kapitel 15 | NavigationCommandHandler | M1 | Umsetzung durch NavigationCommandHandler gemäß Kapitel 15 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| NAV-010 | MUSS | Horizontales Scrollen muss bei breiten Zeilen unterstützt werden. | Kapitel 15 | NavigationCommandHandler | M1 | Umsetzung durch NavigationCommandHandler gemäß Kapitel 15 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| NAV-011 | SOLL | Navigation muss optional eine Auswahl erweitern können. | Kapitel 15 | NavigationCommandHandler | M1 | Umsetzung durch NavigationCommandHandler gemäß Kapitel 15 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| NAV-012 | SOLL | Die gewünschte visuelle Spalte soll bei vertikaler Navigation erhalten bleiben. | Kapitel 15 | NavigationCommandHandler | M1 | Umsetzung durch NavigationCommandHandler gemäß Kapitel 15 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| EDIT-001 | MUSS | Normale Eingabe muss im Einfügemodus Text einschieben und im Überschreibmodus Text ersetzen. | Kapitel 14-15 | EditingCommandHandler und ITextBuffer | M1 | TextSearchService mit SearchOptions, CancellationToken und SearchResult | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| EDIT-002 | MUSS | Zwischen Einfüge- und Überschreibmodus muss umgeschaltet werden können. | Kapitel 14-15 | EditingCommandHandler und ITextBuffer | M1 | Umsetzung durch EditingCommandHandler und ITextBuffer gemäß Kapitel 14-15 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| EDIT-003 | MUSS | Neue Zeilen und leere Zeilen müssen eingefügt werden können. | Kapitel 14-15 | EditingCommandHandler und ITextBuffer | M1 | Umsetzung durch EditingCommandHandler und ITextBuffer gemäß Kapitel 14-15 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| EDIT-004 | MUSS | Zeichen rechts beziehungsweise unter dem Cursor und links vom Cursor müssen gelöscht werden können. | Kapitel 14-15 | EditingCommandHandler und ITextBuffer | M1 | Umsetzung durch EditingCommandHandler und ITextBuffer gemäß Kapitel 14-15 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| EDIT-005 | MUSS | Aktuelle Zeile, Wort rechts und Text bis Zeilenende müssen gelöscht werden können. | Kapitel 14-15 | EditingCommandHandler und ITextBuffer | M1 | LineEndingKind je Zeile und konfigurierbare Save-Policy | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| EDIT-006 | SOLL | Der gesamte Text eines Dokuments darf nur über eine ausdrücklich bestätigbare Aktion gelöscht werden. | Kapitel 14-15 | EditingCommandHandler und ITextBuffer | M1 | Umsetzung durch EditingCommandHandler und ITextBuffer gemäß Kapitel 14-15 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| EDIT-007 | MUSS | Zeilen müssen geteilt, verbunden, verkürzt, erweitert, komprimiert und verschoben werden können, soweit interne oder öffentliche Befehle dies benötigen. | Kapitel 14-15 | EditingCommandHandler und ITextBuffer | M1 | EditorCommandRegistry, Dispatcher und serialisierbare KeyboardProfile | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| EDIT-008 | MUSS | Groß-/Kleinschreibung eines Zeichens oder ausgewählten Textes muss geändert werden können. | Kapitel 14-15 | EditingCommandHandler und ITextBuffer | M1 | Umsetzung durch EditingCommandHandler und ITextBuffer gemäß Kapitel 14-15 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| EDIT-009 | SOLL | Steuerzeichen müssen über einen ausdrücklichen Befehl eingefügt werden können. | Kapitel 14-15 | EditingCommandHandler und ITextBuffer | M1 | EditorCommandRegistry, Dispatcher und serialisierbare KeyboardProfile | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| EDIT-010 | MUSS | Ausschneiden, Kopieren und Einfügen über die Plattformzwischenablage müssen unterstützt werden. | Kapitel 14-15 | EditingCommandHandler und ITextBuffer | M1 | Umsetzung durch EditingCommandHandler und ITextBuffer gemäß Kapitel 14-15 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| EDIT-011 | MUSS | Mehrzeiliger Unicode-Text aus der Zwischenablage muss korrekt verarbeitet werden. | Kapitel 14-15 | EditingCommandHandler und ITextBuffer | M1 | UTF-16-.NET-Strings, Unicode-sichere Caret-Stops und explizite Encoding-Metadaten | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| EDIT-012 | MUSS | Bearbeitungsoperationen müssen atomar sein und ihre Ausführbarkeit vorab prüfen können. | Kapitel 14-15 | EditingCommandHandler und ITextBuffer | M1 | Umsetzung durch EditingCommandHandler und ITextBuffer gemäß Kapitel 14-15 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| WP-001 | MUSS | Word-Wrap muss je Ansicht aktivierbar und deaktivierbar sein. | Kapitel 16 | TextFormattingService und EditorViewState | M2 | ViewLayoutService und optionaler ReformatParagraphCommand | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| WP-002 | MUSS | Word-Wrap und Absatzformatierung müssen linken und rechten Rand berücksichtigen. | Kapitel 16 | TextFormattingService und EditorViewState | M2 | ViewLayoutService und optionaler ReformatParagraphCommand | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| WP-003 | MUSS | Absätze müssen neu formatiert werden können. | Kapitel 16 | TextFormattingService und EditorViewState | M2 | Umsetzung durch TextFormattingService und EditorViewState gemäß Kapitel 16 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| WP-004 | MUSS | Auto-Indent muss je Ansicht aktivierbar sein und die Einrückung der vorherigen Zeile übernehmen. | Kapitel 16 | TextFormattingService und EditorViewState | M2 | AutoIndentPolicy beim NewLineCommand | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| WP-005 | MUSS | Linker und rechter Rand müssen einstellbar sein. | Kapitel 16 | TextFormattingService und EditorViewState | M2 | Umsetzung durch TextFormattingService und EditorViewState gemäß Kapitel 16 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| WP-006 | MUSS | Eine Zeile muss innerhalb der aktiven Ränder zentriert werden können. | Kapitel 16 | TextFormattingService und EditorViewState | M2 | Umsetzung durch TextFormattingService und EditorViewState gemäß Kapitel 16 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| WP-007 | MUSS | Tabulatoren und konfigurierbare Tabulatorbreite müssen unterstützt werden. | Kapitel 16 | TextFormattingService und EditorViewState | M2 | TabSettings und VisualColumnCalculator | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| WP-008 | SOLL | Echte Tabulatorzeichen und visuelle Tabulatornavigation sollen unterscheidbar sein. | Kapitel 16 | TextFormattingService und EditorViewState | M2 | TabSettings und VisualColumnCalculator | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| WP-009 | SOLL | Automatisch umgebrochene Zeilen sollen bei Bedarf als solche annotiert werden können. | Kapitel 16 | TextFormattingService und EditorViewState | M2 | Umsetzung durch TextFormattingService und EditorViewState gemäß Kapitel 16 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| WP-010 | KANN | Historische WordStar-Wrap-Markierungen dürfen nur als expliziter Kompatibilitätsimport/-export angeboten werden. | Kapitel 16 | TextFormattingService und EditorViewState | M2 | Umsetzung durch TextFormattingService und EditorViewState gemäß Kapitel 16 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| WP-011 | SOLL | Wortgrenzen und Trennzeichen müssen erweiterbar und Unicode-gerecht behandelbar sein. | Kapitel 16 | TextFormattingService und EditorViewState | M2 | UTF-16-.NET-Strings, Unicode-sichere Caret-Stops und explizite Encoding-Metadaten | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| SEL-001 | MUSS | Beginn und Ende einer Auswahl beziehungsweise eines historischen Blocks müssen markierbar sein. | Kapitel 13 und 17 | TextSelection, Clipboard- und Blockdienste | M1 | Umsetzung durch TextSelection, Clipboard- und Blockdienste gemäß Kapitel 13 und 17 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| SEL-002 | MUSS | Ausgewählter Text muss kopiert, verschoben und gelöscht werden können. | Kapitel 13 und 17 | TextSelection, Clipboard- und Blockdienste | M1 | Umsetzung durch TextSelection, Clipboard- und Blockdienste gemäß Kapitel 13 und 17 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| SEL-003 | SOLL | Die Hervorhebung einer Auswahl muss ausblendbar sein, ohne den logischen Bereich zwingend zu verwerfen. | Kapitel 13 und 17 | TextSelection, Clipboard- und Blockdienste | M1 | Umsetzung durch TextSelection, Clipboard- und Blockdienste gemäß Kapitel 13 und 17 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| SEL-004 | SOLL | Operationen zwischen Ansichten und Dokumenten müssen unterstützt werden, soweit semantisch zulässig. | Kapitel 13 und 17 | TextSelection, Clipboard- und Blockdienste | M1 | Umsetzung durch TextSelection, Clipboard- und Blockdienste gemäß Kapitel 13 und 17 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| SEL-005 | MUSS | Die moderne Standardauswahl muss zeichenweise und mehrzeilig arbeiten. | Kapitel 13 und 17 | TextSelection, Clipboard- und Blockdienste | M1 | Umsetzung durch TextSelection, Clipboard- und Blockdienste gemäß Kapitel 13 und 17 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| SEL-006 | KANN | Ein historischer zeilenorientierter Blockmodus kann als Kompatibilitätsprofil angeboten werden. | Kapitel 13 und 17 | TextSelection, Clipboard- und Blockdienste | M2 | Umsetzung durch TextSelection, Clipboard- und Blockdienste gemäß Kapitel 13 und 17 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| SEL-007 | MUSS | Gesamtes Dokument auswählen muss unterstützt werden. | Kapitel 13 und 17 | TextSelection, Clipboard- und Blockdienste | M2 | Umsetzung durch TextSelection, Clipboard- und Blockdienste gemäß Kapitel 13 und 17 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| SEL-008 | MUSS | Auswahlpositionen müssen nach Änderungen deterministisch nachgeführt werden. | Kapitel 13 und 17 | TextSelection, Clipboard- und Blockdienste | M2 | Umsetzung durch TextSelection, Clipboard- und Blockdienste gemäß Kapitel 13 und 17 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| SRCH-001 | MUSS | Textsuche, nächste Fundstelle und Suchen/Ersetzen müssen unterstützt werden. | Kapitel 18 | TextSearchService und MarkerCollection | M1 | TextSearchService mit SearchOptions, CancellationToken und SearchResult | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| SRCH-002 | SOLL | Ersetzen aller Fundstellen soll unterstützt werden. | Kapitel 18 | TextSearchService und MarkerCollection | M1 | TextSearchService mit SearchOptions, CancellationToken und SearchResult | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| SRCH-003 | SOLL | Suchoptionen sollen mindestens Groß-/Kleinschreibung, Richtung und Ganzwort umfassen. | Kapitel 18 | TextSearchService und MarkerCollection | M1 | TextSearchService mit SearchOptions, CancellationToken und SearchResult | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| SRCH-004 | KANN | Reguläre Ausdrücke können optional angeboten werden. | Kapitel 18 | TextSearchService und MarkerCollection | M1 | Umsetzung durch TextSearchService und MarkerCollection gemäß Kapitel 18 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| SRCH-005 | SOLL | Suche muss auf Dokument, Auswahl oder definierbarem Bereich arbeiten können. | Kapitel 18 | TextSearchService und MarkerCollection | M2/M3 | TextSearchService mit SearchOptions, CancellationToken und SearchResult | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| SRCH-006 | MUSS | Such- und Ersetzvorgänge müssen abbrechbar sein. | Kapitel 18 | TextSearchService und MarkerCollection | M2/M3 | TextSearchService mit SearchOptions, CancellationToken und SearchResult | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| SRCH-007 | SOLL | Letzte Suchzeichenfolge, Ersetzungszeichenfolge und Optionen sollen erhalten bleiben. | Kapitel 18 | TextSearchService und MarkerCollection | M2/M3 | TextSearchService mit SearchOptions, CancellationToken und SearchResult | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| SRCH-008 | MUSS | Nicht gefundene Muster müssen als eindeutiger Ergebniszustand gemeldet werden. | Kapitel 18 | TextSearchService und MarkerCollection | M2/M3 | Umsetzung durch TextSearchService und MarkerCollection gemäß Kapitel 18 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| SRCH-009 | MUSS | Marker müssen gesetzt, angesprungen und über eine Eingabeaufforderung ausgewählt werden können. | Kapitel 18 | TextSearchService und MarkerCollection | M2/M3 | TextAnchor-basierte MarkerCollection mit Affinity | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| SRCH-010 | SOLL | Mindestens zwanzig Marker sollen kompatibel unterstützt werden; intern darf die Zahl höher sein. | Kapitel 18 | TextSearchService und MarkerCollection | M2/M3 | TextAnchor-basierte MarkerCollection mit Affinity | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| SRCH-011 | MUSS | Marker müssen dokumentbezogen und bei Textänderungen positionsstabil nachgeführt werden. | Kapitel 18 | TextSearchService und MarkerCollection | M2/M3 | TextAnchor-basierte MarkerCollection mit Affinity; TabSettings und VisualColumnCalculator | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| VIEW-001 | MUSS | Mehrere Dokumentansichten müssen gleichzeitig angezeigt werden können. | Kapitel 11 und 20 | EditorWorkspace, EditorViewState und WinForms Workspace | M1 | Umsetzung durch EditorWorkspace, EditorViewState und WinForms Workspace gemäß Kapitel 11 und 20 | WinForms-Integrationstest, UI-Test und manuelle Referenzprüfung |
| VIEW-002 | MUSS | Ansichten müssen erzeugt, geschlossen, aktiviert und in ihrer Größe verändert werden können. | Kapitel 11 und 20 | EditorWorkspace, EditorViewState und WinForms Workspace | M2 | Umsetzung durch EditorWorkspace, EditorViewState und WinForms Workspace gemäß Kapitel 11 und 20 | WinForms-Integrationstest, UI-Test und manuelle Referenzprüfung |
| VIEW-003 | MUSS | Schließen einer Ansicht darf ungespeicherten Dokumentinhalt nicht unbeabsichtigt vernichten. | Kapitel 11 und 20 | EditorWorkspace, EditorViewState und WinForms Workspace | M1 | Savepoint im UndoManager und CloseGuard-Workflow; IDocumentStorage/FileDocumentStorage mit atomarem Speichern | WinForms-Integrationstest, UI-Test und manuelle Referenzprüfung |
| VIEW-004 | MUSS | Zwischen Ansichten muss vorwärts und rückwärts gewechselt werden können. | Kapitel 11 und 20 | EditorWorkspace, EditorViewState und WinForms Workspace | M1 | Umsetzung durch EditorWorkspace, EditorViewState und WinForms Workspace gemäß Kapitel 11 und 20 | WinForms-Integrationstest, UI-Test und manuelle Referenzprüfung |
| VIEW-005 | MUSS | Ansichten müssen über Nummer, Kennung oder Auswahl angesprungen werden können. | Kapitel 11 und 20 | EditorWorkspace, EditorViewState und WinForms Workspace | M2 | Umsetzung durch EditorWorkspace, EditorViewState und WinForms Workspace gemäß Kapitel 11 und 20 | WinForms-Integrationstest, UI-Test und manuelle Referenzprüfung |
| VIEW-006 | MUSS | Zwei oder mehr Ansichten müssen dasselbe Dokument teilen können. | Kapitel 11 und 20 | EditorWorkspace, EditorViewState und WinForms Workspace | M2 | gemeinsames TextDocument mit mehreren unabhängigen EditorViewState-Instanzen | WinForms-Integrationstest, UI-Test und manuelle Referenzprüfung |
| VIEW-007 | MUSS | Änderungen in einer Ansicht müssen in allen verknüpften Ansichten konsistent sichtbar sein. | Kapitel 11 und 20 | EditorWorkspace, EditorViewState und WinForms Workspace | M2 | gemeinsames TextDocument mit mehreren unabhängigen EditorViewState-Instanzen | WinForms-Integrationstest, UI-Test und manuelle Referenzprüfung |
| VIEW-008 | SOLL | Nicht sichtbare Dokumente oder Ansichten dürfen im Speicher gehalten und später wieder eingeblendet werden. | Kapitel 11 und 20 | EditorWorkspace, EditorViewState und WinForms Workspace | M2 | IDocumentStorage/FileDocumentStorage mit atomarem Speichern | WinForms-Integrationstest, UI-Test und manuelle Referenzprüfung |
| VIEW-009 | MUSS | Jede Ansicht muss eigenen Cursor-, Auswahl-, Scroll-, Modus- und Randzustand besitzen. | Kapitel 11 und 20 | EditorWorkspace, EditorViewState und WinForms Workspace | M2 | Umsetzung durch EditorWorkspace, EditorViewState und WinForms Workspace gemäß Kapitel 11 und 20 | WinForms-Integrationstest, UI-Test und manuelle Referenzprüfung |
| VIEW-010 | SOLL | Horizontale und vertikale Teilungen sollen in modernen Desktopadaptern möglich sein. | Kapitel 11 und 20 | EditorWorkspace, EditorViewState und WinForms Workspace | M1 | Umsetzung durch EditorWorkspace, EditorViewState und WinForms Workspace gemäß Kapitel 11 und 20 | WinForms-Integrationstest, UI-Test und manuelle Referenzprüfung |
| VIEW-011 | MUSS | Die Zahl der Ansichten darf nicht künstlich auf acht oder neun begrenzt sein. | Kapitel 11 und 20 | EditorWorkspace, EditorViewState und WinForms Workspace | M2 | Umsetzung durch EditorWorkspace, EditorViewState und WinForms Workspace gemäß Kapitel 11 und 20 | WinForms-Integrationstest, UI-Test und manuelle Referenzprüfung |
| VIEW-012 | MUSS | Aktive Ansicht und Reihenfolge der Ansichten müssen eindeutig verwaltet werden. | Kapitel 11 und 20 | EditorWorkspace, EditorViewState und WinForms Workspace | M2 | Umsetzung durch EditorWorkspace, EditorViewState und WinForms Workspace gemäß Kapitel 11 und 20 | WinForms-Integrationstest, UI-Test und manuelle Referenzprüfung |
| FILE-001 | MUSS | Textdateien müssen in neue oder bestehende Dokumente eingelesen werden können. | Kapitel 19 | IDocumentStorage und FileDocumentStorage | M1 | IDocumentStorage/FileDocumentStorage mit atomarem Speichern | Dateisystem-/Stream-Integrationstests einschließlich Fehlerfällen |
| FILE-002 | MUSS | Speichern, Speichern unter und Schreiben in einen angegebenen Dateinamen müssen unterstützt werden. | Kapitel 19 | IDocumentStorage und FileDocumentStorage | M1 | IDocumentStorage/FileDocumentStorage mit atomarem Speichern | Dateisystem-/Stream-Integrationstests einschließlich Fehlerfällen |
| FILE-003 | MUSS | Dateioperationen müssen interaktiv über Host-Prompts und programmgesteuert über Parameter aufrufbar sein. | Kapitel 19 | IDocumentStorage und FileDocumentStorage | M1 | IDocumentStorage/FileDocumentStorage mit atomarem Speichern | Dateisystem-/Stream-Integrationstests einschließlich Fehlerfällen |
| FILE-004 | SOLL | Dateioperationen sollen asynchron und abbrechbar ausführbar sein. | Kapitel 19 | IDocumentStorage und FileDocumentStorage | M1 | IDocumentStorage/FileDocumentStorage mit atomarem Speichern | Dateisystem-/Stream-Integrationstests einschließlich Fehlerfällen |
| FILE-005 | MUSS | Zeichencodierung und Zeilenenden müssen erkannt, erhalten und bewusst konvertiert werden können. | Kapitel 19 | IDocumentStorage und FileDocumentStorage | M1 | LineEndingKind je Zeile und konfigurierbare Save-Policy | Dateisystem-/Stream-Integrationstests einschließlich Fehlerfällen |
| FILE-006 | MUSS | Binäre oder nicht unterstützte Dateien dürfen nicht stillschweigend als normaler Text überschrieben werden. | Kapitel 19 | IDocumentStorage und FileDocumentStorage | M2/M3 | IDocumentStorage/FileDocumentStorage mit atomarem Speichern | Dateisystem-/Stream-Integrationstests einschließlich Fehlerfällen |
| FILE-007 | MUSS | Lese- und Schreibfehler müssen ohne Verlust des bisherigen Dokumentinhalts behandelt werden. | Kapitel 19 | IDocumentStorage und FileDocumentStorage | M2/M3 | strukturierte EditorError-Codes plus hostseitiger Presenter | Dateisystem-/Stream-Integrationstests einschließlich Fehlerfällen |
| FILE-008 | SOLL | Speichern soll nach Möglichkeit atomar über temporäre Datei und Ersetzung erfolgen. | Kapitel 19 | IDocumentStorage und FileDocumentStorage | M2/M3 | IDocumentStorage/FileDocumentStorage mit atomarem Speichern | Dateisystem-/Stream-Integrationstests einschließlich Fehlerfällen |
| FILE-009 | SOLL | Vor dem Überschreiben extern geänderter Dateien soll gewarnt werden können. | Kapitel 19 | IDocumentStorage und FileDocumentStorage | M2/M3 | IDocumentStorage/FileDocumentStorage mit atomarem Speichern | Dateisystem-/Stream-Integrationstests einschließlich Fehlerfällen |
| FILE-010 | SOLL | Die API soll Stream-basierte Ein- und Ausgabe zusätzlich zu Dateipfaden ermöglichen. | Kapitel 19 | IDocumentStorage und FileDocumentStorage | M2/M3 | IDocumentStorage/FileDocumentStorage mit atomarem Speichern | Dateisystem-/Stream-Integrationstests einschließlich Fehlerfällen |
| FILE-011 | KANN | Historische WordStar-Wrap-Markierungen können als optionaler Import-/Exportmodus angeboten werden. | Kapitel 19 | IDocumentStorage und FileDocumentStorage | M1 | Umsetzung durch IDocumentStorage und FileDocumentStorage gemäß Kapitel 19 | Dateisystem-/Stream-Integrationstests einschließlich Fehlerfällen |
| FILE-012 | KANN | Markierte Blöcke als separate Datei lesen oder schreiben ist eine optionale Erweiterung. | Kapitel 19 | IDocumentStorage und FileDocumentStorage | M2/M3 | IDocumentStorage/FileDocumentStorage mit atomarem Speichern | Dateisystem-/Stream-Integrationstests einschließlich Fehlerfällen |
| FILE-013 | KANN | Dateiumbenennung, Dateikopie, Dateilöschung und Directory Display sind optionale Hostfunktionen, nicht Pflicht des Editor-Kerns. | Kapitel 19 | IDocumentStorage und FileDocumentStorage | M2/M3 | IDocumentStorage/FileDocumentStorage mit atomarem Speichern | Dateisystem-/Stream-Integrationstests einschließlich Fehlerfällen |
| FILE-014 | MUSS | Vor Verwerfen ungespeicherter Änderungen muss der Host Speichern, Verwerfen oder Abbrechen anbieten können. | Kapitel 19 | IDocumentStorage und FileDocumentStorage | M2/M3 | Savepoint im UndoManager und CloseGuard-Workflow; IDocumentStorage/FileDocumentStorage mit atomarem Speichern | Dateisystem-/Stream-Integrationstests einschließlich Fehlerfällen |
| UNDO-001 | MUSS | Bearbeitungs- und Löschoperationen müssen rückgängig gemacht werden können. | Kapitel 12 | UndoManager und TextChangeSet | M1 | Umsetzung durch UndoManager und TextChangeSet gemäß Kapitel 12 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| UNDO-002 | MUSS | Mehrere Undo-Schritte und Redo müssen unterstützt werden. | Kapitel 12 | UndoManager und TextChangeSet | M1 | transaktionaler UndoManager mit Coalescing und Savepoint | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| UNDO-003 | MUSS | Zusammengehörige Änderungen müssen als Transaktion gruppierbar sein. | Kapitel 12 | UndoManager und TextChangeSet | M1 | Umsetzung durch UndoManager und TextChangeSet gemäß Kapitel 12 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| UNDO-004 | MUSS | Das Undo-Limit muss konfigurierbar sein. | Kapitel 12 | UndoManager und TextChangeSet | M1 | transaktionaler UndoManager mit Coalescing und Savepoint | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| UNDO-005 | SOLL | Das Limit soll über Operationen, Speicherbudget oder beides steuerbar sein. | Kapitel 12 | UndoManager und TextChangeSet | M1 | IDocumentStorage/FileDocumentStorage mit atomarem Speichern | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| UNDO-006 | MUSS | Undo/Redo muss dokumentbezogen und von der Anzahl der Ansichten unabhängig sein. | Kapitel 12 | UndoManager und TextChangeSet | M2 | transaktionaler UndoManager mit Coalescing und Savepoint | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| UNDO-007 | MUSS | Dirty State muss verlässlich anzeigen, ob der Inhalt vom letzten gespeicherten Zustand abweicht. | Kapitel 12 | UndoManager und TextChangeSet | M1 | Savepoint im UndoManager und CloseGuard-Workflow; IDocumentStorage/FileDocumentStorage mit atomarem Speichern | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| UNDO-008 | MUSS | Nach erfolgreichem Speichern muss der gespeicherte Zustand als sauber markiert werden. | Kapitel 12 | UndoManager und TextChangeSet | M1 | IDocumentStorage/FileDocumentStorage mit atomarem Speichern | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| UNDO-009 | MUSS | Eigene Erweiterungsbefehle müssen Änderungen in Undo/Redo und Dirty State integrieren können. | Kapitel 12 | UndoManager und TextChangeSet | M1 | Savepoint im UndoManager und CloseGuard-Workflow; transaktionaler UndoManager mit Coalescing und Savepoint; EditorCommandRegistry, Dispatcher und serialisierbare KeyboardProfile | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| CMD-001 | MUSS | Alle Benutzeraktionen müssen über eindeutig identifizierbare Editorbefehle aufrufbar sein. | Kapitel 14 | EditorCommandRegistry, Dispatcher und KeyboardProfile | M1 | EditorCommandRegistry, Dispatcher und serialisierbare KeyboardProfile | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| CMD-002 | MUSS | Ein zentraler Dispatcher muss Eingaben Befehlen zuordnen. | Kapitel 14 | EditorCommandRegistry, Dispatcher und KeyboardProfile | M1 | EditorCommandRegistry, Dispatcher und serialisierbare KeyboardProfile | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| CMD-003 | MUSS | Befehle müssen unabhängig von Menü, Tastatur, Toolbar, Kontextmenü oder API aufrufbar sein. | Kapitel 14 | EditorCommandRegistry, Dispatcher und KeyboardProfile | M1 | EditorCommandRegistry, Dispatcher und serialisierbare KeyboardProfile | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| CMD-004 | MUSS | Befehle müssen ihre Ausführbarkeit prüfen und synchron oder asynchron ausführbar sein. | Kapitel 14 | EditorCommandRegistry, Dispatcher und KeyboardProfile | M1 | EditorCommandRegistry, Dispatcher und serialisierbare KeyboardProfile | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| CMD-005 | MUSS | Hooks/Interceptor müssen Befehle filtern, ersetzen, unterdrücken oder selbst verarbeiten können. | Kapitel 14 | EditorCommandRegistry, Dispatcher und KeyboardProfile | M1 | TextSearchService mit SearchOptions, CancellationToken und SearchResult; EditorCommandRegistry, Dispatcher und serialisierbare KeyboardProfile | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| CMD-006 | MUSS | Ein vollständig behandelter Befehl darf nicht erneut durch den Standarddispatcher ausgeführt werden. | Kapitel 14 | EditorCommandRegistry, Dispatcher und KeyboardProfile | M1 | EditorCommandRegistry, Dispatcher und serialisierbare KeyboardProfile | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| CMD-007 | MUSS | Einzelne Tastendrücke und mehrstufige Präfixsequenzen müssen abbildbar sein. | Kapitel 14 | EditorCommandRegistry, Dispatcher und KeyboardProfile | M1 | EditorCommandRegistry, Dispatcher und serialisierbare KeyboardProfile | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| CMD-008 | MUSS | Tastaturprofile müssen austauschbar und benutzerdefiniert speicherbar sein. | Kapitel 14 | EditorCommandRegistry, Dispatcher und KeyboardProfile | M1 | IDocumentStorage/FileDocumentStorage mit atomarem Speichern; EditorCommandRegistry, Dispatcher und serialisierbare KeyboardProfile | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| CMD-009 | SOLL | Ein modernes Standardprofil und ein Turbo-Editor-/WordStar-kompatibles Profil sollen geliefert werden. | Kapitel 14 | EditorCommandRegistry, Dispatcher und KeyboardProfile | M2 | Umsetzung durch EditorCommandRegistry, Dispatcher und KeyboardProfile gemäß Kapitel 14 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| CMD-010 | MUSS | Hosts müssen eigene Befehle registrieren und Standardbefehle ersetzen oder deaktivieren können. | Kapitel 14 | EditorCommandRegistry, Dispatcher und KeyboardProfile | M2 | TextSearchService mit SearchOptions, CancellationToken und SearchResult; EditorCommandRegistry, Dispatcher und serialisierbare KeyboardProfile | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| CMD-011 | KANN | Mehrere Eingabeereignisse sollen programmatisch in eine Sequenz gestellt werden können. | Kapitel 14 | EditorCommandRegistry, Dispatcher und KeyboardProfile | M2 | Umsetzung durch EditorCommandRegistry, Dispatcher und KeyboardProfile gemäß Kapitel 14 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| CMD-012 | KANN | Makros und eine Befehlssprache müssen architektonisch möglich sein, gehören aber nicht zum ersten Meilenstein. | Kapitel 14 | EditorCommandRegistry, Dispatcher und KeyboardProfile | M6 | EditorCommandRegistry, Dispatcher und serialisierbare KeyboardProfile | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| CMD-013 | MUSS | Ein globaler Abbruchbefehl muss lang laufende Operationen beenden können. | Kapitel 14 | EditorCommandRegistry, Dispatcher und KeyboardProfile | M2 | EditorCommandRegistry, Dispatcher und serialisierbare KeyboardProfile | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| CMD-014 | MUSS | Unbekannte Eingaben dürfen keine unkontrollierte Aktion auslösen. | Kapitel 14 | EditorCommandRegistry, Dispatcher und KeyboardProfile | M6 | Umsetzung durch EditorCommandRegistry, Dispatcher und KeyboardProfile gemäß Kapitel 14 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| UI-001 | MUSS | Die WinForms-Integration muss als wiederverwendbares Control oder klar gekapselte Adapterkomponente vorliegen. | Kapitel 20-22 | Sasd.EditorToolkit.WinForms | M1 | SasdEditorView/UserControl mit eigener EditorSurface | WinForms-Integrationstest, UI-Test und manuelle Referenzprüfung |
| UI-002 | MUSS | Text, Cursor, Auswahl, Suchtreffer, Marker und besondere Annotationen müssen darstellbar sein. | Kapitel 20-22 | Sasd.EditorToolkit.WinForms | M1 | TextAnchor-basierte MarkerCollection mit Affinity; TextSearchService mit SearchOptions, CancellationToken und SearchResult | WinForms-Integrationstest, UI-Test und manuelle Referenzprüfung |
| UI-003 | MUSS | Eine Statusanzeige muss mindestens Dateiname, Zeile, Spalte, Dirty State und Bearbeitungsmodus anzeigen können. | Kapitel 20-22 | Sasd.EditorToolkit.WinForms | M1 | Savepoint im UndoManager und CloseGuard-Workflow; IDocumentStorage/FileDocumentStorage mit atomarem Speichern | WinForms-Integrationstest, UI-Test und manuelle Referenzprüfung |
| UI-004 | MUSS | Statusanzeige muss durch den Host ersetzbar, erweiterbar oder ausblendbar sein. | Kapitel 20-22 | Sasd.EditorToolkit.WinForms | M1 | Umsetzung durch Sasd.EditorToolkit.WinForms gemäß Kapitel 20-22 | WinForms-Integrationstest, UI-Test und manuelle Referenzprüfung |
| UI-005 | SOLL | Eine Befehls-/Meldungszeile oder funktional gleichwertige Prompt-Darstellung soll möglich sein. | Kapitel 20-22 | Sasd.EditorToolkit.WinForms | M1 | EditorCommandRegistry, Dispatcher und serialisierbare KeyboardProfile | WinForms-Integrationstest, UI-Test und manuelle Referenzprüfung |
| UI-006 | MUSS | Menüs, Toolbar und Kontextmenüs müssen dasselbe Command-System verwenden. | Kapitel 20-22 | Sasd.EditorToolkit.WinForms | M1 | EditorCommandRegistry, Dispatcher und serialisierbare KeyboardProfile | WinForms-Integrationstest, UI-Test und manuelle Referenzprüfung |
| UI-007 | KANN | Eine optionale Retro-/MicroStar-Demo kann historische Pulldown-Menüs nachvollziehbar zeigen. | Kapitel 20-22 | Sasd.EditorToolkit.WinForms | M1 | Umsetzung durch Sasd.EditorToolkit.WinForms gemäß Kapitel 20-22 | WinForms-Integrationstest, UI-Test und manuelle Referenzprüfung |
| UI-008 | MUSS | Modale und nichtmodale Eingabeaufforderungen müssen über Hostdienste ersetzbar sein. | Kapitel 20-22 | Sasd.EditorToolkit.WinForms | M1 | Umsetzung durch Sasd.EditorToolkit.WinForms gemäß Kapitel 20-22 | WinForms-Integrationstest, UI-Test und manuelle Referenzprüfung |
| UI-009 | MUSS | Fehler- und Bestätigungsdarstellung muss plattformspezifisch austauschbar sein. | Kapitel 20-22 | Sasd.EditorToolkit.WinForms | M1 | strukturierte EditorError-Codes plus hostseitiger Presenter | WinForms-Integrationstest, UI-Test und manuelle Referenzprüfung |
| UI-010 | MUSS | Themes müssen mindestens normalen Text, Auswahl, Status, Befehle und besondere Inhalte unterscheiden können. | Kapitel 20-22 | Sasd.EditorToolkit.WinForms | M1 | EditorCommandRegistry, Dispatcher und serialisierbare KeyboardProfile | WinForms-Integrationstest, UI-Test und manuelle Referenzprüfung |
| UI-011 | MUSS | Die UI darf nicht von Videospeicherzugriff, Retrace-Timing oder 80x25-Raster abhängen. | Kapitel 20-22 | Sasd.EditorToolkit.WinForms | M1 | IDocumentStorage/FileDocumentStorage mit atomarem Speichern | WinForms-Integrationstest, UI-Test und manuelle Referenzprüfung |
| UI-012 | MUSS | Die WinForms-Demo muss Menüs, Toolbar, Statusleiste, mehrere Ansichten und Dateidialoge demonstrieren. | Kapitel 20-22 | Sasd.EditorToolkit.WinForms | M1 | gemeinsames TextDocument mit mehreren unabhängigen EditorViewState-Instanzen; IDocumentStorage/FileDocumentStorage mit atomarem Speichern; SasdEditorView/UserControl mit eigener EditorSurface | WinForms-Integrationstest, UI-Test und manuelle Referenzprüfung |
| UI-013 | SOLL | WPF und Web erhalten eigene Adapter auf demselben Kern. | Kapitel 20-22 | Sasd.EditorToolkit.WinForms | M4 | separater WPF-Adapter auf unverändertem Core; separater Web-Adapter; Core bleibt DOM- und Browser-frei | WinForms-Integrationstest, UI-Test und manuelle Referenzprüfung |
| UI-014 | SOLL | Legacy-ASPX/Web-Forms ist im ersten Schritt kein First-Class-Control, darf aber den Kern integrieren. | Kapitel 20-22 | Sasd.EditorToolkit.WinForms | M4 | separater Web-Adapter; Core bleibt DOM- und Browser-frei | WinForms-Integrationstest, UI-Test und manuelle Referenzprüfung |
| UI-015 | SOLL | Unsichtbare Zeichen, Zeilenenden, Tabs, Leerzeichen und Steuerzeichen sollen optional darstellbar sein. | Kapitel 20-22 | Sasd.EditorToolkit.WinForms | M5 | LineEndingKind je Zeile und konfigurierbare Save-Policy; TabSettings und VisualColumnCalculator | WinForms-Integrationstest, UI-Test und manuelle Referenzprüfung |
| UI-016 | SOLL | Zeilennummern, Ränder und aktuelle Cursorzeile sollen optional darstellbar sein. | Kapitel 20-22 | Sasd.EditorToolkit.WinForms | M5 | Umsetzung durch Sasd.EditorToolkit.WinForms gemäß Kapitel 20-22 | WinForms-Integrationstest, UI-Test und manuelle Referenzprüfung |
| UI-017 | MUSS | Rendering und Scrollen müssen bei Eingabe reaktionsfähig bleiben. | Kapitel 20-22 | Sasd.EditorToolkit.WinForms | M1 | Umsetzung durch Sasd.EditorToolkit.WinForms gemäß Kapitel 20-22 | WinForms-Integrationstest, UI-Test und manuelle Referenzprüfung |
| UI-018 | MUSS | High DPI, Größenänderung, Fokus und IME müssen im WinForms-Adapter berücksichtigt werden. | Kapitel 20-22 | Sasd.EditorToolkit.WinForms | M1 | SasdEditorView/UserControl mit eigener EditorSurface; DPI-aware WinForms-Layout und Font/Measure-Cache-Invalidierung | WinForms-Integrationstest, UI-Test und manuelle Referenzprüfung |
| ERR-001 | MUSS | Der Kern muss strukturierte Fehler und Ergebnisobjekte liefern, nicht ausschließlich Meldungstext. | Kapitel 23 | EditorResult, ErrorCatalog und Host-Presenter | M1 | strukturierte EditorError-Codes plus hostseitiger Presenter | Unit Tests für Fehlercodes und Host-Presenter-Integration |
| ERR-002 | MUSS | Hosts müssen Fehler vollständig selbst behandeln oder an eine Standardbehandlung weiterreichen können. | Kapitel 23 | EditorResult, ErrorCatalog und Host-Presenter | M1 | strukturierte EditorError-Codes plus hostseitiger Presenter | Unit Tests für Fehlercodes und Host-Presenter-Integration |
| ERR-003 | MUSS | Fehlermeldungen müssen lokalisierbar sein. | Kapitel 23 | EditorResult, ErrorCatalog und Host-Presenter | M1 | strukturierte EditorError-Codes plus hostseitiger Presenter; RESX-Ressourcen und zweisprachige Dokumentation | Unit Tests für Fehlercodes und Host-Presenter-Integration |
| ERR-004 | MUSS | Datei-, Speicher-, Eingabe-, Such- und Zustandsfehler müssen unterscheidbar sein. | Kapitel 23 | EditorResult, ErrorCatalog und Host-Presenter | M1 | TextSearchService mit SearchOptions, CancellationToken und SearchResult; IDocumentStorage/FileDocumentStorage mit atomarem Speichern; strukturierte EditorError-Codes plus hostseitiger Presenter | Unit Tests für Fehlercodes und Host-Presenter-Integration |
| ERR-005 | MUSS | Fehler dürfen keine Teiländerungen oder Datenverluste verursachen. | Kapitel 23 | EditorResult, ErrorCatalog und Host-Presenter | M1 | strukturierte EditorError-Codes plus hostseitiger Presenter | Unit Tests für Fehlercodes und Host-Presenter-Integration |
| ERR-006 | MUSS | Bestätigungen für destruktive Aktionen müssen zentral konfigurierbar sein. | Kapitel 23 | EditorResult, ErrorCatalog und Host-Presenter | M1 | Umsetzung durch EditorResult, ErrorCatalog und Host-Presenter gemäß Kapitel 23 | Unit Tests für Fehlercodes und Host-Presenter-Integration |
| ERR-007 | MUSS | Bibliothekscode darf keine UI-Dialoge erzwingen. | Kapitel 23 | EditorResult, ErrorCatalog und Host-Presenter | M1 | Umsetzung durch EditorResult, ErrorCatalog und Host-Presenter gemäß Kapitel 23 | Unit Tests für Fehlercodes und Host-Presenter-Integration |
| BG-001 | SOLL | Ein generischer Mechanismus für inkrementelle Hintergrund- oder Hosttasks soll vorhanden sein, ohne proprietären Editor-Scheduler. | Kapitel 24 | IBackgroundOperationRunner, Autosave- und Print-Adapter | M3/M6 | Umsetzung durch IBackgroundOperationRunner, Autosave- und Print-Adapter gemäß Kapitel 24 | Cancellation-, Timing- und Wiederanlauftests |
| BG-002 | MUSS | Hintergrundaufgaben müssen schnell auf Eingaben und Abbruch reagieren und ihren Zustand erhalten können. | Kapitel 24 | IBackgroundOperationRunner, Autosave- und Print-Adapter | M3/M6 | Umsetzung durch IBackgroundOperationRunner, Autosave- und Print-Adapter gemäß Kapitel 24 | Cancellation-, Timing- und Wiederanlauftests |
| BG-003 | SOLL | Autosave muss als optionaler Hostdienst integrierbar sein. | Kapitel 24 | IBackgroundOperationRunner, Autosave- und Print-Adapter | M3/M6 | Umsetzung durch IBackgroundOperationRunner, Autosave- und Print-Adapter gemäß Kapitel 24 | Cancellation-, Timing- und Wiederanlauftests |
| BG-004 | KANN | Hintergrunddruck kann als Beispiel oder optionale Erweiterung dokumentiert werden. | Kapitel 24 | IBackgroundOperationRunner, Autosave- und Print-Adapter | M6 | Umsetzung durch IBackgroundOperationRunner, Autosave- und Print-Adapter gemäß Kapitel 24 | Cancellation-, Timing- und Wiederanlauftests |
| BG-005 | NICHT | Historische Modemwahl, Upload/Download oder Festplattenbackup werden nicht als Editorfunktionen übernommen. | Kapitel 24 | IBackgroundOperationRunner, Autosave- und Print-Adapter | M6 | Umsetzung durch IBackgroundOperationRunner, Autosave- und Print-Adapter gemäß Kapitel 24 | Cancellation-, Timing- und Wiederanlauftests |
| BG-006 | SOLL | Moderne Hosts sollen Tasks, CancellationToken und Plattformdispatcher statt Polling verwenden. | Kapitel 24 | IBackgroundOperationRunner, Autosave- und Print-Adapter | M6 | Umsetzung durch IBackgroundOperationRunner, Autosave- und Print-Adapter gemäß Kapitel 24 | Cancellation-, Timing- und Wiederanlauftests |
| INT-001 | MUSS | Der Editor muss in größere Anwendungen eingebettet werden können. | Kapitel 7, 25 und 26 | Öffentliche Core-API und Plattformadapter | M1 | Umsetzung durch Öffentliche Core-API und Plattformadapter gemäß Kapitel 7, 25 und 26 | Architekturtest, Paket-/Repository-Review und Integrationssample |
| INT-002 | MUSS | Hosts müssen Dokumente, Commands, Dienste und Einstellungen programmgesteuert bereitstellen können. | Kapitel 7, 25 und 26 | Öffentliche Core-API und Plattformadapter | M1 | EditorCommandRegistry, Dispatcher und serialisierbare KeyboardProfile | Architekturtest, Paket-/Repository-Review und Integrationssample |
| INT-003 | MUSS | Der Core muss headless für Tests, Batchverarbeitung und serverseitige Funktionen nutzbar sein. | Kapitel 7, 25 und 26 | Öffentliche Core-API und Plattformadapter | M1 | Umsetzung durch Öffentliche Core-API und Plattformadapter gemäß Kapitel 7, 25 und 26 | Architekturtest, Paket-/Repository-Review und Integrationssample |
| INT-004 | MUSS | Clipboard, Dateidialoge, Druck, Theme, Eingabe, Logging und Benachrichtigungen müssen über Adapter/Dienste austauschbar sein. | Kapitel 7, 25 und 26 | Öffentliche Core-API und Plattformadapter | M1 | IDocumentStorage/FileDocumentStorage mit atomarem Speichern; ILogger-Abstraktion ohne UI-Zwang | Architekturtest, Paket-/Repository-Review und Integrationssample |
| INT-005 | MUSS | Integration darf keine Include-Reihenfolge, globalen Variablen, Overlays oder Chain-Files verlangen. | Kapitel 7, 25 und 26 | Öffentliche Core-API und Plattformadapter | M1 | Umsetzung durch Öffentliche Core-API und Plattformadapter gemäß Kapitel 7, 25 und 26 | Architekturtest, Paket-/Repository-Review und Integrationssample |
| INT-006 | SOLL | Dependency Injection und öffentliche Schnittstellen sollen unterstützt werden. | Kapitel 7, 25 und 26 | Öffentliche Core-API und Plattformadapter | M1 | Umsetzung durch Öffentliche Core-API und Plattformadapter gemäß Kapitel 7, 25 und 26 | Architekturtest, Paket-/Repository-Review und Integrationssample |
| INT-007 | SOLL | Die Bibliotheksgruppe soll NuGet-fähig paketierbar sein. | Kapitel 7, 25 und 26 | Öffentliche Core-API und Plattformadapter | M1 | Umsetzung durch Öffentliche Core-API und Plattformadapter gemäß Kapitel 7, 25 und 26 | Architekturtest, Paket-/Repository-Review und Integrationssample |
| INT-008 | MUSS | Öffentliche APIs müssen versioniert und dokumentiert sein. | Kapitel 7, 25 und 26 | Öffentliche Core-API und Plattformadapter | M1 | Umsetzung durch Öffentliche Core-API und Plattformadapter gemäß Kapitel 7, 25 und 26 | Architekturtest, Paket-/Repository-Review und Integrationssample |
| INT-009 | SOLL | Desktop Components soll den Editor nur referenzieren und als Showcase verwenden. | Kapitel 7, 25 und 26 | Öffentliche Core-API und Plattformadapter | M1 | Umsetzung durch Öffentliche Core-API und Plattformadapter gemäß Kapitel 7, 25 und 26 | Architekturtest, Paket-/Repository-Review und Integrationssample |
| INT-010 | MUSS | Data Toolbox darf optionaler Speicherdienst sein, aber keine Pflichtabhängigkeit. | Kapitel 7, 25 und 26 | Öffentliche Core-API und Plattformadapter | M1 | IDocumentStorage/FileDocumentStorage mit atomarem Speichern; TabSettings und VisualColumnCalculator | Architekturtest, Paket-/Repository-Review und Integrationssample |
| INT-011 | MUSS | Numerics, Graphics und GameWorks dürfen den Editor konsumieren, aber keine zyklischen Abhängigkeiten erzeugen. | Kapitel 7, 25 und 26 | Öffentliche Core-API und Plattformadapter | M1 | Umsetzung durch Öffentliche Core-API und Plattformadapter gemäß Kapitel 7, 25 und 26 | Architekturtest, Paket-/Repository-Review und Integrationssample |
| INT-012 | SOLL | Syntaxhervorhebung, Markdown-, Log-, Hex- oder Property-Editoren müssen später auf öffentlichen Erweiterungspunkten aufbauen können. | Kapitel 7, 25 und 26 | Öffentliche Core-API und Plattformadapter | M6 | Umsetzung durch Öffentliche Core-API und Plattformadapter gemäß Kapitel 7, 25 und 26 | Architekturtest, Paket-/Repository-Review und Integrationssample |
| SET-001 | MUSS | Einstellungen müssen Tastaturprofil, Theme, Schrift, Tabulatorbreite, Ränder, Word-Wrap, Auto-Indent, Undo-Limit und Anzeigeoptionen umfassen können. | Kapitel 25 | EditorSettings und JSON-Serialisierung | M1 | transaktionaler UndoManager mit Coalescing und Savepoint; EditorCommandRegistry, Dispatcher und serialisierbare KeyboardProfile; ViewLayoutService und optionaler ReformatParagraphCommand; AutoIndentPolicy beim NewLineCommand; TabSettings und VisualColumnCalculator | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| SET-002 | SOLL | Globale Defaults und dokument-/ansichtsspezifische Überschreibungen sollen getrennt werden. | Kapitel 25 | EditorSettings und JSON-Serialisierung | M1 | Umsetzung durch EditorSettings und JSON-Serialisierung gemäß Kapitel 25 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| SET-003 | SOLL | Einstellungen müssen serialisierbar, validierbar und wiederherstellbar sein. | Kapitel 25 | EditorSettings und JSON-Serialisierung | M1 | Umsetzung durch EditorSettings und JSON-Serialisierung gemäß Kapitel 25 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| SET-004 | MUSS | Ungültige Werte müssen mit klaren Fehlern oder sicheren Defaults behandelt werden. | Kapitel 25 | EditorSettings und JSON-Serialisierung | M1 | strukturierte EditorError-Codes plus hostseitiger Presenter | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| SET-005 | MUSS | 80 Spalten, 25 Zeilen und historische Farben dürfen nur Demo-/Kompatibilitätswerte sein. | Kapitel 25 | EditorSettings und JSON-Serialisierung | M1 | Umsetzung durch EditorSettings und JSON-Serialisierung gemäß Kapitel 25 | Unit Tests; ergänzend Integrations- und Akzeptanztests |
| NFR-ARCH-001 | MUSS | Klare Trennung zwischen Core, Anwendungsdiensten, UI-Adaptern und Samples. | Kapitel 27-34 | Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen | M1-M3 | Umsetzung durch Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen gemäß Kapitel 27-34 | Architektur-, Performance-, Security-, Accessibility- oder Dokumentationsgate |
| NFR-ARCH-002 | MUSS | Keine zyklischen Projekt- oder Paketabhängigkeiten. | Kapitel 27-34 | Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen | M1-M3 | TabSettings und VisualColumnCalculator | Architektur-, Performance-, Security-, Accessibility- oder Dokumentationsgate |
| NFR-ARCH-003 | MUSS | UI-Frameworktypen dürfen nicht in öffentliche Core-APIs gelangen. | Kapitel 27-34 | Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen | M1-M3 | Umsetzung durch Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen gemäß Kapitel 27-34 | Architektur-, Performance-, Security-, Accessibility- oder Dokumentationsgate |
| NFR-PERF-001 | MUSS | Normale Bearbeitungs- und Cursoroperationen müssen subjektiv unmittelbar reagieren. | Kapitel 27-34 | Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen | M1-M3 | Umsetzung durch Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen gemäß Kapitel 27-34 | Architektur-, Performance-, Security-, Accessibility- oder Dokumentationsgate |
| NFR-PERF-002 | SOLL | Rendering soll nur betroffene sichtbare Bereiche aktualisieren. | Kapitel 27-34 | Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen | M1-M3 | Umsetzung durch Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen gemäß Kapitel 27-34 | Architektur-, Performance-, Security-, Accessibility- oder Dokumentationsgate |
| NFR-PERF-003 | MUSS | Lange Datei- und Suchoperationen dürfen die UI nicht dauerhaft blockieren. | Kapitel 27-34 | Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen | M1-M3 | TextSearchService mit SearchOptions, CancellationToken und SearchResult; IDocumentStorage/FileDocumentStorage mit atomarem Speichern | Architektur-, Performance-, Security-, Accessibility- oder Dokumentationsgate |
| NFR-PERF-004 | MUSS | Die Architektur muss spätere optimierte Puffer für sehr große Dateien ermöglichen. | Kapitel 27-34 | Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen | M3 | IDocumentStorage/FileDocumentStorage mit atomarem Speichern | Architektur-, Performance-, Security-, Accessibility- oder Dokumentationsgate |
| NFR-ROB-001 | MUSS | Ungespeicherte Änderungen müssen vor destruktiven Aktionen geschützt werden. | Kapitel 27-34 | Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen | M1-M3 | Savepoint im UndoManager und CloseGuard-Workflow; IDocumentStorage/FileDocumentStorage mit atomarem Speichern | Architektur-, Performance-, Security-, Accessibility- oder Dokumentationsgate |
| NFR-ROB-002 | MUSS | Dateispeichern muss gegen I/O-Fehler möglichst robust sein. | Kapitel 27-34 | Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen | M1-M3 | IDocumentStorage/FileDocumentStorage mit atomarem Speichern; strukturierte EditorError-Codes plus hostseitiger Presenter | Architektur-, Performance-, Security-, Accessibility- oder Dokumentationsgate |
| NFR-ROB-003 | MUSS | Der Core muss deterministisch und ohne globale mutable Singletons testbar sein. | Kapitel 27-34 | Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen | M1-M3 | TabSettings und VisualColumnCalculator | Architektur-, Performance-, Security-, Accessibility- oder Dokumentationsgate |
| NFR-SEC-001 | MUSS | Dateipfade und Inhalte sind als nicht vertrauenswürdig zu behandeln. | Kapitel 27-34 | Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen | M1-M3 | IDocumentStorage/FileDocumentStorage mit atomarem Speichern | Architektur-, Performance-, Security-, Accessibility- oder Dokumentationsgate |
| NFR-SEC-002 | MUSS | Makros, Skripte oder eingebettete Inhalte dürfen nicht automatisch ausgeführt werden. | Kapitel 27-34 | Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen | M6 | Umsetzung durch Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen gemäß Kapitel 27-34 | Architektur-, Performance-, Security-, Accessibility- oder Dokumentationsgate |
| NFR-SEC-003 | SOLL | Temporär- und Autosave-Dateien müssen kontrolliert und mit angemessenen Rechten gespeichert werden. | Kapitel 27-34 | Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen | M1-M3 | IDocumentStorage/FileDocumentStorage mit atomarem Speichern | Architektur-, Performance-, Security-, Accessibility- oder Dokumentationsgate |
| NFR-ACC-001 | MUSS | Desktopadapter müssen vollständig per Tastatur bedienbar sein. | Kapitel 27-34 | Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen | M1-M3 | EditorCommandRegistry, Dispatcher und serialisierbare KeyboardProfile | Architektur-, Performance-, Security-, Accessibility- oder Dokumentationsgate |
| NFR-ACC-002 | SOLL | Kontrast, Fokus und Screenreader-Informationen sollen berücksichtigt werden. | Kapitel 27-34 | Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen | M3 | AccessibleObject und vollständige Tastaturbedienung | Architektur-, Performance-, Security-, Accessibility- oder Dokumentationsgate |
| NFR-TEST-001 | MUSS | Der Core muss umfassende Unit Tests erhalten. | Kapitel 27-34 | Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen | M1-M3 | Umsetzung durch Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen gemäß Kapitel 27-34 | Architektur-, Performance-, Security-, Accessibility- oder Dokumentationsgate |
| NFR-TEST-002 | MUSS | Architekturtests müssen die Abhängigkeitsrichtung sichern. | Kapitel 27-34 | Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen | M1-M3 | Umsetzung durch Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen gemäß Kapitel 27-34 | Architektur-, Performance-, Security-, Accessibility- oder Dokumentationsgate |
| NFR-TEST-003 | MUSS | Integrationstests müssen Dateiabläufe, Undo/Redo, Suche, Ansichten und Tastaturprofile abdecken. | Kapitel 27-34 | Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen | M1-M3 | transaktionaler UndoManager mit Coalescing und Savepoint; TextSearchService mit SearchOptions, CancellationToken und SearchResult; IDocumentStorage/FileDocumentStorage mit atomarem Speichern; EditorCommandRegistry, Dispatcher und serialisierbare KeyboardProfile | Architektur-, Performance-, Security-, Accessibility- oder Dokumentationsgate |
| NFR-DOC-001 | MUSS | Öffentliche APIs müssen XML-Dokumentationskommentare erhalten. | Kapitel 27-34 | Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen | M1-M3 | Umsetzung durch Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen gemäß Kapitel 27-34 | Architektur-, Performance-, Security-, Accessibility- oder Dokumentationsgate |
| NFR-DOC-002 | MUSS | README, Lastenheft, Pflichtenheft, Architektur-, Entwickler-, Test- und Benutzerhandbuch müssen vorgesehen werden. | Kapitel 27-34 | Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen | M1-M3 | Umsetzung durch Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen gemäß Kapitel 27-34 | Architektur-, Performance-, Security-, Accessibility- oder Dokumentationsgate |
| NFR-DOC-003 | MUSS | Historische Funktionen müssen auf moderne Anforderungen oder bewusste Nichtübernahme abgebildet werden. | Kapitel 27-34 | Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen | M3 | Umsetzung durch Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen gemäß Kapitel 27-34 | Architektur-, Performance-, Security-, Accessibility- oder Dokumentationsgate |
| NFR-COMP-001 | SOLL | Der erste produktive Stand soll auf einer unterstützten .NET-LTS-Version basieren; die konkrete Version legt das Pflichtenheft fest. | Kapitel 27-34 | Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen | M1-M3 | Umsetzung durch Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen gemäß Kapitel 27-34 | Architektur-, Performance-, Security-, Accessibility- oder Dokumentationsgate |
| NFR-COMP-002 | MUSS | WinForms und WPF sind Windowsadapter; der Core bleibt plattformneutral. | Kapitel 27-34 | Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen | M4 | SasdEditorView/UserControl mit eigener EditorSurface; separater WPF-Adapter auf unverändertem Core | Architektur-, Performance-, Security-, Accessibility- oder Dokumentationsgate |
| NFR-LIC-001 | MUSS | Originalquellcode, Marken und geschützte Gestaltungselemente dürfen nicht ungeprüft übernommen werden. | Kapitel 27-34 | Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen | M0 | Umsetzung durch Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen gemäß Kapitel 27-34 | Architektur-, Performance-, Security-, Accessibility- oder Dokumentationsgate |
| NFR-LIC-002 | MUSS | Vor Veröffentlichung müssen Name, Markenbezug und Rechtehinweise geprüft werden. | Kapitel 27-34 | Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen | M0 | Umsetzung durch Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen gemäß Kapitel 27-34 | Architektur-, Performance-, Security-, Accessibility- oder Dokumentationsgate |
| NFR-MAINT-001 | MUSS | Code muss verständlich strukturiert, großzügig kommentiert und dokumentiert sein. | Kapitel 27-34 | Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen | M1-M3 | Umsetzung durch Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen gemäß Kapitel 27-34 | Architektur-, Performance-, Security-, Accessibility- oder Dokumentationsgate |
| NFR-MAINT-002 | MUSS | Breaking Changes müssen dokumentiert und versioniert werden. | Kapitel 27-34 | Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen | M3 | Umsetzung durch Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen gemäß Kapitel 27-34 | Architektur-, Performance-, Security-, Accessibility- oder Dokumentationsgate |
| NFR-OBS-001 | SOLL | Fehler und wichtige Dateioperationen sollen über abstrahiertes Logging protokollierbar sein. | Kapitel 27-34 | Querschnittliche Architektur-, Qualitäts- und Betriebsmaßnahmen | M1-M3 | IDocumentStorage/FileDocumentStorage mit atomarem Speichern; strukturierte EditorError-Codes plus hostseitiger Presenter; ILogger-Abstraktion ohne UI-Zwang | Architektur-, Performance-, Security-, Accessibility- oder Dokumentationsgate |

## 39. Abnahmekriterien und Prüfverfahren
| ID | Kriterium | Prüfverfahren | Fällig |
|---|---|---|---|
| AK-001 | Ein leeres Dokument kann erstellt, bearbeitet, gespeichert, geschlossen und identisch erneut geöffnet werden. | End-to-End-Test New/Edit/Save/Close/Open mit Byte-/Textvergleich. | M1 |
| AK-002 | Unicode-Zeichen einschließlich Umlauten, nichtlateinischer Schrift und Emojis bleiben bei unterstützter Codierung erhalten. | Unicode-Testkorpus mit Umlauten, CJK, RTL-Beispiel, Emoji und kombinierenden Zeichen. | M1 |
| AK-003 | Undo/Redo funktioniert über Einfügen, Löschen, mehrzeilige Änderungen, Blockoperationen und Ersetzen. | Undo/Redo-Integrationstest über alle genannten Operationstypen. | M1 |
| AK-004 | Zwei Ansichten desselben Dokuments teilen Inhalt, behalten aber eigene Cursor- und Scrollpositionen. | Workspace-Test mit zwei Views und separaten Caret-/Scrollzuständen. | M2 |
| AK-005 | Alle Befehlskategorien aus Kap. 12 sind implementiert, modern abgebildet oder ausdrücklich verschoben/entfallen. | Command-Katalog- und historische Traceability-Review. | M2 |
| AK-006 | Ein Tastaturprofil kann geladen werden, ohne den Core neu zu kompilieren. | JSON-Profil laden und Commandbindung ohne Core-Rebuild testen. | M2 |
| AK-007 | Ein Hostcommand kann registriert und über Menü und Tastatur ausgelöst werden. | Sample-Hostcommand registrieren, Menü und KeySequence auslösen. | M2 |
| AK-008 | Der Core lässt sich ohne Windows-UI-Thread testen. | Core-Testlauf in normalem Testprozess ohne WinForms-Initialisierung. | M1 |
| AK-009 | Das Core-Projekt enthält keine WinForms-, WPF- oder ASP.NET-Referenzen. | Architecture Test auf Assemblyreferenzen. | M1 |
| AK-010 | Beim Schließen geänderter Dokumente erhält der Host einen Speichern/Verwerfen/Abbrechen-Entscheidungspunkt. | CloseGuard-Integrationstest für Save, Discard und Cancel. | M1 |
| AK-011 | Dateifehler führen nicht zum Verlust des bisherigen Dokumentinhalts. | Fault-injection bei Stream-/Dateifehlern. | M1 |
| AK-012 | Suche und Ersetzen sind abbrechbar und melden 'nicht gefunden' eindeutig. | Cancellation- und NotFound-Tests für Search/Replace. | M1 |
| AK-013 | Die WinForms-Demo zeigt Status, mehrere Ansichten, Dateioperationen, Suche, Undo/Redo und mindestens zwei Tastaturprofile. | Manuelle und automatisierte Sample-Checkliste. | M2 |
| AK-014 | Öffentliche APIs besitzen XML-Dokumentation und Kernfunktionen automatisierte Tests. | XML-Dokumentations- und Test-CI-Gate. | M1 |
| AK-015 | Historische Kapitel- und API-Traceability ist im Repository vorhanden. | Dokumentationsreview gegen Kapitel-, Hook-, Modul- und API-Inventur. | M3 |

## 40. Historische Hooks und Erweiterungspunkte
| Historischer Hook | Historische Aufgabe | Konkrete moderne Umsetzung | Phase |
|---|---|---|---|
| `UserCommand` | Eingaben filtern, umbelegen, selbst verarbeiten oder unterdrücken | `ICommandInterceptor`, `EditorCommandRegistry`, `KeyboardProfile` | M1/M2 |
| `UserError` | Fehler selbst darstellen oder an Standardbehandlung weiterreichen | `IEditorErrorPresenter` und strukturierte `EditorError`-Resultate | M1/M2 |
| `UserStatusLine` | Standardstatus ersetzen oder ergänzen | `EditorStatusSnapshot`, `IEditorStatusSink`, Status-Events | M1/M2 |
| `UserReplace` | Interaktive Entscheidungen während Find/Replace beeinflussen | `IReplaceDecisionProvider` | M1/M2 |
| `UserTask` | Kooperative Hintergrundarbeit im Leerlauf | `IBackgroundOperationRunner`, Tasks und CancellationToken | M6/Adapter |
| `MakeWindow` / `RestoreWindow` | Popup zeichnen und darunterliegenden Bildschirm sichern | WinForms-/WPF-/Web-Dialog- oder Overlay-Service | M6/Adapter |
| `PulldownMenu` / `InitMainMenu` | Menüdefinition und Befehlsauslösung | Hostmenüs auf Command-IDs | M6/Adapter |
| `PrintNext` | Inkrementeller Hintergrunddruck | `IPrintService` ab M6 | M6/Adapter |
| `ErrorCheck` | MicroStar-spezifische Fehlerdarstellung | hostseitiger Presenter/Interceptor | M6/Adapter |
| `SecondChar` | Zweites Zeichen einer Präfixsequenz abfragen | `KeyboardSequenceResolver` | M1/M2 |
| `ReadBlock` / Block Write | Block als Datei einlesen oder schreiben | Selection Import/Export ab M6 | M6/Adapter |
| `SpellingCheck` | Rechtschreibprüfung | externer Sprachdienst ab M6 | M6/Adapter |

## 41. Historische Modul- und Dateizuordnung
| Historische Datei/Gruppe | Verantwortung | Zielzuordnung | Umsetzungsregel |
|---|---|---|---|
| `VARS.ED`, `VARS.MS` | Konstanten, Typen, globale Variablen | Gekapselte Core-Modelle, Optionen und Zustände | Keine direkte Quellcodeübernahme; nur Verantwortlichkeitsabbildung. |
| `USER.ED`, `USER.MS` | Niedrige Kernroutinen | Core/Text Buffer/Document Operations | Keine direkte Quellcodeübernahme; nur Verantwortlichkeitsabbildung. |
| `SCREEN.ED`, `SCREEN.MS` | Bildschirmdarstellung | WinForms-, WPF- oder Web-Renderingadapter | Keine direkte Quellcodeübernahme; nur Verantwortlichkeitsabbildung. |
| `INIT.ED`, `INIT.MS` | Initialisierung | Composition Root, Factory und Dependency Injection | Keine direkte Quellcodeübernahme; nur Verantwortlichkeitsabbildung. |
| `CMD.ED`, `CMD.MS`, `FASTCMD.MS` | Einzelbefehle | Command-Implementierungen | Keine direkte Quellcodeübernahme; nur Verantwortlichkeitsabbildung. |
| `KCMD.*`, `OCMD.*`, `QCMD.*` | Präfixbefehle | Command-Katalog und Key-Chord-Profile | Keine direkte Quellcodeübernahme; nur Verantwortlichkeitsabbildung. |
| `K.*`, `O.*`, `Q.*`, `DISP.*` | Dispatcher | Command Dispatcher / Input Mapping | Keine direkte Quellcodeübernahme; nur Verantwortlichkeitsabbildung. |
| `TASK.*` | Scheduler und Hauptschleife | Host-Eventloop, Tasks und Cancellation | Keine direkte Quellcodeübernahme; nur Verantwortlichkeitsabbildung. |
| `INPUT.*` | Tastatur und Typeahead | Plattform-Inputadapter | Keine direkte Quellcodeübernahme; nur Verantwortlichkeitsabbildung. |
| `EDITERR.MSG` | Fehlerkatalog | Lokalisierte Ressourcen / strukturierte Fehler | Keine direkte Quellcodeübernahme; nur Verantwortlichkeitsabbildung. |
| `FIRST-ED.PAS` | Einfaches Beispiel | Modern-FIRST-ED-Sample | Keine direkte Quellcodeübernahme; nur Verantwortlichkeitsabbildung. |
| `MS.PAS` und `MS.*` | Sophisticated Editor / MicroStar | Erweiterte Demo beziehungsweise Showcase | Keine direkte Quellcodeübernahme; nur Verantwortlichkeitsabbildung. |
| `PRINT.MS` | Hintergrunddruck | Optionaler Print-Service | Keine direkte Quellcodeübernahme; nur Verantwortlichkeitsabbildung. |
| `MSCMD.MS` | MicroStar-spezifische Befehle | Sample-/Hostcommands | Keine direkte Quellcodeübernahme; nur Verantwortlichkeitsabbildung. |
| `SPELL.MS` | Rechtschreibprüfung | Optionale externe Erweiterung | Keine direkte Quellcodeübernahme; nur Verantwortlichkeitsabbildung. |
| `PULLDOWN.MS` | Pulldown-Menüs | UI-Adapter und Command-Menüs | Keine direkte Quellcodeübernahme; nur Verantwortlichkeitsabbildung. |
| `.OVL` und Chain-/Overlay-Dateien | Speicherüberlagerung | Entfällt; Assemblies und normale Laufzeitverwaltung | Keine direkte Quellcodeübernahme; nur Verantwortlichkeitsabbildung. |
| `README.COM`, `READ.ME` | Versions- und Distributionshinweise | README, CHANGELOG und Release Notes | Keine direkte Quellcodeübernahme; nur Verantwortlichkeitsabbildung. |

## 42. Historische Konstanten und Typen
### 42.1 Konstanten
| Historisches Element | Bedeutung | Technische Umsetzung |
|---|---|---|
| `Col1`, `Line1` | Logische Startpositionen | Normale Positionsabstraktion |
| `Colored`, `Inblock`, `Wrapped` | Zeilenflags | Generische Annotationen, Auswahl und Wrap-Metadaten |
| `Ctrla` bis `Ctrlz`, `Del`, `Escape`, `Nul` | ASCII-Tastencodes | Key-Gesture- und Input-Abstraktion |
| `Defhelplen` | Hilfefensterhöhe | UI-Layoutkonfiguration |
| `Defnocols`, `Defnorows` | 80x25-Bildschirm | Nur Retro-Demo, kein Produktlimit |
| `Deftypahd` | Typeahead-Puffergröße | Konfigurierbare Queue oder Frameworkinput |
| `Errorfile` | `EDITERR.MSG` | Lokalisierbarer Fehlerkatalog |
| `Maxlinelength` | 255 Zeichen | Feste Grenze entfällt |
| `Maxmarker` | 20 Marker | Mindestens kompatibel, intern erweiterbar |
| `Nofile` | `NONAME` | Untitled-Dokumentzustand |
| `Screenadr` | Videospeicheradresse | Entfällt vollständig |
| Statuslabels | AI, Col, File, INS, Line, TE, Window, WW | Lokalisierbares Statusmodell |
| `Zero` bis `Nine` | ASCII-Ziffern | Normale Eingabeverarbeitung |

### 42.2 Typen
| Historischer Typ | Bedeutung | Technische Umsetzung |
|---|---|---|
| `Character` | Zeichen plus Farbe | Rendering-Glyph/Style im UI-Adapter |
| `Insflag` | Insert oder Typeover | EditorInsertMode |
| `Linedesc` / `Plinedesc` | Verketteter Zeilendeskriptor | Interne Pufferimplementierung |
| `Textline` / `Ptextline` | Text einer Zeile | Unicode-Zeilen- oder Span-Abstraktion |
| `Windesc` / `Pwindesc` | Fensterzustand und Textstromreferenz | EditorViewState plus DocumentReference |
| feste Pascal-Strings | String80, Varstring, Strvartype, St6 | `string`/Spans ohne historische Längengrenze |


## 43. Historische globale Zustände
| Historische Variable | Moderne Verantwortlichkeit |
|---|---|
| `Abortcmd` | CancellationToken/OperationState |
| `Aborting` | CancellationToken/OperationState |
| `Asking` | Prompt-/Status-Hostzustand |
| `Blockcolor` | EditorTheme/AnnotationStyle |
| `Blockfrom` | TextSelection/SelectionRenderOptions |
| `Blockhide` | TextSelection/SelectionRenderOptions |
| `Blockto` | TextSelection/SelectionRenderOptions |
| `Bordcolor` | EditorTheme/AnnotationStyle |
| `Circbuf` | KeyboardSequenceResolver/Inputadapter; kein globaler Ringpuffer |
| `Circin` | KeyboardSequenceResolver/Inputadapter; kein globaler Ringpuffer |
| `Circout` | KeyboardSequenceResolver/Inputadapter; kein globaler Ringpuffer |
| `Cmdcol` | Prompt-/Status-Hostzustand |
| `Cmdcolor` | EditorTheme/AnnotationStyle |
| `Cmdlinest` | Prompt-/Status-Hostzustand |
| `Curwin` | EditorWorkspace und View-Liste |
| `EditChangeflag` | UndoManager.Savepoint / TextDocument.IsDirty |
| `EditUsercommandInput` | KeyboardSequenceResolver/Inputadapter; kein globaler Ringpuffer |
| `Interactive` | Prompt-/Status-Hostzustand |
| `Intrflag` | CancellationToken/OperationState |
| `Linelength` | WinForms Viewport/Layout/Renderer; nicht Core-global |
| `Logscrcols` | WinForms Viewport/Layout/Renderer; nicht Core-global |
| `Logscrrows` | WinForms Viewport/Layout/Renderer; nicht Core-global |
| `Logtopscr` | WinForms Viewport/Layout/Renderer; nicht Core-global |
| `Marker` | MarkerCollection |
| `Nextstream` | DocumentId-/Workspace-Factory |
| `Notfound` | SearchSession/SearchOptions/SearchResult |
| `Optstr` | SearchSession/SearchOptions/SearchResult |
| `Physcrcols` | WinForms Viewport/Layout/Renderer; nicht Core-global |
| `Physcrrows` | WinForms Viewport/Layout/Renderer; nicht Core-global |
| `Physcrsig` | WinForms Viewport/Layout/Renderer; nicht Core-global |
| `Replacestr` | SearchSession/SearchOptions/SearchResult |
| `Retracemode` | WinForms Viewport/Layout/Renderer; nicht Core-global |
| `Rundown` | CancellationToken/OperationState |
| `Screen` | WinForms Viewport/Layout/Renderer; nicht Core-global |
| `Searchstr` | SearchSession/SearchOptions/SearchResult |
| `Tabsize` | EditorViewState.TabSettings |
| `Txtcolor` | EditorTheme/AnnotationStyle |
| `Typbufovl` | KeyboardSequenceResolver/Inputadapter; kein globaler Ringpuffer |
| `Undocount` | UndoManager-Instanz je Dokument |
| `Undoend` | UndoManager-Instanz je Dokument |
| `Undolimit` | UndoManager-Instanz je Dokument |
| `Undostack` | UndoManager-Instanz je Dokument |
| `Updcurflag` | WinForms Viewport/Layout/Renderer; nicht Core-global |
| `Usercolor` | EditorTheme/AnnotationStyle |
| `Window1` | EditorWorkspace und View-Liste |
| `Winstack` | EditorWorkspace und View-Liste |

## 44. Historische Prozeduren- und Funktionszuordnung
| Historische Routine | Moderne Kategorie | Konkrete Behandlung |
|---|---|---|
| `Advance` | Core Utility | Interne Hilfsfunktion oder durch idiomatische .NET-API ersetzt |
| `EditAbort` | Editing/Formatting | ITextBuffer und Editing-/Format-Commands |
| `EditAppchar` | Commands/Input | Registry, Dispatcher, Profile und SequenceResolver |
| `EditAppcmdnam` | Commands/Input | Registry, Dispatcher, Profile und SequenceResolver |
| `EditAskfor` | Errors/Prompts | EditorError und Hostdienste |
| `EditBackground` | Background/Cancellation | Tasks und CancellationToken; DOS-Scheduler entfällt |
| `EditBeginningEndLine` | Navigation | Navigation-Commands |
| `EditBeginningLine` | Navigation | Navigation-Commands |
| `EditBlockBegin` | Selection/Block | TextSelection und Block-Commands |
| `EditBlockCopy` | Selection/Block | TextSelection und Block-Commands |
| `EditBlockDelete` | Selection/Block | TextSelection und Block-Commands |
| `EditBlockEnd` | Selection/Block | TextSelection und Block-Commands |
| `EditBlockHide` | Selection/Block | TextSelection und Block-Commands |
| `EditBlockMove` | Selection/Block | TextSelection und Block-Commands |
| `EditBottomBlock` | Selection/Block | TextSelection und Block-Commands |
| `EditBreathe` | Background/Cancellation | Tasks und CancellationToken; DOS-Scheduler entfällt |
| `EditCenterLine` | Editing/Formatting | ITextBuffer und Editing-/Format-Commands |
| `EditChangeCase` | Editing/Formatting | ITextBuffer und Editing-/Format-Commands |
| `EditClsinp` | Commands/Input | Registry, Dispatcher, Profile und SequenceResolver |
| `EditColorFile` | WinForms Rendering/Layout | Adapterintern; keine öffentliche 1:1-API |
| `EditColorLine` | WinForms Rendering/Layout | Adapterintern; keine öffentliche 1:1-API |
| `EditCompressLine` | Editing/Formatting | ITextBuffer und Editing-/Format-Commands |
| `EditCpcrewin` | Workspace/View | EditorWorkspace und View-Commands |
| `EditCpdelwin` | Workspace/View | EditorWorkspace und View-Commands |
| `EditCpexit` | Lifecycle | CloseGuard/App.Exit Command |
| `EditCpFileSave` | Storage/Document Lifecycle | IDocumentStorage bzw. File-Commands |
| `EditCpFind` | Search | TextSearchService und Search-/Replace-Commands |
| `EditCpgotocl` | Navigation | Navigation-Commands |
| `EditCpgotoln` | Navigation | Navigation-Commands |
| `EditCpgotowin` | Workspace/View | EditorWorkspace und View-Commands |
| `EditCpjmpmrk` | Marker | MarkerCollection und Marker-Commands |
| `EditCplnkwin` | Workspace/View | EditorWorkspace und View-Commands |
| `EditCpReplace` | Search | TextSearchService und Search-/Replace-Commands |
| `EditCprfw` | Core Utility | Interne Hilfsfunktion oder durch idiomatische .NET-API ersetzt |
| `EditCpsetlm` | Core Utility | Interne Hilfsfunktion oder durch idiomatische .NET-API ersetzt |
| `EditCpsetmrk` | Marker | MarkerCollection und Marker-Commands |
| `EditCpsetrm` | Core Utility | Interne Hilfsfunktion oder durch idiomatische .NET-API ersetzt |
| `EditCptabdef` | Editing/Formatting | ITextBuffer und Editing-/Format-Commands |
| `EditCpundlim` | Core Utility | Interne Hilfsfunktion oder durch idiomatische .NET-API ersetzt |
| `EditCpwfw` | Core Utility | Interne Hilfsfunktion oder durch idiomatische .NET-API ersetzt |
| `EditCvts2i` | Core Utility | Interne Hilfsfunktion oder durch idiomatische .NET-API ersetzt |
| `EditDecline` | Errors/Prompts | EditorError und Hostdienste |
| `EditDefineTab` | Editing/Formatting | ITextBuffer und Editing-/Format-Commands |
| `EditDeleteLeftChar` | Editing/Formatting | ITextBuffer und Editing-/Format-Commands |
| `EditDeleteLine` | Editing/Formatting | ITextBuffer und Editing-/Format-Commands |
| `EditDeleteRightChar` | Editing/Formatting | ITextBuffer und Editing-/Format-Commands |
| `EditDeleteRightWord` | Editing/Formatting | ITextBuffer und Editing-/Format-Commands |
| `EditDeleteTextRight` | Editing/Formatting | ITextBuffer und Editing-/Format-Commands |
| `EditDelline` | Editing/Formatting | ITextBuffer und Editing-/Format-Commands |
| `EditDestxtdes` | Core Utility | Interne Hilfsfunktion oder durch idiomatische .NET-API ersetzt |
| `EditDownLine` | Navigation | Navigation-Commands |
| `EditDownPage` | Navigation | Navigation-Commands |
| `EditEndLine` | Navigation | Navigation-Commands |
| `EditErrormsg` | Errors/Prompts | EditorError und Hostdienste |
| `EditExit` | Lifecycle | CloseGuard/App.Exit Command |
| `EditFileRead` | Storage/Document Lifecycle | IDocumentStorage bzw. File-Commands |
| `EditFileWrite` | Storage/Document Lifecycle | IDocumentStorage bzw. File-Commands |
| `EditFind` | Search | TextSearchService und Search-/Replace-Commands |
| `EditGenlineno` | Core Utility | Interne Hilfsfunktion oder durch idiomatische .NET-API ersetzt |
| `EditGotoColumn` | Navigation | Navigation-Commands |
| `EditGotoLine` | Navigation | Navigation-Commands |
| `EditHscroll` | WinForms Rendering/Layout | Adapterintern; keine öffentliche 1:1-API |
| `EditIncline` | Editing/Formatting | ITextBuffer und Editing-/Format-Commands |
| `EditInitialize` | Composition | EditorToolkitFactory/Composition Root |
| `EditInsertCtrlChar` | Editing/Formatting | ITextBuffer und Editing-/Format-Commands |
| `EditInsertLine` | Editing/Formatting | ITextBuffer und Editing-/Format-Commands |
| `EditJoinLine` | Editing/Formatting | ITextBuffer und Editing-/Format-Commands |
| `EditJumpMarker` | Marker | MarkerCollection und Marker-Commands |
| `EditK` | Commands/Input | Registry, Dispatcher, Profile und SequenceResolver |
| `EditLeftChar` | Navigation | Navigation-Commands |
| `EditLeftWord` | Navigation | Navigation-Commands |
| `EditLongLine` | Editing/Formatting | ITextBuffer und Editing-/Format-Commands |
| `EditMarkblock` | Selection/Block | TextSelection und Block-Commands |
| `EditNewLine` | Editing/Formatting | ITextBuffer und Editing-/Format-Commands |
| `EditO` | Commands/Input | Registry, Dispatcher, Profile und SequenceResolver |
| `EditOffblock` | Selection/Block | TextSelection und Block-Commands |
| `EditPrccmd` | Commands/Input | Registry, Dispatcher, Profile und SequenceResolver |
| `EditPrctxt` | Editing/Formatting | ITextBuffer und Editing-/Format-Commands |
| `EditPushtbf` | Commands/Input | Registry, Dispatcher, Profile und SequenceResolver |
| `EditQ` | Commands/Input | Registry, Dispatcher, Profile und SequenceResolver |
| `EditRealign` | WinForms Rendering/Layout | Adapterintern; keine öffentliche 1:1-API |
| `EditReatxtfil` | Storage/Document Lifecycle | IDocumentStorage bzw. File-Commands |
| `EditReformat` | Editing/Formatting | ITextBuffer und Editing-/Format-Commands |
| `EditReplace` | Search | TextSearchService und Search-/Replace-Commands |
| `EditRightChar` | Navigation | Navigation-Commands |
| `EditRightWord` | Navigation | Navigation-Commands |
| `EditSchedule` | Background/Cancellation | Tasks und CancellationToken; DOS-Scheduler entfällt |
| `EditScrollDown` | Navigation | Navigation-Commands |
| `EditScrollUp` | Navigation | Navigation-Commands |
| `EditSetLeftMargin` | Editing/Formatting | ITextBuffer und Editing-/Format-Commands |
| `EditSetMarker` | Marker | MarkerCollection und Marker-Commands |
| `EditSetRightMargin` | Editing/Formatting | ITextBuffer und Editing-/Format-Commands |
| `EditSetUndoLimit` | Undo | UndoManager und Settings |
| `EditShiftLine` | Editing/Formatting | ITextBuffer und Editing-/Format-Commands |
| `EditShortLine` | Editing/Formatting | ITextBuffer und Editing-/Format-Commands |
| `EditSystem` | Background/Cancellation | Tasks und CancellationToken; DOS-Scheduler entfällt |
| `EditTab` | Editing/Formatting | ITextBuffer und Editing-/Format-Commands |
| `EditToggleAutoindent` | Editing/Formatting | ITextBuffer und Editing-/Format-Commands |
| `EditToggleInsert` | Editing/Formatting | ITextBuffer und Editing-/Format-Commands |
| `EditToggleWordwrap` | Editing/Formatting | ITextBuffer und Editing-/Format-Commands |
| `EditTopBlock` | Selection/Block | TextSelection und Block-Commands |
| `EditUndo` | Undo | UndoManager und Settings |
| `EditUpcase` | Editing/Formatting | ITextBuffer und Editing-/Format-Commands |
| `EditUpdphyscr` | WinForms Rendering/Layout | Adapterintern; keine öffentliche 1:1-API |
| `EditUpdrowasm` | WinForms Rendering/Layout | Adapterintern; keine öffentliche 1:1-API |
| `EditUpdwindow` | WinForms Rendering/Layout | Adapterintern; keine öffentliche 1:1-API |
| `EditUpdwinsl` | WinForms Rendering/Layout | Adapterintern; keine öffentliche 1:1-API |
| `EditUpLine` | Navigation | Navigation-Commands |
| `EditUpPage` | Navigation | Navigation-Commands |
| `EditUserpush` | Commands/Input | Registry, Dispatcher, Profile und SequenceResolver |
| `EditWindowBottomFile` | Workspace/View | EditorWorkspace und View-Commands |
| `EditWindowCreate` | Workspace/View | EditorWorkspace und View-Commands |
| `EditWindowDelete` | Workspace/View | EditorWorkspace und View-Commands |
| `EditWindowDeleteText` | Workspace/View | EditorWorkspace und View-Commands |
| `EditWindowDown` | Workspace/View | EditorWorkspace und View-Commands |
| `EditWindowGoto` | Workspace/View | EditorWorkspace und View-Commands |
| `EditWindowLink` | Workspace/View | EditorWorkspace und View-Commands |
| `EditWindowTopFile` | Workspace/View | EditorWorkspace und View-Commands |
| `EditWindowUp` | Workspace/View | EditorWorkspace und View-Commands |
| `EditZapcmdnam` | Commands/Input | Registry, Dispatcher, Profile und SequenceResolver |
| `MoveFromScreen` | WinForms Rendering/Layout | Adapterintern; keine öffentliche 1:1-API |
| `MoveToScreen` | WinForms Rendering/Layout | Adapterintern; keine öffentliche 1:1-API |
| `Pokechr` | Commands/Input | Registry, Dispatcher, Profile und SequenceResolver |

## 45. Bewusst nicht wörtlich übernommene Technik
| Historische Technik | Grund der Nichtübernahme | Moderner Ersatz |
|---|---|---|
| Direkter Videospeicher und Assembler-Row-Updates | Framework-, Hardware- und Plattformkopplung | WinForms/WPF/Web-Renderingadapter mit Invalidation und Caching |
| DOS-Polling-Scheduler | Blockierend, proprietär und nicht mit modernen Eventloops kompatibel | Task, async/await, CancellationToken, Dispatcher |
| Typeahead-Ringpuffer und rohe Scan-Codes | Nicht portabel und schlecht lokalisierbar | KeyGesture/KeySequence und plattformspezifischer Inputadapter |
| Overlays und Chain-Files | Historische Speicherknappheit besteht nicht in gleicher Form | Assemblies, Pakete, Lazy Loading und normale Prozessverwaltung |
| Globale Pointerlisten | Schlecht testbar, fehleranfällig und nicht thread-/hostfähig | Gekapselte Dokument-, Buffer-, View- und Workspace-Objekte |
| ASCII und feste Pascal-Strings | Unzureichend für moderne Texte | Unicode-.NET-Strings und Streams |
| 255-Zeichen-Zeilenlimit | Unnötige historische Begrenzung | Ressourcenbasierte Limits und austauschbarer Puffer |


## 46. Schlussfolgerung und Implementierungsfreigabe
Mit diesem Pflichtenheft ist die Umsetzung des SASD Editor Toolkit so konkretisiert, dass ein Repository-Skeleton, Architecture Decision Records und anschließend der erste Meilenstein „Modern FIRST-ED 0.1“ erstellt werden können. Der Core wird bewusst klein und UI-unabhängig gehalten; die historische Funktionsbreite wird nachvollziehbar, aber schrittweise ergänzt.

Die Freigabe dieses Pflichtenhefts bestätigt insbesondere:
1. eigenständiges Repository und eigenständige Pakete,
2. .NET-10-Core und WinForms als erster Adapter,
3. eigene WinForms-Editorfläche statt fachlicher RichTextBox-Kopplung,
4. LineTextBuffer für M1 und austauschbare Large-File-Strategie,
5. Command-first-Architektur und serialisierbare Tastaturprofile,
6. strikte Trennung von Dokument und View,
7. vollständige Lastenheft-Traceability,
8. schrittweise Erweiterung zu WPF, Web und spezialisierten Desktop Components ohne Monolithbildung.
