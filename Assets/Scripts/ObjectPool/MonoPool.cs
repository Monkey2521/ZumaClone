using System.Collections.Generic;
using UnityEngine;

public sealed class MonoPool<TObject> : ObjectPool<TObject> where TObject : MonoBehaviour
{
    private TObject _prefab;
    private Transform _poolParent;

    public MonoPool(List<TObject> objects) : base(objects) { }

    public MonoPool(TObject[] objects) : base(objects) { }

    public MonoPool(TObject obj, int capacity, Transform poolParent)
    {
        _prefab = obj;
        _poolParent = poolParent;

        _objects = new List<TObject>(capacity);

        for (int i = 0; i < capacity;  i++)
        {
            CreateObject();
        }
    }

    protected override void CreateObject()
    {
        if (_objects == null)
        {
            _objects = new List<TObject>();
        }

        var obj = Object.Instantiate(_prefab, _poolParent);
        obj.gameObject.SetActive(false);

        _objects.Add(obj);
    }

    public override TObject PullObject()
    {
        TObject obj =  base.PullObject();

        obj.gameObject.SetActive(true);

        return obj;
    }

    public override List<TObject> PullObjects(int count)
    {
        List<TObject> objects =  base.PullObjects(count);

        foreach(var obj in objects)
        {
            obj.gameObject.SetActive(true);
        }

        return objects;
    }

    public override void ReleaseObject(TObject obj)
    {
        obj.gameObject.SetActive(false);

        if (obj.transform.parent != _poolParent)
        {
            obj.transform.parent = _poolParent;
        }

        obj.tag = "Untagged";

        base.ReleaseObject(obj);
    }

    public override void ReleaseObjects(List<TObject> objects)
    {
        foreach (var obj in objects)
        {
            ReleaseObject(obj);
        }
    }

    public override void ReleaseObjects(TObject[] objects)
    {
        foreach (var obj in objects)
        {
            obj.gameObject.SetActive(false);
        }

        base.ReleaseObjects(objects);
    }

    public override void ClearPool()
    {
        foreach(var obj in _objects)
        {
            Object.Destroy(obj.gameObject);
        }

        if (_poolParent != null)
            Object.Destroy(_poolParent.gameObject);

        base.ClearPool();
    }
}
