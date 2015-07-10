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
using LabySystem;


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
      int level = 8;
      var labyGame = new LabyGame(LabyGame.GetLevelSize(level).Item1, LabyGame.GetLevelSize(level).Item1, level * 1234567 * (DateTime.Now.Day + DateTime.Now.Year * 365 + DateTime.Now.Month * 372));
      bool labyPlayer = true;

      int fieldWidth = labyGame.Width;
      int fieldHeight = labyGame.Height;

      int[] fieldPixels = new int[fieldWidth * fieldHeight];

      labyGame.SetFieldChangeEvent((game, type, x, y) =>
      {
        switch (type)
        {
          case LabyGame.FieldType.wall: fieldPixels[x + y * fieldWidth] = 0x000000; break;
          case LabyGame.FieldType.roomVisitedNone: fieldPixels[x + y * fieldWidth] = 0xd3d3d3; break;
          case LabyGame.FieldType.roomVisitedFirst: fieldPixels[x + y * fieldWidth] = 0xfafad2; break;
          case LabyGame.FieldType.roomVisitedSecond:
          case LabyGame.FieldType.roomVisitedMore: fieldPixels[x + y * fieldWidth] = 0xff0000; break;
          default:
          {
            if ((type & LabyGame.FieldType.player) > 0) fieldPixels[x + y * fieldWidth] = 0x008000;
            if ((type & LabyGame.FieldType.finish) > 0) fieldPixels[x + y * fieldWidth] = 0x8b0000;
          } break;
        }
      });
      labyGame.UpdateAll();


      int imgMulti = 4;

      int imgWidth = fieldWidth * imgMulti;
      int imgHeight = fieldHeight * imgMulti;
      var test = new WriteableBitmap(imgWidth, imgHeight);
      byte[] buf = new byte[imgWidth * imgHeight * 4];

      fixed (byte* _buf = buf)
      {
        for (int y = 0; y < fieldHeight; y++)
        {
          for (int x = 0; x < fieldWidth; x++)
          {
            int f = fieldPixels[x + y * fieldWidth];
            int* pix = (int*)&_buf[(x * imgMulti + y * imgMulti * fieldWidth * imgMulti) * 4];
            for (int cy = 0; cy < imgMulti; cy++)
            {
              for (int cx = 0; cx < imgMulti; cx++)
              {
                pix[cx + cy * imgWidth] = f;
              }
            }
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
