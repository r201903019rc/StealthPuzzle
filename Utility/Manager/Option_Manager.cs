//2021/5/17.TKG
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using static Utility_Statics;

public class Option_Manager : UI_window_Origin {
    //各メニューの名前
    public string[] menu_name = new string[(int)Menu_select.MAX];

    //BGM、SE用のテキスト欄
   private Text BGM_text;
    private Text SE_text;

    //各メニュー用のenum
    enum Menu_select {
        BGM_volume, 
        SE_volume,
        MAX
    }
    // Start is called before the first frame update
    void Start() {
        //ウィンドウの初期化処理
        UI_Start((int)Menu_select.MAX);
        //テキストを設定
      for (int i = 0; i < (int)Menu_select.MAX; i++) {
            UI_text[i].text=menu_name[i];
        }
        //BGM、SEの音量を表示するテキスト欄をそれぞれ生成
        BGM_text=Volume_Text_Create(Menu_select.BGM_volume);
        BGM_text.text = Game_Master.Instance.musicer.BGM_volume.ToString();
        SE_text = Volume_Text_Create(Menu_select.SE_volume);
        SE_text.text = Game_Master.Instance.musicer.SE_volume.ToString();
    }

    // Update is called once per frame
    void Update() {
        //ウインドウが動ける状態なら
        if (inputer.Can_Move_Window(input_stack_num)) {
            //キャンセルキー、もしくはメニューキーで離脱
            if (inputer.Input_Button_Down(Input_Manager.cancel_button)||inputer.Input_Button_Down(Input_Manager.menu_button)) {
                    Canvas_OnOff();
                }
                //カーソル移動処理
                Cursor_Move((int)Menu_select.MAX);
                //実際の処理部分
                switch (Get_cursor()) {
                    case (int)Menu_select.BGM_volume:
                        Game_Master.Instance.musicer.BGM_volume = Volume_Change(Game_Master.Instance.musicer.BGM_volume);
                        BGM_text.text = Game_Master.Instance.musicer.BGM_volume.ToString();
                        break;
                    case (int)Menu_select.SE_volume:
                        Game_Master.Instance.musicer.SE_volume = Volume_Change(Game_Master.Instance.musicer.SE_volume);
                        SE_text.text = Game_Master.Instance.musicer.SE_volume.ToString();
                        break;
                    default:
                        break;
                }
            
        }
    }
    
    //現在の音量を受け取り、入力から変更後の音量を返す
    int  Volume_Change(int vol) {
        //右入力があれば1、左入力があれば-1
        int res = 0;
        res -= Convert.ToInt32(inputer.Input_Axis_Down(DIRECTION.RIGHT_DIR));
        res += Convert.ToInt32(inputer.Input_Axis_Down(DIRECTION.LEFT_DIR));
        //音量を変更
        vol += res;
        //0~100の間に調節
        return Mathf.Clamp(vol, 0, 100); 
    }

    //Menu_selectを引数にとって、その番号にあったテキスト欄を生成する
    Text Volume_Text_Create(Menu_select t) {
        Vector2 UI_pos = UI_text[(int)t].rectTransform.position;
        return Text_Create(new Vector2(UI_pos.x + 200,UI_pos.y));
    }
}
