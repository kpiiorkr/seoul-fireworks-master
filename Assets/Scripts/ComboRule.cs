using UnityEngine;

[CreateAssetMenu(fileName = "ComboRule", menuName = "Seoul Firework/Combo Rule")]
public class ComboRule : ScriptableObject
{
    [SerializeField] private string comboName;
    [SerializeField] private FireworkType[] requiredSequence;
    [SerializeField] private bool enforceOrder = true;
    [SerializeField] private float timeLimitSeconds = 2.5f;
    [SerializeField] private int bonusAudience;
    [SerializeField] private int bonusSatisfaction;
    [SerializeField] private string toastMessage;

    public string ComboName => comboName;
    public FireworkType[] RequiredSequence => requiredSequence;
    public bool EnforceOrder => enforceOrder;
    public float TimeLimitSeconds => timeLimitSeconds;
    public int BonusAudience => bonusAudience;
    public int BonusSatisfaction => bonusSatisfaction;
    public string ToastMessage => toastMessage;
}
