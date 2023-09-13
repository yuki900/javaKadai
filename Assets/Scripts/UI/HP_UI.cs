using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// �e�^���N����Ăяo���N���X
[System.Serializable]
public class HP_UI
{

    [SerializeField] private Sprite _fillHP;   // �ʏ�HP
    [SerializeField] private Sprite _damageHP; // �_���[�WHP
    [SerializeField] private Image[] _image;   // �L�����o�X��̃C���[�W�I�u�W�F

    // �R���X�g���N�^
    public HP_UI(Image[] image, Sprite fill,Sprite dama)
    {
        _image = image;
        _fillHP = fill;
        _damageHP = dama;
    }

    /// <summary>
    /// ������(�h��Ԃ�)
    /// </summary>
    public void Initialize()
    {
        for(int i = 0; i < _image.Length;++i)
        {
            _image[i].sprite = _fillHP;
        }
    }

    /// <summary>
    /// �_���[�W����
    /// </summary>
    /// <param name="value"> ���炷�� </param>
    public void Damage(int value)
    {
        for(int i = 0; i < _image.Length;++i)
        {
            // ���Ƀ_���[�W�X�v���C�g�Ȃ�X�L�b�v
            if (_image[i].sprite == _damageHP)
                continue;

            _image[i].sprite = _damageHP;
            break;
        }
    }
}
