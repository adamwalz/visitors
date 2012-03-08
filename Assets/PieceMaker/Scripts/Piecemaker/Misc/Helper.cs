using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MeshUtilities;

namespace MeshUtilities
{
    public static class Helper
    {
        public static bool VectorEqual(Vector3 a, Vector3 b, float tollerance)
        {
            float l = (a - b).sqrMagnitude;
            return l <= (tollerance * tollerance);
        }
        public static VertexDeclaration GetVertexDeclaration(VirtualMesh mesh)
        {
            var vertexDeclaration = VertexDeclaration.Position;
            if (mesh.Normals != null && mesh.Normals.Length > 0)
                vertexDeclaration |= VertexDeclaration.Normal;
            if (mesh.Tangents != null && mesh.Tangents.Length > 0)
                vertexDeclaration |= VertexDeclaration.Tangent;
            if (mesh.UV != null && mesh.UV.Length > 0)
                vertexDeclaration |= VertexDeclaration.UV;
            if (mesh.UV2 != null && mesh.UV2.Length > 0)
                vertexDeclaration |= VertexDeclaration.UV2;
            if (mesh.Colors != null && mesh.Colors.Length > 0)
                vertexDeclaration |= VertexDeclaration.Color;

            return vertexDeclaration;
        }

        public static bool IsInPositiveSide(Vector3 planePoint, Vector3 planeNormal, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            var d1 = Vector3.Dot(v1 - planePoint, planeNormal);
            var d2 = Vector3.Dot(v2 - planePoint, planeNormal);
            var d3 = Vector3.Dot(v3 - planePoint, planeNormal);
            var s1 = d1 < 0;
            var s2 = d2 < 0;
            var s3 = d3 < 0;
            return s1 && s2 && s3;
        }

        public static Matrix4x4 Calc2DPointMatrix(Vector3 PN, Vector3 PP)
        {
            var matTRS = Matrix4x4.TRS(PP, Quaternion.LookRotation(PN), Vector3.one);
            matTRS = matTRS.inverse;
            return matTRS;
        }

        public static Vector2 Calc2DPoint(Vector3 p, Matrix4x4 orth)
        {
            return orth.MultiplyPoint(p);
        }

        public static bool InCircle(Vector2 p, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            //Return TRUE if the point (xp,yp) lies inside the circumcircle
            //made up by points (x1,y1) (x2,y2) (x3,y3)
            //NOTE: A point on the edge is inside the circumcircle

            if (System.Math.Abs(p1.y - p2.y) < double.Epsilon && System.Math.Abs(p2.y - p3.y) < double.Epsilon)
            {
                //INCIRCUM - F - Points are coincident !!
                return false;
            }

            double m1, m2;
            double mx1, mx2;
            double my1, my2;
            double xc, yc;

            if (System.Math.Abs(p2.y - p1.y) < double.Epsilon)
            {
                m2 = -(p3.x - p2.x) / (p3.y - p2.y);
                mx2 = (p2.x + p3.x) * 0.5;
                my2 = (p2.y + p3.y) * 0.5;
                //Calculate CircumCircle center (xc,yc)
                xc = (p2.x + p1.x) * 0.5;
                yc = m2 * (xc - mx2) + my2;
            }
            else if (System.Math.Abs(p3.y - p2.y) < double.Epsilon)
            {
                m1 = -(p2.x - p1.x) / (p2.y - p1.y);
                mx1 = (p1.x + p2.x) * 0.5;
                my1 = (p1.y + p2.y) * 0.5;
                //Calculate CircumCircle center (xc,yc)
                xc = (p3.x + p2.x) * 0.5;
                yc = m1 * (xc - mx1) + my1;
            }
            else
            {
                m1 = -(p2.x - p1.x) / (p2.y - p1.y);
                m2 = -(p3.x - p2.x) / (p3.y - p2.y);
                mx1 = (p1.x + p2.x) * 0.5;
                mx2 = (p2.x + p3.x) * 0.5;
                my1 = (p1.y + p2.y) * 0.5;
                my2 = (p2.y + p3.y) * 0.5;
                //Calculate CircumCircle center (xc,yc)
                xc = (m1 * mx1 - m2 * mx2 + my2 - my1) / (m1 - m2);
                yc = m1 * (xc - mx1) + my1;
            }

            double dx = p2.x - xc;
            double dy = p2.y - yc;
            double rsqr = dx * dx + dy * dy;
            //double r = Math.Sqrt(rsqr); //Circumcircle radius
            dx = p.x - xc;
            dy = p.y - yc;
            double drsqr = dx * dx + dy * dy;

            return (drsqr <= rsqr);
        }

        public static Vector3 GetFacing(Vector3 v0, Vector3 v1, Vector3 v2)
        {
            var a = v2 - v0;
            var b = v1 - v0;
            while (a.sqrMagnitude != 0.0f && a.sqrMagnitude < 1.0f)
                a *= 10000;
            while (b.sqrMagnitude != 0.0f && b.sqrMagnitude < 1.0f)
                b *= 10000;
            var m = Vector3.Cross(a, b);
            while (m.sqrMagnitude != 0.0f && m.sqrMagnitude < 1.0f)
                m = (m * 10000);
            return m.normalized;
        }

        public static bool IsFacingTowards(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 normal)
        {
            var a = v2 - v0;
            var b = v1 - v0;
            while (a.sqrMagnitude != 0.0f && a.sqrMagnitude < 1.0f)
                a *= 10000;
            while (b.sqrMagnitude != 0.0f && b.sqrMagnitude < 1.0f)
                b *= 10000;
            var m = Vector3.Cross(a, b);
            while (m.sqrMagnitude != 0.0f && m.sqrMagnitude < 1.0f)
                m = (m * 10000);
            return Vector3.Dot(normal, m.normalized) > 0;
        }

        public static bool IsPlaneInBounds(Bounds bounds, Plane plane)
        {
            return plane.GetDistanceToPoint(bounds.center) <= bounds.size.magnitude;
        }

        public static SplitResult SplitTriangle(Vector3 cutPlanePos,
                                                Vector3 cutPlaneNormal,
                                                int i1,
                                                int i2,
                                                int i3,
                                                VirtualMesh mesh,
                                                VertexDeclaration vertexDeclaration,
                                                out VertexData[] positive,
                                                out VertexData[] negative)
        {
            VertexData newV1 = null;
            VertexData newV2 = null;

            var v1 = mesh.Vertices[i1];
            var v2 = mesh.Vertices[i2];
            var v3 = mesh.Vertices[i3];

            var d1 = Vector3.Dot(v1 - cutPlanePos, cutPlaneNormal);
            var d2 = Vector3.Dot(v2 - cutPlanePos, cutPlaneNormal);
            var d3 = Vector3.Dot(v3 - cutPlanePos, cutPlaneNormal);

            var s1 = d1 < 0;
            var s2 = d2 < 0;
            var s3 = d3 < 0;

            // if signs are differents then U = (a*Pb-b*Pa)/(a-b)
            // unstable if a,b are near to zero
            if (s2 != s1)
            {
                newV1 = VertexData.Interpolate(i1, i2, d2, d1, mesh, vertexDeclaration);
                if (s1 != s3)
                    newV2 = VertexData.Interpolate(i3, i1, d1, d3, mesh, vertexDeclaration);
                else
                    newV2 = VertexData.Interpolate(i3, i2, d2, d3, mesh, vertexDeclaration);
            }
            else if (s3 != s1)
            {
                newV1 = VertexData.Interpolate(i1, i3, d3, d1, mesh, vertexDeclaration);
                newV2 = VertexData.Interpolate(i3, i2, d2, d3, mesh, vertexDeclaration);
            }
            else
            {
                if (d1 < 0)
                {
                    positive = null;
                    negative = new VertexData[] 
					{
						VertexData.Create(i1, mesh, vertexDeclaration),
						VertexData.Create(i2, mesh, vertexDeclaration),
						VertexData.Create(i3, mesh, vertexDeclaration),
					};
                    return SplitResult.AllNegativ;

                }
                else
                {
                    negative = null;
                    positive = new VertexData[] 
					{
						VertexData.Create(i1, mesh, vertexDeclaration),
						VertexData.Create(i2, mesh, vertexDeclaration),
						VertexData.Create(i3, mesh, vertexDeclaration),
					};
                    return SplitResult.AllPositiv;
                }
            }

            var pos = new List<VertexData>();
            var neg = new List<VertexData>();

            if (!s1)
                pos.Add(VertexData.Create(i1, mesh, vertexDeclaration));
            else
                neg.Add(VertexData.Create(i1, mesh, vertexDeclaration));

            if (!s2)
                pos.Add(VertexData.Create(i2, mesh, vertexDeclaration));
            else
                neg.Add(VertexData.Create(i2, mesh, vertexDeclaration));

            if (!s3)
                pos.Add(VertexData.Create(i3, mesh, vertexDeclaration));
            else
                neg.Add(VertexData.Create(i3, mesh, vertexDeclaration));

            pos.Add(newV1);
            pos.Add(newV2);

            neg.Add(newV1);
            neg.Add(newV2);

            positive = pos.ToArray();
            negative = neg.ToArray();
            return SplitResult.Intersection;
        }

        public static List<List<Edge>> GetEdgeIslands(List<VertexData> vertexData, IEnumerable<Edge> edges)
        {
            // create edges from target mesh
            var islands = new List<List<Edge>>();
            foreach (var edge in edges)
            {
                var otherIslands = islands.Where(island => edge.HasSameVertex(vertexData, island)).ToArray();

                var newIsland = new List<Edge>(new Edge[] { edge });
                if (otherIslands.Any())
                {
                    islands.RemoveAll(island => otherIslands.Contains(island));
                    newIsland.AddRange(otherIslands.SelectMany(island => island));
                }
                islands.Add(newIsland);
            }
            islands.RemoveAll(island => island.Count <= 2);
            return islands;
        }

        public static bool GenerateOrderedEdges(List<VertexData> inVertices,
                                                List<MeshUtilities.Edge> inEdges,
                                                out List<int> finalIndices)
        {
            finalIndices = new List<int>();
            var tmpEdges = inEdges.Distinct().ToList();
            tmpEdges.RemoveAll(e => inVertices[e.i1].Position == inVertices[e.i2].Position);

            Vector3 lastPos = Vector3.zero;
            while (tmpEdges.Any())
            {
                if (finalIndices.Count == 0)
                {
                    var mainEdge = tmpEdges.First();
                    finalIndices.Add(mainEdge.i1);
                    lastPos = inVertices[mainEdge.i2].Position;
                    tmpEdges.Remove(mainEdge);

                    continue;
                }

                var edge = tmpEdges.FirstOrDefault(e => inVertices[e.i1].Position == lastPos || inVertices[e.i2].Position == lastPos);
                if (edge != null)
                {
                    if (inVertices[edge.i1].Position == lastPos)
                    {
                        finalIndices.Add(edge.i1);
                        lastPos = inVertices[edge.i2].Position;
                    }
                    else
                    {
                        finalIndices.Add(edge.i2);
                        lastPos = inVertices[edge.i1].Position;
                    }
                    tmpEdges.Remove(edge);
                }
                else
                {
                    // FAIL
                    return false;
                }
            }
            finalIndices = finalIndices.Distinct().ToList();
            return true;
        }

        public static void RecalculateTangents(List<VertexData> vertices, List<Triangle> triangles)
        {
            var tan1 = new Vector3[vertices.Count];
            var tan2 = new Vector3[vertices.Count];
            foreach (var triangle in triangles)
            {
                var i1 = triangle.i1;
                var i2 = triangle.i2;
                var i3 = triangle.i3;

                var v1 = vertices[i1].Position;
                var v2 = vertices[i2].Position;
                var v3 = vertices[i3].Position;

                var w1 = vertices[i1].UV;
                var w2 = vertices[i2].UV;
                var w3 = vertices[i3].UV;

                var x1 = v2.x - v1.x;
                var x2 = v3.x - v1.x;
                var y1 = v2.y - v1.y;
                var y2 = v3.y - v1.y;
                var z1 = v2.z - v1.z;
                var z2 = v3.z - v1.z;

                var s1 = w2.x - w1.x;
                var s2 = w3.x - w1.x;
                var t1 = w2.y - w1.y;
                var t2 = w3.y - w1.y;

                var r = 1.0f / (s1 * t2 - s2 * t1);
                var sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r,
                      (t2 * z1 - t1 * z2) * r);
                var tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r,
                      (s1 * z2 - s2 * z1) * r);

                tan1[i1] += sdir;
                tan1[i2] += sdir;
                tan1[i3] += sdir;

                tan2[i1] += tdir;
                tan2[i2] += tdir;
                tan2[i3] += tdir;
            }

            for (var a = 0; a < vertices.Count; a++)
            {
                var n = vertices[a].Normal;
                var t = tan1[a];

                // Gram-Schmidt orthogonalize
                vertices[a].Tangent = (t - n * Vector3.Dot(n, t)).normalized;

                // Calculate handedness
                vertices[a].Tangent.w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
            }
        }
    }
}

