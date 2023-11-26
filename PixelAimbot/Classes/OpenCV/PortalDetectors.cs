using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PixelAimbot.Classes.OpenCV
{
    internal class PortalDetectors
    {
        private Image<Bgr, byte> _PortalTemplate;
        private Image<Bgr, byte> _PortalMask;
        private float _thresh;
        private readonly Point _mePosition = new Point(DiabloBot.Recalc(1920), DiabloBot.Recalc(1080, false));
        public PortalDetectors(Image<Bgr, byte> PortalTemplate,
           Image<Bgr, byte> PortalMask, float thresh)
        {
            this._PortalMask = PortalMask;
            this._PortalTemplate = PortalTemplate;
            this._thresh = thresh;
        }
        private List<(Point position, double matchValue)> DetectPortal(Image<Bgr, byte> screenCapture)
        {
            if (!DiabloBot.IsWindowed)
            {
                this._PortalTemplate.Resize(DiabloBot.Recalc(this._PortalTemplate.Size.Width),
                    DiabloBot.Recalc(this._PortalTemplate.Size.Height), Inter.Linear);
                this._PortalMask.Resize(DiabloBot.Recalc(this._PortalMask.Size.Width),
                    DiabloBot.Recalc(this._PortalMask.Size.Height), Inter.Linear);
            }

            List<(Point minPoint, double)> Portals = new List<(Point position, double matchValue)>();

            screenCapture.ROI = new Rectangle(DiabloBot.Recalc(50), DiabloBot.Recalc(124, false), DiabloBot.Recalc(223), DiabloBot.Recalc(252, false));
            var minimap = screenCapture.Copy();
            var res = new Mat();
            double minVal = 0, maxVal = 0;
            Point minPoint = new Point();
            Point maxPoint = new Point();
            CvInvoke.MatchTemplate(minimap, this._PortalTemplate, res, TemplateMatchingType.SqdiffNormed, this._PortalMask);

            int h = this._PortalTemplate.Size.Height;
            int w = this._PortalTemplate.Size.Width;

            while (1 - minVal > this._thresh)
            {
                CvInvoke.MinMaxLoc(res, ref minVal, ref maxVal, ref minPoint, ref maxPoint);
                if (1 - minVal > this._thresh)
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
                    Portals.Add((minPoint, 1 - minVal));
                }
            }

            return Portals;
        }
        private double Distance(Point Portal)
        {
            return Math.Sqrt((Math.Pow(Portal.X - _mePosition.X, 2) + Math.Pow(Portal.Y - _mePosition.Y, 2)));
        }
        public Point? GetBestPortal(Image<Bgr, byte> screenCapture, bool showDetections = false)
        {
            var enemies = DetectPortal(screenCapture);
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
        public Point? GetClosestPortal(Image<Bgr, byte> screenCapture)
        {
            var Portals = DetectPortal(screenCapture);
            var PortalAndPosition = Portals.Select(x => (x, Distance(x.position)));
            if (PortalAndPosition.Any())
            {
                double minDist = Double.MaxValue;
                Point closestPortal = default;
                foreach (var (Portal, distance) in PortalAndPosition)
                {
                    if (distance < minDist)
                    {
                        minDist = distance;
                        closestPortal = Portal.position;
                    }
                }

                return closestPortal;
            }
            else
            {
                return null;
            }
        }
    }
}
