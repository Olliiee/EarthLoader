using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace EarthLoader
{
    internal class Program
    {
        #region Private Fields

        private const Int32 SW_MINIMIZE = 6;

        #endregion Private Fields

        #region Private Methods

        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow([In] IntPtr hWnd, [In] Int32 nCmdShow);

        #endregion Private Methods

        #region Private Methods

        private static void FindLatestImage()
        {
            bool runner = true;
            WebClient client = new WebClient();
            int daysOld = 0;
            int maxRetry = 0;

            Console.WriteLine("Checking for the latest earth image ...");

            do
            {
                if (daysOld < 21)
                {
                    DateTime targeDate = DateTime.Today.AddDays(0 - daysOld); //yesterday.

                    try
                    {
                        string api_url = "http://epic.gsfc.nasa.gov/api/images.php?date=" + targeDate.ToString("yyyyMMdd");
                        var raw_jason = client.DownloadString(api_url);

                        try
                        {
                            var output = JsonConvert.DeserializeObject<IEnumerable<EarthImage>>(raw_jason);
                            if (output.Any())
                            {
                                runner = false;
                                daysOld = 22;
                                Console.WriteLine("Found at " + targeDate.ToString("yyyy-MM-dd"));
                                LoadEarthImage(output, client);
                                
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Nothing found at " + targeDate.ToString("yyyy-MM-dd"));
                                daysOld++;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Nothing found at " + targeDate.ToString("yyyy-MM-dd"));
                            daysOld++;
                        }
                    }
                    catch (Exception)
                    {
                        maxRetry++;
                        Console.WriteLine("Unable to establish a connection.");

                        if (maxRetry == 5)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    runner = false;
                    break;
                }
            }
            while (runner);
        }

        private static void LoadEarthImage(IEnumerable<EarthImage> imageList, WebClient client)
        {
            EarthImage earth = imageList.OrderByDescending(c => c.date).FirstOrDefault();

            Console.WriteLine("The latest image " + earth.date);

            string imageUrl = @"http://epic.gsfc.nasa.gov/epic-archive/png/" + earth.image + ".png";

            Console.WriteLine("Loading image ... " + imageUrl);
            Wallpaper.Set(imageUrl, Wallpaper.Style.Filled, client);
            Console.WriteLine("Done.");
        }

        private static void Main(string[] args)
        {
            MinimizeConsoleWindow();
            FindLatestImage();
        }

        private static void MinimizeConsoleWindow()
        {
            IntPtr hWndConsole = GetConsoleWindow();
            ShowWindow(hWndConsole, SW_MINIMIZE);
        }

        #endregion Private Methods
    }
}