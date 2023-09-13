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
        // テスト
        _gameState = GameState.Title;
    }

    public void Update()
    {
        // アプリケーションの終了
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // エディタなら
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
            // 実行中なら
#elif UNITY_STANDALONE
            Application.Quit();
#endif
        }

       // Debug.Log("" + _fade._fadeoutFlag);

        // ゲームのステート
        switch (_gameState)
        {
            case GameState.Title:
                
                break;
            case GameState.InGame:
                          
                break;
            case GameState.Reslut:                               
               
                for (int i = 0; i < FieldManager.Instance.tanks.Length; i++)
                {
                    // 負けフラグが立っていたらスキップ
                    // ※ 三人対戦以降は少し工夫する
                    if (FieldManager.Instance.tanks[i]._isGameOver)
                        continue;

                    // 勝者の番号を格納
                    _winnerNum = i;
                    break;
                }

                // フェイドアウト
               // _fade._fadeoutFlag = true;

                //if (_fade._fadeEnd)
                //{
                                
                //    //tanks[winnerNum].GetIcon().SetActive(true);
                //}


                break;
        }
    }

}