using System;
using System.Diagnostics;
using System.Threading;

class Program
{
    static void Main()
    {
        DisplayWelcomeScreen();

        while (true)
        {
            Console.Clear();
            DisplayMenu();

            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > 5)
            {
                ShowInvalidInputMessage();
                continue;
            }

            if (choice == 5)
            {
                Console.WriteLine("程序已退出");
                break;
            }

            int seconds = choice switch
            {
                1 => 10,
                2 => 60,
                3 => 20,
                4 => 120,
                _ => 0
            };

            StartCountdown(seconds);
        }
    }

    static void DisplayWelcomeScreen()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;

        Console.WriteLine("==================================================");
        Console.WriteLine("||                                              ||");
        Console.WriteLine("||          口语考试模拟倒计时软件 v1.2         ||");
        Console.WriteLine("||      (Oral Exam Timer with High Precision)   ||");
        Console.WriteLine("||                                              ||");
        Console.WriteLine("==================================================");
        Console.WriteLine();
        Console.WriteLine(" 欢迎使用! 本软件专为口语考试练习设计");
        Console.WriteLine(" 功能特点:");
        Console.WriteLine(" - 多种预设考试时间选项");
        Console.WriteLine(" - 倒计时开始/结束提示音");
        Console.WriteLine(" - 实时剩余时间显示");
        Console.WriteLine(" 提供高精度倒计时和考试提示音功能");
        Console.WriteLine();
        Console.WriteLine(" 更新内容:");
        Console.WriteLine(" - 添加高精度计时器（毫秒级精度）");
        Console.WriteLine(" - 最后5秒显示毫秒倒计时");
        Console.WriteLine();
        Console.WriteLine("==================================================");
        Console.ResetColor();

        Console.WriteLine("\n按任意键继续...");
        Console.ReadKey();
    }

    static void DisplayMenu()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("=== 考试倒计时选项（毫秒级精度） ===");
        Console.ResetColor();
        Console.WriteLine("1. 10秒 (问题准备/回答)");
        Console.WriteLine("2. 60秒 (短文朗读)");
        Console.WriteLine("3. 20秒 (短文问题回答)");
        Console.WriteLine("4. 120秒 (交流)");
        Console.WriteLine("5. 退出程序");
        Console.Write("\n请选择倒计时时长 (1-5): ");
    }

    static void ShowInvalidInputMessage()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("无效输入，请输入1-5之间的数字!");
        Console.ResetColor();
        Thread.Sleep(1500);
    }

    static void StartCountdown(int totalSeconds)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"倒计时开始：{FormatTime(totalSeconds)}");
        Console.ResetColor();

        BeepAlert(); // 开始提示音

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        TimeSpan totalTime = TimeSpan.FromSeconds(totalSeconds);
        TimeSpan remaining = totalTime;
        TimeSpan lastDisplay = TimeSpan.Zero;

        while (remaining.TotalMilliseconds > 0)
        {
            remaining = totalTime - stopwatch.Elapsed;

            // 根据时间动态更新频率
            bool shouldUpdate = false;
            if (remaining.TotalSeconds > 5)
            {
                // 大于5秒时每秒更新
                shouldUpdate = (int)remaining.TotalSeconds != (int)lastDisplay.TotalSeconds;
            }
            else
            {
                // 最后5秒每100毫秒更新
                shouldUpdate = (int)(remaining.TotalMilliseconds / 100) != (int)(lastDisplay.TotalMilliseconds / 100);
            }

            if (shouldUpdate)
            {
                DisplayRemainingTime(remaining, totalSeconds);
                lastDisplay = remaining;
            }

            Thread.Sleep(10);
        }

        DisplayRemainingTime(TimeSpan.Zero, totalSeconds);

        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("\n\n时间到！");
        Console.ResetColor();

        BeepAlert(); // 结束提示音

        Console.WriteLine("\n按任意键返回菜单...");
        Console.ReadKey();
    }

    static void DisplayRemainingTime(TimeSpan remaining, int totalSeconds)
    {
        Console.SetCursorPosition(0, 1);

        if (remaining.TotalSeconds > 5)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"剩余时间: {FormatTime((int)remaining.TotalSeconds)}  ");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if (remaining.TotalMilliseconds > 0)
            {
                Console.WriteLine($"剩余时间: {remaining.Seconds:D2}.{remaining.Milliseconds / 100:D1}秒 ");
            }
            else
            {
                Console.WriteLine($"剩余时间: 00.0秒 ");
            }
        }

        double progress = 1.0 - (remaining.TotalSeconds / totalSeconds);
        DrawProgressBar(progress);
    }

    // 修复bug的关键函数：正确格式化分钟和秒
    static string FormatTime(int totalSeconds)
    {
        if (totalSeconds < 60)
        {
            return $"{totalSeconds}秒";
        }
        else
        {
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            return $"{minutes:D2}:{seconds:D2}";
        }
    }

    static void DrawProgressBar(double progress)
    {
        int width = 50;
        int position = (int)(progress * width);

        Console.SetCursorPosition(0, 3);
        Console.Write("[");
        for (int i = 0; i < width; i++)
        {
            if (i < position)
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.Write(" ");
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.Write(" ");
            }
        }
        Console.ResetColor();
        Console.Write("]");

        Console.SetCursorPosition(width + 3, 3);
        Console.Write($"{progress * 100:F0}%");
    }

    static void BeepAlert()
    {
        try
        {
            Console.Beep(800, 300);
        }
        catch
        {
            Console.Write("\a");
        }
    }
}