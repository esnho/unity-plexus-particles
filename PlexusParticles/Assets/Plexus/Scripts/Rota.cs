using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rota : MonoBehaviour {
	
	public Vector3 vel = new Vector3( 6f, 12f, 0f);
	
	// Update is called once per frame
	void Update () {
		Vector3 tmp = transform.rotation.eulerAngles;
		tmp += Time.deltaTime * vel;
        
		transform.Rotate (vel * Time.deltaTime);// = Quaternion.Euler(tmp.x, tmp.y, tmp.z);
	}
}
