namespace ByteSheep.Events;

public class DynamicArguments
{
	private object[] oneArgument = new object[1];

	private object[] twoArguments = new object[2];

	private object[] threeArguments = new object[3];

	private object[] fourArguments = new object[4];

	private int argumentCount = 1;

	public object[] UpdateDynamicArguments(object arg0, object arg1 = null, object arg2 = null, object arg3 = null)
	{
		argumentCount = 1;
		if (arg1 != null)
		{
			argumentCount++;
		}
		if (arg2 != null)
		{
			argumentCount++;
		}
		if (arg3 != null)
		{
			argumentCount++;
		}
		switch (argumentCount)
		{
		case 1:
			oneArgument[0] = arg0;
			return oneArgument;
		case 2:
			twoArguments[0] = arg0;
			twoArguments[1] = arg1;
			return twoArguments;
		case 3:
			threeArguments[0] = arg0;
			threeArguments[1] = arg1;
			threeArguments[2] = arg2;
			return threeArguments;
		case 4:
			fourArguments[0] = arg0;
			fourArguments[1] = arg1;
			fourArguments[2] = arg2;
			fourArguments[3] = arg3;
			return fourArguments;
		default:
			return null;
		}
	}
}
