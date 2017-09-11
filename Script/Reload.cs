using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Reload : MonoBehaviour {
    string sceneName;


    [SerializeField]
    GameObject Dotween;


    // Use this for initialization
    void Start () {
       // sceneName = SceneManager.GetActiveScene().name;
    }
	

    public void Reset_botton()
    {
        //Debug.Log("押した");
        //Dotween = GameObject.Find("Dotween");
        //Destroy(Dotween);
        UnityEngine.SceneManagement.SceneManager.LoadScene("playtest");
    }
}
