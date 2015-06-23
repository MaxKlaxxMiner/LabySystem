#region # using *.*

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabySystem;

#endregion

namespace LabyConsole
{
  class Program
  {
    static void Main(string[] args)
    {
      int level = 0;
      int gameWidthMax = 159;
      int gameHeightMax = 49;

      var charRoom = ' ';
      var colorRoom = ConsoleColor.DarkGray;

      var charWall = '\x2588';
      var colorWall = ConsoleColor.Black;

      var charMen = '\x4';
      var colorMan = ConsoleColor.Green;

      var charWalked = '\x2591';
      var colorWalked = ConsoleColor.Gray;

      var charFinish = '\x3';
      var colorFinish = ConsoleColor.Red;

      while (level <= 20)
      {
        int gameWidth = (gameWidthMax * level / 20) / 2 * 2 + 1;
        int gameHeight = (gameHeightMax * level / 20) / 2 * 2 + 1;

        ILaby demo = new LabySimpleFast(gameWidth, gameHeight, (DateTime.Now.Day + DateTime.Now.Year * 365 + DateTime.Now.Month * 372) * gameWidth * gameHeight);

        Console.ForegroundColor = colorMan;
        Console.BackgroundColor = colorRoom;
        Console.Write("generate...");

        while (demo.Generate(10000) > 0) ;

        StringBuilder output = new StringBuilder();
        for (int y = 0; y < demo.Height; y++)
        {
          for (int x = 0; x < demo.Width; x++) output.Append(demo.GetWall(x, y) ? charWall : charRoom);
          output.AppendLine();
        }

        Console.Clear();
        Console.ForegroundColor = colorWall;

        Console.Write(output.ToString());

        int px = 1;
        int py = 1;

        int fx = demo.Width - 2;
        int fy = demo.Height - 2;

        bool finishMode = false;

        for (; ; )
        {
          Console.SetCursorPosition(fx, fy);
          Console.ForegroundColor = colorFinish;
          Console.Write(charFinish);

          Console.SetCursorPosition(px, py);
          Console.ForegroundColor = colorMan;
          Console.Write(charMen);

          if (finishMode) Console.SetCursorPosition(fx, fy); else Console.SetCursorPosition(px, py);

          var key = Console.ReadKey().Key;

          if (key == ConsoleKey.Escape)
          {
            level = int.MaxValue;
            break;
          }

          if (key == ConsoleKey.Spacebar)
          {
            finishMode = !finishMode;
          }

          if (finishMode)
          {
            Console.SetCursorPosition(fx, fy);
            Console.ForegroundColor = colorWalked;
            Console.Write(charWalked);

            if ((key == ConsoleKey.LeftArrow || key == ConsoleKey.A || key == ConsoleKey.NumPad4) && !demo.GetWall(fx - 1, fy))
            {
              Console.SetCursorPosition(fx - 1, fy); Console.Write(charWalked);
              fx -= 2;
            }
            if ((key == ConsoleKey.RightArrow || key == ConsoleKey.D || key == ConsoleKey.NumPad6) && !demo.GetWall(fx + 1, fy))
            {
              Console.SetCursorPosition(fx + 1, fy); Console.Write(charWalked);
              fx += 2;
            }
            if ((key == ConsoleKey.UpArrow || key == ConsoleKey.W || key == ConsoleKey.NumPad8) && !demo.GetWall(fx, fy - 1))
            {
              Console.SetCursorPosition(fx, fy - 1); Console.Write(charWalked);
              fy -= 2;
            }
            if ((key == ConsoleKey.DownArrow || key == ConsoleKey.S || key == ConsoleKey.NumPad2) && !demo.GetWall(fx, fy + 1))
            {
              Console.SetCursorPosition(fx, fy + 1); Console.Write(charWalked);
              fy += 2;
            }
          }
          else
          {
            Console.SetCursorPosition(px, py);
            Console.ForegroundColor = colorWalked;
            Console.Write(charWalked);

            if ((key == ConsoleKey.LeftArrow || key == ConsoleKey.A || key == ConsoleKey.NumPad4) && !demo.GetWall(px - 1, py))
            {
              Console.SetCursorPosition(px - 1, py); Console.Write(charWalked);
              px -= 2;
            }
            if ((key == ConsoleKey.RightArrow || key == ConsoleKey.D || key == ConsoleKey.NumPad6) && !demo.GetWall(px + 1, py))
            {
              Console.SetCursorPosition(px + 1, py); Console.Write(charWalked);
              px += 2;
            }
            if ((key == ConsoleKey.UpArrow || key == ConsoleKey.W || key == ConsoleKey.NumPad8) && !demo.GetWall(px, py - 1))
            {
              Console.SetCursorPosition(px, py - 1); Console.Write(charWalked);
              py -= 2;
            }
            if ((key == ConsoleKey.DownArrow || key == ConsoleKey.S || key == ConsoleKey.NumPad2) && !demo.GetWall(px, py + 1))
            {
              Console.SetCursorPosition(px, py + 1); Console.Write(charWalked);
              py += 2;
            }
          }

          if (px == fx && py == fy)
          {
            level++;
            break;
          }
        }
      }
    }
  }
}
