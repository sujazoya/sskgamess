using UnityEngine;
using System.Collections;

public class exitScript : menuScript {

	// don't use Start - it will override start from menuscript
	// Use this for initialization
	//void Start () {
	//}
	
	// don't use Update - it will override update from menuscript
	// Update is called once per frame
	//void Update () {
	//}

	//
	// void OnMouseDown()
	//
	// If exit is clicked, the app is left
	//

	void OnMouseDown() {
		Debug.Log ("Shutting down Application");
		Application.Quit ();
	}
}
