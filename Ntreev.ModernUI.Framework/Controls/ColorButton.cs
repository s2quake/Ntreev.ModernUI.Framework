using Ntreev.ModernUI.Framework.Dialogs.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ntreev.ModernUI.Framework.Controls
{
    public class ColorButton : Button
    {
        public static readonly DependencyProperty ValueProperty =
           DependencyProperty.Register(nameof(Value), typeof(Color), typeof(ColorButton),
               new FrameworkPropertyMetadata(Colors.Red, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ValuePropertyChangedCallback));

        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent(nameof(ValueChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ColorButton));

        public Color Value
        {
            get => (Color)this.GetValue(ValueProperty);
            set => this.SetValue(ValueProperty, value);
        }

        public event RoutedEventHandler ValueChanged
        {
            add { this.AddHandler(ValueChangedEvent, value); }
            remove { this.RemoveHandler(ValueChangedEvent, value); }
        }

        protected override async void OnClick()
        {
            base.OnClick();
            var dialog = new SelectColorViewModel();
            if (await dialog.ShowDialogAsync() == true)
            {
                this.Value = dialog.CurrentColor;
            }
        }

        private static void ValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorButton self)
            {
                self.RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SelectColorViewModel();
            if (await dialog.ShowDialogAsync() == true)
            {
                this.Value = dialog.CurrentColor;
            }
        }
    }
}
