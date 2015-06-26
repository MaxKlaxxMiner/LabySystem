#region # using *.*

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LabySystem;
using System.Drawing.Imaging;

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
        gameGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
      }

      gameGraphics.DrawImage(labyPicture, new Rectangle(0, 0, gamePicture.Width, gamePicture.Height), -0.5f, -0.5f, (float)labyPicture.Width, (float)labyPicture.Height, GraphicsUnit.Pixel);

      gamePictureBox1.Refresh();
    }

    void InitGame()
    {
      if (labyGame != null) labyGame.Dispose();
      labyGame = new LabyGame(LabyGame.GetLevelSize(level).Item1, LabyGame.GetLevelSize(level).Item2, level * 1234567 * (DateTime.Now.Day + DateTime.Now.Year * 365 + DateTime.Now.Month * 372));
      labyPlayer = true;
      labyPicture = new Bitmap(labyGame.Width, labyGame.Height, PixelFormat.Format32bppRgb);
      labyGame.SetFieldChangeEvent((game, type, x, y) =>
      {
        switch (type)
        {
          case LabyGame.FieldType.wall: labyPicture.SetPixel(x, y, Color.Black); break;
          case LabyGame.FieldType.roomVisitedNone: labyPicture.SetPixel(x, y, Color.LightGray); break;
          case LabyGame.FieldType.roomVisitedFirst: labyPicture.SetPixel(x, y, Color.LightGoldenrodYellow); break;
          case LabyGame.FieldType.roomVisitedSecond:
          case LabyGame.FieldType.roomVisitedMore: labyPicture.SetPixel(x, y, Color.Yellow); break;
          default:
          {
            if ((type & LabyGame.FieldType.player) > 0) labyPicture.SetPixel(x, y, Color.Green);
            if ((type & LabyGame.FieldType.finish) > 0) labyPicture.SetPixel(x, y, Color.DarkRed);
          } break;
        }
      });
      labyGame.UpdateAll();
    }

    private void LabyForm_Load(object sender, EventArgs e)
    {
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.WindowState = FormWindowState.Maximized;
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
        case Keys.A: labyGame.MoveLeft(); break;
        case Keys.NumPad4:
        case Keys.Left: labyGame.MoveLeft(false); break;

        case Keys.D: labyGame.MoveRight(); break;
        case Keys.NumPad6:
        case Keys.Right: labyGame.MoveRight(false); break;

        case Keys.W: labyGame.MoveUp(); break;
        case Keys.NumPad8:
        case Keys.Up: labyGame.MoveUp(false); break;

        case Keys.S: labyGame.MoveDown(); break;
        case Keys.NumPad2:
        case Keys.Down: labyGame.MoveDown(false); break;

        case Keys.Space: labyPlayer = !labyPlayer; break;

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
