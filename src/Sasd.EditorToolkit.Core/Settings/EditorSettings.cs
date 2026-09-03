namespace Sasd.EditorToolkit.Settings;

/// <summary>Portable editor settings seed.</summary>
public sealed class EditorSettings
{
    /// <summary>Gets or sets the tab size.</summary>
    public int TabSize { get; set; } = 4;

    /// <summary>Gets or sets whether line numbers should be shown by default.</summary>
    public bool ShowLineNumbers { get; set; } = true;

    /// <summary>Gets or sets whether visual word wrap is enabled by default.</summary>
    public bool WordWrap { get; set; }

    /// <summary>Gets or sets the keyboard profile name.</summary>
    public string KeyboardProfile { get; set; } = "Modern";
}
