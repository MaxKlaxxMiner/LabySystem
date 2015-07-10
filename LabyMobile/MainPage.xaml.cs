using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=391641 dokumentiert.

namespace LabyMobile
{
  /// <summary>
  /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Rahmens navigiert werden kann.
  /// </summary>
  public sealed partial class MainPage
  {
    public MainPage()
    {
      InitializeComponent();

      NavigationCacheMode = NavigationCacheMode.Required;
    }

    /// <summary>
    /// Wird aufgerufen, wenn diese Seite in einem Rahmen angezeigt werden soll.
    /// </summary>
    /// <param name="e">Ereignisdaten, die beschreiben, wie diese Seite erreicht wurde.
    /// Dieser Parameter wird normalerweise zum Konfigurieren der Seite verwendet.</param>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      // TODO: Seite vorbereiten, um sie hier anzuzeigen.

      // TODO: Wenn Ihre Anwendung mehrere Seiten enthält, stellen Sie sicher, dass
      // die Hardware-Zurück-Taste behandelt wird, indem Sie das
      // Windows.Phone.UI.Input.HardwareButtons.BackPressed-Ereignis registrieren.
      // Wenn Sie den NavigationHelper verwenden, der bei einigen Vorlagen zur Verfügung steht,
      // wird dieses Ereignis für Sie behandelt.
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      Application.Current.Exit();
    }

    private unsafe void Image_Tapped(object sender, TappedRoutedEventArgs e)
    {
      //var bild = new BitmapImage(new Uri("https://avatars0.githubusercontent.com/u/10117269?v=3&s=460"));

      int breite = 512;
      int höhe = 512;

      var test = new WriteableBitmap(breite, höhe);
      byte[] buf = new byte[breite * höhe * 4];

      fixed (byte* _buf = buf)
      {
        int* pix = (int*)_buf;

        for (int y = 0; y < höhe; y++)
        {
          for (int x = 0; x < breite; x++)
          {
            pix[x + y * breite] = x * x + y * y;
          }
        }

      }

      using (var str = test.PixelBuffer.AsStream())
      {
        str.Write(buf, 0, buf.Length);
      }

      TestBild.Source = test;
    }
  }
}
