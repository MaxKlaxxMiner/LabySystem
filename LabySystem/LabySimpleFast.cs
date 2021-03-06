﻿#region # using *.*

using System;
using System.Collections.Generic;
using System.Linq;
using LabySystem.Core;

#endregion

namespace LabySystem
{
  /// <summary>
  /// Klasse zum erstellen und benutzen eines Labyrintes
  /// </summary>
  public class LabySimpleFast : ILaby
  {
    #region # // --- Variablen ---
    /// <summary>
    /// merkt sich das gesamte Spielfeld
    /// </summary>
    Knot32[] field;

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
    ulong rnd;

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
    public LabySimpleFast(int pixelWidth, int pixelHeight, int seed)
    {
      fieldWidth = Math.Max(2, (pixelWidth - 1) / 2) + 1;
      fieldHeight = Math.Max(2, (pixelHeight - 1) / 2) + 1;
      field = Knot32.CreateBaseKnotes(fieldWidth, fieldHeight).ToArray();
      this.pixelWidth = fieldWidth * 2 - 1;
      this.pixelHeight = fieldHeight * 2 - 1;
      rnd = (ulong)new Random(seed).Next();

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
          Knot32 k = field[p];
          if (!k.WallTop && field[p - fieldWidth].WallNumber != k.WallNumber) yield return p;
          if (!k.WallLeft && field[p - 1].WallNumber != k.WallNumber) yield return -p;
        }
      }
    }

    #region # // --- FieldNumberFill() ---
    /// <summary>
    /// füllt die Wandnummer einer zusammenhängenden Wand
    /// </summary>
    /// <param name="pos">Position der Wand, welche gefüllt werden soll</param>
    /// <param name="wallNumber">die zu befüllende Wandnummer</param>
    void FieldNumberFill(int pos, int wallNumber)
    {
      var listPos1 = new int[15];
      int listPos1Count = 0;
      var listPos2 = new int[15];
      int listPos2Count = 0;
      listPos1[listPos1Count++] = pos;

      while (listPos1Count > 0)
      {
        for (int i = 0; i < listPos1Count; i++)
        {
          int p = listPos1[i];
          if (field[p].WallNumber == wallNumber) continue;
          field[p].WallNumber = wallNumber;
          if (field[p].WallLeft && field[p - 1].WallNumber != wallNumber) listPos2[listPos2Count++] = p - 1;
          if (field[p + 1].WallLeft && field[p + 1].WallNumber != wallNumber) listPos2[listPos2Count++] = p + 1;
          if (field[p].WallTop && field[p - fieldWidth].WallNumber != wallNumber) listPos2[listPos2Count++] = p - fieldWidth;
          if (field[p + fieldWidth].WallTop && field[p + fieldWidth].WallNumber != wallNumber) listPos2[listPos2Count++] = p + fieldWidth;
          if (listPos2Count + 4 > listPos2.Length) Array.Resize(ref listPos2, listPos2.Length * 2);
        }

        listPos1Count = listPos2Count;
        listPos2Count = 0;

        var tmp = listPos1;
        listPos1 = listPos2;
        listPos2 = tmp;
      }
    }
    #endregion
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
      int remainLimit = (remainList.Length + 1) / 4;

      var _rnd = rnd;
      for (int tick = 0; tick < ticks; tick++)
      {
        remainTicks--;

        if (remainTicks < remainLimit)
        {
          remainList = GetRemainList().ToArray();
          remainTicks = remainList.Length;
          if (remainTicks == 0) break;
          remainLimit = (remainList.Length + 1) / 4;
        }

        int next = remainList[((_rnd = _rnd * 214013L + 2531011L) >> 16) % (uint)remainList.Length];

        if (next < 0) // --- waagerechte Variante (WallLeft) ---
        {
          next = -next;

          int n1 = field[next].WallNumber;
          int n2 = field[next - 1].WallNumber;

          if (n1 == n2 || field[next].WallLeft) continue; // überspringen, da Wand nicht gesetzt werden kann

          field[next].WallLeft = true; // Wand setzen

          if (n1 < n2) FieldNumberFill(next - 1, n1); else FieldNumberFill(next, n2); // angrenzende Wand auffüllen
        }
        else // --- senkrechte Variante (WallTop) ---
        {
          int n1 = field[next].WallNumber;
          int n2 = field[next - fieldWidth].WallNumber;

          if (n1 == n2 || field[next].WallTop) continue; // überspringen, da Wand nicht gesetzt werden kann

          field[next].WallTop = true; // Wand setzen

          if (n1 < n2) FieldNumberFill(next - fieldWidth, n1); else FieldNumberFill(next, n2); // angrenzende Wand auffüllen
        }
      }

      rnd = _rnd;

      if (remainList.Length > 0 && remainTicks > 0) return remainTicks;
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

      Knot32 knot = field[(x + 1) / 2 + (y + 1) / 2 * fieldWidth];

      return (x & 1) == 0 ? knot.WallTop : knot.WallLeft;
    }
    #endregion
  }
}
