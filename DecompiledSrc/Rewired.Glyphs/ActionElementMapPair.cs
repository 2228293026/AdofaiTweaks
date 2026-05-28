using System;
using System.Text;

namespace Rewired.Glyphs;

public struct ActionElementMapPair : IEquatable<ActionElementMapPair>
{
	public ActionElementMap a;

	public ActionElementMap b;

	public int Count => ((a != null) ? 1 : 0) + ((b != null) ? 1 : 0);

	public ActionElementMapPair(ActionElementMap a, ActionElementMap b)
	{
		this.a = a;
		this.b = b;
	}

	public bool Equals(ActionElementMapPair other)
	{
		if (a == other.a)
		{
			return b == other.b;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj == null || !(obj is ActionElementMapPair))
		{
			return false;
		}
		return Equals((ActionElementMapPair)obj);
	}

	public override int GetHashCode()
	{
		return (17 * 29 + ((object)a).GetHashCode()) * 29 + ((object)b).GetHashCode();
	}

	public static bool operator ==(ActionElementMapPair a, ActionElementMapPair b)
	{
		if (a.a == b.a)
		{
			return a.b == b.b;
		}
		return false;
	}

	public static bool operator !=(ActionElementMapPair a, ActionElementMapPair b)
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
