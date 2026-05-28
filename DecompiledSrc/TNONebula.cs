using UnityEngine;

public class TNONebula : MonoBehaviour
{
	[Header("Properties")]
	public Vector2 backScrollSpeed;

	public Vector2 frontScrollSpeed;

	[Header("References")]
	public SpriteRenderer nebulaBack;

	public SpriteRenderer nebulaMid;

	public SpriteRenderer nebulaFront;

	private Material backMat;

	private Material midMat;

	private Material frontMat;

	private Vector2 currentBackScroll;

	private Vector2 currentFrontScroll;

	private void Awake()
	{
		backMat = nebulaBack.material;
		midMat = nebulaMid.material;
		frontMat = nebulaFront.material;
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		currentBackScroll += backScrollSpeed / 100f * deltaTime;
		currentBackScroll = new Vector2(currentBackScroll.x % 1f, currentBackScroll.y % 1f);
		currentFrontScroll += frontScrollSpeed / 100f * deltaTime;
		currentFrontScroll = new Vector2(currentFrontScroll.x % 1f, currentFrontScroll.y % 1f);
		backMat.SetFloat("_ScrollX", currentBackScroll.x);
		backMat.SetFloat("_ScrollY", currentBackScroll.y);
		midMat.SetFloat("_ScrollX", currentBackScroll.x);
		midMat.SetFloat("_ScrollY", currentBackScroll.y);
		frontMat.SetFloat("_ScrollX", currentFrontScroll.x);
		frontMat.SetFloat("_ScrollY", currentFrontScroll.y);
	}
}
