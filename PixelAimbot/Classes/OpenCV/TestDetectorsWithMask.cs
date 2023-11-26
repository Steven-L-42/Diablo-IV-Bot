using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Drawing;

namespace PixelAimbot.Classes.OpenCV
{
    internal class TestDetectorsWithMask
    {
        private Image<Bgra, byte> _enemyTemplate;
        private Image<Gray, byte> _enemyMask;
        private readonly float _threshold;
        private readonly DrawScreen _screenDrawer;
        private readonly int _rectangleX;
        private readonly int _rectangleY;
        private readonly int _rectangleWidth;
        private readonly int _rectangleHeight;
        private readonly TemplateMatchingType _method;
        private readonly Point? _myPosition;

        public Point? FoundPosition { get; private set; }

        public TestDetectorsWithMask(Image<Bgra, byte> enemyTemplate, Image<Gray, byte> enemyMask, float threshold, int rectangleX, int rectangleY, int rectangleWidth, int rectangleHeight, Point? myPosition = null, TemplateMatchingType method = TemplateMatchingType.SqdiffNormed)
        {
            _enemyMask = enemyMask;
            _enemyTemplate = enemyTemplate;
            _threshold = threshold;
            _screenDrawer = new DrawScreen();
            _rectangleHeight = rectangleHeight;
            _rectangleWidth = rectangleWidth;
            _rectangleX = rectangleX;
            _rectangleY = rectangleY;
            _myPosition = myPosition;
            _method = method;
        }

        private bool DetectEnemy(Image<Bgra, byte> screenCapture, bool rescaleImage = true)
        {
            if (rescaleImage && !DiabloBot.IsWindowed)
            {
                _enemyTemplate = _enemyTemplate.Resize(DiabloBot.Recalc(_enemyTemplate.Size.Width), DiabloBot.Recalc(_enemyTemplate.Size.Height, false), Inter.Linear);
                if (_enemyMask != null)
                {
                    _enemyMask = _enemyMask.Resize(DiabloBot.Recalc(_enemyTemplate.Size.Width), DiabloBot.Recalc(_enemyTemplate.Size.Height, false), Inter.Linear);
                }
            }

            screenCapture.ROI = new Rectangle(_rectangleX, _rectangleY, _rectangleWidth, _rectangleHeight);
            var minimap = screenCapture.Copy().Convert<Bgra, byte>();
            var res = new Mat();
            double minVal = 0, maxVal = 0;
            Point minPoint = new Point();
            Point maxPoint = new Point();
            CvInvoke.MatchTemplate(minimap, _enemyTemplate, res, _method, _enemyMask);
            int h = _enemyTemplate.Size.Height;
            int w = _enemyTemplate.Size.Width;

            CvInvoke.MinMaxLoc(res, ref minVal, ref maxVal, ref minPoint, ref maxPoint);
            if (1 - minVal > _threshold)
            {
                var screenPoint = new Point(minPoint.X + _rectangleX + w / 2, minPoint.Y + _rectangleY + h / 2);
                FoundPosition = screenPoint;
                return true;
            }

            minimap.Dispose();
            screenCapture.Dispose();

            FoundPosition = null;
            return false;
        }

        public Point ClickIfFound(Image<Bgra, byte> screenCapture, bool showDetections = false, bool rescaleImage = true)
        {
            if (DetectEnemy(screenCapture, rescaleImage))
            {
                if (FoundPosition.HasValue)
                {
                    return FoundPosition.Value;
                }
                return new Point(0, 0);
            }
            return new Point(0, 0);
        }
    }
}
