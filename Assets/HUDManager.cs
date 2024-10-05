using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    private SheepController[] sheepControllers;
    private int initialSheepCount = 0;
    private SheepIcon[] sheepIcons;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sheepControllers = FindObjectsByType<SheepController>(FindObjectsSortMode.None);
        initialSheepCount = sheepControllers.Length;
    }

    // Update is called once per frame
    void Update() { }
}
