using System.Collections.Generic;
using UnityEngine;

public class BulletController  : MonoBehaviour 
{
   
    // �e�̍��W(���ˎ�)
    private Vector3 _firePoint;
    // �e�̈ړ�����
    private Vector3 _direction;
    // �e�̑���
    private float _speed;
    // �e�I�u�W�F�N�g
    private GameObject _bulletObject;
    private List<Vector3> _moveRouteList;
    private int _reflectionCount;
    private int _reflectionMax;
    private int _typeNum;
    private int _power;
    private List<Vector3> movePos;

    private enum State { None,Move, Die }
    private State _state;
    // ����������
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
    // �e�ݒ�
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
    /// �ړ����[�g�̍쐬
    /// </summary>
    /// <param name="startPos"> �J�n�ʒu     </param>
    /// <param name="forward">  ����         </param>
    /// <param name="counter">  ���݂̔��ː� </param>
    private void CreateRoute(
        in Vector3 startPos,
        in Vector3 forward,
        ref int counter)
    {
        // ���ˏ���𒴂��Ă���I��
        if(counter > _reflectionMax)
            return;
        
        Ray ray = new Ray(startPos, forward);
        RaycastHit hitInfo;
        // �\�����̔��ˈʒu
        Vector3 rayOriginPosition = startPos;
        // �\�����̏I���ʒu
        Vector3 rayEndPosition;
        // ���C�̍ő勗��(�K���ɑ傫���l��OK)
        float maxLength = 100.0f;
        if (Physics.Raycast(ray, out hitInfo, maxLength))
        {
            // �ǂł���Δ���
            if (hitInfo.collider.gameObject.layer ==
                (int)GameManager.GameLayer.Wall)
            {           
                // �I���ʒu���X�V
                rayEndPosition = hitInfo.point;

                // ���˃x�N�g�����v�Z����
                // ���˃x�N�g��
                Vector3 incidentDirection = (rayEndPosition - rayOriginPosition).normalized;
                // ���˖ʂ̖@���x�N�g��
                Vector3 surfaceNormal = hitInfo.normal.normalized;  
                // ���˃x�N�g��
                Vector3 refDir = ReflectedDirection(incidentDirection,surfaceNormal);
                // �e�̈ړ���ɒǉ�
                movePos.Add(rayEndPosition);
                // �J�E���^�[�𑝂₷
                counter++;
               // Debug.Log(counter);
                // ���˂����̂ōċA
                CreateRoute(rayEndPosition, refDir,ref counter);
            }
            else // �ǂ���Ȃ���������̃I�u�W�F�N�g��������
            {
                // ���ˏI��
                rayEndPosition = hitInfo.point;
                movePos.Add(rayEndPosition);
                return;
            }
        }
    }


    /// <summary>
    /// �e�̕`���؂�ւ���
    /// </summary>
    /// <param name="isActive"> �`��L�� </param>
    public void IsActive( in bool isActive ) 
        => transform.gameObject.SetActive( isActive );
    /// <summary>
    /// ���݂̃A�N�e�B�u���
    /// </summary>
    /// <returns> �A�N�e�B�u��� </returns>
    public bool Activity() {
        return transform.gameObject.activeSelf;}

   
    private void Update()
    {
       
        switch (_state)
        {
            case State.None: // �����Ȃ����
                break;
            case State.Move: // �e�ړ�����

                // ����
                var direction = (movePos[0] - transform.position).normalized;
                // ����
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

                // �ړ�����
                transform.position += direction * _speed * Time.deltaTime;
               
                break;
            case State.Die:  // �e���S����
                // ��������
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
    /// �ʂɑ΂��Ă̔��˃x�N�g���̌v�Z
    /// </summary>
    /// <param name="incidentDir">   ���˃x�N�g��         </param>
    /// <param name="surfaceNormal"> ���˖ʂ̖@���x�N�g�� </param>
    /// <returns></returns>
    private Vector3 ReflectedDirection(
        in Vector3 incidentDir,
        in Vector3 surfaceNormal)
    {
        // L' = 2 * N * (L�EN) - L
        return incidentDir + (2 * (surfaceNormal * Mathf.Abs(Vector3.Dot(incidentDir, surfaceNormal))));
    }

    private void Explosion()
    {

    }
 
    /// �������I�ɂ̓R���W�����g��Ȃ����@���l���� <summary>
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        // ���C���[�ԍ����v���C���[��������
        if (collision.gameObject.layer ==
            (int)GameManager.GameLayer.Player)
        {
            // ���������肩���ׂ�
            for (int i = 0; i < FieldManager.Instance.insetanceTank; ++i)
            {
                // ���������I�u�W�F�N�g�ƃ^���N����v���Ă����� 
                if (collision.collider.transform.parent.parent.gameObject ==
                    FieldManager.Instance.tanks[i].gameObject)
                {
                    // ������Ԃ̃^�C�v(�F)���������_���[�W���� 
                    if (_typeNum != FieldManager.Instance.tanks[i].RetrieveType())
                    {
                        FieldManager.Instance.tanks[i].TakeDamage(_power);
                        _state = State.Die;
                    }
                    // ����ȏ㒲�ׂ�K�v���Ȃ��̂Ŕ�����
                    break;
                }
            }
        }
        // ���I�u�W�F��������
        else if(collision.gameObject.layer == 
            (int)GameManager.GameLayer.Box)
        {

            // ���������肩���ׂ�
            for (int i = 0; i < FieldManager.Instance.instanceBox; ++i)
            {
                // ���������I�u�W�F�N�g�ƃ^���N����v���Ă����� 
                if (collision.gameObject == FieldManager.Instance.boxes[i]._obj)
                {
                    FieldManager.Instance.boxes[i].IsActive(false);
                    _state = State.Die;                   
                    // ����ȏ㒲�ׂ�K�v���Ȃ��̂Ŕ�����
                    break;
                }
            }

           
        }
        // �����̒e�܂��͑���̒e��������
        else if(collision.gameObject.layer ==
            (int)GameManager.GameLayer.Bullet)
        {
            _state = State.Die;
        }


    }

}
