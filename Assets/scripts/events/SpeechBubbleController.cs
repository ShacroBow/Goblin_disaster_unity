using UnityEngine;
using TMPro;

public class SpeechBubbleController : MonoBehaviour
{
    [Header("References (assign these in the prefab)")]
    public SpriteRenderer bigCircle;     // Drag your large circle’s SpriteRenderer here
    public TextMeshPro textMesh;         // Drag your TextMeshPro child here

    [Header("Bobbing Settings")]
    public float bobAmplitude = 0.1f;
    public float bobSpeed = 1f;

    [Header("Despawn")]
    public float despawnDelay = 2f;

    private float initialY;

    void Start()
    {
        // Record starting Y for bobbing
        initialY = transform.position.y;

        // Center-align the text if assigned
        if (textMesh != null)
        {
            textMesh.alignment = TextAlignmentOptions.Center;
            CenterText();
        }
    }

    void Update()
    {
        // Simple sine-wave bobbing
        float newY = initialY + Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    /// <summary>
    /// Call this immediately after instantiation to set or change the bubble’s message.
    /// </summary>
    public void SetText(string msg)
    {
        if (textMesh == null)
        {
            Debug.LogError("SpeechBubbleController: textMesh not assigned!");
            return;
        }

        textMesh.text = msg;
        CenterText();
    }

    private void CenterText()
    {
        // Force TMP to update geometry so we can re-position exactly
        textMesh.ForceMeshUpdate();
        textMesh.transform.localPosition = Vector3.zero;
    }
}
