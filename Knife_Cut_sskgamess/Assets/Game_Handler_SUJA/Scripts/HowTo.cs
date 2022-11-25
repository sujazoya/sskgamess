using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HowTo : MonoBehaviour
{
    Button button;
    Animator animator;
    string show = "show";
    [SerializeField] Button closeButton;
    // Start is called before the first frame update
    void Start()
    {
        button   = transform.GetComponent<Button>();
        animator = transform.GetComponent<Animator>();
        closeButton.onClick.AddListener(EnableAnimator);
        button.onClick.AddListener(Show);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
   void Show()
    {
        animator.SetTrigger(show);
    }
    public void DisableAnimator()
    {
        animator.enabled = false;
    }
    public void EnableAnimator()
    {
        animator.enabled = true;
    }
}
