//2021/4/19.TKG(yogurtさんのHowToDark.csを参考に作りました)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/*
このオブジェクトは現在の仕様ではステージ内に2個以上あるとバグります、ご注意を
明るくなる距離を変更したいときはpoint_lightにアタッチするオブジェクトをいじってください
また、暗くなるのはGrid.Tilemapの下のオブジェクトのみなのでご注意ください
*/
public class Gimmicks_Light : MonoBehaviour
{
    //光らせたいオブジェクトのリスト
    public List<GameObject> light_obj=new List<GameObject>();
    //オブジェクトを照らすポイントライト
    public GameObject point_light;
    //入力をもらう
    public Gimic_Logic Input;

    //光らせたいオブジェクトに付けたライトのリスト
    private List<GameObject> lights=new List<GameObject>();
    //ステージのタイルマップのリスト
    private List<TilemapRenderer> stage_maps=new List<TilemapRenderer>();
    //暗いときののマテリアル
    public Material dark_material;
    //明るいときのマテリアル
    public Material light_material;

    //オンオフ情報を格納するbool
    private bool onoff=true;
    private bool onoff_bef;

    // Start is called before the first frame update
    void Start()
    {
        //マップのTilemapRendererを取得
        foreach (GameObject obj in Game_Master.Instance.stager.Stages) {
            stage_maps.Add(obj.GetComponent<TilemapRenderer>());
        }
        //光らせたいオブジェクトの子にライトのオブジェクトを付ける
        for (int i = 0; i < light_obj.Count; i++) {
             lights.Add(Instantiate(point_light, new Vector3(light_obj[i].transform.position.x, light_obj[i].transform.position.y, -0.4f), Quaternion.identity));
             lights[i].transform.parent = light_obj[i].transform;
            lights[i].SetActive(onoff);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //オンオフを取得
        onoff = Input.Get_Output();
        //前フレームとオンオフ状況が異なれば処理を開始
        if (onoff != onoff_bef) {
            if (onoff == true) {//画面が明るくなった時
                Light_Up();
            }
            else {//画面が暗くなった時 
                Light_Down();
            }
        }
        onoff_bef = onoff;
    }

    //照明をつける処理
    void Light_Up() {
        //マップと、マップ内のギミックを通常の色に変更
        foreach (TilemapRenderer maps in stage_maps) {
            maps.material = light_material;
            foreach (Transform childTransform in maps.transform) {
                childTransform.gameObject.GetComponent<SpriteRenderer>().material = light_material;
            }
        }
      
        //ライトを消す
        for (int i = 0; i < lights.Count; i++) {
            lights[i].SetActive(false);
        }
    }

    //照明を消す処理
    void Light_Down() {
        //マップと、マップ内のギミックを黒色に変更
        foreach (TilemapRenderer maps in stage_maps) {
            maps.material = dark_material;
            foreach (Transform childTransform in maps.transform) {
                childTransform.gameObject.GetComponent<SpriteRenderer>().material = dark_material;
            }
        }

        //ライトをつける
        for (int i = 0; i < lights.Count; i++) {
            lights[i].SetActive(true);
        }
    }
}
