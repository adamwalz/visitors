using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace SerializaahNS.Serializers
{
	public class ClassSerializer : SerializerBase
	{
		public override bool CanSerialize(object value)
		{
			return value.GetType().IsClass || value.GetType().IsValueType;
		}

		public override bool CanDeserialize(string value, Type targetType)
		{
			return targetType.IsClass || targetType.IsValueType;
		}

		public override string Serialize(object value)
		{
			var sb = new StringBuilder();
            sb.AppendLine(SerializerBase.SimpleQualifiedName(value.GetType()));
			var fields = value.GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
			var properties = value.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
			foreach (var field in fields)
			{
				if (NonSerializedAttribute.IsDefined(field, typeof(NonSerializedAttribute)))
					continue;
                if (!field.IsPublic && !SerializationAttribute.IsDefined(field, typeof(SerializationAttribute)))
                    continue;
                if (field.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false))
                    continue;
                if (field.FieldType == typeof(List<SerializaahData>))
                    continue;

				var secondData = string.Empty;
				var fieldValue = field.GetValue(value);
                if (fieldValue != null)
                {
                    var serializedValue = Serializer.Serialize(fieldValue);
                    secondData = ToBase64(serializedValue);
                    //UnityEngine.Debug.Log("Name: " + field.Name + " Value: " + serializedValue);
                }
				sb.AppendLine(field.Name + ":" + secondData);
			}
			foreach (var property in properties)
			{
				if (!property.CanRead || !property.CanWrite || property.GetIndexParameters().Length > 0)
					continue;
				if (NonSerializedAttribute.IsDefined(property, typeof(NonSerializedAttribute)))
					continue;

				var secondData = string.Empty;
				var propertyValue = property.GetValue(value, null);
				if (propertyValue != null)
                {
                    var serializedValue = Serializer.Serialize(propertyValue);
                    secondData = ToBase64(serializedValue);
                    //UnityEngine.Debug.Log("Name: " + property.Name + " Value: " + serializedValue);
                }
				sb.AppendLine(property.Name + ":" + secondData);
			}

			return sb.ToString().Replace("\r", string.Empty);
		}

		public override object Deserialize(string value, Type type)
		{
			if (string.IsNullOrEmpty(value))
				return null;
			var lines = value.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
			var realType = SerializerBase.GetType(lines[0]);
            var constructor = realType.GetConstructors().OrderBy(ctor => ctor.GetParameters().Length).First();
            var constructorParams = constructor.GetParameters();
            var constructorObjectParams = new object[constructorParams.Length];
            for (int i = 0; i < constructorParams.Length; ++i)
            {
                if (constructorParams[i].ParameterType.IsValueType)
                    constructorObjectParams[i] = Activator.CreateInstance(constructorParams[i].ParameterType);
            }
            var instance = constructor.Invoke(constructorObjectParams);
			var fields = realType.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
			var properties = realType.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
			for (int i = 1; i < lines.Length; ++i)
			{
				var line = lines[i];
				var elements = line.Split(new char[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);

				var serializedValue = "";
				if (elements.Length == 2)
					serializedValue = FromBase64(elements[1]);

				var field = fields.FirstOrDefault(f => f.Name == elements[0]);
				var property = properties.FirstOrDefault(f => f.Name == elements[0]);
				if (field != null)
				{
					if (NonSerializedAttribute.IsDefined(field, typeof(NonSerializedAttribute)))
						continue;
                    if (!field.IsPublic && !SerializationAttribute.IsDefined(field, typeof(SerializationAttribute)))
                        continue;
                    if (field.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false))
                        continue;
                    if (field.FieldType == typeof(List<SerializaahData>))
                        continue;
                    //UnityEngine.Debug.Log("Name: " + field.Name + " Value: " + serializedValue);

					field.SetValue(instance, Serializer.Deserialize(serializedValue, field.FieldType));
				}
				if (property != null)
				{
                    if (!property.CanRead || !property.CanWrite || property.GetIndexParameters().Length > 0)
						continue;
					if (NonSerializedAttribute.IsDefined(property, typeof(NonSerializedAttribute)))
						continue;
                    //UnityEngine.Debug.Log("Name: " + property.Name + " Value: " + serializedValue);
					property.SetValue(instance, Serializer.Deserialize(serializedValue, property.PropertyType), null);
				}
			}

			return instance;
		}
	}
}
