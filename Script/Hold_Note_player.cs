using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Hold_Note_player : MonoBehaviour {

    public GameObject N_note_OBJ;
    public N_Note_ObjectPool N_note_objP;

    public GameObject PlayerOBJ;
    public Player Player;//ノート速度を決めるために必要

    public Tween Moven;

    public Tween Short;

   

    void Start () {
        N_note_OBJ = GameObject.Find("N_NoteObjectPool");
        N_note_objP = N_note_OBJ.GetComponent<N_Note_ObjectPool>();
        
    }


    void OnEnable()
    {
        PlayerOBJ = GameObject.Find("Player");
        Player = PlayerOBJ.GetComponent<Player>();
        Moven = transform.DOLocalMoveY(-7.81f, (Player.float_steam_time * 1.1863f)).SetEase(Ease.Linear).OnComplete(EndShort);
        
    }



    

    public void Shorten(float HoldTime)
    {

        Short = transform.DOScaleY(0, HoldTime).SetEase(Ease.Linear).OnComplete(des);
        //Debug.Log("shorten");
    }


    void EndShort()//見逃しmissの時に実行される
    {
        transform.DOScaleY(0, (Player.float_steam_time)).SetEase(Ease.Linear).OnComplete(des);
    }


    void des()
    {

        N_note_objP.releaseNote(gameObject,3);
    }
}
