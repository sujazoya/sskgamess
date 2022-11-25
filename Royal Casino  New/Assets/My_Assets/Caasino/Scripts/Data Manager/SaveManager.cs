using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class SaveManager : MonoBehaviour
{
    public InputField saveName;
    public GameObject loadArea;
    public GameObject loadButtonPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnSave()
    {
        SerializationManger.Save(saveName.text, SaveData.Instance);
    }
    public string[] saveFiles;
    public void GetLoadFiles()
    {
        if(!Directory.Exists(Application.persistentDataPath + "/saves/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves/");
        }
        saveFiles=Directory.GetFiles(Application.persistentDataPath + "/saves/");
    }
    public void ShowLoadScreen()
    {
        GetLoadFiles();
        GameObject[] childButtons = loadArea.GetComponentsInChildren<GameObject>();
        foreach (var button in childButtons)
        {
            Destroy(button.gameObject);
        }
        for (int i = 0; i < saveFiles.Length; i++)
        {
            GameObject buttonObject = Instantiate(loadButtonPrefab);
            buttonObject.transform.SetParent(loadArea.transform, false);

            var index = i;
            buttonObject.GetComponent<Button>().onClick.AddListener(() =>
            {
               //PlayerManager.instance.OnLoade(saveFiles[index]);
            });
            buttonObject.GetComponentInChildren<Text>().text = saveFiles[index].Replace(Application.persistentDataPath + "/saves/", "");
        }

    }
}
