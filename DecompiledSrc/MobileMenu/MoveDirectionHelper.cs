using UnityEngine;

namespace MobileMenu;

public static class MoveDirectionHelper
{
	public static MoveDirection Invert(this MoveDirection dir)
	{
		return 3 - dir;
	}

	public static Vector2Int GetVector(this MoveDirection dir)
	{
		return new Vector2Int(dir switch
		{
			MoveDirection.Left => -1, 
			MoveDirection.Right => 1, 
			_ => 0, 
		}, dir switch
		{
			MoveDirection.Down => -1, 
			MoveDirection.Up => 1, 
			_ => 0, 
		});
	}
}
