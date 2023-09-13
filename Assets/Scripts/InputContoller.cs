using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 主にコントローラー操作用クラス
/// </summary>
public class InputContoller : MonoBehaviour
{
    [SerializeField,Header("左旋回")] private string leftArrow;
    [SerializeField,Header("右旋回")] private string rightArrow;
    [SerializeField,Header("前進")] private string upArrow;
    [SerializeField,Header("後退")] private string downArrow;
    [SerializeField,Header("左回転")] private string x;
    [SerializeField,Header("発射")] private string y;
    [SerializeField,Header("発射")] private string a;
    [SerializeField, Header("右回転")] private string b;

    public enum Operation { Keybord,Controller }
    [SerializeField] private Operation _operation;

    public Operation GetOperation() { return _operation; }



    void Update()
    {
        //if (GetLeftButton())
        //{
        //    Debug.Log("わーいllll");
        //}

        //if (GetDownButton())
        //{
        //    Debug.Log("わーいdddd");
        //}
        //if (GetUpButton())
        //{
        //    Debug.Log("わーいuuuuu");
        //}
        //if (GetRightButton())
        //{
        //    Debug.Log("わーいrrrrr");
        //}
    }

    // ----------- 沈み具合(今回はRaw) ---------
    public bool GetLeftButton(){ return Input.GetAxisRaw(leftArrow) > 0; }
    public bool GetRightButton() {return Input.GetAxisRaw(rightArrow) > 0; }
    public bool GetUpButton() {return Input.GetAxisRaw (upArrow) > 0; }
    public bool GetDownButton() {return Input.GetAxisRaw (downArrow) > 0; }


    // ------------- 押された瞬間 -------------
    public bool GetXButtonDown() { return Input.GetKeyDown(x); }
    public bool GetYButtonDown() { return Input.GetKeyDown(y); }
    public bool GetAButtonDown() { return Input.GetKeyDown(a); }
    public bool GetBButtonDown() { return Input.GetKeyDown(b); }
    // ------------- 押してる間 --------------
    public bool GetXButton() { return Input.GetKey(x); }
    public bool GetYButton() { return Input.GetKey(y); }
    public bool GetAButton() { return Input.GetKey(a); }
    public bool GetBButton() { return Input.GetKey(b); }


    // ------------- 離したら ----------------
    public bool GetXButtonUp() { return Input.GetKeyUp(x); }
    public bool GetYButtonUp() { return Input.GetKeyUp(y); }
    public bool GetAButtonUp() { return Input.GetKeyUp(a); }
    public bool GetBButtonUp() { return Input.GetKeyUp(b); }



}
