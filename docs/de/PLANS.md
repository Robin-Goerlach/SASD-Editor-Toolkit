# PLANS.md – Ausführungspläne für umfangreiche Arbeiten

Dieses Dokument legt fest, wie Codex Änderungen planen soll, die für eine einzelne offensichtliche Bearbeitung zu umfangreich sind.

## Wann ein Ausführungsplan erforderlich ist

Lege unter `docs/notes/execplans/YYYY-MM-DD-<slug>.md` einen fortlaufend gepflegten Plan an, sobald mindestens eine dieser Bedingungen zutrifft:

- Die Aufgabe verändert mehr als ein architektonisches Teilsystem.
- Eine öffentliche Core-API oder ein sprachneutraler Vertrag ändert sich.
- Eine Migration oder Kompatibilitätsumstellung ist erforderlich.
- Voraussichtlich sind mehrere Implementierungs- und Testdurchläufe notwendig.
- Mehrere sinnvolle Entwürfe stehen zur Wahl, deren Vor- und Nachteile dokumentiert werden müssen.
- Die Arbeit umfasst Core, einen Adapter und eine Beispielanwendung.
- Ein Fehler könnte Benutzerdateien beschädigen oder die Persistenzkompatibilität verletzen.

Für eine kleine lokale Fehlerkorrektur, eine einfache Testerweiterung oder eine rein dokumentarische Änderung ist kein Ausführungsplan notwendig.

## Planung ist keine Freigabeanfrage

Codex soll den Plan erstellen oder aktualisieren und anschließend weiterarbeiten. Ein bestehender Plan ist kein Grund, die Entwicklung anzuhalten. Nur wenn eine in `docs/ai/CODEX-AUTONOMY.md` definierte Stoppbedingung vorliegt, ist eine Rückfrage erforderlich.

## Erforderliche Planabschnitte

Jeder Plan muss enthalten:

- **Ziel:** Beobachtbares Ergebnis.
- **Nichtziele:** Bewusst ausgeschlossener Umfang.
- **Relevante Quellen:** Spezifikationen, ADRs, vorhandener Code und Tests.
- **Ausgangsstand:** Bisheriges Verhalten und Prüfergebnisse.
- **Entwurf/Invarianten:** Entscheidungen und Regeln, die erhalten bleiben müssen.
- **Implementierungsschritte:** Geordnete, unabhängig prüfbare Änderungen.
- **Validierung:** Konkrete Befehle/Tests und erwartete Nachweise.
- **Fortschritt:** Laufend aktualisierte Checkliste.
- **Entscheidungsprotokoll:** Wichtige Entscheidungen und ihre Gründe.
- **Risiken/Rollback:** Mögliche Fehler und Wiederherstellungsweg.
- **Abschlusszusammenfassung:** Commits, Tests und verbleibende Arbeiten.

## Qualitätsanforderungen an Pläne

Ein guter Plan nennt nach Möglichkeit konkrete Dateien und Komponenten, schreibt aber keine spekulativen zeilenweisen Änderungen vor, bevor der Code geprüft wurde. Der Plan ist aktuell zu halten: veraltete Schritte entfernen, erledigte Schritte markieren und geänderte Entscheidungen dokumentieren.

Der Plan gehört zum Projektwissen und ist kein Chatprotokoll. Keine internen Gedankengänge, unübersichtlichen Terminalausgaben oder langen Tool-Logs hineinkopieren.
