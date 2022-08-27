using System.Collections.Generic;

public abstract class ObjectPool<TObject> where TObject : IPoolable
{
    protected List<TObject> _objects;
    
    public List<TObject> Objects => _objects;
    public bool Empty => _objects.Count > 0;
    
    public ObjectPool()
    {
        _objects = new List<TObject>();
    }

    public ObjectPool(List<TObject> objects)
    {
        _objects = objects;
    }

    public ObjectPool(TObject[] objects)
    {
        _objects = new List<TObject>(objects);
    }

    protected abstract void CreateObject();

    public virtual TObject PullObject()
    {
        if (Empty)
        {
            CreateObject();
        }

        TObject obj = _objects[0];
        _objects.RemoveAt(0);

        return obj;
    }

    public virtual List<TObject> PullObjects(int count)
    {
        if (count < 0) return null;

        List<TObject> objects = new List<TObject>(count);

        for (int i = 0; i < count; i++)
            objects.Add(PullObject());

        return objects;
    }

    public virtual void ReleaseObject(TObject obj)
    {
        obj.ResetObject();
        _objects.Add(obj);
    }

    public virtual void ReleaseObjects(List<TObject> objects)
    {
        foreach(TObject obj in objects)
        {
            ReleaseObject(obj);
        }
    }
    public virtual void ReleaseObjects(TObject[] objects)
    {
        foreach (TObject obj in objects)
        {
            ReleaseObject(obj);
        }
    }

    public virtual void ClearPool()
    {
        _objects.Clear();
    }
}
