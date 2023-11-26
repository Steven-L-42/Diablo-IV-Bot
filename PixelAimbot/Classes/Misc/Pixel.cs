using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EpPathFinding.cs;

namespace PixelAimbot.Classes.Misc
{
    public class Pixel
    {
        public static object PixelSearch(int nLeft, int nTop, int nRight, int nBottom , int ColorValue, int Shade_Variation)
        {
            Rectangle rect = new Rectangle(nLeft, nTop, nRight-nLeft, nBottom-nTop);
            Bitmap RegionIn_Bitmap = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb);

            Color Pixel_Color = Color.FromArgb(ColorValue);
            
            int xOffset = Screen.PrimaryScreen.Bounds.Left;
            int yOffset = Screen.PrimaryScreen.Bounds.Top;

            using (Graphics GFX = Graphics.FromImage(RegionIn_Bitmap))
            {
                GFX.CopyFromScreen(rect.X + xOffset, rect.Y + yOffset, 0, 0, rect.Size, CopyPixelOperation.SourceCopy);
            }

            BitmapData RegionIn_BitmapData = RegionIn_Bitmap.LockBits(
                new Rectangle(0, 0, RegionIn_Bitmap.Width, RegionIn_Bitmap.Height), ImageLockMode.ReadWrite,
                PixelFormat.Format24bppRgb);
            int[] Formatted_Color = new int[3] {Pixel_Color.B, Pixel_Color.G, Pixel_Color.R}; //bgr
            unsafe
            {
                for (int y = 0; y < RegionIn_BitmapData.Height; y++)
                {
                    byte* row = (byte*) RegionIn_BitmapData.Scan0 + (y * RegionIn_BitmapData.Stride);
                    for (int x = 0; x < RegionIn_BitmapData.Width; x++)
                    {
                        if (row[x * 3] >= (Formatted_Color[0] - Shade_Variation) &
                            row[x * 3] <= (Formatted_Color[0] + Shade_Variation)) //blue
                            if (row[(x * 3) + 1] >= (Formatted_Color[1] - Shade_Variation) &
                                row[(x * 3) + 1] <= (Formatted_Color[1] + Shade_Variation)) //green
                                if (row[(x * 3) + 2] >= (Formatted_Color[2] - Shade_Variation) &
                                    row[(x * 3) + 2] <= (Formatted_Color[2] + Shade_Variation)) //red
                                    return new object[] { x + rect.X, y + rect.Y};
                    }
                }
            }

            RegionIn_Bitmap.Dispose();
            return 0;
        }
        
        public static Boolean isGrayScale(int nLeft, int nTop, int nRight, int nBottom, Bitmap img)
        {
            Rectangle rect = new Rectangle(nLeft, nTop, nRight, nBottom);
            Bitmap RegionIn_Bitmap = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb);
            int xOffset = Screen.PrimaryScreen.Bounds.Left;
            int yOffset = Screen.PrimaryScreen.Bounds.Top;
            using (Graphics GFX = Graphics.FromImage(RegionIn_Bitmap))
            {
                GFX.CopyFromScreen(rect.X + xOffset, rect.Y + yOffset, 0, 0, rect.Size, CopyPixelOperation.SourceCopy);
            }
            Boolean result = true;
            for (Int32 h = 0; h < RegionIn_Bitmap.Height; h++)
            for (Int32 w = 0; w < RegionIn_Bitmap.Width; w++)
            {
                Color color = RegionIn_Bitmap.GetPixel(w, h);
                if ((color.R != color.G || color.G != color.B || color.R != color.B) && color.A != 0)
                {
                    result = false;
                    return result;
                }
            }
            return result;
        }
        public BaseGrid PNGtoGrid(Bitmap input, float threshold = 0.99f)
        {
            BaseGrid output = new StaticGrid(
                input.Width, 
                input.Height
            );
            for (int x = 0; x < input.Width; x++)
            {
                for (int y = 0; y < input.Height; y++)
                {
                    Color c = input.GetPixel(
                        x, 
                        y
                    );
                    if (c.GetBrightness() < threshold)
                        output.SetWalkableAt(
                            x, 
                            y, 
                            false
                        );
                    else
                        output.SetWalkableAt(
                            x, 
                            y, 
                            true
                        );
                }
            }
            return output;
        }
        public List<GridPos> findPath(BaseGrid grid, DiagonalMovement move, GridPos startPos, GridPos endPos){
            JumpPointParam jpParam = new JumpPointParam(
                grid, 
                startPos, 
                endPos, 
                EndNodeUnWalkableTreatment.ALLOW,
                move, 
                HeuristicMode.EUCLIDEAN
            );
            List<GridPos> result = JumpPointFinder.FindPath(jpParam); 
            return result;
        }
        public Bitmap addPath(List<GridPos> path, Bitmap image)
        {
            for (int i = 0; i < path.Count; i ++){
                GridPos curr = path.ElementAt(i);
                GridPos next;
                if (i < path.Count()-1) 
                    next = path.ElementAt(i + 1);
                else 
                    next = null;
                if (next == null){
                    image.SetPixel(curr.x,
                        curr.y, 
                        Color.Red
                    );
                    return image;
                }
                using (var graphics = Graphics.FromImage(image))
                {
                    Pen blackPen = new Pen(Color.Red, 2);
                    graphics.DrawLine(blackPen, 
                        curr.x, 
                        curr.y, 
                        next.x, 
                        next.y
                    );
                }
            }
        
            return image;
        }
    }
}