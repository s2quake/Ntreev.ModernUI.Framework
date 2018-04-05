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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Ntreev.ModernUI.Framework.Controls
{
    [TemplatePart(Name = "PART_TextBox", Type = typeof(RichTextBox))]
    public class TerminalControl : Control
    {
        public readonly static DependencyProperty PromptProperty =
            DependencyProperty.Register(nameof(Prompt), typeof(string), typeof(TerminalControl),
                new PropertyMetadata(string.Empty, PromptPropertyChangedCallback, PromptPropertyCoerceValueCallback));

        private readonly static DependencyPropertyKey TextPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(Text), typeof(string), typeof(TerminalControl),
                new PropertyMetadata(string.Empty));
        public readonly static DependencyProperty TextProperty = TextPropertyKey.DependencyProperty;

        public readonly static DependencyProperty OutputForegroundProperty =
            DependencyProperty.Register(nameof(OutputForeground), typeof(Brush), typeof(TerminalControl), 
                new FrameworkPropertyMetadata(OutputForegroundPropertyChangedCallback));

        public readonly static DependencyProperty OutputBackgroundProperty =
            DependencyProperty.Register(nameof(OutputBackground), typeof(Brush), typeof(TerminalControl),
                new FrameworkPropertyMetadata(OutputBackgroundPropertyChangedCallback));

        private RichTextBox textBox;

        private readonly List<string> histories = new List<string>();
        private readonly List<string> completions = new List<string>();
        private int historyIndex;

        private Paragraph promptBlock;
        private Run prompt;
        private Run command;
        private Run output;
        //private Paragraph outputBlock;
        private string promptText;
        private string inputText = string.Empty;
        private int refStack = 0;
        private string completion;

        private bool isChanged;

        public readonly static RoutedEvent ExecutedEvent =
            EventManager.RegisterRoutedEvent(nameof(Executed), RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(TerminalControl));

        public event RoutedEventHandler Executed
        {
            add { AddHandler(ExecutedEvent, value); }
            remove { RemoveHandler(ExecutedEvent, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.textBox = this.Template.FindName("PART_TextBox", this) as RichTextBox;

            if (this.textBox != null)
            {
                this.prompt = new Run();
                this.command = new Run();
                this.promptBlock = new Paragraph();
                this.promptBlock.Inlines.Add(this.prompt);
                this.promptBlock.Inlines.Add(this.command);
                this.textBox.Document.Blocks.Clear();
                this.textBox.Document.Blocks.Add(this.promptBlock);
                this.textBox.CaretPosition = this.textBox.Document.ContentEnd;
                this.textBox.TextChanged += TextBox_TextChanged;
                this.textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
                this.textBox.Selection.Changed += TextBox_Selection_Changed;
            }
        }

        public void Execute()
        {
            this.refStack++;
            try
            {
                this.SetValue(TextPropertyKey, this.command.Text);
                if (this.histories.Contains(this.Text) == false)
                {
                    this.histories.Add(this.Text);
                    this.historyIndex = this.histories.Count;
                }
                else
                {
                    this.historyIndex = this.histories.LastIndexOf(this.Text) + 1;
                }

                this.AppendLine(this.prompt.Text + this.command.Text);
                //this.outputBlock = this.promptBlock;
                this.command.Text = string.Empty;
                this.textBox.CaretPosition = this.command.ContentEnd;
                this.inputText = string.Empty;
                this.completion = string.Empty;
                this.OnExecuted(new RoutedEventArgs(ExecutedEvent));
                this.textBox.CaretPosition = this.command.ContentEnd;
            }
            finally
            {
                this.refStack--;
            }
        }

        public void Clear()
        {
            this.textBox.Selection.Select(this.prompt.ContentEnd, this.textBox.Document.ContentEnd);
            this.textBox.Selection.Text = string.Empty;
        }

        public void MoveToFirst()
        {
            this.textBox.CaretPosition = this.prompt.ContentEnd;
        }

        public void Reset()
        {
            this.output = null;
            this.promptBlock.Inlines.Clear();
            this.promptBlock.Inlines.Add(this.prompt);
            this.promptBlock.Inlines.Add(this.command);
            this.textBox.CaretPosition = this.command.ContentEnd;
        }

        public void Append(string text)
        {
            this.AppendInternal(text);
        }

        public void AppendLine(string text)
        {
            this.AppendInternal(text + Environment.NewLine);
            //using (var sr = new StringReader(text))
            //{
            //    var line = string.Empty;
            //    var count = 0;
            //    while ((line = sr.ReadLine()) != null)
            //    {
            //        if (count > 0)
            //        {
            //            this.InsertNewLine();
            //        }
            //        this.AppendInternal(line);
            //        count++;
            //    }
            //}
            //this.InsertNewLine();
        }

        public string Text => (string)this.GetValue(TextProperty);

        public string Prompt
        {
            get { return (string)this.GetValue(PromptProperty); }
            set { this.SetValue(PromptProperty, value); }
        }

        public Brush OutputForeground
        {
            get { return (Brush)this.GetValue(OutputForegroundProperty); }
            set { this.SetValue(OutputForegroundProperty, value); }
        }

        public Brush OutputBackground
        {
            get { return (Brush)this.GetValue(OutputBackgroundProperty); }
            set { this.SetValue(OutputBackgroundProperty, value); }
        }

        public void NextCompletion()
        {
            this.refStack++;
            try
            {
                this.CompletionImpl(NextCompletion);
                this.textBox.CaretPosition = this.command.ContentEnd;
            }
            finally
            {
                this.refStack--;
            }
        }

        public void PrevCompletion()
        {
            this.refStack++;
            try
            {
                this.CompletionImpl(PrevCompletion);
                this.textBox.CaretPosition = this.command.ContentEnd;
            }
            finally
            {
                this.refStack--;
            }
        }

        public void NextHistory()
        {
            if (this.historyIndex + 1 < this.histories.Count)
            {
                this.command.Text = this.histories[this.historyIndex + 1];
                this.historyIndex++;
            }
        }

        public void PrevHistory()
        {
            if (this.historyIndex > 0)
            {
                this.command.Text = this.histories[this.historyIndex - 1];
                this.historyIndex--;
            }
            else if (this.histories.Count == 1)
            {
                this.command.Text = this.histories[0];
                this.historyIndex = 0;
            }
        }

        public static Match[] MatchCompletion(string text)
        {
            var matches = Regex.Matches(text, "\\S+");
            var argList = new List<Match>();

            foreach (Match item in matches)
            {
                argList.Add(item);
            }

            return argList.ToArray();
        }

        public static string NextCompletion(string[] completions, string text)
        {
            completions = completions.OrderBy(item => item).ToArray();
            if (completions.Contains(text) == true)
            {
                for (var i = 0; i < completions.Length; i++)
                {
                    var r = string.Compare(text, completions[i], true);
                    if (r == 0)
                    {
                        if (i + 1 < completions.Length)
                            return completions[i + 1];
                        else
                            return completions.First();
                    }
                }
            }
            else
            {
                for (var i = 0; i < completions.Length; i++)
                {
                    var r = string.Compare(text, completions[i], true);
                    if (r < 0)
                    {
                        return completions[i];
                    }
                }
            }
            return text;
        }

        public static string PrevCompletion(string[] completions, string text)
        {
            completions = completions.OrderBy(item => item).ToArray();
            if (completions.Contains(text) == true)
            {
                for (var i = completions.Length - 1; i >= 0; i--)
                {
                    var r = string.Compare(text, completions[i], true);
                    if (r == 0)
                    {
                        if (i - 1 >= 0)
                            return completions[i - 1];
                        else
                            return completions.Last();
                    }
                }
            }
            else
            {
                for (var i = completions.Length - 1; i >= 0; i--)
                {
                    var r = string.Compare(text, completions[i], true);
                    if (r < 0)
                    {
                        return completions[i];
                    }
                }
            }
            return text;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            this.textBox?.Focus();
        }

        protected virtual void OnExecuted(RoutedEventArgs e)
        {
            this.RaiseEvent(e);
        }

        protected virtual string[] GetCompletion(string[] items, string find)
        {
            var query = from item in this.completions
                        where item.StartsWith(find)
                        select item;
            return query.ToArray();
        }

        private void CompletionImpl(Func<string[], string, string> func)
        {
            var matches = new List<Match>(CommandStringUtility.MatchCompletion(this.inputText));
            var find = string.Empty;
            var prefix = false;
            var postfix = false;
            var leftText = this.inputText;
            if (matches.Count > 0)
            {
                var match = matches.Last();
                var matchText = match.Value;
                if (matchText.Length > 0 && matchText.First() == '\"')
                {
                    prefix = true;
                    matchText = matchText.Substring(1);
                }
                if (matchText.Length > 1 && matchText.Last() == '\"')
                {
                    postfix = true;
                    matchText = matchText.Remove(matchText.Length - 1);
                }
                if (matchText == string.Empty || matchText.Trim() != string.Empty)
                {
                    find = matchText;
                    matches.RemoveAt(matches.Count - 1);
                    leftText = this.inputText.Remove(match.Index);
                }
            }

            var argList = new List<string>();
            for (var i = 0; i < matches.Count; i++)
            {
                var matchText = CommandStringUtility.TrimQuot(matches[i].Value).Trim();
                if (matchText != string.Empty)
                    argList.Add(matchText);
            }
            
            var completions = this.GetCompletion(argList.ToArray(), find);
            if (completions != null && completions.Any())
            {
                this.completion = func(completions, this.completion);
                var inputText = this.inputText;
                if (prefix == true || postfix == true)
                {
                    this.command.Text = leftText + "\"" + this.completion + "\"";
                }
                else
                {
                    this.command.Text = leftText + this.completion;
                }
                this.inputText = inputText;
            }
        }

        private static void PromptPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TerminalControl self)
            {
                self.SetPrompt(e.NewValue as string);
            }
        }

        private static object PromptPropertyCoerceValueCallback(DependencyObject d, object baseValue)
        {
            return baseValue ?? string.Empty;
        }

        private static void OutputForegroundPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TerminalControl self && self.textBox != null)
            {
                self.isChanged = true;
            }
        }

        private static void OutputBackgroundPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TerminalControl self && self.textBox != null)
            {
                self.isChanged = true;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.refStack > 0)
                return;

            if (this.promptBlock.Inlines.Contains(this.command) == false)
            {
                this.promptBlock.Inlines.Add(this.command);
                this.textBox.CaretPosition = this.command.ContentEnd;
            }

            this.inputText = this.command.Text;
            this.completion = string.Empty;
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Home)
            {
                if (Keyboard.Modifiers == ModifierKeys.None)
                {
                    e.Handled = true;
                    this.MoveToFirst();
                }
                else if (Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    e.Handled = true;
                    this.textBox.Selection.Select(this.command.ContentStart, this.textBox.Selection.Start);
                }
            }
            else if (e.Key == Key.Escape && Keyboard.Modifiers == ModifierKeys.None)
            {
                if (this.textBox.IsReadOnly == true)
                {
                    this.MoveToFirst();
                    e.Handled = true;
                }
                else
                {
                    //this.Clear();
                    //e.Handled = true;
                }
            }
            else if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.None)
            {
                if (this.textBox.IsReadOnly == false)
                {
                    try
                    {
                        this.Execute();
                    }
                    finally
                    {
                        e.Handled = true;
                    }
                }
            }
            else if (e.Key == Key.Tab)
            {
                if (Keyboard.Modifiers == ModifierKeys.None)
                {
                    e.Handled = true;
                    this.NextCompletion();
                }
                else if (Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    e.Handled = true;
                    this.PrevCompletion();
                }
            }
            else if (e.Key == Key.Back && Keyboard.Modifiers == ModifierKeys.None)
            {
                if (this.textBox.CaretPosition.CompareTo(this.command.ContentStart) <= 0)
                {
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Up && Keyboard.Modifiers == ModifierKeys.None)
            {
                if (this.textBox.IsReadOnly == false)
                {
                    this.PrevHistory();
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Down && Keyboard.Modifiers == ModifierKeys.None)
            {
                if (this.textBox.IsReadOnly == false)
                {
                    this.NextHistory();
                    e.Handled = true;
                }
            }
        }

        private void TextBox_Selection_Changed(object sender, EventArgs e)
        {
            if (this.promptBlock != this.textBox.CaretPosition.Paragraph)
            {
                this.textBox.IsReadOnly = true;
            }
            else
            {
                if (this.textBox.Selection.Start.CompareTo(this.prompt.ContentEnd) < 0 || this.textBox.Selection.End.CompareTo(this.prompt.ContentEnd) < 0)
                {
                    this.textBox.IsReadOnly = true;
                }
                else
                {
                    this.textBox.IsReadOnly = false;
                }
            }

            if (this.refStack == 0)
            {
                this.inputText = this.command.Text;
            }
        }

        private void SetPrompt(string prompt)
        {
            if (this.textBox != null)
            {
                this.refStack++;
                this.promptText = prompt;
                this.prompt.Text = prompt;
                this.refStack--;
            }
        }

        private void AppendInternal(string text)
        {
            this.refStack++;

            if (this.output == null || this.isChanged == true)
            {
                var oldOutput = this.output;
                this.output = new Run
                {
                    Foreground = this.OutputForeground,
                    Background = this.OutputBackground
                };
                if (oldOutput == null)
                {
                    this.promptBlock.Inlines.InsertBefore(this.prompt, this.output);
                }
                else
                {
                    this.promptBlock.Inlines.InsertAfter(oldOutput, this.output);
                }
            }

            this.output.Text += text;

            this.refStack--;
        }
    }
}
