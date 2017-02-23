using UnityEngine;
using System.Collections;
using HedgehogTeam.EasyTouch;

public class etouchtest : MonoBehaviour
{
    bool swipeswitch = true   ;
    bool dragswitch = false   ;

    void OnEnable()
    {
        EasyTouch.On_TouchStart += On_TouchStart;
        if (swipeswitch == true)
        {
            EasyTouch.On_SwipeStart += On_SwipeStart;
            EasyTouch.On_Swipe += On_Swipe;
            EasyTouch.On_SwipeEnd += On_SwipeEnd;
        }
        if (dragswitch == true)
        {
            EasyTouch.On_Drag += On_Drag;
            EasyTouch.On_DragEnd += On_DragEnd;
        }
            EasyTouch.On_LongTapStart += On_LongTapStart;
        
        
    }

    void OnDisable()
    {
        EasyTouch.On_TouchStart += On_TouchStart;
        if (swipeswitch == true)
        {
            EasyTouch.On_SwipeStart += On_SwipeStart;
            EasyTouch.On_Swipe += On_Swipe;
            EasyTouch.On_SwipeEnd += On_SwipeEnd;
        }
        EasyTouch.On_LongTapStart += On_LongTapStart;
        if (dragswitch == true)
        {
            EasyTouch.On_Drag += On_Drag;
            EasyTouch.On_DragEnd += On_DragEnd;
        }

    }

    void OnDestroy()
    {
        EasyTouch.On_TouchStart += On_TouchStart;
        if (swipeswitch == true)
        {
            EasyTouch.On_SwipeStart += On_SwipeStart;
            EasyTouch.On_Swipe += On_Swipe;
            EasyTouch.On_SwipeEnd += On_SwipeEnd;
        }
        EasyTouch.On_LongTapStart += On_LongTapStart;
        if (dragswitch == true)
        {
            EasyTouch.On_Drag += On_Drag;
            EasyTouch.On_DragEnd += On_DragEnd;
        }

    }

    public void On_TouchStart(Gesture gesture)
    {
        // Verification that the action on the object
        /*
        Vector2 screenPos = gesture.position;
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos); //スクリーン座標をワールド座標に変換
        Debug.Log("タッチ座標  " + worldPos);
        */
        Vector2 Pos =  gesture.GetTouchToWorldPoint(gesture.position);//この1行で済むよ
        Debug.Log("タッチ座標  " + Pos);
    }

    public void On_SwipeStart(Gesture gesture)
    {
        // Verification that the action on the object
        Vector2 screenPos = gesture.position;
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos); //スクリーン座標をワールド座標に変換
        Debug.Log("On_SwipeStart  " + worldPos);

    }

    public void On_Swipe(Gesture gesture)//スワイプした指が離れるまで反応する
    {
        // Verification that the action on the object
        Vector2 screenPos = gesture.position;
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos); //スクリーン座標をワールド座標に変換
        
        if (worldPos.x >= -10 && -5.6 >= worldPos.x && worldPos.y >= -5 &&
            gesture.deltaPosition.x != 0.0 && gesture.deltaPosition.y !=0.0 )//-10より大きく、-5.6よりも小さい
        {
            Debug.Log("スワイプレーン1  ");
            Debug.Log("前回からの距離 " + gesture.deltaPosition);
            Debug.Log("On_Swipe  " + worldPos);
            //judge(0, 2);
        }

    }





    public void On_SwipeEnd(Gesture gesture)
    {
        // Verification that the action on the object
        Vector2 screenPos = gesture.position;
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos); //スクリーン座標をワールド座標に変換
        Debug.Log("フリックend座標  " + worldPos);
        

    }


    public void On_LongTapStart(Gesture gesture)
    {
        // Verification that the action on the object
        Vector2 screenPos = gesture.position;
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos); //スクリーン座標をワールド座標に変換
        Debug.Log("ホールド座標  " + worldPos);

    }


    public void On_Drag(Gesture gesture)
    {
        // Verification that the action on the object
        
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(gesture.position); //スクリーン座標をワールド座標に変換
        Debug.Log("ドラッグ座標  " + worldPos);
        /*
        Debug.Log("長さ " + gesture.swipeLength);
        Debug.Log("方角 " + gesture.swipe);
        Debug.Log("時間 " + gesture.actionTime);*/
        Debug.Log("前回からの距離 " + gesture.deltaPosition);
    }


    public void On_DragEnd(Gesture gesture)
    {
        // Verification that the action on the object

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(gesture.position); //スクリーン座標をワールド座標に変換
        Debug.Log("ドラッグend座標  " + worldPos);
        /*
        Debug.Log("長さ " + gesture.swipeLength);
        Debug.Log("方角 " + gesture.swipe);
        Debug.Log("時間 " + gesture.actionTime);
        Debug.Log("前回からの距離 " + gesture.deltaPosition);*/
    }
}