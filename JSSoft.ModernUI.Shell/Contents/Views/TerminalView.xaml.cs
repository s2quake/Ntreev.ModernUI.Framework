using System.Windows;
using System.Windows.Controls;

namespace JSSoft.ModernUI.Shell.Contents.Views
{
    /// <summary>
    /// TerminalView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TerminalView : UserControl
    {
        public TerminalView()
        {
            InitializeComponent();
        }

        private void Editor_Executed(object sender, RoutedEventArgs e)
        {

        }

        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                //if (this.editor.ApplyTemplate() == true)
                {
                    this.Editor.Focus();
                    this.Editor.AppendLine("안녕하세요.");
                    this.Editor.Prompt = "c:> ";
                }
            });
        }
    }
}
