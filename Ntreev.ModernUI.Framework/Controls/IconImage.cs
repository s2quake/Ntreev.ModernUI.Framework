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

using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ntreev.ModernUI.Framework.Controls
{
    [TemplatePart(Name = PART_Image, Type = typeof(Image))]
    public class IconImage : Control
    {
        public const string PART_Image = nameof(PART_Image);

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(ImageSource), typeof(IconImage),
                new FrameworkPropertyMetadata(null, SourcePropertyChangedCallback));

        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register(nameof(Stretch), typeof(Stretch), typeof(IconImage),
                new FrameworkPropertyMetadata(Stretch.None, StretchPropertyChangedCallback));

        private static readonly Dictionary<string, WriteableBitmap> items = new Dictionary<string, WriteableBitmap>();

        private Image image;
        private WriteableBitmap imageSource;
        private bool isChanged = true;

        static IconImage()
        {

        }

        public IconImage()
        {

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.image = this.Template.FindName(PART_Image, this) as Image;
            if (this.image != null)
            {
                BindingOperations.SetBinding(this.image, Image.StretchProperty, new Binding(nameof(this.Stretch)) { Source = this, });
                this.image.Source = this.imageSource;
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (this.isChanged == true)
            {
                this.UpdateImage();
                this.isChanged = false;
            }
            base.OnRender(drawingContext);
        }

        public static IconImage FromUri(Uri uri)
        {
            return new IconImage() { Source = new BitmapImage(uri) };
        }

        public ImageSource Source
        {
            get => (ImageSource)this.GetValue(SourceProperty);
            set => this.SetValue(SourceProperty, value);
        }

        public Stretch Stretch
        {
            get => (Stretch)this.GetValue(StretchProperty);
            set => this.SetValue(StretchProperty, value);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property.Name == nameof(this.Foreground) || e.Property.Name == nameof(this.Background))
            {
                this.isChanged = true;
            }
        }

        protected override void OnPreviewMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            this.UpdateImage();
            return base.MeasureOverride(constraint);
        }

        private static void SourcePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as IconImage;
            self.InvalidateMeasure();
        }

        private static void StretchPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        private static byte[] BitmapSourceToArray(BitmapSource bitmapSource)
        {
            var stride = (int)bitmapSource.PixelWidth * (bitmapSource.Format.BitsPerPixel / 8);
            var pixels = new byte[(int)bitmapSource.PixelHeight * stride];
            bitmapSource.CopyPixels(pixels, stride, 0);
            return pixels;
        }

        private static WriteableBitmap ApplyColor(DependencyObject d, BitmapSource bitmapSource, Brush foreground, Brush background)
        {
            var hash = HashUtility.GetHashValue($"{bitmapSource}", $"{foreground}", $"{background}");
            if (items.ContainsKey(hash) == true)
                return items[hash];
            //System.Diagnostics.Trace.WriteLine($"{bitmapSource}, {forground}, {background}");
            var writableBitmap = new WriteableBitmap(bitmapSource);
            var foregroundColor = Colors.Black;
            var backgroundColor = Colors.White;
            if (foreground is SolidColorBrush == true)
            {
                foregroundColor = (foreground as SolidColorBrush).Color;
            }

            if (background is SolidColorBrush == true)
            {
                backgroundColor = (background as SolidColorBrush).Color;
            }

            var pixels = BitmapSourceToArray(writableBitmap);

            for (int i = 0; i < pixels.Length / 4; i++)
            {
                if (pixels[i * 4 + 0] == 0 &&
                    pixels[i * 4 + 1] == 0 &&
                    pixels[i * 4 + 2] == 0)
                {
                    pixels[i * 4 + 0] = foregroundColor.B;
                    pixels[i * 4 + 1] = foregroundColor.G;
                    pixels[i * 4 + 2] = foregroundColor.R;
                }
                else
                {
                    pixels[i * 4 + 0] = backgroundColor.B;
                    pixels[i * 4 + 1] = backgroundColor.G;
                    pixels[i * 4 + 2] = backgroundColor.R;
                }
            }

            var stride = (int)writableBitmap.PixelWidth * (writableBitmap.Format.BitsPerPixel / 8);
            writableBitmap.Lock();
            writableBitmap.WritePixels(new Int32Rect(0, 0, writableBitmap.PixelWidth, writableBitmap.PixelHeight), pixels, stride, 0);
            writableBitmap.Unlock();
            items.Add(hash, writableBitmap);
            return writableBitmap;
        }

        private void UpdateImage()
        {
            if (this.Source is BitmapSource == true)
            {
                this.imageSource = ApplyColor(this, this.Source as BitmapSource, this.Foreground, this.Background);
            }
            else
            {
                this.imageSource = null;
            }

            if (this.image != null)
            {
                this.image.Source = this.imageSource;
            }
        }
    }
}
