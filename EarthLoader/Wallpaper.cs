using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace EarthLoader
{
    /// <summary>
    /// Get and set the wallpaper. 
    /// </summary>
    public class Wallpaper
    {
        #region Private Fields

        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_SENDWININICHANGE = 0x02;
        private const int SPIF_UPDATEINIFILE = 0x01;

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Download and set the new wallpaper. 
        /// </summary>
        /// <param name="uri">            The uri for the image. </param>
        /// <param name="client">         The webclient. </param>
        /// <param name="wallpaperStyle"> The wallpaper style (centered, tiled, ..) </param>
        /// <param name="tileWallpaper">  The wallpaper tile setting. </param>
        public static void Set(string uri, WebClient client, string wallpaperStyle, string tileWallpaper)
        {
            // Create a stream to load the image.
            try
            {
                client.Headers.Add("User-Agent: Other");
                Stream s = client.OpenRead(uri);

                // Create an image depending on the stream.
                System.Drawing.Image img = System.Drawing.Image.FromStream(s);

                // Save the file to the users temp folder.
                string tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
                img.Save(tempPath, System.Drawing.Imaging.ImageFormat.Bmp);

                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

                key.SetValue(@"WallpaperStyle", wallpaperStyle);
                key.SetValue(@"TileWallpaper", tileWallpaper);

                // Set the new wallpaper.
                SystemParametersInfo(SPI_SETDESKWALLPAPER,
                    0,
                    tempPath,
                    SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        #endregion Public Methods

        #region Private Methods

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        #endregion Private Methods
    }
}