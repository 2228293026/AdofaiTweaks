using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ZodiacBackground : MonoBehaviour
{
	public float colorDuration;

	public float transitionDuration;

	public float waveDuration;

	public float animalSpeed;

	public ZodiacBackgroundAnimal zodiacAnimalPrefab;

	public List<Sprite> zodiacAnimalImages;

	public List<Color> animalColors;

	public List<Color> backgroundColors;

	public Image background;

	public List<CanvasGroup> animalContainer;

	public RectTransform canvasRectTransform;

	public static List<ZodiacBackgroundAnimal> animals = new List<ZodiacBackgroundAnimal>();

	private const float animalDirection = 30f;

	private const int firstAnimalToSpawn = 0;

	private int animalToSpawn;

	private const int animalSpawnOrderOffset = 3;

	private Vector2 animalSpawnLocation;

	private float expectedAnimalCount;

	private float ySpawnOffset;

	private float animalDistance;

	private const float animalsPerScreenWidth = 11f;

	private int animalsPerRow;

	private float localKillY;

	private ZodiacBackgroundAnimal previousSpawned;

	private int playerIndex;

	private bool showing;

	public void Show()
	{
		base.gameObject.SetActive(value: true);
		Color color = backgroundColors[0];
		color.a = 0f;
		background.color = color;
		background.DOFade(1f, transitionDuration).SetUpdate(isIndependentUpdate: true);
		playerIndex = 0;
		ySpawnOffset = 0f - canvasRectTransform.rect.height / 3f;
		float num = (0f - canvasRectTransform.rect.height + ySpawnOffset) * Mathf.Tan((float)Math.PI / 6f);
		animalSpawnLocation = new Vector2(0f - canvasRectTransform.rect.width / 2f + num, 0f - canvasRectTransform.rect.height / 2f + ySpawnOffset);
		animalDistance = canvasRectTransform.rect.width / 10f;
		animalsPerRow = Mathf.CeilToInt((canvasRectTransform.rect.width - num) / animalDistance);
		ShowAndHideAnimals(0f);
		showing = true;
	}

	public void Hide()
	{
		showing = false;
		DestroyAnimals();
		background.DOFade(0f, transitionDuration).SetUpdate(isIndependentUpdate: true).OnComplete(delegate
		{
			base.gameObject.SetActive(value: false);
		});
	}

	public void DestroyAnimals()
	{
		foreach (ZodiacBackgroundAnimal animal in animals)
		{
			if (!(animal == null))
			{
				animal.DestroySelf();
			}
		}
		animals.Clear();
	}

	public void SwitchPlayerCount(int index, int direction)
	{
		playerIndex = index;
		background.DOColor(backgroundColors[playerIndex], colorDuration).SetEase(Ease.InOutQuad).SetUpdate(isIndependentUpdate: true);
		foreach (ZodiacBackgroundAnimal animal in animals)
		{
			if (!(animal == null))
			{
				animal.ChangeColor(animalColors[playerIndex], colorDuration);
			}
		}
		ShowAndHideAnimals(colorDuration);
		foreach (ZodiacBackgroundAnimal animal2 in animals)
		{
			if (!(animal2 == null))
			{
				float num = (animal2.transform.localPosition.x + canvasRectTransform.rect.width / 2f) / canvasRectTransform.rect.width;
				float delay = ((direction == 1) ? num : (1f - num)) * waveDuration;
				animal2.Jump(delay);
			}
		}
	}

	private void ShowAndHideAnimals(float duration)
	{
		for (int i = 0; i < animalContainer.Count; i++)
		{
			if (i <= playerIndex)
			{
				animalContainer[i].DOFade(1f, duration).SetUpdate(isIndependentUpdate: true);
			}
			else
			{
				animalContainer[i].DOFade(0f, duration).SetUpdate(isIndependentUpdate: true);
			}
		}
	}

	private void FillScreenWithFreshAnimals()
	{
		DestroyAnimals();
		float num = 0f;
		animalToSpawn = 0;
		float num2 = ySpawnOffset;
		float num3 = canvasRectTransform.rect.height - ySpawnOffset;
		localKillY = 0f - (animalSpawnLocation.y + num * animalDistance * 0.5f / Mathf.Tan((float)Math.PI / 6f));
		while (num2 < num3)
		{
			float num4 = animalSpawnLocation.y + num * animalDistance * 0.5f / Mathf.Tan((float)Math.PI / 6f);
			SpawnRow(animalToSpawn, num4, num == 0f);
			num2 = num4;
			num += 1f;
			animalToSpawn = (animalToSpawn + 3 + zodiacAnimalImages.Count) % zodiacAnimalImages.Count;
		}
		animalToSpawn = (-3 + zodiacAnimalImages.Count) % zodiacAnimalImages.Count;
		expectedAnimalCount = animals.Count;
	}

	private void Update()
	{
		if (!showing)
		{
			return;
		}
		if (previousSpawned == null || (float)animals.Count <= expectedAnimalCount / 2f)
		{
			FillScreenWithFreshAnimals();
		}
		else if (previousSpawned.realPosition.y > animalSpawnLocation.y + animalDistance / 2f / Mathf.Tan((float)Math.PI / 6f))
		{
			int num = Mathf.FloorToInt((previousSpawned.realPosition.y - animalSpawnLocation.y) / (animalDistance / 2f / Mathf.Tan((float)Math.PI / 6f)));
			for (int i = 1; i <= num; i++)
			{
				SpawnRow(animalToSpawn, previousSpawned.realPosition.y - (float)i * (animalDistance / 2f / Mathf.Tan((float)Math.PI / 6f)), i == num);
				animalToSpawn = (animalToSpawn - 3 + zodiacAnimalImages.Count) % zodiacAnimalImages.Count;
			}
		}
	}

	private void SpawnRow(int firstAnimalIndex, float yPosition, bool lowestRow)
	{
		for (int i = 0; i < animalsPerRow; i++)
		{
			int num = (i + firstAnimalIndex) % zodiacAnimalImages.Count;
			ZodiacBackgroundAnimal zodiacBackgroundAnimal = UnityEngine.Object.Instantiate(zodiacAnimalPrefab, animalContainer[num % 4].transform);
			Vector3 localPosition = new Vector3(animalSpawnLocation.x + (float)i * animalDistance, animalSpawnLocation.y, 0f) + new Vector3((yPosition - animalSpawnLocation.y) * Mathf.Tan((float)Math.PI / 6f), yPosition - animalSpawnLocation.y, 0f);
			zodiacBackgroundAnimal.transform.localPosition = localPosition;
			Vector2 moveDirection = new Vector2(Mathf.Sin((float)Math.PI / 6f), Mathf.Cos((float)Math.PI / 6f)) * animalSpeed;
			zodiacBackgroundAnimal.Setup(zodiacAnimalImages[num], moveDirection, localKillY);
			zodiacBackgroundAnimal.ChangeColor(animalColors[playerIndex], 0f);
			animals.Add(zodiacBackgroundAnimal);
			if (i == 0 && lowestRow)
			{
				previousSpawned = zodiacBackgroundAnimal;
			}
		}
		ShowAndHideAnimals(0f);
	}
}
