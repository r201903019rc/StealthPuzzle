//2021/4/22.TKG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Tilemapオブジェクトにアタッチするとそのタイルマップ内でカメラ追従を管理してくれるスクリプト。
画面のアスペクト比が4:3じゃないとバグるので実行前にGame画面の画面解像度を確認してから実行してください。ビルド時には固定します。
 */
public class Camera_Follow : MonoBehaviour
{
    //追従するオブジェクト
    private GameObject follow_obj;
    //ステージのコライダー
    private Collider2D stage_col;
    //バックグラウンドカメラ
    private Camera backcam;
    private GameObject camobj;

    //どの軸について追従するのか
    public bool follow_x;
    public bool follow_y;

    //ステージの表示範囲の中心、表示範囲の大きさ、表示範囲のz座標
    private Vector2 stage_center;
    private Vector2 stage_size;
    //最終的なカメラの座標
    private Vector3 next_pos;

    //カメラに映る右上と左下の座標と、カメラの座標の差分
    private Vector3 col_edge_def;
    //カメラに映る右上と左下の座標、col_edge_pos[0]が左下、[1]が右上
    private Vector3[] col_edge_pos = new Vector3[2];
    //カメラに映す範囲の矩形
    private Rect col_rect;
    //キャラがこのタイル内にいるかどうか
    private bool now_players_visit;

    //タイルマップを離れたときに、タイルマップ内のオブジェクトを元に戻すためのリスト
    List<GameObject> reset_obj = new List<GameObject>();
    List<Vector3> reset_vec = new List<Vector3>();
    List<Enemy> enemy_obj = new List<Enemy>();
    //カメラの端点用のenum
    public enum camera_dir { 
    Right_Down, Left_Up
    }
    void Start()
    {
        //各変数を初期化
        follow_obj = Game_Master.Instance.player;
        stage_col = GetComponent<Collider2D>();
        backcam = Camera.main;
        camobj = Camera.main.transform.gameObject;
        //カメラに映る範囲のステージ矩形を生成
        rect_update();
        //カメラの中央とカメラの端点の座標の距離を取る
        col_edge_def= backcam.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, 0.0f))- camobj.transform.position;
        col_edge_def = new Vector3(Mathf.Abs(col_edge_def.x), Mathf.Abs(col_edge_def.y), 0.0f);
        //マップ内のオブジェクトの位置を記録しておく
        foreach (Transform child in transform) {
            if (child.tag != "Player") {
                reset_obj.Add(child.transform.gameObject);
                reset_vec.Add(child.transform.position);
            }
            if (child.GetComponent<Enemy>()) {
                enemy_obj.Add(child.GetComponent<Enemy>());
            }

        }
    }

    void Update() {
        //このタイルマップにプレイヤーが来たら、Stage_Masterに送る
        if (col_rect.Contains(Game_Master.Instance.player.transform.position)) {
            Game_Master.Instance.stager.Stage_set(transform.gameObject);
        }
        //プレイヤーがマップから離れた瞬間、オブジェクトの位置を元に戻す
        if (((Game_Master.Instance.stager.now_stage == this.gameObject) != now_players_visit)&&now_players_visit==true) {
            Reset();
        }

        //現在プレイヤーがこのタイルマップにいるかどうか
        now_players_visit = (Game_Master.Instance.stager.now_stage == this.gameObject);
    }

    void LateUpdate() {
        //キャラがこのタイルマップにいる間
        if (now_players_visit) {
                //各軸についてキャラの座標を代入して追従
                next_pos = new Vector3(
                    (follow_x ? follow_obj.transform.position.x : camobj.transform.position.x),
                    (follow_y ? follow_obj.transform.position.y : camobj.transform.position.y),
                     camobj.transform.position.z
                    );
            
            //端点の座標を更新
            edge_pos_update();
            //追従処理後のカメラの座標を取得し、範囲内に収める
            float next_x = next_pos.x;
            float next_y = next_pos.y;
            //右
            if (col_rect.xMax < col_edge_pos[(int)camera_dir.Left_Up].x) { next_x =next_x-(col_edge_pos[(int)camera_dir.Left_Up].x - col_rect.xMax); }
            //左
            if (col_rect.xMin > col_edge_pos[(int)camera_dir.Right_Down].x) { next_x = next_x+( col_rect.xMin - col_edge_pos[(int)camera_dir.Right_Down].x); }
            //上
            if (col_rect.yMax < col_edge_pos[(int)camera_dir.Left_Up].y) { next_y = next_y - (col_edge_pos[(int)camera_dir.Left_Up].y - col_rect.yMax); }
            //下
            if (col_rect.yMin > col_edge_pos[(int)camera_dir.Right_Down].y) { next_y = next_y+( col_rect.yMin - col_edge_pos[(int)camera_dir.Right_Down].y); }

            //最終的な代入
            camobj.transform.position = new Vector3(next_x, next_y, next_pos.z);
        }
    }

    //画面の右上、左下の端点座標を更新
    void edge_pos_update() {
        col_edge_pos[(int)camera_dir.Right_Down] = next_pos - col_edge_def;
        col_edge_pos[(int)camera_dir.Left_Up] = next_pos + col_edge_def;
    }


    //ステージの映る範囲の矩形を生成
    void rect_update() {
        stage_center = stage_col.bounds.center;
        stage_size = stage_col.bounds.extents;
        col_rect = new Rect((stage_center.x - stage_size.x), (stage_center.y - stage_size.y), stage_size.x * 2, stage_size.y * 2);
    }
    //マップ内のオブジェクトを元の位置に戻す
    void Reset() {
        for (int i = 0; i < reset_obj.Count; i++) {
            reset_obj[i].transform.position = reset_vec[i];
        }

        for (int i = 0; i < enemy_obj.Count; i++) {
            enemy_obj[i].target_pos = enemy_obj[i].transform.position;
        }

    }

    void OnDrawGizmos() {
        //ステージの範囲の矩形を表示する
        if (now_players_visit) {
            Gizmos.color = Color.green;
        }
        else { 
            Gizmos.color = Color.blue; 
        }
      Gizmos.DrawWireCube(col_rect.center, new Vector2(col_rect.width, col_rect.height));
    }
    
}
