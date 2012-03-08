using System;

namespace SerializaahNS.Serializers
{
	public class PrimitiveSerializer : SerializerBase
	{
		public override bool CanSerialize(object value)
		{
			return (value is IConvertible || value.GetType().IsPrimitive);
		}
		public override bool CanDeserialize(string value, Type targetType)
		{
			return (targetType is IConvertible || targetType.IsPrimitive || targetType == typeof(string));
		}

		public override string Serialize(object value)
		{
			return (string)Convert.ChangeType(value, typeof(string));
		}
		public override object Deserialize(string value, Type type)
		{
			return Convert.ChangeType(value, type);
		}
	}
}
