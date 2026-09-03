using Sasd.EditorToolkit.Search;

namespace Sasd.EditorToolkit.Commands;

/// <summary>
/// Finds text in the current document and selects the found range in the active view state.
/// </summary>
public sealed class FindTextCommand : IEditorCommand
{
    private readonly TextSearchService _searchService;

    /// <summary>Creates a command using the default text search service.</summary>
    public FindTextCommand() : this(new TextSearchService())
    {
    }

    /// <summary>Creates a command with an explicitly supplied search service.</summary>
    public FindTextCommand(TextSearchService searchService)
    {
        _searchService = searchService;
    }

    /// <inheritdoc />
    public EditorCommandId Id => EditorCommandIds.FindText;

    /// <inheritdoc />
    public bool CanExecute(EditorCommandContext context) => TryCreateRequest(context.Parameter, out _);

    /// <inheritdoc />
    public ValueTask<CommandResult> ExecuteAsync(EditorCommandContext context, CancellationToken cancellationToken = default)
    {
        if (!TryCreateRequest(context.Parameter, out var request))
        {
            return ValueTask.FromResult(CommandResult.NotHandled("Search text is missing."));
        }

        var match = _searchService.FindNext(
            context.Document.Buffer,
            context.ViewState.CaretPosition,
            request.SearchText,
            request.Options);

        if (match is null)
        {
            return ValueTask.FromResult(CommandResult.NotHandled($"Text not found: {request.SearchText}"));
        }

        // Keep selection state in Core so every UI adapter gets the same behavior.
        context.ViewState.Selection.Anchor = match.Range.Start;
        context.ViewState.Selection.Active = match.Range.End;
        context.ViewState.CaretPosition = match.Range.End;

        var line = match.Range.Start.Line + 1;
        var column = match.Range.Start.Column + 1;
        return ValueTask.FromResult(CommandResult.Success($"Found at line {line}, column {column}."));
    }

    private static bool TryCreateRequest(object? parameter, out SearchRequest request)
    {
        if (parameter is SearchRequest searchRequest && !string.IsNullOrEmpty(searchRequest.SearchText))
        {
            request = searchRequest;
            return true;
        }

        if (parameter is string searchText && !string.IsNullOrEmpty(searchText))
        {
            request = new SearchRequest(searchText);
            return true;
        }

        request = new SearchRequest(string.Empty);
        return false;
    }
}
