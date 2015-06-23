#region # using *.*

using System.Collections.Generic;

#endregion

namespace LabySystem.Core
{
  /// <summary>
  /// Struktur, welche sich einen (64-Bit) Knoten merkt
  /// </summary>
  public struct Knot64
  {
    /// <summary>
    /// merkt sich den Wert des Knotens
    /// </summary>
    ulong val;

    /// <summary>
    /// Konstruktor eines neuen Knotens
    /// </summary>
    /// <param name="wallNumber">Wandnummer des Knotens</param>
    /// <param name="wallTop">gibt an, ob die obere Wand gesetzt werden soll</param>
    /// <param name="wallLeft">gibt an, ob die rechte Wand gesetzt werden soll</param>
    public Knot64(long wallNumber, bool wallTop, bool wallLeft)
    {
      val = (ulong)(wallNumber << 2) |
            (ulong)(wallTop ? 1 : 0) |
            (ulong)(wallLeft ? 2 : 0);
    }

    /// <summary>
    /// gibt die Wandnummer des Knotens zurück oder setzt diese
    /// </summary>
    public long WallNumber
    {
      get
      {
        return (long)(val >> 2);
      }
      set
      {
        val = (val & 0x3) | (ulong)(value << 2);
      }
    }

    /// <summary>
    /// gibt an, ob die obere Wand gesetzt ist oder nicht
    /// </summary>
    public bool WallTop
    {
      get
      {
        return (val & 1) == 1;
      }
      set
      {
        val = (val & 0xfffffffffffffffe) | (ulong)(value ? 1 : 0);
      }
    }

    /// <summary>
    /// gibt an, ob die linke Wand gesetzt ist oder nicht
    /// </summary>
    public bool WallLeft
    {
      get
      {
        return (val & 2) == 2;
      }
      set
      {
        val = (val & 0xfffffffffffffffd) | (ulong)(value ? 2 : 0);
      }
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
    public static IEnumerable<Knot64> CreateBaseKnotes(long w, long h)
    {
      w--; h--;
      for (long y = 0; y <= h; y++)
      {
        for (long x = 0; x <= w; x++)
        {
          bool top = (x == 0 || x == w) && y > 0;
          bool left = (y == 0 || y == h) && x > 0;
          long number = top || left ? 0 : x + y * w;

          yield return new Knot64(number, top, left);
        }
      }
    }
  }
}
