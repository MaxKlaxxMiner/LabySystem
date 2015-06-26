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
  static class Program
  {
    static void Main()
    {
      int level = 1;
      int levelMax = 45;

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

      var charWalked2 = '\x2591';
      var colorWalked2 = ConsoleColor.Yellow;

      while (level < levelMax)
      {
        Console.ForegroundColor = colorMan;
        Console.BackgroundColor = colorRoom;
        Console.Write("generate...");

        ILaby laby = new LabySimpleFast(LabyGame.GetLevelSize(level).Item1, LabyGame.GetLevelSize(level).Item2, level * 1234567 * (DateTime.Now.Day + DateTime.Now.Year * 365 + DateTime.Now.Month * 372));
        LabyGame game = new LabyGame(laby);

        while (laby.Generate(10000) > 0) { }

        StringBuilder output = new StringBuilder();
        for (int y = 0; y < laby.Height; y++)
        {
          for (int x = 0; x < laby.Width; x++) output.Append(laby.GetWall(x, y) ? charWall : charRoom);
          output.AppendLine();
        }

        Console.Clear();
        Console.ForegroundColor = colorWall;
        Console.Write(output.ToString());

        game.SetFieldChangeEvent((l, t, x, y) =>
        {
          Console.SetCursorPosition(x, y);
          switch (t)
          {
            case LabyGame.FieldType.wall:
            {
              Console.ForegroundColor = colorWall;
              Console.Write(charWall);
            } break;
            case LabyGame.FieldType.roomVisitedNone:
            {
              Console.ForegroundColor = colorWall;
              Console.Write(charRoom);
            } break;
            case LabyGame.FieldType.roomVisitedFirst:
            {
              Console.ForegroundColor = colorWalked;
              Console.Write(charWalked);
            } break;
            case LabyGame.FieldType.roomVisitedSecond:
            case LabyGame.FieldType.roomVisitedMore:
            {
              Console.ForegroundColor = colorWalked2;
              Console.Write(charWalked2);
            } break;
            default:
            {
              if ((t & LabyGame.FieldType.player) > 0)
              {
                Console.ForegroundColor = colorMan;
                Console.Write(charMen);
              }
              else if ((t & LabyGame.FieldType.finish) > 0)
              {
                Console.ForegroundColor = colorFinish;
                Console.Write(charFinish);
              }
            } break;
          }
        });

        game.Update(game.PlayerX, game.PlayerY);
        game.Update(game.FinishX, game.FinishY);

        bool finishMode = false;

        for (; ; )
        {
          if (finishMode) Console.SetCursorPosition(game.FinishX, game.FinishY); else Console.SetCursorPosition(game.PlayerX, game.PlayerY);

          var key = Console.ReadKey(true).Key;

          if (key == ConsoleKey.Escape)
          {
            level = int.MaxValue;
            break;
          }

          if (key == ConsoleKey.Spacebar)
          {
            finishMode = !finishMode;
          }

          switch (key)
          {
            case ConsoleKey.LeftArrow:
            case ConsoleKey.A:
            case ConsoleKey.NumPad4: game.MoveLeft(!finishMode); break;

            case ConsoleKey.RightArrow:
            case ConsoleKey.D:
            case ConsoleKey.NumPad6: game.MoveRight(!finishMode); break;

            case ConsoleKey.UpArrow:
            case ConsoleKey.W:
            case ConsoleKey.NumPad8: game.MoveUp(!finishMode); break;

            case ConsoleKey.DownArrow:
            case ConsoleKey.S:
            case ConsoleKey.NumPad2: game.MoveDown(!finishMode); break;
          }

          if (game.FinishReached)
          {
            level++;
            break;
          }
        }
      }
    }
  }
}
