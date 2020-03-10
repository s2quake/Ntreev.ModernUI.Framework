using FirstFloor.ModernUI.Windows.Controls;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.Controls;
using Ntreev.ModernUI.Framework.Dialogs.ViewModels;
using Ntreev.ModernUI.Framework.Dialogs.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            this.PickColor.Background = new SolidColorBrush(FirstFloor.ModernUI.Presentation.AppearanceManager.Current.AccentColor);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Editor_Executed(object sender, RoutedEventArgs e)
        {
            if (this.editor.Text == "reset")
            {
                this.editor.Reset();
            }
            else
            {
                this.editor.Append("executed: ");
                var oldForeground = this.editor.OutputForeground;
                this.editor.OutputForeground = Brushes.Red;
                this.editor.AppendLine(this.editor.Text);
                this.editor.OutputForeground = oldForeground;
            }
        }

        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                if (this.editor.ApplyTemplate() == true)
                {
                    this.editor.Focus();
                    this.editor.AppendLine("안녕하세요.");
                    this.editor.Prompt = "c:>";
                }
            });
        }

        private void PickColor_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SelectColorViewModel()
            {
                CurrentColor = FirstFloor.ModernUI.Presentation.AppearanceManager.Current.AccentColor,
            };
            if (dialog.ShowDialog() == true)
            {
                FirstFloor.ModernUI.Presentation.AppearanceManager.Current.AccentColor = dialog.CurrentColor;
                this.PickColor.Background = new SolidColorBrush(FirstFloor.ModernUI.Presentation.AppearanceManager.Current.AccentColor);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            AppMessageBox.Show("123");
        }
    }
}