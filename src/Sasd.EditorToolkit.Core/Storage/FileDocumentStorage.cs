using System.Text;
using Sasd.EditorToolkit.Documents;
using Sasd.EditorToolkit.Text;

namespace Sasd.EditorToolkit.Storage;

/// <summary>File and stream based document storage.</summary>
public sealed class FileDocumentStorage : IDocumentStorage
{
    /// <inheritdoc />
    public async Task<DocumentLoadResult> LoadAsync(Stream source, DocumentLoadOptions options, CancellationToken cancellationToken = default)
    {
        using var memory = new MemoryStream();
        await source.CopyToAsync(memory, cancellationToken).ConfigureAwait(false);
        var bytes = memory.ToArray();
        var binarySuspected = bytes.Contains((byte)0);
        var encoding = DetectEncoding(bytes) ?? options.FallbackEncoding ?? new UTF8Encoding(false, true);
        var preambleLength = encoding.GetPreamble().Length;
        var text = encoding.GetString(bytes, StartsWith(bytes, encoding.GetPreamble()) ? preambleLength : 0, StartsWith(bytes, encoding.GetPreamble()) ? bytes.Length - preambleLength : bytes.Length);

        var document = new TextDocument(new LineTextBuffer(text), new DocumentMetadata { Encoding = encoding });
        document.MarkSaved();
        return new DocumentLoadResult(document, binarySuspected);
    }

    /// <inheritdoc />
    public async Task SaveAsync(TextDocument document, Stream destination, DocumentSaveOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document);
        var encoding = document.Metadata.Encoding;
        var bytes = encoding.GetBytes(document.Buffer.GetText());
        await destination.WriteAsync(bytes, cancellationToken).ConfigureAwait(false);
        await destination.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Loads a document from a file path.</summary>
    public async Task<DocumentLoadResult> LoadFileAsync(string path, DocumentLoadOptions? options = null, CancellationToken cancellationToken = default)
    {
        await using var stream = File.OpenRead(path);
        var result = await LoadAsync(stream, options ?? new DocumentLoadOptions(), cancellationToken).ConfigureAwait(false);
        result.Document.Metadata.FilePath = path;
        result.Document.Metadata.DisplayName = Path.GetFileName(path);
        result.Document.MarkSaved();
        return result;
    }

    /// <summary>Saves a document to its path or to a supplied path.</summary>
    public async Task SaveFileAsync(TextDocument document, string? path = null, DocumentSaveOptions? options = null, CancellationToken cancellationToken = default)
    {
        path ??= document.Metadata.FilePath ?? throw new InvalidOperationException("Document has no file path.");
        options ??= new DocumentSaveOptions();

        if (options.AtomicFileReplace)
        {
            var directory = Path.GetDirectoryName(Path.GetFullPath(path)) ?? Directory.GetCurrentDirectory();
            var tempPath = Path.Combine(directory, $".{Path.GetFileName(path)}.{Guid.NewGuid():N}.tmp");
            await using (var stream = File.Create(tempPath))
            {
                await SaveAsync(document, stream, options, cancellationToken).ConfigureAwait(false);
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.Move(tempPath, path);
        }
        else
        {
            await using var stream = File.Create(path);
            await SaveAsync(document, stream, options, cancellationToken).ConfigureAwait(false);
        }

        document.Metadata.FilePath = path;
        document.Metadata.DisplayName = Path.GetFileName(path);
        document.MarkSaved();
    }

    private static Encoding? DetectEncoding(byte[] bytes)
    {
        if (StartsWith(bytes, Encoding.UTF8.GetPreamble())) return new UTF8Encoding(false, true);
        if (StartsWith(bytes, Encoding.Unicode.GetPreamble())) return Encoding.Unicode;
        if (StartsWith(bytes, Encoding.BigEndianUnicode.GetPreamble())) return Encoding.BigEndianUnicode;
        if (StartsWith(bytes, Encoding.UTF32.GetPreamble())) return Encoding.UTF32;
        return null;
    }

    private static bool StartsWith(byte[] bytes, byte[] prefix)
    {
        if (prefix.Length == 0 || bytes.Length < prefix.Length) return false;
        for (var i = 0; i < prefix.Length; i++)
        {
            if (bytes[i] != prefix[i]) return false;
        }

        return true;
    }
}
