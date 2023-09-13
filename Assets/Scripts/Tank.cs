using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static InputContoller;

//[RequireComponent(typeof(Rigidbody))]
public class Tank : BaseTank
{
    [SerializeField]                private BulletManager _bulletManager;    // 弾管理クラス  
    [SerializeField,
    Header("胴体")]                 private GameObject _body;                // 戦車の胴体   
    [SerializeField]                private GameObject _firePoint;           // 弾発射座標
    [SerializeField]                private HP_UI _hpUI;
    [SerializeField]                private GameObject icon;                      // 自分のアイコン
    public enum Type { Blue, Red }
    [SerializeField] private Type _type;
    private const int MAX_HP = 3;
    private int _hp;
    
    private Vector3 _velocity;                  // 移動量  
    private Vector3 _oldVelocity;               // 計算前の移動量  
    private float _vertical;                    // 移動方向(前後)
    private float _turretAddAngle = 1.0f;       // 一度の回転量
    private float _bodyAddAngle = 1;            // 一度の回転量
    private Rigidbody _rb;                      // 物理剛体
    private Vector3 _fireBulletPos;             // 弾発射座標    
    private Vector3 _fireBulletForward;         // 弾発射時の砲身方向   
    private InputContoller _inputContoller;
    private bool isFire;
    // 自分の子供のレンダー
    private Renderer[] renderers;
    // レンダーを見えるようにするかどうか
    private bool isEnabledRenderers;
    // ダメージ中
    private bool _isDamage;
    // 無敵時間
    private float _invincibleTime = 2.0f;
    //ダメージ点滅合計時間    
    float _totalFlashingTime;
    //ダメージ点滅レンダーの有無
     private float _flashingTime;
    //ダメージ点滅インターバル。
    private const float FLASHING_INTERVAL = 0.075f;


    public bool _isGameOver { get; private set; }

    public List<Vector3> GetLinePoints() { return linePoints;}
    public int RetrieveType() {return (int)_type;}
    public GameObject GetIcon() { return icon;}

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _inputContoller = GetComponent<InputContoller>();
        renderers = GetComponentsInChildren<Renderer>(); 
    }

    override protected void Start()
    {
        base.Start();
        _hp = MAX_HP;     
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _hpUI.Initialize();

        switch (_type)
        {
            case Type.Blue:
                _lineRenderer.startColor = Color.blue;
                _lineRenderer.endColor = Color.blue;
                break;
            case Type.Red:
                _lineRenderer.startColor = Color.red;
                _lineRenderer.endColor = Color.red;
                break;
            default:
                break;
        }

       
        _bulletManager.Initialize((int)_type);
    }


    void Update()
    {
        if (Time.timeScale == 0)
            return;

        InputUpdate();
        base.RatationBody(_bodyAddAngle);
        base.RatationTurret(_turretAddAngle);
        Movement(_vertical, _bodyMoveSpeed);
        // 移動状態ならタイヤを回転させる
        if (_vertical != 0)
        {
            // キャラクタの移動方向は常に+ZなのでVector3.rightで成立する
            base.RotationWheels(Vector3.right);
        }

        // 砲身の方向
        _fireBulletForward = _firePoint.transform.forward;
        // 弾の生成座標
        _fireBulletPos = _firePoint.transform.position;
        float maxLength = 5.0f;
        // 予測線座標リストに追加
        base.linePoints.Add(_fireBulletPos);
        // 弾道予測線を描画
        base.BallisticPredictionLine(_fireBulletPos, _fireBulletForward, maxLength);
      
        if(isFire) 
        {
            _bulletManager.CreateBullet(_fireBulletPos, _fireBulletForward);
            isFire = false;
        }

        base.linePoints.Clear();
    }

  
    private void FixedUpdate()
    {
        MoveFixedUpdate();
    }

    /// <summary>
    /// 進行方向を決める
    /// </summary>
    private void InputUpdate()
    {

        switch (_inputContoller.GetOperation())
        {
            case Operation.Keybord: // キーボード操作

                if (Input.GetKey(KeyCode.A)) _bodyRataDir = BodyRataDir.Left;
                if (Input.GetKey(KeyCode.D)) _bodyRataDir = BodyRataDir.Right;
                if (Input.GetKey(KeyCode.W)) _vertical = +1.0f;
                if (Input.GetKey(KeyCode.S)) _vertical = -1.0f;
                if (Input.GetKey(KeyCode.LeftArrow)) _ratadir = TurretRataDir.Left;
                if (Input.GetKey(KeyCode.RightArrow)) _ratadir = TurretRataDir.Right;
                if(Input.GetKeyDown(KeyCode.Space)) isFire = true;


                if (Input.GetKeyUp(KeyCode.A)) _bodyRataDir = BodyRataDir.None;
                if (Input.GetKeyUp(KeyCode.D)) _bodyRataDir = BodyRataDir.None;
                if (Input.GetKeyUp(KeyCode.W)) _vertical = 0.0f;
                if (Input.GetKeyUp(KeyCode.S)) _vertical = 0.0f;
                if (Input.GetKeyUp(KeyCode.LeftArrow)) _ratadir = TurretRataDir.None;
                if (Input.GetKeyUp(KeyCode.RightArrow)) _ratadir = TurretRataDir.None;
                if (Input.GetKeyUp(KeyCode.Space)) isFire = false;

                break;
            case Operation.Controller: // コントローラー操作

                // 左入力が押されていた場合
                if (_inputContoller.GetLeftButton())
                    _bodyRataDir = BodyRataDir.Left;
                // 右入力が押されていた場合                 
                if (_inputContoller.GetRightButton())
                    _bodyRataDir = BodyRataDir.Right;
                
                // 上入力が押されていた場合 
                if (_inputContoller.GetUpButton())
                    _vertical = +1.0f;
                // 下入力が押されていた場合 
                if(_inputContoller.GetDownButton()) 
                    _vertical = -1.0f;

                // 十字ボタン右入力と左入力が押されてない場合
                if (!(_inputContoller.GetLeftButton()) &&
                  !(_inputContoller.GetRightButton()))
                    _bodyRataDir = BodyRataDir.None;

                // 十字ボタン上入力と下入力が押されてない場合
                if (!(_inputContoller.GetUpButton())&&
                   !(_inputContoller.GetDownButton()))
                    _vertical = 0.0f;


                if(_inputContoller.GetAButtonDown()) isFire = true;
                if(_inputContoller.GetYButtonDown()) isFire = true;
                if(_inputContoller.GetXButtonDown()) _ratadir = TurretRataDir.Left; 
                if(_inputContoller.GetBButtonDown()) _ratadir = TurretRataDir.Right;

                if (_inputContoller.GetAButtonUp()) isFire = false;
                if (_inputContoller.GetYButtonUp()) isFire = false;
                if (_inputContoller.GetXButtonUp()) _ratadir = TurretRataDir.None;
                if (_inputContoller.GetBButtonUp()) _ratadir = TurretRataDir.None;

                break;
            default:
                Debug.LogError("InputController Operation Error");
                break;
        }



       

    }

    /// <summary>
    /// 移動量更新処理
    /// </summary>
    /// <param name="v">     前後方向入力値 </param>
    /// <param name="speed"> 移動スピード   </param>
    private void Movement(
        in float v,
        in float speed
        )
    {
        var lerpSpeed = 6.0f;
        // 車体の前方向を基準に移動方向を決定
        _velocity = transform.forward * v * speed;
        // 補間処理
        _velocity = Vector3.Lerp(_oldVelocity,_velocity,lerpSpeed * Time.deltaTime);
        float epsilon = 0.001f;

        // ゼロに限りなく近ければ
        if (_velocity.sqrMagnitude < epsilon * epsilon)
        {
            _velocity = Vector3.zero;
        }
        _oldVelocity = _velocity;
    }

    // ダメージ処理
    public void TakeDamage(in int damage)
    {
        // ダメージ処理中は次の攻撃を受けない
        if (_isDamage)
            return;

        _hp = (int)Mathf.Clamp(_hp - damage, 0, MAX_HP);
        _hpUI.Damage(damage);
        // 点滅処理
         StartCoroutine(Flashing());

        if (_hp == 0)
        {
            _isGameOver = true;
            // リザルトへ
            // ※三人対戦以降は少し工夫する
            GameManager.Instance._gameState = GameManager.GameState.Reslut;
        }
    }


    /// <summary>
    /// 点灯処理
    /// </summary>
    /// <returns></returns>
    private IEnumerator Flashing()
    {

        _isDamage = true;

        _totalFlashingTime = 0;
        _flashingTime = 0;

        // 無敵時間の間点灯する
        while (true)
        {
            _totalFlashingTime += Time.deltaTime;
            _flashingTime += Time.deltaTime;

            // 点滅する
            if(_flashingTime >= FLASHING_INTERVAL)
            {
                _flashingTime = 0;
                // フラグの反転
                isEnabledRenderers = !isEnabledRenderers;
                SetEnabledRenderers(isEnabledRenderers);
            }

            // 無敵時間経過していたら
            if(_totalFlashingTime >= _invincibleTime)
            {
                _isDamage = false;

                isEnabledRenderers = true;
                SetEnabledRenderers(true);
                             
                yield break;
            }


            yield return null;
        }


    }


    /// <summary>
    /// レンダーの有無
    /// </summary>
    /// <param name="isActive"> アクティブ情報 </param>
    private void SetEnabledRenderers(in bool isActive)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = isActive;
        }
    }

    /// <summary>
    /// 計算結果をrigidbodyに反映
    /// </summary>
    private void MoveFixedUpdate()
    {
        var vel = _velocity;
        vel.y = _rb.velocity.y;
        _rb.velocity = vel;
    }
}
