﻿// Released under the MIT License.
// 
// Copyright (c) 2018 Ntreev Soft co., Ltd.
// Copyright (c) 2020 Jeesu Choi
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the
// Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Forked from https://github.com/NtreevSoft/Ntreev.ModernUI.Framework
// Namespaces and files starting with "Ntreev" have been renamed to "JSSoft".

using System;
using System.Windows;

namespace JSSoft.ModernUI.Framework.Controls
{
    public static class TerminalColors
    {
        public readonly static ComponentResourceKey ForegroundKey = new(typeof(TerminalControl), nameof(ForegroundKey));
        public readonly static ComponentResourceKey BackgroundKey = new(typeof(TerminalControl), nameof(BackgroundKey));

        public readonly static ComponentResourceKey BlackForegroundKey = new(typeof(TerminalControl), nameof(BlackForegroundKey));
        public readonly static ComponentResourceKey DarkBlueForegroundKey = new(typeof(TerminalControl), nameof(DarkBlueForegroundKey));
        public readonly static ComponentResourceKey DarkGreenForegroundKey = new(typeof(TerminalControl), nameof(DarkGreenForegroundKey));
        public readonly static ComponentResourceKey DarkCyanForegroundKey = new(typeof(TerminalControl), nameof(DarkCyanForegroundKey));
        public readonly static ComponentResourceKey DarkRedForegroundKey = new(typeof(TerminalControl), nameof(DarkRedForegroundKey));
        public readonly static ComponentResourceKey DarkMagentaForegroundKey = new(typeof(TerminalControl), nameof(DarkMagentaForegroundKey));
        public readonly static ComponentResourceKey DarkYellowForegroundKey = new(typeof(TerminalControl), nameof(DarkYellowForegroundKey));
        public readonly static ComponentResourceKey GrayForegroundKey = new(typeof(TerminalControl), nameof(GrayForegroundKey));
        public readonly static ComponentResourceKey DarkGrayForegroundKey = new(typeof(TerminalControl), nameof(DarkGrayForegroundKey));
        public readonly static ComponentResourceKey BlueForegroundKey = new(typeof(TerminalControl), nameof(BlueForegroundKey));
        public readonly static ComponentResourceKey GreenForegroundKey = new(typeof(TerminalControl), nameof(GreenForegroundKey));
        public readonly static ComponentResourceKey CyanForegroundKey = new(typeof(TerminalControl), nameof(CyanForegroundKey));
        public readonly static ComponentResourceKey RedForegroundKey = new(typeof(TerminalControl), nameof(RedForegroundKey));
        public readonly static ComponentResourceKey MagentaForegroundKey = new(typeof(TerminalControl), nameof(MagentaForegroundKey));
        public readonly static ComponentResourceKey YellowForegroundKey = new(typeof(TerminalControl), nameof(YellowForegroundKey));
        public readonly static ComponentResourceKey WhiteForegroundKey = new(typeof(TerminalControl), nameof(WhiteForegroundKey));

        public readonly static ComponentResourceKey BlackBackgroundKey = new(typeof(TerminalControl), nameof(BlackBackgroundKey));
        public readonly static ComponentResourceKey DarkBlueBackgroundKey = new(typeof(TerminalControl), nameof(DarkBlueBackgroundKey));
        public readonly static ComponentResourceKey DarkGreenBackgroundKey = new(typeof(TerminalControl), nameof(DarkGreenBackgroundKey));
        public readonly static ComponentResourceKey DarkCyanBackgroundKey = new(typeof(TerminalControl), nameof(DarkCyanBackgroundKey));
        public readonly static ComponentResourceKey DarkRedBackgroundKey = new(typeof(TerminalControl), nameof(DarkRedBackgroundKey));
        public readonly static ComponentResourceKey DarkMagentaBackgroundKey = new(typeof(TerminalControl), nameof(DarkMagentaBackgroundKey));
        public readonly static ComponentResourceKey DarkYellowBackgroundKey = new(typeof(TerminalControl), nameof(DarkYellowBackgroundKey));
        public readonly static ComponentResourceKey GrayBackgroundKey = new(typeof(TerminalControl), nameof(GrayBackgroundKey));
        public readonly static ComponentResourceKey DarkGrayBackgroundKey = new(typeof(TerminalControl), nameof(DarkGrayBackgroundKey));
        public readonly static ComponentResourceKey BlueBackgroundKey = new(typeof(TerminalControl), nameof(BlueBackgroundKey));
        public readonly static ComponentResourceKey GreenBackgroundKey = new(typeof(TerminalControl), nameof(GreenBackgroundKey));
        public readonly static ComponentResourceKey CyanBackgroundKey = new(typeof(TerminalControl), nameof(CyanBackgroundKey));
        public readonly static ComponentResourceKey RedBackgroundKey = new(typeof(TerminalControl), nameof(RedBackgroundKey));
        public readonly static ComponentResourceKey MagentaBackgroundKey = new(typeof(TerminalControl), nameof(MagentaBackgroundKey));
        public readonly static ComponentResourceKey YellowBackgroundKey = new(typeof(TerminalControl), nameof(YellowBackgroundKey));
        public readonly static ComponentResourceKey WhiteBackgroundKey = new(typeof(TerminalControl), nameof(WhiteBackgroundKey));

        public static ComponentResourceKey FindForegroundKey(ConsoleColor? consoleColor)
        {
            if (consoleColor.HasValue == false)
                return ForegroundKey;
            return consoleColor.Value switch
            {
                ConsoleColor.Black => BlackForegroundKey,
                ConsoleColor.DarkBlue => DarkBlueForegroundKey,
                ConsoleColor.DarkGreen => DarkGreenForegroundKey,
                ConsoleColor.DarkCyan => DarkCyanForegroundKey,
                ConsoleColor.DarkRed => DarkRedForegroundKey,
                ConsoleColor.DarkMagenta => DarkMagentaForegroundKey,
                ConsoleColor.DarkYellow => DarkYellowForegroundKey,
                ConsoleColor.Gray => GrayForegroundKey,
                ConsoleColor.DarkGray => DarkGrayForegroundKey,
                ConsoleColor.Blue => BlueForegroundKey,
                ConsoleColor.Green => GreenForegroundKey,
                ConsoleColor.Cyan => CyanForegroundKey,
                ConsoleColor.Red => RedForegroundKey,
                ConsoleColor.Magenta => MagentaForegroundKey,
                ConsoleColor.Yellow => YellowForegroundKey,
                ConsoleColor.White => WhiteForegroundKey,
                _ => throw new NotImplementedException(),
            };
        }

        public static ComponentResourceKey FindBackgroundKey(ConsoleColor? consoleColor)
        {
            if (consoleColor.HasValue == false)
                return BackgroundKey;
            return consoleColor.Value switch
            {
                ConsoleColor.Black => BlackBackgroundKey,
                ConsoleColor.DarkBlue => DarkBlueBackgroundKey,
                ConsoleColor.DarkGreen => DarkGreenBackgroundKey,
                ConsoleColor.DarkCyan => DarkCyanBackgroundKey,
                ConsoleColor.DarkRed => DarkRedBackgroundKey,
                ConsoleColor.DarkMagenta => DarkMagentaBackgroundKey,
                ConsoleColor.DarkYellow => DarkYellowBackgroundKey,
                ConsoleColor.Gray => GrayBackgroundKey,
                ConsoleColor.DarkGray => DarkGrayBackgroundKey,
                ConsoleColor.Blue => BlueBackgroundKey,
                ConsoleColor.Green => GreenBackgroundKey,
                ConsoleColor.Cyan => CyanBackgroundKey,
                ConsoleColor.Red => RedBackgroundKey,
                ConsoleColor.Magenta => MagentaBackgroundKey,
                ConsoleColor.Yellow => YellowBackgroundKey,
                ConsoleColor.White => WhiteBackgroundKey,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
