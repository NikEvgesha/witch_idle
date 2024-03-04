using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public FloatingJoystick joystick;
    public StackController stackController;
    public float walkSpeed;

    public Animator animator;
    private float walkAnimSmooth;
    void Start()
    {
        joystick = FindObjectOfType<FloatingJoystick>();
    }

    void Update()
    {
        if (stackController.stackObjects.Count!=0)
        {
            animator.SetLayerWeight(1,1);
        }
        else
        {
            animator.SetLayerWeight(1,0);

        }
        walkAnimSmooth = Mathf.Lerp(walkAnimSmooth, joystick.Direction.magnitude, 10 * Time.deltaTime);
        transform.position += new Vector3(joystick.Horizontal, 0, joystick.Vertical) * walkSpeed * Time.deltaTime;
        animator.SetFloat("Movement",walkAnimSmooth);
        if (joystick.Direction.magnitude>.1f)
        {
            transform.forward = new Vector3(joystick.Horizontal, 0, joystick.Vertical);

        }
    }
}
