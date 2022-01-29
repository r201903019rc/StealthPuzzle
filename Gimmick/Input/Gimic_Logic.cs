//2021/4/07.TKG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ギミックの入力を担当するスーパークラス
//継承先クラスはキャラの動きによって入力を受け取って出力するGimic_Inputerと、複数の入力から論理演算をして1つの出力にまとめるGimic_Calculate

/*
<エディタからの使い方>
ギミックを実際に発動するオブジェクトにGimmicks/Output内のスクリプトをアタッチし、prefabフォルダからGimic_Inputerをヒエラルキーに追加する
複数のGimic_Inputerを使って2つ同時にボタンを押したら発動～とかにしたい場合は別途オブジェクトを用意してGimic_Calculateをアタッチして、
    Gimic_CalculateのlogicsのリストにGimic_Inputerを接続してあげればOK。
    logicsのリストにGimic_Calculate自身を入れることもできるのでOR演算とOR演算でAND演算をしてそれを…とかみたいにして論理演算もできるはず。
あとはギミック発動スクリプトのInputのところに出力が繋がるようにGimic_InputerかGimic_Calculateを接続していけばOK
もちろんGimic_Inputerとギミック発動スクリプトを同じオブジェクトにアタッチして、押したらそのまま何かが起こるようなものにしてもOK、prefabのClear_Gateはそういう作りにしてます
 */
/*
 <ギミック発動スクリプトを書く時>
Gimic_Logic型の変数inputを宣言しているとすると、input.Get_Output()で出力が取り出せる。
エディタから使いやすいようにpublic型で宣言しておくといいと思う。
実際の例はGimmicks_enableとGimmicks_clearでやっているので参考にしてください。　
 */
public class Gimic_Logic : MonoBehaviour
{
    //オンかオフかを判定する
    private bool input;
    //出力を反転するフラグ
    public bool not;

    //現在のオンオフを出力する
    public bool Get_Output() {
        //notのときは反転
        return (not?!input:input);
    }

    //オンにする
    public void On_Input() {
        input = true;
    }

    //オフにする
    public void Off_Input() {
        input = false;
    }

    //オンオフを逆転させる
    public void Rev_Input() {
        input = !input;
    }

    //オンオフを引数にとり、代入する
    public void Change_Input(bool t) {
        input = t;
    }
}
