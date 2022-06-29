using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Relay: MonoBehaviour
{
    /// <summary>
    /// Speed of a runner
    /// </summary>
    public float Speed = 1;

    /// <summary>
    /// List of control points to pass
    /// </summary>
    private Queue<ControlPoint> _targetsQueue;
    
    /// <summary>
    /// Target to run to
    /// </summary>
    private ControlPoint _currentTarget;

    /// <summary>
    /// Runner at the furthest point who will take the item
    /// </summary>
    private RelayRunner _currentRunner;

    /// <summary>
    /// Runner at the furthest point who will take the item
    /// </summary>
    private RelayRunner _nextRunner;

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
    List<RelayRunner> Runners { get; set; }

    /// <summary>
    /// Runners prefab to instantiate all runners
    /// </summary>
    public GameObject RunnerPrefab;

    /// <summary>
    /// Prefab of a torch
    /// </summary>
    public GameObject TorchPrfab;

    /// <summary>
    /// Distance to pass items
    /// </summary>
    private float _passDistance = 0.3f;

    void Start()
    {
        GenerateRunners();
        
        _currentRunner = Runners[0];
        _currentRunner.Take(Instantiate(TorchPrfab));
        
        GenerateQueue();
        NextTarget();
    }
    
    void Update()
    {
        if (_nextRunner && Vector3.Distance(_currentRunner.transform.position, _nextRunner.transform.position) < _passDistance)
        {
            _currentRunner.Give(_nextRunner);
            _currentRunner = _nextRunner;
            GenerateQueue();
            NextTarget();
        }
        else if (Vector3.Distance(_currentRunner.transform.position, _currentTarget.transform.position) < _passDistance)
        {
            NextTarget();
        }
        
        _currentRunner.transform.position = Vector3.MoveTowards(_currentRunner.transform.position, _currentTarget.transform.position, Time.deltaTime * Speed);
    }

    private void OnDrawGizmos()
    {
        if (!_currentRunner) return;
        
        var shiftUp = new Vector3(0, 0.3f, 0);
        
        Gizmos.color = Color.green;
        var targetsFromQueue = _targetsQueue.ToList();

        Gizmos.DrawLine(_currentRunner.transform.position + shiftUp, _currentTarget.transform.position + shiftUp);
        if (targetsFromQueue.Count > 0)
        {
            Gizmos.DrawLine(_currentTarget.transform.position + shiftUp,
                targetsFromQueue[0].transform.position + shiftUp);
        }

        for (var i = 1; i < targetsFromQueue.Count; i++)
        {
            Gizmos.DrawLine(targetsFromQueue[i - 1].transform.position + shiftUp, targetsFromQueue[i].transform.position + shiftUp);
        }
        
        Gizmos.DrawSphere(_currentTarget.transform.position + shiftUp, 0.1f);
        foreach (var point in targetsFromQueue)
        {
            Gizmos.DrawSphere(point.transform.position + shiftUp, 0.1f);
        }
    }

    /// <summary>
    /// Turn runner towards some point
    /// </summary>
    /// <param name="target">Point to look at</param>
    public void LookAt(Vector3 target)
    {
        transform.LookAt(target);
    }
    
    /// <summary>
    /// Method to fill the queue and define the person, who will receive item
    /// </summary>
    void GenerateQueue()
    {
        _targetsQueue = new Queue<ControlPoint>();
        var controlPoints = ControlPoints;
        var homeIndex = controlPoints.FindIndex(t => t.Owner == _currentRunner);
        var cursor = homeIndex + 1;

        while (cursor < controlPoints.Count - 1 && !ControlPoints[cursor].Owner)
        {
            _targetsQueue.Enqueue(ControlPoints[cursor]);
            cursor += 1;
        }
        
        _nextRunner = ControlPoints[cursor].Owner;
        _targetsQueue.Enqueue(ControlPoints[cursor]);
    }

    /// <summary>
    /// Change target to the next in queue
    /// </summary>
    void NextTarget()
    {
        if (_targetsQueue.Count == 0) return;
        _currentTarget = _targetsQueue.Dequeue();
        _currentRunner.transform.LookAt(_currentTarget.transform);
    }

    /// <summary>
    /// Method to initialize runner instances
    /// </summary>
    void GenerateRunners()
    {
        Runners = new List<RelayRunner>();
        foreach (var index in IndicesOfOccupiedControlPoints)
        {
            var newRunner = Instantiate(RunnerPrefab);
            var runnerComponent = newRunner.GetComponent<RelayRunner>();

            ControlPoints[index].Owner = runnerComponent;
            newRunner.transform.position = ControlPoints[index].transform.position;
            
            while (true) {
                var lookTarget = ControlPoints[Random.Range(0, ControlPoints.Count)];
                if (lookTarget == ControlPoints[index]) continue;
                var lookPosition = lookTarget.transform.position;
                runnerComponent.transform.LookAt(lookPosition);
                break;
            }
        
            Runners.Add(runnerComponent);
        }
    }
}