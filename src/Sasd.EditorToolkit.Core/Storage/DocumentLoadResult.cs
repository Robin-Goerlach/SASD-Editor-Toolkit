using Sasd.EditorToolkit.Documents;

namespace Sasd.EditorToolkit.Storage;

/// <summary>Result of a document load operation.</summary>
public sealed record DocumentLoadResult(TextDocument Document, bool BinaryContentSuspected = false);
