using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ɃR���g���[���[����p�N���X
/// </summary>
public class InputContoller : MonoBehaviour
{
    [SerializeField,Header("������")] private string leftArrow;
    [SerializeField,Header("�E����")] private string rightArrow;
    [SerializeField,Header("�O�i")] private string upArrow;
    [SerializeField,Header("���")] private string downArrow;
    [SerializeField,Header("����]")] private string x;
    [SerializeField,Header("����")] private string y;
    [SerializeField,Header("����")] private string a;
    [SerializeField, Header("�E��]")] private string b;

    public enum Operation { Keybord,Controller }
    [SerializeField] private Operation _operation;

    public Operation GetOperation() { return _operation; }



    void Update()
    {
        //if (GetLeftButton())
        //{
        //    Debug.Log("��[��llll");
        //}

        //if (GetDownButton())
        //{
        //    Debug.Log("��[��dddd");
        //}
        //if (GetUpButton())
        //{
        //    Debug.Log("��[��uuuuu");
        //}
        //if (GetRightButton())
        //{
        //    Debug.Log("��[��rrrrr");
        //}
    }

    // ----------- ���݋(�����Raw) ---------
    public bool GetLeftButton(){ return Input.GetAxisRaw(leftArrow) > 0; }
    public bool GetRightButton() {return Input.GetAxisRaw(rightArrow) > 0; }
    public bool GetUpButton() {return Input.GetAxisRaw (upArrow) > 0; }
    public bool GetDownButton() {return Input.GetAxisRaw (downArrow) > 0; }


    // ------------- �����ꂽ�u�� -------------
    public bool GetXButtonDown() { return Input.GetKeyDown(x); }
    public bool GetYButtonDown() { return Input.GetKeyDown(y); }
    public bool GetAButtonDown() { return Input.GetKeyDown(a); }
    public bool GetBButtonDown() { return Input.GetKeyDown(b); }
    // ------------- �����Ă�� --------------
    public bool GetXButton() { return Input.GetKey(x); }
    public bool GetYButton() { return Input.GetKey(y); }
    public bool GetAButton() { return Input.GetKey(a); }
    public bool GetBButton() { return Input.GetKey(b); }


    // ------------- �������� ----------------
    public bool GetXButtonUp() { return Input.GetKeyUp(x); }
    public bool GetYButtonUp() { return Input.GetKeyUp(y); }
    public bool GetAButtonUp() { return Input.GetKeyUp(a); }
    public bool GetBButtonUp() { return Input.GetKeyUp(b); }



}
