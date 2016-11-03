using UnityEngine;
using System.Collections;

public class judge_string_des : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(des());
    }

    IEnumerator des()
    {
        //Debug.Log("コルーチン");
        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);

    }
}
