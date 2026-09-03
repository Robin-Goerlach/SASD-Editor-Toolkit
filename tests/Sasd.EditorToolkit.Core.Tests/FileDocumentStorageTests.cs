using System.Text;
using Sasd.EditorToolkit.Documents;
using Sasd.EditorToolkit.Storage;
using Sasd.EditorToolkit.Text;
using Xunit;

namespace Sasd.EditorToolkit.Core.Tests;

public sealed class FileDocumentStorageTests
{
    [Fact]
    public async Task Load_async_strips_utf8_bom_from_text()
    {
        var storage = new FileDocumentStorage();
        var bytes = Combine(new byte[] { 0xEF, 0xBB, 0xBF }, Encoding.UTF8.GetBytes("Hello"));
        await using var stream = new MemoryStream(bytes);

        var result = await storage.LoadAsync(stream, new DocumentLoadOptions());

        Assert.Equal("Hello", result.Document.Buffer.GetText());
    }

    [Fact]
    public async Task Save_async_preserves_detected_utf8_bom()
    {
        var storage = new FileDocumentStorage();
        var bytes = Combine(new byte[] { 0xEF, 0xBB, 0xBF }, Encoding.UTF8.GetBytes("Hello"));
        await using var source = new MemoryStream(bytes);
        var result = await storage.LoadAsync(source, new DocumentLoadOptions());
        await using var destination = new MemoryStream();

        await storage.SaveAsync(result.Document, destination, new DocumentSaveOptions());
        var output = destination.ToArray();

        Assert.True(output.Length >= 3);
        Assert.Equal(0xEF, output[0]);
        Assert.Equal(0xBB, output[1]);
        Assert.Equal(0xBF, output[2]);
        Assert.Equal("Hello", Encoding.UTF8.GetString(output, 3, output.Length - 3));
    }

    [Fact]
    public async Task Load_async_reports_binary_content_when_nul_byte_is_present()
    {
        var storage = new FileDocumentStorage();
        await using var stream = new MemoryStream(new byte[] { 65, 0, 66 });

        var result = await storage.LoadAsync(stream, new DocumentLoadOptions());

        Assert.True(result.BinaryContentSuspected);
    }

    [Fact]
    public async Task Load_async_detects_utf32_before_utf16_prefix()
    {
        var storage = new FileDocumentStorage();
        var encoding = new UTF32Encoding(bigEndian: false, byteOrderMark: true, throwOnInvalidCharacters: true);
        var bytes = Combine(encoding.GetPreamble(), encoding.GetBytes("A"));
        await using var stream = new MemoryStream(bytes);

        var result = await storage.LoadAsync(stream, new DocumentLoadOptions());

        Assert.Equal("A", result.Document.Buffer.GetText());
        Assert.IsType<UTF32Encoding>(result.Document.Metadata.Encoding);
    }

    [Fact]
    public async Task Save_async_can_skip_encoding_preamble()
    {
        var storage = new FileDocumentStorage();
        var document = new TextDocument(
            new LineTextBuffer("Hello"),
            new DocumentMetadata { Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true) });
        await using var destination = new MemoryStream();

        await storage.SaveAsync(document, destination, new DocumentSaveOptions(WriteEncodingPreamble: false));
        var output = destination.ToArray();

        Assert.Equal("Hello", Encoding.UTF8.GetString(output));
    }

    private static byte[] Combine(byte[] first, byte[] second)
    {
        var result = new byte[first.Length + second.Length];
        Buffer.BlockCopy(first, 0, result, 0, first.Length);
        Buffer.BlockCopy(second, 0, result, first.Length, second.Length);
        return result;
    }
}
