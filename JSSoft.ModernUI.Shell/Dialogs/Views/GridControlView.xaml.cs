using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace JSSoft.ModernUI.Shell.Dialogs.Views
{
    /// <summary>
    /// GridControlView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class GridControlView : UserControl
    {
        public GridControlView()
        {
            InitializeComponent();
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.OK.IsEnabled == true && e.ChangedButton == MouseButton.Left)
            {
                this.OK.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, this.OK));
            }
        }
    }
}
