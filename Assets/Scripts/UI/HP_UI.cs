using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 各タンクから呼び出すクラス
[System.Serializable]
public class HP_UI
{

    [SerializeField] private Sprite _fillHP;   // 通常HP
    [SerializeField] private Sprite _damageHP; // ダメージHP
    [SerializeField] private Image[] _image;   // キャンバス上のイメージオブジェ

    // コンストラクタ
    public HP_UI(Image[] image, Sprite fill,Sprite dama)
    {
        _image = image;
        _fillHP = fill;
        _damageHP = dama;
    }

    /// <summary>
    /// 初期化(塗りつぶし)
    /// </summary>
    public void Initialize()
    {
        for(int i = 0; i < _image.Length;++i)
        {
            _image[i].sprite = _fillHP;
        }
    }

    /// <summary>
    /// ダメージ処理
    /// </summary>
    /// <param name="value"> 減らす数 </param>
    public void Damage(int value)
    {
        for(int i = 0; i < _image.Length;++i)
        {
            // 既にダメージスプライトならスキップ
            if (_image[i].sprite == _damageHP)
                continue;

            _image[i].sprite = _damageHP;
            break;
        }
    }
}
