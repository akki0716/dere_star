using UnityEngine;
using System.Collections;
using DG.Tweening;


/// <summary>
/// ノートを動かすためのスクリプト
/// </summary>
public class Note_player : MonoBehaviour {


    /// <summary>
    /// Note_managerが付いているノート
    /// </summary>
	public GameObject Note_managerOBJ;
	public Note_manager Note_manager;//ノート速度を決めるために必要

	public GameObject Note_ObjectPoolOBJ;
    private Note_ObjectPool Note_ObjectPool;//オブジェクトを消すために必要
    public Tween Tween;



    //public AudioSource Clap;//パフェタイミングでクラップを鳴らすためのテスト用。これ関係も後でなくす

    public float float_time;//テスト用

    void Awake()
    {
        Note_managerOBJ = GameObject.FindWithTag("Note_manager");
        Note_manager = Note_managerOBJ.GetComponent<Note_manager>();
    }



		/*
    void Start () {
		PlayerOBJ = GameObject.Find("Player");
		//Clap = PlayerOBJ.GetComponent<AudioSource>();
		Player = PlayerOBJ.GetComponent<Player>();
		//N_note_OBJ = GameObject.FindWithTag("N_NoteObjectPool");
		//N_note_objP = N_note_OBJ.GetComponent<N_Note_ObjectPool>();
	   
		
	}
	
		*/
	
	

	void OnEnable()
	{

        //PlayerOBJ = GameObject.FindWithTag("Player");
        //Clap = PlayerOBJ.GetComponent<AudioSource>();
        //Player = PlayerOBJ.GetComponent<Player>();
        /*
		N_note_OBJ = GameObject.Find("N_NoteObjectPool");
		N_note_objP = N_note_OBJ.GetComponent<N_Note_ObjectPool>();
		*/
        //Tween =  transform.DOLocalMoveY(-7.81f, (Player.float_steam_time* 1.1863f)).SetEase(Ease.Linear).OnComplete(des);
        //Debug.Log(this.gameObject.name +" "+ transform.localPosition + " " + float_time);
        
         Tween = transform.DOLocalMoveY(-7.81f, (Note_manager.float_steam_time * 1.18636363636f)).SetEase(Ease.Linear);
        
        
       
        //Tween = transform.DOLocalMoveY(-5.35f, float_time).SetEase(Ease.Linear);
        /*アニメーション自体は画面下端まで行う
		移動する速度(時間)(＝Player.float_steam_time)は判定ラインまでで計算している。
		画面上端から下端までの距離は上端から判定ラインの距離の約1.1863倍、上の1.1863fはそこから
	1.19359756
		*/




    }



	void des()
	{
		//Debug.Log("normal note des");
		//N_note_objP.releaseNote(gameObject,1);
	}





}
