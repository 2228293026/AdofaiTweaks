using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI.Editor.ParticleEditor;

public class ParticleEditor : MonoBehaviour
{
	[Header("Containers")]
	public RectTransform tabButtonContainer;

	public RectTransform tabsContainer;

	[Header("Templates")]
	public PropertiesSubTabButton tabButtonTemplate;

	public PropertiesPanel tabTemplate;

	[Header("Texts")]
	public TMP_Text titleText;

	[Header("Preview")]
	public scrParticleDecoration previewDec;

	[Header("Playback")]
	public Button playPauseButton;

	public Button stopButton;

	public Button resetButton;

	public Image playPauseButtonImage;

	public Sprite playImage;

	public Sprite pauseImage;

	[Header("Other")]
	public Button closeButton;

	public InspectorPanel inspectorPanel;

	private Dictionary<ParticleEditorTabType, PropertiesSubTabButton> _tabButtons = new Dictionary<ParticleEditorTabType, PropertiesSubTabButton>();

	public Dictionary<ParticleEditorTabType, PropertiesPanel> Tabs = new Dictionary<ParticleEditorTabType, PropertiesPanel>();

	private bool _initialized;

	public LevelEvent SelectedEvent;

	private void Awake()
	{
		tabButtonTemplate.gameObject.SetActive(value: false);
		tabTemplate.gameObject.SetActive(value: false);
		closeButton.onClick.AddListener(delegate
		{
			scnEditor.instance.HideParticleEditor();
		});
		playPauseButton.onClick.AddListener(delegate
		{
			ParticleSystem particleSystem = previewDec.particleSystem;
			if (particleSystem.isPlaying)
			{
				particleSystem.Pause();
			}
			else
			{
				particleSystem.Play();
			}
		});
		stopButton.onClick.AddListener(delegate
		{
			ParticleSystem particleSystem = previewDec.particleSystem;
			particleSystem.Stop(withChildren: false, ParticleSystemStopBehavior.StopEmittingAndClear);
			particleSystem.time = 0f;
		});
		resetButton.onClick.AddListener(delegate
		{
			ParticleSystem particleSystem = previewDec.particleSystem;
			particleSystem.Clear();
			particleSystem.time = 0f;
		});
	}

	private void DrawSettings()
	{
		if (!_initialized)
		{
			_initialized = true;
			DrawCategory(ParticleEditorTabType.General, delegate(PropertiesPanel tab)
			{
				AddControl(tab, "maxParticles");
				AddControl(tab, "simulationSpace");
				AddControl(tab, "simulationSpeed");
				AddControl(tab, "playDuration");
				AddControl(tab, "loop");
				AddControl(tab, "randomSeed");
			});
			DrawCategory(ParticleEditorTabType.Shape, delegate(PropertiesPanel tab)
			{
				AddControl(tab, "decorationImage");
				AddControl(tab, "randomTextureTiling");
				AddControl(tab, "startRotation");
				AddControl(tab, "color");
				AddControl(tab, "particleLifetime");
				AddControl(tab, "particleSize");
				AddControl(tab, "shapeType");
				AddControl(tab, "shapeRadius");
				AddControl(tab, "emissionRate");
			});
			DrawCategory(ParticleEditorTabType.Transform, delegate(PropertiesPanel tab)
			{
				AddControl(tab, "velocity");
				AddControl(tab, "velocityLimitOverLifetime");
				AddControl(tab, "rotationOverTime");
				AddControl(tab, "colorOverLifetime");
				AddControl(tab, "sizeOverLifetime");
				AddControl(tab, "arc");
				AddControl(tab, "arcMode");
			});
			SelectCategory(ParticleEditorTabType.General);
		}
		static void AddControl(PropertiesPanel tab, string key)
		{
			tab.RenderControl(key, new PropertyInfo(GetPropertyDict(key), GCS.levelEventsInfo["AddParticle"]));
		}
		static Dictionary<string, object> GetPropertyDict(string key)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>(GCS.levelEventsInfo["AddParticle"].propertiesInfo[key].dict);
			if (dictionary.ContainsKey("control"))
			{
				dictionary.Remove("control");
			}
			return dictionary;
		}
	}

	private void DrawCategory(ParticleEditorTabType tab, Action<PropertiesPanel> builder)
	{
		PropertiesSubTabButton propertiesSubTabButton = UnityEngine.Object.Instantiate(tabButtonTemplate, tabButtonContainer);
		Sprite icon = Resources.Load<Sprite>($"LevelEditor/GroupIcons/Particles/{tab}");
		propertiesSubTabButton.SetIcon(icon);
		propertiesSubTabButton.SetSelected(selected: false);
		propertiesSubTabButton.gameObject.SetActive(value: true);
		propertiesSubTabButton.onClick.AddListener(delegate
		{
			SelectCategory(tab);
		});
		_tabButtons.Add(tab, propertiesSubTabButton);
		PropertiesPanel propertiesPanel = UnityEngine.Object.Instantiate(tabTemplate, tabsContainer);
		propertiesPanel.inspectorPanel = inspectorPanel;
		builder(propertiesPanel);
		Tabs.Add(tab, propertiesPanel);
	}

	private void SelectCategory(ParticleEditorTabType tab)
	{
		ParticleEditorTabType particleEditorTabType = default(ParticleEditorTabType);
		PropertiesSubTabButton propertiesSubTabButton = default(PropertiesSubTabButton);
		foreach (KeyValuePair<ParticleEditorTabType, PropertiesSubTabButton> tabButton in _tabButtons)
		{
			tabButton.Deconstruct(ref particleEditorTabType, ref propertiesSubTabButton);
			ParticleEditorTabType particleEditorTabType2 = particleEditorTabType;
			propertiesSubTabButton.SetSelected(particleEditorTabType2 == tab);
		}
		PropertiesPanel propertiesPanel = default(PropertiesPanel);
		foreach (KeyValuePair<ParticleEditorTabType, PropertiesPanel> tab2 in Tabs)
		{
			tab2.Deconstruct(ref particleEditorTabType, ref propertiesPanel);
			ParticleEditorTabType particleEditorTabType3 = particleEditorTabType;
			propertiesPanel.gameObject.SetActive(particleEditorTabType3 == tab);
		}
		titleText.text = RDString.Get("editor.particleEditor.category." + tab);
	}

	public void SetEvent(LevelEvent ev)
	{
		DrawSettings();
		SelectedEvent = ev;
		inspectorPanel.selectedEvent = SelectedEvent;
		previewDec.manager = scrDecorationManager.instance;
		previewDec.decType = DecorationType.Particle;
		previewDec.Setup(SelectedEvent, out var _);
		ParticleEditorTabType particleEditorTabType = default(ParticleEditorTabType);
		PropertiesPanel propertiesPanel = default(PropertiesPanel);
		foreach (KeyValuePair<ParticleEditorTabType, PropertiesPanel> tab in Tabs)
		{
			tab.Deconstruct(ref particleEditorTabType, ref propertiesPanel);
			propertiesPanel.SetProperties(SelectedEvent);
		}
		UpdatePreview(restart: true);
	}

	public void UpdatePreview(bool restart)
	{
		if (restart)
		{
			previewDec.particleSystem.Stop(withChildren: false, ParticleSystemStopBehavior.StopEmittingAndClear);
		}
		previewDec.ResetParticle(SelectedEvent, restart);
		if (restart)
		{
			previewDec.particleSystem.Play();
		}
	}

	private void Update()
	{
		if (RDInput.cancelPress && !scnEditor.instance.userIsEditingAnInputField)
		{
			if (scnEditor.instance.colorPickerPopup.gameObject.activeSelf)
			{
				scnEditor.instance.colorPickerPopup.Hide();
			}
			else if (scnEditor.instance.gradientEditorPopup.gameObject.activeSelf)
			{
				scnEditor.instance.gradientEditorPopup.Hide();
			}
			else
			{
				scnEditor.instance.HideParticleEditor();
			}
		}
		if ((bool)scnEditor.instance && !scnEditor.instance.colorPickerPopup.isActiveAndEnabled && !scnEditor.instance.gradientEditorPopup.isActiveAndEnabled)
		{
			if (RDInput.holdingControl && !RDInput.holdingShift && Input.GetKeyDown(KeyCode.Z))
			{
				scnEditor.instance.Undo();
			}
			if (RDInput.holdingControl && RDInput.holdingShift && Input.GetKeyDown(KeyCode.Z))
			{
				scnEditor.instance.Redo();
			}
		}
		playPauseButtonImage.sprite = (previewDec.particleSystem.isPlaying ? pauseImage : playImage);
	}
}
