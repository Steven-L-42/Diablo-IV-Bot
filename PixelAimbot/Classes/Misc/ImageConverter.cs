using System;
using System.Drawing;

namespace PixelAimbot.Classes.Misc
{
    public static class BitmapConverter
    {
        public static string BitmapToPlaintext(string imagePath)
        {
            Bitmap bitmap = new Bitmap(imagePath);
            int width = bitmap.Width;
            int height = bitmap.Height;

            string plaintext = "";

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);
                    plaintext += $"{pixelColor.R},{pixelColor.G},{pixelColor.B} ";
                }
                plaintext += Environment.NewLine;
            }

            return plaintext;
        }

        public static Bitmap PlaintextToBitmap(string plaintext)
        {
            string[] lines = plaintext.Trim().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            int height = lines.Length;
            int width = lines[0].Split(' ').Length / 3;

            Bitmap bitmap = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                string[] pixelValues = lines[y].Split(' ');

                for (int x = 0; x < width; x++)
                {
                    string[] rgb = pixelValues[x].Split(',');

                    int red = int.Parse(rgb[0]);
                    int green = int.Parse(rgb[1]);
                    int blue = int.Parse(rgb[2]);

                    Color pixelColor = Color.FromArgb(red, green, blue);
                    bitmap.SetPixel(x, y, pixelColor);
                }
            }

            return bitmap;
        }
    }
}