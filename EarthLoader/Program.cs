using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;

namespace EarthLoader
{
    internal class Program
    {
        #region Private Methods

        private static void FindLatestImage()
        {
            bool runner = true;
            WebClient client = new WebClient();
            int daysOld = 2;

            Console.WriteLine("Checking for the latest earth image ...");

            do
            {
                if (daysOld < 21)
                {
                    DateTime targeDate = DateTime.Today.AddDays(0 - daysOld); //yesterday.

                    String api_url = "http://epic.gsfc.nasa.gov/api/images.php?date=" + targeDate.ToString("yyyyMMdd");
                    var raw_jason = client.DownloadString(api_url);

                    try
                    {
                        var output = JsonConvert.DeserializeObject<IEnumerable<EarthImage>>(raw_jason);
                        if (output.Any())
                        {
                            Console.WriteLine("Found at " + targeDate.ToString("yyyy-MM-dd"));
                            LoadEarthImage(output, client);
                            runner = false;
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
            }
            while (runner);
        }

        private static void LoadEarthImage(IEnumerable<EarthImage> imageList, WebClient client)
        {
            EarthImage earth = imageList.OrderByDescending(c => c.date).FirstOrDefault();

            Console.WriteLine("The latest image " + earth.date);

            string imageUrl = @"http://epic.gsfc.nasa.gov/epic-archive/png/" + earth.image + ".png";

            Console.WriteLine("Loading image ...");
            Wallpaper.Set(imageUrl, Wallpaper.Style.Filled, client);
            Console.WriteLine("Done.");
        }

        private static void Main(string[] args)
        {
            FindLatestImage();
        }

        #endregion Private Methods
    }
}