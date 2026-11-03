# Benutzerhandbuch – Modern FIRST-ED (SASD Editor Toolkit)

[English](../en/User-Guide.md) · [Dokumentationsindex](README.md) · [Architektur](Architektur.md)

**Geltungsbereich:** die derzeitige **WinForms-Beispielanwendung** des eigenständigen SASD Editor Toolkit.  
**Stand:** 03.11.2026 · **Status:** M1-Entwicklungsstand, **kein freigegebener Texteditor für produktionskritische Daten**.

## 1. Was ist die Anwendung?

Modern FIRST-ED zeigt, wie sich eine eigenständige Editorbibliothek in eine Windows-Anwendung integrieren lässt. Die Anwendung hat ein selbst gezeichnetes Textfeld, Menü- und Werkzeugleisten, eine Statusanzeige sowie grundlegende Datei-, Editier- und Suchfunktionen.

Die Anwendung ist derzeit eine **Referenz- und Testoberfläche**, keine vollständige Textverarbeitung, IDE oder fertig portierte Turbo Editor Toolbox. Die Dokumentation beschreibt nur Funktionen, die dem vorhandenen Code zugeordnet werden können.

## 2. Voraussetzungen und Start

- Windows mit .NET SDK 10.x und den zum Bauen von WinForms nötigen Desktop-Komponenten.
- Lokale Arbeitskopie des Repositories.
- Für Benutzerdateien zunächst **Kopien statt Originale** verwenden, insbesondere bei wichtigen oder ungewöhnlich codierten Dateien.

Im Repository-Verzeichnis:

\`\`\`powershell
dotnet restore .\SASD-Editor-Toolkit.sln
dotnet run --project .\samples\Sasd.EditorToolkit.Sample.FirstEd.WinForms\Sasd.EditorToolkit.Sample.FirstEd.WinForms.csproj
\`\`\`

Wenn kein .NET SDK installiert ist, die Entwickleranleitung zur Einrichtung lesen. Es wird hier kein fertiger Installer oder öffentlich abgenommenes Release behauptet.

## 3. Aufbau der Oberfläche

| Bereich | Aufgabe |
|---|---|
| **File** | Neues Dokument, Öffnen, Speichern, Speichern unter, Beenden |
| **Edit** | Undo und Redo |
| **Search** | Find und Find Next |
| **View / Tools / Help** | derzeit Menüplatzhalter beziehungsweise Entwicklungsoberfläche; nicht als vollständige Funktionsbereiche zugesagt |
| **Toolbar** | Schnellzugriff auf New, Open, Save, Undo, Redo, Find und Find Next |
| **Editorfläche** | Text, Zeilennummern, markierte aktuelle Zeile, Caret, Suchauswahl |
| **Statusleiste** | vom Sample aufbereitete aktuelle Dokument-/Positionsinformationen |

Der sichtbare Inhalt beim Erstellen eines neuen Dokuments ist derzeit ein **Demotext**, nicht zwingend eine vollkommen leere Datei.

## 4. Dateien bearbeiten

### Neu

**File → New** erstellt ein neues Beispieldokument. Wenn das bisherige Dokument verändert wurde, fragt das Sample nach dem Umgang mit ungespeicherten Änderungen.

### Öffnen

**File → Open** öffnet den Windows-Dateidialog. Voreingestellt sind textartige Dateitypen wie \`.txt\`, \`.md\`, \`.cs\`, \`.json\` und \`.xml\`; andere Dateien lassen sich über den entsprechenden Filter anzeigen.

**Achtung:** Das Toolkit besitzt erste Encoding-/BOM-Erkennung, aber noch keine vollständige sichere Behandlung aller Binärformate und nicht unterstützten Codierungen. Deshalb keine Binärdateien oder unersetzlichen Originale zum Testen verwenden.

### Speichern

**File → Save** schreibt an den bisherigen Dateipfad; bei einem unbenannten Dokument wird ein Ziel angefragt. **Save As** fordert einen neuen Dateinamen an.

Das Core verwendet für Pfadspeicherung standardmäßig eine temporäre Datei und eine anschließende Ersetzung. Das ist **kein** Ersatz für ein Backup oder ein umfassend getestetes Recovery-Konzept. Externe Dateiänderungen werden noch nicht vollständig erkannt.

### Schließen

Bei **File → Exit**, beim Öffnen oder Neu-Anlegen mit ungespeicherten Änderungen bietet die Anwendung sinngemäß **Speichern / Verwerfen / Abbrechen** an. Bei **Abbrechen** bleibt der Vorgang aus.

## 5. Tastatur und Bearbeitung

Im vorhandenen M1-Stand sind grundlegende Bewegungs- und Editieraktionen über Commands verbunden:

| Eingabe | Wirkung |
|---|---|
| Zeichen tippen | Text an der Caretposition einfügen |
| Enter | neue Zeile |
| Backspace / Delete | links/rechts löschen; an Zeilengrenzen können Zeilen verbunden werden |
| Pfeiltasten | Caret links, rechts, hoch und runter |
| Home / End | Anfang/Ende der logischen Zeile |
| Strg+Z / Strg+Y | Undo / Redo |
| Find / Find Next | Suchbegriff eingeben bzw. letzte Suche erneut verwenden |

**Nicht voraussetzen:** vollständige Textauswahl per Maus, Copy/Cut/Paste-Bedienung, vollständige Unicode-Graphemnavigation, frei konfigurierbare Tastaturprofile, WordStar-Präfixkommandos, Word-Wrap-Menüs oder mehrstufige Blockbefehle. Diese Bereiche sind noch Teil künftiger Entwicklungsarbeiten.

## 6. Text suchen

1. **Search → Find…** oder **Find** in der Toolbar aufrufen.
2. Suchbegriff eingeben und bestätigen.
3. Die Suche erfolgt durch den UI-unabhängigen Core und aktualisiert die Auswahl-/Caretposition.
4. **Find Next** verwendet den zuletzt im Sample eingegebenen Begriff erneut.

Die zugrundeliegende Suche unterstützt intern Richtung und Vergleichsoptionen. Das Sample stellt diese Optionen aber derzeit **nicht vollständig als Dialogeinstellungen** bereit. Reguläre Ausdrücke, allgemeines Ersetzen und fortgeschrittene Filter sind noch keine zugesicherten Funktionen der Oberfläche.

## 7. Was die Anwendung noch nicht kann

- keine allgemeine Desktop-IDE, kein vollständiger WYSIWYG-Editor;
- kein fertiges MicroStar- oder historisches WordStar-Bedienprofil;
- keine vollständig integrierte Split-View-/Mehrfensterbedienung im Sample;
- kein umfassendes Replace-All oder Regex-UI;
- keine zugesagte Verarbeitung sehr großer Dateien;
- keine vollständige Barrierefreiheits- oder IME-Abnahme;
- keine produktiv abgenommene Autosave-/Crash-Recovery-Funktion;
- keine gesicherte Warnung bei extern verändertem Zieldokument.

Die Roadmap benennt den vorgesehenen Ausbau. Ein Screenshot ist ein **Gestaltungskonzept**, keine Garantie über jede dargestellte Funktion.

## 8. Probleme eingrenzen

| Problem | Vorgehen |
|---|---|
| Anwendung startet nicht | Windows-/SDK-Version, Restore/Build und GitHub-CI prüfen |
| Datei lässt sich nicht öffnen | Leserechte, Dateityp und Encoding prüfen; Original unverändert lassen |
| Suche findet nichts | Suchbegriff, Startposition und nachfolgendes Find Next prüfen |
| Ungewollte Änderung | zuerst Undo versuchen, nicht wiederholt speichern |
| Speichern scheitert | Dateizugriff/Pfad prüfen; Text nicht verwerfen; Fehler protokollieren |
| Großes Dokument reagiert träge | M1-Puffer ist noch nicht auf Large Files ausgelegt |
| Darstellung/IME/Fokus problematisch | Windows-Version, DPI, Eingabemethode und Schritte für einen Bugreport dokumentieren |

## 9. Fehler melden

Für einen reproduzierbaren Bericht angeben: verwendeter Commit, Windows- und .NET-Version, kurze Schritte, erwartetes Verhalten, beobachtetes Verhalten sowie – falls möglich – eine künstliche, **nicht vertrauliche** Testdatei. Keine Dokumentinhalte oder persönlichen Pfade öffentlich veröffentlichen, wenn sie vertraulich sind.

Weitere Informationen: [Testhandbuch](Testhandbuch.md), [Entwicklerhandbuch](Entwicklerhandbuch.md) und [Roadmap](ROADMAP.md).
