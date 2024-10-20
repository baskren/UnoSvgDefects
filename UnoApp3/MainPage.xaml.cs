namespace UnoApp3;

public sealed partial class MainPage : Page
{
    
    readonly Image Image1 = new()
    {
        Stretch = Microsoft.UI.Xaml.Media.Stretch.Uniform,
        HorizontalAlignment = HorizontalAlignment.Stretch,
        VerticalAlignment = VerticalAlignment.Stretch,
    };
    readonly Image Image2 = new()
    {
        Stretch = Microsoft.UI.Xaml.Media.Stretch.Uniform,
        HorizontalAlignment = HorizontalAlignment.Stretch,
        VerticalAlignment = VerticalAlignment.Stretch,
    };

    public MainPage()
    {
        this.InitializeComponent();
        
        StackPanel.Children.Add(new TextBlock{Text="Imaging.SvgImageSource"});
        var source1 = new Microsoft.UI.Xaml.Media.Imaging.SvgImageSource(new Uri("ms-appx:///Assets/Svg/awc_320x50_ccwd.svg"));
        Image1.Source = source1;
        StackPanel.Children.Add(Image1);
        
        StackPanel.Children.Add(new TextBlock{Text="Imaging.BitmapImage (Resize-atizer)"});
        var source2 = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Ads/awc_320x50_ccwd.png"));
        Image2.Source = source1;
        StackPanel.Children.Add(Image2);
        
        StackPanel.Children.Add(new TextBlock{Text="Work around"});
        var svgImage = new EmbeddedSvgImage("UnoApp3.Resources.awc_320x50_ccwd.svg", GetType().Assembly)
        {
            Stretch = Microsoft.UI.Xaml.Media.Stretch.Uniform,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
        };
        StackPanel.Children.Add(svgImage);
    }

}
