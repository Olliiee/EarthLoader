using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace EarthLoader
{
    public class Wallpaper
    {
        #region Private Fields

        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_SENDWININICHANGE = 0x02;
        private const int SPIF_UPDATEINIFILE = 0x01;

        #endregion Private Fields

        #region Public Enums

        public enum Style
        {
            Tiled,
            Centered,
            Stretched,
            Filled
        }

        #endregion Public Enums

        #region Public Methods

        public static void Set(string uri, Style style, WebClient client)
        {
            System.IO.Stream s = client.OpenRead(uri);

            System.Drawing.Image img = System.Drawing.Image.FromStream(s);
            string tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
            img.Save(tempPath, System.Drawing.Imaging.ImageFormat.Bmp);

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            if (style == Style.Stretched)
            {
                key.SetValue(@"WallpaperStyle", 2.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (style == Style.Centered)
            {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (style == Style.Tiled)
            {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 1.ToString());
            }

            if (style == Style.Filled)
            {
                key.SetValue(@"WallpaperStyle", 10.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                tempPath,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

        #endregion Public Methods

        #region Private Methods

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        #endregion Private Methods
    }
}