using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    [SerializeField]
    private string playerTag = "Player";
    [SerializeField,Header("�ΏۂƂ̃I�t�Z�b�g")]
    private Vector3 offset;
    [SerializeField]
    private Vector3 followPosition;
    // �L���b�V�����������኱�����炵��
    private Transform thisTransform;
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag(playerTag);
        // �L���b�V��
        thisTransform = transform;
    }


    void LateUpdate()
    {
        thisTransform.position = player.transform.position + followPosition;
        // �v���C���[�֌�����
        thisTransform.LookAt(player.transform.position + offset);
    }
}
