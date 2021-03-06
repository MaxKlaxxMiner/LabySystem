﻿#region # using *.*

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using LabySystem;

#endregion

namespace LabyMobile
{
  /// <summary>
  /// Stellt das anwendungsspezifische Verhalten bereit, um die Standardanwendungsklasse zu ergänzen.
  /// </summary>
  public sealed partial class App
  {
    private TransitionCollection transitions;

    /// <summary>
    /// Initialisiert das Singletonanwendungsobjekt.  Dies ist die erste Zeile von erstelltem Code
    /// und daher das logische Äquivalent von main() bzw. WinMain().
    /// </summary>
    public App()
    {
      InitializeComponent();
      Suspending += OnSuspending;
    }

    public class Game
    {
      int level;
      LabyGame labyGame;
      bool labyPlayer;
      int fieldWidth;
      int fieldHeight;
      int[] fieldPixels;
      int imgMulti;
      int imgWidth;
      int imgHeight;
      WriteableBitmap imgBitmap;
      byte[] imgBitmapBuf;
      Image imgOutput;

      public void InitGame(int startLevel, Image targetImage)
      {
        if (targetImage != null) imgOutput = targetImage;
        level = startLevel;
        fieldWidth = LabyGame.GetLevelSize(level).Item1;
        fieldHeight = LabyGame.GetLevelSize(level).Item1;
        fieldPixels = new int[fieldWidth * fieldHeight];

        labyGame = new LabyGame(fieldWidth, fieldHeight, level * 1234567 * (DateTime.Now.Day + DateTime.Now.Year * 365 + DateTime.Now.Month * 372));
        labyPlayer = true;

        imgMulti = 1;
        while ((imgMulti + 1) * fieldWidth < 1024) imgMulti++;

        imgWidth = fieldWidth * imgMulti;
        imgHeight = fieldHeight * imgMulti;
        imgBitmapBuf = new byte[imgWidth * imgHeight * 4];

        labyGame.SetFieldChangeEvent((game, type, x, y) =>
        {
          switch (type)
          {
            case LabyGame.FieldType.wall: fieldPixels[x + y * fieldWidth] = x == 0 || y == 0 || x == fieldWidth - 1 || y == fieldHeight - 1 ? 0x00008b : 0x000000; break;
            case LabyGame.FieldType.roomVisitedNone: fieldPixels[x + y * fieldWidth] = 0xd3d3d3; break;
            case LabyGame.FieldType.roomVisitedFirst: fieldPixels[x + y * fieldWidth] = 0xfafad2; break;
            case LabyGame.FieldType.roomVisitedSecond:
            case LabyGame.FieldType.roomVisitedMore: fieldPixels[x + y * fieldWidth] = 0xffff00; break;
            default:
            {
              if ((type & LabyGame.FieldType.player) > 0) fieldPixels[x + y * fieldWidth] = 0x008000;
              if ((type & LabyGame.FieldType.finish) > 0) fieldPixels[x + y * fieldWidth] = 0x8b0000;
            } break;
          }
          UpdateBitmap(x, y);
        });
        labyGame.UpdateAll();

        imgBitmap = new WriteableBitmap(imgWidth, imgHeight);
        imgOutput.Source = imgBitmap;

        PresentBitmap();
      }

      unsafe void UpdateBitmap(int fieldX, int fieldY)
      {
        fixed (byte* _buf = imgBitmapBuf)
        {
          int f = fieldPixels[fieldX + fieldY * fieldWidth];
          int* pix = (int*)&_buf[(fieldX * imgMulti + fieldY * imgMulti * fieldWidth * imgMulti) * 4];
          for (int cy = 0; cy < imgMulti; cy++)
          {
            for (int cx = 0; cx < imgMulti; cx++)
            {
              pix[cx + cy * imgWidth] = f;
            }
          }
        }
      }

      void PresentBitmap()
      {
        using (var str = imgBitmap.PixelBuffer.AsStream())
        {
          str.Write(imgBitmapBuf, 0, imgBitmapBuf.Length);
        }
        imgBitmap.Invalidate();
      }

      void UpdateGame()
      {
        PresentBitmap();
        if (labyGame.FinishReached)
        {
          InitGame(level + 1, null);
        }
      }

      internal void MoveRight()
      {
        if (labyGame.MoveRight(labyPlayer)) UpdateGame();
      }

      internal void MoveLeft()
      {
        if (labyGame.MoveLeft(labyPlayer)) UpdateGame();
      }

      internal void MoveUp()
      {
        if (labyGame.MoveUp(labyPlayer)) UpdateGame();
      }

      internal void MoveDown()
      {
        if (labyGame.MoveDown(labyPlayer)) UpdateGame();
      }
    }

    public readonly Game game = new Game();

    /// <summary>
    /// Wird aufgerufen, wenn die Anwendung durch den Endbenutzer normal gestartet wird.  Weitere Einstiegspunkte
    /// werden verwendet, wenn die Anwendung zum Öffnen einer bestimmten Datei, zum Anzeigen
    /// von Suchergebnissen usw. gestartet wird.
    /// </summary>
    /// <param name="e">Details über Startanforderung und -prozess.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs e)
    {
#if DEBUG
      if (System.Diagnostics.Debugger.IsAttached)
      {
        DebugSettings.EnableFrameRateCounter = true;
      }
#endif

      var rootFrame = Window.Current.Content as Frame;

      // App-Initialisierung nicht wiederholen, wenn das Fenster bereits Inhalte enthält.
      // Nur sicherstellen, dass das Fenster aktiv ist.
      if (rootFrame == null)
      {


        // Einen Rahmen erstellen, der als Navigationskontext fungiert und zum Parameter der ersten Seite navigieren
        rootFrame = new Frame { CacheSize = 1 };

        if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
        {
          // Hier: Zustand von zuvor angehaltener Anwendung laden
        }

        // Den Rahmen im aktuellen Fenster platzieren
        Window.Current.Content = rootFrame;
      }

      if (rootFrame.Content == null)
      {
        // Entfernt die Drehkreuznavigation für den Start.
        if (rootFrame.ContentTransitions != null)
        {
          transitions = new TransitionCollection();
          foreach (var c in rootFrame.ContentTransitions)
          {
            transitions.Add(c);
          }
        }

        rootFrame.ContentTransitions = null;
        rootFrame.Navigated += RootFrame_FirstNavigated;

        // Wenn der Navigationsstapel nicht wiederhergestellt wird, zur ersten Seite navigieren
        // und die neue Seite konfigurieren, indem die erforderlichen Informationen als Navigationsparameter
        // übergeben werden
        if (!rootFrame.Navigate(typeof(MainPage), e.Arguments))
        {
          throw new Exception("Failed to create initial page");
        }
      }

      // Sicherstellen, dass das aktuelle Fenster aktiv ist
      Window.Current.Activate();
    }

    /// <summary>
    /// Stellt die Inhaltsübergänge nach dem Start der App wieder her.
    /// </summary>
    /// <param name="sender">Das Objekt, an das der Handler angefügt wird.</param>
    /// <param name="e">Details zum Navigationsereignis.</param>
    private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
    {
      var rootFrame = sender as Frame;
      rootFrame.ContentTransitions = transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
      rootFrame.Navigated -= RootFrame_FirstNavigated;
    }

    /// <summary>
    /// Wird aufgerufen, wenn die Ausführung der Anwendung angehalten wird.  Der Anwendungszustand wird gespeichert,
    /// ohne zu wissen, ob die Anwendung beendet oder fortgesetzt wird und die Speicherinhalte dabei
    /// unbeschädigt bleiben.
    /// </summary>
    /// <param name="sender">Die Quelle der Anhalteanforderung.</param>
    /// <param name="e">Details zur Anhalteanforderung.</param>
    private void OnSuspending(object sender, SuspendingEventArgs e)
    {
      var deferral = e.SuspendingOperation.GetDeferral();

      // TODO: Anwendungszustand speichern und alle Hintergrundaktivitäten beenden


      deferral.Complete();
    }
  }
}
