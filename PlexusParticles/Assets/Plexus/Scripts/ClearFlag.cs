using UnityEngine;

public class ClearFlag : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp (KeyCode.Space)) {
			GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
            return;
		}

		if (Input.GetKeyDown (KeyCode.Space)) {
			GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
		}
	}
}
