#region # using *.*

using System;

#endregion

namespace LabySystem
{
  /// <summary>
  /// Klasse zum verarbeiten eines Labyrinth-Spieles
  /// </summary>
  public class LabyGame : IDisposable
  {
    #region # // --- Variablen ---
    /// <summary>
    /// merkt sich das aktuelle Labyrinth
    /// </summary>
    ILaby laby;

    /// <summary>
    /// merkt sich die Breite des Spielfeldes in Pixeln
    /// </summary>
    readonly int width;

    /// <summary>
    /// merkt sich die Höhe des Spielfeldes in Pixeln
    /// </summary>
    readonly int height;

    /// <summary>
    /// merkt sich, welche Felder bereits besucht wurden und wie oft
    /// </summary>
    readonly int[] visitedFields;

    /// <summary>
    /// gibt die Typen an, welches ein Spielfeld annehmen kann (Bit-Verknüpfung als Flags)
    /// </summary>
    [Flags]
    public enum FieldType
    {
      /// <summary>
      /// stellt ein freies begehbares Feld dar, welches noch nicht besucht wurde
      /// </summary>
      roomVisitedNone = 0x00,

      /// <summary>
      /// stellt ein freies begehbares Feld dar, welches bereits einmal besucht wurde
      /// </summary>
      roomVisitedFirst = 0x01,

      /// <summary>
      /// stellt ein freies begehbares Feld dar, welches ein zweites mal besucht wurde (zurück gelaufen)
      /// </summary>
      roomVisitedSecond = 0x02,

      /// <summary>
      /// stellt ein freies begehbares Feld dar, welches mehr als zweimal besucht wurde
      /// </summary>
      roomVisitedMore = 0x03,

      /// <summary>
      /// aktuelle Spielerposition
      /// </summary>
      player = 0x10,

      /// <summary>
      /// aktuelle Zielposition
      /// </summary>
      finish = 0x20,

      /// <summary>
      /// stellt ein nicht begehbares Feld dar (Wand)
      /// </summary>
      wall = 0x40,
    }

    /// <summary>
    /// Methode, welche aufgerufen wird, wenn ein Feld sich geändert hat
    /// </summary>
    DelegateFieldChanged fieldChanged;

    /// <summary>
    /// Delegaten, für Änderungen an einem bestimmten Feld
    /// </summary>
    /// <param name="labyGame">Labyrinth-Spiel, welches betroffen ist</param>
    /// <param name="fieldType">neuer Typ des Feldes</param>
    /// <param name="posX">X-Position des betroffenen Feldes</param>
    /// <param name="posY">Y-Position des betroffenen Feldes</param>
    public delegate void DelegateFieldChanged(LabyGame labyGame, FieldType fieldType, int posX, int posY);
    #endregion

    #region # // --- Konstruktor ---
    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="laby">bereits erstelltes Labyrinth</param>
    public LabyGame(ILaby laby)
    {
      while (laby.Generate(10000) > 0) { } // Labyrinth zu Ende generieren lassen (sofern nicht geschehen)

      this.laby = laby;
      width = laby.Width;
      height = laby.Height;

      PlayerX = 1;
      PlayerY = 1;
      FinishX = width - 2;
      FinishY = height - 2;

      fieldChanged = (l, t, x, y) => { }; // leere Methode für Feldänderungen
      visitedFields = new int[width * height];

      Visit(PlayerX, PlayerY, 1);
      Visit(FinishX, FinishY, 1);
    }

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="width">Breite des Labyrinthes in Pixeln</param>
    /// <param name="height">Höhe des Labyrinthes in Pixeln</param>
    /// <param name="seed">Startwert für den Zufallsgenerator</param>
    public LabyGame(int width, int height, int seed) : this(new LabySimpleFast(width, height, seed)) { }

    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="width">Breite des Labyrinthes in Pixeln</param>
    /// <param name="height">Höhe des Labyrinthes in Pixeln</param>
    public LabyGame(int width, int height) : this(new LabySimpleFast(width, height, Environment.TickCount)) { }
    #endregion

    #region # // --- public Methoden ---

    #region # // --- public static Methoden ---
    /// <summary>
    /// gibt die Spielfeldgröße anhand eines bestimmten Levels zurück
    /// </summary>
    /// <param name="level">Level, welches abgefragt werden soll</param>
    /// <returns>vorgeschlagene Höhe und Breite des Spielfeldes</returns>
    public static Tuple<int, int> GetLevelSize(int level)
    {
      int full = 2;
      int half = 3;
      int multi = 1;
      while (level > 1)
      {
        level--;
        if (full < half)
        {
          multi = full;
          full *= 2;
        }
        else
        {
          multi = half;
          half *= 2;
        }
      }
      return (multi & 1) == 0 ? new Tuple<int, int>(8 * multi - 1, 5 * multi - 1)
                              : new Tuple<int, int>(8 * multi - 1, 5 * multi);
    }

    #endregion

    #region # // --- public Abfrage-Methoden und -Properties ---
    /// <summary>
    /// gibt die Breite des Spielfeldes in Pixeln zurück
    /// </summary>
    public int Width { get { return width; } }

    /// <summary>
    /// gibt die Höhe des Spielfeldes in Pixeln zurück
    /// </summary>
    public int Height { get { return height; } }

    /// <summary>
    /// gibt die X-Position des Spielers zurück
    /// </summary>
    public int PlayerX { get; private set; }

    /// <summary>
    /// gibt die Y-Position des Spielers zurück
    /// </summary>
    public int PlayerY { get; private set; }

    /// <summary>
    /// gibt die X-Position des Zieles zurück
    /// </summary>
    public int FinishX { get; private set; }

    /// <summary>
    /// gibt die Y-Position des Zieles zurück
    /// </summary>
    public int FinishY { get; private set; }

    /// <summary>
    /// gibt an, ob das Ziel erreicht wurde
    /// </summary>
    public bool FinishReached
    {
      get
      {
        return PlayerX == FinishX && PlayerY == FinishY;
      }
    }
    #endregion

    #region # // --- public Update- und Steuerung-Methoden ---
    /// <summary>
    /// Setzt die Methode, welche aufgerufen werden soll, wenn sich ein Feld geändert hat
    /// </summary>
    /// <param name="fieldChangedMethod">Methode, welche pro geändertes Feld aufgerufen werden soll</param>
    public void SetFieldChangeEvent(DelegateFieldChanged fieldChangedMethod)
    {
      fieldChanged = fieldChangedMethod;
    }

    /// <summary>
    /// aktualisiert ein bestimmtes Feld und ruft deren Event auf
    /// </summary>
    /// <param name="posX">X-Position des Feldes</param>
    /// <param name="posY">Y-Position des Feldes</param>
    public void Update(int posX, int posY)
    {
      fieldChanged(this, GetField(posX, posY), posX, posY);
    }

    /// <summary>
    /// aktualisiert alle Felder und ruft deren Events auf
    /// </summary>
    public void UpdateAll()
    {
      for (int y = 0; y < height; y++)
      {
        for (int x = 0; x < width; x++)
        {
          Update(x, y);
        }
      }
    }

    /// <summary>
    /// gibt die Anzahl der Besuche eines Spielfeldes zurück oder ändert dieses
    /// </summary>
    /// <param name="posX">Position X</param>
    /// <param name="posY">Position Y</param>
    /// <param name="change">Änderung der Anzahl der Besuche</param>
    /// <returns>Anzahl der Besuche auf diesem Feld</returns>
    public int Visit(int posX, int posY, int change = 0)
    {
      if (change != 0)
      {
        visitedFields[posX + posY * width] += change;
        Update(posX, posY);
      }
      return visitedFields[posX + posY * width];
    }

    /// <summary>
    /// gibt den Typ eines bestimmten Feldes zurück
    /// </summary>
    /// <param name="posX">X-Position des Feldes</param>
    /// <param name="posY">Y-Position des Feldes</param>
    /// <returns>Typ des Feldes</returns>
    public FieldType GetField(int posX, int posY)
    {
      if (posX < 0 || posX > width || posY < 0 || posY > height) return FieldType.wall;
      if (laby.GetWall(posX, posY)) return FieldType.wall;

      var result = FieldType.roomVisitedNone;
      int visit = Visit(posX, posY);
      if (visit > 0 && visit != 2) result |= FieldType.roomVisitedFirst;
      if (visit > 1) result |= FieldType.roomVisitedSecond;

      if (posX == PlayerX && posY == PlayerY) result |= FieldType.player;
      if (posX == FinishX && posY == FinishY) result |= FieldType.finish;

      return result;
    }

    /// <summary>
    /// bewegt den Spieler bzw. das Zielfeld
    /// </summary>
    /// <param name="player">gibt an, ob der Spieler bewegt werden soll (sonst wird das Ziel bewegt)</param>
    /// <param name="dirX">X-Richtung zum bewegen (-1, 0, +1)</param>
    /// <param name="dirY">Y-Richtung zum bewegen (-1, 0, +1)</param>
    /// <returns></returns>
    public bool Move(bool player, int dirX, int dirY)
    {
      if (player)
      {
        if (laby.GetWall(PlayerX + dirX, PlayerY + dirY)) return false;
        PlayerX += dirX * 2;
        PlayerY += dirY * 2;
        Update(PlayerX - dirX * 2, PlayerY - dirY * 2);
        Visit(PlayerX - dirX, PlayerY - dirY, 1);
        Visit(PlayerX, PlayerY, 1);
      }
      else
      {
        if (laby.GetWall(FinishX + dirX, FinishY + dirY)) return false;
        FinishX += dirX * 2;
        FinishY += dirY * 2;
        Update(FinishX - dirX * 2, FinishY - dirY * 2);
        Visit(FinishX - dirX, FinishY - dirY, 1);
        Visit(FinishX, FinishY, 1);
      }

      return true;
    }

    /// <summary>
    /// bewegt den Spieler bzw. das Zielfeld nach links (sofern möglich)
    /// </summary>
    /// <param name="player">gibt an, ob der Spieler bewegt werden soll (sonst wird das Ziel bewegt)</param>
    /// <returns>true, wenn der Spieler bzw. das Zielfeld erfolgreich bewegt werden konnte</returns>
    public bool MoveLeft(bool player = true)
    {
      return Move(player, -1, 0);
    }

    /// <summary>
    /// bewegt den Spieler bzw. das Zielfeld nach rechts (sofern möglich)
    /// </summary>
    /// <param name="player">gibt an, ob der Spieler bewegt werden soll (sonst wird das Ziel bewegt)</param>
    /// <returns>true, wenn der Spieler bzw. das Zielfeld erfolgreich bewegt werden konnte</returns>
    public bool MoveRight(bool player = true)
    {
      return Move(player, +1, 0);
    }

    /// <summary>
    /// bewegt den Spieler bzw. das Zielfeld nach oben (sofern möglich)
    /// </summary>
    /// <param name="player">gibt an, ob der Spieler bewegt werden soll (sonst wird das Ziel bewegt)</param>
    /// <returns>true, wenn der Spieler bzw. das Zielfeld erfolgreich bewegt werden konnte</returns>
    public bool MoveUp(bool player = true)
    {
      return Move(player, 0, -1);
    }

    /// <summary>
    /// bewegt den Spieler bzw. das Zielfeld nach unten (sofern möglich)
    /// </summary>
    /// <param name="player">gibt an, ob der Spieler bewegt werden soll (sonst wird das Ziel bewegt)</param>
    /// <returns>true, wenn der Spieler bzw. das Zielfeld erfolgreich bewegt werden konnte</returns>
    public bool MoveDown(bool player = true)
    {
      return Move(player, 0, +1);
    }

    #endregion

    #endregion

    #region # // --- Dispose() ---
    /// <summary>
    /// Destruktor
    /// </summary>
    ~LabyGame()
    {
      Dispose();
    }

    /// <summary>
    /// gibt alle Ressourcen wieder frei
    /// </summary>
    public void Dispose()
    {
      if (laby == null) return;
      laby.Dispose();
      laby = null;
    }
    #endregion
  }
}
