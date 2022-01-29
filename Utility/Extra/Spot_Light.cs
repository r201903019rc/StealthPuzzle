using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spot_Light : MonoBehaviour
{
    //動く範囲
    [Range(0,180)]
    public float move_range;
    //動く速度
    public float speed=1f;
    //中央角度
    [Range(0, 360)]
    public float origin_rota;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //Mathf.SinとTime.timeを使って周期運動させる
       transform.rotation =Quaternion.Euler((Mathf.Sin(Time.time*speed) * move_range) + origin_rota, 90f, 90f);
    }
}
