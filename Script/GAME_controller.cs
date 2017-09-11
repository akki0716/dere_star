using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// (内部的な)ゲームの設定
/// </summary>
public class GAME_controller : MonoBehaviour {

    [SerializeField]
    int FPS;

    [SerializeField]
    float GameSpeed;

   

    void Start()
    {
        Application.targetFrameRate = FPS;
        Time.timeScale = GameSpeed;
    }


   

}
