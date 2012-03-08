using System;
using System.Linq;
using System.Collections.Generic;

public abstract class NamedAttribute : Attribute
{
	public string Name { get; set; }
	
	public NamedAttribute()
	{
	}
	
	public NamedAttribute(string name)
	{
		Name = name;
	}
	
	public static KeyValuePair<T, Type>[] GetAvailable<T>() where T : NamedAttribute
	{
		var attributesByType =
		    (from a in System.AppDomain.CurrentDomain.GetAssemblies()
		    from t in a.GetTypes()
		    let attributes = t.GetCustomAttributes(typeof(T), true)
		    where attributes != null && attributes.Length > 0
		    select new KeyValuePair<T, System.Type>(attributes.Cast<T>().FirstOrDefault(), t))
				.ToArray();
		return attributesByType;
	}
}
