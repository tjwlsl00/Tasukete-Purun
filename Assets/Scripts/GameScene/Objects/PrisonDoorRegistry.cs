using UnityEngine;

public class PrisonDoorRegistry : MonoBehaviour
{
    [SerializeField] private GameObject[] sortedPrisonDoors;

    void Start()
    {
        if (PrisonDoorManager.Instance != null)
        {
            PrisonDoorManager.Instance.RegisterDoors(sortedPrisonDoors);
        }
    }
}