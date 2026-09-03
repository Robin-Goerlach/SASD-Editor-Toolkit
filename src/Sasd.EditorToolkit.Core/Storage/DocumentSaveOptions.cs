namespace Sasd.EditorToolkit.Storage;

/// <summary>Options for saving a document.</summary>
public sealed record DocumentSaveOptions(bool AtomicFileReplace = true);
