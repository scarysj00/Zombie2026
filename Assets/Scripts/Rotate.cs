using UnityEngine;

public class Rotate : MonoBehaviour
{
    // 게임 오브젝트를 지속적으로 회전하는 스크립트
    public float rotationSpeed = 60f;

    void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}
