using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
public class Hud_Player : MonoBehaviour
{
    Button button;
    string on = "on";
    string off = "off";
    bool active;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        active = !active;
        if (!active)
        {
            animator.SetTrigger(off);
        }
        else
        {           
            animator.SetTrigger(on);
        }
    }
}
