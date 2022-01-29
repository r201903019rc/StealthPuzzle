//2021/3/19.TKG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utility_Statics;
using System;

//ゲーム中のキーボードからの入力を一括管理するクラス
public class Input_Manager : MonoBehaviour
{
    //入力を返すかどうかのフラグ
    public bool can_get_button=true;
    //縦横の入力
    public Vector2 axis_input;
    private Vector2 axis_input_bef;

    
    //ボタン、用途に応じてそれぞれ宣言する(普通のゲームだと攻撃ボタンは決定ボタンを兼ねてるとかあるけどここでは用途ごとに分離、後々キーコンフィグ処理など書きやすいように)
    //決定ボタン
    static public KeyCode ok_button=KeyCode.Z;
    //キャンセルボタン
    static public KeyCode cancel_button = KeyCode.X;

    //能力選択ボタン
    static public KeyCode skills_select_button = KeyCode.LeftShift;
    //能力使用ボタン
    static public KeyCode skills_use_button = KeyCode.X;

    //メニュー画面ボタン
    static public KeyCode menu_button = KeyCode.Escape;

    //ウインドウ管理スタック
    private List<int> window_stack = new List<int>();


    void Update()
    {
        //Input.GetAxisで、十字キーorWASDの入力がとってこれる
        axis_input.x = Input.GetAxis("Horizontal");
        axis_input.y = Input.GetAxis("Vertical");

    }

    void LateUpdate()
    {
        axis_input_bef = axis_input;
    }

    //ボタン関係
    //他のコードから使いたいときはGame_Master.Instance.inputer.Input_Button_Down(Input_Manager.ok_button)のように使う
    //ボタンを押した瞬間のみtrue
    public bool Input_Button_Down(KeyCode button) {
        return (can_get_button?Input.GetKeyDown(button):false);
    }
    //ボタンを押している間true
    public bool Input_Button_Stay(KeyCode button) {
        return (can_get_button ? Input.GetKey(button):false);
    }
    //ボタンを離した瞬間のみtrue
    public bool Input_Button_Up(KeyCode button) {
        return (can_get_button ? Input.GetKeyUp(button):false);
    }

    //方向キー関係
    //方向キーが押された瞬間値を返す
    public bool Input_Axis_Down(DIRECTION dir) {
        bool tmp = false;
        switch (dir) {
            case DIRECTION.UP_DIR:
                tmp = (axis_input.y > 0)&&(axis_input.y!=axis_input_bef.y)&&(axis_input_bef.y<=0);
                break;
            case DIRECTION.DOWN_DIR:
                tmp = (axis_input.y < 0) && (axis_input.y != axis_input_bef.y) && (axis_input_bef.y >= 0);
                break;
            case DIRECTION.LEFT_DIR:
                tmp = (axis_input.x > 0) && (axis_input.x != axis_input_bef.x) && (axis_input_bef.x <= 0);
                break;
            case DIRECTION.RIGHT_DIR:
                tmp = (axis_input.x < 0) && (axis_input.x != axis_input_bef.x) && (axis_input_bef.x >= 0);
                break;
        }
        return tmp;
    }
    //方向キーが押されている間値を返す(ただし普通に十字の入力を取りたい場合はaxis_inputをそのまま持ってきたほうが楽、方向を指定したいときだけこれを使ってどうぞ)
    public bool Input_Axis_Stay(DIRECTION dir) {
        bool tmp = false;
        switch (dir) {
            case DIRECTION.UP_DIR:
                tmp = (axis_input.y > 0);
                break;
            case DIRECTION.DOWN_DIR:
                tmp = (axis_input.y < 0);
                break;
            case DIRECTION.LEFT_DIR:
                tmp = (axis_input.x > 0);
                break;
            case DIRECTION.RIGHT_DIR:
                tmp = (axis_input.x < 0);
                break;
        }
        return tmp;
    }
    //方向キーが離された瞬間値を返す
    public bool Input_Axis_Up(DIRECTION dir) {
        bool tmp = false;
        switch (dir) {
            case DIRECTION.UP_DIR:
                tmp = (axis_input.y <= 0) && (axis_input.y != axis_input_bef.y) && (axis_input_bef.y > 0);
                break;
            case DIRECTION.DOWN_DIR:
                tmp = (axis_input.y >= 0) && (axis_input.y != axis_input_bef.y) && (axis_input_bef.y < 0);
                break;
            case DIRECTION.LEFT_DIR:
                tmp = (axis_input.x <= 0) && (axis_input.x != axis_input_bef.x) && (axis_input_bef.x > 0);
                break;
            case DIRECTION.RIGHT_DIR:
                tmp = (axis_input.x >= 0) && (axis_input.x != axis_input_bef.x) && (axis_input_bef.x < 0);
                break;
        }
        return tmp;
    }

    //スタック関係
    //スタックを利用する際に呼び出し、スタック番号を返す
    //これ以降スタックを用いたキー入力操作の時にはこのスタック番号を利用するので忘れず保存する
    public int Stack_Add() {
        window_stack.Add(window_stack.Count);
        return window_stack.Count;
    }
    //スタックから削除
    public void Stack_Remove() {
        window_stack.Remove(window_stack.Count-1);
        return;
    }
    //現在のスタック内に入っている要素の数
    public int stack_count() {
        return window_stack.Count;
    }
    //ウインドウのスタック番号を受け取り、動かせる状態か否かを返す
    public bool Can_Move_Window(int num) {
       return (stack_count() == num);
    }
}
