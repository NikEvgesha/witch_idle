using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public Animator animator;

    [SerializeField] private GameObject _playerModel;

    private ControlType _controlType;
    public FloatingJoystick joystick;
    public float walkSpeed;
    private float walkAnimSmooth;
    private float magnitude;
    private Vector3 input;

    void Start()
    {
        joystick = FindObjectOfType<FloatingJoystick>();
        _controlType = ControlType.Joystick; //это нужно сохранять
    }

    private void OnEnable()
    {
        EventManager.ControlSwitch += SwitchControl;
    }

    private void OnDisable()
    {
        EventManager.ControlSwitch -= SwitchControl;
    }

    void Update()
    {
        if (_controlType == ControlType.Joystick)
        {
            input = GetJoystickInput();
            magnitude = joystick.Direction.magnitude;
        } else
        {
            input = GetKeyboardInput();
            magnitude = input.magnitude;
        }

        if (magnitude > 0f)
        {
            transform.Translate(input * Time.deltaTime * walkSpeed);
            _playerModel.transform.forward = input;
        }
        walkAnimSmooth = Mathf.Lerp(walkAnimSmooth, magnitude, 10 * Time.deltaTime);
        animator.SetFloat("Movement", walkAnimSmooth);
    }

    private void SwitchControl(ControlType type)
    {
        _controlType = type;
        if (_controlType != ControlType.Joystick)
        {
            joystick.gameObject.SetActive(false);
        } else
        {
            joystick.gameObject.SetActive(true);
        }
    }



    private Vector3 GetKeyboardInput()
    { 
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            p_Velocity += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            p_Velocity += new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            p_Velocity += new Vector3(1, 0, 0);
        }
        return p_Velocity.normalized;
    }

    private Vector3 GetJoystickInput()
    {
        return new Vector3(joystick.Horizontal, 0, joystick.Vertical);
    }
}
