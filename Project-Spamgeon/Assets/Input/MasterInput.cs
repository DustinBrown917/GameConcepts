// GENERATED AUTOMATICALLY FROM 'Assets/Input/MasterInput.inputactions'

using System;
using UnityEngine;
using UnityEngine.Experimental.Input;


[Serializable]
public class MasterInput : InputActionAssetReference
{
    public MasterInput()
    {
    }
    public MasterInput(InputActionAsset asset)
        : base(asset)
    {
    }
    private bool m_Initialized;
    private void Initialize()
    {
        // MainAction
        m_MainAction = asset.GetActionMap("MainAction");
        m_MainAction_SpamTest = m_MainAction.GetAction("SpamTest");
        m_Initialized = true;
    }
    private void Uninitialize()
    {
        m_MainAction = null;
        m_MainAction_SpamTest = null;
        m_Initialized = false;
    }
    public void SetAsset(InputActionAsset newAsset)
    {
        if (newAsset == asset) return;
        if (m_Initialized) Uninitialize();
        asset = newAsset;
    }
    public override void MakePrivateCopyOfActions()
    {
        SetAsset(ScriptableObject.Instantiate(asset));
    }
    // MainAction
    private InputActionMap m_MainAction;
    private InputAction m_MainAction_SpamTest;
    public struct MainActionActions
    {
        private MasterInput m_Wrapper;
        public MainActionActions(MasterInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @SpamTest { get { return m_Wrapper.m_MainAction_SpamTest; } }
        public InputActionMap Get() { return m_Wrapper.m_MainAction; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled { get { return Get().enabled; } }
        public InputActionMap Clone() { return Get().Clone(); }
        public static implicit operator InputActionMap(MainActionActions set) { return set.Get(); }
    }
    public MainActionActions @MainAction
    {
        get
        {
            if (!m_Initialized) Initialize();
            return new MainActionActions(this);
        }
    }
    private int m_KeyboardSchemeIndex = -1;
    public InputControlScheme KeyboardScheme
    {
        get

        {
            if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.GetControlSchemeIndex("Keyboard");
            return asset.controlSchemes[m_KeyboardSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get

        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.GetControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
}
