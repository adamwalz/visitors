using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace MeshUtilities.UVMappers
{
	[UVMapper("Plane")]
	public class PlaneMapper : UVMapper
	{
		[HelpAttribute("Factor by which each texture coordinate will be translated.")]
		public Vector2 Offset { get; set; }
		
		[HelpAttribute("Factor by which each texture coordinate will be scaled.")]
		public Vector2 Scale { get; set; }

        [HelpAttribute("True when the incoming raw texture coordinates should be normalized (In a range from 0.0 to 1.0.).")]
        public bool Normalize { get; set; }

        [HelpAttribute("True when you want to use a bounding rect to restrict the generated UV coordinates.")]
        public bool UseBounds { get; set; }

        [HelpAttribute("Bounding rect used to restrict the calculated UV coordinates to. Use this to specify a region in your texture which should be used for the cut area.")]
        public Vector4 Bounds { get; set; }
		
		public PlaneMapper()
		{
			Offset = Vector2.zero;
            Scale = Vector2.one;
            UseBounds = false;
            Bounds = new Vector4(0, 0, 1, 1);
            Normalize = true;
		}
		
		public override void Apply(List<VertexData> vertices, Matrix4x4 planeOrthMatrix)
		{
            var data = vertices.Select(v => new
            {
                Point2d = Helper.Calc2DPoint(v.Position, planeOrthMatrix),
                Vertex = v
            }).ToArray();

            if (Normalize)
            {
                var min = data[0].Point2d;
                var max = min;

                foreach (var d in data)
                {
                    min = Vector2.Min(min, d.Point2d);
                    max = Vector2.Max(max, d.Point2d);
                }

                var dif = (max - min);
                data = data.Select(d =>
                    {
                        var newP2D = (d.Point2d - min);
                        newP2D.x /= dif.x;
                        newP2D.y /= dif.y;

                        return new
                        {
                            Point2d = newP2D,
                            Vertex = d.Vertex
                        };
                    }).ToArray();
            }

            foreach (var d in data)
            {
                if (UseBounds)
                {
                    if (Normalize)
                    {
                        var u = d.Point2d.x * (Bounds.z - Bounds.x) + Bounds.x;
                        var v = d.Point2d.y * (Bounds.w - Bounds.y) + Bounds.y;
                        d.Vertex.UV = new Vector2(u, v);
                    }
                    else
                    {
                        var uv = Offset + new Vector2(  Scale.x * d.Point2d.x,
                                                        Scale.y * d.Point2d.y);

                        var u = Mathf.Clamp(uv.x, Bounds.x, Bounds.z);
                        var v = Mathf.Clamp(uv.y, Bounds.y, Bounds.w);
                        d.Vertex.UV = new Vector2(u, v);
                    }
                }
                else
                {
                    d.Vertex.UV = Offset + new Vector2( Scale.x * d.Point2d.x,
                                                        Scale.y * d.Point2d.y);
                }
            }
		}
	}
}

