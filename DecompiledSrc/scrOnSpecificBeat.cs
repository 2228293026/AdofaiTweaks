using System;
using UnityEngine;

public class scrOnSpecificBeat : MonoBehaviour
{
	private float timetotrigger;

	private scrConductor conductor;

	private Action onComplete;

	private void Start()
	{
		conductor = scrConductor.instance;
	}

	private void Update()
	{
	}
}
