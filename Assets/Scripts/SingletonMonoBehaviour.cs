using UnityEngine;

/// <summary>
/// MonoBehaviourに対応したシングルトンクラス
/// （例）public class GameManager : SingletonMonoBehaviour<GameManager>
/// </summary>
/// abstract 継承専用
/// where T : 必ずmonobehaviourを参照してね
public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));

                if (instance == null)
                {
                    Debug.LogError(typeof(T) + "をアタッチしているGameObjectが存在しない");
                }
            }
            return instance;
        }
    }

    //virtual 継承先で書いてなくても書いていることになる
    virtual protected void Awake()
    {
        // 他のGameObjectにアタッチされているか調べる
        if (this != Instance)
        {
            // アタッチされている場合は破棄する
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }
/*
    ＜シングルトンクラスにてAwakeをoverrideする場合は以下を追記＞
    protected override void Awake()
    {
        //--------------------------------
        // DontDestroyOnLoadをする場合
        base.Awake();

        //--------------------------------
        // DontDestroyOnLoadしない場合
        if (this != Instance)
        {
            Destroy(this.gameObject);
            return;
        }

        // 以下、追加したい処理...
    }
*/
}
