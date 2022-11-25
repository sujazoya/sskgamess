using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetItemWithNameFromList : MonoBehaviour
{
    public Text inputField; // Drag & drop the gameObject with the Text component in the inspector
    public List<GameObject> objects;

    public Transform player;

    public void Creator_Input(string name)
    {
        int objectIndex = objects.FindIndex(gameObject => string.Equals(name, gameObject.name));
        if (objectIndex >= 0)
            Instantiate(objects[objectIndex], new Vector3(player.position.x, player.position.y, player.position.z - 2), Quaternion.identity);
    }
}
