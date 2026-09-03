# Security Policy

## Current status

The project is not yet production-ready. Report security-relevant design issues in GitHub issues until a dedicated private reporting channel is configured.

## Principles

- Treat file paths and document contents as untrusted input.
- Do not execute text, macros, scripts or embedded content automatically.
- Do not log document content by default.
- Use host-provided policy for allowed directories, autosave locations and temporary files.
- Add a dedicated security ADR before adding plugins, macros or script execution.
