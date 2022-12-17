using HandyControl.Controls;

namespace WPF.Controls;

public partial class ReportView : Window
{
    public ReportView(object dataContext)
    {
        InitializeComponent();
        DataContext = dataContext;
    }
}