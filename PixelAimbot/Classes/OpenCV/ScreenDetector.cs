using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace PixelAimbot.Classes.OpenCV
{
    internal class ScreenDetector
    {
        public Image<Bgr, byte> _enemyTemplate;
        public Image<Bgr, byte> _enemyMask;
        public float _threshold { get; set; } = 0.7f;
        private Point _myPosition = new Point(DiabloBot.Recalc(150), DiabloBot.Recalc(128, false));
        private DrawScreen _screenDrawer;
        public int rectangleX = 0;
        public int rectangleY = 0;
        public int rectangleWidth = 0;
        public int rectangleHeight = 0;
        public TemplateMatchingType method = TemplateMatchingType.SqdiffNormed;

       
        public ScreenDetector(Image<Bgr, byte> enemyTemplate, Image<Bgr, byte> enemyMask, float threshold, int rectangleX, int rectangleY, int rectangleWidth, int rectangleHeight)
        {

           
            
            this._enemyMask = enemyMask;
            this._enemyTemplate = enemyTemplate;
            this._threshold = threshold;
            this._screenDrawer = new DrawScreen();
            this.rectangleHeight = rectangleHeight;
            this.rectangleWidth = rectangleWidth;
            this.rectangleX = rectangleX;
            this.rectangleY = rectangleY;

            _enemyTemplate.Resize(DiabloBot.Recalc(_enemyTemplate.Size.Width),
                DiabloBot.Recalc(_enemyTemplate.Size.Height, false), Inter.Linear);
            if (_enemyMask != null)
            {
                _enemyMask.Resize(DiabloBot.Recalc(_enemyTemplate.Size.Width),
                    DiabloBot.Recalc(_enemyTemplate.Size.Height, false), Inter.Linear);
            }
        }

        public void setMyPosition(Point point)
        {
            this._myPosition = point;
        }

        public void setMatchingMethod(TemplateMatchingType type)
        {
            this.method = type;
        }
        private List<(Point position, double matchValue)> DetectEnemies(Image<Bgr, byte> screenCapture, bool rescaleImage = true)
        {
            if (rescaleImage && !DiabloBot.IsWindowed)
            {
                this._enemyTemplate.Resize(DiabloBot.Recalc(this._enemyTemplate.Size.Width),
                    DiabloBot.Recalc(this._enemyTemplate.Size.Height, false), Inter.Linear);
                if (this._enemyMask != null)
                {
                    this._enemyMask.Resize(DiabloBot.Recalc(this._enemyTemplate.Size.Width),
                        DiabloBot.Recalc(this._enemyTemplate.Size.Height, false), Inter.Linear);
                }
            }

            List<(Point minPoint, double)> enemies = new List<(Point position, double matchValue)>();
            screenCapture.ROI = new Rectangle(rectangleX, rectangleY, rectangleWidth, rectangleHeight);
            var minimap = screenCapture.Copy();
            var res = new Mat();
            double minVal = 0, maxVal = 0;
            Point minPoint = new Point();
            Point maxPoint = new Point();
            CvInvoke.MatchTemplate(minimap, this._enemyTemplate, res, method, this._enemyMask);
            int h = this._enemyTemplate.Size.Height;
            int w = this._enemyTemplate.Size.Width;

            while (1 - minVal > this._threshold)
            {
                CvInvoke.MinMaxLoc(res, ref minVal, ref maxVal, ref minPoint, ref maxPoint);
                if (1 - minVal > this._threshold)
                {
                    var lowerLeft = new Point(minPoint.X - w / 4, minPoint.Y - h / 4);
                    var upperLeft = new Point(minPoint.X - w / 4, minPoint.Y + h / 4);
                    var lowerRight = new Point(minPoint.X + w / 4, minPoint.Y - h / 4);
                    var upperRight = new Point(minPoint.X + w / 4, minPoint.Y + h / 4);
                    var points = new Point[]
                    {
                                lowerLeft,
                                lowerRight,
                                upperRight,
                                upperLeft
                    };
                    var vector = new VectorOfPoint(points);

                    CvInvoke.FillConvexPoly(res, vector, new MCvScalar(255));
                    enemies.Add((minPoint, 1 - minVal));
                }
            }
            minimap.Dispose();
            screenCapture.Dispose();

            return enemies;
        }

        private double Distance(Point enemy)
        {
            return Math.Sqrt((Math.Pow(enemy.X - _myPosition.X, 2) + Math.Pow(enemy.Y - _myPosition.Y, 2)));
        }



        public Point? GetBest(Image<Bgr, byte> screenCapture, bool showDetections = false, bool rescaleImage = true)
        {
            var enemies = DetectEnemies(screenCapture);
            if (enemies.Any())
            {
                double maxValue = Double.MinValue;
                Point bestEnemy = default;
                foreach (var enemy in enemies)
                {
                    if (enemy.matchValue > maxValue)
                    {
                        maxValue = enemy.matchValue;
                        bestEnemy = enemy.position;
                    }
                }

                return bestEnemy;
            }
            else
            {
                return null;
            }
        }
        public Point? GetClosestBest(Image<Bgr, byte> screenCapture, bool showDetections = false)
        {
            var enemies = DetectEnemies(screenCapture);
            var enemyAndPosition = enemies.Select(x => (x, Distance(x.position)));
            if (enemyAndPosition.Any())
            {
                double minDist = Double.MaxValue;
                double maxValue = Double.MinValue;
                Point closestEnemy = default;
                foreach (var (enemy, distance) in enemyAndPosition)
                {
                    if (distance < minDist)
                    {
                        minDist = distance;

                        if (enemy.matchValue > maxValue)
                        {
                            maxValue = enemy.matchValue;
                            closestEnemy = enemy.position;
                        }
                    }
                }

                return closestEnemy;
            }
            else
            {
                return null;
            }
        }
        public Point? GetClosest(Image<Bgr, byte> screenCapture, bool showDetections = false)
        {
            var enemies = DetectEnemies(screenCapture);
            var enemyAndPosition = enemies.Select(x => (x, Distance(x.position)));
            if (enemyAndPosition.Any())
            {
                double minDist = Double.MaxValue;
                Point closestEnemy = default;
                foreach (var (enemy, distance) in enemyAndPosition)
                {
                    if (distance < minDist)
                    {
                        minDist = distance;
                        closestEnemy = enemy.position;
                    }

                }

                return closestEnemy;
            }
            else
            {
                return null;
            }
        }
        public Point? CompareImage(Image<Bgr, byte> screenCapture)
        {
            // Ausschneiden des relevanten Ausschnitts aus dem screenCapture-Bild
            var roi = new Rectangle(rectangleX, rectangleY, rectangleWidth, rectangleHeight);
            var croppedImage = screenCapture.GetSubRect(roi);

            // Durchführung des Vergleichs auf dem ausgeschnittenen Bild
            var result = new Mat();
            CvInvoke.MatchTemplate(croppedImage, _enemyTemplate, result, TemplateMatchingType.CcoeffNormed);

            double minValue = 0;
            double maxValue = 0;
            Point minLocation = new Point();
            Point maxLocation = new Point();
            CvInvoke.MinMaxLoc(result, ref minValue, ref maxValue, ref minLocation, ref maxLocation);
            if (maxValue >= _threshold)
            {
                // Koordinaten des gefundenen Übereinstimmungspunkts im Gesamtbild berechnen
                Point locationInScreenCapture = new Point(maxLocation.X + roi.X, maxLocation.Y + roi.Y);
                return locationInScreenCapture;
            }

            return null;
        }

    }

}