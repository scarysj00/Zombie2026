using UnityEngine;

// 라이트를 불안정하게 깜박이게 한다.
[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    public float baseIntensity = 2f;        // 아티스트 손잡이
    [Range(0f, 1f)] public float depth = 0.6f;  // 깜박임 깊이 (0 = 안 깜박임)
    public float speed = 8f;            // 깜박임 속도
    public float dropChance = 0.02f;    // 프레임당 완전히 꺼질 확률

    private Light lamp;
    private float seed;

    private void Awake()
    {
        lamp = GetComponent<Light>();
        seed = Random.value * 100f;     // 램프마다 다른 위상
    }
    // Update is called once per frame
    private void Update()
    {
        float noise = Mathf.PerlinNoise(seed, Time.time * speed);  // 0~1, 연속
        float intensity = baseIntensity * Mathf.Lerp(1f - depth, 1f, noise);
        if (Random.value < dropChance) intensity = 0f;
        lamp.intensity = intensity;
    }
}
