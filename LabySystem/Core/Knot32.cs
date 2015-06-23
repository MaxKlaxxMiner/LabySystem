#region # using *.*

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace LabySystem.Core
{
  /// <summary>
  /// Struktur, welche sich einen (64-Bit) Knoten merkt
  /// </summary>
  public struct Knot32
  {
    /// <summary>
    /// gibt an, ob die obere Wand gesetzt ist oder nicht
    /// </summary>
    public bool WallTop;

    /// <summary>
    /// gibt an, ob die linke Wand gesetzt ist oder nicht
    /// </summary>
    public bool WallLeft;

    /// <summary>
    /// gibt die Wandnummer des Knotens zurück oder setzt diese
    /// </summary>
    public int WallNumber;

    /// <summary>
    /// Konstruktor eines neuen Knotens
    /// </summary>
    /// <param name="wallNumber">Wandnummer des Knotens</param>
    /// <param name="wallTop">gibt an, ob die obere Wand gesetzt werden soll</param>
    /// <param name="wallLeft">gibt an, ob die rechte Wand gesetzt werden soll</param>
    public Knot32(int wallNumber, bool wallTop, bool wallLeft)
    {
      WallNumber = wallNumber;
      WallTop = wallTop;
      WallLeft = wallLeft;
    }

    /// <summary>
    /// gibt den Inhalt als lesbare Zeichenkette zurück
    /// </summary>
    /// <returns>lesbare Zeichenkette</returns>
    public override string ToString()
    {
      return "[" + WallNumber + "] Top: " + WallTop + ", Left: " + WallLeft;
    }

    /// <summary>
    /// erstellt das Basis-Spielfeld
    /// </summary>
    /// <param name="w">Breite des Spielfeldes in Knoten</param>
    /// <param name="h">Höhe des Spielfeldes in Knoten</param>
    /// <returns>Enumerable mit allen Spielfeldern</returns>
    public static IEnumerable<Knot32> CreateBaseKnotes(long w, long h)
    {
      w--; h--;
      for (long y = 0; y <= h; y++)
      {
        for (long x = 0; x <= w; x++)
        {
          bool top = (x == 0 || x == w) && y > 0;
          bool left = (y == 0 || y == h) && x > 0;
          long number = top || left ? 0 : x + y * w;

          yield return new Knot32((int)number, top, left);
        }
      }
    }
  }
}
