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

using Ntreev.ModernUI.Framework.Properties;
using Ntreev.ModernUI.Framework.ViewModels;
using System;
using System.Windows;

namespace Ntreev.ModernUI.Framework
{
    public delegate string MessageSelector(object message);

    public static class AppMessageBox
    {
        public readonly static MessageSelector DefaultMessageSelector = SelectDefaultMessage;

        public static MessageBoxResult Show(string text)
        {
            return Show(text, MessageBoxButton.OK, MessageBoxImage.None);
        }

        public static MessageBoxResult Show(string text, string title)
        {
            return Show(text, title, MessageBoxButton.OK, MessageBoxImage.None);
        }

        public static void ShowError(string text)
        {
            Show(text, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowError(Exception e)
        {
            Show(e, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowError(string format, params object[] args)
        {
            ShowError(string.Format(format, args));
        }

        public static void ShowInfo(string text)
        {
            Show(text, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ShowInfo(string format, params object[] args)
        {
            ShowInfo(string.Format(format, args));
        }

        public static void ShowWarning(string text)
        {
            Show(text, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static void ShowWarning(string format, params object[] args)
        {
            ShowWarning(string.Format(format, args));
        }

        public static bool ShowQuestion(string text)
        {
            return Show(text, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        public static bool ShowQuestion(string format, params object[] args)
        {
            return ShowQuestion(string.Format(format, args));
        }

        public static bool? ShowCancelableQuestion(string text)
        {
            var result = Show(text, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
                return true;
            else if (result == MessageBoxResult.No)
                return false;
            else
                return null;
        }

        public static bool? ShowCancelableQuestion(string format, params object[] args)
        {
            return ShowCancelableQuestion(string.Format(format, args));
        }

        public static bool ShowProceed(string text)
        {
            return Show(text, MessageBoxButton.OKCancel, MessageBoxImage.Exclamation) == MessageBoxResult.OK;
        }

        public static bool ShowProceed(string format, params object[] args)
        {
            return ShowProceed(string.Format(format, args));
        }

        public static bool ConfirmDelete()
        {
            return ShowQuestion(Resources.Message_ConfirmToDelete);
        }

        public static bool? ConfirmSaveOnClosing()
        {
            return ShowCancelableQuestion(Resources.Message_ConfirmToSave);
        }

        public static bool? ConfirmCreateOnClosing()
        {
            return ShowCancelableQuestion(Resources.Message_ConfirmToCreate);
        }

        public static MessageBoxResult Show(object message, MessageBoxButton button, MessageBoxImage icon)
        {
            return Show(message, AppInfo.ProductName, button, icon);
        }

        public static MessageBoxResult Show(object message, string title, MessageBoxButton button, MessageBoxImage icon)
        {
            Application.Current.Dispatcher.VerifyAccess();

            var text = (MessageSelector ?? SelectDefaultMessage)(message);

            if (AppWindowManager.Current == null)
            {
                return MessageBox.Show(text, title, button, icon);
            }
            else
            {
                var dialog = new MessageBoxViewModel()
                {
                    DisplayName = title,
                    Message = text,
                    Button = button,
                    Image = icon
                };
                AppWindowManager.Current.ShowDialogAsync(dialog).Wait();
                return dialog.Result;
            }
        }

        private static string SelectDefaultMessage(object message)
        {
            if (message is Exception exception)
            {
                return exception.Message;
            }
            return message.ToString();
        }

        public static MessageSelector MessageSelector
        {
            get; set;
        }
    }
}
