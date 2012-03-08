using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SerializaahNS.Serializers;

namespace SerializaahNS
{
	public class SerializationAttribute : System.Attribute { }

    [System.Serializable]
	public class SerializaahData
	{
		[System.NonSerialized]
		public object Value;

		public string Name;
		public string SerializedValue;

		private PropertyInfo PropertyInfo;
		private FieldInfo FieldInfo;

		public void UpdateFrom(object instance)
		{
			EnsureInfo(instance);
			if (PropertyInfo == null && FieldInfo == null)
				return;

			object value = null;
			if (PropertyInfo != null)
				value = PropertyInfo.GetValue(instance, null);
			else if (FieldInfo != null)
				value = FieldInfo.GetValue(instance);

			Value = value;
            try
            {
                SerializedValue = Serializer.Serialize(value);
            }
            catch
            {
                UnityEngine.Debug.LogError(string.Format("Failed to serialize '{0}'", Name));
                throw;
            }
		}

		public void UpdateTo(object instance)
		{
			EnsureInfo(instance);
			if (Value == null && !(PropertyInfo == null && FieldInfo == null))
			{
                try
                {
                    if (PropertyInfo != null)
                        Value = Serializer.Deserialize(SerializedValue, PropertyInfo.PropertyType);
                    else if (FieldInfo != null)
                        Value = Serializer.Deserialize(SerializedValue, FieldInfo.FieldType);
                }
                catch
                {
                    UnityEngine.Debug.LogError(string.Format("Failed to deserialize '{0}'", Name));
                    throw;
                }
			}
            //UnityEngine.Debug.Log("Name: " + Name + " Value: " + Value);

			if (PropertyInfo != null)
				PropertyInfo.SetValue(instance, Value, null);
			else if (FieldInfo != null)
				FieldInfo.SetValue(instance, Value);
		}

		private void EnsureInfo(object instance)
		{
			if (PropertyInfo == null && FieldInfo == null)
			{
				if (PropertyInfo == null)
					PropertyInfo = instance.GetType().GetProperty(Name);
				if (FieldInfo == null)
					FieldInfo = instance.GetType().GetField(Name);
			}
		}

		internal void Clear()
		{
			PropertyInfo = null;
			FieldInfo = null;
			Value = null;
		}
	}

    [System.Serializable]
	public class Serializaah
	{
        [UnityEngine.SerializeField]
		public List<SerializaahData> SerializedData = new List<SerializaahData>();
		private bool hasBeenRefreshed = false;
	
		public Serializaah()
		{
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playmodeStateChanged = () =>
            {
                var isPlaying = UnityEditor.EditorApplication.isPlaying;
                var isChangingOrPlaying = UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode;

                //UnityEngine.Debug.Log(string.Format("isPlaying: {0} isPlayingOrWillChangePlaymode: {1}", isPlaying, isChangingOrPlaying));

                if ((!isPlaying && isChangingOrPlaying) || (isPlaying && !isChangingOrPlaying))
                    SaveChanges();
                else if ((isPlaying && isChangingOrPlaying) || (!isPlaying && !isChangingOrPlaying))
                    Load(true);
            };
#endif
		}

		public void SaveChanges()
		{
			ExtractProperties();
			ExtractFields();
		}

		public void NotifyChanges(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				SaveChanges();
				return;
			}
			var data = SerializedData.FirstOrDefault(entry => entry.Name == name);
			if (data == null)
				return;
			data.UpdateFrom(this);
		}

		public void Load(bool force)
		{
			if (!force && hasBeenRefreshed)
				return;
			hasBeenRefreshed = true;
			foreach (var data in SerializedData)
				data.UpdateTo(this);
		}

		private void ExtractProperties()
		{
			var thisType = this.GetType();
			var properties = thisType.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
			foreach (var propertyInfo in properties)
			{
				if (!propertyInfo.CanRead || !propertyInfo.CanWrite)
					continue;
				if (System.NonSerializedAttribute.IsDefined(propertyInfo, typeof(System.NonSerializedAttribute)))
					continue;
				var data = SerializedData.FirstOrDefault(entry => entry.Name == propertyInfo.Name);
				if (data == null)
				{
					data = new SerializaahData()
					{
						Name = propertyInfo.Name,
					};
					SerializedData.Add(data);
				}

				data.UpdateFrom(this);
			}
		}

		private void ExtractFields()
		{
			var thisType = this.GetType();
			var fields = thisType.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
			foreach (var fieldInfo in fields)
			{
				if (System.NonSerializedAttribute.IsDefined(fieldInfo, typeof(System.NonSerializedAttribute)))
					continue;
				if (!fieldInfo.IsPublic && !SerializationAttribute.IsDefined(fieldInfo, typeof(SerializationAttribute)))
					continue;
                if (fieldInfo.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false))
                    continue;
                if (fieldInfo.FieldType == typeof(List<SerializaahData>))
                    continue;
				var data = SerializedData.FirstOrDefault(entry => entry.Name == fieldInfo.Name);
				if (data == null)
				{
					data = new SerializaahData()
					{
						Name = fieldInfo.Name,
					};
					SerializedData.Add(data);
				}
				data.UpdateFrom(this);
			}
		}
	}
}
