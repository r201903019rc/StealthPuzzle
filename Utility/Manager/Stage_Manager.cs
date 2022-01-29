//2021/3/19.TKG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

//ステージ開始の処理など、ステージを管理するスプリクト
//ステージ名とかもここで管理してもいいかもしれない
public class Stage_Manager : MonoBehaviour
{
    //シーン内にあるタイルマップ(＝ステージ画面)を管理
    [System.NonSerialized]
    public List<GameObject> Stages = new List<GameObject>();
    //メニュー画面を管理
    [System.NonSerialized]
    public Menu_Manager menus;
    //現在プレイヤーがいるマップを管理
    [System.NonSerialized]
    public GameObject now_stage;
    //プレイヤーのアニメーターコンポーネント
    private Animator playeranim;
    //プレイヤーのアニメーションの再生状況 0の時再生開始、1のとき終了
    //ミス時のアニメーションが終了しているかどうかを判別するために使う
    private float animcall;
    //代入された名前のアニメーションを処理する
    private string animname;
    //現在のステージ状況
    private NOW_STATE isnow;
    //次のステージのScene
    private SceneObject nextscene;
    //ステージのポーズ用フラグ
    private bool pause_flag;
    //現在のステージ状況判別用enum
    public enum NOW_STATE { 
        START_FADEIN_NOW, //ステージ開始時のフェードインが終わっていないとき
        START_NOW,//フェードインが終わり、開始時の演出が行われているとき
        PLAY_NOW,//プレイ中
        MISS_NOW,//ミス時の演出が行われているとき
        CLEAR_NOW,//クリア時演出が行われているとき
        END//enum末尾
    }
    void Start(){
        menus = GetComponent<Menu_Manager>();
        playeranim = Game_Master.Instance.player.GetComponent<Animator>();
        animname = null;
        Scene_Start();

        //子をすべて取得し、そのうちTIlemapを持つ者のみをステージとして取得する
        foreach (Transform child in gameObject.transform) {
            if (child.GetComponent<Tilemap>() == true) { 
            Stages.Add(child.gameObject);
            }
        }

    }

    void Update(){
        //アニメーションを処理するフラグが経ったとき
        if (animname != null) {
            if (anim_now()) {//アニメーションが終了したなら
                switch (isnow) {
                    case NOW_STATE.START_NOW:
                        //アニメーションを移動用に
                        playeranim.Play("Walk");
                        //キャラの操作を可能にする
                        Game_Master.Instance.player.GetComponent<Player_Move>().can_chara_control = true;
                        break;
                    case NOW_STATE.MISS_NOW:　
                        //ステージをリセット
                        Scene_Reset();
                        break;
                    case NOW_STATE.CLEAR_NOW:
                        //シーンを遷移
                        if (nextscene == "") {
                            World_Manager.Next_Scene();
                        }
                        else {
                            Game_Master.Instance.scener.Scene_Change_Fade(nextscene);
                        }
                        break;
                    default:
                        break;
                }
                //判別子をプレイ中に
                isnow = NOW_STATE.PLAY_NOW;
            }
        }

        //ステージ開始時のフェードイン演出が終わったら
        if ((isnow == NOW_STATE.START_FADEIN_NOW)&&(Game_Master.Instance.scener.is_fadein == false)){ 
                //ステージ開始時のアニメーションを進行
                animname = "Start_1";
                playeranim.Play(animname);
                //判別子を更新
                isnow = NOW_STATE.START_NOW;
        }

        //ウインドウの数が0になったor0でなくなったらフラグを変更しポーズ処理に
        if (pause_flag != (Game_Master.Instance.inputer.stack_count() == 0)) {
            pause_flag = (Game_Master.Instance.inputer.stack_count() == 0);
            Stage_Pause();
        }
    }

    //ステージ開始時に行われる処理
    public void Scene_Start() {
        //キャラの操作を不能に
        Game_Master.Instance.player.GetComponent<Player_Move>().can_chara_control = false;
        //スタート時のアニメーションを再生
        animname = null;
        playeranim.Play("Start_0");
        //フェードインを開始
        Game_Master.Instance.scener.FadeIn();
        //フェードインスタート判別子をオンに
        isnow = NOW_STATE.START_FADEIN_NOW;
    }

    //ミス時に行われる処理
    public void Scene_Miss() {
        //キャラの速度をゼロに、操作を不可にする
        Game_Master.Instance.player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Game_Master.Instance.player.GetComponent<Player_Move>().can_chara_control = false;
        //ダメージ用アニメーションを再生
        animname = "Damage";
        playeranim.Play(animname);
        //判別子をミスに
        isnow = NOW_STATE.MISS_NOW;
    }
    //クリア時に行われる処理
   public void Scene_Clear(SceneObject next) {
        //キャラの速度をゼロに、操作を不可にする
        Game_Master.Instance.player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Game_Master.Instance.player.GetComponent<Player_Move>().can_chara_control = false;
        //クリアアニメーションを開始
        animname = "Clear";
        playeranim.Play(animname);
        //判別子をクリアに
        isnow = NOW_STATE.CLEAR_NOW;
        nextscene = next;
    }

    //シーンを再読み込みし、リセットする
    public void Scene_Reset() { 
        Game_Master.Instance.scener.Scene_Change_Fade(SceneManager.GetActiveScene().name);
    }

    //animnameで指定されたアニメーションが終了しているかを返す
    bool anim_now() {
        //アニメーションが指定していなければ返す
        if (animname == null) { return false; }
        //アニメーションが終了したら
         if (playeranim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.2f) {
            //処理するアニメーション名をnullに
                animname = null;
                return true;
         }
        return false;
    }
    //現在のステージを更新する
    public void Stage_set(GameObject stage) {
      now_stage=  stage;
    }
    //ステージを一時停止する
    public void Stage_Pause() {
            //ギミックをすべて停止
            GameObject[] gimmicks = GameObject.FindGameObjectsWithTag("Gimmick");
            foreach (GameObject ene in gimmicks) {
                if (ene.GetComponent<Enemy>()) {
                    ene.GetComponent<Enemy>().enabled = pause_flag;
                }
            }
            //プレイヤーも停止
            Game_Master.Instance.player.GetComponent<Player_Move>().can_chara_control = pause_flag;
            Game_Master.Instance.player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        
        return;
    }
}
