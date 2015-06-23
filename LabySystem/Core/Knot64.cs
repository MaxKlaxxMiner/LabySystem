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
      this.val = (ulong)(wallNumber << 2) |
                 (wallTop ? (ulong)1 : (ulong)0) |
                 (wallLeft ? (ulong)2 : (ulong)0);
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
        val = (val & 0xfffffffffffffffe) | (value ? (ulong)1 : (ulong)0);
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
        val = (val & 0xfffffffffffffffd) | (value ? (ulong)2 : (ulong)0);
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
  }
}
