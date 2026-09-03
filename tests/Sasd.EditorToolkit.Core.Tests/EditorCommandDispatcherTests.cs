using Sasd.EditorToolkit.Commands;
using Sasd.EditorToolkit.Documents;
using Sasd.EditorToolkit.Editing;
using Sasd.EditorToolkit.Text;
using Xunit;

namespace Sasd.EditorToolkit.Core.Tests;

public sealed class EditorCommandDispatcherTests
{
    [Fact]
    public async Task Execute_async_inserts_text_at_caret()
    {
        var document = new TextDocument(new LineTextBuffer("Hello"));
        var viewState = new EditorViewState { CaretPosition = new TextPosition(0, 5) };
        var dispatcher = CreateM1Dispatcher();

        var result = await dispatcher.ExecuteAsync(
            EditorCommandIds.InsertText,
            new EditorCommandContext(document, viewState, " World"));

        Assert.True(result.Handled);
        Assert.Equal("Hello World", document.Buffer.GetText());
        Assert.Equal(new TextPosition(0, 11), viewState.CaretPosition);
    }

    [Fact]
    public async Task Execute_async_inserts_preferred_newline()
    {
        var document = new TextDocument(new LineTextBuffer("HelloWorld"));
        document.Metadata.PreferredLineEnding = LineEndingKind.Lf;
        var viewState = new EditorViewState { CaretPosition = new TextPosition(0, 5) };
        var dispatcher = CreateM1Dispatcher();

        var result = await dispatcher.ExecuteAsync(
            EditorCommandIds.NewLine,
            new EditorCommandContext(document, viewState));

        Assert.True(result.Handled);
        Assert.Equal("Hello\nWorld", document.Buffer.GetText());
        Assert.Equal(new TextPosition(1, 0), viewState.CaretPosition);
    }

    [Fact]
    public async Task Execute_async_deletes_left_of_caret()
    {
        var document = new TextDocument(new LineTextBuffer("Hello"));
        var viewState = new EditorViewState { CaretPosition = new TextPosition(0, 5) };
        var dispatcher = CreateM1Dispatcher();

        var result = await dispatcher.ExecuteAsync(
            EditorCommandIds.DeleteLeft,
            new EditorCommandContext(document, viewState));

        Assert.True(result.Handled);
        Assert.Equal("Hell", document.Buffer.GetText());
        Assert.Equal(new TextPosition(0, 4), viewState.CaretPosition);
    }

    [Fact]
    public async Task Execute_async_deletes_right_of_caret()
    {
        var document = new TextDocument(new LineTextBuffer("Hello"));
        var viewState = new EditorViewState { CaretPosition = new TextPosition(0, 0) };
        var dispatcher = CreateM1Dispatcher();

        var result = await dispatcher.ExecuteAsync(
            EditorCommandIds.DeleteRight,
            new EditorCommandContext(document, viewState));

        Assert.True(result.Handled);
        Assert.Equal("ello", document.Buffer.GetText());
        Assert.Equal(new TextPosition(0, 0), viewState.CaretPosition);
    }

    [Fact]
    public async Task Execute_async_moves_caret_left_and_right()
    {
        var document = new TextDocument(new LineTextBuffer("Hello"));
        var viewState = new EditorViewState { CaretPosition = new TextPosition(0, 3) };
        var dispatcher = CreateM1Dispatcher();

        var left = await dispatcher.ExecuteAsync(
            EditorCommandIds.MoveCaretLeft,
            new EditorCommandContext(document, viewState));
        var right = await dispatcher.ExecuteAsync(
            EditorCommandIds.MoveCaretRight,
            new EditorCommandContext(document, viewState));

        Assert.True(left.Handled);
        Assert.True(right.Handled);
        Assert.Equal(new TextPosition(0, 3), viewState.CaretPosition);
    }

    [Fact]
    public async Task Execute_async_moves_caret_across_line_boundary()
    {
        var document = new TextDocument(new LineTextBuffer("Hello\nWorld"));
        var viewState = new EditorViewState { CaretPosition = new TextPosition(1, 0) };
        var dispatcher = CreateM1Dispatcher();

        var result = await dispatcher.ExecuteAsync(
            EditorCommandIds.MoveCaretLeft,
            new EditorCommandContext(document, viewState));

        Assert.True(result.Handled);
        Assert.Equal(new TextPosition(0, 5), viewState.CaretPosition);
    }

    [Fact]
    public async Task Execute_async_moves_caret_up_and_down()
    {
        var document = new TextDocument(new LineTextBuffer("abc\ndefgh"));
        var viewState = new EditorViewState { CaretPosition = new TextPosition(1, 4) };
        var dispatcher = CreateM1Dispatcher();

        var up = await dispatcher.ExecuteAsync(
            EditorCommandIds.MoveCaretUp,
            new EditorCommandContext(document, viewState));
        var down = await dispatcher.ExecuteAsync(
            EditorCommandIds.MoveCaretDown,
            new EditorCommandContext(document, viewState));

        Assert.True(up.Handled);
        Assert.True(down.Handled);
        Assert.Equal(new TextPosition(1, 3), viewState.CaretPosition);
    }

    [Fact]
    public async Task Execute_async_returns_not_handled_for_unknown_command()
    {
        var document = new TextDocument(new LineTextBuffer("Hello"));
        var dispatcher = CreateM1Dispatcher();

        var result = await dispatcher.ExecuteAsync(
            new EditorCommandId("Unknown.Command"),
            new EditorCommandContext(document, new EditorViewState()));

        Assert.False(result.Handled);
        Assert.Contains("Unknown command", result.Message ?? string.Empty);
    }

    private static EditorCommandDispatcher CreateM1Dispatcher()
    {
        var registry = new EditorCommandRegistry();
        registry.RegisterM1Defaults();
        return new EditorCommandDispatcher(registry);
    }
}
