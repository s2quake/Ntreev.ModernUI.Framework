using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xceed.Wpf.Toolkit;

namespace Ntreev.ModernUI.Framework.DataGrid.Controls
{
    public class NumericTextBox : Xceed.Wpf.Toolkit.Primitives.ValueRangeTextBox
    {
        public static readonly DependencyProperty NumberStylesProperty =
            DependencyProperty.Register(nameof(NumberStyles), typeof(NumberStyles), typeof(NumericTextBox));

        public NumberStyles NumberStyles
        {
            get { return (NumberStyles)this.GetValue(NumberStylesProperty); }
            set { this.SetValue(NumberStylesProperty, value); }
        }
    }
}
