using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    [SerializeField]
    private string playerTag = "Player";
    [SerializeField,Header("対象とのオフセット")]
    private Vector3 offset;
    [SerializeField]
    private Vector3 followPosition;
    // キャッシュした方が若干速いらしい
    private Transform thisTransform;
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag(playerTag);
        // キャッシュ
        thisTransform = transform;
    }


    void LateUpdate()
    {
        thisTransform.position = player.transform.position + followPosition;
        // プレイヤーへ向ける
        thisTransform.LookAt(player.transform.position + offset);
    }
}
