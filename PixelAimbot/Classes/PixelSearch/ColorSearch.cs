using Newtonsoft.Json.Linq;
using PixelAimbot.Classes.Misc;
using PixelAimbot.Classes.OpenCV;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PixelAimbot
{
    public partial class ColorSearch
    {
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);
  
        #region LOCKBIT
        //Diese Methode sucht nach einer bestimmten Farbe auf dem Bildschirm und verschiebt den Cursor zu dieser Position.
        //Es werden alle verfügbaren Bildschirme durchlaufen und die Bilddaten in eine Liste von Bitmaps gespeichert.
        //Es wird dann parallel über die Bilddaten iteriert und die Farben der Pixel verglichen, um die gewünschte Farbe zu finden.
        //Sobald die Farbe gefunden wurde, wird die Entfernung zur aktuellen Cursorposition berechnet und gespeichert,
        //falls es die nächste zur aktuellen Cursorposition ist.
        //Der Cursor wird dann zur Position der gefundenen Farbe verschoben und die Position als Point zurückgegeben.
        public static Point SearchAndMove(Color colorToFind, int tolerance, int inHeightMin, int inHeightMax,
            int inWidthMin, int inWidthMax, bool useScreen1, bool expandSearch, bool expandSearchEnabled, bool startMiddle)
        {
            // Erstellen einer Liste von Bitmaps, in die die Bilddaten aller verfügbaren Bildschirme gespeichert werden
            List<Bitmap> screens = new List<Bitmap>();

            // Durchlaufen aller verfügbaren Bildschirme und nur den angegebenen Bildschirm verwenden
            // (falls useScreen1 true ist) oder nicht den angegebenen Bildschirm verwenden (falls useScreen1 false ist)
            foreach (var screen in Screen.AllScreens)
            {
                // only use the specified screen
                if (useScreen1 && screen.Primary || !useScreen1 && !screen.Primary)
                {
                    // Erstellen einer Bitmap aus den Bilddaten des aktuellen Bildschirms und diese in die Liste von Bitmaps hinzufügen
                    Bitmap bmp = new Bitmap(screen.Bounds.Width, screen.Bounds.Height);
                    using (Graphics g = Graphics.FromImage(bmp))
                    using (Bitmap source = new Bitmap(screen.Bounds.Width, screen.Bounds.Height))
                    using (Graphics memoryGraphics = Graphics.FromImage(source))
                    {
                        memoryGraphics.CopyFromScreen(screen.Bounds.X, screen.Bounds.Y, 0, 0, source.Size);
                        g.DrawImage(source, 0, 0);
                    }
                    screens.Add(bmp);
                }
            }

            Point cursor;
            int closestX = -1;
            int closestY = -1;
            double closestDistance = double.MaxValue;
            bool colorFound = false;

            // Verwenden von paralleler Programmierung, um über die Liste von Bitmaps zu iterieren und die Farben der Pixel zu vergleichen, um die gewünschte Farbe zu finden
            Parallel.For(0, screens.Count, i =>
            {
                BitmapData bmpData = screens[i].LockBits(new Rectangle(0, 0, screens[i].Width, screens[i].Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                IntPtr ptr = bmpData.Scan0;
                int bytes = Math.Abs(bmpData.Stride) * screens[i].Height;
                byte[] rgbValues = new byte[bytes];
                Marshal.Copy(ptr, rgbValues, 0, bytes);


                int searchWidthMin = inWidthMin;
                int searchWidthMax = inWidthMax;
                int searchHeightMin = inHeightMin;
                int searchHeightMax = inHeightMax;

                if (startMiddle)
                {
                    int middleWidth = (inWidthMax + inWidthMin) / 2;
                    int middleHeight = (inHeightMax + inHeightMin) / 2;

                    searchWidthMin = middleWidth - (middleWidth - inWidthMin) / 2;
                    searchWidthMax = middleWidth + (inWidthMax - middleWidth) / 2;
                    searchHeightMin = middleHeight - (middleHeight - inHeightMin) / 2;
                    searchHeightMax = middleHeight + (inHeightMax - middleHeight) / 2;
                }

                int tried = 0;
                while (colorFound == false)
                {
                    for (int height = searchHeightMin; height < searchHeightMax; height++)
                    {
                        for (int width = searchWidthMin; width < searchWidthMax; width++)
                        {
                            int offset = (height * bmpData.Stride) + (width * 4);
                            int blue = rgbValues[offset];
                            int green = rgbValues[offset + 1];
                            int red = rgbValues[offset + 2];
                            Color pixelColor = Color.FromArgb(red, green, blue);
                            if (Math.Abs(pixelColor.R - colorToFind.R) <= tolerance &&
                            Math.Abs(pixelColor.G - colorToFind.G) <= tolerance &&
                            Math.Abs(pixelColor.B - colorToFind.B) <= tolerance)
                            {
                                // Berechnen der Entfernung zwischen dem gefundenen Pixel und der aktuellen Cursorposition
                                double distance = Math.Sqrt(Math.Pow(width - Cursor.Position.X, 2) + Math.Pow(height - Cursor.Position.Y, 2));
                                if (distance < closestDistance)
                                {
                                    // Aktualisiere die nächste Position und Entfernung

                                    closestX = width;
                                    closestY = height;
                                    closestDistance = distance;
                                    colorFound = true;
                                    break;

                                }
                            }

                        }
                        if (colorFound)
                            break;

                    }
                    if (colorFound || tried == 1 || !expandSearchEnabled || !expandSearch)
                        break;
                    else
                    {
                        tried++;
                        searchWidthMin = 0;
                        searchWidthMax = DiabloBot.Recalc(1920);
                        searchHeightMin = 0;
                        searchHeightMax = DiabloBot.Recalc(1080, false);
                    }

                }

                Marshal.Copy(rgbValues, 0, ptr, bytes);
                screens[i].UnlockBits(bmpData);
            });
            if (closestX != -1 && closestY != -1)
            {
                // Verschieben des Cursors zur Position des gefundenen Pixels und Rückgabe der Position als Point

                if (useScreen1)
                {
                   // SetCursorPos(closestX, closestY);
                    cursor = new Point(closestX, closestY);
                }
                else
                {
                    closestX = Screen.AllScreens[1].Bounds.Width - closestX;
                 //   SetCursorPos(-closestX, closestY);
                    cursor = new Point(-closestX, closestY);
                }
                return cursor;

            }
            else
            {
                // Falls keine Farbe gefunden wurde, wird ein Point mit den Werten (0,0) zurückgegeben
                cursor = new Point(0, 0);
                return cursor;
            }

        }
        #endregion

    }
}
