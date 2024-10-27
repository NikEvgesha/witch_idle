using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public FloatingJoystick joystick;
    public float walkSpeed;

    public Animator animator;
    private float walkAnimSmooth;
    void Start()
    {
        joystick = FindObjectOfType<FloatingJoystick>();
    }

    void Update()
    {
        walkAnimSmooth = Mathf.Lerp(walkAnimSmooth, joystick.Direction.magnitude, 10 * Time.deltaTime);
        transform.position += new Vector3(joystick.Horizontal, 0, joystick.Vertical) * walkSpeed * Time.deltaTime;
        animator.SetFloat("Movement",walkAnimSmooth);
        if (joystick.Direction.magnitude>.1f)
        {
            transform.forward = new Vector3(joystick.Horizontal, 0, joystick.Vertical);

        }
    }
}
