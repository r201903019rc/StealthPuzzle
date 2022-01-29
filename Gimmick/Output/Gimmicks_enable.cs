//2021/4/07.TKG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//オンになるとこのスクリプトがアタッチされているオブジェクトを表示し、オフにすると消えるギミック
public class Gimmicks_enable : MonoBehaviour
{
    //入力をもらう
    public Gimic_Logic Input;
    //表示を担当するレンダラーと、当たり判定を担当するコライダー
    private SpriteRenderer rend;
    private Collider2D[] col;
    //オンオフ情報を格納するbool
    public bool onoff;
    private bool onoff_bef;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        col = GetComponents<Collider2D>();

        rend.enabled = onoff;
        for (int i = 0; i < col.Length; i++) {
            col[i].enabled = onoff;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //オンオフを取得
        onoff= Input.Get_Output();
        //前フレームとオンオフ状況が異なればレンダラーとコライダーをOutputと同期
        if (onoff != onoff_bef) {
            //単純にゲームオブジェクトをオンオフするtransform.gameObject.SetActiveを使うと、オフの時にこのスクリプトが呼ばれなくなってしまうのでこういう処理に
            rend.enabled = onoff;
            for (int i = 0; i < col.Length; i++) {
                col[i].enabled = onoff;
            }
        }
        onoff_bef = onoff;
    }
}
