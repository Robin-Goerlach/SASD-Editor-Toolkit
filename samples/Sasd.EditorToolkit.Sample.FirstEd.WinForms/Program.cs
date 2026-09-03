using System.Windows.Forms;
namespace Sasd.EditorToolkit.Sample.FirstEd.WinForms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
