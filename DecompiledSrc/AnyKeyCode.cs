using System;

public struct AnyKeyCode
{
	public readonly object value;

	public readonly Type valueType;

	public readonly Func<object, object, bool> equalsCriteria;

	public AnyKeyCode(object value, Type valueType = null, Func<object, object, bool> equalsCriteria = null)
	{
		if (value is AnyKeyCode)
		{
			throw new ArgumentException("Your value should not be an instance of AnyKeyCode");
		}
		this.value = value;
		this.valueType = valueType ?? value?.GetType() ?? typeof(object);
		this.equalsCriteria = equalsCriteria ?? new Func<object, object, bool>(DefaultEqualsCriteria);
	}

	public bool Equals(AnyKeyCode keyCode)
	{
		return equalsCriteria(keyCode.value, value);
	}

	public static bool operator ==(AnyKeyCode keyCode, AnyKeyCode otherKeyCode)
	{
		return keyCode.Equals(otherKeyCode);
	}

	public static bool operator !=(AnyKeyCode keyCode, AnyKeyCode otherKeyCode)
	{
		return !keyCode.Equals(otherKeyCode);
	}

	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override string ToString()
	{
		if (value != null)
		{
			return value.ToString();
		}
		return "null";
	}

	public string ToStringWithType()
	{
		return "<" + valueType.Name + ">" + ToString();
	}

	public T Cast<T>()
	{
		return (T)value;
	}

	public static AnyKeyCode[] CreateKeyCodeArray(object[] values, Type valueType = null, Func<object, object, bool> equalsCriteria = null)
	{
		AnyKeyCode[] array = new AnyKeyCode[values.Length];
		if ((object)valueType == null)
		{
			valueType = ((values.Length != 0) ? values[0].GetType() : typeof(object));
		}
		for (int i = 0; i < values.Length; i++)
		{
			array[i] = new AnyKeyCode(values[i], valueType, equalsCriteria);
		}
		return array;
	}

	private static bool DefaultEqualsCriteria(object o1, object o2)
	{
		return o1 == o2;
	}
}
