using UnityEngine;

public class FollowTranslationOnly : MonoBehaviour
{
    [Tooltip("Object whose TRANSLATION this GameObject should follow.")]
    public Transform target;           // drag the parent­-to­-follow here

    Vector3 worldOffset;               // initial gap in world space

    void Awake()
    {
        if (target == null)            
            Debug.LogError($"{name}: FollowTranslationOnly needs a Target");

        // world-space gap at start
        worldOffset = transform.position - target.position;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // copy ONLY the target’s translation
        transform.position = target.position + worldOffset;
        // leave rotation & scale unchanged
    }
}
