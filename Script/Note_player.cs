using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Note_player : MonoBehaviour {

    public GameObject PlayerOBJ;
    public Player Player;//ノート速度を決めるために必要

    public GameObject N_note_OBJ;
    public N_Note_ObjectPool N_note_objP;//オブジェクトを消すために必要
    public Tween Tween;
   


    //public AudioSource Clap;//パフェタイミングでクラップを鳴らすためのテスト用。これ関係も後でなくす

    
        
    void Start () {
        /*
        PlayerOBJ = GameObject.Find("Player");
        //Clap = PlayerOBJ.GetComponent<AudioSource>();
        Player = PlayerOBJ.GetComponent<Player>();
        */
        N_note_OBJ = GameObject.Find("N_NoteObjectPool");
        N_note_objP = N_note_OBJ.GetComponent<N_Note_ObjectPool>();
       
        
    }
	
	
	

    void OnEnable()
    {
        
        PlayerOBJ = GameObject.Find("Player");
        //Clap = PlayerOBJ.GetComponent<AudioSource>();
        Player = PlayerOBJ.GetComponent<Player>();
/*
        N_note_OBJ = GameObject.Find("N_NoteObjectPool");
        N_note_objP = N_note_OBJ.GetComponent<N_Note_ObjectPool>();
        */

        Tween =  transform.DOLocalMoveY(-7.81f, (Player.float_steam_time* 1.1863f)).SetEase(Ease.Linear).OnComplete(des);
        
    }

   

    void des()
    {

        N_note_objP.releaseNote(gameObject,1);
    }


    



}
