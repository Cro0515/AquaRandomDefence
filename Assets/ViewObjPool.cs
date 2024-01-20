// Decompiled with JetBrains decompiler
// Type: ObjectPoolClass.ObjectPool
// Assembly: ObjectPoolClass, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C65A500B-580C-4A0D-A178-98CA8CF8C126
// Assembly location: C:\Users\Cro\Desktop\ObjectPoolClass.dll


/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public Queue<GameObject> ObjPool_Queue;
    public GameObject Resource_Obj;
    public Transform Pool_Tr;
    public Transform Active_Tr;


    public void Create_ObjPool(GameObject _obj, int _size, Transform _poolTr, Transform _activeTr = null)
    {
        this.ObjPool_Queue = new Queue<GameObject>();
        this.Pool_Tr = _poolTr;
        
        
        if (_activeTr != null)
            this.Active_Tr = _activeTr;
        
        this.Resource_Obj = _obj;

        for (int i = 0; i < _size; i++)
            this.PushObject(Object.Instantiate<GameObject>(_obj));
    }



    public GameObject PopObject()
    {
        GameObject gameObject;
        if (this.ObjPool_Queue.Count > 0)
        {
            gameObject = this.ObjPool_Queue.Dequeue();
            gameObject.SetActive(true);
        }
        else
            gameObject = Object.Instantiate<GameObject>(this.Resource_Obj);
        gameObject.transform.parent = this.Active_Tr;
        return gameObject;
    }

    public void PushObject(GameObject _obj)
    {
        this.ObjPool_Queue.Enqueue(_obj);
        _obj.transform.parent = this.Pool_Tr;
        _obj.transform.localPosition = Vector3.zero;
        _obj.SetActive(false);
    }


    public void ClearPool()
    {
        this.ObjPool_Queue = (Queue<GameObject>)null;
        foreach (Component component in this.Pool_Tr)
            Object.Destroy((Object)component.gameObject);
    }


}


*/