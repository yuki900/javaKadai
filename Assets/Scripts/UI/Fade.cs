using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{

     private Image _image;  // �t�F�[�h�Ŏg���摜

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    private float _fadeoutTime = 1.0f;
    public bool _fadeoutFlag { get; set; }

    public bool _fadeEnd { get; private set; }

    private float defAlpha;

    private void Start()
    {
        defAlpha = _image.color.a;
        ResetFade();
    }

    public void ResetFade()
    {
        _fadeoutFlag = false;
        _fadeEnd = false;

        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, defAlpha); 
    }

    public void Update()
    {
        // �t�F�[�h�t���O�������Ă�����t�F�[�h����
        if (_fadeoutFlag)
        {
            if (_fadeoutTime > 0f)
            {
                // �z���C�g�A�E�g���Ă���
                _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, _image.color.a + Time.deltaTime / _fadeoutTime);
            }
            else
            {
                // fadeoutTime��0���}�C�i�X�Ȃ�A�����Ɋ�������
                _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 1.0f);
            }

            if(_image.color.a >= 0.99f)
            {
                _fadeEnd = true;
            }

        }
    }

    //private IEnumerator FadeOut()
    //{
        
    //    float totalTime = 0;

    //    while (true)
    //    {
    //        //if (_fadeoutTime > 0f)
    //        //{
    //        //    // �z���C�g�A�E�g���Ă���
    //        //    _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, _image.color.a + Time.deltaTime / _fadeoutTime);
    //        //}
    //        //else
    //        //{
    //        //    // fadeoutTime��0���}�C�i�X�Ȃ�A�����Ɋ�������
    //        //    _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 1.0f);
    //        //}

    //        totalTime += Time.deltaTime;

    //        if (totalTime >= _fadeoutTime)
    //        {
    //            fadeEnd = true;
    //            yield break;
    //        }
    //        else
    //        {
    //            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, _image.color.a + Time.deltaTime / _fadeoutTime);
    //        }

    //        yield return null;
    //    }
    //}

    //// �t�F�[�h�A�E�g���I���ɂ���
    //public void Fadeout()
    //{
    //   StartCoroutine(FadeOut());
    //}

}
