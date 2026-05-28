using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace ByteSheep.Events;

[Serializable]
public class AdvancedPersistentCallGroup
{
	[FormerlySerializedAs("m_calls")]
	public List<AdvancedPersistentCall> calls = new List<AdvancedPersistentCall>();
}
