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
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);

        using var memory = new MemoryStream();
        await source.CopyToAsync(memory, cancellationToken).ConfigureAwait(false);
        var bytes = memory.ToArray();

        var binarySuspected = Array.IndexOf(bytes, (byte)0) >= 0;
        var detection = DetectEncoding(bytes);
        var encoding = detection?.Encoding ?? options.FallbackEncoding ?? new UTF8Encoding(false, true);
        var offset = detection?.PreambleLength ?? 0;
        var text = encoding.GetString(bytes, offset, bytes.Length - offset);

        var document = new TextDocument(new LineTextBuffer(text), new DocumentMetadata { Encoding = encoding });
        document.MarkSaved();
        return new DocumentLoadResult(document, binarySuspected);
    }

    /// <inheritdoc />
    public async Task SaveAsync(TextDocument document, Stream destination, DocumentSaveOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(destination);
        ArgumentNullException.ThrowIfNull(options);

        var encoding = document.Metadata.Encoding;
        if (options.WriteEncodingPreamble)
        {
            var preamble = encoding.GetPreamble();
            if (preamble.Length > 0)
            {
                await destination.WriteAsync(preamble, cancellationToken).ConfigureAwait(false);
            }
        }

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

    private static EncodingDetection? DetectEncoding(byte[] bytes)
    {
        var utf32LittleEndian = new UTF32Encoding(bigEndian: false, byteOrderMark: true, throwOnInvalidCharacters: true);
        var utf32BigEndian = new UTF32Encoding(bigEndian: true, byteOrderMark: true, throwOnInvalidCharacters: true);
        var utf8WithBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true, throwOnInvalidBytes: true);
        var utf16LittleEndian = new UnicodeEncoding(bigEndian: false, byteOrderMark: true, throwOnInvalidBytes: true);
        var utf16BigEndian = new UnicodeEncoding(bigEndian: true, byteOrderMark: true, throwOnInvalidBytes: true);

        return Detect(bytes, utf32LittleEndian)
            ?? Detect(bytes, utf32BigEndian)
            ?? Detect(bytes, utf8WithBom)
            ?? Detect(bytes, utf16LittleEndian)
            ?? Detect(bytes, utf16BigEndian);
    }

    private static EncodingDetection? Detect(byte[] bytes, Encoding encoding)
    {
        var preamble = encoding.GetPreamble();
        return StartsWith(bytes, preamble) ? new EncodingDetection(encoding, preamble.Length) : null;
    }

    private static bool StartsWith(byte[] bytes, byte[] prefix)
    {
        if (prefix.Length == 0 || bytes.Length < prefix.Length)
        {
            return false;
        }

        for (var i = 0; i < prefix.Length; i++)
        {
            if (bytes[i] != prefix[i])
            {
                return false;
            }
        }

        return true;
    }

    private sealed record EncodingDetection(Encoding Encoding, int PreambleLength);
}
