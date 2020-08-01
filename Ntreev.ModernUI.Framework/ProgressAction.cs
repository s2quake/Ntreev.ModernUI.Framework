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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Ntreev.ModernUI.Framework
{
    public class ProgressAction
    {
        private readonly IProgressable progressable;

        public ProgressAction(IProgressable progressable)
        {
            this.progressable = progressable;
        }

        public Func<Task> Try { get; set; }

        public Func<Exception, Task> Catch { get; set; }

        public Func<Task> Finally { get; set; }

        public string BeginMessage { get; set; } = string.Empty;

        public string EndMessage { get; set; } = string.Empty;

        public async Task RunAsync()
        {
            try
            {
                this.progressable.BeginProgress(this.BeginMessage);
                if (this.Try != null)
                    await this.Try.Invoke();
            }
            catch (Exception e)
            {
                await AppMessageBox.ShowErrorAsync(e);
                if (this.Catch != null)
                    await this.Catch.Invoke(e);
            }
            finally
            {
                if (this.Finally != null)
                    await this.Finally.Invoke();
                this.progressable.EndProgress(this.EndMessage);
            }
        }
    }
}
