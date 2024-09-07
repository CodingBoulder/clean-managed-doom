using System;

namespace ManagedDoom.Silk;

public static class SilkProgram
{
    public static void Main(string[] args)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine(ApplicationInfo.Title);
        Console.ResetColor();

        try
        {
            string quitMessage;

            using (var app = new SilkDoom(new CommandLineArgs(args)))
            {
                app.Run();
                quitMessage = app.QuitMessage;
            }

            if (!string.IsNullOrWhiteSpace(quitMessage))
            {
                ExitMessage(
                    ConsoleColor.Green,
                    quitMessage);
            }
        }
        catch (Exception e)
        {
            ExitMessage(
                ConsoleColor.Red,
                e.ToString());
        }
    }

    private static void ExitMessage(
        ConsoleColor consoleColor,
        string? quitMessage)
    {
        Console.ForegroundColor = consoleColor;
        Console.WriteLine(quitMessage);
        Console.ResetColor();
        Console.Write("Press any key to exit.");
        Console.ReadKey();
    }
}
