namespace Sasd.EditorToolkit.Text;

/// <summary>Describes the line terminator stored after a logical line.</summary>
public enum LineEndingKind
{
    /// <summary>No terminator, normally used for the final line.</summary>
    None = 0,

    /// <summary>Windows style CRLF terminator.</summary>
    CrLf = 1,

    /// <summary>Unix style LF terminator.</summary>
    Lf = 2,

    /// <summary>Classic Mac style CR terminator.</summary>
    Cr = 3
}
