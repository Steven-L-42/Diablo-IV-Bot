using Emgu.Util;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PixelAimbot
{
    public class CustomCheckBox : CheckBox
    {
        private Color checkboxColor = Color.FromArgb(150, 82, 197); // Desired color for the rectangle
        private Color checkmarkColor = Color.GhostWhite; // Desired color for the checkmark
        private Color uncheckedColor = Color.GhostWhite; // Desired color for the rectangle when unchecked

        private int cornerRadius = 4; // Radius for the rounded corners
        private ContentAlignment checkAlign = ContentAlignment.MiddleLeft; // Alignment for the checkmark

        public Color CheckboxColor
        {
            get { return checkboxColor; }
            set
            {
                checkboxColor = value;
                Invalidate(); // Refresh the control when the alignment is changed
            }
        }

        public ContentAlignment CustomCheckAlign
        {
            get { return checkAlign; }
            set
            {
                checkAlign = value;
                Invalidate(); // Refresh the control when the alignment is changed
            }
        }
        public int CornerRadius
        {
            get { return cornerRadius; }
            set
            {
                cornerRadius = value;
                Invalidate(); // Refresh the control when the radius is changed
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Rectangle checkboxRect = new Rectangle(0, (Height - 14) / 2, 14, 14); // Rectangle for the checkbox
            if (checkAlign == ContentAlignment.MiddleRight)
            {
                checkboxRect.X = Width - checkboxRect.Width;
            }
            else if (checkAlign == ContentAlignment.MiddleCenter)
            {
                checkboxRect.X = (Width - checkboxRect.Width) / 2;
            }

            // Determine the color for the rectangle
            Color rectangleColor = Checked ? checkboxColor : uncheckedColor;

            // Create a rounded rectangle shape for the checkbox
            using (GraphicsPath path = CreateRoundedRectangle(checkboxRect, cornerRadius))
            {
                // Draw the checkbox shape with the desired color
                using (SolidBrush brush = new SolidBrush(rectangleColor))
                {
                    e.Graphics.FillPath(brush, path);
                }
            }

            if (Checked)
            {
                // Draw the checkmark in white
                using (Pen checkmarkPen = new Pen(checkmarkColor, 2))
                {
                    e.Graphics.DrawLine(checkmarkPen, checkboxRect.Left + 2, checkboxRect.Top + 7, checkboxRect.Left + 5, checkboxRect.Top + 11);
                    e.Graphics.DrawLine(checkmarkPen, checkboxRect.Left + 5, checkboxRect.Top + 11, checkboxRect.Left + 10, checkboxRect.Top + 3);
                }
            }
        }

        protected override void OnCheckedChanged(EventArgs e)
        {
            base.OnCheckedChanged(e);

            // Refresh the control to update the appearance
            Invalidate();
        }


        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
            path.AddLine(rect.X + radius, rect.Y, rect.Right - radius * 2, rect.Y);
            path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
            path.AddLine(rect.Right, rect.Y + radius, rect.Right, rect.Bottom - radius * 2);
            path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2 - 1, radius * 2, radius * 2, 0, 90);
            path.AddLine(rect.Right - radius, rect.Bottom, rect.X + radius * 2, rect.Bottom);
            path.AddArc(rect.X, rect.Bottom - radius * 2 - 1, radius * 2, radius * 2, 90, 90);
            path.AddLine(rect.X, rect.Bottom - radius, rect.X, rect.Y + radius * 2);
            path.CloseFigure();
            return path;
        }
    }
}