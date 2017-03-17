﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public class PropertyChange {

	private const BindingFlags bflags = BindingFlags.Public | BindingFlags.Instance;
	
	public static object SetValue(object component, string property, object value, string subproperty = null)
	{
		System.Type type = GetValue (component, property).GetType ();
		if (type == typeof(Vector2)) {
			Vector2 v;
			if (subproperty == null || subproperty.Length == 0) {
				v = (component as Vector2?).GetValueOrDefault();
				if (value.GetType() == typeof(float))	// uniform scale if float
					v.x = v.y = (float)value;
				if (value.GetType() == typeof(Vector3))
					v = (Vector2)value;

			} else {
				v = (Vector2)GetValue(component, property);
				if (subproperty == "x") v.x = (float)value;
				if (subproperty == "y") v.y = (float)value;
			}
			value = (object)v;
		}
		if (type == typeof(Vector3)) {
			Vector3 v;
			if (subproperty == null || subproperty.Length == 0) {
				v = (component as Vector3?).GetValueOrDefault();
				if (value.GetType() == typeof(float))
					v.x = v.y = v.z = (float)value;
				if (value.GetType() == typeof(Vector3))
					v = (Vector3)value;
			} else {
				v = (Vector3)GetValue(component, property);
				if (subproperty == "x") v.x = (float)value;
				if (subproperty == "y") v.y = (float)value;
				if (subproperty == "z") v.z = (float)value;
			}
			value = (object)v;
		}
		
		
		if (type == typeof(int)) {
			value = System.Convert.ToInt32(value);
		}
		
		if (type == typeof(Color)) {
			Color c = (Color)GetValue(component, property);
			if (subproperty == null || subproperty.Length == 0) {
				c = (Color)value;
			} else {
				if (subproperty == "r") c.r = (float)value;
				if (subproperty == "g") c.g = (float)value;
				if (subproperty == "b") c.b = (float)value;
				if (subproperty == "a") c.a = (float)value;
			}
			value = (object)c;
		}
		
		if (component.GetType().GetProperty(property, bflags) != null) {
			
			component.GetType().GetProperty(property, bflags).SetValue(component, value, null);
			return GetValue(component, property);
		}
		if (component.GetType().GetField(property, bflags) != null) {
			component.GetType().GetField(property, bflags).SetValue(component, value);
			return GetValue(component, property);
		}
		return GetValue(component, property);
	}
	#region static
	public static List<string> GetProperties(object source)
	{
		List<string> props = new List<string>();
		if (source != null) {
			// property = public string myField { get { x } set { x } }
			foreach (PropertyInfo pi in source.GetType().GetProperties(bflags)) {
				if(isTypeSupported(pi.PropertyType) && pi.CanWrite && !pi.GetSetMethod().IsStatic && !pi.IsDefined(typeof(System.ObsoleteAttribute), true))
					props.Add(pi.Name);
			}
			// field  = private string myField;
			foreach( FieldInfo fi in source.GetType().GetFields(bflags)) {
				if(isTypeSupported(fi.FieldType) && fi.IsPublic && !fi.IsLiteral && !fi.IsStatic)
					props.Add(fi.Name);
			}
			
		}
		return props;
	}
	public static object GetValue(object source, string property, string subproperty = "")
	{
		object val = null;
		if (source != null && property != null) {

			// property = public string myField { get { x } set { x } }
			if (source.GetType().GetProperty(property, bflags) != null)
				val = source.GetType().GetProperty(property, bflags).GetValue(source, null);

			// field  = private string myField;
			if (source.GetType().GetField(property, bflags) != null)
				val = source.GetType().GetField(property, bflags).GetValue(source);

			if (subproperty.Length > 0 && GetValue(val, subproperty) != null) {
				val = GetValue(val, subproperty);
			}
		}
		return val;
	}
	public static bool isTypeSupported(System.Type PropertyType) {
		System.Type[] supportedTypes = {
			typeof(Vector2),
			typeof(Vector3),
			typeof(float),
			typeof(double),
			typeof(int),
			typeof(string),
			typeof(Color)
		};
		return supportedTypes.Contains(PropertyType);
	}
	#endregion
	
	
}
