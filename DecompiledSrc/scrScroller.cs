using UnityEngine;

public class scrScroller : ADOBase
{
	private float jumpamount;

	public float scrollspeed;

	public float jumpOffset;

	private float curroffset;

	private SpriteRenderer spr;

	public Transform cameraTransform;

	private Vector3 startpos;

	private void Start()
	{
		spr = GetComponent<SpriteRenderer>();
		jumpamount = spr.sprite.bounds.max.x - spr.sprite.bounds.min.x;
		startpos = base.gameObject.transform.position;
		cameraTransform = ADOBase.controller.camy.transform;
		if (jumpOffset != 0f)
		{
			jumpamount = jumpOffset;
		}
	}

	private void Update()
	{
		Vector3 zero = Vector3.zero;
		zero.x = startpos.x + cameraTransform.position.x + curroffset;
		zero.y = startpos.y + cameraTransform.position.y;
		zero.z = startpos.z;
		base.transform.position = zero;
		Vector3 position = base.transform.position;
		position.z = startpos.z;
		base.transform.position = position;
		curroffset += scrollspeed * Time.deltaTime * ADOBase.controller.d_speed;
		if (curroffset < 0f - jumpamount)
		{
			curroffset = 0f;
		}
	}
}
