using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Wms3pl.WpfClient.UILib.Views
{
    /// <summary>
    /// Interaction logic for WindowList.xaml
    /// </summary>
    public partial class WindowList : Window
    {
        public WindowList()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var q = from Window w in Application.Current.Windows
                    where w.GetType() != typeof (WindowList)
                    select new WindowInfo { Window = w, Image = w.GetJpgImage(0.5, 90) };
            lv.ItemsSource = q.ToList();
        }

        private void lv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var win = (lv.SelectedItem as WindowInfo).Window;

            if (!win.IsVisible)
            {
                win.Show();
            }

            if (win.WindowState == WindowState.Minimized)
            {
                win.WindowState = WindowState.Normal;
            }

            win.Activate();
            win.Topmost = true;
            win.Topmost = false;
            win.Focus();
            this.Close();
        }
    }

    public class WindowInfo
    {
        public Window Window { get; set; }
        public ImageSource Image { get; set; }
    }

    public static class Screenshot
    {
        /// <summary>
        /// Gets a JPG "screenshot" of the current UIElement
        /// </summary>
        /// <param name="source">UIElement to screenshot</param>
        /// <param name="scale">Scale to render the screenshot</param>
        /// <param name="quality">JPG Quality</param>
        /// <returns>Byte array of JPG data</returns>
        public static ImageSource GetJpgImage(this UIElement source, double scale, int quality)
        {
            double actualHeight = source.RenderSize.Height;
            double actualWidth = source.RenderSize.Width;

            double renderHeight = Math.Max(actualHeight * scale,200);
            double renderWidth = Math.Max(actualWidth * scale, 190);


            RenderTargetBitmap renderTarget = new RenderTargetBitmap((int)renderWidth, (int)renderHeight, 96, 96, PixelFormats.Pbgra32);
            VisualBrush sourceBrush = new VisualBrush(source);

            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            using (drawingContext)
            {
                drawingContext.PushTransform(new ScaleTransform(scale, scale));
                drawingContext.DrawRectangle(sourceBrush, null, new Rect(new Point(0, 0), new Point(actualWidth, actualHeight)));
            }
            renderTarget.Render(drawingVisual);
            return renderTarget;

            //JpegBitmapEncoder jpgEncoder = new JpegBitmapEncoder();
            //jpgEncoder.QualityLevel = quality;
            //jpgEncoder.Frames.Add(BitmapFrame.Create(renderTarget));

            //Byte[] _imageArray;

            //using (MemoryStream outputStream = new MemoryStream())
            //{
            //    jpgEncoder.Save(outputStream);
            //    _imageArray = outputStream.ToArray();
            //}

            //return _imageArray;
        }
    }

}
