using System.Text;

namespace Sasd.EditorToolkit.Storage;

/// <summary>Options for loading a document.</summary>
public sealed record DocumentLoadOptions(Encoding? FallbackEncoding = null);
