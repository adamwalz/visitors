using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MeshUtilities;

public class Slicer
{
	public VirtualMesh[] Slice(VirtualMesh mesh, UnityEngine.Plane cutPlane, SliceSettings sliceSettings)
	{
        //if (!Helper.IsPlaneInBounds(mesh.Bounds, cutPlane))
        //    return new VirtualMesh[0];
		
		var sliceResult = Tools.SplitMesh(cutPlane, mesh, sliceSettings);
		if (sliceResult.Success)
			return new VirtualMesh[] { sliceResult.PositiveResult, sliceResult.NegativeResult };
		return new VirtualMesh[0];		
	}
	
	public VirtualMesh[] Slice(VirtualMesh mesh, MultiSliceSettings sliceSettings, object syncRoot, ref int currentSliceIndex)
	{
		var sliceResult = new List<VirtualMesh>();
		foreach (var slicePlane in sliceSettings.SlicePlanes)
		{
            lock (syncRoot)
                currentSliceIndex++;
			if (sliceResult.Count == 0)
			{
				var cutResult = Slice(mesh, slicePlane, sliceSettings);
				sliceResult.AddRange(cutResult);
			}
			else
			{
				var isolatedCopy = sliceResult.ToList();
				foreach (var o in isolatedCopy)
				{
					var cutResult = Slice(o, slicePlane, sliceSettings);
					if (cutResult.Length > 0)
					{
						sliceResult.Remove(o);
						sliceResult.AddRange(cutResult);
					}
				}
			}
		}
        var result = sliceResult.Distinct().ToArray();
        //if (result.Length < 4)
        //{
        //    var maxX = mesh.Vertices.Max(v => v.x);
        //    var maxY = mesh.Vertices.Max(v => v.y);
        //    var maxZ = mesh.Vertices.Max(v => v.z);

        //    var minX = mesh.Vertices.Min(v => v.x);
        //    var minY = mesh.Vertices.Min(v => v.y);
        //    var minZ = mesh.Vertices.Min(v => v.z);
        //    var d = string.Join("\n", sliceSettings.SlicePlanes.Select(p => string.Format("{0} - {1}", -p.normal * p.distance, p.normal)).ToArray());
        //    UnityEngine.Debug.Log(  "Max: " + maxX + " " + maxY + " " + maxZ + "\n" +
        //                            "Min: " + minX + " " + minY + " " + minZ + "\n" + mesh.Bounds + "\n\n" + d);
        //}
        
        return result;
	}
	
	public IEnumerable<UnityEngine.Plane> CreateSlicePlanes(Bounds bounds, int sliceCountX, int sliceCountY, int sliceCountZ, Vector3 chaosAngle)
	{
		var boundsCenter = bounds.center;
		var boundsExtends = bounds.extents;
		
		var slicePlanes = new List<UnityEngine.Plane>();
		for (int i = 0; i < sliceCountX; ++i)
		{
			var normal = Vector3.right;
			var point = boundsCenter;
			point.x = boundsCenter.x + (-1 + ((i+1) * 2) / (sliceCountX+1.0f)) * boundsExtends.x;
			
			slicePlanes.Add(new UnityEngine.Plane(normal, -point.x));
		}
		for (int i = 0; i < sliceCountY; ++i)
		{
			var normal = Vector3.up;
			var point = boundsCenter;
			point.y = boundsCenter.y + (-1 + ((i+1) * 2) / (sliceCountY+1.0f)) * boundsExtends.y;
			
			slicePlanes.Add(new UnityEngine.Plane(normal, -point.y));
		}
		
		for (int i = 0; i < sliceCountZ; ++i)
		{
			var normal = Vector3.forward;
			var point = boundsCenter;
			point.z = boundsCenter.z + (-1 + ((i+1) * 2) / (sliceCountZ+1.0f)) * boundsExtends.z;
			
			slicePlanes.Add(new UnityEngine.Plane(normal, -point.z));
		}
		if (chaosAngle != Vector3.zero)
		{
			System.Random random = new System.Random();
			slicePlanes = slicePlanes.Select(plane =>
				{
					var rotation = Quaternion.Euler(
				                                random.Next((int)chaosAngle.x),
				                                random.Next((int)chaosAngle.y),
				                                random.Next((int)chaosAngle.z));
					return new UnityEngine.Plane(rotation * plane.normal, plane.distance);
				}).ToList();
		}
		
		return slicePlanes;
	}
}
