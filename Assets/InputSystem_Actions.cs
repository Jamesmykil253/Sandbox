// This is the auto-generated script, now with a "Score" action added.
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @InputSystem_Actions: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputSystem_Actions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputSystem_Actions"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""df70fa95-8a34-4494-b137-73ab6b9c7d37"",
            ""actions"": [
                { ""name"": ""Move"", ""type"": ""Value"", ""id"": ""351f2ccd-1f9f-44bf-9bec-d62ac5c5f408"", ""expectedControlType"": ""Vector2"", ""processors"": """", ""interactions"": """", ""initialStateCheck"": true },
                { ""name"": ""Jump"", ""type"": ""Button"", ""id"": ""f1ba0d36-48eb-4cd5-b651-1c94a6531f70"", ""expectedControlType"": ""Button"", ""processors"": """", ""interactions"": """", ""initialStateCheck"": false },
                { ""name"": ""CycleCamera"", ""type"": ""Button"", ""id"": ""YOUR_UNIQUE_ID_FOR_CYCLE_CAMERA"", ""expectedControlType"": ""Button"", ""processors"": """", ""interactions"": """", ""initialStateCheck"": false },
                { ""name"": ""Attack"", ""type"": ""Button"", ""id"": ""6c2ab1b8-8984-453a-af3d-a3c78ae1679a"", ""expectedControlType"": ""Button"", ""processors"": """", ""interactions"": """", ""initialStateCheck"": false },
                { ""name"": ""Score"", ""type"": ""Button"", ""id"": ""a1b2c3d4-e5f6-7890-1234-567890abcdef"", ""expectedControlType"": ""Button"", ""processors"": """", ""interactions"": ""Hold(duration=0.1)"", ""initialStateCheck"": false }
            ],
            ""bindings"": [
                { ""name"": ""WASD"", ""id"": ""00ca640b-d935-4593-8157-c05846ea39b3"", ""path"": ""Dpad"", ""interactions"": """", ""processors"": """", ""groups"": """", ""action"": ""Move"", ""isComposite"": true, ""isPartOfComposite"": false },
                { ""name"": ""up"", ""id"": ""e2062cb9-1b15-46a2-838c-2f8d72a0bdd9"", ""path"": ""<Keyboard>/w"", ""groups"": ""Keyboard&Mouse"", ""action"": ""Move"", ""isComposite"": false, ""isPartOfComposite"": true },
                { ""name"": ""down"", ""id"": ""320bffee-a40b-4347-ac70-c210eb8bc73a"", ""path"": ""<Keyboard>/s"", ""groups"": ""Keyboard&Mouse"", ""action"": ""Move"", ""isComposite"": false, ""isPartOfComposite"": true },
                { ""name"": ""left"", ""id"": ""d2581a9b-1d11-4566-b27d-b92aff5fabbc"", ""path"": ""<Keyboard>/a"", ""groups"": ""Keyboard&Mouse"", ""action"": ""Move"", ""isComposite"": false, ""isPartOfComposite"": true },
                { ""name"": ""right"", ""id"": ""fcfe95b8-67b9-4526-84b5-5d0bc98d6400"", ""path"": ""<Keyboard>/d"", ""groups"": ""Keyboard&Mouse"", ""action"": ""Move"", ""isComposite"": false, ""isPartOfComposite"": true },
                { ""name"": """", ""id"": ""978bfe49-cc26-4a3d-ab7b-7d7a29327403"", ""path"": ""<Gamepad>/leftStick"", ""groups"": "";Gamepad"", ""action"": ""Move"", ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """", ""id"": ""eb40bb66-4559-4dfa-9a2f-820438abb426"", ""path"": ""<Keyboard>/space"", ""groups"": ""Keyboard&Mouse"", ""action"": ""Jump"", ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """", ""id"": ""daba33a1-ad0c-4742-a909-43ad1cdfbeb6"", ""path"": ""<Gamepad>/buttonSouth"", ""groups"": ""Gamepad"", ""action"": ""Jump"", ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """", ""id"": ""YOUR_UNIQUE_BINDING_ID"", ""path"": ""<Keyboard>/tab"", ""groups"": ""Keyboard&Mouse"", ""action"": ""CycleCamera"", ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """", ""id"": ""ANOTHER_UNIQUE_BINDING_ID"", ""path"": ""<Gamepad>/rightShoulder"", ""groups"": ""Gamepad"", ""action"": ""CycleCamera"", ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """", ""id"": ""143bb1cd-cc10-4eca-a2f0-a3664166fe91"", ""path"": ""<Gamepad>/buttonWest"", ""groups"": "";Gamepad"", ""action"": ""Attack"", ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """", ""id"": ""05f6913d-c316-48b2-a6bb-e225f14c7960"", ""path"": ""<Mouse>/leftButton"", ""groups"": "";Keyboard&Mouse"", ""action"": ""Attack"", ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """", ""id"": ""KEYBOARD_SCORE_BINDING"", ""path"": ""<Keyboard>/e"", ""interactions"": """", ""processors"": """", ""groups"": ""Keyboard&Mouse"", ""action"": ""Score"", ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """", ""id"": ""GAMEPAD_SCORE_BINDING"", ""path"": ""<Gamepad>/buttonNorth"", ""interactions"": """", ""processors"": """", ""groups"": ""Gamepad"", ""action"": ""Score"", ""isComposite"": false, ""isPartOfComposite"": false }
            ]
        }
    ],
    ""controlSchemes"": [ { ""name"": ""Keyboard&Mouse"", ""bindingGroup"": ""Keyboard&Mouse"", ""devices"": [ { ""devicePath"": ""<Keyboard>"", ""isOptional"": false, ""isOR"": false }, { ""devicePath"": ""<Mouse>"", ""isOptional"": false, ""isOR"": false } ] }, { ""name"": ""Gamepad"", ""bindingGroup"": ""Gamepad"", ""devices"": [ { ""devicePath"": ""<Gamepad>"", ""isOptional"": false, ""isOR"": false } ] } ]
}");
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
        m_Player_Jump = m_Player.FindAction("Jump", throwIfNotFound: true);
        m_Player_CycleCamera = m_Player.FindAction("CycleCamera", throwIfNotFound: true);
        m_Player_Attack = m_Player.FindAction("Attack", throwIfNotFound: true);
        m_Player_Score = m_Player.FindAction("Score", throwIfNotFound: true);
    }
    public void Dispose() { UnityEngine.Object.Destroy(asset); }
    public InputBinding? bindingMask { get => asset.bindingMask; set => asset.bindingMask = value; }
    public ReadOnlyArray<InputDevice>? devices { get => asset.devices; set => asset.devices = value; }
    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;
    public bool Contains(InputAction action) { return asset.Contains(action); }
    public IEnumerator<InputAction> GetEnumerator() { return asset.GetEnumerator(); }
    IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    public void Enable() { asset.Enable(); }
    public void Disable() { asset.Disable(); }
    public IEnumerable<InputBinding> bindings => asset.bindings;
    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false) { return asset.FindAction(actionNameOrId, throwIfNotFound); }
    public int FindBinding(InputBinding bindingMask, out InputAction action) { return asset.FindBinding(bindingMask, out action); }
    private readonly InputActionMap m_Player;
    private List<IPlayerActions> m_PlayerActionsCallbackInterfaces = new List<IPlayerActions>();
    private readonly InputAction m_Player_Move;
    private readonly InputAction m_Player_Jump;
    private readonly InputAction m_Player_CycleCamera;
    private readonly InputAction m_Player_Attack;
    private readonly InputAction m_Player_Score;
    public struct PlayerActions
    {
        private @InputSystem_Actions m_Wrapper;
        public PlayerActions(@InputSystem_Actions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Player_Move;
        public InputAction @Jump => m_Wrapper.m_Player_Jump;
        public InputAction @CycleCamera => m_Wrapper.m_Player_CycleCamera;
        public InputAction @Attack => m_Wrapper.m_Player_Attack;
        public InputAction @Score => m_Wrapper.m_Player_Score;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void AddCallbacks(IPlayerActions instance)
        {
            if (instance == null || m_Wrapper.m_PlayerActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove; @Move.performed += instance.OnMove; @Move.canceled += instance.OnMove;
            @Jump.started += instance.OnJump; @Jump.performed += instance.OnJump; @Jump.canceled += instance.OnJump;
            @CycleCamera.started += instance.OnCycleCamera; @CycleCamera.performed += instance.OnCycleCamera; @CycleCamera.canceled += instance.OnCycleCamera;
            @Attack.started += instance.OnAttack; @Attack.performed += instance.OnAttack; @Attack.canceled += instance.OnAttack;
            @Score.started += instance.OnScore; @Score.performed += instance.OnScore; @Score.canceled += instance.OnScore;
        }
        private void UnregisterCallbacks(IPlayerActions instance)
        {
            @Move.started -= instance.OnMove; @Move.performed -= instance.OnMove; @Move.canceled -= instance.OnMove;
            @Jump.started -= instance.OnJump; @Jump.performed -= instance.OnJump; @Jump.canceled -= instance.OnJump;
            @CycleCamera.started -= instance.OnCycleCamera; @CycleCamera.performed -= instance.OnCycleCamera; @CycleCamera.canceled -= instance.OnCycleCamera;
            @Attack.started -= instance.OnAttack; @Attack.performed -= instance.OnAttack; @Attack.canceled -= instance.OnAttack;
            @Score.started -= instance.OnScore; @Score.performed -= instance.OnScore; @Score.canceled -= instance.OnScore;
        }
        public void RemoveCallbacks(IPlayerActions instance) { if (m_Wrapper.m_PlayerActionsCallbackInterfaces.Remove(instance)) UnregisterCallbacks(instance); }
        public void SetCallbacks(IPlayerActions instance)
        {
            foreach (var item in m_Wrapper.m_PlayerActionsCallbackInterfaces) UnregisterCallbacks(item);
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnCycleCamera(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
        void OnScore(InputAction.CallbackContext context);
    }
}
