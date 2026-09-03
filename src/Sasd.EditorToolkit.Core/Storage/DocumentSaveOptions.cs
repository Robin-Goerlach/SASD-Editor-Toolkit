namespace Sasd.EditorToolkit.Storage;

/// <summary>Options for saving a document.</summary>
/// <param name="AtomicFileReplace">Use a temporary file and replace the target path when saving through file APIs.</param>
/// <param name="WriteEncodingPreamble">Write the encoding preamble, for example a BOM, when the document encoding provides one.</param>
public sealed record DocumentSaveOptions(bool AtomicFileReplace = true, bool WriteEncodingPreamble = true);
