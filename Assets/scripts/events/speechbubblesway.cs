// SwayOnXAxis.cs
using UnityEngine;

public class SwayOnXAxis : MonoBehaviour
{
    public float swayDistance = 0.1f;
    public float swaySpeed = 1f;
    public float phaseOffset = 0f;

    private float initialX;

    void Start()
    {
        initialX = transform.localPosition.x;
    }

    void Update()
    {
        float x = initialX + Mathf.Sin(Time.time * swaySpeed + phaseOffset) * swayDistance;
        Vector3 p = transform.localPosition;
        p.x = x;
        transform.localPosition = p;
    }
}
