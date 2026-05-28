using System;
using System.Collections.Generic;
using System.Text;

namespace Rewired.Glyphs;

public struct Pair<T>(T a, T b) : IEquatable<Pair<T>>
{
	public T a = a;

	public T b = b;

	public bool Equals(Pair<T> other)
	{
		if (EqualityComparer<T>.Default.Equals(a, other.a))
		{
			return EqualityComparer<T>.Default.Equals(b, other.b);
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj == null || !(obj is Pair<T>))
		{
			return false;
		}
		return Equals((Pair<T>)obj);
	}

	public override int GetHashCode()
	{
		return (17 * 29 + a.GetHashCode()) * 29 + b.GetHashCode();
	}

	public static bool operator ==(Pair<T> a, Pair<T> b)
	{
		if (EqualityComparer<T>.Default.Equals(a.a, b.a))
		{
			return EqualityComparer<T>.Default.Equals(a.b, b.b);
		}
		return false;
	}

	public static bool operator !=(Pair<T> a, Pair<T> b)
	{
		return !(a == b);
	}

	public override string ToString()
	{
		return new StringBuilder().Append("a: ").Append(a).AppendLine()
			.Append("b: ")
			.Append(b)
			.ToString();
	}
}
