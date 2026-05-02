using SW = System.Windows;

namespace GpuSpoofer;

public partial class WhyDialog : SW.Window
{
    public WhyDialog()
    {
        InitializeComponent();
        Owner = SW.Application.Current.MainWindow;
    }

    private void Close_Click(object sender, SW.RoutedEventArgs e)
    {
        Close();
    }
}
