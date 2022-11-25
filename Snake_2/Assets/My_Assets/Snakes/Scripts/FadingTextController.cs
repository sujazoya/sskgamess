using System.Collections;
using UnityEngine;

public class FadingTextController : MonoBehaviour
{
	[SerializeField]
	private TextMesh textMesh;

	public void SetTextAndMoveUp(string text, float movingUpSpeed, Color textColor)
	{
		textMesh.text = text;
		textMesh.color = textColor;
		StartCoroutine(CRMovingUp(movingUpSpeed));
	}

	private IEnumerator CRMovingUp(float speed)
	{
		Vector3 startPos = base.transform.position;
		Vector3 endPos = startPos + Vector3.up * 5f;
		Color startColor = textMesh.color;
		Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
		float movingTime = Vector3.Distance(startPos, endPos) / speed;
		float t = 0f;
		while (t < movingTime)
		{
			t += Time.deltaTime;
			float t2 = t / movingTime;
			base.transform.position = Vector3.Lerp(startPos, endPos, t2);
			textMesh.color = Color.Lerp(startColor, endColor, t2);
			yield return null;
		}
		textMesh.color = startColor;
		base.gameObject.SetActive(value: false);
	}
}
