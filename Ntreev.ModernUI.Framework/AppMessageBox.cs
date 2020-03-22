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
using System.Threading.Tasks;
using System.Windows;

namespace Ntreev.ModernUI.Framework
{
    public delegate string MessageSelector(object message);

    public static class AppMessageBox
    {
        public readonly static MessageSelector DefaultMessageSelector = SelectDefaultMessage;

        public static Task<MessageBoxResult> ShowAsync(string text)
        {
            return ShowAsync(text, MessageBoxButton.OK, MessageBoxImage.None);
        }

        public static Task<MessageBoxResult> ShowAsync(string text, string title)
        {
            return ShowAsync(text, title, MessageBoxButton.OK, MessageBoxImage.None);
        }

        public static Task ShowErrorAsync(string text)
        {
            return ShowAsync(text, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static Task ShowErrorAsync(Exception e)
        {
            return ShowAsync(e, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static Task ShowErrorAsync(string format, params object[] args)
        {
            return ShowErrorAsync(string.Format(format, args));
        }

        public static Task ShowInfoAsync(string text)
        {
            return ShowAsync(text, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static Task ShowInfoAsync(string format, params object[] args)
        {
            return ShowInfoAsync(string.Format(format, args));
        }

        public static Task ShowWarningAsync(string text)
        {
            return ShowAsync(text, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static Task ShowWarningAsync(string format, params object[] args)
        {
            return ShowWarningAsync(string.Format(format, args));
        }

        public static async Task<bool> ShowQuestionAsync(string text)
        {
            return await ShowAsync(text, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        public static Task<bool> ShowQuestion(string format, params object[] args)
        {
            return ShowQuestionAsync(string.Format(format, args));
        }

        public static async Task<bool?> ShowCancelableQuestionAsync(string text)
        {
            var result = await ShowAsync(text, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
                return true;
            else if (result == MessageBoxResult.No)
                return false;
            else
                return null;
        }

        public static Task<bool?> ShowCancelableQuestionAsync(string format, params object[] args)
        {
            return ShowCancelableQuestionAsync(string.Format(format, args));
        }

        public static async Task<bool> ShowProceedAsync(string text)
        {
            return await ShowAsync(text, MessageBoxButton.OKCancel, MessageBoxImage.Exclamation) == MessageBoxResult.OK;
        }

        public static Task<bool> ShowProceedAsync(string format, params object[] args)
        {
            return ShowProceedAsync(string.Format(format, args));
        }

        public static Task<bool> ConfirmDeleteAsync()
        {
            return ShowQuestionAsync(Resources.Message_ConfirmToDelete);
        }

        public static Task<bool?> ConfirmSaveOnClosingAsync()
        {
            return ShowCancelableQuestionAsync(Resources.Message_ConfirmToSave);
        }

        public static Task<bool?> ConfirmCreateOnClosingAsync()
        {
            return ShowCancelableQuestionAsync(Resources.Message_ConfirmToCreate);
        }

        public static Task<MessageBoxResult> ShowAsync(object message, MessageBoxButton button, MessageBoxImage icon)
        {
            return ShowAsync(message, AppInfo.ProductName, button, icon);
        }

        public static async Task<MessageBoxResult> ShowAsync(object message, string title, MessageBoxButton button, MessageBoxImage icon)
        {
            var text = (MessageSelector ?? SelectDefaultMessage)(message);
            if (AppWindowManager.Current == null)
            {
                return await Application.Current.Dispatcher.InvokeAsync(() => MessageBox.Show(text, title, button, icon));
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
                await AppWindowManager.Current.ShowDialogAsync(dialog);
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

        public static MessageSelector MessageSelector { get; set; }
    }
}
