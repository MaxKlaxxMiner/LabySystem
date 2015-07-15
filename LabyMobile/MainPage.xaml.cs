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

    private void Image_Tapped(object sender, TappedRoutedEventArgs e)
    {
      var app = (App)Application.Current;
      app.game.InitGame();
      TestBild.Source = app.game.imgBitmap;
    }

    private void ButtonUp_Tapped(object sender, TappedRoutedEventArgs e)
    {

    }

    private void ButtonLeft_Tapped(object sender, TappedRoutedEventArgs e)
    {

    }

    private void ButtonDown_Tapped(object sender, TappedRoutedEventArgs e)
    {

    }

    private void ButtonRight_Tapped(object sender, TappedRoutedEventArgs e)
    {

    }

  }
}
