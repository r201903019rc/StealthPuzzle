//2021/3/19.TKG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Scene遷移を管理するスプリクト
//全てのSceneの番号とかを取得して変数として置いておいてもいいかもしれない
public class Scene_Manager : MonoBehaviour
{
    //移行するシーン名
    private SceneObject next_scene;
    //フェードにつかうCanvasとImage
    private  Canvas fade_canvas;
    private Image fade_image;
    //フェード時の透明度
    private float alpha = 0.0f;
    //フェードインアウトのフラグ
    public  bool is_fadein = false;
    public  bool is_fadeout = false;
    //フェードに必要な秒数
    private float fadetime = 1.2f;

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update(){
        //それぞれフェードイン、アウトのフラグ有効なら毎フレーム画面の透明度を変化
        if (is_fadein) {
            //経過時間から透明度計算
            alpha -= Time.deltaTime / fadetime;
            //フェードイン終了判定
            if (alpha <= 0.0f) {
                is_fadein = false;
                alpha = 0.0f;
                fade_canvas.enabled = false;
            }
            //フェード用Imageの色・透明度設定
            fade_image.color = new Color(0.0f, 0.0f, 0.0f, alpha);
        }
        else if (is_fadeout) {
            //経過時間から透明度計算
            alpha += Time.deltaTime / fadetime;
            //フェードアウト終了判定
            if (alpha >= 1.0f) {
                is_fadeout = false;
                alpha = 1.0f;
                //最後に次のシーンへ遷移
                SceneManager.LoadScene(next_scene);
            }
            //フェード用Imageの色・透明度設定
            fade_image.color = new Color(0.0f, 0.0f, 0.0f, alpha);
        }
    }
    //SceneObjectを受け取り、遷移するメソッド
    //SceneObjectクラスはシーン名やシーン番号を引数にしても自動で型変換してくれるのでそれで呼び出しても可
    public void Scene_Change(SceneObject scenename) {
        SceneManager.LoadScene(scenename);
    }
    //フェードアウトあり版の遷移メソッド
    public void Scene_Change_Fade(SceneObject scenename) {
        next_scene = scenename;
        FadeOut(); 
    }

    //フェードイン、アウトの際に使用する画面を覆うマスク用Canvasの生成
   void FadeCanvasReset() {
        //Canvas生成
        GameObject FadeCanvasObject = new GameObject("CanvasFade");
        fade_canvas = FadeCanvasObject.AddComponent<Canvas>();
        FadeCanvasObject.AddComponent<GraphicRaycaster>();
        fade_canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        FadeCanvasObject.AddComponent<Scene_Manager>();

        //最前面になるよう適当にレイヤー順を設定
        fade_canvas.sortingOrder = 100;

        //フェード用のImage生成
        fade_image = new GameObject("ImageFade").AddComponent<Image>();
        fade_image.transform.SetParent(fade_canvas.transform, false);
        fade_image.rectTransform.anchoredPosition = Vector3.zero;

        //Imageサイズは適当に大きく
        fade_image.rectTransform.sizeDelta = new Vector2(9999, 9999);
    }

    //フェードインをするときに呼び出すメソッド、void Start()内で呼ぶといい
    public void FadeIn() {
        //もしマスク用のCanvasがなければ生成
        if (fade_image == null) { FadeCanvasReset(); }
        fade_image.color = Color.black;
        is_fadein = true;
    }

    //フェードアウト開始
    void FadeOut() {
        if (fade_image == null) { FadeCanvasReset(); }
        fade_image.color = Color.clear;
        fade_canvas.enabled = true;
        is_fadeout = true;
    }
}
