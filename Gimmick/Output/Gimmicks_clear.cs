//2021/4/07.TKG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmicks_clear : MonoBehaviour
{
    public Gimic_Logic input;
    [Header("↓nullの場合、ワールド管理で選ばれた次のステージへ自動で移行")]
    public SceneObject nextscene;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if (input.Get_Output()) {
            //Stage_Managerのclear判定を呼ぶ
                Game_Master.Instance.stager.Scene_Clear(nextscene);
            //何度もシーン遷移しないように入力を切る
            input.Rev_Input();
        }
    }
}
