using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class BulletManager : MonoBehaviour
{

    [SerializeField] private Tank tank;
    // 今のところは全ての弾に適応
    [SerializeField, Header("弾の速度")]
    private float bulletMoveSpeed;
    // 弾のエントリー最大数
    private readonly int MAX_ENTRY_INSTANCE = 5;
    // 弾の最大飛距離
    //private float maxBulletDistance = 100f;
    // 弾の倉庫
    private List<BulletController> storage; 
    private const int REFLECTION_MAX = 4;
    [SerializeField] private GameObject _bullet;                 // 弾オブジェクト
    private int power = 1;
   
    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <param name="tankTypeNum"></param>
    public void Initialize(in int TankTypeNum)
    {
        // 初期化
        // キャパシティを指定(最大より少し大きめで確保 + 4)
        storage = new List<BulletController>(MAX_ENTRY_INSTANCE + 4);
     
        for (int i = 0; i < MAX_ENTRY_INSTANCE; ++i)
        {
            // 弾を生成
            var bulletObj = Instantiate(_bullet,
                                        Vector3.zero,
                                        Quaternion.identity).GetComponent<BulletController>();
            // 初期化処理
            bulletObj.Initialize(bulletMoveSpeed,
                                 _bullet,
                                 REFLECTION_MAX,
                                 power,
                                 TankTypeNum);
            // 描画を切る
            bulletObj.IsActive(false);
            // 倉庫に保存
            storage.Add(bulletObj);
           
        }
    }
   
    /// <summary>
    /// 弾の生成
    /// </summary>
    public void CreateBullet(
        in Vector3 firePos,
        in Vector3 dir
        )
    {
        // リストから非アクティブな弾丸を探す
        foreach (BulletController bullet in storage)
        {
            if (!bullet.Activity())
            {
                // 弾丸を発射位置に移動してアクティブにする               
                bullet.SetBullet(firePos, dir, tank.GetLinePoints());
                bullet.IsActive(true);
                // ループを抜ける
                break;
            }
        }

        
    } 
}
