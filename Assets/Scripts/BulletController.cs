using System.Collections.Generic;
using UnityEngine;

public class BulletController  : MonoBehaviour 
{
   
    // 弾の座標(発射時)
    private Vector3 _firePoint;
    // 弾の移動方向
    private Vector3 _direction;
    // 弾の速さ
    private float _speed;
    // 弾オブジェクト
    private GameObject _bulletObject;
    private List<Vector3> _moveRouteList;
    private int _reflectionCount;
    private int _reflectionMax;
    private int _typeNum;
    private int _power;
    private List<Vector3> movePos;

    private enum State { None,Move, Die }
    private State _state;
    // 初期化処理
    public void Initialize(
        in float speed,
        in GameObject bulletObj,
        in int reflectionMax,
        in int power,
        in int typeNum
        )
    {
        _speed = speed;
        _bulletObject = bulletObj;
        _moveRouteList = new List<Vector3>();
       _reflectionMax = reflectionMax;
        _power = power;
        _typeNum = typeNum;
        movePos = new List<Vector3>();
    }
    // 弾設定
    public void SetBullet(
        in Vector3 pos,
        in Vector3 dir,
        in List<Vector3> routeList
        )
    {
        _firePoint = pos;
        _direction = dir;
        transform.forward = dir;
       
        transform.position = pos;
        _reflectionCount = 0;
       
        for (int i = 0; i < routeList.Count; i++)
        {
            _moveRouteList.Add(routeList[i]);
        }

        CreateRoute(pos,dir,ref _reflectionCount);

        _state = State.Move;
    }

    /// <summary>
    /// 移動ルートの作成
    /// </summary>
    /// <param name="startPos"> 開始位置     </param>
    /// <param name="forward">  方向         </param>
    /// <param name="counter">  現在の反射数 </param>
    private void CreateRoute(
        in Vector3 startPos,
        in Vector3 forward,
        ref int counter)
    {
        // 反射上限を超えてたら終了
        if(counter > _reflectionMax)
            return;
        
        Ray ray = new Ray(startPos, forward);
        RaycastHit hitInfo;
        // 予測線の発射位置
        Vector3 rayOriginPosition = startPos;
        // 予測線の終了位置
        Vector3 rayEndPosition;
        // レイの最大距離(適当に大きい値でOK)
        float maxLength = 100.0f;
        if (Physics.Raycast(ray, out hitInfo, maxLength))
        {
            // 壁であれば反射
            if (hitInfo.collider.gameObject.layer ==
                (int)GameManager.GameLayer.Wall)
            {           
                // 終了位置を更新
                rayEndPosition = hitInfo.point;

                // 反射ベクトルを計算する
                // 入射ベクトル
                Vector3 incidentDirection = (rayEndPosition - rayOriginPosition).normalized;
                // 反射面の法線ベクトル
                Vector3 surfaceNormal = hitInfo.normal.normalized;  
                // 反射ベクトル
                Vector3 refDir = ReflectedDirection(incidentDirection,surfaceNormal);
                // 弾の移動先に追加
                movePos.Add(rayEndPosition);
                // カウンターを増やす
                counter++;
               // Debug.Log(counter);
                // 反射したので再帰
                CreateRoute(rayEndPosition, refDir,ref counter);
            }
            else // 壁じゃない何かしらのオブジェクトだったら
            {
                // 反射終了
                rayEndPosition = hitInfo.point;
                movePos.Add(rayEndPosition);
                return;
            }
        }
    }


    /// <summary>
    /// 弾の描画を切り替える
    /// </summary>
    /// <param name="isActive"> 描画有無 </param>
    public void IsActive( in bool isActive ) 
        => transform.gameObject.SetActive( isActive );
    /// <summary>
    /// 現在のアクティブ状態
    /// </summary>
    /// <returns> アクティブ状態 </returns>
    public bool Activity() {
        return transform.gameObject.activeSelf;}

   
    private void Update()
    {
       
        switch (_state)
        {
            case State.None: // 何もない状態
                break;
            case State.Move: // 弾移動処理

                // 方向
                var direction = (movePos[0] - transform.position).normalized;
                // 距離
                var magnitude = (movePos[0] - transform.position).sqrMagnitude;
                //Debug.DrawRay(movePos[0], direction * 10,Color.white);
                transform.forward = direction;
                float epsilon = 0.05f;
                if (magnitude < epsilon)
                {
                    movePos.RemoveAt(0);

                    if (movePos.Count == 0)
                        _state = State.Die;
                }

                // 移動処理
                transform.position += direction * _speed * Time.deltaTime;
               
                break;
            case State.Die:  // 弾死亡処理
                // 爆発処理
                Explosion();
                IsActive(false);
                movePos.Clear();
                _state = State.None;
                break;
            default:
                Debug.LogError("Bullet State Error");
                break;
        }

      
    }
    


    

    /// <summary>
    /// 面に対しての反射ベクトルの計算
    /// </summary>
    /// <param name="incidentDir">   入射ベクトル         </param>
    /// <param name="surfaceNormal"> 反射面の法線ベクトル </param>
    /// <returns></returns>
    private Vector3 ReflectedDirection(
        in Vector3 incidentDir,
        in Vector3 surfaceNormal)
    {
        // L' = 2 * N * (L・N) - L
        return incidentDir + (2 * (surfaceNormal * Mathf.Abs(Vector3.Dot(incidentDir, surfaceNormal))));
    }

    private void Explosion()
    {

    }
 
    /// ※将来的にはコリジョン使わない方法を考える <summary>
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        // レイヤー番号がプレイヤーだったら
        if (collision.gameObject.layer ==
            (int)GameManager.GameLayer.Player)
        {
            // 自分か相手か調べる
            for (int i = 0; i < FieldManager.Instance.insetanceTank; ++i)
            {
                // 当たったオブジェクトとタンクが一致していたら 
                if (collision.collider.transform.parent.parent.gameObject ==
                    FieldManager.Instance.tanks[i].gameObject)
                {
                    // もし戦車のタイプ(色)が違ったらダメージ処理 
                    if (_typeNum != FieldManager.Instance.tanks[i].RetrieveType())
                    {
                        FieldManager.Instance.tanks[i].TakeDamage(_power);
                        _state = State.Die;
                    }
                    // これ以上調べる必要がないので抜ける
                    break;
                }
            }
        }
        // 箱オブジェだったら
        else if(collision.gameObject.layer == 
            (int)GameManager.GameLayer.Box)
        {

            // 自分か相手か調べる
            for (int i = 0; i < FieldManager.Instance.instanceBox; ++i)
            {
                // 当たったオブジェクトとタンクが一致していたら 
                if (collision.gameObject == FieldManager.Instance.boxes[i]._obj)
                {
                    FieldManager.Instance.boxes[i].IsActive(false);
                    _state = State.Die;                   
                    // これ以上調べる必要がないので抜ける
                    break;
                }
            }

           
        }
        // 自分の弾または相手の弾だったら
        else if(collision.gameObject.layer ==
            (int)GameManager.GameLayer.Bullet)
        {
            _state = State.Die;
        }


    }

}
