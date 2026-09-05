using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static event Action<float, int, int, int> OnStateUpdated;
    public static event Action<FireworkData, int, int> OnFireworkLaunched;
    public static event Action<ComboRule, int, int> OnComboTriggered;
    public static event Action<int, int, int> OnGameEnded;

    [Header("Game Time")]
    [SerializeField] private float totalDuration = 180f;

    [Header("Resource")]
    [SerializeField] private int maxResource = 200;
    [SerializeField] private float resourceRegenPerSecond = 18f;
    [SerializeField] private int startResource = 120;

    [Header("Data")]
    [SerializeField] private FireworkData[] fireworkDatas;
    [SerializeField] private ComboRule[] comboRules;

    [Header("Score")]
    [SerializeField] private int maxDisplayedScore = 10000;

    private readonly Dictionary<FireworkType, FireworkData> fireworkByType = new Dictionary<FireworkType, FireworkData>();
    private readonly Dictionary<FireworkType, float> nextReadyTimeByType = new Dictionary<FireworkType, float>();
    private readonly List<LaunchRecord> launchHistory = new List<LaunchRecord>();

    private float remainingTime;
    private float currentResourceFloat;
    private int currentAudience;
    private int currentSatisfaction;
    private bool isGameEnded;

    private struct LaunchRecord
    {
        public FireworkType Type;
        public float TimeStamp;

        public LaunchRecord(FireworkType type, float timeStamp)
        {
            Type = type;
            TimeStamp = timeStamp;
        }
    }

    public float RemainingTime => remainingTime;
    public int CurrentResource => Mathf.FloorToInt(currentResourceFloat);
    public int CurrentAudience => currentAudience;
    public int CurrentSatisfaction => currentSatisfaction;
    public bool IsGameEnded => isGameEnded;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        BuildFireworkDictionary();
        ResetGameState();
    }

    private void Update()
    {
        if (isGameEnded)
        {
            return;
        }

        TickTimer(Time.deltaTime);
        RegenResource(Time.deltaTime);
        BroadcastState();

        if (remainingTime <= 0f)
        {
            EndGame();
        }
    }

    public bool TryLaunchFirework(FireworkType type, out string failReason)
    {
        failReason = string.Empty;
        if (isGameEnded)
        {
            failReason = "GameEnded";
            return false;
        }

        FireworkData data;
        if (!fireworkByType.TryGetValue(type, out data))
        {
            failReason = "MissingData";
            return false;
        }

        float now = Time.time;
        float nextReadyTime;
        if (nextReadyTimeByType.TryGetValue(type, out nextReadyTime) && now < nextReadyTime)
        {
            failReason = "Cooldown";
            return false;
        }

        int currentResource = CurrentResource;
        if (currentResource < data.ResourceCost)
        {
            failReason = "InsufficientResource";
            return false;
        }

        nextReadyTimeByType[type] = now + data.Cooldown;
        currentResourceFloat -= data.ResourceCost;
        if (currentResourceFloat < 0f)
        {
            currentResourceFloat = 0f;
        }

        float audienceMultiplier;
        float satisfactionMultiplier;
        GetTimelineMultipliers(out audienceMultiplier, out satisfactionMultiplier);

        int gainedAudience = Mathf.RoundToInt(data.AudienceGain * audienceMultiplier);
        int gainedSatisfaction = Mathf.RoundToInt(data.SatisfactionGain * satisfactionMultiplier);

        currentAudience += gainedAudience;
        currentSatisfaction += gainedSatisfaction;

        float launchTime = totalDuration - remainingTime;
        launchHistory.Add(new LaunchRecord(type, launchTime));

        OnFireworkLaunched?.Invoke(data, gainedAudience, gainedSatisfaction);
        EvaluateCombos(launchTime);
        BroadcastState();

        return true;
    }

    public float GetCooldownRemaining(FireworkType type)
    {
        float nextReadyTime;
        if (!nextReadyTimeByType.TryGetValue(type, out nextReadyTime))
        {
            return 0f;
        }

        float remaining = nextReadyTime - Time.time;
        return remaining > 0f ? remaining : 0f;
    }

    public FireworkData GetFireworkData(FireworkType type)
    {
        FireworkData data;
        fireworkByType.TryGetValue(type, out data);
        return data;
    }

    public int CalculateFinalScore()
    {
        return Mathf.Clamp(currentSatisfaction, 0, maxDisplayedScore);
    }

    private void BuildFireworkDictionary()
    {
        fireworkByType.Clear();
        nextReadyTimeByType.Clear();

        if (fireworkDatas == null)
        {
            return;
        }

        for (int i = 0; i < fireworkDatas.Length; i++)
        {
            FireworkData data = fireworkDatas[i];
            if (data == null)
            {
                continue;
            }

            fireworkByType[data.Type] = data;
            nextReadyTimeByType[data.Type] = 0f;
        }
    }

    private void ResetGameState()
    {
        remainingTime = totalDuration;
        currentResourceFloat = Mathf.Clamp(startResource, 0, maxResource);
        currentAudience = 0;
        currentSatisfaction = 0;
        launchHistory.Clear();
        isGameEnded = false;
        BroadcastState();
    }

    private void TickTimer(float deltaTime)
    {
        remainingTime -= deltaTime;
        if (remainingTime < 0f)
        {
            remainingTime = 0f;
        }
    }

    private void RegenResource(float deltaTime)
    {
        currentResourceFloat += resourceRegenPerSecond * deltaTime;
        if (currentResourceFloat > maxResource)
        {
            currentResourceFloat = maxResource;
        }
    }

    private void BroadcastState()
    {
        OnStateUpdated?.Invoke(remainingTime, CurrentResource, currentAudience, currentSatisfaction);
    }

    private void EndGame()
    {
        if (isGameEnded)
        {
            return;
        }

        isGameEnded = true;
        int finalScore = CalculateFinalScore();
        OnGameEnded?.Invoke(finalScore, currentAudience, currentSatisfaction);
    }

    private void GetTimelineMultipliers(out float audienceMultiplier, out float satisfactionMultiplier)
    {
        float elapsed = totalDuration - remainingTime;
        audienceMultiplier = 1f;
        satisfactionMultiplier = 1f;

        if (elapsed < 45f)
        {
            audienceMultiplier = 1f;
            satisfactionMultiplier = 1f;
            return;
        }

        if (elapsed < 90f)
        {
            audienceMultiplier = 1.2f;
            satisfactionMultiplier = 1.2f;
            return;
        }

        if (elapsed < 135f)
        {
            audienceMultiplier = 1.5f;
            satisfactionMultiplier = 1.5f;
            return;
        }

        if (elapsed < 165f)
        {
            audienceMultiplier = 1.8f;
            satisfactionMultiplier = 1.8f;
            return;
        }

        audienceMultiplier = 1.8f;
        satisfactionMultiplier = 2.2f;
    }

    private void EvaluateCombos(float currentLaunchTime)
    {
        if (comboRules == null)
        {
            return;
        }

        for (int i = 0; i < comboRules.Length; i++)
        {
            ComboRule rule = comboRules[i];
            if (rule == null || rule.RequiredSequence == null || rule.RequiredSequence.Length == 0)
            {
                continue;
            }

            if (TryMatchCombo(rule, currentLaunchTime))
            {
                float audienceMultiplier;
                float satisfactionMultiplier;
                GetTimelineMultipliers(out audienceMultiplier, out satisfactionMultiplier);

                int comboAudience = Mathf.RoundToInt(rule.BonusAudience * audienceMultiplier);
                int comboSatisfaction = Mathf.RoundToInt(rule.BonusSatisfaction * satisfactionMultiplier);

                currentAudience += comboAudience;
                currentSatisfaction += comboSatisfaction;

                OnComboTriggered?.Invoke(rule, comboAudience, comboSatisfaction);
                BroadcastState();
            }
        }
    }

    private bool TryMatchCombo(ComboRule rule, float currentLaunchTime)
    {
        int neededCount = rule.RequiredSequence.Length;
        if (launchHistory.Count < neededCount)
        {
            return false;
        }

        int startIndex = launchHistory.Count - neededCount;
        float firstTime = launchHistory[startIndex].TimeStamp;
        float lastTime = launchHistory[launchHistory.Count - 1].TimeStamp;

        if (lastTime - firstTime > rule.TimeLimitSeconds)
        {
            return false;
        }

        if (rule.EnforceOrder)
        {
            for (int i = 0; i < neededCount; i++)
            {
                if (launchHistory[startIndex + i].Type != rule.RequiredSequence[i])
                {
                    return false;
                }
            }

            return true;
        }

        Dictionary<FireworkType, int> requiredCount = new Dictionary<FireworkType, int>();
        Dictionary<FireworkType, int> actualCount = new Dictionary<FireworkType, int>();

        for (int i = 0; i < neededCount; i++)
        {
            FireworkType required = rule.RequiredSequence[i];
            if (!requiredCount.ContainsKey(required))
            {
                requiredCount[required] = 0;
            }
            requiredCount[required] += 1;

            FireworkType actual = launchHistory[startIndex + i].Type;
            if (!actualCount.ContainsKey(actual))
            {
                actualCount[actual] = 0;
            }
            actualCount[actual] += 1;
        }

        foreach (KeyValuePair<FireworkType, int> pair in requiredCount)
        {
            int count;
            if (!actualCount.TryGetValue(pair.Key, out count))
            {
                return false;
            }

            if (count != pair.Value)
            {
                return false;
            }
        }

        if (launchHistory.Count > 0)
        {
            launchHistory.RemoveAt(launchHistory.Count - 1);
            launchHistory.Add(new LaunchRecord(launchHistory[launchHistory.Count - 1].Type, currentLaunchTime));
        }

        return true;
    }
}
