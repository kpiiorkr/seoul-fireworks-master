using UnityEngine;

public enum FireworkType
{
    Peony,
    Niagara,
    Ring,
    Willow,
    Crossette
}

[CreateAssetMenu(fileName = "FireworkData", menuName = "Seoul Firework/Firework Data")]
public class FireworkData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private FireworkType type;
    [SerializeField] private string displayName;
    [SerializeField] private Sprite icon;

    [Header("Gameplay")]
    [SerializeField] private float cooldown;
    [SerializeField] private int resourceCost;
    [SerializeField] private int audienceGain;
    [SerializeField] private int satisfactionGain;

    [Header("Effects")]
    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private AudioClip launchSfx;

    public FireworkType Type => type;
    public string DisplayName => displayName;
    public Sprite Icon => icon;
    public float Cooldown => cooldown;
    public int ResourceCost => resourceCost;
    public int AudienceGain => audienceGain;
    public int SatisfactionGain => satisfactionGain;
    public GameObject ParticlePrefab => particlePrefab;
    public AudioClip LaunchSfx => launchSfx;
}
