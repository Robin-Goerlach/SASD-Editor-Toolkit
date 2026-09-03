using Sasd.EditorToolkit.Documents;

namespace Sasd.EditorToolkit.Storage;

/// <summary>Loads and saves text documents.</summary>
public interface IDocumentStorage
{
    /// <summary>Loads a document from a stream.</summary>
    Task<DocumentLoadResult> LoadAsync(Stream source, DocumentLoadOptions options, CancellationToken cancellationToken = default);

    /// <summary>Saves a document to a stream.</summary>
    Task SaveAsync(TextDocument document, Stream destination, DocumentSaveOptions options, CancellationToken cancellationToken = default);
}
