using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SerializaahNS.Serializers
{
	public abstract class SerializerBase
	{
		public abstract bool CanSerialize(object value);
		public abstract bool CanDeserialize(string value, Type targetType);

		public abstract string Serialize(object value);
		public abstract object Deserialize(string value, Type type);

		internal static string ToBase64(string data)
		{
			var encodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(data);
			return System.Convert.ToBase64String(encodeAsBytes);
		}

        internal static string FromBase64(string data)
		{
			var encodedDataAsBytes = System.Convert.FromBase64String(data);
			return System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
		}

        internal static string SimpleQualifiedName(Type t)
        {
            var simple = t.AssemblyQualifiedName;
            simple = Regex.Replace(simple, @", Version=\d+.\d+.\d+.\d+", string.Empty);
            simple = Regex.Replace(simple, @", Culture=\w+", string.Empty);
            simple = Regex.Replace(simple, @", PublicKeyToken=\w+", string.Empty);
            simple = simple.Replace(System.Reflection.Assembly.GetCallingAssembly().GetName().Name, "SerializahCoreAssembly");
            //UnityEngine.Debug.Log("Type: " + t.AssemblyQualifiedName + "Simple: " + simple);
            return simple;
        }

        internal static Type GetType(string typeString)
        {
            typeString = typeString.Replace("SerializahCoreAssembly", System.Reflection.Assembly.GetCallingAssembly().GetName().Name);
            var type = Type.GetType(typeString);
            if (type == null)
                UnityEngine.Debug.Log("Type not found: " + typeString);
            return type; 
        }
	}

	public static class Serializer
	{
		static List<SerializerBase> availableSerializers = new List<SerializerBase>(new SerializerBase[]
			{
				new SerializahhSerializer(),
                new EnumSerializer(),
				new PrimitiveSerializer(),
                //new UnityObjectSerializer(),
				new DictionarySerializer(),
				new ArraySerializer(),
				new ListSerializer(),
				new ClassSerializer()
			});


		public static List<SerializerBase> AvailableSerializers { get { return availableSerializers; } }

		public static string Serialize(object value)
		{
			if (value == null)
				return null;
			foreach (var serializer in availableSerializers)
				if (serializer.CanSerialize(value))
					return serializer.Serialize(value);
			return null;
		}

		public static object Deserialize(string value, Type type)
		{
			foreach (var serializer in availableSerializers)
				if (serializer.CanDeserialize(value, type))
					return serializer.Deserialize(value, type);
			return null;
		}

		public static T Deserialize<T>(string value)
		{
			return (T)Deserialize(value, typeof(T));
		}
	}
}
