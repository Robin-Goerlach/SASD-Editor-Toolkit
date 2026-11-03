# Änderungsprotokoll

## Noch nicht veröffentlicht

- Deutsche Fassungen von Änderungsprotokoll, Mitwirkungsregeln, Sicherheitsrichtlinie, Ausführungsplänen und GitHub-Kurzbeschreibung ergänzt; die deutsche Codex-Autonomieanleitung auf den vollständigen Umfang der englischen Ausgangsfassung erweitert und zweisprachige Verzeichnisse aktualisiert. Die vollständigen englischen Übersetzungen der Anforderungsdokumente stehen noch aus.


- Das vollständige Änderungsprotokoll, die Mitwirkungs- und Sicherheitsrichtlinien, Regeln für Ausführungspläne sowie die GitHub-Kurzbeschreibung wurden nach `docs/en/` verschoben. Der bisherige Roadmap-Verweis im Root wurde entfernt; die GitHub-Erkennungseinträge liegen weiterhin unter `.github/`. Zugehörige Verweise wurden angepasst, ohne das Editorverhalten zu verändern.
- Die zweisprachige Dokumentationsnavigation wurde neu organisiert; englische und deutsche Roadmaps liegen unter `docs/`. Ein zunächst beibehaltener Root-Roadmap-Verweis wurde später entfernt. Keine Änderung am Produktverhalten.
- Eine Codex-orientierte Infrastruktur für autonome Entwicklung wurde ergänzt: AGENTS-Anweisungen im Root und in Teilverzeichnissen, Regeln für Ausführungspläne, Dokumente zum Entwicklungsstand und zur nächsten Arbeit, zweisprachige Autonomieleitlinien und plattformabhängige Prüfskripte.
- Die Fertigstellung von Suche und Auswahl in M1c wurde in der Roadmap präzisiert; historische Hinweise zum Anhalten auf einem Branch gelten nicht als allgemeiner Entwicklungsstopp.
- Ein UI-unabhängiger Textsuchdienst für M1c wurde ergänzt, mit Vorwärts- und Rückwärtssuche, Umlauf und Vergleichsoptionen.
- Der Befehl `Search.Find` wurde integriert; er aktualisiert Auswahl und Caret über das Core-Befehlsmodell.
- Die Modern-FIRST-ED-WinForms-Demo unterstützt eine einfache Suche und »Weitersuchen«.
- Die Dateispeicherung aus M1a wurde bei BOM-/Präambelerkennung und -erhaltung robuster gemacht.
- Speichertests für UTF-8-BOM, die Reihenfolge beim Prüfen auf UTF-32-BOM, den Verdacht auf Binärinhalt und Metadaten beim Überschreiben wurden ergänzt.
- Der M1-Befehl `Edit.DeleteRight` sowie Tests für den Dispatcher wurden ergänzt.
- Caret-Navigation nach links, rechts, oben und unten sowie zum Zeilenanfang/-ende wurde ergänzt.
- Die WinForms-Tastatureingabe verwendet nun den Command-Dispatcher für Zeichen, Enter, Backspace, Delete, Pfeiltasten, Home, End, Strg+Z und Strg+Y.
- Der Befehlsdispatcher ist über `EditorSurface` und `SasdEditorView` für Hostanwendungen verfügbar.
- Vor Neu, Öffnen und Beenden fragt die Modern-FIRST-ED-Demo bei ungespeicherten Änderungen nach.
- Die XML-Dokumentationserzeugung wurde für Test- und Beispielassemblies deaktiviert, bleibt aber für Produktassemblies aktiviert.

## 0.0.0-repository-seed – 2026-09-03

- Das Repository-Gerüst für die C#/.NET-Referenzimplementierung wurde angelegt.
- Die Projektstruktur für Core, WinForms, Beispielanwendung und Tests wurde angelegt.
- Eine praktische Roadmap, AGENTS-Anweisungen, ADRs und sprachneutrale Spezifikationen wurden ergänzt.
- Das deutsche Lastenheft und Pflichtenheft wurden als Ausgangsdokumente übernommen.
- Ein UI-Konzeptscreenshot wurde in die README aufgenommen.
