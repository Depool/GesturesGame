using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameFacilities
{
    public class Gesture : UnityEngine.Object, IDisposable
    {
        private List<Vector2> points = new List<Vector2>();
        private double distDelta = 0.75;
        private double lenDelta = 0.15;
        private Vector3 position = new Vector3(0, 0, 0);
        private Rect boundingBox = new Rect();
        private LineRenderer renderer;
        private bool isDraw = true;

        private void initRenderer()
        {
            renderer = ((GameObject)Instantiate(Resources.Load("Prefabs/TaskLineDrawer"))).GetComponent<LineRenderer>();
            //renderer.materials[0] = Resources.Load("TaskDrawerMaterial") as Material;
            //renderer.material = Resources.Load("TaskDrawerMaterial") as Material;
            //renderer.SetWidth(0.5f, 0.5f);
        }

        private void fillPoint(Vector2 p)
        {
            if (points.Count == 1)
            {
                boundingBox = new Rect(p.x, p.y, 0, 0);
            }
            else
            {
                boundingBox = SomePlainMath.AddPointToBoundintBox(p, boundingBox);
            }

            if (isDraw)
            {
                renderer.SetVertexCount(points.Count);
                renderer.SetPosition(points.FindLastIndex((pt) => pt == p), new Vector3(position.x + p.x, position.y + p.y, 0));
            }
        }

        //properties

        public bool Draw
        {
            set
            {
                isDraw = value;
            }
        }

        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                if (value.z != 0)
                    throw new ApplicationException("z value must be 0, it is a plane game");
                position = value;

                if (isDraw)
                for (int i = 0; i < points.Count; ++i)
                {
                    renderer.SetPosition(i, new Vector3(position.x + points[i].x, position.y + points[i].y, 0));
                }
            }
        }

        public Transform Parent
        {
            get
            {
                return renderer.transform.parent;
            }
            set
            {
                renderer.transform.parent = value;
            }
        }

        public Gesture()
        {
            initRenderer();
        }

        public Gesture(Vector2 p1, Vector2 p2)
        {
            points.AddRange(new[] { p1, p2 });
            boundingBox = new Rect(p1.x, p1.y, 0, 0);

            initRenderer();

            fillPoint(p1);
            fillPoint(p2);
        }

        public Gesture(List<Vector2> points)
        {
            if (points == null && points.Count < 2)
                throw new ApplicationException("TaskLine Must Contain two or more points");

            Rect bBox = SomePlainMath.CountBoundingox(points);

            for (int i = 0; i < points.Count; ++i)
                points[i] = new Vector2(points[i].x - bBox.x, points[i].y - bBox.y);

            this.points = points;

            initRenderer();

            boundingBox = new Rect(points[0].x, points[0].y, 0, 0);

            foreach (Vector2 p in this.points)
                fillPoint(p);
        }

        public void AddPoint(Vector2 point)
        {
            points.Add(point);
            fillPoint(point);
        }

        public List<Vector2> GetNormalizedPoints(float scaleFactor)
        {
            List<Vector2> resPoints = new List<Vector2>();

            foreach (Vector2 point in points)
            {
                Vector2 normalized = new Vector2((point.x - boundingBox.x) * scaleFactor, (point.y - boundingBox.y) * scaleFactor);
                resPoints.Add(normalized);
            }
            return resPoints;
        }


        private bool checkIfFits(List<Vector2> ethalon, List<Vector2> testing, float scaleRatio, float ethalonLength)
        {
            int curIndex;
            int coveredCount = 0;
            bool res = true;

            bool[] covered = new bool[ethalon.Count];

            for (int i = 0; i < covered.Length; ++i)
                covered[i] = false;

            float dist = SomePlainMath.PointToLineDistance(testing[0], ethalon, out curIndex);

            res = dist <= distDelta * scaleRatio;

            covered[curIndex] = true;
            coveredCount = 1;

            for (int i = 1; i < testing.Count && res; ++i)
                if (SomePlainMath.PointToSegmentDistance(ethalon[curIndex], ethalon[curIndex + 1], testing[i]) > distDelta * scaleRatio)
                {
                    int nextId = 0;
                    int prev = curIndex - 1;
                    if (prev < 0)
                        prev = ethalon.Count - 2;
                    int next = curIndex + 1;
                    if (next == ethalon.Count - 1)
                        next = 0;

                    if (SomePlainMath.PointToSegmentDistance(ethalon[prev], ethalon[prev + 1], testing[i]) <= distDelta * scaleRatio)
                        nextId = curIndex - 1;
                    else
                        if (SomePlainMath.PointToSegmentDistance(ethalon[next], ethalon[next + 1], testing[i]) <= distDelta * scaleRatio)
                            nextId = curIndex + 1;
                        else
                            if (SomePlainMath.PointToLineDistance(testing[i], ethalon, out nextId) > distDelta * scaleRatio)
                                res = false;

                    curIndex = nextId;
                    if (curIndex < 0)
                        curIndex = ethalon.Count - 2;
                    if (curIndex == ethalon.Count - 1)
                        curIndex = 0;

                    if (!covered[curIndex])
                    {
                        covered[curIndex] = true; coveredCount++;
                    }
                }

            if (res)
            {
                //float scaledTestingLengthRatio = SomePlainMath.LineLenght(testing) / ethalonLength;
                res = (ethalon.Count - 1) - coveredCount <= ethalon.Count / 10; //1.0 - lenDelta <= scaledTestingLengthRatio && 1.0 + lenDelta >= scaledTestingLengthRatio;
            }
            return res;
        }

        public bool Fits(Gesture gest)
        {
            List<Vector2> ethalon = GetNormalizedPoints(1.0f);
            List<Vector2> testing = gest.GetNormalizedPoints(1.0f);

            float etLength = SomePlainMath.LineLenght(ethalon);
            float testLength = SomePlainMath.LineLenght(testing);

            if (etLength == 0 && testLength == 0)
                return true;

            if (etLength * testLength == 0)
                return false;

            float minScaleRatio = 0.8f * etLength / testLength;
            float maxScaleRatio = 1.2f * etLength / testLength;

            bool res = false;

            float step = (float)Math.Min(0.01f, (maxScaleRatio - maxScaleRatio) / 100.0f);

            for (float scaleRatio = minScaleRatio; scaleRatio < maxScaleRatio && !res; scaleRatio += 0.01f)
            {
                ethalon = GetNormalizedPoints(1.0f);
                testing = gest.GetNormalizedPoints(scaleRatio);
                res = checkIfFits(ethalon, testing, scaleRatio, etLength);
            }

            return res;
        }

        public void Reset()
        {
            points.Clear();
            renderer.SetVertexCount(0);
        }

        public override string ToString()
        {
            String res = String.Empty;
            foreach (Vector2 p in points)
                res += "new Vector2(" + p.x.ToString() + "f, " + p.y.ToString() + "f),\n";

            return res;
        }

        public void Dispose()
        {
            renderer.gameObject.transform.parent = null;
            Destroy(renderer.gameObject);
        }
    }
}
