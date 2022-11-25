using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]

public class AvatarButton : MonoBehaviour
{
    Button button;
    Image image;
    Sprite avatar;
    PlayerManager avatarSelecter;
    [SerializeField] int avatarIndex;
    // Start is called before the first frame update
    void Start()
    {
        avatarSelecter = FindObjectOfType<PlayerManager>();
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        avatar = image.sprite;
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        avatarSelecter.ActivateAvatar(avatar, avatarIndex);
    }
}
