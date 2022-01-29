//2021/4/07.TKG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimic_Inputer : Gimic_Logic
{
    public Inputer_Type type;
    //オンオフに合わせて色を変化させる、nullにするとどちらも最初にSpriteRenderに設定されていたものを使う
    public Sprite off_sprite;
    public Sprite on_sprite;

    //オブジェクトに乗っかったらオンにしたいときはtrue、触れたらオンにしたいときはfalse
    public bool ride_or_touch;

    //TypeがTIMEのとき、この時間の間だけオンになる
    public int time_limit_frame=120;
    //カウンター
    private int time_count;

    private SpriteRenderer rend;
    //4つある当たり判定について触れた瞬間インクリメントされ、降りた瞬間デクリメントされる
    private int hit_num;
    //キャラが乗っているとき、すでにオンオフが切り替わったかどうかのフラグ
    private bool toggle_key;

    public enum Inputer_Type {
        TOGGLE,//乗るたびにオンになる
        TRIGGER,//乗っている間のみオンになる
        TIME,//乗ると一定時間の間オンになる
    }

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        if (off_sprite == null) { off_sprite = rend.sprite; }
        if (on_sprite == null) { on_sprite = rend.sprite; }
    }

    // Update is called once per frame
    void Update()
    {
        //スイッチのタイプによって処理を変化
        switch (type) {
            case Inputer_Type.TOGGLE://乗るたびにオンオフが切り替わる
                {
                    if (Charactors_On() && !toggle_key) {//フラグがオフかつキャラが乗ったら、入力をオンオフ切り替えしてフラグをオン
                        Rev_Input();
                            toggle_key = true;
                    }
                    else if (!Charactors_On() && toggle_key) { //フラグがオンかつキャラが下りたら、フラグをオフ
                        toggle_key = false;
                        }
                    break;
                }
            case Inputer_Type.TRIGGER://乗っている間はオン、降りるとオフ
                {
                   //フラグがオフかつキャラが乗ったら、入力をオンにしてフラグをオン
                        if (Charactors_On()&& !toggle_key) {
                            On_Input();
                            toggle_key = true;
                        }
                    //フラグがオンかつキャラが下りたら、入力をオフにしてフラグをオフ
                    else  if (!Charactors_On() && toggle_key) {
                            Off_Input();
                            toggle_key = false;
                        }
                    
                    break;
                }
            case Inputer_Type.TIME:
                {
                    //キャラが乗ったら、入力をオンにしてフラグをオン、カウンターもリセット
                    if (Charactors_On()) {
                        On_Input();
                        toggle_key = true;
                        time_count = 0;
                    }        
                    //カウンターを加算
                    time_count++;
                    //フラグがオンでキャラが乗っていないかつ指定の時間が来たら入力をオフにしてフラグもオフ
                    if (time_limit_frame > 0) {
                        if (toggle_key && !Charactors_On() && ((time_count % Mathf.Abs(time_limit_frame)) == 0)) {
                            Off_Input();
                            toggle_key = false;
                        }
                    }
                        break;
                }
        }

        //オンオフに応じて色を変化
       rend.sprite=(Get_Output()?on_sprite:off_sprite);
    }

    //キャラが上に乗っているかどうか
    bool Charactors_On() {
        if (ride_or_touch) {//完全にキャラ乗ったらオンになる
            if (hit_num >= 4) {
                return true;
            }
        }
        else {//キャラが少しでも触れたらオンになる
            if (hit_num >= 1) {
                return true;
            }
        }
        return false;
    }
    void OnTriggerEnter2D(Collider2D col) {
        //当たったものがプレイャーであるとき
        if (col.gameObject.tag == "Player") {
            hit_num++;
        }
    }
    void OnTriggerExit2D(Collider2D col) {
        //当たったものがプレイャーであるとき
        if (col.gameObject.tag == "Player") {
            hit_num--;
        }
    }
}
