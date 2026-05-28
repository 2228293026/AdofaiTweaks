using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using UnityEngine;

namespace ADOFAI.Editor.Models;

public struct SerializedMinMaxGradient
{
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public ParticleSystemGradientMode mode { get; set; }

	[JsonIgnore(/*Could not decode attribute arguments.*/)]
	[CanBeNull]
	public string color1 { get; set; }

	[JsonIgnore(/*Could not decode attribute arguments.*/)]
	[CanBeNull]
	public string color2 { get; set; }

	[JsonIgnore(/*Could not decode attribute arguments.*/)]
	public SerializedGradient? gradient1 { get; set; }

	[JsonIgnore(/*Could not decode attribute arguments.*/)]
	public SerializedGradient? gradient2 { get; set; }

	public static SerializedMinMaxGradient Default()
	{
		return Decode((JsonNode)(object)JsonValue.Create("ffffff", (JsonNodeOptions?)null));
	}

	public static SerializedMinMaxGradient Decode(JsonNode node)
	{
		JsonValue val = (JsonValue)(object)((node is JsonValue) ? node : null);
		if (val != null)
		{
			return new SerializedMinMaxGradient
			{
				mode = ParticleSystemGradientMode.Color,
				color1 = ((JsonNode)val).GetValue<string>()
			};
		}
		JsonObject val2 = node.AsObject();
		if (val2 != null)
		{
			return JsonSerializer.Deserialize<SerializedMinMaxGradient>((JsonNode)(object)val2, (JsonSerializerOptions)null);
		}
		throw new NotImplementedException();
	}

	public ParticleSystem.MinMaxGradient ToMinMaxGradient()
	{
		switch (mode)
		{
		case ParticleSystemGradientMode.Color:
			return new ParticleSystem.MinMaxGradient(color1.HexToColor());
		case ParticleSystemGradientMode.TwoColors:
			return new ParticleSystem.MinMaxGradient(color1.HexToColor(), color2.HexToColor());
		case ParticleSystemGradientMode.RandomColor:
		{
			ParticleSystem.MinMaxGradient result = new ParticleSystem.MinMaxGradient(gradient1.Value.ToGradient());
			result.mode = ParticleSystemGradientMode.RandomColor;
			return result;
		}
		case ParticleSystemGradientMode.Gradient:
			return new ParticleSystem.MinMaxGradient(gradient1.Value.ToGradient());
		case ParticleSystemGradientMode.TwoGradients:
			return new ParticleSystem.MinMaxGradient(gradient1.Value.ToGradient(), gradient2.Value.ToGradient());
		default:
			throw new NotImplementedException();
		}
	}
}
