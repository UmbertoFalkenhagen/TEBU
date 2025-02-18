using UnityEngine;

public interface IFactory<T> where T : ScriptableObject
{
    GameObject CreateObject(T data, GameObject positionObject, Quaternion rotation, GameObject parentObject = null);
}