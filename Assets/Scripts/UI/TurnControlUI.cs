using UnityEngine;
using UnityEngine.UIElements;

public class TurnControlUI : MonoBehaviour
{
    #region Singleton & Serialized Fields

    public static TurnControlUI Instance { get; private set; }

    [Header("UI Document References")]
    [SerializeField] private UIDocument uiDocument;

    #endregion

    #region UI Element References

    private VisualElement turnControlContainer;
    private Label turnCounterLabel;
    private Label turnTimerLabel;
    private Button endTurnButton;
    private Button automateTurnsButton;
    private Button speedUpButton;
    private Button slowDownButton;
    private VisualElement highlightCircle;

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
    }

    private void Update()
    {
        UpdateTimerDisplay();
    }

    #endregion

    #region Initialization

    private void SetupUI()
    {
        var root = uiDocument.rootVisualElement;

        turnControlContainer = root.Q<VisualElement>("TurnControlContainer");
        turnCounterLabel = root.Q<Label>("TurnCounterLabel");
        turnTimerLabel = root.Q<Label>("TurnTimerLabel");
        endTurnButton = root.Q<Button>("EndTurnButton");
        automateTurnsButton = root.Q<Button>("AutomateTurnsButton");
        speedUpButton = root.Q<Button>("SpeedUpButton");
        slowDownButton = root.Q<Button>("SlowDownButton");
        highlightCircle = root.Q<VisualElement>("HighlightCircle");

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
            speedUpButton.SetEnabled(false);
        }

        if (slowDownButton != null)
        {
            slowDownButton.clicked += OnSlowDownClicked;
            slowDownButton.SetEnabled(false);
        }

        UpdateUIState(false);
        UpdateTurnDisplay(0);
    }

    #endregion

    #region Event Subscriptions

    private void SubscribeToTickManagerEvents()
    {
        TickManager.OnTurnStarted += OnTurnStarted;
        TickManager.OnAutomationToggled += OnAutomationStateChanged;
        TickManager.OnTurnSpeedChanged += OnTurnSpeedChanged;
    }

    private void UnsubscribeFromTickManagerEvents()
    {
        TickManager.OnTurnStarted -= OnTurnStarted;
        TickManager.OnAutomationToggled -= OnAutomationStateChanged;
        TickManager.OnTurnSpeedChanged -= OnTurnSpeedChanged;
    }

    #endregion

    #region Button Click Handlers

    private void OnEndTurnClicked()
    {
        if (TickManager.Instance != null)
        {
            TickManager.Instance.AdvanceTurn();
        }
    }

    private void OnAutomateToggled()
    {
        if (TickManager.Instance != null)
        {
            TickManager.Instance.ToggleAutomation();
        }
    }

    private void OnSpeedUpClicked()
    {
        if (TickManager.Instance != null)
        {
            TickManager.Instance.SpeedUpTurns();
        }
    }

    private void OnSlowDownClicked()
    {
        if (TickManager.Instance != null)
        {
            TickManager.Instance.SlowDownTurns();
        }
    }

    #endregion

    #region TickManager Event Handlers

    private void OnTurnStarted(int turnNumber)
    {
        UpdateTurnDisplay(turnNumber);
    }

    private void OnAutomationStateChanged(bool isAutomated)
    {
        UpdateUIState(isAutomated);
    }

    private void OnTurnSpeedChanged(float newDuration)
    {
        Debug.Log($"[TurnControlUI] Turn speed changed to {newDuration}s");
    }

    #endregion

    #region UI Update Methods

    private void UpdateUIState(bool isAutomated)
    {
        if (endTurnButton != null)
        {
            endTurnButton.SetEnabled(!isAutomated);
        }

        if (highlightCircle != null)
        {
            highlightCircle.style.display = isAutomated ? DisplayStyle.Flex : DisplayStyle.None;
        }

        if (automateTurnsButton != null)
        {
            if (isAutomated)
            {
                automateTurnsButton.AddToClassList("automation-active");
            }
            else
            {
                automateTurnsButton.RemoveFromClassList("automation-active");
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
