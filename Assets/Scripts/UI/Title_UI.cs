using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title_UI : MonoBehaviour
{
    [SerializeField] private GameObject _start;
    [SerializeField] private Fade _fade;
    private Button startButton;
    
    private void Awake()
    {
        startButton = _start.GetComponent<Button>();
        startButton.onClick.AddListener(() => _fade._fadeoutFlag = true);
        Time.timeScale = 1;
    }

    public void Update()
    {
        if (GameManager.Instance._gameState == GameManager.GameState.Title)
        {
            if (_fade._fadeEnd)
            {
                _fade.ResetFade();
                GameManager.Instance._gameState = GameManager.GameState.InGame;
                SceneManager.LoadScene("InGame");
            }
        }
    }

}
