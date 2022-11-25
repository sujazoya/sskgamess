using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot_Trigger : MonoBehaviour
{
    public bool timeToCheck;
    SlotManager manager;
    Slot_Piece slot_Piece;
    [SerializeField] AudioSource slot_AudioSource;
    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<SlotManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (manager.playCasino)
        {
            
            slot_Piece = other.transform.parent.GetComponent<Slot_Piece>();
            switch (other.tag)
            {
                case "bonus":
                    slot_Piece.gotItem = Slot_Piece.GotItem.Bonus;
                    break;
                case "bar":
                    slot_Piece.gotItem = Slot_Piece.GotItem.Bar;
                    break;
                case "saven":
                    slot_Piece.gotItem = Slot_Piece.GotItem.Seven;
                    break;
                case "wild":
                    slot_Piece.gotItem = Slot_Piece.GotItem.Wild;
                    break;
            }
            if (slot_AudioSource)
            {
                if (!slot_AudioSource.isPlaying)
                {
                    slot_AudioSource.Play();
                }
            }
        }
        
    }  
}
