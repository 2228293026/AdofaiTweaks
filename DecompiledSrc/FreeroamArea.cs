using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class FreeroamArea : IEnumerable
{
	private class FreeroamAreaEnumerator : IEnumerator
	{
		private FreeroamArea instance;

		private int position = -1;

		public object Current => instance.listFloors[position];

		public FreeroamAreaEnumerator(FreeroamArea inst)
		{
			instance = inst;
		}

		public bool MoveNext()
		{
			position++;
			return position < instance.listFloors.Count;
		}

		public void Reset()
		{
			position = 0;
		}
	}

	public List<scrFloor> listFloors = new List<scrFloor>();

	public scrFloor parentFloor;

	public scrFloor this[int index]
	{
		get
		{
			return listFloors[index];
		}
		set
		{
			listFloors[index] = value;
		}
	}

	public int Count => listFloors.Count;

	public FreeroamArea(scrFloor parentFloor)
	{
		this.parentFloor = parentFloor;
		parentFloor.freeroamArea = this;
	}

	public void Add(scrFloor floor)
	{
		listFloors.Add(floor);
	}

	public void Clear()
	{
		listFloors.Clear();
	}

	public IEnumerator GetEnumerator()
	{
		return new FreeroamAreaEnumerator(this);
	}
}
