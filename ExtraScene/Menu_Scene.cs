//2021/3/30.TKG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_Scene : MonoBehaviour
{
    private int select;
    //メニュー項目のリスト
    public List<Menu_Item> items=new List<Menu_Item>();
    //複製元となるテキストオブジェクト
    public Text origin_text;
    //行間サイズ
    public float text_space;
    //メニュー項目のclass
    [System.Serializable]
    public class Menu_Item {
        public string Menu_Name;//項目名
        public SceneObject Menu_Scene;//そのメニューを選択したときに移行するScene
    }
    // Start is called before the first frame update
    void Start() {
        //フェードイン
        Game_Master.Instance.scener.FadeIn();
        //文字間隔を設定
        origin_text.lineSpacing = text_space;
        //描画
        items_write();
    }

    // Update is called once per frame
    void Update() {
        //フェードアウトが終わるまで入力を禁止
        if ((Game_Master.Instance.inputer.can_get_button == false) && (Game_Master.Instance.scener.is_fadein == false) && (Game_Master.Instance.scener.is_fadeout == false)) {
            Game_Master.Instance.inputer.can_get_button = true;
        }
        select_move();
        //決定ボタンが押されたらシーン遷移
        if (Game_Master.Instance.inputer.Input_Button_Down(Input_Manager.ok_button) && (Game_Master.Instance.scener.is_fadein == false) && (Game_Master.Instance.scener.is_fadeout == false)) {
            Game_Master.Instance.inputer.can_get_button = false;
            Game_Master.Instance.scener.Scene_Change_Fade(items[select].Menu_Scene);
        }
    }
    //カーソルの移動
    void select_move() {
        //前ループのselectの保存
        int selece_before = select;
        //入力受付
        if (Game_Master.Instance.inputer.Input_Axis_Down(Utility_Statics.DIRECTION.UP_DIR)) {
            select--;
        }
        else if (Game_Master.Instance.inputer.Input_Axis_Down(Utility_Statics.DIRECTION.DOWN_DIR)) {
            select++;
        }
        //入力範囲外にならないように
        if (select < 0) { select = items.Count - 1; }
        else if (select > items.Count - 1) { select = 0; }
        //もし入力があれば再描画
        if (select != selece_before) {
            items_write();
        }
    }

    //項目名を描画する
    void items_write() {
        //テキストを初期化
        origin_text.text = "";
        //リストの描画
        for (int i = 0; i < items.Count; i++) {
            if (i == select) { 
                origin_text.text = (origin_text.text + (i != 0 ? "\n→" : "→") +items[i].Menu_Name );
            }
            else {
                origin_text.text = (origin_text.text+(i != 0 ? "\n　" : "　") + items[i].Menu_Name);
            }
        }
    }
}

