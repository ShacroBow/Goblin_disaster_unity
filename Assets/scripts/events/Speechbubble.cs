using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class SpeechBubbleTrigger : MonoBehaviour
{
    public GameObject bubblePrefab;
    public Transform bubbleAnchor;
    public Vector3 bubbleOffset = Vector3.up * 1.5f;
    public float bubbleScale = 1f;
    public string fullMessage;
    public int charsPerPage = 100;
    public float textFontSize = 5f;
    public bool hasTalked = false;

    private GameObject spawnedBubble;
    private SpeechBubbleController controller;
    private TextMeshPro tmpComponent;
    private List<string> pages;
    private int currentPage = 0;
    private bool playerInside = false;

    private void Update()
    {
        if (playerInside && spawnedBubble != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (pages == null || pages.Count == 0)
                return;

            if (currentPage < pages.Count - 1)
            {
                currentPage++;
                controller.SetText(pages[currentPage]);
                if (tmpComponent != null)
                {
                    tmpComponent.ForceMeshUpdate();
                    tmpComponent.transform.localPosition = Vector3.zero;
                }
                if (currentPage == pages.Count - 1)
                    hasTalked = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || spawnedBubble != null)
            return;

        playerInside = true;

        if (bubblePrefab == null)
            return;

        Vector3 basePos = bubbleAnchor != null ? bubbleAnchor.position : transform.position;
        Vector3 spawnPos = basePos + bubbleOffset;
        spawnedBubble = Instantiate(bubblePrefab, spawnPos, Quaternion.identity);
        spawnedBubble.transform.localScale = Vector3.one * bubbleScale;

        controller = spawnedBubble.GetComponent<SpeechBubbleController>();
        tmpComponent = spawnedBubble.GetComponentInChildren<TextMeshPro>();

        if (tmpComponent != null)
        {
            tmpComponent.fontSize = textFontSize;
            tmpComponent.alignment = TextAlignmentOptions.Center;
            tmpComponent.ForceMeshUpdate();
            RectTransform rt = tmpComponent.rectTransform;
            rt.sizeDelta = new Vector2(1f, 1f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            tmpComponent.transform.localPosition = Vector3.zero;
        }

        pages = new List<string>();
        if (!string.IsNullOrEmpty(fullMessage) && charsPerPage > 0)
        {
            for (int i = 0; i < fullMessage.Length; i += charsPerPage)
            {
                int length = Mathf.Min(charsPerPage, fullMessage.Length - i);
                pages.Add(fullMessage.Substring(i, length));
            }
        }
        else
        {
            pages.Add("");
        }

        currentPage = 0;
        if (controller != null && pages.Count > 0)
            controller.SetText(pages[currentPage]);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || spawnedBubble == null)
            return;

        playerInside = false;
        float delay = (controller != null) ? controller.despawnDelay : 0f;
        Destroy(spawnedBubble, delay);
        spawnedBubble = null;
        controller = null;
        tmpComponent = null;
        pages = null;
        currentPage = 0;
    }
}
