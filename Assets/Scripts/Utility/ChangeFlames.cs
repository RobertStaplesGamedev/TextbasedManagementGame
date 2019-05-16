using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFlames : StateMachineBehaviour
{
    void OnStateEnter(Animator animator) {
        animator.GetComponent<Transform>().GetChild(0).gameObject.SetActive(true);
    }

    void OnStateExit(Animator animator) {
        animator.GetComponent<Transform>().GetChild(0).gameObject.SetActive(false);
    }
}
