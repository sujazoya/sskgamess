using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSoundPlayer : MonoBehaviour
{
    Button Button_;
    // Start is called before the first frame update
    void Start()
    {
        Button_ = GetComponent<Button>();
        if (Button_) { Button_.onClick.AddListener(OnButtonClick); }
    }    
    void OnButtonClick()
    {
        MusicManager.PlaySfx("button");
    }
}
