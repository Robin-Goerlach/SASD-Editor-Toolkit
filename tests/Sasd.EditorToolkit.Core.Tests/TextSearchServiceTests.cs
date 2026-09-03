using Sasd.EditorToolkit.Search;
using Sasd.EditorToolkit.Text;
using Xunit;

namespace Sasd.EditorToolkit.Core.Tests;

public sealed class TextSearchServiceTests
{
    [Fact]
    public void Find_next_finds_forward_match_from_position()
    {
        var buffer = new LineTextBuffer("alpha beta gamma beta");
        var service = new TextSearchService();

        var match = service.FindNext(buffer, new TextPosition(0, 7), "beta");

        Assert.NotNull(match);
        Assert.Equal(new TextPosition(0, 17), match.Range.Start);
        Assert.Equal(new TextPosition(0, 21), match.Range.End);
        Assert.Equal("beta", match.Value);
    }

    [Fact]
    public void Find_next_wraps_forward_when_requested()
    {
        var buffer = new LineTextBuffer("alpha beta gamma");
        var service = new TextSearchService();

        var match = service.FindNext(buffer, new TextPosition(0, 12), "beta");

        Assert.NotNull(match);
        Assert.Equal(new TextPosition(0, 6), match.Range.Start);
    }

    [Fact]
    public void Find_next_can_search_without_wrapping()
    {
        var buffer = new LineTextBuffer("alpha beta gamma");
        var service = new TextSearchService();
        var options = new TextSearchOptions(Wrap: false);

        var match = service.FindNext(buffer, new TextPosition(0, 12), "beta", options);

        Assert.Null(match);
    }

    [Fact]
    public void Find_next_can_search_backward()
    {
        var buffer = new LineTextBuffer("alpha beta gamma beta");
        var service = new TextSearchService();
        var options = new TextSearchOptions(Direction: SearchDirection.Backward);

        var match = service.FindNext(buffer, new TextPosition(0, 16), "beta", options);

        Assert.NotNull(match);
        Assert.Equal(new TextPosition(0, 6), match.Range.Start);
    }

    [Fact]
    public void Find_next_honors_case_sensitive_comparison()
    {
        var buffer = new LineTextBuffer("Alpha alpha");
        var service = new TextSearchService();
        var options = new TextSearchOptions(StringComparison.Ordinal, SearchDirection.Forward, Wrap: true);

        var match = service.FindNext(buffer, TextPosition.Start, "alpha", options);

        Assert.NotNull(match);
        Assert.Equal(new TextPosition(0, 6), match.Range.Start);
    }

    [Fact]
    public void Find_next_returns_null_for_empty_search_text()
    {
        var buffer = new LineTextBuffer("alpha");
        var service = new TextSearchService();

        var match = service.FindNext(buffer, TextPosition.Start, string.Empty);

        Assert.Null(match);
    }

    [Fact]
    public void Find_next_can_return_match_across_line_endings()
    {
        var buffer = new LineTextBuffer("alpha\r\nbeta");
        var service = new TextSearchService();

        var match = service.FindNext(buffer, TextPosition.Start, "ha\r\nbe");

        Assert.NotNull(match);
        Assert.Equal(new TextPosition(0, 3), match.Range.Start);
        Assert.Equal(new TextPosition(1, 2), match.Range.End);
    }
}
