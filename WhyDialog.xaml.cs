using System.Windows;

namespace GpuSpoofer;

public partial class WhyDialog : Window
{
    public WhyDialog()
    {
        InitializeComponent();
        Owner = Application.Current.MainWindow;
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
