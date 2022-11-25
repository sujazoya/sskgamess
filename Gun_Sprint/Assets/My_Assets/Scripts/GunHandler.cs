using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunHandler : MonoBehaviour
{
    [SerializeField] string gunID;
    [SerializeField] int gunPrice;
    [SerializeField] Text gunPrice_Text;
    GameObject lock_;
    Button button_;
    Text buttonText;
    [SerializeField] int gunIndex;
    [SerializeField] Text gunNameText;
    // Start is called before the first frame update
    void OnEnable()
    {
        button_ = GetComponent<Button>();
        lock_ = transform.Find("lock").gameObject;
        buttonText = transform.Find("Text").GetComponent<Text>();
        gunNameText.text = gunID;
        gunPrice_Text.text = gunPrice.ToString();
        CheckGunStatus();
    }

    // Update is called once per frame
    void CheckGunStatus()
    {
        if (!GameManager_Sprint.Instance.test_build)
        {
            if (!PlayerPrefs.HasKey(gunID))
            {
                button_.onClick.RemoveAllListeners();
                button_.onClick.AddListener(BuyGun);
                buttonText.text = "Buy";
            }
            else
            {
                button_.onClick.RemoveAllListeners();
                button_.onClick.AddListener(ActivateGun);
                lock_.SetActive(false);
                buttonText.text = "Activate";
            }
        }
        else
        {
            button_.onClick.RemoveAllListeners();
            button_.onClick.AddListener(ActivateGun);
            lock_.SetActive(false);
            buttonText.text = "Activate";
        }
    }
    void ActivateGun()
    {
        Game.CurrentGun = gunIndex;
        GameManager_Sprint.Instance.CloseGunPanel();
    }
    void BuyGun()
    {
        if (!GameManager_Sprint.Instance.test_build)
        {
            if (Game.TotalCoins >= gunPrice)
            {
                Game.TotalCoins -= gunPrice;
                PlayerPrefs.SetString(gunID, gunID);
                lock_.SetActive(false);
                button_.onClick.RemoveAllListeners();
                button_.onClick.AddListener(ActivateGun);
                lock_.SetActive(false);
                buttonText.text = "Activate";
            }
            else
            {
                MessegeManager.insance.ShowMassege("Coin Shortage","You Don't Have Enough Coins",false);
            }
        }
        else 
        {
            PlayerPrefs.SetString(gunID, gunID);
            lock_.SetActive(false);
            button_.onClick.RemoveAllListeners();
            button_.onClick.AddListener(ActivateGun);
            lock_.SetActive(false);
            buttonText.text = "Activate";
        }
    }
}
