// SpeechBubbleTrigger.cs
using UnityEngine;
using TMPro;

public class SpeechBubbleTrigger : MonoBehaviour
{
    [Header("Prefab & Anchor (assign bubblePrefab)")]
    public GameObject bubblePrefab;       // Drag your bubble prefab here
    public Transform bubbleAnchor;        // (optional) where to spawn relative to

    [Header("Offsets")]
    public Vector3 bubbleOffset = Vector3.up * 1.5f;

    [Header("Scale Overrides (per-trigger)")]
    public float bubbleScale = 1f;        // Uniform scale for the entire bubble

    [Header("Text & Style")]
    public string message;
    public float textFontSize = 5f;       // TMP font size inside the bubble

    private GameObject spawnedBubble;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || spawnedBubble != null)
            return;

        if (bubblePrefab == null)
        {
            Debug.LogError("SpeechBubbleTrigger: bubblePrefab is not assigned!");
            return;
        }

        // 1. Compute spawn position
        Vector3 basePos = (bubbleAnchor != null) ? bubbleAnchor.position : transform.position;
        Vector3 spawnPos = basePos + bubbleOffset;

        // 2. Instantiate the bubble prefab
        spawnedBubble = Instantiate(bubblePrefab, spawnPos, Quaternion.identity);

        // 3. Apply a uniform scale to the whole bubble (so it never depends on text size)
        spawnedBubble.transform.localScale = Vector3.one * bubbleScale;

        // 4. Set the bubble text via its controller
        var controller = spawnedBubble.GetComponent<SpeechBubbleController>();
        if (controller == null)
        {
            Debug.LogError("SpeechBubbleTrigger: Spawned prefab is missing SpeechBubbleController!");
        }
        else
        {
            controller.SetText(message);
        }

        // 5. Configure the TextMeshPro child: font size, centered, and force RectTransform to 1×1
        var tmp = spawnedBubble.GetComponentInChildren<TextMeshPro>();
        if (tmp == null)
        {
            Debug.LogError("SpeechBubbleTrigger: Cannot find TextMeshPro in spawned bubble!");
        }
        else
        {
            // Set font size
            tmp.fontSize = textFontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.ForceMeshUpdate();

            // Force RectTransform to be exactly 1×1 units
            RectTransform rt = tmp.rectTransform;
            rt.sizeDelta = new Vector2(1f, 1f);
            rt.pivot     = new Vector2(0.5f, 0.5f);
            tmp.transform.localPosition = Vector3.zero;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || spawnedBubble == null)
            return;

        // Delay-destroy using the controller’s despawnDelay (if present)
        var controller = spawnedBubble.GetComponent<SpeechBubbleController>();
        float delay = (controller != null) ? controller.despawnDelay : 0f;
        Destroy(spawnedBubble, delay);
        spawnedBubble = null;
    }
}
