using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ADOFAI.Editor.Components.Gradients;

public class GradientEditor : MonoBehaviour
{
	private struct SnapshotData
	{
		public GradientColorKey[] ColorKeys;

		public GradientAlphaKey[] AlphaKeys;

		public int SelectedMarkerIndex;
	}

	public Ease showEase;

	public Ease hideEase;

	public float animDuration;

	public RectTransform arrow;

	public RectTransform panel;

	public RectTransform rectTransform;

	public GradientMarker colorMarkerTemplate;

	public GradientMarker alphaMarkerTemplate;

	public GradientMarkerLine colorMarkerLine;

	public GradientMarkerLine alphaMarkerLine;

	public RectTransform colorMarkerContainer;

	public RectTransform alphaMarkerContainer;

	public GradientGenerator preview;

	public ColorField colorField;

	public TMP_InputField timeField;

	public DraggableNumberInputField alphaField;

	public TweakableDropdown modeField;

	public GameObject colorFieldContainer;

	public GameObject alphaFieldContainer;

	public GameObject inspectorPanel;

	public GameObject inspectorPlaceholder;

	public Gradient value;

	public RDColorPickerPopup colorPickerPopup;

	private readonly List<GradientMarker> _colorMarkers = new List<GradientMarker>();

	private readonly List<GradientMarker> _alphaMarkers = new List<GradientMarker>();

	private readonly List<GradientMarker> _allColorMarkers = new List<GradientMarker>();

	private readonly List<GradientMarker> _allAlphaMarkers = new List<GradientMarker>();

	private GradientMarker _selectedMarker;

	private Vector2? _lastMousePos;

	private GradientColorKey _draggingColorKey;

	private GradientAlphaKey _draggingAlphaKey;

	public Action onChange;

	private bool _initialized;

	private List<SnapshotData> _undoSnapshots = new List<SnapshotData>();

	private List<SnapshotData> _redoSnapshots = new List<SnapshotData>();

	[CanBeNull]
	private GradientMarker draggingMarker { get; set; }

	private bool draggingMarkerIsColorMarker { get; set; }

	private void Awake()
	{
		if (!_initialized)
		{
			_initialized = true;
			base.gameObject.SetActive(value: false);
			colorField.colorPickerPopup = colorPickerPopup;
			rectTransform = (RectTransform)base.transform;
			colorMarkerTemplate.gameObject.SetActive(value: false);
			alphaMarkerTemplate.gameObject.SetActive(value: false);
			colorMarkerLine.onClick.AddListener(AddColorMarker);
			alphaMarkerLine.onClick.AddListener(AddAlphaMarker);
			colorField.onChange.AddListener(OnColorChange);
			alphaField.onValueChanged.AddListener(OnAlphaChange);
			alphaField.onEndEdit.AddListener(OnAlphaEndEdit);
			timeField.onEndEdit.AddListener(OnTimeChange);
			modeField.Setup();
			modeField.enumTypeString = "GradientMode";
			modeField.localizeEnumStrings = true;
			modeField.itemValues.Clear();
			modeField.itemValues.AddRange(Enum.GetNames(typeof(GradientMode)));
			modeField.gameObject.SetActive(value: true);
			modeField.ReloadList();
			TweakableDropdown tweakableDropdown = modeField;
			tweakableDropdown.onValueChanged = (Action<TweakableDropdownItem>)Delegate.Combine(tweakableDropdown.onValueChanged, (Action<TweakableDropdownItem>)delegate(TweakableDropdownItem item)
			{
				GradientMode mode = Enum.Parse<GradientMode>(item.value);
				value.mode = mode;
				preview.UpdateGradient();
			});
		}
	}

	private void Update()
	{
		if (Input.GetMouseButton(0))
		{
			if (_lastMousePos != Input.mousePosition)
			{
				OnDrag(Input.mousePosition);
			}
			_lastMousePos = Input.mousePosition;
		}
		else
		{
			GradientMarker gradientMarker = draggingMarker;
			if ((bool)gradientMarker && !gradientMarker.gameObject.activeSelf)
			{
				_selectedMarker = null;
				UnityEngine.Object.Destroy(gradientMarker.gameObject);
				UpdateInspector();
				_allColorMarkers.Remove(gradientMarker);
				_allAlphaMarkers.Remove(gradientMarker);
			}
			draggingMarker = null;
		}
		if (!colorPickerPopup.isActiveAndEnabled)
		{
			if (RDInput.holdingControl && !RDInput.holdingShift && Input.GetKeyDown(KeyCode.Z))
			{
				Undo();
			}
			if (RDInput.holdingControl && RDInput.holdingShift && Input.GetKeyDown(KeyCode.Z))
			{
				Redo();
			}
		}
	}

	public void Refresh(bool reset = true)
	{
		Awake();
		modeField.SelectItem(modeField.items[(int)value.mode]);
		preview.gradient = value;
		preview.UpdateGradient();
		foreach (GradientMarker alphaMarker in _alphaMarkers)
		{
			UnityEngine.Object.Destroy(alphaMarker.gameObject);
		}
		foreach (GradientMarker colorMarker in _colorMarkers)
		{
			UnityEngine.Object.Destroy(colorMarker.gameObject);
		}
		_alphaMarkers.Clear();
		_colorMarkers.Clear();
		GradientColorKey[] colorKeys = value.colorKeys;
		foreach (GradientColorKey colorKey in colorKeys)
		{
			CreateColorMarker(colorKey);
		}
		GradientAlphaKey[] alphaKeys = value.alphaKeys;
		foreach (GradientAlphaKey alphaKey in alphaKeys)
		{
			CreateAlphaMarker(alphaKey);
		}
		if (reset)
		{
			_undoSnapshots.Clear();
			_redoSnapshots.Clear();
		}
	}

	public void Hide()
	{
		Animate(show: false);
		onChange();
	}

	public void Show(RectTransform anchor)
	{
		_selectedMarker = null;
		rectTransform.localScale = Vector3.zero;
		UpdateAnchor(anchor);
		UpdateInspector();
		base.gameObject.SetActive(value: true);
		Animate(show: true);
		if ((bool)scnEditor.instance)
		{
			GetComponent<Canvas>().sortingOrder = scnEditor.instance.PushPopupBlocker(Hide);
		}
	}

	private void UpdateAnchor(RectTransform anchor)
	{
		Canvas componentInParent = GetComponentInParent<Canvas>();
		Vector3[] array = new Vector3[4];
		anchor.GetWorldCorners(array);
		bool flag = array[0].x < componentInParent.pixelRect.width / 2f;
		if (flag)
		{
			rectTransform.position = array[2];
			rectTransform.anchoredPosition += new Vector2(24f, 0f - anchor.sizeDelta.y / 2f);
		}
		else
		{
			rectTransform.position = array[0];
			rectTransform.anchoredPosition -= new Vector2(12f, 0f - anchor.sizeDelta.y / 2f);
		}
		arrow.ScaleX(flag ? (-1f) : 1f);
		Vector2 anchoredPosition = rectTransform.anchoredPosition;
		float y = ((RectTransform)componentInParent.transform).sizeDelta.y;
		panel.pivot = new Vector2(flag ? 0f : 1f, anchoredPosition.y / y);
	}

	private void Animate(bool show)
	{
		Vector3 localScale = (show ? Vector3.zero : Vector3.one);
		Vector3 endValue = (show ? Vector3.one : Vector3.zero);
		Ease ease = (show ? showEase : hideEase);
		rectTransform.DOKill();
		rectTransform.localScale = localScale;
		rectTransform.DOScale(endValue, animDuration).SetEase(ease).SetUpdate(isIndependentUpdate: true)
			.OnComplete(delegate
			{
				base.gameObject.SetActive(show);
				if (show)
				{
					Refresh();
				}
			});
	}

	private GradientMarker CreateAlphaMarker(GradientAlphaKey alphaKey)
	{
		GradientMarker marker = UnityEngine.Object.Instantiate(alphaMarkerTemplate, alphaMarkerContainer);
		SetMarkerPosition(marker, alphaMarkerContainer, alphaKey.time);
		marker.onClick.AddListener(delegate
		{
			SelectMarker(marker);
		});
		marker.onDelete.AddListener(delegate
		{
			if (_allAlphaMarkers.Count > 2)
			{
				Snapshot();
				int index = _alphaMarkers.IndexOf(marker);
				_alphaMarkers.RemoveAt(index);
				_allAlphaMarkers.Remove(marker);
				List<GradientAlphaKey> list = value.alphaKeys.ToList();
				list.RemoveAt(index);
				value.SetKeys(value.colorKeys, list.ToArray());
				UnityEngine.Object.Destroy(marker.gameObject);
				UpdateInspector();
				preview.UpdateGradient();
			}
		});
		_alphaMarkers.Add(marker);
		_allAlphaMarkers.Add(marker);
		marker.gameObject.SetActive(value: true);
		return marker;
	}

	private GradientMarker CreateColorMarker(GradientColorKey colorKey)
	{
		GradientMarker marker = UnityEngine.Object.Instantiate(colorMarkerTemplate, colorMarkerContainer);
		marker.SetSelected(selected: false);
		marker.onClick.AddListener(delegate
		{
			SelectMarker(marker);
		});
		marker.onDelete.AddListener(delegate
		{
			if (_allColorMarkers.Count > 2)
			{
				Snapshot();
				int index = _colorMarkers.IndexOf(marker);
				_colorMarkers.RemoveAt(index);
				_allColorMarkers.Remove(marker);
				List<GradientColorKey> list = value.colorKeys.ToList();
				list.RemoveAt(index);
				value.SetKeys(list.ToArray(), value.alphaKeys);
				UpdateInspector();
				UnityEngine.Object.Destroy(marker.gameObject);
				preview.UpdateGradient();
			}
		});
		marker.color = colorKey.color;
		SetMarkerPosition(marker, colorMarkerContainer, colorKey.time);
		marker.UpdateColor();
		_colorMarkers.Add(marker);
		_allColorMarkers.Add(marker);
		marker.gameObject.SetActive(value: true);
		return marker;
	}

	private void DragMarker(Vector2 pos, RectTransform container, Action<float> keyUpdater)
	{
		float timeAtPointer = GetTimeAtPointer(pos, container);
		timeField.text = $"{timeAtPointer * 100f}";
		keyUpdater(timeAtPointer);
		preview.UpdateGradient();
	}

	private void SelectMarker(GradientMarker marker, bool snapshot = true)
	{
		foreach (GradientMarker colorMarker in _colorMarkers)
		{
			colorMarker.SetSelected(colorMarker == marker);
		}
		foreach (GradientMarker alphaMarker in _alphaMarkers)
		{
			alphaMarker.SetSelected(alphaMarker == marker);
		}
		_selectedMarker = marker;
		UpdateInspector();
		draggingMarker = marker;
		draggingMarkerIsColorMarker = _colorMarkers.Contains(marker);
		_lastMousePos = Input.mousePosition;
	}

	private void UpdateInspector()
	{
		if (!_selectedMarker)
		{
			inspectorPanel.SetActive(value: false);
			inspectorPlaceholder.SetActive(value: true);
			return;
		}
		inspectorPanel.SetActive(value: true);
		inspectorPlaceholder.SetActive(value: false);
		bool flag = _colorMarkers.Contains(_selectedMarker);
		bool flag2 = _alphaMarkers.Contains(_selectedMarker);
		if (flag || flag2)
		{
			colorFieldContainer.gameObject.SetActive(flag);
			alphaFieldContainer.gameObject.SetActive(!flag);
			if (flag)
			{
				GradientColorKey gradientColorKey = value.colorKeys[_colorMarkers.IndexOf(_selectedMarker)];
				timeField.text = $"{gradientColorKey.time * 100f}";
				colorField.value = gradientColorKey.color.ToHex(useAlpha: false, hash: false);
			}
			else if (flag2)
			{
				GradientAlphaKey gradientAlphaKey = value.alphaKeys[_alphaMarkers.IndexOf(_selectedMarker)];
				timeField.text = $"{gradientAlphaKey.time * 100f}";
				alphaField.field.text = $"{gradientAlphaKey.alpha * 100f}";
			}
		}
	}

	private void OnColorChange(string color)
	{
		Snapshot();
		GradientMarker selectedMarker = _selectedMarker;
		int num = _colorMarkers.IndexOf(selectedMarker);
		if (num >= 0)
		{
			GradientColorKey[] array = new GradientColorKey[value.colorKeys.Length];
			Array.Copy(value.colorKeys, array, value.colorKeys.Length);
			GradientColorKey gradientColorKey = array[num];
			gradientColorKey.color = color.HexToColor();
			array[num] = gradientColorKey;
			value.SetKeys(array, value.alphaKeys);
			selectedMarker.color = gradientColorKey.color;
			selectedMarker.UpdateColor();
			preview.UpdateGradient();
		}
	}

	private void OnAlphaChange(string alpha)
	{
		GradientMarker selectedMarker = _selectedMarker;
		int num = _alphaMarkers.IndexOf(selectedMarker);
		if (num >= 0)
		{
			GradientAlphaKey[] array = new GradientAlphaKey[value.alphaKeys.Length];
			Array.Copy(value.alphaKeys, array, value.alphaKeys.Length);
			GradientAlphaKey gradientAlphaKey = array[num];
			if (float.TryParse(alpha, out var result))
			{
				result = Mathf.Clamp(result, 0f, 100f);
				gradientAlphaKey.alpha = result / 100f;
				array[num] = gradientAlphaKey;
				value.SetKeys(value.colorKeys, array);
				preview.UpdateGradient();
			}
		}
	}

	private void OnAlphaEndEdit(string rawAlpha)
	{
		Snapshot();
		if (!float.TryParse(rawAlpha, out var result))
		{
			alphaField.field.text = $"{value.alphaKeys[_alphaMarkers.IndexOf(_selectedMarker)].alpha * 100f}";
		}
		else
		{
			alphaField.field.text = $"{Mathf.Clamp(result, 0f, 100f)}";
		}
	}

	private void OnTimeChange(string timeStr)
	{
		Snapshot();
		GradientMarker selectedMarker = _selectedMarker;
		if (_colorMarkers.Contains(selectedMarker))
		{
			int num = _colorMarkers.IndexOf(selectedMarker);
			GradientColorKey[] array = new GradientColorKey[value.colorKeys.Length];
			Array.Copy(value.colorKeys, array, value.colorKeys.Length);
			GradientColorKey gradientColorKey = array[num];
			if (!float.TryParse(timeStr, out var result) || result > 100f || result < 0f)
			{
				timeField.text = $"{gradientColorKey.time * 100f}";
				return;
			}
			gradientColorKey.time = result / 100f;
			array[num] = gradientColorKey;
			value.SetKeys(array, value.alphaKeys);
			SetMarkerPosition(selectedMarker, colorMarkerContainer, gradientColorKey.time);
		}
		else
		{
			int num2 = _alphaMarkers.IndexOf(selectedMarker);
			GradientAlphaKey[] array2 = new GradientAlphaKey[value.alphaKeys.Length];
			Array.Copy(value.alphaKeys, array2, value.alphaKeys.Length);
			GradientAlphaKey gradientAlphaKey = array2[num2];
			if (!float.TryParse(timeStr, out var result2))
			{
				timeField.text = $"{gradientAlphaKey.time * 100f}";
				return;
			}
			gradientAlphaKey.time = result2 / 100f;
			array2[num2] = gradientAlphaKey;
			value.SetKeys(value.colorKeys, array2);
			SetMarkerPosition(selectedMarker, alphaMarkerContainer, gradientAlphaKey.time);
		}
		preview.UpdateGradient();
	}

	private void SetMarkerPosition(GradientMarker marker, RectTransform container, float time)
	{
		float x = container.sizeDelta.x;
		float x2 = time * x;
		RectTransform obj = (RectTransform)marker.transform;
		obj.anchoredPosition = obj.anchoredPosition.WithX(x2);
	}

	private float GetTimeAtPointer(Vector2 pos, RectTransform container)
	{
		Vector3[] array = new Vector3[4];
		container.GetWorldCorners(array);
		float x = array[0].x;
		float x2 = array[2].x;
		return (Mathf.Clamp(pos.x, x, x2) - x) / (x2 - x);
	}

	private void AddColorMarker(PointerEventData ev)
	{
		Snapshot();
		float timeAtPointer = GetTimeAtPointer(ev.position, colorMarkerContainer);
		GradientColorKey[] array = new GradientColorKey[value.colorKeys.Length + 1];
		Array.Copy(value.colorKeys, array, value.colorKeys.Length);
		GradientColorKey gradientColorKey = new GradientColorKey
		{
			color = value.Evaluate(timeAtPointer),
			time = timeAtPointer
		};
		array[value.colorKeys.Length] = gradientColorKey;
		value.SetKeys(array, value.alphaKeys);
		GradientMarker marker = CreateColorMarker(gradientColorKey);
		_colorMarkers.Sort((GradientMarker a, GradientMarker b) => a.transform.position.x.CompareTo(b.transform.position.x));
		SelectMarker(marker);
	}

	private void AddAlphaMarker(PointerEventData ev)
	{
		Snapshot();
		float timeAtPointer = GetTimeAtPointer(ev.position, alphaMarkerContainer);
		GradientAlphaKey[] array = new GradientAlphaKey[value.alphaKeys.Length + 1];
		Array.Copy(value.alphaKeys, array, value.alphaKeys.Length);
		GradientAlphaKey gradientAlphaKey = new GradientAlphaKey
		{
			alpha = value.Evaluate(timeAtPointer).a,
			time = timeAtPointer
		};
		array[value.alphaKeys.Length] = gradientAlphaKey;
		value.SetKeys(value.colorKeys, array);
		GradientMarker marker = CreateAlphaMarker(gradientAlphaKey);
		_alphaMarkers.Sort((GradientMarker a, GradientMarker b) => a.transform.position.x.CompareTo(b.transform.position.x));
		SelectMarker(marker);
	}

	private void OnDrag(Vector2 pos)
	{
		GradientMarker marker = draggingMarker;
		if (!marker)
		{
			return;
		}
		if (draggingMarkerIsColorMarker)
		{
			if (_allColorMarkers.Count > 2 && IsDeleteOp(pos, colorMarkerContainer))
			{
				if (marker.gameObject.activeSelf)
				{
					marker.gameObject.SetActive(value: false);
					int index = _colorMarkers.IndexOf(marker);
					_colorMarkers.RemoveAt(index);
					List<GradientColorKey> list = value.colorKeys.ToList();
					_draggingColorKey = list[index];
					list.RemoveAt(index);
					value.SetKeys(list.ToArray(), value.alphaKeys);
					UpdateInspector();
					preview.UpdateGradient();
				}
				return;
			}
			if (!marker.gameObject.activeSelf)
			{
				_colorMarkers.Add(marker);
				_allColorMarkers.Add(marker);
				_colorMarkers.Sort((GradientMarker a, GradientMarker b) => a.transform.position.x.CompareTo(b.transform.position.x));
				List<GradientColorKey> list2 = value.colorKeys.ToList();
				list2.Add(_draggingColorKey);
				list2.Sort((GradientColorKey a, GradientColorKey b) => a.time.CompareTo(b.time));
				value.SetKeys(list2.ToArray(), value.alphaKeys);
				SelectMarker(marker);
				preview.UpdateGradient();
				marker.gameObject.SetActive(value: true);
			}
			DragMarker(pos, colorMarkerContainer, delegate(float time)
			{
				int num = _colorMarkers.IndexOf(marker);
				GradientColorKey[] array = new GradientColorKey[value.colorKeys.Length];
				Array.Copy(value.colorKeys, array, value.colorKeys.Length);
				GradientColorKey gradientColorKey = array[num];
				gradientColorKey.time = time;
				array[num] = gradientColorKey;
				Array.Sort(array, (GradientColorKey a, GradientColorKey b) => a.time.CompareTo(b.time));
				SetMarkerPosition(marker, colorMarkerContainer, time);
				_colorMarkers.Sort((GradientMarker a, GradientMarker b) => a.transform.position.x.CompareTo(b.transform.position.x));
				value.SetKeys(array, value.alphaKeys);
			});
			return;
		}
		if (_allAlphaMarkers.Count > 2 && IsDeleteOp(pos, alphaMarkerContainer))
		{
			if (marker.gameObject.activeSelf)
			{
				marker.gameObject.SetActive(value: false);
				int index2 = _alphaMarkers.IndexOf(marker);
				_alphaMarkers.RemoveAt(index2);
				List<GradientAlphaKey> list3 = value.alphaKeys.ToList();
				_draggingAlphaKey = list3[index2];
				list3.RemoveAt(index2);
				value.SetKeys(value.colorKeys, list3.ToArray());
				UpdateInspector();
				preview.UpdateGradient();
			}
			return;
		}
		if (!marker.gameObject.activeSelf)
		{
			_alphaMarkers.Add(marker);
			_alphaMarkers.Sort((GradientMarker a, GradientMarker b) => a.transform.position.x.CompareTo(b.transform.position.x));
			_allAlphaMarkers.Add(marker);
			List<GradientAlphaKey> list4 = value.alphaKeys.ToList();
			list4.Add(_draggingAlphaKey);
			list4.Sort((GradientAlphaKey a, GradientAlphaKey b) => a.time.CompareTo(b.time));
			value.SetKeys(value.colorKeys, list4.ToArray());
			SelectMarker(marker);
			preview.UpdateGradient();
			marker.gameObject.SetActive(value: true);
		}
		DragMarker(pos, colorMarkerContainer, delegate(float time)
		{
			int num = _alphaMarkers.IndexOf(marker);
			GradientAlphaKey[] array = new GradientAlphaKey[value.alphaKeys.Length];
			Array.Copy(value.alphaKeys, array, value.alphaKeys.Length);
			GradientAlphaKey gradientAlphaKey = array[num];
			gradientAlphaKey.time = time;
			array[num] = gradientAlphaKey;
			Array.Sort(array, (GradientAlphaKey a, GradientAlphaKey b) => a.time.CompareTo(b.time));
			SetMarkerPosition(marker, alphaMarkerContainer, time);
			_alphaMarkers.Sort((GradientMarker a, GradientMarker b) => a.transform.position.x.CompareTo(b.transform.position.x));
			value.SetKeys(value.colorKeys, array);
		});
	}

	private bool IsDeleteOp(Vector2 pos, RectTransform container)
	{
		Vector3[] array = new Vector3[4];
		container.GetWorldCorners(array);
		float num = array[0].y - 10f;
		float num2 = array[1].y + 10f;
		float y = pos.y;
		if (y > num)
		{
			return !(y < num2);
		}
		return true;
	}

	private SnapshotData GetCurrentSnapshot()
	{
		GradientColorKey[] array = new GradientColorKey[value.colorKeys.Length];
		GradientAlphaKey[] array2 = new GradientAlphaKey[value.alphaKeys.Length];
		value.colorKeys.CopyTo(array, 0);
		value.alphaKeys.CopyTo(array2, 0);
		return new SnapshotData
		{
			AlphaKeys = array2,
			ColorKeys = array,
			SelectedMarkerIndex = GetAllMarkers().IndexOf(_selectedMarker)
		};
	}

	private void Snapshot(bool clearRedo = true)
	{
		GradientColorKey[] array = new GradientColorKey[value.colorKeys.Length];
		GradientAlphaKey[] array2 = new GradientAlphaKey[value.alphaKeys.Length];
		value.colorKeys.CopyTo(array, 0);
		value.alphaKeys.CopyTo(array2, 0);
		if (clearRedo)
		{
			_redoSnapshots.Clear();
		}
		_undoSnapshots.Add(GetCurrentSnapshot());
		if (_undoSnapshots.Count > 100)
		{
			_undoSnapshots.RemoveAt(0);
		}
	}

	private void Undo()
	{
		SnapshotData snapshotData = _undoSnapshots.LastOrDefault();
		if (!snapshotData.Equals(default(SnapshotData)))
		{
			Snapshot(clearRedo: false);
			_redoSnapshots.Add(_undoSnapshots.Pop());
			_undoSnapshots.Pop();
			value.SetKeys(snapshotData.ColorKeys, snapshotData.AlphaKeys);
			Refresh(reset: false);
			List<GradientMarker> allMarkers = GetAllMarkers();
			int selectedMarkerIndex = snapshotData.SelectedMarkerIndex;
			if (selectedMarkerIndex >= 0)
			{
				SelectMarker(allMarkers[selectedMarkerIndex], snapshot: false);
			}
			else
			{
				_selectedMarker = null;
			}
			UpdateInspector();
		}
	}

	private void Redo()
	{
		SnapshotData snapshotData = _redoSnapshots.LastOrDefault();
		if (!snapshotData.Equals(default(SnapshotData)))
		{
			Snapshot(clearRedo: false);
			_redoSnapshots.Pop();
			value.SetKeys(snapshotData.ColorKeys, snapshotData.AlphaKeys);
			Refresh(reset: false);
			List<GradientMarker> allMarkers = GetAllMarkers();
			int selectedMarkerIndex = snapshotData.SelectedMarkerIndex;
			if (selectedMarkerIndex >= 0)
			{
				SelectMarker(allMarkers[selectedMarkerIndex], snapshot: false);
			}
			else
			{
				_selectedMarker = null;
			}
			UpdateInspector();
		}
	}

	private List<GradientMarker> GetAllMarkers()
	{
		List<GradientMarker> list = new List<GradientMarker>();
		list.AddRange(_alphaMarkers);
		list.AddRange(_colorMarkers);
		return list;
	}
}
