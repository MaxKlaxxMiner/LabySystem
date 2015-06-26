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
    static int level = 1;
    const int levelMax = 22;

    const char charRoom = ' ';
    const ConsoleColor colorRoom = ConsoleColor.DarkGray;

    const char charWall = '\x2588';
    const ConsoleColor colorWall = ConsoleColor.Black;

    const char charMen = '\x4';
    const ConsoleColor colorMan = ConsoleColor.Green;

    const char charWalked = '\x2591';
    const ConsoleColor colorWalked = ConsoleColor.Gray;

    const char charFinish = '\x3';
    const ConsoleColor colorFinish = ConsoleColor.Red;

    const char charWalked2 = '\x2591';
    const ConsoleColor colorWalked2 = ConsoleColor.Yellow;

    static void DrawField(LabyGame game, ILaby laby, int offsetX, int offsetY)
    {
      Console.BackgroundColor = colorRoom;
      Console.Clear();

      StringBuilder output = new StringBuilder();

      int maxX = Console.WindowWidth - 1;
      int maxY = Console.WindowHeight - 2;
      for (int y = offsetY; y < laby.Height; y++)
      {
        for (int x = offsetX; x < laby.Width; x++)
        {
          var f = game.GetField(x, y);

          switch (f)
          {
            case LabyGame.FieldType.wall: output.Append(charWall); break;
            case LabyGame.FieldType.roomVisitedFirst:
            {
              Console.ForegroundColor = colorWall;
              Console.Write(output.ToString());
              output.Clear();
              Console.ForegroundColor = colorWalked;
              Console.Write(charWalked);
            } break;
            case LabyGame.FieldType.roomVisitedSecond:
            case LabyGame.FieldType.roomVisitedMore:
            {
              Console.ForegroundColor = colorWall;
              Console.Write(output.ToString());
              output.Clear();
              Console.ForegroundColor = colorWalked2;
              Console.Write(charWalked2);
            } break;
            default: output.Append(charRoom); break;
          }

          if (x == laby.Width - 1) output.AppendLine();
          if (x - offsetX >= maxX) break;
        }
        if (y - offsetY >= maxY) break;
      }

      Console.ForegroundColor = colorWall;
      Console.Write(output.ToString());
    }

    static void Main()
    {
      while (level <= levelMax)
      {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.BackgroundColor = ConsoleColor.Black;
        Console.WriteLine("generate level {0} ({1} x {2})...", level, LabyGame.GetLevelSize(level).Item1.ToString("#,##0"), LabyGame.GetLevelSize(level).Item2.ToString("#,##0"));

        ILaby laby = new LabySimpleFast(LabyGame.GetLevelSize(level).Item1, LabyGame.GetLevelSize(level).Item2, level * 1234567 * (DateTime.Now.Day + DateTime.Now.Year * 365 + DateTime.Now.Month * 372));
        LabyGame game = new LabyGame(laby);

        long rest = 0;
        while ((rest = laby.Generate(500000)) > 0) Console.WriteLine((rest / 500000).ToString("#,##0"));

        int offsetX = 0;
        int offsetY = 0;
        DrawField(game, laby, offsetX, offsetY);

        game.SetFieldChangeEvent((l, t, x, y) =>
        {
          int cx = x - offsetX;
          int cy = y - offsetY;
          if (cx < 0 || cy < 0 || cx >= Console.WindowWidth || cy >= Console.WindowHeight - 1) return;
          Console.SetCursorPosition(cx, cy);
          switch (t)
          {
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
                Console.SetCursorPosition(cx, cy);
              }
              else if ((t & LabyGame.FieldType.finish) > 0)
              {
                Console.ForegroundColor = colorFinish;
                Console.Write(charFinish);
                Console.SetCursorPosition(cx, cy);
              }
            } break;
          }
        });

        game.Update(game.FinishX, game.FinishY);
        game.Update(game.PlayerX, game.PlayerY);

        bool finishMode = false;

        for (; ; )
        {
          var key = Console.ReadKey(true).Key;

          if (key == ConsoleKey.Escape)
          {
            level = int.MaxValue;
            break;
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

            case ConsoleKey.Spacebar: finishMode = !finishMode; goto case ConsoleKey.Enter;
            case ConsoleKey.Enter:
            {
              int tx = finishMode ? game.FinishX : game.PlayerX;
              int ty = finishMode ? game.FinishY : game.PlayerY;

              offsetX = tx - Console.WindowWidth / 2;
              offsetY = ty - Console.WindowHeight / 2;

              if (offsetX > game.Width - Console.WindowWidth) offsetX = game.Width - Console.WindowWidth + 1;
              if (offsetY > game.Height - Console.WindowHeight + 1) offsetY = game.Height - Console.WindowHeight + 1;
              if (offsetX < 0) offsetX = 0;
              if (offsetY < 0) offsetY = 0;

              DrawField(game, laby, offsetX, offsetY);

              if (finishMode)
              {
                game.Update(game.PlayerX, game.PlayerY);
                game.Update(game.FinishX, game.FinishY);
              }
              else
              {
                game.Update(game.FinishX, game.FinishY);
                game.Update(game.PlayerX, game.PlayerY);
              }
            } break;
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
