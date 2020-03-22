using FirstFloor.ModernUI.Windows.Controls;
using Ntreev.ModernUI.Framework;
using System.Windows;

namespace Ntreev.ModernUI.Shell
{
    /// <summary>
    /// ShellView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ShellView : ModernWindow
    {
        public ShellView()
        {
            this.InitializeComponent();
            //this.PickColor.Background = new SolidColorBrush(FirstFloor.ModernUI.Presentation.AppearanceManager.Current.AccentColor);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Editor_Executed(object sender, RoutedEventArgs e)
        {
            //if (this.editor.Text == "reset")
            //{
            //    this.editor.Reset();
            //}
            //else
            //{
            //    this.editor.Append("executed: ");
            //    var oldForeground = this.editor.OutputForeground;
            //    this.editor.OutputForeground = Brushes.Red;
            //    this.editor.AppendLine(this.editor.Text);
            //    this.editor.OutputForeground = oldForeground;
            //}
        }

        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                if (this.editor.ApplyTemplate() == true)
                {
                    this.editor.Focus();
                    this.editor.AppendLine("안녕하세요.");
                    this.editor.Prompt = "c:> ";
                }
            });
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            AppMessageBox.ShowAsync("123");
        }
    }
}