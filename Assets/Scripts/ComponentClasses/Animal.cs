using UnityEngine;

public class Animal : MonoBehaviour
{
    public ObjectIdentifier animalID;
    public AnimalType animalType;

    public void SayHello()
    {
        if (animalID == null)
        {
            Debug.LogWarning("Animal: Cannot say hello - animalID is null");
            return;
        }

        AnimalManager.Instance.PrintAnimalInfo(animalID);
    }
}
