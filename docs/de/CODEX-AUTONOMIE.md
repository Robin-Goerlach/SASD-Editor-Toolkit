# Codex – Leitfaden für autonome Entwicklung

Dieses Dokument ist die vollständige deutschsprachige Fassung des ausführlichen Agentenvertrags für das SASD Editor Toolkit. Die englische Ausgangsfassung liegt unter [docs/ai/CODEX-AUTONOMY.md](../ai/CODEX-AUTONOMY.md). Das `AGENTS.md` im Root bleibt bewusst kurz; hier stehen die Hintergründe und detaillierten Regeln für die Ausführung.

## 1. Ziel

Codex soll eine kurze Anweisung wie die folgende erhalten können:

> Arbeite am nächsten Meilenstein des SASD Editor Toolkit weiter. Entwickle autonom bis zum nächsten in sich geschlossenen, erfolgreich geprüften Zwischenstand.

Danach soll Codex den Repository-Zustand ermitteln, den nächsten freigegebenen Arbeitspunkt auswählen, implementieren, testen, die Dokumentation nachführen, committen und CI prüfen – ohne immer wieder nach Routineentscheidungen zu fragen.

Autonomie bedeutet **keine** unbegrenzte Befugnis, sondern ein hohes Maß an Selbstständigkeit innerhalb klarer Repository-, Sicherheits- und Produktgrenzen.

## 2. Maßgebliche Quellen im Repository

Die folgende Hierarchie gilt.

### Produkt und Architektur

- `docs/de/ROADMAP.md` definiert die Reihenfolge und den Umfang der Meilensteine.
- `docs/adr/` dokumentiert akzeptierte Architekturentscheidungen.
- `docs/spec/` beschreibt sprachneutral das beobachtbare Verhalten.
- `docs/de/Pflichtenheft.md` und `docs/de/Lastenheft.md` enthalten die umfassenden Anforderungen; sie sind kein Auftrag, den gesamten Umfang auf einmal umzusetzen.

### Ausführung durch Agenten

- `AGENTS.md` enthält die kurzen verbindlichen Regeln.
- Untergeordnete `AGENTS.md`-Dateien enthalten komponentenspezifische Regeln.
- `docs/ai/CURRENT-STATE.md` ist ein datierter Zwischenstand, kein Ersatz für die Git-Prüfung.
- `docs/ai/NEXT-WORK.md` ist die Standard-Arbeitsliste, wenn nur »weiter« angefordert wird.
- `docs/de/PLANS.md` legt fest, wann und wie ein Ausführungsplan zu führen ist.

### Historische Notizen

`docs/notes/` enthält Arbeitsprotokolle und branchbezogene Notizen. Diese sind nützliche Belege, stehen in ihrer Verbindlichkeit aber hinter aktuellen, repositoryweiten Regeln. Insbesondere darf ein alter Hinweis wie »hier mit diesem Branch aufhören« nicht als dauerhafter Projektstopp verstanden werden.

## 3. Innerhalb des Repositories gewährte Autonomie

Bei einer klar abgegrenzten Aufgabe aus der Arbeitsliste darf Codex ohne Rückfrage:

- alle Repository-Dateien und die Git-Historie untersuchen;
- Core, Adapter, Beispiele, Tests, Dokumentation, Spezifikationen und CI-Konfiguration ändern;
- interne Implementierungsdateien hinzufügen oder entfernen, sofern die Architektur konsistent bleibt;
- zur Erledigung erforderliche Refactorings durchführen;
- deterministische Tests und Testdaten ergänzen;
- XML-Dokumentation, deutsch- und englischsprachige Dokumente sowie Änderungsprotokolle aktualisieren;
- Restore-, Build- und Testbefehle ausführen;
- durch die aktuelle Arbeit verursachte Warnungen oder Testfehler beheben;
- bei der Implementierung entdeckte kleine benachbarte Korrektheitsfehler beheben, sofern sie getestet und im Abschlussbericht dokumentiert werden;
- in sich geschlossene grüne Zwischenstände direkt auf `main` committen;
- normale Fast-Forward-Pushes durchführen;
- GitHub Actions untersuchen und durch den eigenen Commit verursachte Fehler korrigieren;
- mit dem nächsten Teilschritt desselben Meilensteins fortfahren, wenn dessen Abnahmekriterien den nächsten Schritt eindeutig festlegen.

Das Projekt geht derzeit von einem einzelnen Entwickler und direkter Arbeit auf `main` aus. Eine spätere Benutzeranweisung kann dies ändern, ohne die Architektur umzuschreiben.

## 4. Aktionen, die ein Anhalten oder ausdrückliche Zustimmung erfordern

In den folgenden Fällen anhalten und nachfragen:

1. **Secrets und Zugangsdaten:** Eine Aufgabe benötigt ein neues Secret, Token, Signaturschlüssel, Zugangsdaten für private Paketquellen oder eine Benutzerkontoaktion.
2. **Nicht rückgängig zu machende externe Wirkung:** Pakete veröffentlichen, öffentliche Releases bzw. Release-Tags erstellen, externe Daten löschen, Abrechnungen ändern, Eigentum übertragen oder fremde Repositories ändern.
3. **Zerstörung der Git-Historie:** Force-Push, Löschen von Remote-Branches mit nicht integriertem Code, Rebase veröffentlichter gemeinsamer Historie oder destruktives Zurücksetzen von Benutzerarbeit.
4. **Änderung der Sicherheitsgrenze:** Frei ausführbare Skripte/Makros, Laden nicht vertrauenswürdiger Plugins im Prozess, Abschwächen der Pfadvalidierung oder neue, noch nicht freigegebene Netzwerkfunktionen.
5. **Rechtliche Unsicherheit:** Die vorgeschlagene Implementierung würde das Kopieren historischen Borland-Quellcodes oder Handbuchmaterials erfordern statt einer unabhängigen Implementierung des Verhaltens.
6. **Produktmehrdeutigkeit mit inkompatiblen öffentlichen Ergebnissen:** Mindestens zwei Optionen führen zu wesentlich unterschiedlichen öffentlichen APIs oder Dateiformaten, ohne dass Spezifikation oder ADR eine Entscheidung treffen.
7. **Nicht behebbare Validierungslücke:** Trotz angemessener Diagnose kann eine riskante Änderung nicht geprüft werden, und es steht auch kein entsprechender CI-Weg zur Verfügung.
8. **Unerwartete fremde lokale Änderungen:** Im Arbeitsverzeichnis liegen Benutzeränderungen, die durch die Aufgabe überschrieben oder vermischt würden.

**Nicht** wegen Benennungen, privater Hilfsmethoden, Testorganisation, Kommentaren, Routine-Refactorings oder anderen durch vorhandene Muster klärbaren Entscheidungen anhalten.

## 5. Standardablauf für autonomes Arbeiten

### Schritt A – Sichere Ausgangslage herstellen

Ausführen:

```bash
git status --short --branch
git pull --ff-only
git log -5 --oneline
```

Wenn das Arbeitsverzeichnis nicht sauber ist, klären, ob die Änderungen zur aktuellen Codex-Sitzung gehören. Unbekannte Benutzerarbeit niemals verwerfen.

Root-Anweisungen, aktuellen Stand, nächste Arbeiten, Roadmap und relevante Spezifikationen/ADRs lesen. Vor einer Änderung im Teilverzeichnis dessen lokale `AGENTS.md` lesen.

### Schritt B – Vor der Änderung verstehen

Die vorhandene Implementierung und die dem gewünschten Verhalten nächstliegenden Tests untersuchen. Vorhandene Command-IDs, Schnittstellen und Muster suchen, statt parallele Abstraktionen neu zu erfinden.

Bestehende Erweiterungsstellen vorzugsweise ausbauen, statt einen nahezu identischen weiteren Dienst einzuführen.

Bei Verhaltensänderungen festhalten:

- bisheriges beobachtbares Verhalten;
- gewünschtes beobachtbares Verhalten;
- betroffene Invarianten;
- Tests, die vor der Korrektur oder Funktion fehlschlagen sollten;
- Änderungen an Dokumentation und Spezifikation.

### Schritt C – Bedarf für einen ExecPlan entscheiden

`docs/de/PLANS.md` verwenden. Falls ein Plan erforderlich ist, ihn erstellen und weiterarbeiten. Der Plan ist ein lebendiges Entwicklungsartefakt, kein Haltepunkt.

### Schritt D – In kleinen, zusammenhängenden Schritten implementieren

Eine leicht überprüfbare, auf Korrektheit ausgerichtete Implementierung bevorzugen.

Für Core-Verhalten:

- Tests parallel zum Verhalten ergänzen oder anpassen;
- Core unabhängig von UI-Frameworks halten;
- Zeilenenden- und Unicode-Regeln erhalten;
- versteckten, veränderlichen globalen Zustand vermeiden;
- Command-/String-Verträge stabil halten oder ihre sprachneutrale Spezifikation bewusst aktualisieren.

Für Adapterverhalten:

- Eingabe und Darstellung des Hosts in Core-Konzepte übertragen;
- Editierzustandsmaschinen nicht in WinForms duplizieren;
- Dialoge und Lebenszyklusentscheidungen gehören in Beispielanwendung/Host, sofern sie keine wiederverwendbare Adapterfunktion sind.

### Schritt E – Lokal prüfen

Unter Linux/macOS:

```bash
./scripts/verify-core.sh
```

Unter Windows:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\verify-all.ps1
```

Bei schrittweiser Entwicklung zuerst einen möglichst engen, aussagekräftigen Test ausführen, abschließend aber die stärkste plattformgerechte Prüfung.

Zusätzlich:

```bash
git diff --check
git status --short
```

### Schritt F – Dokumentation als Teil der Funktion aktualisieren

Bei Änderungen des beobachtbaren Verhaltens die betreffende Datei unter `docs/spec/` aktualisieren. Bei Architekturänderungen eine ADR hinzufügen oder überarbeiten. Deutsche und englische erläuternde Dokumentation nach Möglichkeit parallel pflegen. Wesentliche Änderungen mit einem kurzen Eintrag unter `docs/en/CHANGELOG.md` dokumentieren; die deutsche Fassung liegt unter `docs/de/CHANGELOG.md`.

Kommentare im Code erklären Invarianten und das Warum. Sie ersetzen weder Spezifikationen noch Tests.

### Schritt G – Commit und Prüfung

Einen fokussierten Commit mit konventioneller Commit-Nachricht verwenden. Einen veröffentlichten Commit nicht nachträglich ändern.

Nach dem Commit:

```bash
git status --short --branch
git log -1 --oneline
```

Wenn Netzwerkzugang und Berechtigungen vorhanden sind, pushen und den zugehörigen GitHub-Actions-Lauf prüfen. Falls CI wegen des Commits fehlschlägt: Ursache analysieren, korrigieren, erneut prüfen, Korrektur committen und CI nochmals kontrollieren.

Eine Code-Aufgabe nicht als abgeschlossen melden, solange die aktuelle CI rot ist.

## 6. Plattformabhängige Validierung

Die vollständige Solution enthält WinForms-Projekte mit Ziel `net10.0-windows`.

### Linux-/macOS-Agent

Der verlässliche lokale Prüfvertrag umfasst Core und Core-Tests. `scripts/verify-core.sh` verwenden. Ein Agent darf nach sorgfältiger Prüfung auch WinForms-Code ändern, muss aber offenlegen, dass die vollständige WinForms-Kompilierung von Windows-CI bestätigt wurde; ein Linux-Core-Build allein reicht dafür nicht.

### Windows-Agent

`scripts/verify-all.ps1` verwenden. Das Skript führt Restore, Build und Tests der gesamten Solution aus.

### GitHub Actions

Bei lokaler Entwicklung unter Linux sind Windows-GitHub-Actions die maßgebliche vollständige Solution-Prüfung. Ein grüner lokaler Core-Test bei gleichzeitig roter Windows-CI bedeutet **keine** abgeschlossene Aufgabe.

## 7. Umgang mit Fehlern

Fehler als Information behandeln, nicht als automatische Rückgabe der Arbeit an den Benutzer.

Fehler klassifizieren:

- **Produktregression:** Code, Tests und Dokumentation korrigieren.
- **Veraltete Testerwartung:** Erst nach Bestätigung des beabsichtigten Verhaltens anhand der Spezifikation aktualisieren.
- **Plattform-/Toolchain-Problem:** Genaue Einschränkung dokumentieren und einen anderen unterstützten Prüfweg nutzen.
- **Netzwerk-/temporärer Restore-Fehler:** In begrenztem Umfang erneut versuchen.
- **Vorhandener Fehler:** Vor dem Weiterarbeiten belegen, dass der Fehler schon zuvor bestand; nicht kaschieren.
- **Neuer Architekturkonflikt:** ADR-/Spezifikationshierarchie konsultieren und bei größerer Entscheidung eine ADR erstellen.

Fehlschlagende Tests niemals durch Abschwächung der Assertions »reparieren«, ohne zuvor eine Anforderungsänderung nachzuweisen.

## 8. Abhängigkeitsrichtlinie

BCL und bestehende Pakete bevorzugen. Neue NuGet-Abhängigkeiten benötigen eine nachvollziehbare Begründung und sollen keine triviale Funktionalität duplizieren.

Vor einer neuen Abhängigkeit Wartung und Aktivität, Lizenzkompatibilität, Paketgröße und transitive Abhängigkeiten, Architekturzuordnung und Kosten für spätere Nicht-.NET-Portierungen prüfen.

Für dieses Toolkit ist eine kleine Eigenimplementierung häufig einer großen Framework-Abhängigkeit vorzuziehen.

## 9. Richtlinie für Sicherheit und Dateischutz

Editorcode verarbeitet Benutzerdateien. Daher haben Risiken von Datenverlust hohe Priorität.

- Encoding-, BOM- und Zeilenendemetadaten gemäß den aktuellen Speicherverträgen erhalten.
- Dateien nicht allein zum Testen des Speicherns überschreiben; Tests verwenden temporäre Verzeichnisse.
- Schutz bei ungespeicherten Änderungen in Host-Abläufen erhalten.
- Bei binär wirkendem Inhalt oder beschädigten Einstellungen konservativ handeln.
- Dokumentinhalte niemals ausführen.
- Keine freie Makro-, Skript- oder Plugin-Ausführung ohne Sicherheits-ADR und ausdrücklichen Auftrag einführen.

## 10. Historische Kompatibilität und Clean-Room-Prinzip

Die Turbo Editor Toolbox darf als Referenz für Verhalten und Terminologie dienen; die Implementierung muss unabhängig geschrieben werden.

**Erlaubt:** Verhalten in eigenen Worten dokumentieren, beobachtete Konzepte in moderner C#-Architektur implementieren und kurze historische Namen zur Nachverfolgbarkeit in der Dokumentation verwenden.

**Nicht erlaubt:** Historischen Pascal-Quellcode kopieren, lange Handbuchabschnitte in Kommentare oder Dokumente übernehmen, Handbuchscans, Umschläge und Logos einchecken oder eine offizielle Zugehörigkeit behaupten.

## 11. Standards für Dokumentation und Kommentare

Gute Kommentare erläutern eine leicht verletzbare Invariante, den Grund für einen ungewöhnlichen Zweig, eine Kompatibilitätsentscheidung, eine Unicode-/Zeilenendengrenze oder die bewusste Wahl einer einfachen Lösung vor späterer Optimierung.

Keine Kommentare, die lediglich offensichtliche Syntax wiedergeben.

Öffentliche APIs benötigen nützliche XML-Dokumentation: Verhalten, Parameter, nicht offensichtliche Ausnahmen und gegebenenfalls Veränderbarkeit sowie Eigentums-/Lebensdauerregeln.

## 12. Definition of Done

Ein Arbeitspunkt ist erst abgeschlossen, wenn alle jeweils zutreffenden Bedingungen erfüllt sind:

- Das Verhalten erfüllt die Abnahmekriterien von Aufgabe und Arbeitsliste.
- Tests decken Erfolg und wichtige Grenzfälle ab.
- Die Architekturgrenzen des Core bleiben intakt.
- Öffentlich sichtbares Verhalten ist in Spezifikationen dokumentiert.
- Deutsche und englische Dokumentation ist dort aktualisiert, wo erforderlich.
- Wesentliche Verhaltensänderungen stehen im Änderungsprotokoll.
- Die stärkste verfügbare lokale Prüfung ist erfolgreich.
- Windows-CI ist bei Änderungen an kompilierbarem Produktcode grün.
- Keine generierten Binärdateien oder temporären Dateien wurden eingecheckt.
- Der Commit ist inhaltlich fokussiert, das Arbeitsverzeichnis sauber.
- `docs/ai/NEXT-WORK.md` ist aktualisiert, falls der Arbeitspunkt aus dieser Liste stammt.

## 13. Nützlicher Stil für Arbeitsaufträge

Mit dieser Infrastruktur genügt eine kurze Fortsetzungsanweisung:

> Arbeite autonom am ersten noch offenen Punkt in docs/ai/NEXT-WORK.md weiter. Befolge AGENTS.md, halte den Core UI-unabhängig, ergänze Tests sowie deutsche und englische Dokumentation, committe grüne Zwischenstände direkt nach main und halte nur bei einer dokumentierten Stoppbedingung an.

Für konkrete Funktionen genügt eine Beschreibung ähnlich einem GitHub-Issue: gewünschtes Verhalten, betroffene Komponente und Abnahmekriterien. Codex soll trotzdem zuerst das Repository untersuchen, bevor Implementierungsdetails gewählt werden.

## 14. Lokale Codex-Konfiguration

Repositoryanweisungen erfordern keine gefährliche globale Codex-Konfiguration. Ein ausgewogener Ansatz nutzt eine beschreibbare Repository-Sandbox und fragt nur dann nach Genehmigungen, wenn Codex deren Grenzen überschreiten muss.

Aktuelle OpenAI-Codex-Versionen unterstützen `approval_policy = "untrusted"` nicht mehr. Rechnerspezifische Konfiguration gehört in die persönliche `~/.codex/config.toml`, nicht in dieses Repository.

Wo verfügbar, können automatische Prüfung und Genehmigung risikoarmer Sandbox-Anfragen Unterbrechungen verringern und gleichzeitig eine Grenze für destruktive oder externe Aktionen erhalten.
