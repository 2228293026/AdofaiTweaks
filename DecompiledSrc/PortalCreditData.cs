using System.Collections.Generic;
using System.Linq;

public class PortalCreditData
{
	public enum LinkType
	{
		None,
		Soundcloud,
		Youtube,
		Twitter
	}

	public string credit;

	public string people;

	public string link;

	public LinkType linkType;

	private string creditKey;

	private string[] peopleKeys;

	private string linkKey;

	public PortalCreditData(Dictionary<string, object> dict)
	{
		Decode(dict);
	}

	public void Decode(Dictionary<string, object> dict)
	{
		creditKey = (string)dict["credit"];
		peopleKeys = ((List<object>)dict["people"]).OfType<string>().ToArray();
		if (dict.TryGetValueAs<string, object, string>("soundcloud", out linkKey))
		{
			linkType = LinkType.Soundcloud;
		}
		else if (dict.TryGetValueAs<string, object, string>("youtube", out linkKey))
		{
			linkType = LinkType.Youtube;
		}
		else if (dict.TryGetValueAs<string, object, string>("twitter", out linkKey))
		{
			linkType = LinkType.Twitter;
		}
	}

	public void Localize()
	{
		credit = RDString.Get("levelSelect." + creditKey);
		IEnumerable<string> enumerable = peopleKeys.Select((string p) => RDString.Get((p.Contains(".") ? "" : "levelSelect.") + p));
		people = string.Join<string>('\n', enumerable);
		if (linkKey != null)
		{
			link = RDString.Get("levelSelect." + linkKey);
		}
	}
}
