#region # using *.*

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabySystem.Core;

#endregion

namespace LabySystem
{
  /// <summary>
  /// Klasse zum erstellen und benutzen eines Labyrintes
  /// </summary>
  public class LabySimple : ILaby
  {
    #region # // --- Variablen ---
    /// <summary>
    /// merkt sich das gesamte Spielfeld
    /// </summary>
    Knot64[] field;

    /// <summary>
    /// merkt sich die Breite des Spielfeldes in Knoten
    /// </summary>
    int fieldWidth;

    /// <summary>
    /// merkt sich die Höhe des Spielfeldes in Knoten
    /// </summary>
    int fieldHeight;

    /// <summary>
    /// merkt sich die Breite des Spielfeldes in Pixeln
    /// </summary>
    int pixelWidth;

    /// <summary>
    /// merkt sich die Höhe des Spielfeldes in Pixeln
    /// </summary>
    int pixelHeight;

    /// <summary>
    /// merkt sich den aktuellen Zufallsgenerator
    /// </summary>
    Random rnd;
    #endregion

    #region # // --- Konstruktor / Dispose ---
    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="pixelWidth">Breite des Spielfeldes in Pixeln (min: 5)</param>
    /// <param name="pixelHeight">Höhe des Spielfeldes in Pixeln (min: 5)</param>
    /// <param name="seed">Random-Startwert</param>
    public LabySimple(int pixelWidth, int pixelHeight, int seed)
    {
      fieldWidth = Math.Max(2, (pixelWidth - 1) / 2) + 1;
      fieldHeight = Math.Max(2, (pixelHeight - 1) / 2) + 1;
      this.field = new Knot64[fieldWidth * fieldHeight];
      this.pixelWidth = fieldWidth * 2 - 1;
      this.pixelHeight = fieldHeight * 2 - 1;
      this.rnd = new Random(seed);
    }

    /// <summary>
    /// gibt alle Ressourcen wieder frei
    /// </summary>
    public void Dispose()
    {
      field = null;
    }
    #endregion

    #region # // --- ILaby ---
    /// <summary>
    /// generiert ein Teil des Labyrinthes
    /// </summary>
    /// <param name="ticks">Anzahl der abzuarbeitenden Rechenschritte</param>
    /// <returns>Anzahl der noch abzuarbeitenden Rechenschritte (0 = fertig)</returns>
    public long Generate(int ticks)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// gibt die Breite des Spielfeldes in Pixeln zurück
    /// </summary>
    public int Width
    {
      get
      {
        return pixelWidth;
      }
    }

    /// <summary>
    /// gibt die Höhe des Spielfeldes in Pixeln zurück
    /// </summary>
    public int Height
    {
      get
      {
        return pixelHeight;
      }
    }

    /// <summary>
    /// gibt an, ob an einer bestimmten Position eine Wand ist
    /// </summary>
    /// <param name="x">X-Position auf dem Spielfeld</param>
    /// <param name="y">Y-Position auf dem Spielfeld</param>
    /// <returns>true, wenn das Spielfeld eine Wand/blockiert ist</returns>
    public bool GetWall(int x, int y)
    {
      throw new NotImplementedException();
    }
    #endregion
  }
}
