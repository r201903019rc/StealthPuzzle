//2021/3/19.TKG
//2021/9/09 yogurt:メッセージウィンドウ追加
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
//ゲーム中常駐すべきスプリクトはここに
[RequireComponent(typeof(Input_Manager))]
[RequireComponent(typeof(Scene_Manager))]
[RequireComponent(typeof(Music_Manager))]

//ゲームの起動中に常駐して全体を管理するスプリクト
//ミスやクリア、開始などステージ中の動きを管理するStage_Managerとは別なので注意
public  class Game_Master : SingletonMonoBehaviour<Game_Master> {
    //他オブジェクトはこのGame_Masterスプリクトさえ所得すれば他のユーティリティ系スプリクトにアクセスできるように、クラス内にユーティリティ系スプリクトを持っておく
    [System.NonSerialized]
    public Input_Manager inputer;
    [System.NonSerialized]
    public Scene_Manager scener;
    [System.NonSerialized]
    public Music_Manager musicer;
    [System.NonSerialized]
    public GameObject player;
    [System.NonSerialized]
    public Stage_Manager stager;
    [System.NonSerialized]
    public Menu_Manager menuer;
    [System.NonSerialized]
    public Option_Manager optioner;

    [System.NonSerialized]//メッセージウィンドウ
    public MesseWin_Manager winner;

    //ゲームを起動時にのみ呼ばれるメソッド
    //起動時に自動でGameMasterオブジェクトを生成してくれるように
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeBeforeSceneLoad() {
        //オブジェクトの生成
       GameObject masterobj= (GameObject)Instantiate(Resources.Load("Prefabs/Utility/GameMaster"));
        //シーンをまたいでも消えないように
        DontDestroyOnLoad(masterobj);
    }

    void Start()
    {
        scener = GetComponent<Scene_Manager>();
        inputer = GetComponent<Input_Manager>();
        musicer = GetComponent<Music_Manager>();
        optioner = GetComponent<Option_Manager>();
        winner = GetComponent<MesseWin_Manager>();
        //プレイヤー関係
        try {
            player = GameObject.FindGameObjectWithTag("Player");
            //プレイヤーの操作を無効に
            player.GetComponent<Player_Move>().can_chara_control = false;
        }
        catch (System.NullReferenceException) { }
        //ステージ関係
        try {//エラー対策
            stager = GameObject.FindGameObjectWithTag("StageMaster").GetComponent<Stage_Manager>();
            menuer= GameObject.FindGameObjectWithTag("StageMaster").GetComponent<Menu_Manager>();
        }
        catch (System.NullReferenceException) { }
        
        //OnActiveSceneChangedがシーン遷移時に呼ばれるように登録
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
        RenderSettings.ambientLight = Color.black;
       
    }
    //シーンが遷移したときに呼ばれる
    void OnActiveSceneChanged(Scene prevScene, Scene nextScene) {
        //プレイヤー関係
        try {
            player = GameObject.FindGameObjectWithTag("Player");
            //プレイヤーの操作を無効に
            player.GetComponent<Player_Move>().can_chara_control = false;
        }
        catch (System.NullReferenceException) { }
        //ステージ関係
        try {//エラー対策
            stager = GameObject.FindGameObjectWithTag("StageMaster").GetComponent<Stage_Manager>();
            menuer = GameObject.FindGameObjectWithTag("StageMaster").GetComponent<Menu_Manager>();
        }
        catch (System.NullReferenceException) { }
    }

}
//いろんなところ使うenumとか定数とかはこの中に
//スプリクトの初めにusing static Utility_Statics;を追加すると自由に使える
public class Utility_Statics {
    public enum DIRECTION {
        RIGHT_DIR, UP_DIR, LEFT_DIR, DOWN_DIR, MAX
    };
}