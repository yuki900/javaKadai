using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class BulletManager : MonoBehaviour
{

    [SerializeField] private Tank tank;
    // ���̂Ƃ���͑S�Ă̒e�ɓK��
    [SerializeField, Header("�e�̑��x")]
    private float bulletMoveSpeed;
    // �e�̃G���g���[�ő吔
    private readonly int MAX_ENTRY_INSTANCE = 5;
    // �e�̍ő�򋗗�
    //private float maxBulletDistance = 100f;
    // �e�̑q��
    private List<BulletController> storage; 
    private const int REFLECTION_MAX = 4;
    [SerializeField] private GameObject _bullet;                 // �e�I�u�W�F�N�g
    private int power = 1;
   
    /// <summary>
    /// ����������
    /// </summary>
    /// <param name="tankTypeNum"></param>
    public void Initialize(in int TankTypeNum)
    {
        // ������
        // �L���p�V�e�B���w��(�ő��菭���傫�߂Ŋm�� + 4)
        storage = new List<BulletController>(MAX_ENTRY_INSTANCE + 4);
     
        for (int i = 0; i < MAX_ENTRY_INSTANCE; ++i)
        {
            // �e�𐶐�
            var bulletObj = Instantiate(_bullet,
                                        Vector3.zero,
                                        Quaternion.identity).GetComponent<BulletController>();
            // ����������
            bulletObj.Initialize(bulletMoveSpeed,
                                 _bullet,
                                 REFLECTION_MAX,
                                 power,
                                 TankTypeNum);
            // �`���؂�
            bulletObj.IsActive(false);
            // �q�ɂɕۑ�
            storage.Add(bulletObj);
           
        }
    }
   
    /// <summary>
    /// �e�̐���
    /// </summary>
    public void CreateBullet(
        in Vector3 firePos,
        in Vector3 dir
        )
    {
        // ���X�g�����A�N�e�B�u�Ȓe�ۂ�T��
        foreach (BulletController bullet in storage)
        {
            if (!bullet.Activity())
            {
                // �e�ۂ𔭎ˈʒu�Ɉړ����ăA�N�e�B�u�ɂ���               
                bullet.SetBullet(firePos, dir, tank.GetLinePoints());
                bullet.IsActive(true);
                // ���[�v�𔲂���
                break;
            }
        }

        
    } 
}
