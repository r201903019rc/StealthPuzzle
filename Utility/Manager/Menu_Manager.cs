//2021/5/11.TKG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_Manager : UI_window_Origin {
   //各メニューの名前
    public string[] menu_name=new string[(int)Menu_select.MAX];

    //タイトル画面のScene
    public SceneObject Title_scene;

    //時間関係
    public Text time_text;//表示用UI
    public float time;//経過時間
    //各メニュー用のenum
    enum Menu_select { 
    REROAD,//現在のステージを最初から始める
    OPTION,//オプション画面を呼び出す
    TITLE,//タイトル画面に戻る
    MAX
    }
    // Start is called before the first frame update
    void Start() 
        {
        //ウィンドウの初期化処理
        UI_Start((int)Menu_select.MAX);
        //テキストを設定
          for (int i = 0; i < (int)Menu_select.MAX; i++) {
        UI_text[i].text = menu_name[i];
            }
    }

    // Update is called once per frame
    void Update()
    {
        //メニュー画面ボタンでオンオフ
        if (inputer.Input_Button_Down(Input_Manager.menu_button)) {
            Canvas_OnOff();
        }
        //ウインドウが動ける状態なら
        if (inputer.Can_Move_Window(input_stack_num)) {
            //キャンセルキーでも離脱可能
            if (inputer.Input_Button_Down(Input_Manager.cancel_button)) {
                Canvas_OnOff();
            }
            //カーソルの移動
            Cursor_Move((int)Menu_select.MAX);
            //決定ボダンが押されたら処理する
            if (inputer.Input_Button_Down(Input_Manager.ok_button)) {
                //実際の処理部分
                switch (Get_cursor()) {
                    case (int)Menu_select.REROAD:
                        Game_Master.Instance.stager.Scene_Reset();
                        break;
                    case (int)Menu_select.OPTION:
                        Game_Master.Instance.optioner.Canvas_OnOff();
                        break;
                    case (int)Menu_select.TITLE:
                        Game_Master.Instance.scener.Scene_Change_Fade(Title_scene);
                        break;
                    default:
                        break;
                }
            }
        }
        //カウントを追加
        time += Time.deltaTime;
        //時間を表示
        time_text.text = (((int)time / 60).ToString("00") + ":"+((int)time%60).ToString("00"));
    }

}
