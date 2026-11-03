# Sicherheitsrichtlinie

## Aktueller Stand

Das Projekt ist noch nicht produktionsreif. Sicherheitsrelevante Entwurfsprobleme können vorerst über GitHub-Issues gemeldet werden, solange noch kein gesonderter privater Meldekanal eingerichtet ist.

## Grundsätze

- Dateipfade und Dokumentinhalte als nicht vertrauenswürdige Eingaben behandeln.
- Texte, Makros, Skripte oder eingebettete Inhalte niemals automatisch ausführen.
- Dokumentinhalte standardmäßig nicht protokollieren.
- Vom Host vorgegebene Regeln für erlaubte Verzeichnisse, Autosave-Speicherorte und temporäre Dateien einhalten.
- Vor der Einführung von Plugins, Makros oder Skriptausführung eine eigene Sicherheits-ADR erstellen.
