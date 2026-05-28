using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace ADOFAI.Editor.Components;

public class DraggableNumberInputField : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
{
	public enum Axis
	{
		Vertical,
		Horizontal
	}

	private const float MinDistance = 1f;

	public TMP_Text unitText;

	public TMP_InputField field;

	public float stepPerPixel = 1f;

	public int maxFloatingPoints = 2;

	public GameObject[] arrows;

	public bool clamp;

	public float min;

	public float max;

	public Axis axis;

	public UnityEvent onDrag;

	private bool _down;

	private bool _isDragging;

	private Vector2 _startPos;

	private float _startValue;

	public TMP_InputField.OnChangeEvent onValueChanged => field.onValueChanged;

	public TMP_InputField.SubmitEvent onEndEdit => field.onEndEdit;

	private void Awake()
	{
		SetArrowsVisible(visible: false);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (!field.isFocused && float.TryParse(field.text, out _startValue))
		{
			_startPos = eventData.position;
			_down = true;
			SetArrowsVisible(visible: true);
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		_down = false;
		_isDragging = false;
		SetArrowsVisible(visible: false);
	}

	private void Update()
	{
		if (!_down)
		{
			return;
		}
		Vector2 vector = (Vector2)Input.mousePosition - _startPos;
		bool flag = false;
		float num = 0f;
		switch (axis)
		{
		case Axis.Vertical:
			if (Mathf.Abs(vector.x) > 1f && !_isDragging)
			{
				SetArrowsVisible(visible: false);
			}
			else if (Mathf.Abs(vector.y) > 1f || _isDragging)
			{
				flag = true;
				num = vector.y;
			}
			break;
		case Axis.Horizontal:
			if (Mathf.Abs(vector.y) > 1f && !_isDragging)
			{
				SetArrowsVisible(visible: false);
			}
			else if (Mathf.Abs(vector.x) > 1f || _isDragging)
			{
				flag = true;
				num = vector.x;
			}
			break;
		}
		if (flag)
		{
			field.DeactivateInputField();
			_isDragging = true;
			float value = _startValue + stepPerPixel * num;
			if (clamp)
			{
				value = Mathf.Clamp(value, min, max);
			}
			field.text = value.ToString((maxFloatingPoints > 0) ? ("0." + new string('#', maxFloatingPoints)) : "0");
			onDrag.Invoke();
		}
	}

	private void SetArrowsVisible(bool visible)
	{
		GameObject[] array = arrows;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(visible);
		}
	}
}
