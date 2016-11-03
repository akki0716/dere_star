using UnityEngine;
using System.Collections;
using TouchScript.Gestures;

public class touchtest : MonoBehaviour {

    private void OnEnable()
    {
        GetComponent<FlickGesture>().Flicked += OnFlick;
        GetComponent<TapGesture>().Tapped += ontap;
        GetComponent<LongPressGesture>().LongPressed += holdon;
        GetComponent<ReleaseGesture>().Released += holdout;
    }
    private void OnDisable()
    {
        GetComponent<FlickGesture>().Flicked -= OnFlick;
        GetComponent<TapGesture>().Tapped -= ontap;
        GetComponent<LongPressGesture>().LongPressed -= holdon;
        GetComponent<ReleaseGesture>().Released += holdout;
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
        string str = "タッチ: " + gesture.ScreenPosition;
        Debug.Log(str);
    }

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
}
