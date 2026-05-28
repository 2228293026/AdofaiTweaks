using System.Linq;
using System.Text.Json.Serialization;
using UnityEngine;

namespace ADOFAI.Editor.Models;

public struct SerializedGradient
{
	public struct ColorKey
	{
		public decimal time { get; set; }

		public string color { get; set; }

		public GradientColorKey ToGradientKey()
		{
			return new GradientColorKey
			{
				color = color.HexToColor(),
				time = (float)time
			};
		}
	}

	public struct AlphaKey
	{
		public decimal time { get; set; }

		public decimal alpha { get; set; }

		public GradientAlphaKey ToGradientKey()
		{
			return new GradientAlphaKey
			{
				alpha = (float)alpha,
				time = (float)time
			};
		}
	}

	[JsonConverter(typeof(JsonStringEnumConverter))]
	public GradientMode mode { get; set; }

	public AlphaKey[] alphaKeys { get; set; }

	public ColorKey[] colorKeys { get; set; }

	public Gradient ToGradient()
	{
		Gradient gradient = new Gradient();
		gradient.mode = mode;
		gradient.SetKeys(colorKeys.Select((ColorKey x) => x.ToGradientKey()).ToArray(), alphaKeys.Select((AlphaKey x) => x.ToGradientKey()).ToArray());
		return gradient;
	}

	public static SerializedGradient FromGradient(Gradient gradient)
	{
		return new SerializedGradient
		{
			mode = gradient.mode,
			alphaKeys = gradient.alphaKeys.Select((GradientAlphaKey v) => new AlphaKey
			{
				time = (decimal)v.time,
				alpha = (decimal)v.alpha
			}).ToArray(),
			colorKeys = gradient.colorKeys.Select((GradientColorKey v) => new ColorKey
			{
				time = (decimal)v.time,
				color = v.color.ToHex(useAlpha: false, hash: false)
			}).ToArray()
		};
	}
}
