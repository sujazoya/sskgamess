using CBGames;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollviewSnap : MonoBehaviour, IEndDragHandler, IEventSystemHandler
{
	[Header("Snapping Config")]
	public float snapTime = 0.3f;

	private ScrollRect scrollRect;

	private RectTransform content;

	private GridLayoutGroup contentGridLayoutGroup;

	private IEnumerator snapCoroutine;

	private void Start()
	{
		scrollRect = GetComponent<ScrollRect>();
		content = scrollRect.content;
		contentGridLayoutGroup = content.GetComponent<GridLayoutGroup>();
		scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
		scrollRect.inertia = false;
		float x = contentGridLayoutGroup.cellSize.x * (float)(content.transform.childCount - 1) + contentGridLayoutGroup.spacing.x * (float)(content.transform.childCount - 1);
		content.anchoredPosition = Vector2.zero;
		content.sizeDelta = new Vector2(x, 0f);
	}

	public void OnBeginDrag(PointerEventData data)
	{
		if (snapCoroutine != null)
		{
			StopCoroutine(snapCoroutine);
			snapCoroutine = null;
		}
	}

	public void OnEndDrag(PointerEventData data)
	{
		float num = 0f - content.localPosition.x;
		float num2 = contentGridLayoutGroup.cellSize.x + contentGridLayoutGroup.spacing.x;
		int num3 = Mathf.RoundToInt(num / num2);
		num3 = ((num3 >= 0) ? ((num3 > content.childCount - 1) ? (content.childCount - 1) : num3) : 0);
		float num4 = Mathf.Round(num3) * num2;
		Vector3 newPos = new Vector3(0f - num4, 0f, 0f);
		snapCoroutine = CRSnap(newPos, snapTime);
		StartCoroutine(snapCoroutine);
	}

	private IEnumerator CRSnap(Vector3 newPos, float duration)
	{
		float timePast = 0f;
		Vector3 startPos = content.localPosition;
		while (timePast < duration)
		{
			timePast += Time.deltaTime;
			float t = timePast / duration;
			content.localPosition = Vector3.Lerp(startPos, newPos, t);
			yield return null;
		}
		ServicesManager.Instance.SoundManager.PlayOneSound(ServicesManager.Instance.SoundManager.tick);
	}
}
