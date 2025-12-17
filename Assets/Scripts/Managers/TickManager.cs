using System;
using System.Collections;
using UnityEngine;

public class TickManager : MonoBehaviour
{
    #region Singleton

    public static TickManager Instance { get; private set; }

    #endregion

    #region Constants

    private const float MIN_TURN_DURATION = 10f;
    private const float DEFAULT_TURN_DURATION = 20f;
    private const float MAX_TURN_DURATION = 40f;

    #endregion

    #region Events

    public static event Action<int> OnTurnStarted;
    public static event Action OnTurnEnded;
    public static event Action<bool> OnAutomationToggled;
    public static event Action<float> OnTurnSpeedChanged;

    #endregion

    #region Properties

    public int CurrentTurn { get; private set; }
    public bool IsAutomated { get; private set; }
    public float CurrentTurnDuration { get; private set; }
    public float TurnSpeedMultiplier { get; private set; }
    public float TimeUntilNextTurn { get; private set; }

    #endregion

    #region Private Fields

    private Coroutine automationCoroutine;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeTickSystem();
    }

    private void Update()
    {
        if (IsAutomated)
        {
            UpdateAutomationTimer();
        }
    }

    #endregion

    #region Initialization

    private void InitializeTickSystem()
    {
        CurrentTurn = 0;
        IsAutomated = false;
        CurrentTurnDuration = DEFAULT_TURN_DURATION;
        TurnSpeedMultiplier = 1f;
        TimeUntilNextTurn = 0f;

        Debug.Log("[TickManager] Initialized - Starting in manual turn-based mode");
    }

    #endregion

    #region Turn Management

    public void AdvanceTurn()
    {
        if (IsAutomated)
        {
            Debug.LogWarning("[TickManager] Cannot manually advance turn while automation is active");
            return;
        }

        ExecuteTurn();
    }

    private void ExecuteTurn()
    {
        CurrentTurn++;

        Debug.Log($"[TickManager] Turn {CurrentTurn} started");

        OnTurnStarted?.Invoke(CurrentTurn);

        OnTurnEnded?.Invoke();
    }

    #endregion

    #region Automation Control

    public void ToggleAutomation()
    {
        IsAutomated = !IsAutomated;

        if (IsAutomated)
        {
            StartAutomation();
        }
        else
        {
            StopAutomation();
        }

        OnAutomationToggled?.Invoke(IsAutomated);

        Debug.Log($"[TickManager] Automation {(IsAutomated ? "enabled" : "disabled")}");
    }

    private void StartAutomation()
    {
        if (automationCoroutine != null)
        {
            StopCoroutine(automationCoroutine);
        }

        TimeUntilNextTurn = CurrentTurnDuration;
        automationCoroutine = StartCoroutine(AutomationLoop());
    }

    private void StopAutomation()
    {
        if (automationCoroutine != null)
        {
            StopCoroutine(automationCoroutine);
            automationCoroutine = null;
        }

        TimeUntilNextTurn = 0f;
    }

    private IEnumerator AutomationLoop()
    {
        while (IsAutomated)
        {
            yield return new WaitForSeconds(CurrentTurnDuration);
            ExecuteTurn();
            TimeUntilNextTurn = CurrentTurnDuration;
        }
    }

    private void UpdateAutomationTimer()
    {
        if (TimeUntilNextTurn > 0)
        {
            TimeUntilNextTurn -= Time.deltaTime;
            TimeUntilNextTurn = Mathf.Max(0f, TimeUntilNextTurn);
        }
    }

    #endregion

    #region Speed Control

    public void SpeedUpTurns()
    {
        if (CurrentTurnDuration <= MIN_TURN_DURATION)
        {
            Debug.LogWarning("[TickManager] Already at maximum speed");
            return;
        }

        float oldDuration = CurrentTurnDuration;
        CurrentTurnDuration = Mathf.Max(MIN_TURN_DURATION, CurrentTurnDuration * 0.5f);
        UpdateSpeedMultiplier();

        Debug.Log($"[TickManager] Turn duration decreased to {CurrentTurnDuration}s");

        OnTurnSpeedChanged?.Invoke(CurrentTurnDuration);

        if (IsAutomated)
        {
            AdjustRemainingTime(oldDuration, CurrentTurnDuration);
        }
    }

    public void SlowDownTurns()
    {
        if (CurrentTurnDuration >= MAX_TURN_DURATION)
        {
            Debug.LogWarning("[TickManager] Already at minimum speed");
            return;
        }

        float oldDuration = CurrentTurnDuration;
        CurrentTurnDuration = Mathf.Min(MAX_TURN_DURATION, CurrentTurnDuration * 2f);
        UpdateSpeedMultiplier();

        Debug.Log($"[TickManager] Turn duration increased to {CurrentTurnDuration}s");

        OnTurnSpeedChanged?.Invoke(CurrentTurnDuration);

        if (IsAutomated)
        {
            AdjustRemainingTime(oldDuration, CurrentTurnDuration);
        }
    }

    public void ResetTurnSpeed()
    {
        float oldDuration = CurrentTurnDuration;
        CurrentTurnDuration = DEFAULT_TURN_DURATION;
        UpdateSpeedMultiplier();

        Debug.Log($"[TickManager] Turn duration reset to {CurrentTurnDuration}s");

        OnTurnSpeedChanged?.Invoke(CurrentTurnDuration);

        if (IsAutomated)
        {
            AdjustRemainingTime(oldDuration, CurrentTurnDuration);
        }
    }

    private void UpdateSpeedMultiplier()
    {
        TurnSpeedMultiplier = DEFAULT_TURN_DURATION / CurrentTurnDuration;
    }

    private void AdjustRemainingTime(float oldDuration, float newDuration)
    {
        float progressRatio = 1f - (TimeUntilNextTurn / oldDuration);
        TimeUntilNextTurn = newDuration * (1f - progressRatio);

        Debug.Log($"[TickManager] Adjusted remaining time from {TimeUntilNextTurn + (newDuration - oldDuration):F1}s to {TimeUntilNextTurn:F1}s");

        RestartAutomationWithAdjustedTime();
    }

    private void RestartAutomationWithAdjustedTime()
    {
        if (automationCoroutine != null)
        {
            StopCoroutine(automationCoroutine);
        }

        automationCoroutine = StartCoroutine(AutomationLoopWithRemainingTime());
    }

    private IEnumerator AutomationLoopWithRemainingTime()
    {
        yield return new WaitForSeconds(TimeUntilNextTurn);

        ExecuteTurn();
        TimeUntilNextTurn = CurrentTurnDuration;

        while (IsAutomated)
        {
            yield return new WaitForSeconds(CurrentTurnDuration);
            ExecuteTurn();
            TimeUntilNextTurn = CurrentTurnDuration;
        }
    }

    #endregion

    #region Debug Utilities

    public string GetDebugInfo()
    {
        return $"Turn: {CurrentTurn} | Auto: {IsAutomated} | Duration: {CurrentTurnDuration}s | Speed: {TurnSpeedMultiplier}x" +
               (IsAutomated ? $" | Next in: {TimeUntilNextTurn:F1}s" : "");
    }

    #endregion
}
