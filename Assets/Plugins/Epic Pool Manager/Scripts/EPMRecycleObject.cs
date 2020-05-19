using UnityEngine;

public class EPMReycleObject : MonoBehaviour
{
    public event System.Action<EPMReycleObject> Recycled;
    public int ID { get; private set; }    

    public void Recycle()
    {
        Recycled?.Invoke(this);
    }

    public void Init(int id, Transform parent)
    {
        ID = id;
        transform.SetParent(parent);
        gameObject.SetActive(false);
    }
}
