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
        throw new NotImplementedException(); 
      }
    }

    /// <summary>
    /// gibt die Höhe des Spielfeldes in Pixeln zurück
    /// </summary>
    public int Height
    {
      get
      {
        throw new NotImplementedException(); 
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
