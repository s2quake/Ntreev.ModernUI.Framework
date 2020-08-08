using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ntreev.ModernUI.Shell.Contents.Views
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
