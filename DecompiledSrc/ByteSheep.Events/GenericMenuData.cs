using System;
using UnityEngine.Serialization;

namespace ByteSheep.Events;

[Serializable]
public class GenericMenuData
{
	[FormerlySerializedAs("m_selectedComponent")]
	public int selectedComponent;

	[FormerlySerializedAs("m_selectedMember")]
	public int selectedMember;

	[FormerlySerializedAs("m_isDynamic")]
	public bool isDynamic;
}
