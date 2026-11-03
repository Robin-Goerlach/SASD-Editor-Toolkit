# Testhandbuch und Qualitätssicherung – SASD Editor Toolkit

[English](../en/Testing-Guide.md) · [Architektur](Architektur.md) · [Entwicklerhandbuch](Entwicklerhandbuch.md) · [Roadmap](ROADMAP.md)

**Stand:** 03.11.2026. **Geltung:** eigenständiges SASD Editor Toolkit.  
**Teststatus ist immer commit- und umgebungsbezogen.** Dieses Dokument beschreibt einen Prüfprozess und behauptet keine nicht ausgeführten Tests.

## 1. Qualitätsziele

1. Text- und Dateiintegrität hat Vorrang vor UI-Politur und Geschwindigkeit.
2. Fachliches Verhalten wird im Core automatisiert geprüft, unabhängig von WinForms.
3. Ein Adaptertest prüft Integration und Bedienung – er ersetzt keinen Core-Test.
4. Jede neue öffentliche Verhaltensregel besitzt Beispiele und nach Möglichkeit sprachneutrale Conformance-Vektoren.
5. Änderungen dürfen weder Architekturgrenzen noch Sicherheit und Zugänglichkeit stillschweigend verschlechtern.
6. Ein grüner Testlauf ist kein Produktrelease; fehlende Testklassen und manuelle Beobachtungen bleiben sichtbar.

## 2. Testpyramide und tatsächliche Infrastruktur

| Ebene | Zweck | Heute | Geplante Ergänzung |
|---|---|---|---|
| Unit / Core | Textmodell, Commands, Suche, Undo, Storage | xUnit unter \`tests/Sasd.EditorToolkit.Core.Tests/\` | Unicode-/Selection-/M1c-Randfälle |
| Integration | Dateisystem, Streams, mehrere Views, Hostgrenzen | einige Storage-/Workspace-bezogene Prüfungen, kein vollständiges eigenes Integrationsprojekt | Fehler-/Conflict- und Recovery-Tests |
| Architektur | Abhängigkeitsrichtung, stabile IDs, keine Doppelimplementierung | Vorgaben in AGENTS/ADRs | automatische Assembly-/Projekt- und ID-Prüfung |
| WinForms | Eingabe, Rendering, Caret, DPI, IME, Dialoge | Beispielanwendung vorhanden | reproduzierbare UI-Smokes und ggf. UI-Automation |
| Conformance | gleiche beobachtbare Semantik für spätere Ports | Spezifikationsbeispiel, noch keine vollständige Vector-Suite | JSON-Vektoren für Editieren/Suche |
| Performance | Zeit-/Speicherbudget unter definierten Bedingungen | nicht als vollständiges Gate nachgewiesen | Benchmarks, Large-File-Profile |
| Release | API, Pakete, Dokumente, Lizenz, Consumer | keine produktive Release-Abnahme behauptet | M3-Gate vor Veröffentlichung |

## 3. Prüfungen ausführen

**Linux/macOS/WSL – Core:**

\`\`\`bash
./scripts/verify-core.sh
\`\`\`

Das Skript führt Restore, Build und Test des Core-Testprojekts sowie \`git diff --check\` aus. Es kompiliert **nicht** die vollständige WinForms-Solution.

**Windows – gesamte Solution:**

\`\`\`powershell
powershell -ExecutionPolicy Bypass -File .\scripts\verify-all.ps1
\`\`\`

Dieses Skript führt Restore, Build und Tests über \`SASD-Editor-Toolkit.sln\` aus. Alternativ:

\`\`\`powershell
dotnet restore .\SASD-Editor-Toolkit.sln
dotnet build .\SASD-Editor-Toolkit.sln -c Debug --no-restore
dotnet test .\SASD-Editor-Toolkit.sln -c Debug --no-build
\`\`\`

**CI:** \`.github/workflows/ci.yml\` baut und testet die Solution auf \`windows-latest\`. Der Workflow umfasst zurzeit **nicht** automatisch alle im Pflichtenheft vorgesehenen Architektur-, Paket-, Release- und Performance-Gates. Für einen konkreten Commit dessen GitHub-Actions-Lauf prüfen.

## 4. Testfallkatalog nach Risiko

| Bereich | Mindestfälle | Schwerpunkt |
|---|---|---|
| Buffer / Text | leere Datei, Insert, Delete, Replace, Zeilengrenze, CRLF/LF/CR, kein Final Newline | deterministische Texte und Ranges |
| Positionen / Unicode | UTF-16-Indizes, Surrogatpaare, kombinierte Zeichen, RTL, Emoji, Tabs, längste Zeile | Graphem- und Caret-Grenzen |
| Undo / Dirty | Insert → Undo → Redo, Savepoint, neue Änderung nach Undo, mehrere Ansichten | keine verlorene Änderung |
| Suche | vorwärts/rückwärts, Wrap an/aus, Vergleichsoptionen, kein Treffer, mehrzeilig | korrekte Match-Ranges |
| M1c Replace | kürzere/längere Ersetzung, Auswahl, Unicode, Undo, kein Treffer, wiederholter Replace | noch ausstehender Slice |
| Storage | BOM UTF-8/16/32, Fallback, ungültiges UTF-8, Binärverdacht, Fehler beim Schreiben | kein Datenverlust |
| Datei-Konflikt | externe Änderung vor Save, Rechtefehler, Abbruch, temporäre Datei, konkurrierender Writer | geplante Härtung |
| Commands | unbekannter Command, falscher Parameter, CanExecute=false, Registrierung, Cancel | keine unkontrollierte Aktion |
| Workspace | zwei Views auf einem Dokument, getrennte Carets, Schließen letzter View | noch nicht umfassend abgenommen |
| WinForms | Keyboard, Fokus, Resize, DPI, IME, Mausauswahl, Save/Discard/Cancel | manuelle Windows-Prüfung |
| Sicherheit | beschädigtes JSON, fremde Pfade, Dokumentinhalt als Skript, Logging | kein Execution-/Leak-Pfad |

**Abdeckung zählt nur bei tatsächlich ausgeführten Tests.** Geplante Tests sind keine „PASS“-Markierungen.

## 5. Tests im Stil des Projekts entwickeln

Ein xUnit-Test sollte einen konkreten Vertrag beweisen und keine Implementierungsdetails festzementieren:

\`\`\`csharp
using Sasd.EditorToolkit.Documents;
using Sasd.EditorToolkit.Text;
using Xunit;

public sealed class DocumentMutationTests
{
    [Fact]
    public void Undo_restores_text_after_insert()
    {
        var document = new TextDocument(new LineTextBuffer("A"));
        document.Insert(new TextPosition(0, 1), "B");

        Assert.Equal("AB", document.Buffer.GetText());
        Assert.True(document.Undo());
        Assert.Equal("A", document.Buffer.GetText());
    }
}
\`\`\`

Ergänzend Parameter-/Grenzfalltests verwenden. Datei-I/O in temporären Verzeichnissen prüfen und ausnahmslos im \`finally\` oder via \`IDisposable\` bereinigen. Testdateien dürfen keine privaten Dokumente enthalten. Für einen absichtlich fehlerhaften Stream einen kontrollierten Testdouble verwenden; niemals echte Benutzerdateien beschädigen.

## 6. Manuelle Windows-Abnahme

Pro Testlauf festhalten: **Commit-SHA, Betriebssystemversion, .NET SDK, Auflösung/DPI, Eingabemethode, Testdatei, Ergebnis, erwartetes/ beobachtetes Verhalten**.

| ID | Prüfschritt | Erwartung |
|---|---|---|
| UI-01 | Sample starten, Cursor mit Pfeilen und Home/End bewegen | Position entspricht Core-Text |
| UI-02 | Unicode-Text schreiben, speichern und wieder öffnen | identischer Text bei unterstütztem Encoding |
| UI-03 | Undo/Redo durchführen | kein Verlust von Text |
| UI-04 | Find/Find Next inklusive „nicht gefunden“ | klare Auswahl/Status, kein Absturz |
| UI-05 | Ungespeicherte Änderungen; New/Open/Exit auslösen | Host fragt Save/Discard/Cancel |
| UI-06 | Fenstergröße, Font/DPI, Fokus und IME variieren | kein fehlerhafter Caret oder verloren gegangener Text |
| UI-07 | Binärverdächtige Datei öffnen | **Zielprüfung**, noch keine vollständige Host-Warnfunktion |
| UI-08 | Mehrere Ansichten/Split | **geplante Prüfung**, kein M1-PASS ohne implementierten Hostpfad |

Das Testprotokoll muss zwischen **PASS**, **FAIL**, **BLOCKED** und **NOT RUN** unterscheiden. Ein nicht vorhandenes Feature bekommt **NOT IMPLEMENTED**, kein „FAIL“ für einen bewusst nicht begonnenen späteren Meilenstein.

## 7. Dateiintegrität und Fehlertests

Die kritischste Freigabegrenze ist **kein Verlust von Benutzerdateien**. Vor M3/V1 sind nachzuweisen:

- BOM-Erhalt und bewusstes Verhalten bei Dateien ohne BOM;
- gemischte Zeilenenden und fehlender Endterminator;
- keine stillschweigende Ersetzung nicht unterstützter/binary Inhalte;
- Schutzhinweis bei extern geändertem Zieldokument;
- atomare Speicherung soweit Dateisystem und Laufzeit es garantieren, dokumentierter Fallback;
- Fehler während Write/Flush/Replace ohne versehentlichen Clean State;
- Schutz bei Schließen/Öffnen einer Dirty-Datei;
- Symlink-, Berechtigungs- und Konkurrenzfälle unter Windows und Linux Core.

Ein Test, der nur \`File.Exists\` prüft, reicht für Datenintegrität nicht aus; Inhalt, Encoding, Metadaten und Dirty/Undo-Zustand prüfen.

## 8. Conformance-Tests und weitere Sprachen

\`docs/spec/Conformance-Tests.md\` beschreibt ein geplantes neutrales Vektorformat. Solange kein ausführbares Runner-/Vektorsystem vorliegt, nicht von bestandener Cross-Language-Conformance sprechen.

**Zielvektor:** Starttext und Cursor; Befehl samt Parametern; erwarteter Endtext, Caret, Selection, Dirty, Undo/Redo und Fehlerklasse. Vergleich basiert auf **beobachtbarem Verhalten**, nicht auf identischem .NET-/Java-/C++-Klassendesign. Encoding und Indexierungsmodell der jeweiligen Plattform ausdrücklich angeben.

## 9. Abnahmekriterien und Nachweisführung

Die IDs \`AK-001\` bis \`AK-015\` stammen aus Lasten-/Pflichtenheft. Die Matrix in Kapitel 39 des Pflichtenhefts enthält vorgeschlagene Verfahren und Meilensteine; sie ist noch kein ausgefülltes Abnahmeprotokoll.

**Definition für einen abgeschlossenen Slice:** Anforderung/Spec eindeutig; Code vorhanden; relevante Core-Tests bestanden; Windows-CI bei UI-Änderungen bestanden; Risiken und manuelle Lücken genannt; DE/EN-Dokumentation konsistent; Commit eindeutig.

**Definition für eine Releasefreigabe:** zusätzlich alle zugeordneten Abnahmekriterien, reproduzierbare Pakete, Lizenz-/Security-Prüfung, dokumentierte API-Baseline, Consumer-Smokes und explizite Owner-Freigabe. Eine Dokumentationsänderung erteilt keine Releasefreigabe.

## 10. Vorlage für einen Prüfbericht

\`\`\`text
Repository: Robin-Goerlach/SASD-Editor-Toolkit
Commit: <vollständiger SHA>
Datum/Zeitzone:
Plattform / SDK:
Prüfumfang: Core | vollständige Windows-Solution | UI | Paket | Release
Ausgeführte Befehle:
Automatisierte Tests: bestanden / fehlgeschlagen / übersprungen
Manuelle IDs: UI-01 ... UI-08
Abweichungen / Issues:
Nicht geprüfte Bereiche:
CI-Run-URL:
Ergebnis: PASS | FAIL | BLOCKED (für genau diesen Prüfumfang)
Prüfer / Freigabe:
\`\`\`

Siehe auch [Historische Nachverfolgung](Historical-Traceability.md) und [Entwicklerhandbuch](Entwicklerhandbuch.md). 
