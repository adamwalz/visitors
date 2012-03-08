using System;
using System.Collections.Generic;
using UnityEngine;

namespace MeshUtilities
{
	public abstract class Triangulator
	{
		public abstract bool FillEdges(List<VertexData> vertices, 
		                      List<Triangle> outTriangles, 
		                      List<MeshUtilities.Edge> triangulationEdges, 
		                      VertexDeclaration vertexDeclaration, 
		                      Vector3 planeNormal, 
		                      Vector3 planePosition, 
		                      SliceAreaSettings sliceAreaSettings);
		
	}
	
	public class TriangulatorAttribute : NamedAttribute
	{
		
		public TriangulatorAttribute()
			: base()
		{
		}
		
		public TriangulatorAttribute(string name)
			: base(name)
		{
		}
		
		static KeyValuePair<TriangulatorAttribute, Type>[] availableAttributes;
		public static KeyValuePair<TriangulatorAttribute, Type>[] GetAvailable()
		{
			if (availableAttributes == null)
				availableAttributes = NamedAttribute.GetAvailable<TriangulatorAttribute>();
			return availableAttributes;
		}
	}
}