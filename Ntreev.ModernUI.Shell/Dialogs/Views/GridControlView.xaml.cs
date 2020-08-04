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

namespace Ntreev.ModernUI.Shell.Dialogs.Views
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

        private void GridControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
