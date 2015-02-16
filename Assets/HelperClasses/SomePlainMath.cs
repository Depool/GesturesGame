using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameFacilities
{
    public static class SomePlainMath
    {
        private static readonly double comparsionEps = 1e-9;
        public static Vector2 NonParralelLinesIntersectionPoint(Vector3 lineOne, Vector3 lineTwo)
        {
            return new Vector2((-lineOne.z * lineTwo.y + lineOne.y * lineTwo.z) / (lineOne.x * lineTwo.y - lineOne.y * lineTwo.x),
                               (lineOne.z * lineTwo.x - lineOne.x * lineTwo.z) / (lineOne.x * lineTwo.y - lineOne.y * lineTwo.x));
        }

        public static bool PointBelongsToSegment(Vector2 s1, Vector2 s2, Vector2 point)
        {
            return Math.Abs(Vector2.Distance(point, s1) + Vector2.Distance(point, s2) - Vector2.Distance(s1, s2)) < 1e-5;
        }

        public static Vector3 PerpendicularLineThroughPoint(Vector3 line, Vector2 point)
        {
            float a = line.y;
            float b = -line.x;
            float c = -point.x * a - point.y * b;
            return new Vector3(a, b, c);
        }

        public static Vector3 LineByPoints(Vector2 p1, Vector2 p2)
        {
            float a = p2.y - p1.y;
            float b = p1.x - p2.x;
            float c = -p1.x * a - p1.y * b;
            return new Vector3(a, b, c);
        }

        public static float PointToSegmentDistance(Vector2 segStart, Vector3 segFinish, Vector2 point)
        {
            Vector3 segLine = LineByPoints(segStart, segFinish);
            Vector3 perpLine = PerpendicularLineThroughPoint(segLine, point);
            Vector2 intersection = NonParralelLinesIntersectionPoint(segLine, perpLine);

            if (PointBelongsToSegment(segStart, segFinish, intersection))
                return Vector2.Distance(point, intersection);
            else
                return Math.Min(Vector2.Distance(segStart, point), Vector2.Distance(segFinish, point));
        }

        public static float PointToLineDistance(Vector2 p, List<Vector2> points, out int nearestIndex)
        {
            if (points == null)
                throw new ApplicationException("Line must contain at least one point");

            if (points.Count == 1)
            {
                nearestIndex = -1;
                return Vector2.Distance(p, points[0]);
            }

            float res = PointToSegmentDistance(points[0], points[1], p);
            nearestIndex = 0;

            for (int i = 2; i < points.Count; ++i)
            {
                float dist = PointToSegmentDistance(points[i - 1], points[i], p);
                if (res - dist > 1e-5)
                {
                    res = dist;
                    nearestIndex = i - 1;
                }
            }

            return res;
        }

        public static float LineLenght(List<Vector2> points)
        {
            if (points == null)
                throw new ApplicationException("Line must contain at least one point");

            float res = 0.0f;

            for (int i = 1; i < points.Count; ++i)
                res += Vector2.Distance(points[i - 1], points[i]);

            return res;
        }

        public static Rect AddPointToBoundintBox(Vector2 point, Rect bbox)
        {
            if (bbox.x - point.x > comparsionEps)
                bbox.x = point.x;

            if (point.x - (bbox.x + bbox.width) > comparsionEps)
                bbox.width = point.x - bbox.x;

            if (bbox.y - point.y > comparsionEps)
                bbox.y = point.y;

            if (point.y - (bbox.y + bbox.height) > comparsionEps)
                bbox.height = point.y - bbox.y;

            return bbox;
        }

        public static Rect CountBoundingox(List<Vector2> points)
        {
            if (points == null || points.Count == 0)
                return new Rect();

            Rect res = new Rect(points[0].x, points[0].y, 0, 0);

            for (int i = 1; i < points.Count; ++i)
                res = SomePlainMath.AddPointToBoundintBox(points[i], res);

            return res;
        }
    }
}
