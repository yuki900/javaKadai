using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTank : MonoBehaviour
{
    [SerializeField,
    Header("胴体の移動速度")]       protected float _bodyMoveSpeed = 3.0f;     // 戦車胴体の速さ  
    [SerializeField,
    Header("射程範囲")]             private float _attackDistance;           // 弾の射程距離(※現在は直線のみ)
    [SerializeField,
    Header("砲身")]                 private GameObject _turret;              // 砲身   
    [SerializeField]                private GameObject[] _wheels;            // タイヤ
    [SerializeField]                private Transform _rayStartPos;          // レイ原点 

    protected enum TurretRataDir                 // 砲身の回転方向
    { None, Right, Left };
    protected TurretRataDir _ratadir;
    protected enum BodyRataDir                   // 胴体の旋回方向
    { None, Right, Left };
    protected BodyRataDir _bodyRataDir;

    protected LineRenderer _lineRenderer;         // ラインレンダラー
    protected List<Vector3> linePoints;           // ライン用座標リスト(始点、交点、終点)
    protected Quaternion _bodyrotation;


    virtual protected void Start()
    {
        _ratadir = TurretRataDir.None;
        _bodyRataDir = BodyRataDir.None;
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        linePoints = new List<Vector3>();
    }

    /// <summary>
    /// 旋回処理
    /// </summary>
    protected void RatationBody(in float addAngle)
    {

        switch (_bodyRataDir)
        {
            case BodyRataDir.None:  // 旋回しない
                // Debug.Log("旋回しない");            
                break;
            case BodyRataDir.Right: // 右旋回
                // Y軸を基準に回転
                Quaternion rot = Quaternion.AngleAxis(addAngle, transform.up);
                // 自身の回転情報を取得
                Quaternion q = transform.rotation;
                // 合成して自身に代入
                transform.rotation = q * rot;
                RotationWheels(Vector3.right);
                // 回転情報を保持
                _bodyrotation = rot;
                break;
            case BodyRataDir.Left:  // 左旋回
                                    // Y軸を基準に1度回転
                rot = Quaternion.AngleAxis(-addAngle, transform.up);
                // 自身の回転情報を取得
                q = transform.transform.rotation;
                // 合成して自身に代入
                transform.rotation = q * rot;
                RotationWheels(Vector3.right);
                // 回転情報を保持
                _bodyrotation = rot;
                break;
            default:
                break;
        }
    }

    protected void RotationWheels(
       in Vector3 rotaForward
       )
    {
        // タイヤの回転
        for (int i = 0; i < _wheels.Length; ++i)
        {
            // 1度の回転量
            float degree = 4.0f;
            // Y軸を基準に回転
            Quaternion rot = Quaternion.AngleAxis(degree, rotaForward);
            // 自身の回転情報を取得
            Quaternion q = _wheels[i].transform.rotation;
            // 合成して自身に代入
            _wheels[i].transform.rotation = q * rot;
        }
    }

    /// <summary>
    /// 砲塔の回転   
    /// </summary>
    protected void RatationTurret(in float AddAngle)
    {
        switch (_ratadir)
        {
            case TurretRataDir.None:  // 回転しない

                // 車体旋回時に親子の影響を無視した砲塔制御
                if (_bodyRataDir == BodyRataDir.Left ||
                    _bodyRataDir == BodyRataDir.Right)
                {
                    // 砲塔の回転情報を取得
                    Quaternion q = _turret.transform.rotation;
                    // 逆数をかけ合わせた物を代入
                    _turret.transform.rotation
                        = q * Quaternion.Inverse(_bodyrotation);
                }

                break;
            case TurretRataDir.Right: // 右旋回
                _turret.transform.Rotate(0, AddAngle, 0, Space.World);

                break;
            case TurretRataDir.Left:  // 左旋回
                _turret.transform.Rotate(0, -AddAngle, 0, Space.World);

                break;
        }
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="forward"></param>
    /// <param name="maxLength"></param>
    protected void BallisticPredictionLine(
              in Vector3 startPos,
              in Vector3 forward,
              in float maxLength            
              )
    {
        Ray ray = new Ray(startPos, forward);
        RaycastHit hitInfo;
        // 予測線の発射位置
        Vector3 rayOriginPosition = startPos;
        // 予測線の終了位置
        Vector3 rayEndPosition = startPos + forward * maxLength;

        // Debug.DrawRay(startPos, forward * maxLength, Color.red); // 直線ベクトルを赤色表示
        if (Physics.Raycast(ray, out hitInfo, maxLength))
        {
            // 壁であれば反射
            if (hitInfo.collider.gameObject.layer ==
               (int)GameManager.GameLayer.Wall)
            {
                // 当たった位置との距離
                var distance = hitInfo.distance;
                // 反射後の残りの距離
                var rDistance = maxLength - distance;
                // 終了位置を更新
                rayEndPosition = hitInfo.point;
                // 予測線座標リストに追加
                linePoints.Add(rayEndPosition);
                // 反射ベクトルを計算する
                Vector3 incidentDirection = (rayEndPosition - rayOriginPosition).normalized;       // 入射ベクトル
                Vector3 surfaceNormal = hitInfo.normal.normalized;                                 // 反射面の法線ベクトル
                // L' = 2 * N * (L・N) - L
                float a = Mathf.Abs(Vector3.Dot(incidentDirection, surfaceNormal));
                Vector3 refDir = incidentDirection + (2 * (surfaceNormal * a));
                // 反射ベクトルをデバッグ表示
                //  Debug.DrawRay(rayOriginPosition, incidentDirection * maxLength, Color.red); // 入射ベクトルを赤色で表示
                //  Debug.DrawRay(hitInfo.point, reflectedDirection * maxLength, Color.green);  // 反射ベクトルを緑色で表示

                // 反射したので再帰
                BallisticPredictionLine(rayEndPosition, refDir, rDistance);
            }
            else
            {
                rayEndPosition = hitInfo.point;
            }
        }
        // 幅指定
        _lineRenderer.startWidth = 0.04f;
        _lineRenderer.endWidth = 0.04f;
        // 予測線座標リストに追加
        linePoints.Add(rayEndPosition);
        // ラインの数指定
        _lineRenderer.positionCount = linePoints.Count;
        // 予測線を可視化
        for (int i = 0; i < linePoints.Count; ++i)
        {
            _lineRenderer.SetPosition(i, linePoints[i]);
        }

    }









    
    

}
