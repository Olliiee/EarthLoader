using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Linq;
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

        /// <summary>
        /// Find the latest NASA image.
        /// </summary>
        private static void FindLatestImage()
        {
            bool runner = true;
            WebClient client = new WebClient();
            int daysOld = 0;
            int maxRetryCount = 0;
            XDocument xmlDocument = LoadXmlFile();

            // If the settings.xml file is available go on.
            if (xmlDocument != null)
            {
                int maxRetry = Convert.ToInt32(xmlDocument.Element("earth").Element("maxretry").Value);

                Console.WriteLine("Checking for the latest earth image ...");

                do
                {
                    // Only checking the last 21 days.
                    if (daysOld < 21)
                    {
                        // Start always with yesterday (not the beatles song!)
                        DateTime targeDate = DateTime.Today.AddDays(0 - daysOld); //yesterday.

                        try
                        {
                            // Build the URL for each day.
                            string nasaUrl = xmlDocument.Element("earth").Element("searchurl").Value
                                + targeDate.ToString("yyyyMMdd");

                            // Get the JSON 
                            client.Headers.Add("User-Agent: Other");
                            var imageJson = client.DownloadString(nasaUrl);

                            try
                            {
                                // Get all object of this day.
                                IEnumerable<EarthImage> output = JsonConvert.DeserializeObject<IEnumerable<EarthImage>>(imageJson);

                                // Anyone home?
                                if (output.Any())
                                {
                                    // No need to run this again.
                                    runner = false;
                                    daysOld = 22;

                                    Console.WriteLine("Found at " + targeDate.ToString("yyyy-MM-dd"));

                                    // Load the image and set a wallpaper.
                                    LoadEarthImage(output, client);

                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("Nothing found at " + targeDate.ToString("yyyy-MM-dd"));
                                    daysOld++;
                                }
                            }
                            catch
                            {
                                // No image available for this day. Look at the next day.
                                Console.WriteLine("Nothing found at " + targeDate.ToString("yyyy-MM-dd"));
                                daysOld++;
                            }
                        }
                        catch (Exception ex)
                        {
                            // Unable to check for images. Retry again.
                            maxRetryCount++;
                            Console.WriteLine("Unable to establish a connection.");

                            if (maxRetryCount == maxRetry)
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
        }

        /// <summary>
        /// Get the image data depending on the settings.
        /// </summary>
        /// <param name="imageList">The list of earth images.</param>
        /// <returns>Returns a single object or null.</returns>
        private static EarthImage GetImageObject(IEnumerable<EarthImage> imageList)
        {
            XDocument xmlDocument = LoadXmlFile();

            // Do we want a special timeframe?
            if (xmlDocument.Element("earth").Element("besttime").Attribute("active").Value == "true")
            {
                // Get the start time.
                DateTime startTime = DateTime.ParseExact(xmlDocument.Element("earth").Element("besttime").Value,
                    "HH:mm:ss",
                    System.Globalization.CultureInfo.InvariantCulture);

                // Calc the end time.
                DateTime endTime = startTime.AddHours(Convert.ToDouble(xmlDocument.Element("earth").Element("besttime").Attribute("maxhours").Value));

                // Select the first object within the list.
                return imageList.Where(c => c.date.TimeOfDay >= startTime.TimeOfDay && c.date.TimeOfDay <= endTime.TimeOfDay)
                    .OrderByDescending(c => c.date)
                    .FirstOrDefault();
            }
            else
            {
                // Return the the first object ordered by date.
                return imageList.OrderByDescending(c => c.date).FirstOrDefault();
            }
        }

        /// <summary>
        /// Download the image and set as wallpaper.
        /// </summary>
        /// <param name="imageList">The earth image object list.</param>
        /// <param name="client">The webclient object.</param>
        private static void LoadEarthImage(IEnumerable<EarthImage> imageList, WebClient client)
        {
            XDocument xmlDocument = LoadXmlFile();
            EarthImage earth = GetImageObject(imageList);

            if (earth != null)
            {
                Console.WriteLine("The latest image " + earth.date);

                // Build the download url.
                string imageUrl = @xmlDocument.Element("earth").Element("imagepath").Value + earth.image + ".png";

                Console.WriteLine("Loading image ... " + imageUrl);

                // Get the wallpaper settings.
                var style = xmlDocument.Elements("earth")
                    .Elements("wallpaperstyle")
                    .Elements("style")
                    .Where(c => c.Attribute("active").Value == "true")
                    .FirstOrDefault();

                Wallpaper.Set(imageUrl, client, style.Attribute("WallpaperStyle").Value, style.Attribute("TileWallpaper").Value);

                Console.WriteLine("Done.");
            }
            else
            {
                Console.WriteLine("No new image within the timerange available.");
            }
        }

        /// <summary>
        /// Load the settings.xml file.
        /// </summary>
        /// <returns>Returns the xdocument.</returns>
        private static XDocument LoadXmlFile()
        {
            XDocument xmlDocument = null;

            if (File.Exists(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Settings.xml"))
            {
                xmlDocument = XDocument.Load(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Settings.xml");
            }
            else
            {
                Console.WriteLine("No settings.xml file available.");
            }

            return xmlDocument;
        }

        /// <summary>
        /// Main entry point.
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            MinimizeConsoleWindow();
            FindLatestImage();
        }

        /// <summary>
        /// Minimize the window.
        /// </summary>
        private static void MinimizeConsoleWindow()
        {
            IntPtr hWndConsole = GetConsoleWindow();
            ShowWindow(hWndConsole, SW_MINIMIZE);
        }

        #endregion Private Methods
    }
}