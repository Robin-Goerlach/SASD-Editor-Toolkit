# Codex - autonomes Arbeiten am SASD Editor Toolkit

Dieses Dokument ist die deutsche Erläuterung des ausführlichen Agentenvertrags in `docs/ai/CODEX-AUTONOMY.md`. Für die Detailregeln gilt die englische Datei zusammen mit `AGENTS.md`.

## Ziel

Codex soll nach einer kurzen Anweisung wie „Arbeite autonom am nächsten offenen Punkt weiter“ selbständig den Stand prüfen, den nächsten freigegebenen Arbeitspunkt auswählen, Code und Tests erstellen, Dokumentation nachziehen, einen sauberen Commit erzeugen und CI kontrollieren können.

Autonomie bedeutet nicht unbegrenzte Rechte. Der Arbeitsraum ist das Repository; riskante externe oder irreversible Aktionen bleiben Zustimmungspunkte.

## Quellen der Wahrheit

Reihenfolge: aktuelle Benutzeranweisung; `AGENTS.md` und lokale Agentenregeln; ADRs und `docs/spec/`; `ROADMAP.md`; `docs/ai/CURRENT-STATE.md` und `docs/ai/NEXT-WORK.md`; ältere Arbeitsnotizen.

Die Lasten-/Pflichtenhefte sind Anforderungen, aber kein Auftrag, den gesamten Umfang in einem Milestone umzusetzen.

## Was Codex ohne Rückfrage tun darf

Codex darf innerhalb einer klaren Aufgabe Code lesen/ändern, refaktorieren, Tests ergänzen, Dokumentation aktualisieren, Build/Test ausführen, durch die eigene Änderung verursachte Fehler beheben, direkt auf `main` committen und normale Fast-Forward-Pushes ausführen. Bei einem bloßen „weiter“ wird der erste offene Punkt aus `docs/ai/NEXT-WORK.md` gewählt.

Routineentscheidungen wie private Helfernamen, Teststruktur oder kleine interne Refactorings sind keine Rückfragegründe.

## Wann Codex anhalten muss

Rückfrage bei Secrets/Zugangsdaten, Veröffentlichungen oder irreversiblen externen Aktionen, History-Rewrites/Force-Push, neuer Sicherheitsgrenze (z. B. frei ausführbare Makros), rechtlich fragwürdiger Übernahme historischer Quellen, mehreren inkompatiblen öffentlichen API-/Dateiformatvarianten ohne ADR/Spec-Entscheidung sowie unbekannten lokalen Benutzeränderungen, die überschrieben würden.

## Direkte Arbeit auf main

Für dieses Single-Developer-Repository ist direkte Arbeit auf `main` derzeit Standard:

```bash
git status --short --branch
git pull --ff-only
```

Nicht erlaubt sind Force-Push, Umschreiben veröffentlichter Commits oder Wegwerfen fremder lokaler Änderungen.

## Prüfen

Linux/macOS:

```bash
./scripts/verify-core.sh
```

Windows:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\verify-all.ps1
```

Da WinForms `net10.0-windows` nutzt, ist bei Linux-Arbeit die Windows-GitHub-Action die verbindliche Vollprüfung.

## Codequalität und Dokumentation

Core bleibt UI-unabhängig; WinForms ist Adapter; das Sample ist Host/Demo. Öffentliche APIs bekommen XML-Dokumentation. Kommentare erklären Absicht, Invarianten und ungewöhnliche Entscheidungen, nicht offensichtliche Syntax.

Wir optimieren nicht aggressiv. Korrektheit, Tests, gute Grenzen und verständlicher Code sind wichtiger. Ändert sich beobachtbares Verhalten, wird `docs/spec/` aktualisiert; Architekturentscheidungen gehören in ADRs; größere Änderungen pflegen englische und deutsche Dokumentation parallel; relevante Änderungen kommen in `CHANGELOG.md`.

## Abschluss

Eine Aufgabe ist erst fertig, wenn passende Tests grün sind, Dokumentation stimmt, `git diff --check` sauber ist, ein fokussierter Commit existiert und bei Produktcode die Windows-CI grün ist. War die Aufgabe Teil der autonomen Queue, wird `docs/ai/NEXT-WORK.md` aktualisiert.
