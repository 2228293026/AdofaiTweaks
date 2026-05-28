using System;

namespace UnityEngine.Purchasing.Security;

public class AppleTangle
{
	private static byte[] data = Convert.FromBase64String("eZEWqj7p1bIyPnFuqCEfLpKpXdG1vW+MWU1L37ExX63m5f1u0/i9UiiHUjNmqfOShcLtaYXsaMxpLlHfR7kbF2IJXkgPAGrNqZU9JVm9y3E0mFaY6RMfHxsbHi58LxUuFxgdSz5dXy6cHzwuExgXNJhWmOkTHx8fcns+V3B9MC84LjoYHUsaFQ0DX25hX7aG58/UeII6dQ/OvaX6BTTdAZFtn37YBUUXMYys5lpW7n4mgAvranZxbHdqZy8ILgoYHUsaHQ0TX24unBqlLpwdvb4dHB8cHB8cLhMYFzguOhgdSxoVDQNfbm5yez5de2xqcHo+fXFwendqd3FwbT5xeD5rbXsYHUsDEBoIGgo1zndZimgX4Op1k3d4d31/andxcD5fa2p2cWx3amcvOvz1z6luwRFb/znU73Nm8/mrCQkWQC6cHw8YHUsDPhqcHxYunB8aLhGDI+01VzYE1uDQq6cQx0ACyNUjCC4KGB1LGh0NE19ubnJ7PkxxcWpkLpwfaC4QGB1LAxEfH+EaGh0cHxY1GB8bGxkcHwgAdmpqbm0kMTFpV8ZogS0Ke79pitczHB0fHh+9nB8jOHk+lC106ROc0cD1vTHnTXRFeq8uRvJEGiySdq2RA8B7beF5QHuiLg8YHUsaFA0UX25ucns+V3B9MC98cns+bWp/cHp/bHo+antsc20+f6DqbYXwzHoR1WdRKsa8IOdm4XXWand4d31/ans+fGc+f3BnPm5/bGoBm52bBYcjWSnst4VekDLKr44MxltgAVJ1Tohfl9pqfBUOnV+ZLZSfGC4RGB1LAw0fH+EaGy4dHx/hLgOrJLPqERAejBWvPwgwassiE8V8CCssLyouLShECRMtKy4sLicsLyou3n0taekkGTJI9cQRPxDEpG0HUasTGBc0mFaY6RMfHxsbHh2cHx8eQhseHZwfER4unB8UHJwfHx76j7cXTHtyd39wfXs+cXA+anZ3bT59e2x6Kz0LVQtHA62K6eiCgNFOpN9GThnyYyedlU0+zSbar6GEURR14TXiaWkwf25ucnswfXFzMX9ubnJ7fX+pBaONXDoMNNkRA6hTgkB91lWeCW5yez5McXFqPl1fLgAJEy4oLiosLShELnwvFS4XGB1LGhgNHEtNLw3HKGHfmUvHuYenLFzlxstvgGC/TJ4KNc53WYpoF+DqdZMwXrjpWVNhi4BkErpZlUXKCCkt1doRU9AKd8/XB2zrQxDLYUGF7DsdpEuRU0MT77bCYDwr1DvLxxHIdcq8Oj0P6b+yGhgNHEtNLw0uDxgdSxoUDRRfbm4+cXg+anZ7Pmp2e3A+f25ucnd9f25yez5de2xqd3h3fX9qd3FwPl9rZz5/bW1rc3ttPn99fXtuan9wfXsyPn17bGp3eHd9f2p7Pm5xcnd9ZzBeuOlZU2EWQC4BGB1LAz0aBi4IPn9wej59e2xqd3h3fX9qd3FwPm4Bj8UAWU71G/NAZ5oz9Si8SVJL8pUHl8DnVXLrGbU8Lhz2BiDmThfNMS6f3RgWNRgfGxsZHBwun6gEn61sf31qd317Pm1qf2p7c3twam0wLpwfHhgXNJhWmOl9ehsfLp/sLjQYTrSUy8T64s4XGSmua2s/");

	private static int[] order = new int[61]
	{
		21, 24, 48, 52, 24, 13, 52, 17, 55, 13,
		21, 21, 43, 30, 21, 53, 33, 22, 43, 28,
		35, 27, 56, 26, 30, 42, 50, 39, 49, 42,
		42, 50, 56, 57, 47, 39, 38, 39, 51, 51,
		59, 52, 55, 58, 56, 58, 49, 54, 48, 52,
		55, 59, 56, 55, 58, 55, 57, 59, 59, 59,
		60
	};

	private static int key = 30;

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
