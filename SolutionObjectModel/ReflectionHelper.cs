using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SolutionObjectModel
{
	/// <summary>
	/// Simplifies accessing members using reflection. The goal of this class is ease of use and not performance.
	/// </summary>
	internal static class ReflectionHelper
	{
		public static object GetProperty(this object obj, string name)
		{
			return obj.GetType()
				.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.GetValue(obj, null);
		}

		public static void SetProperty(this object obj, string name, object value)
		{
			obj.GetType()
				.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.SetValue(obj, value, null);
		}

		public static object InvokeMethod(this object obj, string name, params object[] args)
		{
			return obj.GetType()
				.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.Invoke(obj, args);
		}
	}
}
