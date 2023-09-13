using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : SingletonMonoBehaviour<FieldManager>
{
    public Tank[] tanks { get; private set; }
    public Box[] boxes { get; private set; }
    public int insetanceTank { get; private set; }
    public int instanceBox { get; private set; }


    protected override void Awake()
    {
       
    }

    void Start()
    {
        Init();
    }

    private void Init()
    {
        insetanceTank = 2;
        instanceBox = 48;
        tanks = new Tank[insetanceTank];
        var charaObj = GameObject.FindGameObjectsWithTag("Player");
        var obj = GameObject.FindGameObjectsWithTag("Box");
        for (int i = 0; i < tanks.Length; i++)
        {
            tanks[i] = charaObj[i].GetComponent<Tank>();
        }
        boxes = new Box[instanceBox];
        for (int i = 0; i < boxes.Length; ++i)
        {
            Box box = new Box(obj[i]);
            box.IsActive(true);
            boxes[i] = box;
        }
    }

    private void OnDestroy()
    {
        Destroy(this);
    }

}
