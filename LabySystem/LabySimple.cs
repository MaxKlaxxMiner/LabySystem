#region # using *.*

using System;
using System.Collections.Generic;
using System.Linq;
using LabySystem.Core;

#endregion

namespace LabySystem
{
#if DEBUG
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

    /// <summary>
    /// merkt sich ein Array mit den noch offenen Feldern (als Cache, muss daher nicht aktuell sein)
    /// </summary>
    int[] remainList;

    /// <summary>
    /// merkt sich die Anzahl der noch offenen Ticks
    /// </summary>
    int remainTicks;
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
      field = Knot64.CreateBaseKnotes(fieldWidth, fieldHeight).ToArray();
      this.pixelWidth = fieldWidth * 2 - 1;
      this.pixelHeight = fieldHeight * 2 - 1;
      rnd = new Random(seed);

      remainList = GetRemainList().ToArray();
      remainTicks = remainList.Length;
    }

    /// <summary>
    /// gibt alle Ressourcen wieder frei
    /// </summary>
    public void Dispose()
    {
      field = null;
      remainList = null;
    }
    #endregion

    #region # // --- private Methoden ---
    /// <summary>
    /// gibt eine Liste mit den allen noch offenen Möglichkeiten zurück
    /// </summary>
    IEnumerable<int> GetRemainList()
    {
      for (int y = 1; y < fieldHeight; y++)
      {
        for (int x = 1; x < fieldWidth; x++)
        {
          int p = x + y * fieldWidth;
          Knot64 k = field[p];
          if (!k.WallTop && field[p - fieldWidth].WallNumber != k.WallNumber) yield return p;
          if (!k.WallLeft && field[p - 1].WallNumber != k.WallNumber) yield return -p;
        }
      }
    }

    /// <summary>
    /// füllt mit einer bestimmten Wandnummer einer zusammenhängenden Wand aus
    /// </summary>
    /// <param name="pos">Position der Wand, welche gefüllt werden soll</param>
    /// <param name="wallNumber">die zu befüllende Wandnummer</param>
    void FieldNumberFill(int pos, long wallNumber)
    {
      if (field[pos].WallNumber != wallNumber)
      {
        field[pos].WallNumber = wallNumber;
        if (field[pos].WallLeft) FieldNumberFill(pos - 1, wallNumber);
        if (field[pos + 1].WallLeft) FieldNumberFill(pos + 1, wallNumber);
        if (field[pos].WallTop) FieldNumberFill(pos - fieldWidth, wallNumber);
        if (field[pos + fieldWidth].WallTop) FieldNumberFill(pos + fieldWidth, wallNumber);
      }
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
      if (remainList.Length == 0) return 0;
      int remainLimit = (remainList.Length + 1) / 2;

      for (int tick = 0; tick < ticks; tick++)
      {
        remainTicks--;

        if (remainTicks < remainLimit)
        {
          remainList = GetRemainList().ToArray();
          remainTicks = remainList.Length;
          if (remainTicks == 0) break;
          remainLimit = (remainList.Length + 1) / 2;
        }

        int next = remainList[rnd.Next(remainList.Length)];

        if (next < 0) // --- waagerechte Variante (WallLeft) ---
        {
          next = -next;
          if (field[next].WallLeft || field[next].WallNumber == field[next - 1].WallNumber) continue; // überspringen, da Wand nicht gesetzt werden kann

          field[next].WallLeft = true;
          long number = Math.Min(field[next].WallNumber, field[next - 1].WallNumber);
          FieldNumberFill(next, number);
          FieldNumberFill(next - 1, number);
        }
        else // --- senkrechte Variante (WallTop) ---
        {
          if (field[next].WallTop || field[next].WallNumber == field[next - fieldWidth].WallNumber) continue; // überspringen, da Wand nicht gesetzt werdem kann

          field[next].WallTop = true;
          long number = Math.Min(field[next].WallNumber, field[next - fieldWidth].WallNumber);
          FieldNumberFill(next, number);
          FieldNumberFill(next - fieldWidth, number);
        }

      }

      return remainList.Length;
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
      if (x < 0 || y < 0 || x > pixelWidth || y > pixelHeight) throw new ArgumentOutOfRangeException();

      if ((x & 1) + (y & 1) == 0) return true;
      if ((x & 1) + (y & 1) == 2) return false;

      Knot64 knot = field[(x + 1) / 2 + (y + 1) / 2 * fieldWidth];

      return (x & 1) == 0 ? knot.WallTop : knot.WallLeft;
    }
    #endregion
  }
#endif
}
