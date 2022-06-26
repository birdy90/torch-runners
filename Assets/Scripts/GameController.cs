using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    /// <summary>
    /// List of all available control points
    /// </summary>
    public List<ControlPoint> ControlPoints;

    /// <summary>
    /// Indices of control points that should be populated with runners
    /// </summary>
    public List<int> IndicesOfOccupiedControlPoints; 
        
    /// <summary>
    /// List of all runners
    /// </summary>
    List<Runner> Runners { get; set; }

    /// <summary>
    /// Runners prefab to instantiate all runners
    /// </summary>
    public GameObject RunnerPrefab;

    /// <summary>
    /// Prefab of a torch
    /// </summary>
    public GameObject TorchPrfab;

    /// <summary>
    /// Movement direction. 1 for forward and -1 for backward
    /// </summary>
    public int MovementDirection = 1;
    
    /// <summary>
    /// List of available colors for runners bodies
    /// </summary>
    private List<Color> Colors = new List<Color>
    {
        new Color(85/255f, 147/255f, 76/255f),
        new Color(49/255f, 145/255f, 241/255f),
        new Color(212/255f, 118/255f, 33/255f),
        new Color(208/255f, 127/255f, 166/255f),
    };

    void Start()
    {
        GenerateRunners();
        Runners[0].Take(Instantiate(TorchPrfab));
    }

    /// <summary>
    /// Method to initialize runner instances
    /// </summary>
    void GenerateRunners()
    {
        var services = GetComponentInParent<Services>();
        
        Runners = new List<Runner>();
        foreach (var index in IndicesOfOccupiedControlPoints)
        {
            var newRunner = Instantiate(RunnerPrefab);
            var runnerComponent = newRunner.GetComponent<Runner>();
            runnerComponent.Services = services;
            runnerComponent.HomeControlPoint = ControlPoints[index];
            runnerComponent.Speed += Random.value / 2;
            
            var selectedColor = Colors[Random.Range(0, Colors.Count)];
            runnerComponent.SetColor(selectedColor);
            Colors.Remove(selectedColor);
            
            ControlPoints[index].Owner = runnerComponent;
            newRunner.transform.position = runnerComponent.HomeControlPoint.transform.position;
            
            while (true) {
                var lookTarget = ControlPoints[Random.Range(0, ControlPoints.Count)];
                if (lookTarget == runnerComponent.HomeControlPoint) continue;
                runnerComponent.LookAt(lookTarget.transform.position);
                break;
            }

            Runners.Add(runnerComponent);
        }
    }
}
