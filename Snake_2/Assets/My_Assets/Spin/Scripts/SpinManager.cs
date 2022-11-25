using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EasyUI.PickerWheelUI;

[System.Serializable]
public class PopupItems
{
    public GameObject popupPanel;
    public Text ammountText;
    public Button ok_Button;
}
[System.Serializable]
public class SkinManager
{
    [Header("For Background")]
    public Sprite background;
    [Header("Spin Wheel Items")]
    public Sprite circle_Bg;
    public Sprite spin_Circle;
    public Sprite indicator;
    public Sprite middleCircle_Bg;
    public Sprite middleCircle;
    [Space]
    public Sprite spinButton_Sprite;
    public Sprite leftSpin_Bg;
    public Sprite backButton_Bg;
    public Sprite back_Sprite;
    public Sprite character;
}
public class SpinManager : MonoBehaviour
{
    [SerializeField] SkinManager[] skins;

    [SerializeField] Text[] aommuntText;
    [SerializeField] PopupItems popupItems;
    [SerializeField] Text spinCount_Text;
    string firstSpinKey = "firstSpinKey";

    [SerializeField] private Button uiSpinButton;
    [SerializeField] private Text uiSpinButtonText;

    [SerializeField] private PickerWheel pickerWheel;
    Spin_MusicManager musicManager; 
    [Header("Skin Images")]
    [Header("For Background")]
    public Image background;
    [Header("Spin Wheel Items")]
    public Image circle_Bg;
    public Image spin_Circle;
    public Image indicator;
    public Image middleCircle_Bg;
    public Image middleCircle;
    [Space]
    public Image spinButton_Sprite;
    public Image leftSpin_Bg;
    public Image backButton_Bg;
    public Image back_Sprite;
    public Image music_bg;
    public Image character;
    public Image coinBg;
    // Start is called before the first frame update
    void Start()
    {
        #region GIVE SPIN COUNT
        if (!PlayerPrefs.HasKey(firstSpinKey))
        {
            Game.TotalSpinCount = 30;
            PlayerPrefs.SetString(firstSpinKey, firstSpinKey);
        }
        #endregion
        musicManager = FindObjectOfType<Spin_MusicManager>();
        if (popupItems.ok_Button) { popupItems.ok_Button.onClick.AddListener(ClosePopup); }
        UpdateUI();
        ClosePopup();
        AssignSpinButton();
        
    }
    public void ActivateSkin(int index)
    {
        background.sprite=skins[index].background;
       
        circle_Bg.sprite = skins[index].backButton_Bg; 
        spin_Circle.sprite = skins[index].spin_Circle; 
        indicator.sprite = skins[index].indicator; 
        middleCircle_Bg.sprite = skins[index].middleCircle_Bg; 
        middleCircle.sprite = skins[index].middleCircle; 

        spinButton_Sprite.sprite = skins[index].spinButton_Sprite; 
        leftSpin_Bg.sprite = skins[index].leftSpin_Bg; 
        backButton_Bg.sprite = skins[index].backButton_Bg; 
        back_Sprite.sprite = skins[index].back_Sprite;
        music_bg.sprite = skins[index].backButton_Bg;
        coinBg.sprite = skins[index].leftSpin_Bg;
        if (skins[index].character) 
        { 
            character.sprite = skins[index].character;
            character.sprite = null;
            var tempColor = character.color;
            tempColor.a = 1f;
            character.color = tempColor;
        }
        else
        {
            character.sprite = null;
            var tempColor = character.color;
            tempColor.a = 0f;
            character.color = tempColor;            
        }
    }
    void AssignSpinButton()
    {
        uiSpinButton.onClick.AddListener(() => {

            uiSpinButton.interactable = false;
            uiSpinButtonText.text = "Spinning";

            pickerWheel.OnSpinEnd(wheelPiece => {
                Game.TotalSpinCount--;
                Game.TotalCoins += wheelPiece.Amount;
                ShowPopup(wheelPiece.Amount);

                //Debug.Log(
                //   @" <b>Index:</b> " + wheelPiece.Index + "           <b>Label:</b> " + wheelPiece.Label
                //   + "\n <b>Amount:</b> " + wheelPiece.Amount + "      <b>Chance:</b> " + wheelPiece.Chance + "%"
                //);

                uiSpinButton.interactable = true;
                uiSpinButtonText.text = "SPIN NOW";
            });

            pickerWheel.Spin();

        });
    }
    void ClosePopup()
    {
        popupItems.popupPanel.SetActive(false);
        if (pickerWheel.spinButtonAnimator) { pickerWheel.spinButtonAnimator.enabled = true; }
        musicManager.PlayMusic();
        AdmobAdmanager.Instance.ShowInterstitial();

    }
    public void ShowPopup(int ammount)
    {
        popupItems.popupPanel.SetActive(true);
        popupItems.ammountText.text = ammount.ToString();
        UpdateUI();
        musicManager.StopMusic();
        StartCoroutine(ShowInterstitial(5f));
    }
    void UpdateUI()
    {
        if (aommuntText.Length > 0)
        {
            for (int i = 0; i < aommuntText.Length; i++)
            {
                aommuntText[i].text = Game.TotalCoins.ToString();
            }
        }
        spinCount_Text.text = Game.TotalSpinCount.ToString();
    }
    IEnumerator ShowInterstitial(float wait)
    {
        yield return new WaitForSeconds(wait);
        AdmobAdmanager.Instance.ShowInterstitial();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
