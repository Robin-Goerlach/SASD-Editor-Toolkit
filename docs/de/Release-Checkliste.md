# Release- und Abnahmecheckliste – SASD Editor Toolkit

[English](../en/Release-Checklist.md) · [Roadmap](ROADMAP.md) · [Testhandbuch](Testhandbuch.md) · [Architektur](Architektur.md)

**Status:** Vorausschauendes Prüfverfahren für das **eigenständige** Editor Toolkit, **keine** erteilte Releasefreigabe. Stand: 03.11.2026.

## 1. Freigabeprinzip

Ein lauffähiges FIRST-ED-Sample, ein grüner CI-Lauf oder eine vollständige Dokumentationssammlung allein reichen nicht für eine öffentliche Produktfreigabe. Eine Veröffentlichung erfordert nachweisbare Anforderungen, reproduzierbare Artefakte, sichere Dateioperationen, eine tragfähige API und eine ausdrückliche Entscheidung des Product Owners.

## 2. Verbindliche Gates

| Gate | Geforderter Nachweis | Stand 03.11.2026 |
|---|---|---|
| Umfang | freigegebene M1/M2/M3-Grenze, bekannte Nichtziele | Roadmap vorhanden; V1-Scope nicht vollständig abgenommen |
| Funktion | Tests zu zugeordneten \`AK-001\` bis \`AK-015\` | nicht als vollständig bestanden belegt |
| Datenintegrität | Encoding/BOM, Zeilenenden, Fehler, konfliktfreies Speichern, CloseGuard | Teilabdeckung; Konflikt/Recovery offen |
| Architektur | Core/UI-Grenzen, Command-IDs, keine Zyklen, Mutation/Undo-Vertrag | Entwurf vorhanden; automatische Gates offen |
| Windows | vollständiger Solution-Build und Tests mit genauem Commit-SHA | CI vorhanden; Ergebnis vor Freigabe frisch prüfen |
| Linux/macOS | Core-Restore/Build/Tests | Skript vorhanden; kein vollständiger GUI-Test |
| Bedienbarkeit | UI-Smoke, DPI, Fokus, IME, Tastatur, Accessibility | vollständiger Abnahmenachweis offen |
| Sicherheit | Quell-/Lizenzprüfung, Path-/Input-Review, Logging, keine Makroausführung | Sicherheitsgrundsätze vorhanden |
| Verpackung | NuGet-Pakete, Versionsmetadaten, XML-Dokumentation, Prüfsummen | für M3 geplant |
| Verbraucher | Beispiel-Host mit publizierten Paketen statt Quellprojekt | nicht abgenommen |
| Dokumentation | DE/EN-Lasten-/Pflichtenheft, Architektur, Entwickler-, Test-, Benutzerhandbuch, Traceability | vorhanden; auf aktuelle Releasefeatures abgleichen |
| Entscheidung | nachvollziehbare Freigabe mit SHA, Artefakten und bekannten Risiken | **ausstehend** |

## 3. Vorgehen zur Releasevorbereitung

1. Zielversion und Release-Scope fixieren; jeden bewussten Aufschub dokumentieren.
2. Ausstehende sicherheits- und datenverlustrelevante Befunde beheben.
3. Alle zur Freigabe gehörenden Tests auf einem **exakten Commit** ausführen.
4. Manuelle Windows-Checks auf derselben Revision dokumentieren.
5. NuGet-Pakete herstellen, Metadaten und Lizenzen prüfen, Checksums erfassen.
6. Pakete in einem sauberen externen Consumer-Sample installieren und verwenden.
7. README, DE/EN-Handbücher, Changelog, Einschränkungen und Migrationhinweise abgleichen.
8. Resultate als Freigabeprotokoll dokumentieren; Product Owner entscheidet.
9. Erst nach Freigabe Tag/Release/Publikation erzeugen; Rollback und Supportweg benennen.

## 4. Freigabeprotokoll

\`\`\`text
Produkt: SASD Editor Toolkit
Zielversion:
Commit-SHA:
Release-Artefakte + Prüfsummen:
Windows-CI:
Core Linux/macOS:
Manuelle UI-/Accessibility-Prüfungen:
AK-001 ... AK-015: je PASS | FAIL | NOT RUN | DEFERRED mit Begründung
Offene Fehler / akzeptierte Risiken:
Lizenz-, Marken- und Sicherheitsprüfung:
Dokumentationsrevision:
Product Owner / Freigabeentscheidung:
Datum:
Rollback-/Hotfix-Plan:
\`\`\`

**Wichtig:** Eine neu erstellte Dokumentation oder ein Proposed-ADR bestätigt keinen Produktmeilenstein. Weitere Einzelheiten stehen in [Testhandbuch](Testhandbuch.md) und [Historische Nachverfolgung](Historische-Nachverfolgung.md).
