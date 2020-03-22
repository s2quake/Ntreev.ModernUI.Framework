//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ntreev.ModernUI.Framework.Views
{
    [TemplatePart(Name = "PART_OK", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Yes", Type = typeof(Button))]
    [TemplatePart(Name = "PART_No", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Cancel", Type = typeof(Button))]
    public class MessageBoxView1 : Control
    {
        public static readonly DependencyProperty MessageProperty = 
            DependencyProperty.Register(nameof(Message), typeof(string), typeof(MessageBoxView1));

        public static readonly DependencyProperty ImageProperty = 
            DependencyProperty.Register(nameof(Image), typeof(MessageBoxImage), typeof(MessageBoxView1));

        public static readonly DependencyProperty ButtonProperty = 
            DependencyProperty.Register(nameof(Button), typeof(MessageBoxButton), typeof(MessageBoxView1));

        public static readonly DependencyProperty ResultProperty = 
            DependencyProperty.Register(nameof(Result), typeof(MessageBoxResult), typeof(MessageBoxView1));

        public static readonly DependencyProperty ErrorContentProperty =
            DependencyProperty.Register(nameof(ErrorContent), typeof(object), typeof(MessageBoxView1));

        public MessageBoxView1()
        {
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, (s, e) =>
            {
                Clipboard.SetText(this.Message);
            }));

            this.Loaded += MessageBoxView_Loaded;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.Template.FindName("PART_OK", this) is Button okButton)
            {
                okButton.Click += (s, e) => this.Result = MessageBoxResult.OK;
            }

            if (this.Template.FindName("PART_Yes", this) is Button yesButton)
            {
                yesButton.Click += (s, e) => this.Result = MessageBoxResult.Yes;
            }

            if (this.Template.FindName("PART_No", this) is Button noButton)
            {
                noButton.Click += (s, e) => this.Result = MessageBoxResult.No;
            }

            if (this.Template.FindName("PART_Cancel", this) is Button cancelButton)
            {
                cancelButton.Click += (s, e) => this.Result = MessageBoxResult.Cancel;
            }
        }

        public string Message
        {
            get => (string)this.GetValue(MessageProperty);
            set => this.SetValue(MessageProperty, value);
        }

        public MessageBoxImage Image
        {
            get => (MessageBoxImage)this.GetValue(ImageProperty);
            set => this.SetValue(ImageProperty, value);
        }

        public MessageBoxResult Result
        {
            get => (MessageBoxResult)this.GetValue(ResultProperty);
            set => this.SetValue(ResultProperty, value);
        }

        public MessageBoxButton Button
        {
            get => (MessageBoxButton)this.GetValue(ButtonProperty);
            set => this.SetValue(ButtonProperty, value);
        }

        public object ErrorContent
        {
            get => (object)this.GetValue(ErrorContentProperty);
            set => this.SetValue(ErrorContentProperty, value);
        }

        private void MessageBoxView_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Template.FindName("PART_OK", this) is Button okButton)
            {
                if (okButton.IsVisible == true)
                {
                    okButton.Focus();
                    return;
                }
            }

            if (this.Template.FindName("PART_Yes", this) is Button yesButton)
            {
                if (yesButton.IsVisible == true)
                {
                    yesButton.Focus();
                    return;
                }
            }
        }
    }
}
