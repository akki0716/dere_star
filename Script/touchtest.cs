using UnityEngine;
using System.Collections;
using TouchScript.Gestures;

public class touchtest : MonoBehaviour {

    private void OnEnable()
    {
        GetComponent<FlickGesture>().Flicked += OnFlick;
        GetComponent<TapGesture>().Tapped += ontap;
        //GetComponent<LongPressGesture>().LongPressed += holdon;
        //GetComponent<ReleaseGesture>().Released += holdout;
    }
    private void OnDisable()
    {
        GetComponent<FlickGesture>().Flicked -= OnFlick;
        GetComponent<TapGesture>().Tapped -= ontap;
        //GetComponent<LongPressGesture>().LongPressed -= holdon;
        //GetComponent<ReleaseGesture>().Released += holdout;
    }

    // フリックジェスチャーが成功すると呼ばれるメソッド
    private void OnFlick(object sender, System.EventArgs e)
    {
        var gesture = sender as FlickGesture;
        string str = "フリック: " + gesture.ScreenFlickVector + " (" + gesture.ScreenFlickTime + "秒)";
        Debug.Log(str);
    }

    // タップが成功すると呼ばれるメソッド
    private void ontap(object sender, System.EventArgs e)
    {
        var gesture = sender as TapGesture;
        Vector2 screenPos = gesture.ScreenPosition; //スクリーン座標を取得し一旦変数に保存
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos); //スクリーン座標をワールド座標に変換

        string str = "タッチ: " + worldPos;

        
        Debug.Log(str);
        Debug.Log("指本数 " + gesture.NumTouches);
    }
    /*
        // ホールドが成功すると呼ばれるメソッド
        private void holdon(object sender, System.EventArgs e)
        {
            var gesture = sender as LongPressGesture ;

            string str = "ホールド: " + gesture.ScreenPosition;

            Debug.Log(str);
        }

        private void holdout(object sender, System.EventArgs e)
        {
            var gesture = sender as ReleaseGesture;
            string str = "離した: " + gesture.ScreenPosition;
            Debug.Log(str);
        }
        */

  

}
