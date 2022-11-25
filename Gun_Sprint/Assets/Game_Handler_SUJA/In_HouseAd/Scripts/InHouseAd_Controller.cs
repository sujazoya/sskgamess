using SimpleJSON;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Video;
using UnityEngine.UI;

public class InHouseAd_Controller : MonoBehaviour
{
   
    public static bool show_ad_as_index;

    #region get videos
    public static string  ad_1    ;
    public static string  ad_2    ;
    public static string  ad_3    ;
    public static string  ad_4    ;
    public static string  ad_5    ;
    public static string  ad_6    ;
    public static string  ad_7    ;
    public static string  ad_8    ;
    public static string  ad_9    ;
    public static string  ad_10    ;
    public static string  ad_11    ;
    public static string  ad_12    ;
    public static string  ad_13    ;
    public static string  ad_14    ;
    public static string  ad_15    ;
    public static string  ad_16    ;
    public static string  ad_17    ;
    public static string  ad_18    ;
    public static string  ad_19    ;
    public static string  ad_20    ;
    public static string  ad_21    ;
    public static string  ad_22    ;
    public static string  ad_23    ;
    public static string  ad_24    ;
    public static string  ad_25    ;
    public static string  ad_26    ;
    public static string  ad_27    ;
    public static string  ad_28    ;
    public static string  ad_29    ;
    public static string  ad_30    ;
    public static string  ad_31    ;
    public static string  ad_32    ;
    public static string  ad_33    ;
    public static string  ad_34    ;
    public static string  ad_35    ;
    public static string  ad_36    ;
    public static string  ad_37    ;
    public static string  ad_38    ;
    public static string  ad_39    ;
    public static string  ad_40    ;
    public static string  ad_41    ;
    public static string  ad_42    ;
    public static string  ad_43    ;
    public static string  ad_44    ;
    public static string  ad_45    ;
    public static string  ad_46    ;
    public static string  ad_47    ;
    public static string  ad_48    ;
    public static string  ad_49    ;
    public static string  ad_50    ;
    #endregion
    #region get app_link_
    public static string  ad_1_app_link;
    public static string  ad_2_app_link;
    public static string  ad_3_app_link;
    public static string  ad_4_app_link;
    public static string  ad_5_app_link;
    public static string  ad_6_app_link;
    public static string  ad_7_app_link;
    public static string  ad_8_app_link;
    public static string  ad_9_app_link;
    public static string ad_10_app_link;
    public static string ad_11_app_link;
    public static string ad_12_app_link;
    public static string ad_13_app_link;
    public static string ad_14_app_link;
    public static string ad_15_app_link;
    public static string ad_16_app_link;
    public static string ad_17_app_link;
    public static string ad_18_app_link;
    public static string ad_19_app_link;
    public static string ad_20_app_link;
    public static string ad_21_app_link;
    public static string ad_22_app_link;
    public static string ad_23_app_link;
    public static string ad_24_app_link;
    public static string ad_25_app_link;
    public static string ad_26_app_link;
    public static string ad_27_app_link;
    public static string ad_28_app_link;
    public static string ad_29_app_link;
    public static string ad_30_app_link;
    public static string ad_31_app_link;
    public static string ad_32_app_link;
    public static string ad_33_app_link;
    public static string ad_34_app_link;
    public static string ad_35_app_link;
    public static string ad_36_app_link;
    public static string ad_37_app_link;
    public static string ad_38_app_link;
    public static string ad_39_app_link;
    public static string ad_40_app_link;
    public static string ad_41_app_link;
    public static string ad_42_app_link;
    public static string ad_43_app_link;
    public static string ad_44_app_link;
    public static string ad_45_app_link;
    public static string ad_46_app_link;
    public static string ad_47_app_link;
    public static string ad_48_app_link;
    public static string ad_49_app_link;
    public static string ad_50_app_link;
    #endregion

    public static int ad_counts_we_have;




    public static string localInfo;
    private static bool BooleanChecker(string value)
    {
        string tmpstring = value.ToLower();
        if (tmpstring == "true") return true;
        else return false;
    }
    public  void GetVideosFromAPI(string snapshot)
    {
        JSONNode appLink_info              = JSON.Parse(snapshot)[0];
        JSONNode itle_info                 = JSON.Parse(snapshot)[1];
        JSONNode description_info          = JSON.Parse(snapshot)[2];
        JSONNode logoLink_info             = JSON.Parse(snapshot)[3];
        JSONNode image1Link_info           = JSON.Parse(snapshot)[4];
        JSONNode image2Link_info           = JSON.Parse(snapshot)[5];
        JSONNode image3Link_info           = JSON.Parse(snapshot)[6];
        JSONNode image4Link_info           = JSON.Parse(snapshot)[7];
        ad_counts_we_have =int.Parse(appLink_info["ad_counts_we_have"].Value.ToString());

        GameObject InHouseAds = new GameObject();
        InHouseAds.name = "InHouseAds";
        for (int i = 0; i < ad_counts_we_have; i++)
        {
            GameObject myAd = new GameObject();        
            int num = i ++;
            myAd.name = "myAd" + num.ToString();
            InHouse_Ad newAd     = myAd.AddComponent<InHouse_Ad>();
            newAd.appLink        = appLink_info         ["ad_"+num.ToString()].Value.ToString();
            newAd.appTitle       = itle_info            ["ad_" + num.ToString()].Value.ToString();
            newAd.Description    = description_info     ["ad_" + num.ToString()].Value.ToString();
            newAd.appLogo_Link        = logoLink_info        ["ad_" + num.ToString()].Value.ToString();
            newAd.image1_Link         = image1Link_info      ["ad_" + num.ToString()].Value.ToString();
            newAd.image2_Link         = image2Link_info      ["ad_" + num.ToString()].Value.ToString();
            newAd.image3_Link         = image3Link_info      ["ad_" + num.ToString()].Value.ToString();
            newAd.image4_Link         = image4Link_info      ["ad_" + num.ToString()].Value.ToString();
            myAd.transform.parent = InHouseAds.transform;
        }

        //ad_1 = appLink_info["ad_1"].Value.ToString();
        //ad_2  = appLink_info["ad_2"].Value.ToString();
        //ad_3  = appLink_info["ad_3"].Value.ToString();
        //ad_4  = appLink_info["ad_4"].Value.ToString();
        //ad_5  = appLink_info["ad_5"].Value.ToString();
        //ad_6  = appLink_info["ad_6"].Value.ToString();
        //ad_7  = appLink_info["ad_7"].Value.ToString();
        //ad_8  = appLink_info["ad_8"].Value.ToString();
        //ad_9  = appLink_info["ad_9"].Value.ToString();
        //ad_10 = appLink_info["ad_10"].Value.ToString();
        //ad_11 = appLink_info["ad_11"].Value.ToString();
        //ad_12 = appLink_info["ad_12"].Value.ToString();
        //ad_13 = appLink_info["ad_13"].Value.ToString();
        //ad_14 = appLink_info["ad_14"].Value.ToString();
        //ad_15 = appLink_info["ad_15"].Value.ToString();
        //ad_16 = appLink_info["ad_16"].Value.ToString();
        //ad_17 = appLink_info["ad_17"].Value.ToString();
        //ad_18 = appLink_info["ad_18"].Value.ToString();
        //ad_19 = appLink_info["ad_19"].Value.ToString();
        //ad_20 = appLink_info["ad_20"].Value.ToString();
        //ad_21 = appLink_info["ad_21"].Value.ToString();
        //ad_22 = appLink_info["ad_22"].Value.ToString();
        //ad_23 = appLink_info["ad_23"].Value.ToString();
        //ad_24 = appLink_info["ad_24"].Value.ToString();
        //ad_25 = appLink_info["ad_25"].Value.ToString();
        //ad_26 = appLink_info["ad_26"].Value.ToString();
        //ad_27 = appLink_info["ad_27"].Value.ToString();
        //ad_28 = appLink_info["ad_28"].Value.ToString();
        //ad_29 = appLink_info["ad_29"].Value.ToString();
        //ad_30 = appLink_info["ad_30"].Value.ToString();
        //ad_31 = appLink_info["ad_31"].Value.ToString();
        //ad_32 = appLink_info["ad_32"].Value.ToString();
        //ad_33 = appLink_info["ad_33"].Value.ToString();
        //ad_34 = appLink_info["ad_34"].Value.ToString();
        //ad_35 = appLink_info["ad_35"].Value.ToString();
        //ad_36 = appLink_info["ad_36"].Value.ToString();
        //ad_37 = appLink_info["ad_37"].Value.ToString();
        //ad_38 = appLink_info["ad_38"].Value.ToString();
        //ad_39 = appLink_info["ad_39"].Value.ToString();
        //ad_40 = appLink_info["ad_40"].Value.ToString();
        //ad_41 = appLink_info["ad_41"].Value.ToString();
        //ad_42 = appLink_info["ad_42"].Value.ToString();
        //ad_43 = appLink_info["ad_43"].Value.ToString();
        //ad_44 = appLink_info["ad_44"].Value.ToString();
        //ad_45 = appLink_info["ad_45"].Value.ToString();
        //ad_46 = appLink_info["ad_46"].Value.ToString();
        //ad_47 = appLink_info["ad_47"].Value.ToString();
        //ad_48 = appLink_info["ad_48"].Value.ToString();
        //ad_49 = appLink_info["ad_49"].Value.ToString();
        //ad_50 = appLink_info["ad_50"].Value.ToString();
        //InHouseAdManager.Instance.names = ad_50;      

        //show_loading = BooleanChecker(info["show_loading"].Value);
        //googlesheetInitilized = true;       

    }
    void CreateAds(int count)
    {
        
    }
   
}
