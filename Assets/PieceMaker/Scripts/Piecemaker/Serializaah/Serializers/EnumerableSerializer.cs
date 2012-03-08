using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SerializaahNS.Serializers
{
	public abstract class EnumerableSerializer : SerializerBase
	{
		public override bool CanSerialize(object value)
		{
			return (value is IEnumerable);
		}
		public override string Serialize(object value)
		{
			var sb = new StringBuilder();
            sb.AppendLine(SerializerBase.SimpleQualifiedName(value.GetType()));
			var enumerable = (IEnumerable)value;

			var elementType = enumerable.GetType().GetElementType();
			if (elementType == null)
			{
				var interfaces = enumerable.GetType().GetInterfaces();
				foreach (var interf in interfaces)
					if (interf.IsGenericType && interf.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)))
						elementType = interf.GetGenericArguments()[0];
			}

			sb.AppendLine(SerializerBase.SimpleQualifiedName(elementType));
			foreach (var entry in enumerable)
			{
                string type = string.Empty;
                string val = string.Empty;
                if (entry != null)
                {
                    type = ToBase64(SerializerBase.SimpleQualifiedName(entry.GetType()));
				    val = ToBase64(Serializer.Serialize(entry));
                }
				sb.AppendLine(type + ":" + val);
			}
			return sb.ToString().Replace("\r", string.Empty);
		}
	}

	public class ArraySerializer : EnumerableSerializer
	{
		public override bool CanDeserialize(string value, Type targetType)
		{
			return (typeof(Array).IsAssignableFrom(targetType));
		}

		public override object Deserialize(string value, Type type)
		{
			if (string.IsNullOrEmpty(value))
				return null;
			var lines = value.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            var elementType = SerializerBase.GetType(lines[1]);
			var instance = Array.CreateInstance(elementType, lines.Length - 2);
			for (int i = 2; i < lines.Length; ++i)
			{
				var line = lines[i];
				var elements = line.Split(new char[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (elements.Length == 0)
                    continue;
                var serializedType = SerializerBase.GetType(FromBase64(elements[0]));
				var serializedValue = FromBase64(elements[1]);
				var entry = Serializer.Deserialize(serializedValue, serializedType);
				instance.SetValue(entry, i - 2);
			}

			return instance;
		}
	}

	public class ListSerializer : EnumerableSerializer
	{
		public override bool CanDeserialize(string value, Type targetType)
		{
			return (typeof(IList).IsAssignableFrom(targetType));
		}

		public override object Deserialize(string value, Type type)
		{
			if (string.IsNullOrEmpty(value))
				return null;
            var lines = value.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var enumerableType = SerializerBase.GetType(lines[0]);
			var instance = (IList)System.Activator.CreateInstance(enumerableType);

			for (int i = 2; i < lines.Length; ++i)
			{
				var line = lines[i];
				var elements = line.Split(new char[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
                var serializedType = SerializerBase.GetType(FromBase64(elements[0]));
				var serializedValue = FromBase64(elements[1]);
				var entry = Serializer.Deserialize(serializedValue, serializedType);
				instance.Add(entry);
			}

			return instance;
		}
	}

	public class DictionarySerializer : EnumerableSerializer
	{
		public override bool CanDeserialize(string value, Type targetType)
		{
			return (typeof(IDictionary).IsAssignableFrom(targetType));
		}

		public override object Deserialize(string value, Type type)
		{
			if (string.IsNullOrEmpty(value))
				return null;
			var lines = value.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var enumerableType = SerializerBase.GetType(lines[0]);
            var elementType = SerializerBase.GetType(lines[1]);

			var instance = (IDictionary)System.Activator.CreateInstance(enumerableType);
			var keyFieldInfo = elementType.GetField("key", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
			var valueFieldInfo = elementType.GetField("value", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

			for (int i = 2; i < lines.Length; ++i)
			{
				var line = lines[i];
				var elements = line.Split(new char[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
                var serializedType = SerializerBase.GetType(FromBase64(elements[0]));
				var serializedValue = FromBase64(elements[1]);
				var entry = Serializer.Deserialize(serializedValue, serializedType);
				instance.Add(keyFieldInfo.GetValue(entry), valueFieldInfo.GetValue(entry));
			}

			return instance;
		}
	}
}
