using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace ByteSheep.Events;

[Serializable]
public class QuickPersistentCallGroup
{
	[FormerlySerializedAs("m_calls")]
	public List<QuickPersistentCall> calls = new List<QuickPersistentCall>();
}
