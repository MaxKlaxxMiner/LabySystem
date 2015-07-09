#region # using *.*

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
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

    Bitmap gamePicture = new Bitmap(1, 1, PixelFormat.Format32bppRgb);
    Graphics gameGraphics;
    int level = 1;
    LabyGame labyGame;
    bool labyPlayer = true;
    Bitmap labyPicture;
    int offsetX;
    int offsetY;

    /// <summary>
    /// minimale Kachelgröße in Pixeln (Multiplikator von 2,2,2,2,3,5)
    /// </summary>
    const int minWidth = 2 * 2 * 2 * 2;
    const int fieldWidth = 1920 / minWidth;
    const int fieldHeight = 1080 / minWidth;
    const int fieldJumps = 6;

    List<Size> marker = new List<Size>();
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

        while (labyPicture.Width * (drawMul + 1) < 1920 && labyPicture.Height * (drawMul + 1) < 1080) drawMul++;

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

      gamePictureBox1.Refresh();
    }

    #region # void InitGame() // Spielfeld initialisieren und zeichnen
    /// <summary>
    /// Spielfeld initialisieren und zeichnen
    /// </summary>
    void InitGame()
    {
      if (level > 1)
      {
        MessageBox.Show("Level: " + level + " (" + LabyGame.GetLevelSize(level).Item1.ToString("#,##0") + " x " + LabyGame.GetLevelSize(level).Item2.ToString("#,##0") + ")", "next Level");
      }
      if (labyGame != null) labyGame.Dispose();
      labyGame = new LabyGame(LabyGame.GetLevelSize(level).Item1, LabyGame.GetLevelSize(level).Item2, level * 1234567 * (DateTime.Now.Day + DateTime.Now.Year * 365 + DateTime.Now.Month * 372));
      labyPlayer = true;
      labyPicture = new Bitmap(labyGame.Width, labyGame.Height, PixelFormat.Format32bppRgb);
      offsetX = 0;
      offsetY = 0;
      marker = new List<Size>();
      #region # // --- Spielfeld zeichnen ---
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
          case LabyGame.FieldType.roomVisitedNone: labyPicture.SetPixel(x, y, Color.LightGray); break;
          case LabyGame.FieldType.roomVisitedFirst: labyPicture.SetPixel(x, y, marker.Any(m => m.Width == x && m.Height == y) ? Color.Coral : Color.LightGoldenrodYellow); break;
          case LabyGame.FieldType.roomVisitedSecond:
          case LabyGame.FieldType.roomVisitedMore: labyPicture.SetPixel(x, y, marker.Any(m => m.Width == x && m.Height == y) ? Color.Coral : Color.Yellow); break;
          default:
          {
            if ((type & LabyGame.FieldType.player) > 0) labyPicture.SetPixel(x, y, Color.Green);
            if ((type & LabyGame.FieldType.finish) > 0) labyPicture.SetPixel(x, y, Color.DarkRed);
          } break;
        }
      });
      labyGame.UpdateAll();
      #endregion
    }
    #endregion

    private void LabyForm_Load(object sender, EventArgs e)
    {
      FormBorderStyle = FormBorderStyle.None;
      WindowState = FormWindowState.Maximized;
      InitGame();
    }

    private void gameTimer_Tick(object sender, EventArgs e)
    {
      DrawLaby();
    }

    private void LabyForm_KeyDown(object sender, KeyEventArgs e)
    {
      switch (e.KeyCode)
      {
        case Keys.A:
        case Keys.NumPad4:
        case Keys.Left:
        {
          labyGame.MoveLeft(labyPlayer);
          if ((labyPlayer ? labyGame.PlayerX : labyGame.FinishX) < offsetX + fieldJumps)
          {
            offsetX = labyPlayer ? labyGame.PlayerX : labyGame.FinishX;
            offsetX -= fieldWidth / 2;
            goto case Keys.Control;
          }
        } break;

        case Keys.D:
        case Keys.NumPad6:
        case Keys.Right:
        {
          labyGame.MoveRight(labyPlayer);
          if ((labyPlayer ? labyGame.PlayerX : labyGame.FinishX) >= offsetX + fieldWidth - fieldJumps)
          {
            offsetX = labyPlayer ? labyGame.PlayerX : labyGame.FinishX;
            offsetX -= fieldWidth / 2;
            goto case Keys.Control;
          }
        } break;

        case Keys.W:
        case Keys.NumPad8:
        case Keys.Up:
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

        case Keys.S:
        case Keys.NumPad2:
        case Keys.Down:
        {
          labyGame.MoveDown(labyPlayer);
          if ((labyPlayer ? labyGame.PlayerY : labyGame.FinishY) >= offsetY + fieldHeight - fieldJumps)
          {
            offsetY = labyPlayer ? labyGame.PlayerY : labyGame.FinishY;
            offsetY -= fieldHeight / 2;
            goto case Keys.Control;
          }
        } break;

        case Keys.Space: labyPlayer = !labyPlayer; goto case Keys.Enter;

        case Keys.Back:
        {
          marker.Add(labyPlayer ? new Size(labyGame.PlayerX, labyGame.PlayerY) : new Size(labyGame.FinishX, labyGame.FinishY));
          if (marker.Count > 100) marker.RemoveAt(0);
        } break;

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
        level++;
        InitGame();
      }
    }
  }
}
