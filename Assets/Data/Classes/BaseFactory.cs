using UnityEngine;

public interface IFactory<T> where T : ScriptableObject
{
    GameObject CreateObject(T data, Vector3 position, Quaternion rotation, Transform parent = null);
}
