#region # using *.*

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LabySystem;
using LabySystem.Core;

#endregion

namespace LabyTest
{
  [TestClass]
  public class LabySimpleTests
  {
    /// <summary>
    /// Struktur einer Testauflösung
    /// </summary>
    public struct ResolutionSet
    {
      /// <summary>
      /// gewünschte Breite des Spielfeldes in Pixeln
      /// </summary>
      public int w;

      /// <summary>
      /// gewünschte Höhe des Spielfeldes in Pixeln
      /// </summary>
      public int h;

      /// <summary>
      /// erwartet reale Breite des Spielfeldes in Pixeln
      /// </summary>
      public int pw;

      /// <summary>
      /// erwartet reale Höhe des Spielfeldes in Pixeln
      /// </summary>
      public int ph;

      /// <summary>
      /// gibt eine Liste mit mehreren Auflösungen zum testen zurück
      /// </summary>
      /// <returns>Liste mit verschiedenen Auflösungen</returns>
      public static ResolutionSet[] GetTestSet()
      {
        return new[]
        {
          new ResolutionSet { w = -1, h = -1, pw = 5, ph = 5 }, // negativ-values
          new ResolutionSet { w = 0, h = 0, pw = 5, ph = 5 },   // 0-values
          new ResolutionSet { w = 3, h = 1, pw = 5, ph = 5 },   // too low
          new ResolutionSet { w = 5, h = 5, pw = 5, ph = 5 },   // minimum
          new ResolutionSet { w = 99, h = 5, pw = 99, ph = 5 }, // column
          new ResolutionSet { w = 5, h = 99, pw = 5, ph = 99 }, // row
          new ResolutionSet { w = 8, h = 6, pw = 7, ph = 5 },   // small steps...
          new ResolutionSet { w = 11, h = 7, pw = 11, ph = 7 },
          new ResolutionSet { w = 14, h = 8, pw = 13, ph = 7 },
          new ResolutionSet { w = 17, h = 9, pw = 17, ph = 9 },
          new ResolutionSet { w = 20, h = 10, pw = 19, ph = 9 },
          new ResolutionSet { w = 23, h = 11, pw = 23, ph = 11 },
          new ResolutionSet { w = 26, h = 12, pw = 25, ph = 11 },
          new ResolutionSet { w = 29, h = 13, pw = 29, ph = 13 },
          new ResolutionSet { w = 32, h = 14, pw = 31, ph = 13 },
          new ResolutionSet { w = 35, h = 15, pw = 35, ph = 15 },
          new ResolutionSet { w = 320, h = 200, pw = 319, ph = 199 },     // low res: Amiga / Old VGA
          new ResolutionSet { w = 640, h = 480, pw = 639, ph = 479 },     // SVGA
          new ResolutionSet { w = 1280, h = 720, pw = 1279, ph = 719 },   // HD 720p
          new ResolutionSet { w = 1920, h = 1080, pw = 1919, ph = 1079 }, // FullHD 1080p
          new ResolutionSet { w = 3840, h = 2160, pw = 3839, ph = 2159 }, // 4K
        };
      }
    }

#if DEBUG
    /// <summary>
    /// LabySimple testen
    /// </summary>
    [TestMethod]
    public void TestSimple()
    {

      foreach (var set in ResolutionSet.GetTestSet())
      {
        using (var laby = new LabySimple(set.w, set.h, 12345))
        {
          Assert.AreEqual(set.pw, laby.Width);
          Assert.AreEqual(set.ph, laby.Height);
          while (laby.Generate(1000000) > 0) ;
        }
      }
    }
#endif

    /// <summary>
    /// LabySimpleFast testen
    /// </summary>
    [TestMethod]
    public void TestSimpleFast()
    {

      foreach (var set in ResolutionSet.GetTestSet())
      {
        using (var laby = new LabySimpleFast(set.w, set.h, 12345))
        {
          Assert.AreEqual(set.pw, laby.Width);
          Assert.AreEqual(set.ph, laby.Height);
          while (laby.Generate(1000000) > 0) ;
        }
      }
    }
  }
}
