using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputRebinding : MonoBehaviour {

    private const string PLAYER_PREFS_BINDING_OVERRIDES = "Binding Overrides";
    
    public static InputRebinding Instance { get; private set; }

    public event EventHandler OnInputRebindingStarted;
    public event EventHandler OnInputRebindingCompleted;

    [SerializeField] private PlayerInput playerInput;

    [Header("Input Actions")]
    [SerializeField] string moveInputActionString = "Move";
    [SerializeField] string jumpInputActionString = "Jump";
    [SerializeField] string fireInputActionString = "Fire";
    [SerializeField] string launchMagnetInputActionString = "LaunchMagnet";
    [SerializeField] string attractInputActionString = "Attract";
    [SerializeField] string repelInputActionString = "Repel";
    [SerializeField] string chargeInputActionString = "Charge";

    public enum Binding {
        MOVE_UP,
        MOVE_DOWN,
        MOVE_LEFT,
        MOVE_RIGHT,
        JUMP,
        FIRE,
        LAUNCH_MAGNET,
        ATTRACT,
        REPEL,
        CHARGE
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
    }

    public void RebindBinding(Binding binding) {
        InputAction action;
        int bindingIndex = 0;
        switch (binding) {
            default:
            case Binding.MOVE_UP:
                action = playerInput.actions.FindAction(moveInputActionString);
                bindingIndex = 1;
                break;
            case Binding.MOVE_LEFT:
                action = playerInput.actions.FindAction(moveInputActionString);
                bindingIndex = 2;
                break;
            case Binding.MOVE_DOWN:
                action = playerInput.actions.FindAction(moveInputActionString);
                bindingIndex = 3;
                break;
            case Binding.MOVE_RIGHT:
                action = playerInput.actions.FindAction(moveInputActionString);
                bindingIndex = 4;
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
            case Binding.CHARGE:
                action = playerInput.actions.FindAction(chargeInputActionString);
                break;
        }

        OnInputRebindingStarted?.Invoke(this, EventArgs.Empty);
        playerInput.actions.FindActionMap("Player").Disable();
        action.Disable();
        action.PerformInteractiveRebinding(bindingIndex).OnComplete(callback => {
            callback.Dispose();
            playerInput.actions.FindActionMap("Player").Enable();
            action.Enable();
            PlayerPrefs.SetString(PLAYER_PREFS_BINDING_OVERRIDES, playerInput.actions.SaveBindingOverridesAsJson());
            OnInputRebindingCompleted?.Invoke(this, EventArgs.Empty);
        }).Start();
    }

    public void ResetAllBindings() {
        OnInputRebindingStarted?.Invoke(this, EventArgs.Empty);
        InputActionMap playerActionMap = playerInput.actions.FindActionMap("Player");
        playerActionMap.Disable();
        playerActionMap.RemoveAllBindingOverrides();
        playerActionMap.Enable();

        PlayerPrefs.SetString(PLAYER_PREFS_BINDING_OVERRIDES, playerInput.actions.SaveBindingOverridesAsJson());
        OnInputRebindingCompleted?.Invoke(this, EventArgs.Empty);
    }

    public string GetBinding(Binding binding) {
        switch (binding) {
            default:
            case Binding.MOVE_UP:
                return playerInput.actions.FindAction(moveInputActionString).bindings[1].ToDisplayString();
            case Binding.MOVE_LEFT:
                return playerInput.actions.FindAction(moveInputActionString).bindings[2].ToDisplayString();
            case Binding.MOVE_DOWN:
                return playerInput.actions.FindAction(moveInputActionString).bindings[3].ToDisplayString();
            case Binding.MOVE_RIGHT:
                return playerInput.actions.FindAction(moveInputActionString).bindings[4].ToDisplayString();
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
            case Binding.CHARGE:
                return playerInput.actions.FindAction(chargeInputActionString).bindings[0].ToDisplayString();
        }
    }
}
