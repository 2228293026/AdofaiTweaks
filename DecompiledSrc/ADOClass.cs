using System.Collections.Generic;
using RDTools;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ADOClass : RDClassDll
{
	public static Scene[] levelScenes;

	public static readonly Color ClearWhite = new Color(1f, 1f, 1f, 0f);

	public AudioManager audioManager => AudioManager.Instance;

	public scrConductor conductor => scrConductor.instance;

	public scrController controller => scrController.instance;

	public scrLevelMaker lm => scrLevelMaker.instance;

	public scrUIController uiController => scrUIController.instance;

	public scnCLS cls => scnCLS.instance;

	public scnEditor editor => scnEditor.instance;

	public scnGame customLevel => scnGame.instance;

	public string levelPath => scnGame.instance.levelPath;

	public RDConstants gc => RDConstants.data;

	public Dictionary<string, GCNS.WorldData> worldData => GCNS.worldData;

	public scnLevelSelect levelSelect => scnLevelSelect.instance;

	public scrDecorationManager decorationManager => scrDecorationManager.instance;

	public bool isLevelEditor => editor != null;

	public bool isEditingLevel => isLevelEditor;

	public bool isUnityEditor => Application.isEditor;

	public bool isCLS => cls != null;

	public static string currentLevel => scrController.instance.levelName;
}
