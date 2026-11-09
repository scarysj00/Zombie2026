using UnityEngine;
using UnityEngine.UI;  // UI 관련 코드

// 플레이어 캐릭터의 생명체로서의 동작을 담당
public class PlayerHealth : LivingEntity
{
    public Slider healthSlider; // 체력을 표시할 UI 슬라이더

    public AudioClip deathClip; // 사망 소리
    public AudioClip hitClip; // 피격 소리
    public AudioClip itemPickupClip; // 아이템 습득 소리

    private AudioSource playerAudioPlayer; // 플레이어 소리 재생기
    private Animator playerAnimator; // 플레이어의 애니메이터

    private PlayerMovement playerMovement;  // 플레이어의 움직임 컴포넌트
    private PlayerShooter playerShooter;  // 플레이어 슈터 컴포넌트

    [Header("Damage Vignette")]
    public Material damageVignette;         // M_DamageVignette 할당
    public float vignettePeak = 0.8f;       // 아티스트 손잡이
    public float vignetteDecay = 3f;        // 초당 감소량
    private static readonly int IntensityId = Shader.PropertyToID("_Intensity");
    private float vignetteValue;

    private void Awake()
    {
        // 사용할 컴포넌트 가져오기
        playerAnimator = GetComponent<Animator>();
        playerAudioPlayer = GetComponent<AudioSource>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShooter = GetComponent<PlayerShooter>();
    }

    private void Update()
    {
        if (vignetteValue <= 0f || damageVignette == null) return;  // 칸이 비어있어도 게임은 계속
        vignetteValue = Mathf.Max(0f, vignetteValue - vignetteDecay * Time.deltaTime);
        damageVignette.SetFloat(IntensityId, vignetteValue);
    }

    protected override void OnEnable()
    {
        // LivingEntity의 OnEnable() 실행 (상태 초기화)
        base.OnEnable();

        // 체력 슬라이드 활성화
        healthSlider.gameObject.SetActive(true);
        // 체력 슬라이더의 최대값을 기본 체력값으로 변경
        healthSlider.maxValue = startingHealth;
        // 체력 슬라이더의 값을 현재 체력값으로 변경
        healthSlider.value = health;

        // 플레이어 조작을 받는 컴포넌트 활성화
        playerMovement.enabled = true;
        playerShooter.enabled = true;
    }

    private void OnDisable() // 에디터에서 머티리얼 에셋 값이 남지 않게
    {
        if (damageVignette != null) damageVignette.SetFloat(IntensityId, 0f);
    }

    // 체력 회복
    public override void RestoreHealth(float newHealth)
    {
        // LivigEntity의 RestoreHealth() 실행 (체력 증가)
        base.RestoreHealth(newHealth);
        // 갱신된 체력으로 체력 슬라이더 갱신
        healthSlider.value = health;
    }

    // 데미지 처리
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (!dead)
        {
            // 사망하지 않은 경우에만 효과음 재생
            playerAudioPlayer.PlayOneShot(hitClip);
        }

        // LivingEntity의 OnDamage() 실행(데미지 적용)
        base.OnDamage(damage, hitPoint, hitDirection);
        vignetteValue = vignettePeak;
        // 갱신된 체력을 체력 슬라이더에 반영
        healthSlider.value = health;
    }

    // 사망 처리
    public override void Die()
    {
        // LivingEntity의 Die() 실행 (사망 적용)
        base.Die();

        // 체력 슬라이더 비활성화
        healthSlider.gameObject.SetActive(false);
        // 사망음 재생
        playerAudioPlayer.PlayOneShot(deathClip);
        // 애니메이터의 Die 트리거를 발동시켜 사망 애니메이션 재생
        playerAnimator.SetTrigger("Die");

        // 플레이어ㅓ 조작을 받는 컴포넌트 비활성화
        playerMovement.enabled = false;
        playerShooter.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 아이템과 충돌한 경우 해당 아이템을 사용하는 처리
        // 사망하지 않은 경우에만 아이템 사용 가능
        if (!dead)
        {
            // 충돌한 게임 오브젝트에서 IItem컴포넌트를 가져오기
            IItem item = other.GetComponent<IItem>();
            if (item != null)
            {
                // 아이템 사용
                item.Use(gameObject);
                // 아이템 습득 소리 재생
                playerAudioPlayer.PlayOneShot(itemPickupClip);
            }
        }
    }

    
}
