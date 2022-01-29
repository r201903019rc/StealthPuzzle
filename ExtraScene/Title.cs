//2021/3/30.TKG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    //"○○を押してください"のところに描画するためのTextオブジェクト
    public Text text_buttons;
    //次のシーン
    public SceneObject next_scene;

    // Start is called before the first frame update
    void Start()
    {
        //フェードイン
        Game_Master.Instance.scener.FadeIn();
        //決定ボタンが変わったら自動で表示も変化するようにok_button.ToString()を利用
        text_buttons.text = ("PUSH "+Input_Manager.ok_button.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        //フェードアウトが終わるまで入力を禁止
        if ((Game_Master.Instance.inputer.can_get_button == false) && (Game_Master.Instance.scener.is_fadein == false)&& (Game_Master.Instance.scener.is_fadeout == false)) {
            Game_Master.Instance.inputer.can_get_button=true;
        }
            
        //決定ボタンが押されたらシーン遷移
        if (Game_Master.Instance.inputer.Input_Button_Down(Input_Manager.ok_button) && (Game_Master.Instance.scener.is_fadein == false) && (Game_Master.Instance.scener.is_fadeout == false)) {
            Game_Master.Instance.inputer.can_get_button =false;
            World_Manager.Start_World(0);
        }
    }
}
