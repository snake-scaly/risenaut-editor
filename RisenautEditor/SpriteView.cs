using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RisenautEditor
{
    class SpriteView : Control
    {
        private ImageSource image_source;

        public static readonly DependencyProperty SpriteDependencyProperty =
            DependencyProperty.Register("Sprite", typeof(Sprite), typeof(SpriteView),
            new PropertyMetadata(OnSpriteDependencyPropertyChanged));

        public Sprite Sprite
        {
            get { return (Sprite)GetValue(SpriteDependencyProperty); }
            set { SetValue(SpriteDependencyProperty, value); }
        }

        private static void OnSpriteDependencyPropertyChanged(
            DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((SpriteView)sender).UpdateSprite();
        }

        private void UpdateSprite()
        {
            int width = Sprite.PixelWidth;
            int height = Sprite.PixelHeight;
            int pitch = width * 3;
            byte[] pixels = new byte[pitch * height];
            var canvas = new Bgr24Canvas(pixels, pitch, width, height);
            Sprite.Draw(canvas, 0, 0);

            var bmp = new WriteableBitmap(Sprite.PixelWidth, Sprite.PixelHeight, 96, 96, PixelFormats.Bgr24, null);
            bmp.Lock();
            bmp.WritePixels(new Int32Rect(0, 0, width, height), pixels, pitch, 0);
            bmp.Unlock();

            image_source = bmp;
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            drawingContext.DrawImage(image_source, new Rect(0, 0, ActualWidth, ActualHeight));
        }
    }
}
