# Roadmap – SASD Editor Toolkit

[English](../en/ROADMAP.md) · [Dokumentationsübersicht](README.md)

**Stand: 3. November 2026.** Diese Roadmap beschreibt das **eigenständige SASD Editor Toolkit**, ohne eine geplante Übernahme der SASD Editor Toolbox. Die Meilensteine sind technische Entwicklungsschritte, keine verbindlichen Freigabetermine. Ein implementierter Punkt bedeutet nicht automatisch eine abgeschlossene Abnahme. Siehe [CURRENT-STATE](../ai/CURRENT-STATE.md) und [NEXT-WORK](../ai/NEXT-WORK.md).

## Belegter Entwicklungsstand (keine Produktfreigabe)

- [x] **Dokumentationsgrundlage:** Architektur, Entwickler-, Test- und Benutzerhandbuch, historische Nachverfolgung und Release-Checkliste auf Deutsch und Englisch ergänzt (03.11.2026). **Dies bestätigt keine M2/M3-Funktion oder Freigabe.**

- [x] **M0:** Repository, deutsche Anforderungen, ADRs, Spezifikationen, Agentenregeln und CI-Grundlage vorhanden.
- [x] **M1a:** UI-unabhängiges .NET-Modell, Textbearbeitung, Speicherung, grundlegendes Undo/Redo und Tests vorhanden.
- [x] **M1b, Fundament:** WinForms-Oberfläche und FIRST-ED-Beispiel vorhanden; eine vollständige manuelle Abnahme ist nicht nachgewiesen.
- [x] **M1c, erster Teil:** UI-unabhängige Suche, `Search.Find` und einfaches Find/Find Next im Sample.
- [ ] **M1c, Rest:** `Search.Replace`, auswahlbewusstes Editieren, Zeichnen der Auswahl und portable Konformitätstestvektoren.
- [ ] **M2–M6:** Spätere Entwicklung, nicht als abgeschlossen oder veröffentlicht ausgewiesen.

Alle Pfade wie `docs/spec/` und `docs/ai/` beziehen sich auf das Repository-Root.

## Strategische Leitlinien

1. Zuerst eine funktionsfähige C#/.NET-Referenzimplementierung.
2. Den Core von Anfang an unabhängig von Oberflächen halten.
3. Den ersten Produktstand praktisch nutzbar und testbar machen, ohne übertriebene Komplexität.
4. Sprachneutrales Verhalten in kurzen, konkreten Spezifikationen dokumentieren.
5. Andere Sprachimplementierungen erst nach Stabilisierung des Referenzverhaltens beginnen.

## M0 – Repository und Umfangsbegrenzung

**Ziel:** Ein verständliches, klar eingegrenztes Fundament schaffen.

**Lieferumfang:**
- Repository-Gerüst;
- Lastenheft und Pflichtenheft unter `docs/de/`;
- praktische Spezifikationen unter `docs/spec/`;
- ADRs zu Referenzimplementierung, Repository-Strategie, Umfang und Unicode/Indexierung;
- README mit Screenshot;
- `AGENTS.md` für Mitwirkende und KI-Agenten;
- CI-Grundlage;
- ausdrückliche M1-Umfangsgrenze.

**Abnahmekriterien:**
- Das Repository ist schnell zu klonen und zu verstehen.
- Das gesamte Pflichtenheft wird nicht als ein einziger M1-Auftrag missverstanden.
- Historische Quellen werden referenziert, nicht kopiert.

## M1a – Kern der Textbearbeitung

**Ziel:** Text ohne grafische Oberfläche bearbeiten und durch Tests prüfen.

**Umfang:**
- `TextPosition`, `TextRange` und Zeilenenden;
- `ITextBuffer` und `LineTextBuffer`;
- `TextDocument` mit Metadaten und Änderungszustand;
- Einfügen, Löschen und Ersetzen von Textbereichen;
- Zeilenumbruchbehandlung;
- einfaches Undo/Redo;
- Datei-/Stream-Laden und -Speichern;
- Core-Tests.

**Nicht Bestandteil von M1a:**
- Feinabstimmung der Desktop-Darstellung;
- Syntax-Highlighting und Marker;
- vollständiger Blockmodus;
- Word-Wrap und Neuformatierung;
- Leistungsoptimierung für sehr große Dateien.

**Abnahmekriterien:**
- Core-Tests bestehen.
- Der Core referenziert keine WinForms-/WPF-/ASP.NET-/Browser-Typen.
- Grundlegende Operationen bewahren Unicode-Text und Zeilenenden.

## M1b – Moderne FIRST-ED-WinForms-Demo

**Ziel:** Eine benutzbare Desktop-Demo beweist die Trennung von Core und Adapter.

**Umfang:**
- eigenes WinForms-Control `SasdEditorView`;
- grundlegende Tastatureingabe;
- Öffnen-/Speichern-Dialoge im Host;
- Statuszeile;
- Toolbar-/Menübefehle über Command-IDs, wo sinnvoll;
- Anzeige des Änderungszustands;
- Beispieldaten und bei Verfügbarkeit ein echter Anwendungsscreenshot.

**Abnahmekriterien:**
- Textdateien öffnen, bearbeiten, speichern und schließen.
- Keine Duplizierung des Dokumentmodells im UI.
- WinForms-Adapter bleibt austauschbar.

## M1c – Suche und Auswahl vervollständigen

**Ziel:** Den ersten UI-unabhängigen Such-/Ersetzungs- und Auswahlbearbeitungsschnitt vor M2 abschließen.

**Umfang:**
- bisheriges `Search.Find` als Ausgangspunkt;
- `Search.Replace` über das Core-Command-Modell ergänzen;
- Bearbeiten, Ersetzen und Löschen markierter Texte mit definierten Caret-Regeln;
- ausgewählten Text im WinForms-Adapter deutlicher darstellen;
- nach Stabilisierung sprachneutrale Such-/Ersetzungs-Testvektoren.

**Nicht Bestandteil:** Regex, inkrementelle oder unscharfe Suche, Großdatei-Suchindizes, Mehrfachauswahl, syntaxabhängige Ersetzung.

**Abnahmekriterien:**
- Suchen und einfaches wörtliches Ersetzen funktionieren ohne UI durch Core-Befehle.
- Auswahlbearbeitung besitzt deterministische Core-Tests.
- Die Demo verwendet die Core-Logik, statt eine eigene Suchlogik zu besitzen.
- Command-IDs und portables Verhalten sind unter `docs/spec/` dokumentiert.
- Die vollständige Windows-CI ist grün.

Die detaillierte Reihenfolge steht in [NEXT-WORK](../ai/NEXT-WORK.md).

## M2 – Historische Funktionsbreite

**Ziel:** Wichtige Funktionen eines Editor-Baukastens über den Minimaleditor hinaus.

**Umfang:**
- mehrere Dokumente und Ansichten;
- geteilte und verknüpfte Ansichten eines Dokuments;
- Auswahl- und Blockoperationen;
- verbesserte Suche und Ersetzung;
- Marker;
- Tastaturprofile einschließlich Turbo-/WordStar-kompatiblem Profil;
- Einstellungen und Themes;
- durch den Host austauschbare Status-, Prompt- und Fehlermeldungen.

**Abnahmekriterien:**
- Eine MicroStar-ähnliche Demo benötigt keine Änderung am Core-Modell.
- Command-IDs sind ausreichend stabil für fremde Hosts.
- Das Tastaturprofilformat ist dokumentiert und getestet.

## M3 – Stabilisierung für Version 1.0

**Ziel:** Die .NET-Implementierung als nutzbare Bibliothek veröffentlichen können.

**Umfang:**
- API-Grundstand;
- NuGet-Pakete;
- ausführlichere XML-Kommentare;
- Konformitätstestvektoren;
- Architekturtests;
- einfache Leistungstests;
- Binärdateiwarnungen und Erkennung externer Dateiänderungen;
- optional ein Versuch mit Piece Table oder vergleichbarem Großdateipuffer;
- Verbesserungen der Barrierefreiheit.

**Abnahmekriterien:**
- `Sasd.EditorToolkit.Core` und `Sasd.EditorToolkit.WinForms` lassen sich paketieren.
- Öffentliche APIs sind dokumentiert.
- M1-/M2-Verhalten ist durch Tests und Konformitätsfälle abgesichert.

## M4 – Entscheidung über den Webadapter

**Ziel:** Einen geeigneten technischen Webweg bestimmen, bevor Code geschrieben wird.

**Optionen:**
- Blazor-Adapter mit dem C#-Core, wo geeignet;
- Integration von CodeMirror/Monaco über SASD-Brücken für Befehle, Einstellungen und Speicherung;
- serverseitige Core-Dienste für Validierung, Import/Export und Texttransformation.

PHP ist hier für Integration und Serverdienste gedacht, nicht für die erste vollwertige Editoroberfläche.

## M5 – Entscheidung über Java oder C++

**Ziel:** Die erste vollständige Nicht-.NET-Implementierung anhand des tatsächlichen Bedarfs wählen.

- Java zuerst, wenn plattformübergreifende Desktop-/Servernutzung wichtiger ist.
- C++ zuerst, wenn native Linux-Einbettung über Qt/wxWidgets/GTK wichtiger ist.
- Nicht beide vollständigen Ports zugleich beginnen.

## M6 – Spezialisierte Komponenten

**Mögliche spätere Erweiterungen:**
- Markdown-Editor/-Viewer;
- Logviewer;
- Konfigurationseditor;
- Syntax-Highlighting;
- kombinierter Hex-/Text-Viewer;
- Makromodell mit explizitem Sicherheitskonzept;
- Druckdienst.

## Ausblick auf die Sprachunterstützung

| Sprache/Plattform | Ziel | Zeitpunkt | Hinweise |
|---|---|---|---|
| C#/.NET | Vollständige Referenz | jetzt | Zuerst Core + WinForms, WPF/Web später |
| Java | Mögliche vollständige Implementierung | nach M3 | Swing/JavaFX/SWT später entscheiden |
| C++ | Mögliche native Implementierung | nach M3/M5 | Linux/native, aber aufwendiger |
| TypeScript/Web | Zunächst Adapter/Bridge | nach M3 | Vorhandene Editorengines sinnvoll nutzen |
| PHP | Begrenzte Integrationsbibliothek | nach Webentscheidung | Hosting, Speicherung, Validierung, Transformation |
| Rust | Beobachtungskandidat | keine Zusage | Eventuell späterer portabler Buffer-Core |
