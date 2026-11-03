# Requirements Specification (Lastenheft)
## SASD Editor Toolkit — Modern Reinterpretation of the Borland Turbo Editor Toolbox

| Attribute | Value |
|---|---|
| **Document type** | Requirements specification / functional and quality-related product requirements |
| **Project** | SASD Editor Toolkit |
| **Document version** | 0.1 |
| **Status** | Translation of the German working draft for review and approval |
| **Date** | 2026-07-24 |
| **Language** | English (translation of the German source document) |
| **Intended project language** | English for code and public APIs; German and English for documentation |
| **First reference UI** | WinForms |
| **Future UIs** | WPF and modern ASP.NET/web UIs, preferably Blazor or Razor |
| **Repository** | Independent repository |
| **Product boundary** | Independent of SASD Desktop Components, SASD Data Toolbox, SASD Numerics, SASD Graphics and SASD GameWorks |

> **Purpose:** This requirements specification primarily defines **what** the product must do and the quality attributes it must meet. Concrete classes, projects, algorithms, UI technologies and implementation details belong in the subsequent implementation specification (Pflichtenheft) and architecture document. Structural decisions already agreed upon—especially the UI-independent core and independent repository—are nevertheless included as product requirements.

---

## 1. Purpose of the document

This document defines the functional, qualitative and organizational requirements for a modern C#/.NET reinterpretation of the **Turbo Editor Toolbox Version 1.0** from 1985.

The historical toolbox was neither simply a text field nor a finished editor. It was a modular construction kit for creating both simple and sophisticated editors and text-editing functions inside other applications. Its handbook covers, among other things:

- characters, words, lines, text streams, windows, blocks and files;
- several text-buffer structures;
- caret, deletion, text-processing, block, window and file commands;
- configurable command processing and keyboard handling;
- multiple windows and shared text streams;
- search, replacement, markers, word wrap, automatic indentation, margins and undo;
- customizable status messages, errors, menus and pop-ups;
- cooperative background work;
- embedding the editor in other programs;
- a technical reference containing constants, data types, global variables, procedures and functions.

This document turns those capabilities into a modern product goal. Historical DOS, Turbo Pascal, memory, overlay and video-memory techniques are not to be ported literally. Their functional intent is mapped to contemporary .NET, desktop and web concepts.

---

## 2. Source material and methodology

### 2.1 Primary sources

The analysis is based on two provided PDF files:

1. `Turbo_Editor_Toolbox_Version_1.0_1985(1).pdf`
2. `Borland_Turbo_Editor_Toolbox_Version_1.0_1985(2).pdf`

Review showed that both contain the same 264-page **Turbo Editor Toolbox Version 1.0 — Owner's Handbook**, copyright 1985 Borland International Inc. The second copy was used as a cross-check; it is not a different specification.

### 2.2 Additional project decisions

The following previously agreed decisions were also considered:

- independent reimplementation in C#/.NET;
- WinForms as the first reference UI;
- UI-independent editor core;
- later WPF and web integration without reimplementing the core;
- preferred web integration via Blazor or Razor;
- independent `SASD-Editor-Toolkit` repository;
- use by SASD Desktop Components only through a dependency/reference;
- clear separation from Data Toolbox, Numerics, Graphics and GameWorks;
- no uncritical one-to-one adoption of historical technical limits;
- comprehensive documentation, well-commented code and automated tests.

### 2.3 Origin classification

- **Original:** directly derived from the handbook.
- **Modernization:** contemporary interpretation of a historical capability.
- **Project decision:** goal expressly agreed for this project.
- **SASD Development Standard:** organizational or qualitative rule.
- **Derived:** necessary consequence for reliable modern implementation.

### 2.4 Scope and completeness boundary

Advertising pages at the end of the handbook for Turbo Graphix Toolbox, Turbo Database Toolbox and Turbo GameWorks are not functional specifications of the Editor Toolbox and do not become editor requirements. Other SASD product lines are mentioned solely to define organizational boundaries.

---

## 3. Initial situation

The historical Turbo Editor Toolbox addressed a problem that remains relevant: applications frequently need text editing but do not want to develop a complete editor themselves or tie themselves permanently to a particular editor application.

Current SASD applications especially need:

- a reusable WinForms editor control;
- a functional core for later WPF and web adapters;
- one command model for menus, keyboard, toolbars and APIs;
- reliable document, file, undo/redo and search behavior;
- multiple views of one or several documents;
- extensibility for Markdown, log, configuration, source-code and specialized editors;
- integration into Desktop Components and further SASD applications;
- educational and documentation value from modernizing a historical construction kit.

A quick wrapper around `RichTextBox` could serve as a short-term prototype, but uncontrolled WinForms coupling would make future reuse difficult. The product must therefore distinguish the functional editor state from platform-specific presentation from the outset.

---

## 4. Product vision

**SASD Editor Toolkit** should become an open, understandable, well-documented .NET construction kit for text editing.

It should:

1. preserve the essential functional capabilities of the Turbo Editor Toolbox;
2. consistently modernize outdated technical restrictions;
3. be embeddable as a library in other applications;
4. begin with WinForms without being limited to WinForms;
5. enable simple and more advanced editors using the same core building blocks;
6. serve as a foundation for further SASD Desktop Components;
7. offer a dependable API, tests, documentation and example applications;
8. grow incrementally without unnecessary framework overengineering during its first milestone.

---

## 5. Goals

### 5.1 Main goals

- UI-independent, testable editor core;
- WinForms component suitable for productive use;
- modern coverage of the historical Editor Toolbox feature categories;
- Unicode, file processing, undo/redo and multiple-view support;
- freely configurable commands and keyboard profiles;
- easy embedding in SASD and third-party applications;
- dependable basis for WPF and web;
- independent versioning and release capability.

### 5.2 Quality goals

- high comprehensibility and maintainability;
- safe treatment of unsaved data;
- responsive interaction;
- automated tests;
- clear API and architectural boundaries;
- localizable UI and messages;
- no hidden reliance on historical hardware or memory assumptions.

### 5.3 Education and documentation goals

- traceability showing how each historical function was modernized;
- documented reasons for deliberate omissions;
- examples inspired by FIRST-ED and MicroStar;
- use as a reference project for the SASD Development Standard.

---

## 6. Non-goals

The first product cycle does not require:

- full word processing with page layout, tables and images;
- WYSIWYG document formats such as Microsoft Word;
- real-time collaborative editing;
- a complete IDE with compiler and debugger;
- a mature plugin ecosystem in the first milestone;
- complete syntax highlighting in the first milestone;
- one-to-one Turbo Pascal source compatibility;
- direct use of historical `.ED`, `.MS`, `.OVL` or `.INC` modules;
- DOS overlays, chain files, BIOS calls or video-memory access;
- the historical 80×25 UI as the primary interface;
- mandatory dependencies on other SASD toolboxes;
- a first-class ASPX/Web Forms control in the initial release.

An optional retro demonstration mode may showcase historical interaction patterns but must not dictate the modern architecture.

---

## 7. Stakeholders

| Stakeholder | Interest |
|---|---|
| **SASD GmbH / Product Owner** | Reusable, maintainable editor product with a long lifetime |
| **SASD developers** | Understandable API, comments, tests and easy extensibility |
| **WinForms application developers** | Quickly embeddable control with file, search, undo and multiple-view functions |
| **Future WPF/web developers** | Reuse of the same core |
| **End users** | Dependable, fast and clear text editing |
| **Maintainers** | Clear modules, versioned APIs and low technical debt |
| **Open-source users** | Transparent licensing, examples and stable releases |
| **SASD Desktop Components** | Consume and demonstrate the editor without owning the core |

---

## 8. Product boundary and repository strategy

### 8.1 Independent product

SASD Editor Toolkit is maintained in an independent repository with its own:

- versioning;
- releases;
- documentation;
- tests;
- roadmap;
- packages;
- example applications.

### 8.2 Relationship to SASD Desktop Components

`SASD Desktop Components` may:

- reference the WinForms adapter;
- offer a showcase page or example application;
- build specialized components such as Markdown editors or log viewers on top of it.

It must **not**:

- duplicate the editor core;
- move core classes into the components project;
- create cyclic dependencies.

### 8.3 Relationship to other toolboxes

- **SASD Data Toolbox:** independent data, indexing and storage product; no mandatory dependency.
- **SASD Numerics:** independent mathematics library or part of a building-blocks lab.
- **SASD Graphics:** independent graphics library or adapter family.
- **SASD GameWorks:** potential user of Numerics and Graphics; may consume editor functions.
- **SASD Building Blocks:** possible product family, not one monolithic DLL.

---

## 9. System context

```text
+-------------------------------------------------------------+
| Host applications                                           |
| SASD Desktop Components, Mail Workbench, tools, samples ... |
+--------------------------+----------------------------------+
                           |
                           v
+-------------------------------------------------------------+
| UI adapters                                                 |
| WinForms | later WPF | later Blazor/Razor | host adapters    |
+--------------------------+----------------------------------+
                           |
                           v
+-------------------------------------------------------------+
| Editor application services                                 |
| Commands, sessions, search, file workflows, settings        |
+--------------------------+----------------------------------+
                           |
                           v
+-------------------------------------------------------------+
| UI-independent editor core                                  |
| Document, text buffer, positions, selection, undo, markers  |
+--------------------------+----------------------------------+
                           |
                           v
+-------------------------------------------------------------+
| Replaceable platform and infrastructure services            |
| Files/streams, clipboard, logging, dialogs, print, autosave |
+-------------------------------------------------------------+
```

The implementation specification determines the final assembly and interface count. Avoid splitting the product into unnecessary tiny projects.

---

## 10. Terminology

| Term | Meaning in this project |
|---|---|
| **Editor** | Core, commands and UI for viewing and changing text |
| **Document** | Functional text content, file metadata and dirty state |
| **Text buffer** | Internal replaceable structure for storing and changing text |
| **Text stream** | Historical term for a contiguous sequence of lines |
| **View** | Presentation of a document with its own caret, selection and scroll state |
| **Window** | Historical term; today usually a view, pane or editor control |
| **Cursor / caret** | Current insertion or editing position |
| **Selection** | Marked text range, including the historical concept of a block |
| **Block** | Historically mostly a contiguous sequence of lines |
| **Marker** | Named or numbered document position updated as edits occur |
| **Command** | Uniquely identifiable user or API action |
| **Command dispatcher** | Maps input or command IDs to executable commands |
| **Command processor** | Historical term for command implementation |
| **Dirty state** | Current content differs from the most recently saved state |
| **Word wrap** | Wrapped display or historical paragraph-reflow function |
| **Auto-indent** | Carrying indentation onto a new line |
| **Adapter** | Platform-specific implementation of presentation, input or services |
| **Host** | Application into which the editor is embedded |

---

## 11. Product requirements

| ID | Requirement | Priority | Origin / evidence |
|---|---|---|---|
| PROD-001 | The product must be supplied as a reusable editor construction kit, not only as a finished application. | MUST | Handbook introduction and ch. 14 |
| PROD-002 | The toolkit must support simple editors, sophisticated editors and embedded text editing in business applications. | MUST | Handbook introduction; FIRST-ED and MicroStar |
| PROD-003 | The functional core must be independent of WinForms, WPF, ASP.NET and browser technologies. | MUST | Binding project decision |
| PROD-004 | WinForms is the first production-capable reference interface. | MUST | Binding project decision |
| PROD-005 | WPF must be able to reuse the same core later without reimplementing behavior. | MUST | Binding project decision |
| PROD-006 | The core must be usable from ASP.NET-based hosts; modern interactive web integration should preferably use Blazor or Razor. | SHOULD | Project decision; ASPX/Web Forms only as compatibility route |
| PROD-007 | SASD Editor Toolkit is maintained as its own repository, package and release unit. | MUST | Binding project decision |
| PROD-008 | SASD Desktop Components may use and demonstrate the editor but must not own the editor core. | MUST | Binding project decision |
| PROD-009 | Editor Toolkit must remain independently versionable from Data Toolbox, Numerics, Graphics and GameWorks. | MUST | Product-family decision |
| PROD-010 | Modernize historical behavior functionally; do not port DOS, Turbo Pascal, overlays or video-memory mechanisms one-to-one. | MUST | Handbook and modernization decision |
| PROD-011 | Code, public APIs and package names should be English; documentation should be available in German and English. | SHOULD | SASD project standard |
| PROD-012 | Libraries, UI components and runnable samples must be provided together. | MUST | Handbook examples and modern product requirement |

---

## 12. User groups and main use cases

### 12.1 User groups

1. **End user of an embedded editor:** edits configuration, notes, source code, logs, templates or other text.
2. **Application developer:** integrates the toolkit, registers commands and supplies host services.
3. **Component developer:** creates WinForms, WPF or web adapters and specialized components.
4. **Maintainer and tester:** checks API stability, performance, data integrity and historical coverage.

### 12.2 Use cases

| ID | Use case | Short description |
|---|---|---|
| UC-01 | New document | Create an empty document, enter text, save it |
| UC-02 | Edit file | Open, modify and safely save a file |
| UC-03 | Embed editor | Add WinForms control to a business application and provide host services |
| UC-04 | Compare files | Use multiple document views simultaneously |
| UC-05 | Split same document | Display different positions of one document in two views |
| UC-06 | Edit block | Copy, move or delete a selection |
| UC-07 | Find and replace | Find matches, navigate and replace individually or in bulk |
| UC-08 | Undo/redo | Undo and redo multiple changes |
| UC-09 | Historical key profile | Enable Turbo/WordStar prefix commands |
| UC-10 | Register a custom command | Host registers application-specific command for menu, toolbar and key |
| UC-11 | Protect unsaved changes | Closing/opening requires Save/Discard/Cancel |
| UC-12 | Large file | Open a large file responsively and allow cancellation |
| UC-13 | Autosave | Host stores recovery checkpoints in the background |
| UC-14 | Create another UI adapter | WPF or web adapter uses same core |
| UC-15 | Desktop Components showcase | Components project demonstrates editor, Markdown or log viewer |

---

## 13. Functional requirements

### 13.1 Document, text and buffer model

| ID | Requirement | Priority | Origin / evidence |
|---|---|---|---|
| CORE-001 | Model text as characters, words, lines, streams, views, blocks/selections and files. | MUST | Introduction and ch. 1 |
| CORE-002 | Keep document content separate from view state. | MUST | Modernization of stream and window descriptors |
| CORE-003 | Allow several views to display and edit one document simultaneously. | MUST | Window linking, chs. 9 and 12 |
| CORE-004 | Document supports text, optional path, display name, encoding, line endings, dirty state, markers and undo/redo. | MUST | Original plus modernization |
| CORE-005 | View holds caret, selection, scroll position, insert/overwrite, word wrap, auto-indent, margins and tabs. | MUST | Windesc, ch. 9 |
| CORE-006 | Encapsulate text buffer behind a replaceable abstraction. | MUST | Ch. 3: array of lines, fixed buffer, linked list |
| CORE-007 | First implementation may be line-oriented but should allow later large-file engines. | SHOULD | Project decision |
| CORE-008 | Lines must not have a 255-character limit. | MUST | Modernize Maxlinelength |
| CORE-009 | Internal text representation must support Unicode. | MUST | Modernize ASCII baseline |
| CORE-010 | Detect, preserve and convert CRLF, LF and other supported line endings. | MUST | Modernize historical file handling |
| CORE-011 | Support empty and unnamed documents. | MUST | Nofile/NONAME |
| CORE-012 | Represent changes as atomic and traceable operations. | MUST | Undo/redo, tests, extensibility |
| CORE-013 | Core must be testable without a UI thread or UI framework. | MUST | Project decision |
| CORE-014 | Expose document/non-document editing modes as configurable options. | SHOULD | Ch. 1 |
| CORE-015 | Preserve control characters and optionally display them visibly. | SHOULD | Ch. 1; Insert Control Character |
| CORE-016 | Lines/ranges may carry extensible annotations for wrap, selection, marking and specialized display. | SHOULD | Inblock, Wrapped, Colored |
| CORE-017 | Positions/ranges must be unambiguous, validatable and deterministic under edits. | MUST | Derived from caret, block, markers, search |

---

### 13.2 Caret, navigation and scrolling

| ID | Requirement | Priority | Origin / evidence |
|---|---|---|---|
| NAV-001 | Move caret one character left or right. | MUST | EditLeftChar/EditRightChar |
| NAV-002 | Move caret one line up or down. | MUST | EditUpLine/EditDownLine |
| NAV-003 | Scroll view one line up or down. | MUST | EditScrollUp/EditScrollDown |
| NAV-004 | Navigate one page up or down. | MUST | EditUpPage/EditDownPage |
| NAV-005 | Navigate one word left or right. | MUST | EditLeftWord/EditRightWord |
| NAV-006 | Jump to line start/end; optionally toggle between these positions. | MUST | EditBeginningLine, EditEndLine, EditBeginningEndLine |
| NAV-007 | Jump to document start/end. | MUST | EditWindowTopFile/EditWindowBottomFile |
| NAV-008 | Jump to start/end of selection or block. | MUST | EditTopBlock/EditBottomBlock |
| NAV-009 | Jump to a specified line or column. | MUST | EditGotoLine/EditGotoColumn and Cp variants |
| NAV-010 | Support horizontal scrolling for wide lines. | MUST | Leftedge/EditHscroll |
| NAV-011 | Navigation should optionally extend a selection. | SHOULD | Modern extension |
| NAV-012 | Retain desired visual column during vertical caret movement where possible. | SHOULD | Modern UX requirement |

---

### 13.3 Text input and deletion

| ID | Requirement | Priority | Origin / evidence |
|---|---|---|---|
| EDIT-001 | Normal typing must insert text in insert mode and replace text in overwrite mode. | MUST | EditPrctxt/Insertflag |
| EDIT-002 | Allow switching between insert and overwrite modes. | MUST | EditToggleInsert |
| EDIT-003 | Support inserting new and empty lines. | MUST | EditNewLine/EditInsertLine |
| EDIT-004 | Delete characters at/right of the caret and to the left of it. | MUST | EditDeleteRightChar/EditDeleteLeftChar |
| EDIT-005 | Delete the current line, next word and text to the end of the line. | MUST | EditDeleteLine/EditDeleteRightWord/EditDeleteTextRight |
| EDIT-006 | Deleting the entire document should require an explicitly confirmable action. | SHOULD | EditWindowDeleteText with MicroStar safety adjustment |
| EDIT-007 | Allow lines to be split, joined, shortened, extended, compressed and shifted as required by internal or public commands. | MUST | EditJoinLine, EditShortLine, EditLongLine, EditCompressLine, EditShiftLine |
| EDIT-008 | Change the letter case of a character or selected text. | MUST | EditChangeCase, extended for modern use |
| EDIT-009 | Permit explicit insertion of control characters. | SHOULD | EditInsertCtrlChar |
| EDIT-010 | Support cut, copy and paste using the platform clipboard. | MUST | Modern block-operation mapping |
| EDIT-011 | Correctly process multiline Unicode clipboard text. | MUST | Modernization |
| EDIT-012 | Editing operations must be atomic and able to check preconditions before execution. | MUST | Quality requirement |

---

### 13.4 Text processing, word wrap, margins and tabs

| ID | Requirement | Priority | Origin / evidence |
|---|---|---|---|
| WP-001 | Enable or disable word wrap per view. | MUST | EditToggleWordwrap/WW |
| WP-002 | Word wrap and paragraph formatting must respect left and right margins. | MUST | Lmargin/Rmargin, EditPrctxt, EditReformat |
| WP-003 | Support paragraph reformatting. | MUST | EditReformat |
| WP-004 | Auto-indent must be configurable per view and carry over the previous line's indentation. | MUST | EditToggleAutoindent/AI |
| WP-005 | Allow setting the left and right margins. | MUST | EditSetLeftMargin/EditSetRightMargin |
| WP-006 | Center a line between the active margins. | MUST | EditCenterLine |
| WP-007 | Support tab characters and configurable tab width. | MUST | EditTab/EditDefineTab |
| WP-008 | Distinguish actual tab characters from visual tab navigation. | SHOULD | Modernization |
| WP-009 | Optionally annotate automatically wrapped lines. | SHOULD | Wrapped flag |
| WP-010 | Historical WordStar wrap markers may only be exposed via an explicit compatibility import/export mode. | MAY | ASCII 141 in ch. 12 |
| WP-011 | Word boundaries and separators must be extensible and Unicode-aware. | SHOULD | Ch. 1 and modernization |

---

### 13.5 Selection and block operations

| ID | Requirement | Priority | Origin / evidence |
|---|---|---|---|
| SEL-001 | Mark the start and end of a selection or historical block. | MUST | EditBlockBegin/EditBlockEnd |
| SEL-002 | Copy, move and delete selected text. | MUST | EditBlockCopy/EditBlockMove/EditBlockDelete |
| SEL-003 | Allow hiding selection highlighting without necessarily discarding the logical range. | SHOULD | EditBlockHide |
| SEL-004 | Support operations between views/documents where semantically valid. | SHOULD | Ch. 12: blocks from another window/file |
| SEL-005 | Default modern selection must work at character granularity across lines. | MUST | Modernize line-block model |
| SEL-006 | A historical line-oriented block mode may be offered as a compatibility profile. | MAY | Optional historical fidelity |
| SEL-007 | Support selecting the entire document. | MUST | Modern baseline |
| SEL-008 | Update selection positions deterministically after edits. | MUST | Quality requirement |

---

### 13.6 Search, replacement and markers

| ID | Requirement | Priority | Origin / evidence |
|---|---|---|---|
| SRCH-001 | Support text search, find next and find/replace. | MUST | EditFind/EditReplace and Cp variants |
| SRCH-002 | Support replace all. | SHOULD | Modern feature |
| SRCH-003 | Search options should include at least case sensitivity, direction and whole word. | SHOULD | Handbook find/replace with options |
| SRCH-004 | Regular expressions may be offered optionally. | MAY | Modern extension |
| SRCH-005 | Search within document, selection or a defined range. | SHOULD | Modern extension |
| SRCH-006 | Search and replacement operations must be cancellable. | MUST | Abortcmd/EditAbort/EditBreathe, modernized |
| SRCH-007 | Preserve recent search/replacement strings and options where useful. | SHOULD | Searchstr/Replacestr/Optstr |
| SRCH-008 | Report a missing match as an unambiguous result state. | MUST | Notfound |
| SRCH-009 | Allow setting, jumping to and choosing markers via a prompt. | MUST | EditSetMarker/EditJumpMarker/EditCpjmpmrk |
| SRCH-010 | Compatibility should support at least 20 markers; internal capacity may be greater. | SHOULD | Maxmarker = 20 |
| SRCH-011 | Markers must belong to documents and track positions reliably through edits. | MUST | Modernization |

---

### 13.7 Multiple views, windows and linked text streams

| ID | Requirement | Priority | Origin / evidence |
|---|---|---|---|
| VIEW-001 | Display several document views simultaneously. | MUST | Ch. 12, Multiple Windows |
| VIEW-002 | Create, close, activate and resize views. | MUST | EditWindowCreate/Delete/Goto |
| VIEW-003 | Closing a view must not inadvertently destroy unsaved document content. | MUST | Historical behavior and modern data-safety requirement |
| VIEW-004 | Switch forward and backward among views. | MUST | EditWindowUp/EditWindowDown |
| VIEW-005 | Activate views by number, identifier or selection. | MUST | EditWindowGoto/EditCpgotowin |
| VIEW-006 | Allow two or more views to share the same document. | MUST | EditWindowLink/EditCplnkwin |
| VIEW-007 | Changes made through one view must appear consistently in linked views. | MUST | Ch. 12 |
| VIEW-008 | Keep hidden documents/views in memory and show them later. | SHOULD | Ch. 12 |
| VIEW-009 | Each view maintains its own caret, selection, scrolling, mode and margin state. | MUST | Windesc |
| VIEW-010 | Modern desktop adapters should support horizontal and vertical splits. | SHOULD | Modernize historic horizontal split |
| VIEW-011 | Do not artificially cap views at eight or nine. | MUST | Modernization |
| VIEW-012 | Manage active view and view ordering unambiguously. | MUST | Curwin and circular window list |

---

### 13.8 File and stream operations

| ID | Requirement | Priority | Origin / evidence |
|---|---|---|---|
| FILE-001 | Read text files into new or existing documents. | MUST | EditFileRead/EditReatxtfil |
| FILE-002 | Support Save, Save As and writing to a named file. | MUST | EditFileWrite/EditFileSave |
| FILE-003 | Invoke file operations interactively through host prompts or programmatically with parameters. | MUST | Cp and parameterized layers |
| FILE-004 | File operations should be asynchronous and cancellable. | SHOULD | Modernize Breathe/Abort |
| FILE-005 | Detect, preserve and intentionally convert encodings and line endings. | MUST | Modernization |
| FILE-006 | Never silently overwrite binary/unsupported files as ordinary text. | MUST | Security requirement |
| FILE-007 | Handle read/write failures without losing the existing document content. | MUST | File I/O errors and error handling |
| FILE-008 | Where possible, save atomically using a temporary file followed by replacement. | SHOULD | Modern robustness |
| FILE-009 | Warn before overwriting externally modified files where possible. | SHOULD | Modern data-safety |
| FILE-010 | Support streams in addition to filesystem paths. | SHOULD | Modern integration |
| FILE-011 | Historical WordStar wrap markers may be supported via optional import/export. | MAY | Ch. 12 |
| FILE-012 | Reading/writing a marked block as a separate file is optional. | MAY | MicroStar Block Read/Write |
| FILE-013 | Renaming, copying, deleting and directory listing are optional host services, not mandatory core capabilities. | MAY | MicroStar additions |
| FILE-014 | Before discarding unsaved changes, host must be able to offer Save/Discard/Cancel. | MUST | Dirty bit; Save/Open/Abandon |

---

### 13.9 Undo, redo and dirty state

| ID | Requirement | Priority | Origin / evidence |
|---|---|---|---|
| UNDO-001 | Editing and deletion operations must be undoable. | MUST | EditUndo |
| UNDO-002 | Support multiple undo steps and redo. | MUST | Modern baseline |
| UNDO-003 | Group related edits into transactions. | MUST | Modern requirement |
| UNDO-004 | Undo limit must be configurable. | MUST | EditSetUndoLimit/Undolimit |
| UNDO-005 | Support limiting by operation count, memory budget or both. | SHOULD | Modernization |
| UNDO-006 | Undo/redo belongs to the document, independent of the number of views. | MUST | Shared document |
| UNDO-007 | Dirty state must reliably reflect differences from the saved state. | MUST | EditChangeFlag |
| UNDO-008 | Mark the saved state clean after successful save. | MUST | Ch. 8 |
| UNDO-009 | Custom extension commands must be able to integrate with undo/redo and dirty state. | MUST | Ch. 8: custom commands set ChangeFlag |

---

### 13.10 Command system and keyboard profiles

| ID | Requirement | Priority | Origin / evidence |
|---|---|---|---|
| CMD-001 | Invoke all user operations via uniquely identifiable editor commands. | MUST | Command / Command Processor |
| CMD-002 | Use a central dispatcher to map input to commands. | MUST | EditPrccmd |
| CMD-003 | Commands must be callable independently from menu, keyboard, toolbar, context menu or API. | MUST | Handbook and modernization |
| CMD-004 | Commands must check executability and be callable synchronously or asynchronously. | MUST | Modern command requirement |
| CMD-005 | Hooks/interceptors must be able to filter, replace, suppress or handle commands. | MUST | UserCommand |
| CMD-006 | A fully handled command must not be executed again by the default dispatcher. | MUST | UserCommand return 255 |
| CMD-007 | Support individual keypresses and multistage prefix sequences. | MUST | UserCommand; Ctrl-K/O/Q |
| CMD-008 | Keyboard profiles must be replaceable and serializable as user configurations. | MUST | Configurable dispatchers and modernization |
| CMD-009 | Provide a modern default and Turbo Editor/WordStar-compatible profile. | SHOULD | Project decision |
| CMD-010 | Hosts must be able to register custom commands and replace/disable built-in commands. | MUST | Ch. 6 |
| CMD-011 | Optionally enqueue several input events programmatically. | MAY | EditUserPush/Pokechr, macros |
| CMD-012 | Architecture should permit macros/command languages, but these are not first-milestone features. | MAY | Ch. 6 |
| CMD-013 | Provide global cancellation of long-running operations. | MUST | Ctrl-U/EditAbort |
| CMD-014 | Unknown input must not trigger uncontrolled behavior. | MUST | Quality requirement |

---

### 13.11 UI and presentation

| ID | Requirement | Priority | Origin / evidence |
|---|---|---|---|
| UI-001 | Provide WinForms as a reusable control or cleanly encapsulated adapter component. | MUST | Project decision |
| UI-002 | Display text, caret, selection, matches, markers and special annotations. | MUST | Screen routines, blocks, Colored |
| UI-003 | Status display must show at least filename, line, column, dirty state and editing mode. | MUST | Status line/Windesc |
| UI-004 | Host must be able to replace, extend or hide status display. | MUST | UserStatusLine |
| UI-005 | Command/message line or functionally equivalent prompts should be possible. | SHOULD | Command line |
| UI-006 | Menus, toolbars and context menus must use the same command model. | MUST | Modernized MicroStar pulldown menu |
| UI-007 | Optional retro/MicroStar sample may recreate historic pulldown menus. | MAY | MicroStar |
| UI-008 | Host services must be able to replace modal and modeless prompts. | MUST | MakeWindow/RestoreWindow/EditAskfor |
| UI-009 | Error and confirmation presentation must be platform-replaceable. | MUST | UserError |
| UI-010 | Themes must distinguish ordinary text, selection, status, commands and special content. | MUST | Txtcolor/Blockcolor/Bordcolor/Cmdcolor/Usercolor |
| UI-011 | UI must not depend on video memory, retrace timing or an 80×25 grid. | MUST | Modernize ch. 11 |
| UI-012 | WinForms sample must demonstrate menus, toolbar, status bar, multiple views and file dialogs. | MUST | Project decision |
| UI-013 | WPF and web obtain their own adapters to the same core. | SHOULD | Project decision |
| UI-014 | Legacy ASPX/Web Forms is not a first-class initial control but may integrate the core. | SHOULD | Project decision |
| UI-015 | Optionally display invisibles: line endings, tabs, spaces and control characters. | SHOULD | Modernize ch. 1 |
| UI-016 | Optionally display line numbers, margins and current-caret line. | SHOULD | Modern enhancement |
| UI-017 | Rendering and scrolling must remain responsive during typing. | MUST | Interruptible screen updates, ch. 11 |
| UI-018 | WinForms adapter must account for high DPI, resizing, focus and IME. | MUST | Modern desktop requirement |

---

### 13.12 Errors, messages and confirmations

| ID | Requirement | Priority | Origin / evidence |
|---|---|---|---|
| ERR-001 | Core returns structured errors and result objects, not only human-readable messages. | MUST | Modernize EDITERR.MSG |
| ERR-002 | Hosts may handle errors entirely or forward them to default handling. | MUST | UserError, Msgno=0 |
| ERR-003 | Error messages must be localizable. | MUST | Modernize external error catalog |
| ERR-004 | Distinguish file, memory, input, search and state errors. | MUST | Historical error codes |
| ERR-005 | Errors must not cause partial edits or data loss. | MUST | Quality requirement |
| ERR-006 | Configure confirmations for destructive operations centrally. | MUST | MicroStar Delete-Text confirmation |
| ERR-007 | Library code must not force UI dialogs. | MUST | UI independence |

---

### 13.13 Background tasks

| ID | Requirement | Priority | Origin / evidence |
|---|---|---|---|
| BG-001 | Provide a generic facility for incremental background/host work without a proprietary editor scheduler. | SHOULD | Modernize UserTask |
| BG-002 | Background tasks must react promptly to input/cancellation and preserve state. | MUST | Cooperative multitasking, ch. 10 |
| BG-003 | Optional autosave must be integrable as a host service. | SHOULD | Proposed UserTask use |
| BG-004 | Background printing may be documented as an example/optional extension. | MAY | MicroStar PrintNext |
| BG-005 | Historical modem dialing, upload/download and disk backup are not editor features. | EXCLUDED | Ch. 10; retain only generic task hook |
| BG-006 | Modern hosts should use tasks, CancellationToken and platform dispatchers rather than polling. | SHOULD | Modernization decision |

---

### 13.14 Embedding and extensibility

| ID | Requirement | Priority | Origin / evidence |
|---|---|---|---|
| INT-001 | Embed editor in larger applications. | MUST | Ch. 14 |
| INT-002 | Hosts provide documents, commands, services and settings programmatically. | MUST | Original plus modernization |
| INT-003 | Core must run headlessly for tests, batch and server-side work. | MUST | Project decision |
| INT-004 | Clipboard, file dialogs, printing, themes, input, logging and notifications must be replaceable through adapters/services. | MUST | Project decision |
| INT-005 | Integration must not require special include order, globals, overlays or chain files. | MUST | Modernize chs. 6, 13, 14 |
| INT-006 | Support dependency injection and public interfaces. | SHOULD | SASD Development Standard |
| INT-007 | Library family should be packagable through NuGet. | SHOULD | Modern distribution |
| INT-008 | Public APIs must be versioned and documented. | MUST | Product requirement |
| INT-009 | Desktop Components should only reference/showcase the editor. | SHOULD | Project decision |
| INT-010 | Data Toolbox may supply optional storage, never a mandatory dependency. | MUST | Product boundary |
| INT-011 | Numerics, Graphics and GameWorks may consume the editor without cyclic dependencies. | MUST | Product-family decision |
| INT-012 | Future syntax highlighting and Markdown/log/hex/property editors should use public extension points. | SHOULD | Product vision |

---

### 13.15 Settings

| ID | Requirement | Priority | Origin / evidence |
|---|---|---|---|
| SET-001 | Settings must cover keyboard profiles, themes, fonts, tab width, margins, word wrap, auto-indent, undo limits and display options. | MUST | Original plus modernization |
| SET-002 | Separate global defaults from per-document/per-view overrides. | SHOULD | Modernization |
| SET-003 | Settings should be serializable, validatable and recoverable. | SHOULD | Modern requirement |
| SET-004 | Invalid values must cause clear errors or safe defaults. | MUST | Quality requirement |
| SET-005 | 80 columns, 25 lines and historical colors may only be demonstration/compatibility settings. | MUST | Modernization |

---

## 14. Nonfunctional requirements


| ID | Requirement | Priority | Origin / evidence |
|---|---|---|---|
| NFR-ARCH-001 | Clearly separate Core, application services, UI adapters and samples. | MUST | Project decision |
| NFR-ARCH-002 | Do not introduce cyclic project or package dependencies. | MUST | SASD Development Standard |
| NFR-ARCH-003 | UI-framework types must not appear in public Core APIs. | MUST | Project decision |
| NFR-PERF-001 | Normal editing and caret operations must feel immediate to the user. | MUST | Historical “lightning fast” goal |
| NFR-PERF-002 | Rendering should update only affected visible regions. | SHOULD | Differential screen updates, ch. 11 |
| NFR-PERF-003 | Long-running file and search operations must not permanently block the UI. | MUST | Modernized interruptible routines |
| NFR-PERF-004 | Architecture must allow later optimized buffers for very large files. | MUST | Project decision |
| NFR-ROB-001 | Protect unsaved changes before destructive operations. | MUST | Dirty bit |
| NFR-ROB-002 | Saving must be as robust as possible against I/O errors. | MUST | Modern requirement |
| NFR-ROB-003 | Core must be deterministic and testable without global mutable singletons. | MUST | Modernization |
| NFR-SEC-001 | Treat file paths and contents as untrusted. | MUST | Modern security |
| NFR-SEC-002 | Never execute macros, scripts or embedded content automatically. | MUST | Modern security |
| NFR-SEC-003 | Store temporary and autosave files in controlled locations with suitable permissions. | SHOULD | Modern security |
| NFR-ACC-001 | Desktop adapters must be fully usable via keyboard. | MUST | Historical keyboard focus plus accessibility |
| NFR-ACC-002 | Consider contrast, focus and screen-reader information. | SHOULD | Modern accessibility |
| NFR-I18N-001 | Support Unicode, localizable messages and culture-sensitive UI text. | MUST | Modernization |
| NFR-TEST-001 | Core must have comprehensive unit tests. | MUST | SASD Development Standard |
| NFR-TEST-002 | Architecture tests must enforce dependency direction. | MUST | Project decision |
| NFR-TEST-003 | Integration tests must cover file workflows, undo/redo, search, views and keyboard profiles. | MUST | Acceptance requirement |
| NFR-DOC-001 | Public APIs must include XML documentation comments. | MUST | User preference and SASD standard |
| NFR-DOC-002 | Plan README, requirements/implementation specifications, architecture, developer, test and user manuals. | MUST | SASD Development Standard |
| NFR-DOC-003 | Map historical functions to modern requirements or deliberate exclusions. | MUST | Completeness requirement |
| NFR-COMP-001 | The first production version should use a supported .NET LTS version, selected in the implementation specification. | SHOULD | Modern project requirement |
| NFR-COMP-002 | WinForms and WPF are Windows adapters; Core remains platform-neutral. | MUST | Project decision |
| NFR-LIC-001 | Do not adopt original source, trademarks or protected visual design without review. | MUST | Handbook copyright notice |
| NFR-LIC-002 | Review the name, trademark references and copyright notices before publication. | MUST | Legal caution |
| NFR-MAINT-001 | Code must be clearly structured, generously commented and documented. | MUST | User preference |
| NFR-MAINT-002 | Document and version breaking changes. | MUST | Product requirement |
| NFR-OBS-001 | Errors and important file operations should support abstracted logging. | SHOULD | Operational requirement |

---

## 15. Historical command coverage

The following table ensures that no command category described in the handbook's functional ch. 12 or its sample applications disappears without an explicit decision.

| Category | Historical function coverage | Treatment |
|---|---|---|
| **Caret/navigation** | Left/Right Character, Up/Down Line, Scroll Up/Down, Up/Down Page, Left/Right Word, Beginning/End Line, Top/Bottom File, Top/Bottom Block, Go To Line, Go To Column | Covered by NAV requirements |
| **Deleting text** | Delete Right/Left Character, Delete Line, Delete Window Text, Delete Block, Delete Text Right, Delete Right Word | Covered by EDIT and SEL requirements |
| **Text processing** | Replace, Center Line, Change Case, Find, Jump/Set Marker, Undo Limit, margins, Auto-Indent, Word-Wrap, Reformat, Control Character, Undo, Insert, Tab | Covered by WP, SRCH, UNDO and EDIT requirements |
| **Windows/views** | Create, Delete, Top/Bottom File, Delete Text, Link, Up, Down, Goto | Covered by VIEW requirements |
| **Blocks** | Begin, End, Copy, Move, Delete, Hide/Show | Covered by SEL requirements |
| **Files** | Read, Write, Save, interactive versions and text-file I/O | Covered by FILE requirements |
| **Exiting** | Exit and interactive Exit version | Covered by host lifecycle and dirty-state decision |
| **MicroStar additions** | Pulldown menus, pop-ups, Save/Open/Abandon, Block Read/Write, file management, directory display, background printing, spell checking | Core features covered; optional extensions identified |

### 15.1 FIRST-ED compatibility profile

The handbook documents a WordStar-like FIRST-ED interaction profile featuring:

- Ctrl-A/S/D/F/E/X/R/C/W/Z for navigation;
- Return, Insert Line, character/word/line deletion;
- Reformat, Insert mode, Find Next and Undo;
- Ctrl-O commands for windows, word wrap, centering, columns/lines, letter case, margins and undo limit;
- Ctrl-Q commands for rapid navigation, auto-indent, block boundaries, markers, find and replace;
- Ctrl-K commands for block operations, files, tabs, markers and exit.

These sequences are not imposed as the only modern default keyboard layout. They should be representable through an optional compatibility profile.

### 15.2 MicroStar additions

MicroStar adds or modifies:

- F10 pulldown menus;
- pop-up windows for input and confirmation;
- Save-and-Resume, Save-and-Open, Abandon-and-Open;
- Block Read and Block Write;
- background printing;
- optional spell checking;
- customized status and error display;
- changed WordStar-compatible key bindings.

For v1.0, command/menu integration, customizable status/errors and safe file workflows are important. Spell checking, block files and background printing remain optional.

---

## 16. Data and state requirements

### 16.1 Document state

A document must hold at least:

- unique identity;
- text;
- optional file path;
- display name;
- character encoding;
- line-ending format;
- saved reference state;
- dirty state;
- undo/redo history;
- markers;
- optional annotations.

### 16.2 View state

A view must hold at least:

- referenced document;
- caret position;
- selection;
- vertical and horizontal scroll position;
- preferred visual column;
- insert/overwrite mode;
- word wrap;
- auto-indent;
- left and right margin;
- tab width;
- zoom and display options;
- active focus state.

### 16.3 Session state

A host may additionally manage:

- open documents;
- visible/hidden views;
- layout and split configuration;
- active view;
- keyboard profile;
- theme;
- recent search options;
- recovery/autosave information.

### 16.4 Eliminate global state

Historical global variables must not be reproduced as mutable process-wide globals. Instead, distribute their responsibilities among document, view, session, command, rendering and service state.

---

## 17. Platform and interface requirements

### 17.1 WinForms

The first reference integration must:

- be embeddable as a reusable component;
- handle keyboard and mouse;
- support clipboard;
- connect file dialogs and host services;
- demonstrate menus, toolbar and status bar;
- support several views;
- be high-DPI capable;
- avoid duplicating functional logic.

### 17.2 WPF

The later WPF adapter must:

- use the same core;
- encapsulate WPF-specific commands, input bindings, dependency properties and rendering;
- require no WinForms dependency.

### 17.3 ASP.NET and web

- Core must remain usable server-side.
- Interactive browser UI requires its own adapter.
- Prefer Blazor or Razor over building a major new ASPX/Web Forms component.
- ASPX applications may integrate via host adapters, JavaScript components or server-side Core services.
- Browser clipboard, IME, focus, selection and rendering belong to web adapters.

### 17.4 Headless use

Core must work without a visible UI for:

- unit tests;
- text transformations;
- server-side search/replace;
- import/export;
- batch processing;
- specialized host applications.

---

## 18. Security and privacy requirements

1. Treat text files and paths as untrusted.
2. Never automatically execute files as code, macros or scripts.
3. Destructive file operations require an explicit host decision.
4. Temporary and recovery files must not create avoidable plaintext copies in unexpected locations.
5. Errors must not unnecessarily log sensitive file contents or credentials.
6. External file changes should be detectable.
7. An editor control must not perform network communication without an explicit host request.
8. Optional plugins and macros will need a separate security model.

---

## 19. Documentation requirements

At a minimum, the repository must provide or plan the following:

```text
README.md                      # GitHub repository entry point
AGENTS.md                      # Agent instructions
LICENSE                        # GitHub license detection

.github/
├── CONTRIBUTING.md            # GitHub discovery pointer
└── SECURITY.md                # GitHub discovery pointer

docs/
├── de/
│   ├── README.md
│   ├── ROADMAP.md
│   ├── Lastenheft.md
│   ├── Pflichtenheft.md
│   ├── Architektur.md             # existing
│   ├── Benutzerhandbuch.md        # existing
│   ├── Entwicklerhandbuch.md      # existing
│   ├── Testhandbuch.md            # existing
│   └── Historical-Traceability.md # existing
└── en/
    ├── README.md
    ├── ROADMAP.md
    ├── CHANGELOG.md
    ├── CONTRIBUTING.md
    ├── SECURITY.md
    ├── PLANS.md
    └── further English documents

src/
tests/
samples/
```

The implementation specification determines the concrete structure. The directory organization updated on 3 November 2026 places English repository guidance under `docs/en/`, while German source requirements stay under `docs/de/`. Guides marked “existing” were added on 2026-11-03. Their existence does not certify that planned product features have been accepted. Public APIs need XML documentation, short examples and information about side effects, threading, cancellation, error conditions and API stability.

---


## 20. Testing and quality requirements

### 20.1 Unit tests

At minimum, test:

- text positions and ranges;
- insertion, overwriting and deletion;
- splitting and joining lines;
- caret movement;
- selection and block operations;
- word wrap and auto-indent;
- margins and tabs;
- search and replace;
- markers;
- undo/redo;
- dirty state;
- multiple views;
- file and stream operations;
- command routing and key profiles;
- cancellation and error handling.

### 20.2 Integration and adapter tests

- WinForms control in a sample host;
- clipboard;
- integration of file dialogs and host services;
- high DPI and resizing;
- input focus and IME;
- keyboard sequences;
- multiple views;
- Save/Discard/Cancel.

### 20.3 Architecture tests

- Core references neither WinForms, WPF nor ASP.NET.
- Adapters reference Core, never the reverse.
- Samples do not implement the core themselves.
- Desktop Components consumes projects/packages without cyclic dependencies.

### 20.4 Load and performance tests

- extremely long lines;
- extremely many lines;
- repeated undo/redo;
- search in large files;
- multiple linked views;
- fast scrolling and typing;
- memory release after closing large documents.

---

## 21. Acceptance criteria

| ID | Criterion |
|---|---|
| AK-001 | An empty document can be created, edited, saved, closed and reopened with identical content. |
| AK-002 | Unicode characters, including German umlauts, non-Latin writing systems and emoji, survive with supported encodings. |
| AK-003 | Undo/redo works for insertion, deletion, multiline edits, block operations and replacement. |
| AK-004 | Two views of one document share content but retain separate caret and scroll positions. |
| AK-005 | Every command category from ch. 12 is implemented, mapped to a modern equivalent or explicitly deferred/excluded. |
| AK-006 | A keyboard profile can be loaded without recompiling Core. |
| AK-007 | A host command can be registered and triggered through menu and keyboard. |
| AK-008 | Core is testable without a Windows UI thread. |
| AK-009 | The Core project has no WinForms, WPF or ASP.NET references. |
| AK-010 | Closing modified documents gives the host a Save/Discard/Cancel decision point. |
| AK-011 | File errors do not destroy the previous document content. |
| AK-012 | Search/replace is cancellable and reports “not found” unambiguously. |
| AK-013 | WinForms demo shows status, multiple views, file operations, search, undo/redo and at least two keyboard profiles. |
| AK-014 | Public APIs have XML documentation; core functions have automated tests. |
| AK-015 | Historical chapter/API traceability exists in the repository. |

---

## 22. Priorities and milestones

| Milestone | Goal | Contents |
|---|---|---|
| M0 | Requirements and architecture baseline | Requirements specification, legal/naming review, implementation specification, architecture, rendering decision |
| M1 | Modern FIRST-ED / 0.1 | Core, basic edits, files, undo/redo, search, dirty state, commands, WinForms control, sample, tests |
| M2 | Historical functionality / 0.5 | Margins, wrapping, auto-indent, markers, blocks, windows, key profiles, themes, error/prompt services |
| M3 | Stable library / 1.0 | API stabilization, NuGet, performance, large-file path, DE/EN documentation, integration and architecture tests |
| M4 | WPF adapter | Dedicated WPF control and sample using the identical core |
| M5 | Web adapter | Blazor/Razor component; optional ASPX integration |
| M6 | Extensions | Syntax highlighting, Markdown, log/hex viewer, macros, optional background printing |

### 22.1 Priority semantics

- **MUST:** necessary for product value or safe architecture.
- **SHOULD:** important but may be deferred with justification.
- **MAY:** optional extension.
- **EXCLUDED:** historical functionality/technology deliberately not adopted.

### 22.2 Recommended MVP boundary

Version 0.1 does not require:

- complete WPF/web adapters;
- plugin framework;
- syntax highlighting;
- highly optimized Rope/Piece Table buffer;
- background printing;
- spell checking;
- full retro UI;
- complex macro language.

Its architecture must nevertheless allow those features to be added later without rebuilding the core.

---

## 23. Risks and mitigation

| Risk | Impact | Mitigation |
|---|---|---|
| R-01 — Underestimating custom rendering | A full independent editor renderer costs much more than wrapping RichTextBox. | Decide wrapper/hybrid/custom rendering early; limit MVP. |
| R-02 — Premature multi-UI work | Concurrent development destabilizes APIs and schedule. | Core and WinForms first; other adapters after API stabilizes. |
| R-03 — Adopting historical limits | ASCII, 255-character lines and global state reduce future viability. | Transfer functional ideas only. |
| R-04 — Overengineering | Too many assemblies, plugins and buffer engines delay 0.1. | Lean initial structure; extend for proven needs. |
| R-05 — Large files | Naive strings/lists can be slow and memory-heavy. | Buffer abstraction, benchmarks, later Piece Table/Rope. |
| R-06 — IME and international input | Custom text input is complex. | Encapsulate in adapter; test Unicode/IME early. |
| R-07 — Trademark/copyright confusion | Names or original code can create legal issues. | Independent SASD branding, clean-room implementation and review. |
| R-08 — Blurring the Desktop Components boundary | Duplication or cyclic dependencies. | Independent editor; Desktop Components is a consumer only. |

---

## 24. Assumptions and open decisions

Decide before development or in the implementation specification:

1. Should the first WinForms adapter wrap an existing text control, use a hybrid approach or draw everything itself?
2. Which .NET LTS target version?
3. What are maximum file sizes for 0.1 and 1.0?
4. Which initial buffer implementation?
5. Are word wrap and margins purely presentational, text-changing paragraph formatting, or both?
6. Which search options are mandatory in 0.1?
7. How do markers track complex edits?
8. Which NuGet packages should be published?
9. Which license should the new implementation use?
10. Which public product/package name avoids trademark confusion?
11. Does the historic Ctrl-K/O/Q profile belong in 0.1 or 0.5?
12. What accessibility level is expected for the first WinForms adapter?
13. What kind of autosave and crash recovery should 1.0 provide?
14. Will the web adapter use Blazor, JavaScript interop or server-side editor APIs?

---

## 25. Historical chapter traceability

| Source | Essential content | Coverage |
|---|---|---|
| Introduction | Purpose; simple and complex editors; characters, words, lines, streams, windows, blocks, files, screen | PROD, CORE, INT |
| Ch. 1 | Terminology, document/non-document mode | CORE, CMD |
| Ch. 2 | Line editor, WYSIWYG, RAM, virtual and swapping editors | Product boundary, performance |
| Ch. 3 | Array of lines, fixed buffer, linked list | Buffer abstraction, large-file support |
| Ch. 4 | Simple editor | Samples and learning path |
| Ch. 5 | FIRST-ED, initialization, hooks, default commands | Core commands, status, sample |
| Ch. 6 | UserCommand, single and prefix commands, macro/command-language hooks | Commands, extensibility |
| Ch. 7 | MicroStar, menus, extended commands | UI and optional features |
| Ch. 8 | Dispatcher, menus, popups, background print, errors, status, dirty bit | UI, errors, background, dirty state |
| Ch. 9 | Text/window structures, window linking | Document/view model |
| Ch. 10 | Input, scheduler, dispatcher, UserTask | Input, commands, cancellation, background |
| Ch. 11 | Screen drawing, differential updates, colors | Rendering, performance, themes |
| Ch. 12 | Navigation, deletion, text processing, windows, blocks, files, exit | Functional requirements |
| Ch. 13 | Overlays and memory optimization | Not ported literally; modern packages |
| Ch. 14 | Embedding in programs, overlay/chain | Library and adapter |
| Ch. 15 | Historical module/file structure | Modularity reference |
| Ch. 16 | Constants | Compatibility and traceability appendix |
| Ch. 17 | Data types | Modern data model |
| Ch. 18 | Global variables | Encapsulated state and services |
| Ch. 19 | Procedures and functions | API traceability |

---

## 26. Historical constants, types and state

### 26.1 Constants

| Historical element | Meaning | Modern treatment |
|---|---|---|
| `Col1`, `Line1` | Logical starting positions | Normal position abstraction |
| `Colored`, `Inblock`, `Wrapped` | Line flags | Generic annotations, selection and wrap metadata |
| `Ctrla` through `Ctrlz`, `Del`, `Escape`, `Nul` | ASCII key codes | Key-gesture/input abstraction |
| `Defhelplen` | Help window height | UI layout setting |
| `Defnocols`, `Defnorows` | 80×25 screen | Retro sample only, no product limit |
| `Deftypahd` | Typeahead buffer size | Configurable queue or framework input |
| `Errorfile` | `EDITERR.MSG` | Localizable error catalog |
| `Maxlinelength` | 255 characters | Remove fixed limit |
| `Maxmarker` | 20 markers | At least compatible; extensible internally |
| `Nofile` | `NONAME` | Untitled-document state |
| `Screenadr` | Video-memory address | Removed completely |
| Status labels | AI, Col, File, INS, Line, TE, Window, WW | Localizable status model |
| `Zero` through `Nine` | ASCII digits | Standard input |

### 26.2 Data types

| Historical type | Meaning | Modern treatment |
|---|---|---|
| `Character` | Character and color | Rendering glyph/style in UI adapter |
| `Insflag` | Insert/typeover | EditorInsertMode |
| `Linedesc` / `Plinedesc` | Linked line descriptor | Internal buffer implementation |
| `Textline` / `Ptextline` | Line text | Unicode line/span abstraction |
| `Windesc` / `Pwindesc` | Window state and text-stream reference | EditorViewState and DocumentReference |
| Fixed-length Pascal strings | String80, Varstring, Strvartype, St6 | `string`/spans without historic length cap |

### 26.3 Global variables

The handbook documents these global state items:

- `Abortcmd`
- `Aborting`
- `Asking`
- `Blockcolor`
- `Blockfrom`
- `Blockhide`
- `Blockto`
- `Bordcolor`
- `Circbuf`
- `Circin`
- `Circout`
- `Cmdcol`
- `Cmdcolor`
- `Cmdlinest`
- `Curwin`
- `EditChangeflag`
- `EditUsercommandInput`
- `Interactive`
- `Intrflag`
- `Linelength`
- `Logscrcols`
- `Logscrrows`
- `Logtopscr`
- `Marker`
- `Nextstream`
- `Notfound`
- `Optstr`
- `Physcrcols`
- `Physcrrows`
- `Physcrsig`
- `Replacestr`
- `Retracemode`
- `Rundown`
- `Screen`
- `Searchstr`
- `Tabsize`
- `Txtcolor`
- `Typbufovl`
- `Undocount`
- `Undoend`
- `Undolimit`
- `Undostack`
- `Updcurflag`
- `Usercolor`
- `Window1`
- `Winstack`

These are not recreated as global variables. Their responsibilities are assigned to document, view, session, command context, search service, undo/redo, theme/rendering, input queues, and host/cancellation services.

---

### 26.4 Mandatory historical hooks and MicroStar extension points

In addition to toolbox routines, the handbook requires or demonstrates host-supplied hooks. The modern architecture expressly covers these as well:

| Historical hook / extension point | Handbook responsibility | Modern treatment |
|---|---|---|
| `UserCommand` | Filter, remap, handle or suppress input | Command interceptors, key profiles and registerable host commands |
| `UserError` | Present errors or delegate to default handling | Replaceable error/notification service |
| `UserStatusLine` | Replace or extend default status | Status model, events and host-owned status component |
| `UserReplace` | Control interactive find/replace decisions | Replacement strategy, callback or prompt service |
| `UserTask` | Cooperative idle-time background tasks | Tasks, CancellationToken, dispatchers and host services |
| `MakeWindow` / `RestoreWindow` | Draw popups and preserve the underlying screen | Platform-specific dialogs, flyouts or overlays |
| `PulldownMenu` / `InitMainMenu` | Menu definition and command execution | Menus using shared command catalog |
| `PrintNext` | Incremental background printing | Optional print service/sample |
| `ErrorCheck` | MicroStar-specific error display | Host-defined error presenter |
| `SecondChar` | Read the second character in a prefix sequence | Key-chord/sequence resolver |
| `ReadBlock` / Block Write | Read or write a block as a file | Optional selection import/export |
| `SpellingCheck` | Spell checking | Later external language service, not required in Core |

### 26.5 Historical modules and files

The original modularization is not to be replicated as include-file structure, but is retained as evidence of responsibility boundaries.

| Historical file/group | Responsibility | Modern allocation |
|---|---|---|
| `VARS.ED`, `VARS.MS` | Constants, types, globals | Encapsulated Core models, options and state |
| `USER.ED`, `USER.MS` | Low-level core routines | Core/text buffer/document operations |
| `SCREEN.ED`, `SCREEN.MS` | Screen display | WinForms/WPF/web rendering adapters |
| `INIT.ED`, `INIT.MS` | Initialization | Composition root, factory, dependency injection |
| `CMD.ED`, `CMD.MS`, `FASTCMD.MS` | Individual commands | Command implementations |
| `KCMD.*`, `OCMD.*`, `QCMD.*` | Prefix commands | Command catalog, key-chord profiles |
| `K.*`, `O.*`, `Q.*`, `DISP.*` | Dispatchers | Command dispatcher/input mapping |
| `TASK.*` | Scheduler and main loop | Host event loop, tasks, cancellation |
| `INPUT.*` | Keyboard and typeahead | Platform input adapters |
| `EDITERR.MSG` | Error catalog | Localized resources/structured errors |
| `FIRST-ED.PAS` | Simple sample | Modern FIRST-ED sample |
| `MS.PAS`, `MS.*` | Sophisticated editor/MicroStar | Advanced sample/showcase |
| `PRINT.MS` | Background printing | Optional print service |
| `MSCMD.MS` | MicroStar commands | Sample/host commands |
| `SPELL.MS` | Spell checking | Optional external feature |
| `PULLDOWN.MS` | Pulldown menus | UI adapter and command menus |
| `.OVL` and chain/overlay files | Memory overlays | Eliminated; assemblies and normal runtime management |
| `README.COM`, `READ.ME` | Version and distribution notes | README, changelog and release notes |

---

## 27. Historical procedure and function inventory

The following list preserves the procedures from the handbook index for traceability. It does **not** require public C# APIs with identical names or granularity. The implementation specification maps each routine to a modern API, internal mechanism, optional extension or deliberate exclusion.

- `Advance`
- `EditAbort`
- `EditAppchar`
- `EditAppcmdnam`
- `EditAskfor`
- `EditBackground`
- `EditBeginningEndLine`
- `EditBeginningLine`
- `EditBlockBegin`
- `EditBlockCopy`
- `EditBlockDelete`
- `EditBlockEnd`
- `EditBlockHide`
- `EditBlockMove`
- `EditBottomBlock`
- `EditBreathe`
- `EditCenterLine`
- `EditChangeCase`
- `EditClsinp`
- `EditColorFile`
- `EditColorLine`
- `EditCompressLine`
- `EditCpcrewin`
- `EditCpdelwin`
- `EditCpexit`
- `EditCpFileSave`
- `EditCpFind`
- `EditCpgotocl`
- `EditCpgotoln`
- `EditCpgotowin`
- `EditCpjmpmrk`
- `EditCplnkwin`
- `EditCpReplace`
- `EditCprfw`
- `EditCpsetlm`
- `EditCpsetmrk`
- `EditCpsetrm`
- `EditCptabdef`
- `EditCpundlim`
- `EditCpwfw`
- `EditCvts2i`
- `EditDecline`
- `EditDefineTab`
- `EditDeleteLeftChar`
- `EditDeleteLine`
- `EditDeleteRightChar`
- `EditDeleteRightWord`
- `EditDeleteTextRight`
- `EditDelline`
- `EditDestxtdes`
- `EditDownLine`
- `EditDownPage`
- `EditEndLine`
- `EditErrormsg`
- `EditExit`
- `EditFileRead`
- `EditFileWrite`
- `EditFind`
- `EditGenlineno`
- `EditGotoColumn`
- `EditGotoLine`
- `EditHscroll`
- `EditIncline`
- `EditInitialize`
- `EditInsertCtrlChar`
- `EditInsertLine`
- `EditJoinLine`
- `EditJumpMarker`
- `EditK`
- `EditLeftChar`
- `EditLeftWord`
- `EditLongLine`
- `EditMarkblock`
- `EditNewLine`
- `EditO`
- `EditOffblock`
- `EditPrccmd`
- `EditPrctxt`
- `EditPushtbf`
- `EditQ`
- `EditRealign`
- `EditReatxtfil`
- `EditReformat`
- `EditReplace`
- `EditRightChar`
- `EditRightWord`
- `EditSchedule`
- `EditScrollDown`
- `EditScrollUp`
- `EditSetLeftMargin`
- `EditSetMarker`
- `EditSetRightMargin`
- `EditSetUndoLimit`
- `EditShiftLine`
- `EditShortLine`
- `EditSystem`
- `EditTab`
- `EditToggleAutoindent`
- `EditToggleInsert`
- `EditToggleWordwrap`
- `EditTopBlock`
- `EditUndo`
- `EditUpcase`
- `EditUpdphyscr`
- `EditUpdrowasm`
- `EditUpdwindow`
- `EditUpdwinsl`
- `EditUpLine`
- `EditUpPage`
- `EditUserpush`
- `EditWindowBottomFile`
- `EditWindowCreate`
- `EditWindowDelete`
- `EditWindowDeleteText`
- `EditWindowDown`
- `EditWindowGoto`
- `EditWindowLink`
- `EditWindowTopFile`
- `EditWindowUp`
- `EditZapcmdnam`
- `MoveFromScreen`
- `MoveToScreen`
- `Pokechr`

---

### 27.1 Routines deliberately not ported literally

- `MoveFromScreen`, `MoveToScreen`, `EditUpdrowasm`: direct video-memory access is removed; modern UI frameworks handle rendering.
- `EditSchedule`, `EditSystem`, `UserTask` in their DOS polling form: replaced by event loops, tasks, dispatchers and cancellation.
- `Pokechr`, typeahead ring buffers and raw ASCII scan codes: replaced by input adapters and framework-independent key gestures.
- Overlay and chain mechanisms: replaced by assemblies, packages, lazy loading and standard memory management.
- Global pointer lists for lines and windows: replaced by encapsulated document, buffer and view types.

---

## 28. Legal and historical separation

The handbook names Borland International Inc. as the 1985 document copyright owner. The SASD project aims to be an **independent modern reimplementation of the described ideas and capabilities**.

Consequently:

- no unreviewed reuse of historical source;
- no publication of scanned handbook material in the repository;
- no copying of logos, cover art or advertising text;
- review trademark use in product names and marketing;
- label the project clearly as independent SASD work;
- apply a separate license to newly written code;
- cite historical sources in a background document without implying official Borland succession.

This requirements specification is not legal advice. Public commercialization or a trademark-adjacent name requires a separate rights assessment.

---

## 29. Definition of done for the requirements specification

The requirements specification is ready for approval when:

- product vision and scope boundaries are confirmed;
- MUST/SHOULD/MAY priorities are reviewed;
- independent repository choice is confirmed;
- UI adapter sequence is confirmed;
- outstanding legal/naming questions are recorded;
- all historical chapters and command categories appear in traceability;
- MVP scope boundary is confirmed;
- a following implementation specification can define architecture, projects, APIs, data structures, technology choices and tests.

---

## 30. Overall approval recommendation

The project is functionally sensible and realistic if treated as an incrementally built editor library rather than a quick WinForms wrapper.

The fundamental direction is:

1. independent SASD Editor Toolkit;
2. UI-independent core;
3. WinForms as first reference UI;
4. full modern functional mapping of historical core capabilities;
5. WPF/web only after stabilizing the core;
6. Desktop Components as consumer and showcase;
7. other toolboxes as independent products;
8. historical traceability without a literal technical port;
9. lean MVP, avoiding early overengineering;
10. sound documentation, tests and a long-lived stable API.

This provides a foundation for drafting the **implementation specification and architecture document for v0.1 — Modern FIRST-ED**.

<!-- English translation prepared from docs/de/Lastenheft.md; requirement identifiers and historical routine names preserved. -->
