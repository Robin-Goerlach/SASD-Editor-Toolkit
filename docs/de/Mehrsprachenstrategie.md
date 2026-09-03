# Mehrsprachenstrategie

## Entscheidung

Das SASD Editor Toolkit startet mit **C#/.NET als Referenzimplementierung**. Weitere Sprachen werden vorbereitet, aber nicht parallel entwickelt.

Die Strategie lautet:

> Erst ein belastbares Produkt, dann Ports. Aber die Spezifikation wird von Anfang an so geschrieben, dass Ports möglich bleiben.

## Warum nicht alles parallel?

Parallelentwicklung in C#, Java, C++, Web und PHP würde sofort mehrere Buildsysteme, Paketmanager, UI-Techniken, Unicode-Modelle und Testsysteme erzwingen. Das würde das überschaubare M1-Ziel gefährden.

## Zielsprachen

| Sprache/Plattform | Geplante Rolle | Status |
|---|---|---|
| C#/.NET | vollständige Referenzimplementierung | Start jetzt |
| Java | möglicher vollständiger Port | nach stabiler C#-API |
| C++ | möglicher nativer Port für Linux/Desktop/Embedding | nach stabiler Spec |
| TypeScript/Web | Adapter/Bridge, wahrscheinlich mit CodeMirror/Monaco oder Blazor | später |
| PHP | begrenzte Integrationsbibliothek, nicht erster vollständiger Editor-Core | später bei Bedarf |
| Rust | Beobachtung, keine Zusage | offen |

## PHP-Bewertung

PHP ist für SASD wegen Webhosting und Serverlogik praktisch, aber für einen interaktiven Editor-Core nicht der erste Kandidat. Ein sinnvoller PHP-Beitrag wäre später:

- Lesen und Schreiben gemeinsamer Settings/Profile;
- Validierung von Command IDs;
- serverseitige Suche/Ersetzung;
- Import/Export für Webhosts;
- Storage-Adapter für SASD-Webanwendungen.

Ein vollständiger PHP-Editor-Core wird erst geprüft, wenn ein konkreter Hostnutzen entsteht.

## API-Strategie

Die APIs sollen nicht sklavisch identisch sein, aber möglichst viel Wissen wiederverwendbar machen.

Sprachübergreifend stabil:

- Begriffe: Document, Buffer, View, Position, Range, Selection, Command, Workspace;
- Command IDs;
- Settings-Struktur;
- Keyboard-Profile;
- beobachtbares Verhalten;
- Conformance-Testvektoren.

Sprachspezifisch idiomatisch:

- Fehlerbehandlung;
- async/threading;
- String- und Speicherverwaltung;
- Package-Konventionen;
- UI-Adapter.

## Unicode und Positionen

C# und Java sind UTF-16-nah. C++ unter Linux wird eher UTF-8-nah erwartet. Deshalb gilt:

- Die C#-Implementierung darf UTF-16-Code-Unit-Positionen exponieren.
- Die sprachneutrale Spezifikation definiert Verhalten, nicht .NET-Interna.
- Eine C++-Implementierung darf UTF-8 intern verwenden, wenn sie dieselben Conformance-Tests besteht.
- Sichtbare Cursorbewegung darf zusammengesetzte Zeichen nicht absichtlich beschädigen.

## Repository-Strategie

Start:

```text
SASD-Editor-Toolkit              # C#/.NET reference implementation + practical specs
```

Später nur bei realem Bedarf:

```text
SASD-Editor-Toolkit-Spec         # falls die Spezifikation eigenständig werden muss
SASD-Editor-Toolkit-Java
SASD-Editor-Toolkit-Cpp
SASD-Editor-Toolkit-Web
SASD-Editor-Toolkit-PHP          # nur Integrations-/Serverpaket, kein UI-Versprechen
```
