using UnityEngine;
using UnityEngine.UI;

public class FireworkLauncher : MonoBehaviour
{
    [SerializeField] private FireworkType fireworkType;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Image cooldownRadialFill;
    [SerializeField] private AudioSource audioSource;

    private FireworkData fireworkData;

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager instance is missing in scene.");
            enabled = false;
            return;
        }

        fireworkData = GameManager.Instance.GetFireworkData(fireworkType);
        if (fireworkData == null)
        {
            Debug.LogError("FireworkData is missing for type: " + fireworkType);
            enabled = false;
            return;
        }

        if (cooldownRadialFill != null)
        {
            cooldownRadialFill.fillAmount = 0f;
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null || fireworkData == null)
        {
            return;
        }

        if (cooldownRadialFill != null)
        {
            float remain = GameManager.Instance.GetCooldownRemaining(fireworkType);
            cooldownRadialFill.fillAmount = fireworkData.Cooldown > 0f ? remain / fireworkData.Cooldown : 0f;
        }
    }

    public void OnClickLaunch()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        string failReason;
        if (GameManager.Instance.TryLaunchFirework(fireworkType, out failReason))
        {
            PlayEffects();
        }
    }

    private void PlayEffects()
    {
        Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;

        if (fireworkData.ParticlePrefab != null)
        {
            Instantiate(fireworkData.ParticlePrefab, position, Quaternion.identity);
        }

        if (fireworkData.LaunchSfx != null)
        {
            if (audioSource != null)
            {
                audioSource.PlayOneShot(fireworkData.LaunchSfx);
            }
            else
            {
                AudioSource.PlayClipAtPoint(fireworkData.LaunchSfx, position);
            }
        }
    }
}
