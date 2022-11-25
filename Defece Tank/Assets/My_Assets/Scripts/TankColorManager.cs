using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankColorManager : MonoBehaviour
{
    [SerializeField] Color[] colors;
    [SerializeField] Material tankMat;
    // Start is called before the first frame update
    void Start()
    {
        int index = Random.Range(0, colors.Length);
        ChangeTankColor(index);
        GameManager_Defence.onGameStart += OnGameStart;
    }
    public void ChangeTankColor(int colorIncex)
    {
        if (tankMat)
        {
            tankMat.color = colors[colorIncex];
        }
    }
    void OnGameStart()
    {
        switch (Game.EnvTypes)
        {
            case 1:
                if (tankMat)  { tankMat.color = colors[0]; }
                break;
            case 2:
                if (tankMat) { tankMat.color = colors[1]; }
                break;
            case 3:
                if (tankMat) { tankMat.color = colors[2]; }
                break;
        }
    }
    private void OnDestroy()
    {
        GameManager_Defence.onGameStart -= OnGameStart;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
