

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//2021/3/19.TKG
//BGMやSEの再生を処理するスプリクト
//とりあえずガワだけ、まだ中身はなし

/*2021年6月9日：yougurt
 * 設計図通りに作った→これで大体完成でいいかも
 */

// 2021/07/28 yogurt
// パーソナルから本環境に移動
// 読み込むBGMとSEのフォルダを、本環境のものに変更

// 2021/08/13
// オーディオソースをパブリックに
// →メッセージウィンドウ側でSEの再生止めるとき用に
//ミュートとミュート解除関数が逆だったので修正

public class Music_Manager : MonoBehaviour
    {
    //BGMの音量
    [Range(0, 100)]
    public int BGM_volume;
    //SEの音量
    [Range(0, 100)]
    public int SE_volume;

    //BGMが入っているフォルダのパス
    public string bgm_directory = "Assets/Musics/BGM/";
    //SEが入っているフォルダのパス
    public string se_directory = "Assets/Musics/SE/";

    //再生に使用するオーディオソースコンポーネント
    public AudioSource[] _audio_sources = new AudioSource[2];

    //読み込んだファイルのパスを一時的に入れる配列
    private string[] file_pass_str;

    //読み込んだBGMとSEのオーディオクリップを入れるリスト
    private List<AudioClip> se_list = new List<AudioClip>();
    private List<AudioClip> bgm_list = new List<AudioClip>();

    //読みこんだオーディオクリップを一時的に入れるやつ
    private AudioClip now_sound;

    //呼び出したBGMとSEのオーディオクリップを一時的に入れるやつ
    private AudioClip play_se;
    private AudioClip play_bgm;

    /*現在のカレントディレクトリのパスを取得する(～/Assetsの～)
    private string CurrentDirectory =
        (Directory.GetCurrentDirectory()).Replace("\\", "/") + "/";*/


    void Start()
    {

        //0番目をBGM用、1番目をSE用のオーディオソースにする
        //別々に音量を調整できるようにしたいため
        for (int i = 0; i < 2; i++)
        {
            _audio_sources[i] = this.gameObject.AddComponent<AudioSource>();
        }


        ReadFile_And_Addlist(bgm_list, bgm_directory, "mp3");
        ReadFile_And_Addlist(bgm_list, bgm_directory, "ogg");
        ReadFile_And_Addlist(bgm_list, bgm_directory, "wav");
        ReadFile_And_Addlist(bgm_list, bgm_directory, "aif");

        ReadFile_And_Addlist(se_list, se_directory, "mp3");
        ReadFile_And_Addlist(se_list, se_directory, "ogg");
        ReadFile_And_Addlist(se_list, se_directory, "wav");
        ReadFile_And_Addlist(se_list, se_directory, "aif");

    }

    /// <summary>
    /// 指定したディレクトリの中にある、
    /// 指定した拡張子のオーディオクリップを読み取り、
    /// 指定したリスト(オーディオクリップ)の中に入れるメソッドです。
    /// </summary>
    /// <param name="sound_list">BGMかSEのリスト(オーディオクリップ)</param>
    /// <param name="directory">BGMかSEのファイルがあるフォルダ</param>
    /// <param name="filetype">オーディオファイルの拡張子</param>
    private void ReadFile_And_Addlist(List<AudioClip> sound_list, string directory, string filetype)
    {
        //配列の中に、オーディオファイルのパス(拡張子含む)が全部入ります
        file_pass_str = Directory.GetFiles(directory, "*." + filetype);

        //全部のパスでアセットデータベースロードする
        foreach (string file_pass in file_pass_str)
        {
            //Debug.Log(file_pass);
            now_sound = AssetDatabase.LoadAssetAtPath<AudioClip>(file_pass);

            if (now_sound == null)
            {//中身が空だったら読み込めてないのでエラー
                Debug.Log(name + ":読み込みに失敗しているようです");
            }

            sound_list.Add(now_sound);
        }
    }

    /// <summary>
    /// 指定したBGMを再生する(BGM名に拡張子は含まないことに注意!)。
    /// 「test_BGM1.mp3」を呼ぶ場合は("test_BGM1")って書いてください。
    /// 2つ目の引数にはループするかしないかを指定してください→指定なしの場合はループします。
    /// </summary>
    /// <param name="bgm_name">BGM名</param>
    /// <param name="loop">ループするかしないか</param>
    public void PlayBGM_Sound(string bgm_name, bool loop)
    {
        //リストの中から検索
        play_bgm = bgm_list.Find(n => n.name == bgm_name);

        if (play_bgm == null)
        {//空だったらエラー
            Debug.Log(bgm_name + ":BGMの音声ファイルが空です");
        }

        _audio_sources[0].clip = play_bgm;
        _audio_sources[0].volume = (float)BGM_volume / 100;
        _audio_sources[0].loop = loop;
        _audio_sources[0].Play();
    }

    /// <summary>
    /// 指定したBGMを再生する(BGM名に拡張子は含まないことに注意!)。
    /// 「test_BGM1.mp3」を呼ぶ場合は("test_BGM1")って書いてください。
    /// 2つ目の引数にはループするかしないかを指定してください→指定なしの場合はループします。
    /// </summary>
    /// <param name="bgm_name">BGM名</param>
    /// <param name="loop">ループするかしないか</param>
    public void PlayBGM_Sound(string bgm_name)
    {
        PlayBGM_Sound(bgm_name, true);
    }



    /// <summary>
    /// 3つ目の引数で指定した秒だけ遅れて、BGMを再生します。
    /// 「test_BGM1.mp3」を呼ぶ場合は("test_BGM1")って書いてください。
    /// 2つ目の引数にはループするかしないかを指定してください→指定なしの場合はループします。
    /// </summary>
    /// <param name="bgm_name">BGMの名前</param>
    /// <param name="loop">ループするかしないか</param>
    /// <param name="wait_time">遅延する秒数</param>
    /// <returns></returns>
    /// 
    public void PlayBGM_Delay_Sound(string bgm_name, bool loop, float wait_time)
    {
        StartCoroutine(PlayBGM_Delay_Sound_C(bgm_name, loop, wait_time));
    }

    /// <summary>
    /// 3つ目の引数で指定した秒だけ遅れて、BGMを再生します。
    /// 「test_BGM1.mp3」を呼ぶ場合は("test_BGM1")って書いてください。
    /// 2つ目の引数にはループするかしないかを指定してください→指定なしの場合はループします。
    /// </summary>
    /// <param name="bgm_name">BGMの名前</param>
    /// <param name="wait_time">遅延する秒数</param>
    /// <returns></returns>
    public void PlayBGM_Delay_Sound(string bgm_name, float wait_time)
    {
        StartCoroutine(PlayBGM_Delay_Sound_C(bgm_name, true, wait_time));
    }

    private IEnumerator PlayBGM_Delay_Sound_C(string bgm_name, bool loop, float wait_time)
    {
        yield return new WaitForSeconds(wait_time);
        PlayBGM_Sound(bgm_name, loop);
        yield break;
    }



    /// <summary>
    /// 指定したSEを再生する(SE名に拡張子は含まないことに注意!)
    /// 「test_SE1.ogg」を呼ぶ場合は("test_SE1")って書いてください
    /// </summary>
    /// <param name="se_name">SE名</param>
    public void PlaySE_Sound(string se_name)
    {
        //リストの中から検索
        play_se = se_list.Find(n => n.name == se_name);

        if (play_se == null)
        {//空だったらエラー
            Debug.Log(se_name + ":SEの音声ファイルが空です");
        }


        _audio_sources[1].PlayOneShot(play_se, (float)SE_volume / 100);
    }

    /// <summary>
    /// 2つ目の引数で指定した秒だけ遅れて、SEを再生します。
    /// 「test_SE1.ogg」を呼ぶ場合は("test_SE1")って書いてください
    /// </summary>
    /// <param name="se_name">SEの名前</param>
    /// <param name="wait_time">遅延する秒数</param>
    /// <returns></returns>
    /// 
    public void PlaySE_Delay_Sound(string se_name, float wait_time)
    {
        StartCoroutine(PlaySE_Delay_Sound_C(se_name, wait_time));
    }
    private IEnumerator PlaySE_Delay_Sound_C(string se_name, float wait_time)
    {
        yield return new WaitForSeconds(wait_time);
        PlaySE_Sound(se_name);
    }


    /// <summary>
    /// 現在再生しているBGMを停止します。一時停止ではなく完全停止です。
    /// </summary>
    public void StopBGM_Sound()
    {
        _audio_sources[0].Stop();
        Debug.Log("BGMを停止しました");
    }


    /// <summary>
    /// 現在再生しているBGMを一時停止します。
    /// </summary>
    public void PauseBGM_Sound()
    {
        _audio_sources[0].Pause();
        Debug.Log("BGMを一時停止しました");
    }
    
    /// <summary>
    /// 一時停止しているBGMを再生します。
    /// </summary>

    public void UnPauseBGM_Sound()
    {
        _audio_sources[0].UnPause();
        Debug.Log("BGMの一時停止を解除しました");
    }

    /// <summary>
    /// 現在再生しているBGMをミュートします。
    /// </summary>
    public void MuteBGM_Sound()
    {
        _audio_sources[0].mute = true;
        Debug.Log("BGMをミュートにしました");
    }

    /// <summary>
    /// BGMのミュートを解除します。
    /// </summary>
    public void UnMuteBGM_Sound()
    {
        _audio_sources[0].mute = false;
        Debug.Log("BGMのミュートを解除しました");
    }


    public void StopDelayPlay()
    {
        StopAllCoroutines();
    }
}


