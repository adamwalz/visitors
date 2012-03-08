using System;
using System.Collections.Generic;
using UnityEngine;
//using SerializaahNS;

namespace MeshUtilities
{
    public abstract class UVMapper
	{	
		public abstract void Apply(List<VertexData> vertices, Matrix4x4 planeOrthMatrix);
	}
	
	public class UVMapperAttribute : NamedAttribute
	{
		public UVMapperAttribute()
			: base()
		{
		}
		
		public UVMapperAttribute(string name)
			: base(name)
		{
		}
		
		static KeyValuePair<UVMapperAttribute, Type>[] availableAttributes;
		public static KeyValuePair<UVMapperAttribute, Type>[] GetAvailable()
		{
			if (availableAttributes == null)
				availableAttributes = NamedAttribute.GetAvailable<UVMapperAttribute>();
			return availableAttributes;
		}
	}
}

