#region # using *.*

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace LabySystem
{
  /// <summary>
  /// Schnittstelle eines Labyrinth-Systems
  /// </summary>
  public interface ILaby
  {
    /// <summary>
    /// generiert ein Teil des Labyrinthes
    /// </summary>
    /// <param name="ticks">Anzahl der abzuarbeitenden Rechenschritte</param>
    /// <returns>Anzahl der noch abzuarbeitenden Rechenschritte (0 = fertig)</returns>
    long Generate(int ticks);

    /// <summary>
    /// gibt die Breite des Spielfeldes in Pixeln zurück
    /// </summary>
    int Width { get; }

    /// <summary>
    /// gibt die Höhe des Spielfeldes in Pixeln zurück
    /// </summary>
    int Height { get; }

    /// <summary>
    /// gibt an, ob an einer bestimmten Position eine Wand ist
    /// </summary>
    /// <param name="x">X-Position auf dem Spielfeld</param>
    /// <param name="y">Y-Position auf dem Spielfeld</param>
    /// <returns>true, wenn das Spielfeld eine Wand/blockiert ist</returns>
    bool GetWall(int x, int y);
  }
}
