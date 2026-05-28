using System;

namespace UnityEngine.Purchasing.Security;

public class GooglePlayTangle
{
	private static byte[] data = Convert.FromBase64String("KdDwMGEDEfX1stys4BVc4B5ryI/y0ET0JZ2CtUrP3o8Dg7PO+cxHZmjaWXpoVV5Rct4Q3q9VWVlZXVhbPIZH4eyzVvkUc1zh7uz4B+nY4QA8z0JiB7DmmzRCuyZyil8dA+ddOl+CSO4Q1qr3U902k6n0Uxs2MkBnFuvWIISsXyg40LeaskBZ9KHOF0C48UMhZdG91UF6RIbpzRFOxQc3GL6LsMsTZC4VC6nIX6PBzd3DzlTO2llXWGjaWVJa2llZWM0WjPUJaTEWYZw/TKHNR+8u4Tg3aM5cAEbTU76ih9tjb8EyR3vT0rLqQ2MbTZXDtmf2OsZowi34FIGw14eHUtP6Lfoddy5BEHttgfDz5RrETyy5i76HOzYyOhq6OMCaxVpbWVhZ");

	private static int[] order = new int[15]
	{
		9, 12, 9, 6, 6, 6, 6, 7, 10, 12,
		13, 11, 13, 13, 14
	};

	private static int key = 88;

	public static readonly bool IsPopulated = true;

	public static byte[] Data()
	{
		if (!IsPopulated)
		{
			return null;
		}
		return Obfuscator.DeObfuscate(data, order, key);
	}
}
