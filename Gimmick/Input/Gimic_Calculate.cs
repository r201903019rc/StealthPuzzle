//2021/4/07.TKG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Gimic_InputerやGimic_Calculate同士で和や積といった論理演算をするスクリプト
public class Gimic_Calculate : Gimic_Logic {
    //演算したいGimic_InputerやGimic_Calculateを入れる
    public List<Gimic_Logic> logics;
    public Calculate_List calculate_type;

    public enum Calculate_List {
    OR,
    AND,
    XOR
    }
    private bool output;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //calculate_typeごとに演算を分岐
        switch (calculate_type) {
            case Calculate_List.OR://OR演算
                output = logics[0].Get_Output();
                for (int i = 1; i < logics.Count; i++) {
                    output = output || logics[i].Get_Output();
                }
                break;
            case Calculate_List.AND://AND演算
                output = logics[0].Get_Output();
                for (int i = 1; i < logics.Count; i++) {
                    output = output && logics[i].Get_Output();
                }
                break;
            case Calculate_List.XOR://XOR演算
                output = logics[0].Get_Output();
                for (int i = 1; i < logics.Count; i++) {
                    output = output ^ logics[i].Get_Output();
                }
                break;
        }
        //出力を演算後のものに変更する
        Change_Input(output);
    }
    
}
