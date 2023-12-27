#region # using *.*

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using LabySystem;

#endregion

namespace LabyWindows
{
  public partial class LabyForm : Form
  {
    public LabyForm()
    {
      InitializeComponent();
    }

    int bre;
    int hei;

    Bitmap gamePicture = new Bitmap(1, 1, PixelFormat.Format32bppRgb);
    Graphics gameGraphics;
    int level = 1;
    LabyGame labyGame;
    bool labyPlayer = true;
    Bitmap labyPicture;
    int offsetX;
    int offsetY;

    int zoomLevel = 6;
    readonly int[] zoomsWidth = { 1, 2, 3, 4, 5, 6, 8, 10, 12, 15, 20, 24, 30, 40, 60, 96, 120 };
    int fieldWidth;
    int fieldHeight;
    const int fieldJumps = 6;

    readonly List<Point> marker = new List<Point>();
    readonly HashSet<Point> deadLines = new HashSet<Point>();
    bool zoomOut;

    void DrawLaby()
    {
      if (gamePicture.Width != gamePictureBox1.Width || gamePicture.Height != gamePictureBox1.Height)
      {
        gamePictureBox1.Image = null;
        gamePicture.Dispose();
        gamePicture = new Bitmap(gamePictureBox1.Width, gamePictureBox1.Height, PixelFormat.Format32bppRgb);
        gamePictureBox1.Image = gamePicture;
        if (gameGraphics != null) gameGraphics.Dispose();
        gameGraphics = Graphics.FromImage(gamePicture);
        gameGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
      }

      if (zoomOut || (labyPicture.Width < fieldWidth || labyPicture.Height < fieldHeight))
      {
        gameGraphics.FillRectangle(new SolidBrush(Color.Gray), 0, 0, gamePicture.Width, gamePicture.Height);
        int drawMul = 1;

        while (labyPicture.Width * (drawMul + 1) < bre && labyPicture.Height * (drawMul + 1) < hei) drawMul++;

        if (drawMul > 1)
        {
          gameGraphics.DrawImage(labyPicture, new Rectangle((gamePicture.Width - labyPicture.Width * drawMul) / 2, (gamePicture.Height - labyPicture.Height * drawMul) / 2, labyPicture.Width * drawMul, labyPicture.Height * drawMul), -0.5f, -0.5f, labyPicture.Width, labyPicture.Height, GraphicsUnit.Pixel);
        }
        else
        {
          gameGraphics.DrawImage(labyPicture, (gamePicture.Width - labyPicture.Width) / 2, (gamePicture.Height - labyPicture.Height) / 2);
        }
      }
      else
      {
        gameGraphics.DrawImage(labyPicture, new Rectangle(0, 0, gamePicture.Width, gamePicture.Height), -0.5f + offsetX, -0.5f + offsetY, fieldWidth, fieldHeight, GraphicsUnit.Pixel);
      }

      if (nextDeleteTimer > 0)
      {
        string txt = ((Environment.TickCount - nextDeleteTimer + 30000) / 30000.0).ToString("N1");
        var f = new Font("Consolas", 14f);
        var rect = gameGraphics.MeasureString(txt, f);
        gameGraphics.FillRectangle(new SolidBrush(Color.White), 0, 0, rect.Width, rect.Height);
        gameGraphics.DrawString(txt, f, new SolidBrush(Color.Black), 0f, 0f);
      }
      gamePictureBox1.Refresh();
    }
    void DeadLineScan(int startX, int startY, int posX, int posY, int endX, int endY, int deadLimit)
    {
      HashSet<uint> ok = new HashSet<uint>();
      Stack<Point> todo = new Stack<Point>();
      todo.Push(new Point(posX, posY));
      List<Point> deadMarker = new List<Point>();
      int width = labyGame.Width - 2;
      int height = labyGame.Height - 2;

      while (todo.Count > 0)
      {
        var set = todo.Pop();
        uint id = (uint)set.X + (uint)set.Y * 65536;
        if (ok.Contains(id)) continue;
        ok.Add(id);
        if (ok.Count > deadLimit) return;
        if (labyGame.GetField(set.X, set.Y) != LabyGame.FieldType.wall)
        {
          if (set.X == startX && set.Y == startY) continue;
          if (set.X == endX && set.Y == endY) return;
          if (set.X == 1 && set.Y == 1) return;
          if (set.X == width && set.Y == height) return;
          deadMarker.Add(set);
          todo.Push(new Point(set.X - 1, set.Y));
          todo.Push(new Point(set.X + 1, set.Y));
          todo.Push(new Point(set.X, set.Y - 1));
          todo.Push(new Point(set.X, set.Y + 1));
        }
      }

      //deadLines.AddRange(deadMarker);
      deadLines.UnionWith(deadMarker);
    }

    void DeadLineScan(int startX, int startY, int endX, int endY, int deadLimit)
    {
      DeadLineScan(startX, startY, startX - 1, startY, endX, endY, deadLimit);
      DeadLineScan(startX, startY, startX + 1, startY, endX, endY, deadLimit);
      DeadLineScan(startX, startY, startX, startY - 1, endX, endY, deadLimit);
      DeadLineScan(startX, startY, startX, startY + 1, endX, endY, deadLimit);
    }

    static int deadLimit = 1;
    static int deadMuli = 3;

    void DeadLineScanner()
    {
      //      var temp = deadLines.ToArray();
      deadLines.Clear();

      //foreach (var dl in temp)
      //{
      //  labyGame.Update(dl.X, dl.Y);
      //}

      if (labyPlayer)
      {
        DeadLineScan(labyGame.PlayerX, labyGame.PlayerY, labyGame.FinishX, labyGame.FinishY, deadLimit);
      }
      else
      {
        DeadLineScan(labyGame.FinishX, labyGame.FinishY, labyGame.PlayerX, labyGame.PlayerY, deadLimit);
      }

      foreach (var dl in deadLines)
      {
        labyGame.Update(dl.X, dl.Y);
      }

      deadLimit = 1;
    }

    #region # void InitGame() // Spielfeld initialisieren und zeichnen
    /// <summary>
    /// Spielfeld initialisieren und zeichnen
    /// </summary>
    void InitGame()
    {
      if (bre == 0)
      {
        bre = Size.Width;
        hei = Size.Height;
      }
      if (level > 1)
      {
        MessageBox.Show("Level: " + level + " (" + LabyGame.GetLevelSize(level).Item1.ToString("#,##0") + " x " + LabyGame.GetLevelSize(level).Item2.ToString("#,##0") + " = " + (LabyGame.GetLevelSize(level).Item1 * LabyGame.GetLevelSize(level).Item2).ToString("#,##0") + ")", "next Level");
      }
      if (labyGame != null) labyGame.Dispose();
      labyGame = new LabyGame(LabyGame.GetLevelSize(level).Item1, LabyGame.GetLevelSize(level).Item2, level * 12345678 * (DateTime.Now.Day + DateTime.Now.Year * 365 + DateTime.Now.Month * 372));
      labyPlayer = true;
      labyPicture = new Bitmap(labyGame.Width, labyGame.Height, PixelFormat.Format32bppRgb);
      offsetX = 0;
      offsetY = 0;
      marker.Clear();
      deadLines.Clear();
      fieldWidth = bre / zoomsWidth[zoomLevel];
      fieldHeight = hei / zoomsWidth[zoomLevel];

      #region # // --- Spielfeld zeichnen ---
      int labyLine = labyPicture.Width;
      int[] fastPixel = new int[labyPicture.Width * labyPicture.Height];

      labyGame.SetFieldChangeEvent((game, type, x, y) =>
      {
        Color f;
        switch (type)
        {
          case LabyGame.FieldType.wall: f = Color.Black; break;
          case LabyGame.FieldType.roomVisitedNone:
          case LabyGame.FieldType.roomVisitedFirst:
          case LabyGame.FieldType.roomVisitedSecond:
          case LabyGame.FieldType.roomVisitedMore: f = Color.LightGray; break;
          default:
          {
            f = Color.White;
            if ((type & LabyGame.FieldType.player) > 0) f = Color.Green;
            if ((type & LabyGame.FieldType.finish) > 0) f = Color.DarkRed;
          } break;
        }
        fastPixel[x + y * labyLine] = f.ToArgb();
      });
      labyGame.UpdateAll();

      for (int x = 0; x < labyPicture.Width; x++)
      {
        fastPixel[x] = Color.DarkBlue.ToArgb();
        fastPixel[x + (labyPicture.Height - 1) * labyLine] = Color.DarkBlue.ToArgb();
      }

      for (int y = 0; y < labyPicture.Height; y++)
      {
        fastPixel[y * labyLine] = Color.DarkBlue.ToArgb();
        fastPixel[labyLine - 1 + y * labyLine] = Color.DarkBlue.ToArgb();
      }

      var bitmapData = labyPicture.LockBits(new Rectangle(0, 0, labyPicture.Width, labyPicture.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
      Marshal.Copy(fastPixel, 0, bitmapData.Scan0, fastPixel.Length);
      labyPicture.UnlockBits(bitmapData);

      // --- zur direkten (langsameren) Methode wechseln ---
      labyGame.SetFieldChangeEvent((game, type, x, y) =>
      {
        switch (type)
        {
          case LabyGame.FieldType.wall:
          {
            if (x == 0 || y == 0 || x == labyPicture.Width - 1 || y == labyPicture.Height - 1)
            {
              labyPicture.SetPixel(x, y, Color.DarkBlue);
            }
            else
            {
              labyPicture.SetPixel(x, y, Color.Black);
            }
          } break;
          case LabyGame.FieldType.roomVisitedNone: labyPicture.SetPixel(x, y, deadLines.Contains(new Point(x, y)) ? Color.PaleVioletRed : Color.LightGray); break;
          case LabyGame.FieldType.roomVisitedFirst: labyPicture.SetPixel(x, y, marker.Any(m => m.X == x && m.Y == y) ? Color.Coral : Color.LightGoldenrodYellow); break;
          case LabyGame.FieldType.roomVisitedSecond:
          case LabyGame.FieldType.roomVisitedMore: labyPicture.SetPixel(x, y, marker.Any(m => m.X == x && m.Y == y) ? Color.Coral : Color.Yellow); break;
          default:
          {
            if ((type & LabyGame.FieldType.player) > 0) labyPicture.SetPixel(x, y, Color.Green);
            if ((type & LabyGame.FieldType.finish) > 0) labyPicture.SetPixel(x, y, Color.DarkRed);
          } break;
        }
      });
      #endregion
    }
    #endregion

    private void LabyForm_Load(object sender, EventArgs e)
    {
      FormBorderStyle = FormBorderStyle.None;
      WindowState = FormWindowState.Maximized;
      InitGame();
      DrawLaby();
    }

    int lastKeyInsert;
    int nextDeleteTimer = Environment.TickCount + 1000;

    private void LabyForm_KeyDown(object sender, KeyEventArgs e)
    {
      bool first = true;
      while (first || (e.KeyCode == Keys.Insert && lastKeyInsert + 100 > Environment.TickCount))
      {
        first = false;
        switch (e.KeyCode)
        {
          case Keys.Left:
          {
            labyGame.MoveLeft(!labyPlayer);
          } break;
          case Keys.A:
          case Keys.NumPad4:
          {
            labyGame.MoveLeft(labyPlayer);
            if ((labyPlayer ? labyGame.PlayerX : labyGame.FinishX) < offsetX + fieldJumps)
            {
              offsetX = labyPlayer ? labyGame.PlayerX : labyGame.FinishX;
              offsetX -= fieldWidth / 2;
              goto case Keys.Control;
            }
          } break;

          case Keys.Right:
          {
            labyGame.MoveRight(!labyPlayer);
          } break;
          case Keys.D:
          case Keys.NumPad6:
          {
            labyGame.MoveRight(labyPlayer);
            if ((labyPlayer ? labyGame.PlayerX : labyGame.FinishX) >= offsetX + fieldWidth - fieldJumps)
            {
              offsetX = labyPlayer ? labyGame.PlayerX : labyGame.FinishX;
              offsetX -= fieldWidth / 2;
              goto case Keys.Control;
            }
          } break;

          case Keys.Up:
          {
            labyGame.MoveUp(!labyPlayer);
          } break;
          case Keys.W:
          case Keys.NumPad8:
          {
            labyGame.MoveUp(labyPlayer);
            if ((labyPlayer ? labyGame.PlayerY : labyGame.FinishY) < offsetY + fieldJumps)
            {
              offsetY = labyPlayer ? labyGame.PlayerY : labyGame.FinishY;
              offsetY -= fieldHeight / 2;
              goto case Keys.Control;
            }
          } break;

          case Keys.Tab: zoomOut = !zoomOut; break;

          case Keys.Down:
          {
            labyGame.MoveDown(!labyPlayer);
          } break;
          case Keys.S:
          case Keys.NumPad2:
          {
            labyGame.MoveDown(labyPlayer);
            if ((labyPlayer ? labyGame.PlayerY : labyGame.FinishY) >= offsetY + fieldHeight - fieldJumps)
            {
              offsetY = labyPlayer ? labyGame.PlayerY : labyGame.FinishY;
              offsetY -= fieldHeight / 2;
              goto case Keys.Control;
            }
          } break;

          case Keys.Delete:
          {
            int next = nextDeleteTimer - Environment.TickCount;
            if (next > 0)
            {
              MessageBox.Show("Wait: " + (next / 1000.0).ToString("N1") + " s");
              break;
            }
            deadLimit = 20000000;
            nextDeleteTimer += 30000;
          } break;

          case Keys.Insert:
          {
            deadLimit = 100;

            int cx = 0;
            int cy = 0;
            int lim = 0;
            while (lim < 100)
            {
              if (labyPlayer)
              {
                cx = labyGame.PlayerX;
                cy = labyGame.PlayerY;
              }
              else
              {
                cx = labyGame.FinishX;
                cy = labyGame.FinishY;
              }

              if (
                (labyGame.GetField(cx - 1, cy) != LabyGame.FieldType.roomVisitedNone || labyPicture.GetPixel(cx - 1, cy).ToArgb() == Color.PaleVioletRed.ToArgb()) &&
                (labyGame.GetField(cx + 1, cy) != LabyGame.FieldType.roomVisitedNone || labyPicture.GetPixel(cx + 1, cy).ToArgb() == Color.PaleVioletRed.ToArgb()) &&
                (labyGame.GetField(cx, cy - 1) != LabyGame.FieldType.roomVisitedNone || labyPicture.GetPixel(cx, cy - 1).ToArgb() == Color.PaleVioletRed.ToArgb()) &&
                (labyGame.GetField(cx, cy + 1) == LabyGame.FieldType.roomVisitedNone && labyPicture.GetPixel(cx, cy + 1).ToArgb() != Color.PaleVioletRed.ToArgb())
                )
              { labyGame.MoveDown(labyPlayer); lim++; continue; }

              if (
                (labyGame.GetField(cx - 1, cy) != LabyGame.FieldType.roomVisitedNone || labyPicture.GetPixel(cx - 1, cy).ToArgb() == Color.PaleVioletRed.ToArgb()) &&
                (labyGame.GetField(cx + 1, cy) == LabyGame.FieldType.roomVisitedNone && labyPicture.GetPixel(cx + 1, cy).ToArgb() != Color.PaleVioletRed.ToArgb()) &&
                (labyGame.GetField(cx, cy - 1) != LabyGame.FieldType.roomVisitedNone || labyPicture.GetPixel(cx, cy - 1).ToArgb() == Color.PaleVioletRed.ToArgb()) &&
                (labyGame.GetField(cx, cy + 1) != LabyGame.FieldType.roomVisitedNone || labyPicture.GetPixel(cx, cy + 1).ToArgb() == Color.PaleVioletRed.ToArgb())
                )
              { labyGame.MoveRight(labyPlayer); lim++; continue; }

              if (
                (labyGame.GetField(cx - 1, cy) != LabyGame.FieldType.roomVisitedNone || labyPicture.GetPixel(cx - 1, cy).ToArgb() == Color.PaleVioletRed.ToArgb()) &&
                (labyGame.GetField(cx + 1, cy) != LabyGame.FieldType.roomVisitedNone || labyPicture.GetPixel(cx + 1, cy).ToArgb() == Color.PaleVioletRed.ToArgb()) &&
                (labyGame.GetField(cx, cy - 1) == LabyGame.FieldType.roomVisitedNone && labyPicture.GetPixel(cx, cy - 1).ToArgb() != Color.PaleVioletRed.ToArgb()) &&
                (labyGame.GetField(cx, cy + 1) != LabyGame.FieldType.roomVisitedNone || labyPicture.GetPixel(cx, cy + 1).ToArgb() == Color.PaleVioletRed.ToArgb())
                )
              { labyGame.MoveUp(labyPlayer); lim++; continue; }

              if (
                (labyGame.GetField(cx - 1, cy) == LabyGame.FieldType.roomVisitedNone && labyPicture.GetPixel(cx - 1, cy).ToArgb() != Color.PaleVioletRed.ToArgb()) &&
                (labyGame.GetField(cx + 1, cy) != LabyGame.FieldType.roomVisitedNone || labyPicture.GetPixel(cx + 1, cy).ToArgb() == Color.PaleVioletRed.ToArgb()) &&
                (labyGame.GetField(cx, cy - 1) != LabyGame.FieldType.roomVisitedNone || labyPicture.GetPixel(cx, cy - 1).ToArgb() == Color.PaleVioletRed.ToArgb()) &&
                (labyGame.GetField(cx, cy + 1) != LabyGame.FieldType.roomVisitedNone || labyPicture.GetPixel(cx, cy + 1).ToArgb() == Color.PaleVioletRed.ToArgb())
                )
              { labyGame.MoveLeft(labyPlayer); lim++; continue; }

              break;
            }

            if (lim == 0)
            {
              if (deadMuli < 1500 &&
                  (
                    (labyGame.GetField(cx, cy + 1) == LabyGame.FieldType.roomVisitedNone && labyPicture.GetPixel(cx, cy + 1).ToArgb() != Color.PaleVioletRed.ToArgb()) ||
                    (labyGame.GetField(cx + 1, cy) == LabyGame.FieldType.roomVisitedNone && labyPicture.GetPixel(cx + 1, cy).ToArgb() != Color.PaleVioletRed.ToArgb()) ||
                    (labyGame.GetField(cx, cy - 1) == LabyGame.FieldType.roomVisitedNone && labyPicture.GetPixel(cx, cy - 1).ToArgb() != Color.PaleVioletRed.ToArgb()) ||
                    (labyGame.GetField(cx - 1, cy) == LabyGame.FieldType.roomVisitedNone && labyPicture.GetPixel(cx - 1, cy).ToArgb() != Color.PaleVioletRed.ToArgb())
                  ))
              {
                deadLimit *= deadMuli;
                deadMuli *= 7;
                break;
              }
              deadLimit = 100;
              deadMuli = 3;

              if (labyGame.GetField(cx, cy + 1) == LabyGame.FieldType.roomVisitedNone && labyPicture.GetPixel(cx, cy + 1).ToArgb() != Color.PaleVioletRed.ToArgb()) { labyGame.MoveDown(labyPlayer); break; }
              if (labyGame.GetField(cx + 1, cy) == LabyGame.FieldType.roomVisitedNone && labyPicture.GetPixel(cx + 1, cy).ToArgb() != Color.PaleVioletRed.ToArgb()) { labyGame.MoveRight(labyPlayer); break; }
              if (labyGame.GetField(cx, cy - 1) == LabyGame.FieldType.roomVisitedNone && labyPicture.GetPixel(cx, cy - 1).ToArgb() != Color.PaleVioletRed.ToArgb()) { labyGame.MoveUp(labyPlayer); break; }
              if (labyGame.GetField(cx - 1, cy) == LabyGame.FieldType.roomVisitedNone && labyPicture.GetPixel(cx - 1, cy).ToArgb() != Color.PaleVioletRed.ToArgb()) { labyGame.MoveLeft(labyPlayer); break; }

              if (labyGame.GetField(cx, cy + 1) == LabyGame.FieldType.roomVisitedFirst && labyPicture.GetPixel(cx, cy + 1).ToArgb() != Color.PaleVioletRed.ToArgb()) { labyGame.MoveDown(labyPlayer); break; }
              if (labyGame.GetField(cx + 1, cy) == LabyGame.FieldType.roomVisitedFirst && labyPicture.GetPixel(cx + 1, cy).ToArgb() != Color.PaleVioletRed.ToArgb()) { labyGame.MoveRight(labyPlayer); break; }
              if (labyGame.GetField(cx, cy - 1) == LabyGame.FieldType.roomVisitedFirst && labyPicture.GetPixel(cx, cy - 1).ToArgb() != Color.PaleVioletRed.ToArgb()) { labyGame.MoveUp(labyPlayer); break; }
              if (labyGame.GetField(cx - 1, cy) == LabyGame.FieldType.roomVisitedFirst && labyPicture.GetPixel(cx - 1, cy).ToArgb() != Color.PaleVioletRed.ToArgb()) { labyGame.MoveLeft(labyPlayer); break; }
            }
            deadMuli = 3;

            if ((labyPlayer ? labyGame.PlayerX : labyGame.FinishX) < offsetX + fieldJumps) goto case Keys.Enter;
            if ((labyPlayer ? labyGame.PlayerX : labyGame.FinishX) >= offsetX + fieldWidth - fieldJumps) goto case Keys.Enter;
            if ((labyPlayer ? labyGame.PlayerY : labyGame.FinishY) < offsetY + fieldJumps) goto case Keys.Enter;
            if ((labyPlayer ? labyGame.PlayerY : labyGame.FinishY) >= offsetY + fieldHeight - fieldJumps) goto case Keys.Enter;
          } break;

          case Keys.Space: labyPlayer = !labyPlayer; goto case Keys.Enter;

          case Keys.Back:
          {
            marker.Add(labyPlayer ? new Point(labyGame.PlayerX, labyGame.PlayerY) : new Point(labyGame.FinishX, labyGame.FinishY));
            if (marker.Count > 100) marker.RemoveAt(0);
          } break;

          case Keys.Add:
          {
            zoomLevel = Math.Min(zoomLevel + 1, zoomsWidth.Length - 1);
            fieldWidth = bre / zoomsWidth[zoomLevel];
            fieldHeight = hei / zoomsWidth[zoomLevel];
            goto case Keys.Enter;
          }

          case Keys.Subtract:
          {
            zoomLevel = Math.Max(zoomLevel - 1, 0);
            fieldWidth = bre / zoomsWidth[zoomLevel];
            fieldHeight = hei / zoomsWidth[zoomLevel];
            goto case Keys.Enter;
          }

          case Keys.Enter:
          {
            if (labyPicture.Width > fieldWidth && labyPicture.Height > fieldHeight)
            {
              if (labyPlayer)
              {
                offsetX = labyGame.PlayerX;
                offsetY = labyGame.PlayerY;
              }
              else
              {
                offsetX = labyGame.FinishX;
                offsetY = labyGame.FinishY;
              }
              offsetX -= fieldWidth / 2;
              offsetY -= fieldHeight / 2;
            }
            goto case Keys.Control;
          }

          case Keys.Control:
          {
            if (labyPicture.Width > fieldWidth && labyPicture.Height > fieldHeight)
            {
              if (offsetX < 0) offsetX = 0;
              if (offsetY < 0) offsetY = 0;
              if (offsetX > labyPicture.Width - fieldWidth) offsetX = labyPicture.Width - fieldWidth;
              if (offsetY > labyPicture.Height - fieldHeight) offsetY = labyPicture.Height - fieldHeight;
            }
          } break;

          case Keys.Escape: Close(); break;
        }

        if (labyGame.FinishReached)
        {
          DrawLaby();
          level++;
          InitGame();
        }

        DeadLineScanner();

      }
      DrawLaby();

      if (e.KeyCode == Keys.Insert) lastKeyInsert = Environment.TickCount;
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
    }
  }
}
