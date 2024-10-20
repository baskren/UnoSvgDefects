using System.Reflection;
using Windows.Foundation;
using SkiaSharp;
using Microsoft.UI;
using SkiaSharp.Views.Windows;

namespace UnoApp3;

[Microsoft.UI.Xaml.Data.Bindable]
public partial class EmbeddedSvgImage : SKXamlCanvas
{
    #region Properties

        #region Stretch Property
        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(
            nameof(Stretch),
            typeof(Stretch),
            typeof(EmbeddedSvgImage),
            new PropertyMetadata(Stretch.Uniform)
        );
        public Stretch Stretch
        {
            get => (Stretch)GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }
        #endregion Stretch Property


        #endregion


        #region Fields

        SkiaSharp.Extended.Svg.SKSvg? _skSvg;
        double _imageAspect = 1.0;

        #endregion


        #region Construction
        public EmbeddedSvgImage()
        {
            Background = new SolidColorBrush(Colors.Transparent);
            PaintSurface += OnPaintSurface;
            MinHeight = 20;
            MinWidth = 20;
#if __IOS__
            Opaque = false;
#endif

        }

        public EmbeddedSvgImage(string resourceId, Assembly assembly) : this()
        {
            SetSource(resourceId, assembly);
        }
#endregion


        public void SetSource(string resourceId, Assembly assembly)
        {
            _skSvg = null;

            if (string.IsNullOrWhiteSpace(resourceId))
                return;


            using var stream = assembly.GetManifestResourceStream(resourceId);
            if (stream is null)
            {
                var resources = assembly.GetManifestResourceNames();
                Console.WriteLine($"ERROR: Cannot find embedded resource [{resourceId}] in assembly [{assembly}].");
                Console.WriteLine($"       Resources found:");
                foreach (var resource in resources)
                    Console.WriteLine($"       [{resource}]");
            }
            else
            {
                _skSvg = new SkiaSharp.Extended.Svg.SKSvg();
                _skSvg.Load(stream);
                _imageAspect = _skSvg.CanvasSize.Width < 1 || _skSvg.CanvasSize.Height < 1 
                    ? 1 
                    : _skSvg.CanvasSize.Width / _skSvg.CanvasSize.Height;
                Invalidate();
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {

            if (double.IsInfinity(availableSize.Width) == double.IsInfinity(availableSize.Height))
                return base.MeasureOverride(availableSize);
            
            var availableWidth = Math.Max(availableSize.Width, MinWidth);
            var availableHeight = Math.Max(availableSize.Height, MinHeight);
            
            if (availableWidth < 1 || availableHeight < 1)
                return base.MeasureOverride(availableSize);
            
            return double.IsInfinity(availableSize.Width) 
                ? new Size(availableSize.Height  * _imageAspect, availableSize.Height) 
                : new Size(availableSize.Width, availableSize.Width / _imageAspect);
        }

        private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            if (e.Surface?.Canvas is not { } workingCanvas)
                return;

            workingCanvas.Clear();
            if (_skSvg?.Picture is not { } picture)
                return;
            
            workingCanvas.Save();

            var fillRect = e.Info.Rect;
            var fillRectAspect = fillRect.Width / (float)fillRect.Height;

            if (_skSvg.CanvasSize.Width <= 0 || _skSvg.CanvasSize.Height <= 0)
            {
                Console.WriteLine("Cannot tile, scale or justify an SVG image with zero or negative Width or Height. Verify, in the SVG source, that the x, y, width, height, and viewBox attributes of the <SVG> tag are present and set correctly.");
            }
            else if (Stretch == Stretch.UniformToFill)
            {
                var scale = _imageAspect > fillRectAspect 
                    ? fillRect.Height / _skSvg.CanvasSize.Height 
                    : fillRect.Width / _skSvg.CanvasSize.Width;
                workingCanvas.Scale(scale, scale);
            }
            else if (Stretch == Stretch.Uniform)
            {
                var scale = _imageAspect > fillRectAspect 
                    ? fillRect.Width / _skSvg.CanvasSize.Width 
                    : fillRect.Height / _skSvg.CanvasSize.Height;
                workingCanvas.Scale(scale, scale);
            }
            else if (Stretch == Stretch.Fill)
            {
                var scaleX = fillRect.Width / _skSvg.CanvasSize.Width;
                var scaleY = fillRect.Height / _skSvg.CanvasSize.Height;
                workingCanvas.Scale(scaleX, scaleY);
            }


            if (Opacity is < 1.0 and >= 0)
            {
                var alpha = (byte)(Opacity * 255);
                var transparency = SKColors.White.WithAlpha(alpha); 
                var paint = new SKPaint { ColorFilter = SKColorFilter.CreateBlendMode(transparency, SKBlendMode.DstIn) };
                workingCanvas.DrawPicture(picture, paint);
            }
            else
                workingCanvas.DrawPicture(picture);

            workingCanvas.Restore();


        }

}
