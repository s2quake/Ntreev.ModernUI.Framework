using FirstFloor.ModernUI.Windows.Controls;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.Controls;
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
    public partial class ShellView : Window
    {
        public ShellView()
        {
            this.InitializeComponent();
            var ss = 1 | 2 | 4;
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
    }
}