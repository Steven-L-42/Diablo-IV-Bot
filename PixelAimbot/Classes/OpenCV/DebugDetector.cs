using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PixelAimbot.Classes.OpenCV
{
    internal class DebugDetector
    {
        public Image<Bgr, byte> _enemyTemplate;
        public Image<Bgr, byte> _enemyMask;
        public float _threshold { get; set; } = 0.7f;
        private Point _myPosition = new Point(DiabloBot.Recalc(150), DiabloBot.Recalc(128, false));
        private DrawScreenWin _screenDrawer;
        public int rectangleX = 0;
        public int rectangleY = 0;
        public int rectangleWidth = 0;
        public int rectangleHeight = 0;
        public TemplateMatchingType method = TemplateMatchingType.SqdiffNormed;


        public DebugDetector(Image<Bgr, byte> enemyTemplate, Image<Bgr, byte> enemyMask, float threshold, int rectangleX, int rectangleY, int rectangleWidth, int rectangleHeight)
        {
            this._enemyMask = enemyMask;
            this._enemyTemplate = enemyTemplate;
            this._threshold = threshold;
            this._screenDrawer = new DrawScreenWin();
            this.rectangleHeight = rectangleHeight;
            this.rectangleWidth = rectangleWidth;
            this.rectangleX = rectangleX;
            this.rectangleY = rectangleY;

        }
        public void setMatchingMethod(TemplateMatchingType type)
        {
            this.method = type;
        }

        public void setMyPosition(Point position)
        {
            _myPosition = position;
        }
        private List<(Point position, double matchValue)> DetectEnemies(Image<Bgr, byte> screenCapture)
        {
            if (!DiabloBot.IsWindowed)
            {
                this._enemyTemplate.Resize(DiabloBot.Recalc(this._enemyTemplate.Size.Width),
                    DiabloBot.Recalc(this._enemyTemplate.Size.Height), Inter.Linear);
                if (this._enemyMask != null)
                {
                    this._enemyMask.Resize(DiabloBot.Recalc(this._enemyMask.Size.Width),
                        DiabloBot.Recalc(this._enemyMask.Size.Height), Inter.Linear);
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

            return enemies;
        }

        private double Distance(Point enemy)
        {
            return Math.Sqrt((Math.Pow(enemy.X - _myPosition.X, 2) + Math.Pow(enemy.Y - _myPosition.Y, 2)));
        }



        public Point? GetBestEnemy(Image<Bgr, byte> screenCapture, bool showDetections = false, Form form = null)
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

                    if (showDetections)
                    {
                        // Draw enemy detection
                        int h = this._enemyTemplate.Size.Height;
                        int w = this._enemyTemplate.Size.Width;

                        _screenDrawer.Draw(form, enemy.position.X, enemy.position.Y, w, h);

                    }
                }

                return bestEnemy;
            }
            else
            {
                return null;
            }
        }

        public Point? GetClosestBest(Image<Bgr, byte> screenCapture, bool showDetections = false, Form form = null)
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
                     //   closestEnemy = enemy.position;

                        if (enemy.matchValue > maxValue)
                        {
                            maxValue = enemy.matchValue;
                            closestEnemy = enemy.position;
                        }
                    }

                    if (showDetections)
                    {
                        // Draw enemy detection
                        int h = this._enemyTemplate.Size.Height;
                        int w = this._enemyTemplate.Size.Width;

                        _screenDrawer.Draw(form, enemy.position.X, enemy.position.Y, DiabloBot.Recalc(w), DiabloBot.Recalc(h));

                    }
                }

                return closestEnemy;
            }
            else
            {
                return null;
            }
        }

        public Point? GetClosestEnemy(Image<Bgr, byte> screenCapture, bool showDetections = false, Form form = null)
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

                    if (showDetections)
                    {
                        // Draw enemy detection
                        int h = this._enemyTemplate.Size.Height;
                        int w = this._enemyTemplate.Size.Width;
                        //   _screenDrawer.Draw(ChaosBot.recalc(1593), ChaosBot.recalc(40, false), ChaosBot.recalc(296, false), ChaosBot.recalc(255));
                        _screenDrawer.Draw(form, enemy.position.X, enemy.position.Y, w, h);
                    }
                }

                return closestEnemy;
            }
            else
            {
                return null;
            }
        }
    }


}