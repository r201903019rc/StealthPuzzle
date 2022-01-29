//2021/5/17.TKG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Utility_Statics;

/*
 ポップアップウィンドウ用のスーパークラス
ウィンドウの生成とか、カーソルの処理とかはこちらで担当
 */

public class UI_window_Origin : MonoBehaviour
{
    //動くかどうかのフラグ
   // public bool can_use_flag;
    //表示するCanvas
    public GameObject UI_canvas;
    //元となるテキスト欄
    protected GameObject text_origin = null;
    //各メニューのテキストコンポーネント
    protected List<Text> UI_text = new List<Text>();
    //カーソル用テキスト
    private Text cursor_text;
    //選択用カーソルの位置
    private int cursor;
    //入力用マネージャ
    protected Input_Manager inputer;
    //ウインドウのスタック番号
    protected int input_stack_num;

    //UIウィンドウの初期化処理
   public void UI_Start(int max) {
        input_stack_num = -1;
        //入力受け取り用スクリプトを取得
        inputer = Game_Master.Instance.inputer;
        //キャンバスをアクティブに
        UI_canvas.SetActive(true);
        //テキストを生成
        Select_UI_Update(max);
    }
    //カーソルの移動処理
    public void Cursor_Move(int max) {
        //カーソルの移動
        if (inputer.Input_Axis_Down(DIRECTION.DOWN_DIR)) {
            cursor++;
        }
        else if (inputer.Input_Axis_Down(DIRECTION.UP_DIR)) {
            cursor--;
        }

        //上限下限の処理
        if (cursor < 0) { cursor = max - 1; }
        else if (cursor >= max) { cursor = 0; }
        //カーソルの移動処理
        Cursor_Pos(cursor);
    }
    //現在のカーソル番号を返すメソッド
    public int Get_cursor() {
        return cursor;
    }
    //オプション画面用UIの生成
   public void Select_UI_Update(int text_num) {
        //キャンバスの子のテキストオブジェクトを取得
        foreach (Transform child in UI_canvas.transform) {
            if (child.GetComponent<Text>()) {
                text_origin = child.gameObject;
                UI_text.Add(text_origin.GetComponent<Text>());
            }
        }
        //元となるテキスト欄のRect情報を取得
        RectTransform origin_rect = text_origin.GetComponent<RectTransform>();
        //text_numの分だけテキスト欄を生成
        for (int i = 1; i < text_num; i++) {
            UI_text.Add(Text_Create(new Vector2(origin_rect.position.x, origin_rect.position.y - (origin_rect.rect.height) * (i))));
        }
        //カーソル用のテキスト欄も生成
        cursor_text = Text_Create(new Vector2(origin_rect.position.x - 20, origin_rect.position.y));
        cursor_text.text = "→";
        //キャンバスの表示をオフに
        UI_canvas.SetActive(false);
    }
    //テキスト欄を1つ作るメソッド
    public Text Text_Create(Vector2 pos) {
        GameObject new_text = Copy_Rect(text_origin);
        RectTransform text_rect = new_text.GetComponent<RectTransform>();
        text_rect.position = pos;
        return new_text.GetComponent<Text>();
    }
    //テキストを複製するメソッド
  public  GameObject Copy_Rect(GameObject origin) {
        GameObject tmp = Instantiate(origin);
        RectTransform origin_rect = origin.GetComponent<RectTransform>();
        RectTransform tmp_rect = tmp.GetComponent<RectTransform>();
        tmp_rect.SetParent(origin_rect.parent);

        tmp_rect.localPosition = Vector3.zero;
        tmp_rect.localRotation = Quaternion.identity;
        tmp_rect.localScale = Vector3.one;
        tmp_rect.pivot = origin_rect.pivot;
        tmp_rect.anchorMin = origin_rect.anchorMin;
        tmp_rect.anchorMax = origin_rect.anchorMax;
        tmp_rect.anchoredPosition = origin_rect.anchoredPosition;
        tmp_rect.sizeDelta = origin_rect.sizeDelta;
        return tmp;
    }
    //カーソルの座標位置を動かすメソッド
  public  void Cursor_Pos(int i) {
        RectTransform cursor_rect = cursor_text.GetComponent<RectTransform>();
        cursor_rect.position = new Vector2(cursor_rect.position.x,
            UI_text[i].transform.position.y);
    }
    //メニュー画面のオンオフ時に呼ばれる
  public void Canvas_OnOff() {
        //画面をオンオフ
        UI_canvas.SetActive(!UI_canvas.activeInHierarchy);
        
        if (UI_canvas.activeInHierarchy == true) {//オンになるとき
            //入力を開始
            input_stack_num = inputer.Stack_Add();
        }
        else { //オフになるとき
            input_stack_num = -1;
            inputer.Stack_Remove();
        }
    }
}
