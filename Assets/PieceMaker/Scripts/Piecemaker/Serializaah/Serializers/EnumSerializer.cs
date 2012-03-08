using System;

namespace SerializaahNS.Serializers
{
	public class EnumSerializer : SerializerBase
	{
		public override bool CanSerialize(object value)
		{
			return (value is Enum);
		}
		public override bool CanDeserialize(string value, Type targetType)
		{
            return targetType.IsEnum;
		}

		public override string Serialize(object value)
		{
			return (string)value.ToString();
		}
		public override object Deserialize(string value, Type type)
		{
            return Enum.Parse(type, value);
		}
	}
}
