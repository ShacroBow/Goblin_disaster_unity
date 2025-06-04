using UnityEngine;

public class ChangeColorOnTalk : MonoBehaviour
{
	private SpriteRenderer spriteRenderer;
	private Color originalColor;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		originalColor = originalColor;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		var speechTrigger = other.GetComponent<SpeechBubbleTrigger>();
		if (speechTrigger != null && speechTrigger.hasTalked)
		{
			spriteRenderer.color = Color.red;
		}
	}
	
	private void OnTriggerExit2D(Collider2D other)
	{
		var speechTrigger = other.GetComponent<SpeechBubbleTrigger>();
		if (speechTrigger != null)
		{
			spriteRenderer.color = originalColor;
		}
	}
	
}
