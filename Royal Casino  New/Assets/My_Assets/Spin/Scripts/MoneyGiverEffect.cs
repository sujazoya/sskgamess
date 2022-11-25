using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyGiverEffect : MonoBehaviour
{
    public GameObject mpneyGiver;
    public Text name_;
    public Text addingText;
    public Text ammount;
    public GameObject[] givverEffect;
    public GameObject awardEffect;
    public GameObject starEffect;
    public Text flashText;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    IEnumerator StopMoneyGiving()
    {
        MusicManager.PauseMusic(0.5f);
        giveMoney = false;
        for (int i = 0; i < givverEffect.Length; i++)
        {
            givverEffect[i].SetActive(false);
        }
       flashText.gameObject.SetActive(true);
       starEffect.gameObject.SetActive(true);
       flashText.text = "2000";
       ammount.text = "Credits";
        yield return new WaitForSeconds(2);
        flashText.gameObject.SetActive(false);
        ammount.gameObject.SetActive(false);
        starEffect.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        flashText.gameObject.SetActive(true);
        ammount.gameObject.SetActive(true);
        starEffect.gameObject.SetActive(true);
        flashText.text = "500";
        ammount.text = "Robux";
        yield return new WaitForSeconds(2);
        flashText.gameObject.SetActive(false);
        ammount.gameObject.SetActive(false);
        starEffect.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        flashText.gameObject.SetActive(true);
        ammount.gameObject.SetActive(true);
        starEffect.gameObject.SetActive(true);
        flashText.text = "10";
        ammount.text = "Diemonds";
        yield return new WaitForSeconds(2);
       flashText.gameObject.SetActive(false);
       ammount.gameObject.SetActive(false);
       starEffect.gameObject.SetActive(false);
        yield return new WaitForSeconds(2);
        mpneyGiver.SetActive(false);       
        RewardedAdManager.Instance.ShowRewardedAd();
        //UpdatePlayerMoney();
    }
    bool giveMoney;
    int playerMoneyGiven;
    public void GiveMoney(string names)
    {        
        mpneyGiver.SetActive(true);
        for (int i = 0; i < givverEffect.Length; i++)
        {
            givverEffect[i].SetActive(true);
        }
        ammount.gameObject.SetActive(true);
        name_.text = names;
        givingAmmount = 0;
        ammount.text = givingAmmount.ToString();
        StartCoroutine(ProccessGiveMoney());
    }
    private float givingAmmount;

    private void LateUpdate()
    {
        if (giveMoney && givingAmmount < 2000)
        {
            givingAmmount += Time.deltaTime * 100;
            ammount.text = givingAmmount.ToString();
            if (givingAmmount >= 2000 && giveMoney)
            {
                giveMoney = false;
                StartCoroutine(StopMoneyGiving());
            }
        }
    }
    IEnumerator ProccessGiveMoney()
    {      
        yield return new WaitForSeconds(2f);
        giveMoney = true;
    }
}
