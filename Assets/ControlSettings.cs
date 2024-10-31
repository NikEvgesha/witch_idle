using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlSettings : MonoBehaviour
{
    [SerializeField] private Toggle _keyboardToggle;
    [SerializeField] private Toggle _joystickToggle;
    private ControlType _currentControlType;
    private ControlType _newControlType;

    public Action<ControlType> ControlSwitch;

    private void Start()
    {
        _currentControlType = ControlType.Joystick;
    }
    public void onToggleControl()
    {
        if (_keyboardToggle.isOn)
        {
            _newControlType = ControlType.Keyboard;
        } else if (_joystickToggle.isOn)
        {
            _newControlType = ControlType.Joystick;
        }
        if (_newControlType != _currentControlType)
        {
            _currentControlType = _newControlType;
            EventManager.ControlSwitch?.Invoke(_newControlType);
        }

    }

}
