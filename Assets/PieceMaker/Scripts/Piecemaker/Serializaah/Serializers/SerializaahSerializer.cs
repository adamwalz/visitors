using System;
using System.Text;

namespace SerializaahNS.Serializers
{
	public class SerializahhSerializer : SerializerBase
	{
		public override bool CanSerialize(object value)
		{
			return (value is Serializaah);
		}

		public override bool CanDeserialize(string value, Type targetType)
		{
			return (typeof(Serializaah).IsAssignableFrom(targetType));
		}

		public override string Serialize(object value)
		{
			var serializaah = (Serializaah)value;
			serializaah.SaveChanges();
			var serializedData = serializaah.SerializedData;
			var sb = new StringBuilder();
            sb.AppendLine(SerializerBase.SimpleQualifiedName(value.GetType()));
			foreach (var data in serializedData)
			{
				var secondData = string.Empty;
				if (data.SerializedValue != null)
					secondData = ToBase64(data.SerializedValue);

				sb.AppendLine(data.Name + ":" + secondData);
			}

			return sb.ToString().Replace("\r", string.Empty);
		}

		public override object Deserialize(string value, Type type)
		{
			if (string.IsNullOrEmpty(value))
				return null;
			var lines = value.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var realType = SerializerBase.GetType(lines[0]);
			var instance = (Serializaah)System.Activator.CreateInstance(realType);
			for (int i = 1; i < lines.Length; ++i)
			{
				var line = lines[i];
				var elements = line.Split(new char[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);

				var serializedValue = "";
				if (elements.Length == 2)
					serializedValue = FromBase64(elements[1]);

				var data = new SerializaahData() { Name = elements[0], SerializedValue = serializedValue };
				data.UpdateTo(instance);
				instance.SerializedData.Add(data);
			}

			return instance;
		}
	}
}
