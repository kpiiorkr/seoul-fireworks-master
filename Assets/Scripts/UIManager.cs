using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI resourceText;
    [SerializeField] private TextMeshProUGUI audienceText;
    [SerializeField] private TextMeshProUGUI satisfactionText;

    [Header("Combo Toast")]
    [SerializeField] private CanvasGroup comboToastGroup;
    [SerializeField] private TextMeshProUGUI comboToastText;
    [SerializeField] private float comboToastDuration = 1.25f;

    [Header("Result Panel")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI finalAudienceText;
    [SerializeField] private TextMeshProUGUI finalSatisfactionText;
    [SerializeField] private TextMeshProUGUI finalGradeText;
    [SerializeField] private TextMeshProUGUI finalTitleText;
    [SerializeField] private Button shareButton;

    [Header("Share")]
    [SerializeField] private ShareManager shareManager;

    private Coroutine comboToastCoroutine;

    private void OnEnable()
    {
        GameManager.OnStateUpdated += HandleStateUpdated;
        GameManager.OnComboTriggered += HandleComboTriggered;
        GameManager.OnGameEnded += HandleGameEnded;

        if (shareButton != null)
        {
            shareButton.onClick.AddListener(HandleShareClicked);
        }
    }

    private void OnDisable()
    {
        GameManager.OnStateUpdated -= HandleStateUpdated;
        GameManager.OnComboTriggered -= HandleComboTriggered;
        GameManager.OnGameEnded -= HandleGameEnded;

        if (shareButton != null)
        {
            shareButton.onClick.RemoveListener(HandleShareClicked);
        }
    }

    private void Start()
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }

        if (comboToastGroup != null)
        {
            comboToastGroup.alpha = 0f;
            comboToastGroup.interactable = false;
            comboToastGroup.blocksRaycasts = false;
        }
    }

    private void HandleStateUpdated(float remainingTime, int currentResource, int audience, int satisfaction)
    {
        if (timeText != null)
        {
            TimeSpan span = TimeSpan.FromSeconds(Mathf.CeilToInt(remainingTime));
            timeText.text = string.Format("{0:D2}:{1:D2}", span.Minutes, span.Seconds);
        }

        if (resourceText != null)
        {
            resourceText.text = "자원 " + currentResource;
        }

        if (audienceText != null)
        {
            audienceText.text = "관중 " + audience.ToString("N0");
        }

        if (satisfactionText != null)
        {
            satisfactionText.text = "만족도 " + satisfaction.ToString("N0");
        }
    }

    private void HandleComboTriggered(ComboRule comboRule, int bonusAudience, int bonusSatisfaction)
    {
        if (comboRule == null)
        {
            return;
        }

        string message = string.IsNullOrWhiteSpace(comboRule.ToastMessage)
            ? comboRule.ComboName + " 콤보!"
            : comboRule.ToastMessage;

        if (comboToastCoroutine != null)
        {
            StopCoroutine(comboToastCoroutine);
        }

        comboToastCoroutine = StartCoroutine(PlayComboToast(message, bonusAudience, bonusSatisfaction));
    }

    private IEnumerator PlayComboToast(string message, int bonusAudience, int bonusSatisfaction)
    {
        if (comboToastGroup == null || comboToastText == null)
        {
            yield break;
        }

        comboToastText.text = message + "  +관중 " + bonusAudience.ToString("N0") + " / +만족도 " + bonusSatisfaction.ToString("N0");
        comboToastGroup.alpha = 1f;

        float elapsed = 0f;
        while (elapsed < comboToastDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        float fadeDuration = 0.3f;
        float fadeElapsed = 0f;
        while (fadeElapsed < fadeDuration)
        {
            fadeElapsed += Time.deltaTime;
            comboToastGroup.alpha = Mathf.Lerp(1f, 0f, fadeElapsed / fadeDuration);
            yield return null;
        }

        comboToastGroup.alpha = 0f;
        comboToastCoroutine = null;
    }

    private void HandleGameEnded(int finalScore, int finalAudience, int finalSatisfaction)
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = "최종 점수 " + finalScore.ToString("N0");
        }

        if (finalAudienceText != null)
        {
            finalAudienceText.text = "최종 관중 " + finalAudience.ToString("N0");
        }

        if (finalSatisfactionText != null)
        {
            finalSatisfactionText.text = "최종 만족도 " + finalSatisfaction.ToString("N0");
        }

        string grade;
        string title;
        EvaluateGrade(finalScore, out grade, out title);

        if (finalGradeText != null)
        {
            finalGradeText.text = "등급 " + grade;
        }

        if (finalTitleText != null)
        {
            finalTitleText.text = title;
        }
    }

    private void EvaluateGrade(int score, out string grade, out string title)
    {
        if (score >= 9500)
        {
            grade = "SSS";
            title = "불꽃의 골든아워 마스터";
            return;
        }

        if (score >= 8800)
        {
            grade = "SS";
            title = "라이트업 마에스트로";
            return;
        }

        if (score >= 7800)
        {
            grade = "S";
            title = "여의도 스카이 디렉터";
            return;
        }

        if (score >= 6500)
        {
            grade = "A";
            title = "오렌지 스카이 메이커";
            return;
        }

        grade = "B";
        title = "한강 야경 점화반장";
    }

    private void HandleShareClicked()
    {
        if (shareManager != null)
        {
            shareManager.CaptureAndSaveResult();
        }
    }
}
