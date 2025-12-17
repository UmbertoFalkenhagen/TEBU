using UnityEngine;

public class Animal : MonoBehaviour
{
    public ObjectIdentifier animalID;
    public AnimalType animalType;

    public ObjectIdentifier assignedBuilding;
    public bool isEmployed => assignedBuilding != null;

    public void SayHello()
    {
        if (animalID == null)
        {
            Debug.LogWarning("Animal: Cannot say hello - animalID is null");
            return;
        }

        DBAnimalValue animalData = DatabaseManager.Instance.AnimalDictionary.TryGetValue(animalID, out var data)
            ? data
            : null;

        if (animalData == null)
        {
            Debug.LogWarning($"Animal: No database entry found for {animalID}");
            return;
        }

        string parentCityName = "Unknown City";
        if (DatabaseManager.Instance.CityCenterDictionary.TryGetValue(animalData._parentCityCenter, out var cityCenter))
        {
            parentCityName = cityCenter._cityName;
        }

        string jobStatus = "Unemployed";
        if (animalData._parentBuilding != null)
        {
            DBBuildingValue buildingData = DatabaseManager.Instance.GetBuildingValue(animalData._parentBuilding);
            if (buildingData != null)
            {
                jobStatus = $"Working at {buildingData._type}";
            }
        }

        Debug.Log($"?? Hello! I'm {animalData._animalName}, a {animalData._type} (ID: {animalID})\n" +
                  $"   ?? Living in: {parentCityName}\n" +
                  $"   ?? Job: {jobStatus}\n" +
                  $"   ? Priority: {animalData.priority}");
    }
}
