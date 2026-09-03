using System.Text;
using Sasd.EditorToolkit.Text;

namespace Sasd.EditorToolkit.Documents;

/// <summary>Metadata associated with a text document.</summary>
public sealed class DocumentMetadata
{
    /// <summary>Gets or sets the display name shown by hosts.</summary>
    public string DisplayName { get; set; } = "Untitled";

    /// <summary>Gets or sets the optional file path.</summary>
    public string? FilePath { get; set; }

    /// <summary>Gets or sets the document encoding used for saving.</summary>
    public Encoding Encoding { get; set; } = new UTF8Encoding(false);

    /// <summary>Gets or sets the preferred line ending for newly created content.</summary>
    public LineEndingKind PreferredLineEnding { get; set; } = LineEndingKind.CrLf;
}
