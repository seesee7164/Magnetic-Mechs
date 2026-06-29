using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputRebinding : MonoBehaviour {

    private const string PLAYER_PREFS_BINDING_OVERRIDES = "Binding Overrides";
    private const string PLAYER_PREFS_HOLD_TO_ATTRACT = "Hold to Attract";

    public static InputRebinding Instance { get; private set; }

    public event EventHandler OnInputRebindingStarted;
    public event EventHandler OnInputRebindingCompleted;
    public event EventHandler OnHoldToAttractChanged;

    [SerializeField] private PlayerInput playerInput;

    [Header("Input Actions")]
    [SerializeField] private string moveInputActionString = "Move";
    [SerializeField] private string jumpInputActionString = "Jump";
    [SerializeField] private string fireInputActionString = "Fire";
    [SerializeField] private string launchMagnetInputActionString = "LaunchMagnet";
    [SerializeField] private string attractInputActionString = "Attract";
    [SerializeField] private string repelInputActionString = "Repel";
    [SerializeField] private string recoverMagnetInputActionString = "RecoverMagnet";

    private bool holdToAttract = false;

    public enum Binding {
        MOVE_DOWN,
        MOVE_LEFT,
        MOVE_RIGHT,
        JUMP,
        FIRE,
        LAUNCH_MAGNET,
        ATTRACT,
        REPEL,
        RECOVER_MAGNET
    }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Debug.LogError("There is more than one InputRebinding " + this);
        }
    }

    private void Start() {
        playerInput.actions.Enable();

        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDING_OVERRIDES)) {
            string overridesJson = PlayerPrefs.GetString(PLAYER_PREFS_BINDING_OVERRIDES);
            playerInput.actions.LoadBindingOverridesFromJson(overridesJson);
        }
        if (PlayerPrefs.HasKey(PLAYER_PREFS_HOLD_TO_ATTRACT)) {
            SetHoldToAttract(PlayerPrefs.GetInt(PLAYER_PREFS_HOLD_TO_ATTRACT) != 0);
        }
    }

    public void RebindBinding(Binding binding) {
        InputAction action;
        int bindingIndex = 0;
        switch (binding) {
            default:
            case Binding.MOVE_LEFT:
                action = playerInput.actions.FindAction(moveInputActionString);
                bindingIndex = 1;
                break;
            case Binding.MOVE_DOWN:
                action = playerInput.actions.FindAction(moveInputActionString);
                bindingIndex = 2;
                break;
            case Binding.MOVE_RIGHT:
                action = playerInput.actions.FindAction(moveInputActionString);
                bindingIndex = 3;
                break;
            case Binding.JUMP:
                action = playerInput.actions.FindAction(jumpInputActionString);
                break;
            case Binding.FIRE:
                action = playerInput.actions.FindAction(fireInputActionString);
                break;
            case Binding.LAUNCH_MAGNET:
                action = playerInput.actions.FindAction(launchMagnetInputActionString);
                break;
            case Binding.ATTRACT:
                action = playerInput.actions.FindAction(attractInputActionString);
                break;
            case Binding.REPEL:
                action = playerInput.actions.FindAction(repelInputActionString);
                break;
            case Binding.RECOVER_MAGNET:
                action = playerInput.actions.FindAction(recoverMagnetInputActionString);
                break;
        }

        OnInputRebindingStarted?.Invoke(this, EventArgs.Empty);
        InputBinding oldBinding = action.bindings[bindingIndex];
        playerInput.actions.FindActionMap("Player").Disable();
        action.Disable();
        action.PerformInteractiveRebinding(bindingIndex).WithCancelingThrough("<Keyboard>/Escape").OnComplete(callback => {
            callback.Dispose();
            InputBinding newBinding = action.bindings[bindingIndex];
            foreach (InputAction currentAction in action.actionMap.actions)
            {
                for(int i = 0; i < currentAction.bindings.Count; i++)
                {
                    if (action.Equals(currentAction) && i == bindingIndex) continue;
                    if (currentAction.bindings[i].isComposite) continue;
                    if (newBinding.effectivePath == currentAction.bindings[i].effectivePath)
                    {
                        currentAction.Disable();
                        currentAction.ApplyBindingOverride(i, oldBinding.effectivePath);
                        currentAction.Enable();
                    }
                }
            }
            playerInput.actions.FindActionMap("Player").Enable();
            action.Enable();
            PlayerPrefs.SetString(PLAYER_PREFS_BINDING_OVERRIDES, playerInput.actions.SaveBindingOverridesAsJson());
            OnInputRebindingCompleted?.Invoke(this, EventArgs.Empty);
        }).OnCancel(callback => {
            callback.Dispose();
            playerInput.actions.FindActionMap("Player").Enable();
            action.Enable();
            OnInputRebindingCompleted?.Invoke(this, EventArgs.Empty);
        }).Start();
        //buttonSelectionManager.ResetButtonColors();
    }

    public void ResetAllBindings() {
        OnInputRebindingStarted?.Invoke(this, EventArgs.Empty);
        InputActionMap playerActionMap = playerInput.actions.FindActionMap("Player");
        playerActionMap.Disable();
        playerActionMap.RemoveAllBindingOverrides();
        playerActionMap.Enable();

        PlayerPrefs.SetString(PLAYER_PREFS_BINDING_OVERRIDES, playerInput.actions.SaveBindingOverridesAsJson());
        OnInputRebindingCompleted?.Invoke(this, EventArgs.Empty);

        SetHoldToAttract(false);
        PlayerPrefs.SetInt(PLAYER_PREFS_HOLD_TO_ATTRACT, holdToAttract ? 1 : 0);
        OnHoldToAttractChanged?.Invoke(this, EventArgs.Empty);
    }

    public string GetBinding(Binding binding) {
        switch (binding) {
            default:
            case Binding.MOVE_LEFT:
                return playerInput.actions.FindAction(moveInputActionString).bindings[1].ToDisplayString();
            case Binding.MOVE_DOWN:
                return playerInput.actions.FindAction(moveInputActionString).bindings[2].ToDisplayString();
            case Binding.MOVE_RIGHT:
                return playerInput.actions.FindAction(moveInputActionString).bindings[3].ToDisplayString();
            case Binding.JUMP:
                return playerInput.actions.FindAction(jumpInputActionString).bindings[0].ToDisplayString();
            case Binding.FIRE:
                return playerInput.actions.FindAction(fireInputActionString).bindings[0].ToDisplayString();
            case Binding.LAUNCH_MAGNET:
                return playerInput.actions.FindAction(launchMagnetInputActionString).bindings[0].ToDisplayString();
            case Binding.ATTRACT:
                return playerInput.actions.FindAction(attractInputActionString).bindings[0].ToDisplayString();
            case Binding.REPEL:
                return playerInput.actions.FindAction(repelInputActionString).bindings[0].ToDisplayString();
            case Binding.RECOVER_MAGNET:
                return playerInput.actions.FindAction(recoverMagnetInputActionString).bindings[0].ToDisplayString();
        }
    }

    public void SetHoldToAttract(bool holdToAttract) {
        this.holdToAttract = holdToAttract;
        PlayerPrefs.SetInt(PLAYER_PREFS_HOLD_TO_ATTRACT, holdToAttract ? 1 : 0);
        OnHoldToAttractChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool GetHoldToAttract() {
        return holdToAttract;
    }
}
