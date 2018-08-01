namespace TinyBee
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using UnityEngine;

	public static class ReflectionExtension : object
	{
		public static bool HasAttribute(this PropertyInfo self, Type attributeType, bool inherit)
		{
			return self.GetCustomAttributes(attributeType, inherit).Length > 0;
		}

		public static bool HasAttribute(this FieldInfo self, Type attributeType, bool inherit)
		{
			return self.GetCustomAttributes(attributeType, inherit).Length > 0;
		}

		public static bool HasAttribute(this Type self, Type attributeType, bool inherit)
		{
			return self.GetCustomAttributes(attributeType, inherit).Length > 0;
		}

		public static bool HasAttribute(this MethodInfo self, Type attributeType, bool inherit)
		{
			return self.GetCustomAttributes(attributeType, inherit).Length > 0;
		}

		public static object InvokeByReflect(this object self, string methodName, params object[] args)
		{
			var methodInfo = self.GetType().GetMethod(methodName);
			return methodInfo == null ? null : methodInfo.Invoke(self, args);
		}

		public static object GetFieldByReflect(this object self, string fieldName)
		{
			var fieldInfo = self.GetType().GetField(fieldName);
			return fieldInfo == null ? null : fieldInfo.GetValue(self);
		}

		public static object GetPropertyByReflect(this object self, string propertyName, object[] index = null)
		{
			var propertyInfo = self.GetType().GetProperty(propertyName);
			return propertyInfo == null ? null : propertyInfo.GetValue(self, index);
		}
	}
}