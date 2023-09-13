using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTank : MonoBehaviour
{
    [SerializeField,
    Header("���̂̈ړ����x")]       protected float _bodyMoveSpeed = 3.0f;     // ��ԓ��̂̑���  
    [SerializeField,
    Header("�˒��͈�")]             private float _attackDistance;           // �e�̎˒�����(�����݂͒����̂�)
    [SerializeField,
    Header("�C�g")]                 private GameObject _turret;              // �C�g   
    [SerializeField]                private GameObject[] _wheels;            // �^�C��
    [SerializeField]                private Transform _rayStartPos;          // ���C���_ 

    protected enum TurretRataDir                 // �C�g�̉�]����
    { None, Right, Left };
    protected TurretRataDir _ratadir;
    protected enum BodyRataDir                   // ���̂̐������
    { None, Right, Left };
    protected BodyRataDir _bodyRataDir;

    protected LineRenderer _lineRenderer;         // ���C�������_���[
    protected List<Vector3> linePoints;           // ���C���p���W���X�g(�n�_�A��_�A�I�_)
    protected Quaternion _bodyrotation;


    virtual protected void Start()
    {
        _ratadir = TurretRataDir.None;
        _bodyRataDir = BodyRataDir.None;
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        linePoints = new List<Vector3>();
    }

    /// <summary>
    /// ���񏈗�
    /// </summary>
    protected void RatationBody(in float addAngle)
    {

        switch (_bodyRataDir)
        {
            case BodyRataDir.None:  // ���񂵂Ȃ�
                // Debug.Log("���񂵂Ȃ�");            
                break;
            case BodyRataDir.Right: // �E����
                // Y������ɉ�]
                Quaternion rot = Quaternion.AngleAxis(addAngle, transform.up);
                // ���g�̉�]�����擾
                Quaternion q = transform.rotation;
                // �������Ď��g�ɑ��
                transform.rotation = q * rot;
                RotationWheels(Vector3.right);
                // ��]����ێ�
                _bodyrotation = rot;
                break;
            case BodyRataDir.Left:  // ������
                                    // Y�������1�x��]
                rot = Quaternion.AngleAxis(-addAngle, transform.up);
                // ���g�̉�]�����擾
                q = transform.transform.rotation;
                // �������Ď��g�ɑ��
                transform.rotation = q * rot;
                RotationWheels(Vector3.right);
                // ��]����ێ�
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
        // �^�C���̉�]
        for (int i = 0; i < _wheels.Length; ++i)
        {
            // 1�x�̉�]��
            float degree = 4.0f;
            // Y������ɉ�]
            Quaternion rot = Quaternion.AngleAxis(degree, rotaForward);
            // ���g�̉�]�����擾
            Quaternion q = _wheels[i].transform.rotation;
            // �������Ď��g�ɑ��
            _wheels[i].transform.rotation = q * rot;
        }
    }

    /// <summary>
    /// �C���̉�]   
    /// </summary>
    protected void RatationTurret(in float AddAngle)
    {
        switch (_ratadir)
        {
            case TurretRataDir.None:  // ��]���Ȃ�

                // �ԑ̐��񎞂ɐe�q�̉e���𖳎������C������
                if (_bodyRataDir == BodyRataDir.Left ||
                    _bodyRataDir == BodyRataDir.Right)
                {
                    // �C���̉�]�����擾
                    Quaternion q = _turret.transform.rotation;
                    // �t�����������킹��������
                    _turret.transform.rotation
                        = q * Quaternion.Inverse(_bodyrotation);
                }

                break;
            case TurretRataDir.Right: // �E����
                _turret.transform.Rotate(0, AddAngle, 0, Space.World);

                break;
            case TurretRataDir.Left:  // ������
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
        // �\�����̔��ˈʒu
        Vector3 rayOriginPosition = startPos;
        // �\�����̏I���ʒu
        Vector3 rayEndPosition = startPos + forward * maxLength;

        // Debug.DrawRay(startPos, forward * maxLength, Color.red); // �����x�N�g����ԐF�\��
        if (Physics.Raycast(ray, out hitInfo, maxLength))
        {
            // �ǂł���Δ���
            if (hitInfo.collider.gameObject.layer ==
               (int)GameManager.GameLayer.Wall)
            {
                // ���������ʒu�Ƃ̋���
                var distance = hitInfo.distance;
                // ���ˌ�̎c��̋���
                var rDistance = maxLength - distance;
                // �I���ʒu���X�V
                rayEndPosition = hitInfo.point;
                // �\�������W���X�g�ɒǉ�
                linePoints.Add(rayEndPosition);
                // ���˃x�N�g�����v�Z����
                Vector3 incidentDirection = (rayEndPosition - rayOriginPosition).normalized;       // ���˃x�N�g��
                Vector3 surfaceNormal = hitInfo.normal.normalized;                                 // ���˖ʂ̖@���x�N�g��
                // L' = 2 * N * (L�EN) - L
                float a = Mathf.Abs(Vector3.Dot(incidentDirection, surfaceNormal));
                Vector3 refDir = incidentDirection + (2 * (surfaceNormal * a));
                // ���˃x�N�g�����f�o�b�O�\��
                //  Debug.DrawRay(rayOriginPosition, incidentDirection * maxLength, Color.red); // ���˃x�N�g����ԐF�ŕ\��
                //  Debug.DrawRay(hitInfo.point, reflectedDirection * maxLength, Color.green);  // ���˃x�N�g����ΐF�ŕ\��

                // ���˂����̂ōċA
                BallisticPredictionLine(rayEndPosition, refDir, rDistance);
            }
            else
            {
                rayEndPosition = hitInfo.point;
            }
        }
        // ���w��
        _lineRenderer.startWidth = 0.04f;
        _lineRenderer.endWidth = 0.04f;
        // �\�������W���X�g�ɒǉ�
        linePoints.Add(rayEndPosition);
        // ���C���̐��w��
        _lineRenderer.positionCount = linePoints.Count;
        // �\����������
        for (int i = 0; i < linePoints.Count; ++i)
        {
            _lineRenderer.SetPosition(i, linePoints[i]);
        }

    }









    
    

}
