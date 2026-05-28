using System;
using DG.Tweening;
using UnityEngine;

public class Shatter : MonoBehaviour
{
	public class Piece
	{
		public Vector3 finalPosition;

		public Vector3 startPosition;

		public Vector3 startRotation;

		public Transform transform;

		public Vector3 positionStart;

		public Shatter shatter;

		public float speed;

		public float angleSpeed;

		public float angle;

		public Renderer renderer;

		public void Rewind()
		{
			transform.DOKill();
			transform.position = startPosition;
			transform.eulerAngles = startRotation;
			renderer.material.DOKill();
			renderer.material.color = Color.white;
		}

		public void PlayAnimation()
		{
			transform.DOMove(finalPosition, shatter.animDuration).SetEase(shatter.ease);
			transform.DORotate(Vector3.forward * angle * 57.29578f * shatter.animDuration * shatter.angleSpeed, shatter.animDuration, RotateMode.WorldAxisAdd);
			renderer.material.DOFade(0f, shatter.fadeDuration).SetEase(shatter.colorEase);
		}
	}

	public Transform[] pieceTransforms;

	public Piece[] pieces;

	public float angle0;

	public float angle1;

	public float speed0;

	public float speed1;

	public float angleSpeed;

	public float animDuration;

	public float fadeDuration;

	public Ease ease;

	public Ease colorEase;

	public GameObject[] windowFrame;

	private void Start()
	{
		Setup();
	}

	private void Setup()
	{
		pieces = new Piece[pieceTransforms.Length];
		for (int i = 0; i < pieces.Length; i++)
		{
			Piece piece = new Piece();
			piece.transform = pieceTransforms[i];
			piece.startPosition = piece.transform.position;
			piece.startRotation = piece.transform.eulerAngles;
			float t = Mathf.InverseLerp(40f, -40f, piece.transform.localPosition.z);
			float num = Mathf.LerpAngle(angle0, angle1, t) * ((float)Math.PI / 180f);
			float t2 = 1f - Mathf.InverseLerp(0f, 40f, Mathf.Abs(piece.transform.localPosition.z));
			piece.speed = Mathf.Lerp(speed0, speed1, t2) * UnityEngine.Random.Range(0.8f, 1.2f);
			piece.angle = num;
			piece.finalPosition = new Vector3(piece.startPosition.x + Mathf.Cos(num) * piece.speed * animDuration, piece.startPosition.y + Mathf.Sin(num) * piece.speed * animDuration, piece.startPosition.z);
			piece.renderer = piece.transform.GetComponent<Renderer>();
			piece.shatter = this;
			pieces[i] = piece;
		}
	}

	public void StartShatter()
	{
		for (int i = 0; i < pieces.Length; i++)
		{
			pieces[i].PlayAnimation();
		}
		GameObject[] array = windowFrame;
		for (int j = 0; j < array.Length; j++)
		{
			array[j].gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
		for (int i = 0; i < pieces.Length; i++)
		{
			Piece piece = pieces[i];
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				piece.Rewind();
			}
			if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				piece.PlayAnimation();
			}
		}
	}
}
