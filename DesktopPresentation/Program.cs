using System;
using System.Runtime.InteropServices;
using System.IO;

namespace DesktopPresentation
{

    // a joke console app that change desktop wallpaper using arrow keys.

    // usage:
    //
    // DesktopPresentation {your_presentation_directory}
    //
    // {your_presentation_directory} should contain numbered PNG files like...
    // 1.png
    // 2.png
    // 3.png
    // ...
    //
    // 

    internal class Program
    {
        static int getNextPage(int currentIndex, string[] files)
        {
            return ++currentIndex != files.Length ? currentIndex : files.Length - 1;
        }

        static int getPreviousPage(int currentIndex, string[] files)
        {
            return --currentIndex >= 0 ? currentIndex : 0;
        }

        static void Main(string[] args)
        {
            int pageIndex = 0;
            var presentationDir = args[0];
            FileAttributes attr = File.GetAttributes(presentationDir);
            if (attr != FileAttributes.Directory) {
                Console.WriteLine("Please specify valid directory that contains your presentation.");
                Environment.Exit(0);
            }

            var files = Directory.GetFiles(presentationDir, "*.png");
            foreach (var file in files) {
                Console.WriteLine(file);
            }
            if(files.Length == 0)
            {
                Console.WriteLine("No content.");
                Environment.Exit(0);
            }

            string photo = Path.Combine(presentationDir, files[0]);
            DisplayPicture(photo);

            while (true)
            {
                var ch = Console.ReadKey(false).Key;
                switch (ch)
                {
                    case ConsoleKey.Escape:
                        Environment.Exit(0);
                        break;
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.Spacebar:
                    case ConsoleKey.DownArrow:
                        pageIndex = getNextPage(pageIndex, files);
                        DisplayPicture(Path.Combine(presentationDir, files[pageIndex]));
                        break;
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.UpArrow:
                        pageIndex = getPreviousPage(pageIndex, files);
                        DisplayPicture(Path.Combine(presentationDir, files[pageIndex]));
                        break;
                }
            }

        }

        // method "DisplayPicture" from
        // https://www.codeproject.com/Questions/1252479/How-do-I-change-the-desktop-background-using-Cshar


        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SystemParametersInfo(uint uiAction, uint uiParam, String pvParam, uint fWinIni);

        private const uint SPI_SETDESKWALLPAPER = 0x14;
        private const uint SPIF_UPDATEINIFILE = 0x1;
        private const uint SPIF_SENDWININICHANGE = 0x2;

        private static void DisplayPicture(string file_name)
        {
            uint flags = 0;
            if (!SystemParametersInfo(SPI_SETDESKWALLPAPER,
                    0, file_name, flags))
            {
                Console.WriteLine("Error");
            }
        }
    }
}
