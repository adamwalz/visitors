using System;
using System.Linq;

namespace SerializaahNS.Serializers
{
	public class UnityObjectSerializer : SerializerBase
	{
		public override bool CanSerialize(object value)
		{
            return (value is UnityEngine.Object);
		}
		public override bool CanDeserialize(string value, Type targetType)
		{
            return (typeof(UnityEngine.Object).IsAssignableFrom(targetType));
        }

		public override string Serialize(object value)
		{
            var obj = (UnityEngine.Object)value;
            if (obj == null)
                return null;
            var result = UnityEditor.AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(result))
                result = obj.name;

            return result;
		}
		public override object Deserialize(string value, Type type)
		{
            var primitiveNames = Enum.GetNames(typeof(UnityEngine.PrimitiveType));
            if (primitiveNames.Contains(value))
            {
                var gameObject = (UnityEngine.GameObject)UnityEngine.GameObject.CreatePrimitive((UnityEngine.PrimitiveType)Enum.Parse(typeof(UnityEngine.PrimitiveType), value));
                var sharedMesh = gameObject.GetComponent<UnityEngine.MeshFilter>().sharedMesh;

                UnityEngine.GameObject.DestroyImmediate(gameObject);

                return sharedMesh;
            }
            return UnityEditor.AssetDatabase.LoadAssetAtPath(value, type);
		}
	}
}
