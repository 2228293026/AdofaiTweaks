using System.Collections.Generic;
using Rewired.ControllerExtensions;
using Rewired.Interfaces;
using UnityEngine;

namespace Rewired.Demos;

[AddComponentMenu("")]
public class DualShock4SpecialFeaturesExample : MonoBehaviour
{
	private class Touch
	{
		public GameObject go;

		public int touchId = -1;
	}

	private const int maxTouches = 2;

	public int playerId;

	public Transform touchpadTransform;

	public GameObject lightObject;

	public Transform accelerometerTransform;

	private List<Touch> touches;

	private Queue<Touch> unusedTouches;

	private bool isFlashing;

	private GUIStyle textStyle;

	private Player player => ReInput.players.GetPlayer(playerId);

	private void Awake()
	{
		InitializeTouchObjects();
	}

	private void Update()
	{
		if (!ReInput.isReady)
		{
			return;
		}
		IDualShock4Extension firstDS = GetFirstDS4(player);
		if (firstDS != null)
		{
			base.transform.rotation = firstDS.GetOrientation();
			HandleTouchpad(firstDS);
			Vector3 accelerometerValue = firstDS.GetAccelerometerValue();
			accelerometerTransform.LookAt(accelerometerTransform.position + accelerometerValue);
		}
		if (player.GetButtonDown("CycleLight"))
		{
			SetRandomLightColor();
		}
		if (player.GetButtonDown("ResetOrientation"))
		{
			ResetOrientation();
		}
		if (player.GetButtonDown("ToggleLightFlash"))
		{
			if (isFlashing)
			{
				StopLightFlash();
			}
			else
			{
				StartLightFlash();
			}
			isFlashing = !isFlashing;
		}
		if (player.GetButtonDown("VibrateLeft"))
		{
			((IControllerVibrator)firstDS).SetVibration(0, 1f, 1f);
		}
		if (player.GetButtonDown("VibrateRight"))
		{
			((IControllerVibrator)firstDS).SetVibration(1, 1f, 1f);
		}
	}

	private void OnGUI()
	{
		if (textStyle == null)
		{
			textStyle = new GUIStyle(GUI.skin.label);
			textStyle.fontSize = 20;
			textStyle.wordWrap = true;
		}
		if (GetFirstDS4(player) != null)
		{
			GUILayout.BeginArea(new Rect(200f, 100f, (float)Screen.width - 400f, (float)Screen.height - 200f));
			GUILayout.Label("Rotate the Dual Shock 4 to see the model rotate in sync.", textStyle);
			GUILayout.Label("Touch the touchpad to see them appear on the model.", textStyle);
			ActionElementMap firstElementMapWithAction = player.controllers.maps.GetFirstElementMapWithAction((ControllerType)2, "ResetOrientation", true);
			if (firstElementMapWithAction != null)
			{
				GUILayout.Label("Press " + firstElementMapWithAction.elementIdentifierName + " to reset the orientation. Hold the gamepad facing the screen with sticks pointing up and press the button.", textStyle);
			}
			firstElementMapWithAction = player.controllers.maps.GetFirstElementMapWithAction((ControllerType)2, "CycleLight", true);
			if (firstElementMapWithAction != null)
			{
				GUILayout.Label("Press " + firstElementMapWithAction.elementIdentifierName + " to change the light color.", textStyle);
			}
			firstElementMapWithAction = player.controllers.maps.GetFirstElementMapWithAction((ControllerType)2, "ToggleLightFlash", true);
			if (firstElementMapWithAction != null)
			{
				GUILayout.Label("Press " + firstElementMapWithAction.elementIdentifierName + " to start or stop the light flashing.", textStyle);
			}
			firstElementMapWithAction = player.controllers.maps.GetFirstElementMapWithAction((ControllerType)2, "VibrateLeft", true);
			if (firstElementMapWithAction != null)
			{
				GUILayout.Label("Press " + firstElementMapWithAction.elementIdentifierName + " vibrate the left motor.", textStyle);
			}
			firstElementMapWithAction = player.controllers.maps.GetFirstElementMapWithAction((ControllerType)2, "VibrateRight", true);
			if (firstElementMapWithAction != null)
			{
				GUILayout.Label("Press " + firstElementMapWithAction.elementIdentifierName + " vibrate the right motor.", textStyle);
			}
			GUILayout.EndArea();
		}
	}

	private void ResetOrientation()
	{
		IDualShock4Extension firstDS = GetFirstDS4(player);
		if (firstDS != null)
		{
			firstDS.ResetOrientation();
		}
	}

	private void SetRandomLightColor()
	{
		IDualShock4Extension firstDS = GetFirstDS4(player);
		if (firstDS != null)
		{
			Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
			firstDS.SetLightColor(color);
			lightObject.GetComponent<MeshRenderer>().material.color = color;
		}
	}

	private void StartLightFlash()
	{
		IDualShock4Extension firstDS = GetFirstDS4(player);
		DualShock4Extension val = (DualShock4Extension)(object)((firstDS is DualShock4Extension) ? firstDS : null);
		if (val != null)
		{
			val.SetLightFlash(0.5f, 0.5f);
		}
	}

	private void StopLightFlash()
	{
		IDualShock4Extension firstDS = GetFirstDS4(player);
		DualShock4Extension val = (DualShock4Extension)(object)((firstDS is DualShock4Extension) ? firstDS : null);
		if (val != null)
		{
			val.StopLightFlash();
		}
	}

	private IDualShock4Extension GetFirstDS4(Player player)
	{
		foreach (Joystick joystick in player.controllers.Joysticks)
		{
			IDualShock4Extension extension = ((Controller)joystick).GetExtension<IDualShock4Extension>();
			if (extension != null)
			{
				return extension;
			}
		}
		return null;
	}

	private void InitializeTouchObjects()
	{
		touches = new List<Touch>(2);
		unusedTouches = new Queue<Touch>(2);
		for (int i = 0; i < 2; i++)
		{
			Touch touch = new Touch();
			touch.go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			touch.go.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
			touch.go.transform.SetParent(touchpadTransform, worldPositionStays: true);
			touch.go.GetComponent<MeshRenderer>().material.color = ((i == 0) ? Color.red : Color.green);
			touch.go.SetActive(value: false);
			unusedTouches.Enqueue(touch);
		}
	}

	private void HandleTouchpad(IDualShock4Extension ds4)
	{
		for (int num = touches.Count - 1; num >= 0; num--)
		{
			Touch touch = touches[num];
			if (!ds4.IsTouchingByTouchId(touch.touchId))
			{
				touch.go.SetActive(value: false);
				unusedTouches.Enqueue(touch);
				touches.RemoveAt(num);
			}
		}
		Vector2 vector = default(Vector2);
		for (int i = 0; i < ds4.maxTouches; i++)
		{
			if (ds4.IsTouching(i))
			{
				int touchId = ds4.GetTouchId(i);
				Touch touch2 = touches.Find((Touch x) => x.touchId == touchId);
				if (touch2 == null)
				{
					touch2 = unusedTouches.Dequeue();
					touches.Add(touch2);
				}
				touch2.touchId = touchId;
				touch2.go.SetActive(value: true);
				ds4.GetTouchPosition(i, ref vector);
				touch2.go.transform.localPosition = new Vector3(vector.x - 0.5f, 0.5f + touch2.go.transform.localScale.y * 0.5f, vector.y - 0.5f);
			}
		}
	}
}
