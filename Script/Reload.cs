using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Reload : MonoBehaviour {
    string sceneName;
    // Use this for initialization
    void Start () {
        sceneName = SceneManager.GetActiveScene().name;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void onTapButton()
    {
        SceneManager.LoadScene(sceneName);
    }
}
