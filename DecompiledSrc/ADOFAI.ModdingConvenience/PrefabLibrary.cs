using ADOFAI.ModdingConvenience.DocumentationType;
using UnityEngine;

namespace ADOFAI.ModdingConvenience;

public class PrefabLibrary : MonoBehaviour
{
	private const string DISCLAIMER = "This class is not internally used, and only exists for modding convenience purposes. This class contains the most feature-rich prefabs both inside and outside of the Resources/ directory.";

	private static PrefabLibrary _prefabLibrary;

	public scrConductor scrConductorPrefab;

	public scrController scrControllerPrefab;

	[FieldDocumentation("This had to be a GameObject because the child GameObject contains the PauseMenu instance, not the top parent.")]
	public GameObject pauseMenuPrefab;

	public scrFloor floorMeshLongPrefab;

	public scrFloor scnLevelSelectFloorPrefab;

	[FieldDocumentation("This prefab is meant to be instantiated inside a prefab with a type of scrFloor.")]
	public SpriteRenderer floorIconPrefab;

	[FieldDocumentation("This prefab is meant to be instantiated inside a prefab with a type of scrFloor.")]
	public scrPortalParticles lastTilePortalPrefab;

	public scrCamera scrCameraPrefab;

	public scrPlanet scrPlanetRedPrefab;

	public scrPlanet scrPlanetBluePrefab;

	[FieldDocumentation("Unity false nulls are technically false positives, and we don't need false positives anyway, so we use default null coalesces.")]
	public static PrefabLibrary instance => _prefabLibrary ?? (_prefabLibrary = Resources.Load<GameObject>("PrefabLibrary").GetComponent<PrefabLibrary>());

	[MethodDocumentation("Returns a PauseMenu component from an instantiated pause menu prefab GameObject.", new string[] { "A GameObject instantiated from pauseMenuPrefab." }, "A PauseMenu component nested in a GameObject.", new string[] { "NullReferenceException if RDPauseMenu GameObject's transform is not nested in the top-parent GameObject." })]
	public static PauseMenu GetPauseMenuComponentFromInstantiatedPauseMenuPrefab(GameObject instantiatedPauseMenuPrefab)
	{
		return instantiatedPauseMenuPrefab.transform.Find("RDPauseMenu").gameObject.GetComponent<PauseMenu>();
	}
}
