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

      char charWall = '\x2588';
      char charRoom = ' ';
      char charMen = '\x4';
      char charFinish = '\x3';
      char charWalked = '\x2591';

      while (level <= 20)
      {
        int gameWidth = (gameWidthMax * level / 20) / 2 * 2 + 1;
        int gameHeight = (gameHeightMax * level / 20) / 2 * 2 + 1;

        ILaby demo = new LabySimple(gameWidth, gameHeight, 11111 * gameWidth * gameHeight);

        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("generate...");

        while (demo.Generate(100) > 0) ;

        Console.Clear();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        for (int y = 0; y < demo.Height; y++)
        {
          for (int x = 0; x < demo.Width; x++)
          {
            Console.Write(demo.GetWall(x, y) ? charWall : charRoom);
          }
          Console.WriteLine();
        }

        Console.ForegroundColor = ConsoleColor.Red;
        Console.SetCursorPosition(1, 1);
        Console.Write(charFinish);

        int px = demo.Width - 2;
        int py = demo.Height - 2;

        for (; ; )
        {
          Console.ForegroundColor = ConsoleColor.Yellow;
          Console.SetCursorPosition(px, py);
          Console.Write(charMen);
          Console.SetCursorPosition(px, py);

          var key = Console.ReadKey().Key;
          if (key == ConsoleKey.Escape)
          {
            level = int.MaxValue;
            break;
          }

          Console.ForegroundColor = ConsoleColor.DarkGray;
          Console.SetCursorPosition(px, py); Console.Write(charWalked);

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

          if (px == 1 && py == 1)
          {
            level++;
            break;
          }
        }
      }
    }
  }
}
