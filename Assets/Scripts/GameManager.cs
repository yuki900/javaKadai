using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    //private Fade _fade;
    //[SerializeField] private Result_UI _result;
    //[SerializeField] private Title_UI _title;
    public enum GameLayer
    {
        Wall = 9,
        Bullet = 10,
        Player = 11,
        Box = 12
    };
    public enum GameState
    {
        Title,
        InGame,
        Reslut
    };

   
    public GameState _gameState { get; set; }
    public int _winnerNum { get; private set; }

    private void Start()
    {
        //_fade = GameObject.FindGameObjectWithTag("Fade").GetComponent<Fade>();
        // �e�X�g
        _gameState = GameState.Title;
    }

    public void Update()
    {
        // �A�v���P�[�V�����̏I��
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // �G�f�B�^�Ȃ�
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
            // ���s���Ȃ�
#elif UNITY_STANDALONE
            Application.Quit();
#endif
        }

       // Debug.Log("" + _fade._fadeoutFlag);

        // �Q�[���̃X�e�[�g
        switch (_gameState)
        {
            case GameState.Title:
                
                break;
            case GameState.InGame:
                          
                break;
            case GameState.Reslut:                               
               
                for (int i = 0; i < FieldManager.Instance.tanks.Length; i++)
                {
                    // �����t���O�������Ă�����X�L�b�v
                    // �� �O�l�ΐ�ȍ~�͏����H�v����
                    if (FieldManager.Instance.tanks[i]._isGameOver)
                        continue;

                    // ���҂̔ԍ����i�[
                    _winnerNum = i;
                    break;
                }

                // �t�F�C�h�A�E�g
               // _fade._fadeoutFlag = true;

                //if (_fade._fadeEnd)
                //{
                                
                //    //tanks[winnerNum].GetIcon().SetActive(true);
                //}


                break;
        }
    }

}