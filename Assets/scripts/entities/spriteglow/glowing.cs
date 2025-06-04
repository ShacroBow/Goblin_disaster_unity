using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteGlowLight2D : MonoBehaviour
{
    public Color colorA = Color.red;
    public Color colorB = Color.blue;
    public float colorSpeed = 1f;
    public float minPulse = 0.5f;
    public float maxPulse = 1.5f;
    public float pulseSpeed = 1f;
    public float emissionIntensity = 4f;
    public float lightBaseIntensity = 1f;
    public float innerRadius = 0.5f;
    public float outerRadius = 1.5f;

    SpriteRenderer sr;
    Material mat;
    Light2D l2d;
    float t;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        mat = new Material(sr.sharedMaterial);
        mat.EnableKeyword("_EMISSION");
        sr.sharedMaterial = mat;

        l2d = gameObject.AddComponent<Light2D>();
        l2d.lightType = Light2D.LightType.Point;
    }

    void Update()
    {
        t += Time.deltaTime * colorSpeed;
        float lerp = Mathf.PingPong(t, 1f);
        Color c = Color.Lerp(colorA, colorB, lerp);

        float pulse = Mathf.Lerp(minPulse, maxPulse, (Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f));

        mat.SetColor("_EmissionColor", c * (emissionIntensity * pulse));
        sr.color = c;

        l2d.color = c;
        l2d.intensity = lightBaseIntensity * pulse;
        l2d.pointLightInnerRadius = innerRadius;
        l2d.pointLightOuterRadius = outerRadius;
    }

    public void SetPulseRange(float min, float max) { minPulse = min; maxPulse = max; }
    public void SetColorRange(Color a, Color b) { colorA = a; colorB = b; t = 0f; }
}
