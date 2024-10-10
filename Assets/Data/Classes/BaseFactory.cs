using UnityEngine;

public interface IFactory<T> where T : ScriptableObject
{
    GameObject CreateElement(T data, Vector3 position, Quaternion rotation, Transform parent = null);
    //GameObject GetInitialObjectForTile(ScriptableTile tileData);
}
