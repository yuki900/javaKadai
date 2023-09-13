using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box
{
    public GameObject _obj { get;private set; }
    public Box(in GameObject obj)
    {
        _obj = obj;
    }

    public void IsActive(in bool isActive)
    {
        _obj.SetActive(isActive);
    }

   
}
