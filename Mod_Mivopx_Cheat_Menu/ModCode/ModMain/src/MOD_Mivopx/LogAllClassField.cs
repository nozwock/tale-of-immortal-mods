using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TaleOfImmortalCheat.UI;

namespace MOD_Mivopx;

public class LogAllClassField
{
	private static readonly HashSet<object> VisitedObjects;

	static LogAllClassField()
	{
		VisitedObjects = new HashSet<object>();
		UIRefreshManager.OnLanguageChanged += UpdateUITexts;
	}

	public static void LogAllFieldsAndProperties(object obj, int depth = 0)
	{
		if (obj == null)
		{
			ModMain.Log("Object is null");
			return;
		}
		Type type = obj.GetType();
		if (VisitedObjects.Contains(obj))
		{
			ModMain.Log(new string('-', depth * 2) + "[Circular reference detected for type: " + type.FullName + "]");
			return;
		}
		VisitedObjects.Add(obj);
		StringBuilder stringBuilder = new StringBuilder();
		string text = new string(' ', depth * 2);
		stringBuilder.AppendLine(text + "Logging all fields and properties for Type: " + type.FullName);
		FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (FieldInfo fieldInfo in fields)
		{
			object obj2 = null;
			try
			{
				obj2 = fieldInfo.GetValue(obj);
			}
			catch (Exception ex)
			{
				stringBuilder.AppendLine(text + "Field: " + fieldInfo.Name + ", [Error: " + ex.Message + "]");
				continue;
			}
			stringBuilder.AppendLine(text + "Field: " + fieldInfo.Name + ", Type: " + fieldInfo.FieldType.Name + ", Value: " + FormatValue(obj2));
			if (IsComplexType(fieldInfo.FieldType) && (fieldInfo.Name == "_allConfList" || fieldInfo.Name == "allConfList"))
			{
				LogAllFieldsAndProperties(obj2, depth + 1);
			}
		}
		PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (PropertyInfo propertyInfo in properties)
		{
			if (propertyInfo.CanRead)
			{
				object obj3 = null;
				try
				{
					obj3 = propertyInfo.GetValue(obj, null);
				}
				catch (Exception ex2)
				{
					stringBuilder.AppendLine(text + "Property: " + propertyInfo.Name + ", [Error: " + ex2.Message + "]");
					continue;
				}
				stringBuilder.AppendLine(text + "Property: " + propertyInfo.Name + ", Type: " + propertyInfo.PropertyType.Name + ", Value: " + FormatValue(obj3));
				if (IsComplexType(propertyInfo.PropertyType) && (propertyInfo.Name == "_allConfList" || propertyInfo.Name == "allConfList"))
				{
					LogAllFieldsAndProperties(obj3, depth + 1);
				}
			}
		}
		ModMain.Log(stringBuilder.ToString());
		VisitedObjects.Remove(obj);
	}

	private static bool IsComplexType(Type type)
	{
		if (!type.IsPrimitive && !(type == typeof(string)))
		{
			return !type.IsEnum;
		}
		return false;
	}

	private static string FormatValue(object value, int depth = 0)
	{
		if (value == null)
		{
			return "null";
		}
		if (value is IEnumerable enumerable && !(value is string))
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(new string(' ', depth * 2) + "[");
			foreach (object item in enumerable)
			{
				stringBuilder.AppendLine(new string(' ', (depth + 1) * 2) + FormatValue(item, depth + 1));
			}
			stringBuilder.AppendLine(new string(' ', depth * 2) + "]");
			return stringBuilder.ToString();
		}
		return value.ToString();
	}

	private static void UpdateUITexts()
	{
	}
}
