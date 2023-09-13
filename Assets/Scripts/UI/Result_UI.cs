using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Result_UI : MonoBehaviour
{

    [SerializeField] private GameObject _text;
    //[SerializeField] private GameObject retry;
    [SerializeField] private GameObject _title;
    [SerializeField] private Fade _fade;

    private Button retryButton;
    private Button titleButton;

    private void Awake()
    {
        //retryButton = retry.GetComponent<Button>();
        titleButton = _title.GetComponent<Button>();
    }

    private void Start()
    {
        _text.SetActive(false);
       // retry.SetActive(false);
        _title.SetActive(false);

        titleButton.onClick.AddListener(() => GameManager.Instance._gameState = GameManager.GameState.Title);
        titleButton.onClick.AddListener(() => _fade.ResetFade());
        titleButton.onClick.AddListener(() => SceneManager.LoadScene("Title"));
        // retryButton.onClick.AddListener(()=>SceneManager.LoadScene("InGame"));
    }

    public void Update()
    {
        if (GameManager.Instance._gameState == GameManager.GameState.Reslut)
        {
            _fade._fadeoutFlag = true;

            if (_fade._fadeEnd)
            {
                _text.SetActive(true);
                //retry.SetActive(true);
                _title.SetActive(true);
                FieldManager.Instance.tanks[GameManager.Instance._winnerNum].GetIcon().SetActive(true);
                // ÉQÅ[ÉÄì‡éûä‘Çé~ÇﬂÇÈ
                Time.timeScale = 0;
            }
        }
    }

 

}