using Sasd.EditorToolkit.Commands;
using Sasd.EditorToolkit.Documents;
using Sasd.EditorToolkit.Editing;
using Sasd.EditorToolkit.Search;
using Sasd.EditorToolkit.Text;
using Xunit;

namespace Sasd.EditorToolkit.Core.Tests;

public sealed class FindTextCommandTests
{
    [Fact]
    public async Task Execute_async_selects_found_text_and_moves_caret_to_match_end()
    {
        var document = new TextDocument(new LineTextBuffer("alpha beta gamma"));
        var viewState = new EditorViewState { CaretPosition = TextPosition.Start };
        var dispatcher = CreateM1Dispatcher();

        var result = await dispatcher.ExecuteAsync(
            EditorCommandIds.FindText,
            new EditorCommandContext(document, viewState, "beta"));

        Assert.True(result.Handled);
        Assert.Equal(new TextPosition(0, 6), viewState.Selection.Anchor);
        Assert.Equal(new TextPosition(0, 10), viewState.Selection.Active);
        Assert.Equal(new TextPosition(0, 10), viewState.CaretPosition);
        Assert.Contains("Found at line 1, column 7", result.Message ?? string.Empty);
    }

    [Fact]
    public async Task Execute_async_accepts_search_request_options()
    {
        var document = new TextDocument(new LineTextBuffer("alpha beta gamma beta"));
        var viewState = new EditorViewState { CaretPosition = new TextPosition(0, 16) };
        var dispatcher = CreateM1Dispatcher();
        var request = new SearchRequest(
            "beta",
            new TextSearchOptions(Direction: SearchDirection.Backward));

        var result = await dispatcher.ExecuteAsync(
            EditorCommandIds.FindText,
            new EditorCommandContext(document, viewState, request));

        Assert.True(result.Handled);
        Assert.Equal(new TextPosition(0, 6), viewState.Selection.Anchor);
        Assert.Equal(new TextPosition(0, 10), viewState.Selection.Active);
    }

    [Fact]
    public async Task Execute_async_returns_not_handled_when_text_is_not_found()
    {
        var document = new TextDocument(new LineTextBuffer("alpha beta"));
        var viewState = new EditorViewState { CaretPosition = TextPosition.Start };
        var dispatcher = CreateM1Dispatcher();

        var result = await dispatcher.ExecuteAsync(
            EditorCommandIds.FindText,
            new EditorCommandContext(document, viewState, "missing"));

        Assert.False(result.Handled);
        Assert.Contains("Text not found", result.Message ?? string.Empty);
        Assert.True(viewState.Selection.IsEmpty);
        Assert.Equal(TextPosition.Start, viewState.CaretPosition);
    }

    private static EditorCommandDispatcher CreateM1Dispatcher()
    {
        var registry = new EditorCommandRegistry();
        registry.RegisterM1Defaults();
        return new EditorCommandDispatcher(registry);
    }
}
