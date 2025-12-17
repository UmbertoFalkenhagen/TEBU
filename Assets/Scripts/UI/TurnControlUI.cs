using UnityEngine;
using UnityEngine.UIElements;

public class TurnControlUI : MonoBehaviour
{
    #region Singleton & Serialized Fields

    public static TurnControlUI Instance { get; private set; }

    [Header("UI Document References")]
    [SerializeField] private UIDocument uiDocument;

    [Header("Animation Settings")]
    [SerializeField] private float manualRotationDuration = 0.5f;

    #endregion

    #region UI Element References

    private VisualElement turnControlContainer;
    private VisualElement rotatingBackground;
    private Label turnCounterLabel;
    private Label turnTimerLabel;
    private Button endTurnButton;
    private Button automateTurnsButton;
    private Button speedUpButton;
    private Button slowDownButton;

    #endregion

    #region Rotation State

    private float currentRotation = 0f;
    private bool isManualRotating = false;
    private float manualRotationProgress = 0f;
    private float manualRotationStartAngle = 0f;

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
    }

    private void OnEnable()
    {
        if (uiDocument == null)
        {
            uiDocument = GetComponent<UIDocument>();
        }

        if (uiDocument != null && uiDocument.rootVisualElement != null)
        {
            SetupUI();
        }

        SubscribeToTickManagerEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromTickManagerEvents();
        UnregisterButtonEvents();
    }

    private void Update()
    {
        UpdateTimerDisplay();
        UpdateBackgroundRotation();
    }

    #endregion

    #region Initialization

    private void SetupUI()
    {
        var root = uiDocument.rootVisualElement;

        turnControlContainer = root.Q<VisualElement>("TurnControlContainer");
        rotatingBackground = root.Q<VisualElement>("RotatingBackground");
        turnCounterLabel = root.Q<Label>("TurnCounterLabel");
        turnTimerLabel = root.Q<Label>("TurnTimerLabel");
        endTurnButton = root.Q<Button>("EndTurnButton");
        automateTurnsButton = root.Q<Button>("AutomateTurnsButton");
        speedUpButton = root.Q<Button>("SpeedUpButton");
        slowDownButton = root.Q<Button>("SlowDownButton");

        if (rotatingBackground != null)
        {
            rotatingBackground.style.transformOrigin = new TransformOrigin(Length.Percent(50), Length.Percent(50));
        }

        RegisterButtonEvents();

        UpdateUIState(false);
        UpdateTurnDisplay(0);
    }

    private void RegisterButtonEvents()
    {
        if (endTurnButton != null)
        {
            endTurnButton.clicked += OnEndTurnClicked;
        }

        if (automateTurnsButton != null)
        {
            automateTurnsButton.clicked += OnAutomateToggled;
        }

        if (speedUpButton != null)
        {
            speedUpButton.clicked += OnSpeedUpClicked;
        }

        if (slowDownButton != null)
        {
            slowDownButton.clicked += OnSlowDownClicked;
        }
    }

    private void UnregisterButtonEvents()
    {
        if (endTurnButton != null)
        {
            endTurnButton.clicked -= OnEndTurnClicked;
        }

        if (automateTurnsButton != null)
        {
            automateTurnsButton.clicked -= OnAutomateToggled;
        }

        if (speedUpButton != null)
        {
            speedUpButton.clicked -= OnSpeedUpClicked;
        }

        if (slowDownButton != null)
        {
            slowDownButton.clicked -= OnSlowDownClicked;
        }
    }

    #endregion

    #region Event Subscriptions

    private void SubscribeToTickManagerEvents()
    {
        TickManager.OnTurnStarted += OnTurnStarted;
        TickManager.OnTurnEnded += OnTurnEnded;
        TickManager.OnAutomationToggled += OnAutomationStateChanged;
        TickManager.OnTurnSpeedChanged += OnTurnSpeedChanged;
    }

    private void UnsubscribeFromTickManagerEvents()
    {
        TickManager.OnTurnStarted -= OnTurnStarted;
        TickManager.OnTurnEnded -= OnTurnEnded;
        TickManager.OnAutomationToggled -= OnAutomationStateChanged;
        TickManager.OnTurnSpeedChanged -= OnTurnSpeedChanged;
    }

    #endregion

    #region Button Click Handlers

    private void OnEndTurnClicked()
    {
        if (TickManager.Instance != null)
        {
            Debug.Log("[TurnControlUI] End Turn button clicked");
            TickManager.Instance.AdvanceTurn();
        }
    }

    private void OnAutomateToggled()
    {
        if (TickManager.Instance != null)
        {
            Debug.Log("[TurnControlUI] Automate button clicked");
            TickManager.Instance.ToggleAutomation();
        }
    }

    private void OnSpeedUpClicked()
    {
        if (TickManager.Instance != null)
        {
            Debug.Log("[TurnControlUI] Speed Up button clicked");
            TickManager.Instance.SpeedUpTurns();
        }
    }

    private void OnSlowDownClicked()
    {
        if (TickManager.Instance != null)
        {
            Debug.Log("[TurnControlUI] Slow Down button clicked");
            TickManager.Instance.SlowDownTurns();
        }
    }

    #endregion

    #region TickManager Event Handlers

    private void OnTurnStarted(int turnNumber)
    {
        UpdateTurnDisplay(turnNumber);
    }

    private void OnTurnEnded()
    {
        if (TickManager.Instance != null && !TickManager.Instance.IsAutomated)
        {
            TriggerManualRotation();
        }
    }

    private void OnAutomationStateChanged(bool isAutomated)
    {
        Debug.Log($"[TurnControlUI] Automation state changed to: {isAutomated}");
        UpdateUIState(isAutomated);

        if (!isAutomated)
        {
            isManualRotating = false;
            manualRotationProgress = 0f;
        }
    }

    private void OnTurnSpeedChanged(float newDuration)
    {
        Debug.Log($"[TurnControlUI] Turn speed changed to {newDuration}s");
    }

    #endregion

    #region UI Update Methods

    private void UpdateUIState(bool isAutomated)
    {
        Debug.Log($"[TurnControlUI] UpdateUIState called with isAutomated={isAutomated}");

        if (endTurnButton != null)
        {
            endTurnButton.SetEnabled(!isAutomated);
        }

        if (speedUpButton != null)
        {
            speedUpButton.SetEnabled(isAutomated);
        }

        if (slowDownButton != null)
        {
            slowDownButton.SetEnabled(isAutomated);
        }

        if (automateTurnsButton != null)
        {
            automateTurnsButton.SetEnabled(true);

            if (isAutomated)
            {
                automateTurnsButton.AddToClassList("automation-active");
                automateTurnsButton.text = "Stop Auto";
            }
            else
            {
                automateTurnsButton.RemoveFromClassList("automation-active");
                automateTurnsButton.text = "Auto";
            }
        }

        if (turnTimerLabel != null)
        {
            turnTimerLabel.style.display = isAutomated ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    private void UpdateTurnDisplay(int turnNumber)
    {
        if (turnCounterLabel != null)
        {
            turnCounterLabel.text = $"Turn: {turnNumber}";
        }
    }

    private void UpdateTimerDisplay()
    {
        if (TickManager.Instance == null || turnTimerLabel == null) return;

        if (TickManager.Instance.IsAutomated)
        {
            float timeRemaining = TickManager.Instance.TimeUntilNextTurn;
            turnTimerLabel.text = $"Next turn in: {timeRemaining:F1}s";
        }
    }

    #endregion

    #region Background Rotation

    private void UpdateBackgroundRotation()
    {
        if (rotatingBackground == null) return;

        if (TickManager.Instance != null && TickManager.Instance.IsAutomated)
        {
            UpdateAutomatedRotation();
        }
        else if (isManualRotating)
        {
            UpdateManualRotation();
        }

        ApplyRotation();
    }

    private void UpdateAutomatedRotation()
    {
        float turnDuration = TickManager.Instance.CurrentTurnDuration;
        float timeRemaining = TickManager.Instance.TimeUntilNextTurn;

        if (turnDuration > 0)
        {
            float progress = 1f - (timeRemaining / turnDuration);
            currentRotation = progress * 360f;
        }
    }

    private void UpdateManualRotation()
    {
        manualRotationProgress += Time.deltaTime / manualRotationDuration;

        if (manualRotationProgress >= 1f)
        {
            manualRotationProgress = 1f;
            isManualRotating = false;
            currentRotation = 0f;
        }
        else
        {
            currentRotation = manualRotationStartAngle + (manualRotationProgress * 360f);
        }
    }

    private void TriggerManualRotation()
    {
        isManualRotating = true;
        manualRotationProgress = 0f;
        manualRotationStartAngle = currentRotation;
    }

    private void ApplyRotation()
    {
        if (rotatingBackground != null)
        {
            float normalizedRotation = currentRotation % 360f;
            rotatingBackground.style.rotate = new Rotate(new Angle(-normalizedRotation, AngleUnit.Degree));
        }
    }

    #endregion

    #region Public Methods

    public void Show()
    {
        if (turnControlContainer != null)
        {
            turnControlContainer.style.display = DisplayStyle.Flex;
        }
    }

    public void Hide()
    {
        if (turnControlContainer != null)
        {
            turnControlContainer.style.display = DisplayStyle.None;
        }
    }

    #endregion
}
