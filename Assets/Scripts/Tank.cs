using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static InputContoller;

//[RequireComponent(typeof(Rigidbody))]
public class Tank : BaseTank
{
    [SerializeField]                private BulletManager _bulletManager;    // �e�Ǘ��N���X  
    [SerializeField,
    Header("����")]                 private GameObject _body;                // ��Ԃ̓���   
    [SerializeField]                private GameObject _firePoint;           // �e���ˍ��W
    [SerializeField]                private HP_UI _hpUI;
    [SerializeField]                private GameObject icon;                      // �����̃A�C�R��
    public enum Type { Blue, Red }
    [SerializeField] private Type _type;
    private const int MAX_HP = 3;
    private int _hp;
    
    private Vector3 _velocity;                  // �ړ���  
    private Vector3 _oldVelocity;               // �v�Z�O�̈ړ���  
    private float _vertical;                    // �ړ�����(�O��)
    private float _turretAddAngle = 1.0f;       // ��x�̉�]��
    private float _bodyAddAngle = 1;            // ��x�̉�]��
    private Rigidbody _rb;                      // ��������
    private Vector3 _fireBulletPos;             // �e���ˍ��W    
    private Vector3 _fireBulletForward;         // �e���ˎ��̖C�g����   
    private InputContoller _inputContoller;
    private bool isFire;
    // �����̎q���̃����_�[
    private Renderer[] renderers;
    // �����_�[��������悤�ɂ��邩�ǂ���
    private bool isEnabledRenderers;
    // �_���[�W��
    private bool _isDamage;
    // ���G����
    private float _invincibleTime = 2.0f;
    //�_���[�W�_�ō��v����    
    float _totalFlashingTime;
    //�_���[�W�_�Ń����_�[�̗L��
     private float _flashingTime;
    //�_���[�W�_�ŃC���^�[�o���B
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
        // �ړ���ԂȂ�^�C������]������
        if (_vertical != 0)
        {
            // �L�����N�^�̈ړ������͏��+Z�Ȃ̂�Vector3.right�Ő�������
            base.RotationWheels(Vector3.right);
        }

        // �C�g�̕���
        _fireBulletForward = _firePoint.transform.forward;
        // �e�̐������W
        _fireBulletPos = _firePoint.transform.position;
        float maxLength = 5.0f;
        // �\�������W���X�g�ɒǉ�
        base.linePoints.Add(_fireBulletPos);
        // �e���\������`��
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
    /// �i�s���������߂�
    /// </summary>
    private void InputUpdate()
    {

        switch (_inputContoller.GetOperation())
        {
            case Operation.Keybord: // �L�[�{�[�h����

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
            case Operation.Controller: // �R���g���[���[����

                // �����͂�������Ă����ꍇ
                if (_inputContoller.GetLeftButton())
                    _bodyRataDir = BodyRataDir.Left;
                // �E���͂�������Ă����ꍇ                 
                if (_inputContoller.GetRightButton())
                    _bodyRataDir = BodyRataDir.Right;
                
                // ����͂�������Ă����ꍇ 
                if (_inputContoller.GetUpButton())
                    _vertical = +1.0f;
                // �����͂�������Ă����ꍇ 
                if(_inputContoller.GetDownButton()) 
                    _vertical = -1.0f;

                // �\���{�^���E���͂ƍ����͂�������ĂȂ��ꍇ
                if (!(_inputContoller.GetLeftButton()) &&
                  !(_inputContoller.GetRightButton()))
                    _bodyRataDir = BodyRataDir.None;

                // �\���{�^������͂Ɖ����͂�������ĂȂ��ꍇ
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
    /// �ړ��ʍX�V����
    /// </summary>
    /// <param name="v">     �O��������͒l </param>
    /// <param name="speed"> �ړ��X�s�[�h   </param>
    private void Movement(
        in float v,
        in float speed
        )
    {
        var lerpSpeed = 6.0f;
        // �ԑ̂̑O��������Ɉړ�����������
        _velocity = transform.forward * v * speed;
        // ��ԏ���
        _velocity = Vector3.Lerp(_oldVelocity,_velocity,lerpSpeed * Time.deltaTime);
        float epsilon = 0.001f;

        // �[���Ɍ���Ȃ��߂����
        if (_velocity.sqrMagnitude < epsilon * epsilon)
        {
            _velocity = Vector3.zero;
        }
        _oldVelocity = _velocity;
    }

    // �_���[�W����
    public void TakeDamage(in int damage)
    {
        // �_���[�W�������͎��̍U�����󂯂Ȃ�
        if (_isDamage)
            return;

        _hp = (int)Mathf.Clamp(_hp - damage, 0, MAX_HP);
        _hpUI.Damage(damage);
        // �_�ŏ���
         StartCoroutine(Flashing());

        if (_hp == 0)
        {
            _isGameOver = true;
            // ���U���g��
            // ���O�l�ΐ�ȍ~�͏����H�v����
            GameManager.Instance._gameState = GameManager.GameState.Reslut;
        }
    }


    /// <summary>
    /// �_������
    /// </summary>
    /// <returns></returns>
    private IEnumerator Flashing()
    {

        _isDamage = true;

        _totalFlashingTime = 0;
        _flashingTime = 0;

        // ���G���Ԃ̊ԓ_������
        while (true)
        {
            _totalFlashingTime += Time.deltaTime;
            _flashingTime += Time.deltaTime;

            // �_�ł���
            if(_flashingTime >= FLASHING_INTERVAL)
            {
                _flashingTime = 0;
                // �t���O�̔��]
                isEnabledRenderers = !isEnabledRenderers;
                SetEnabledRenderers(isEnabledRenderers);
            }

            // ���G���Ԍo�߂��Ă�����
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
    /// �����_�[�̗L��
    /// </summary>
    /// <param name="isActive"> �A�N�e�B�u��� </param>
    private void SetEnabledRenderers(in bool isActive)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = isActive;
        }
    }

    /// <summary>
    /// �v�Z���ʂ�rigidbody�ɔ��f
    /// </summary>
    private void MoveFixedUpdate()
    {
        var vel = _velocity;
        vel.y = _rb.velocity.y;
        _rb.velocity = vel;
    }
}
