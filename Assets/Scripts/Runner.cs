using System;
using System.Collections.Generic;
using UnityEngine;

public class Runner: MonoBehaviour
{
    /// <summary>
    /// List of points to move to
    /// </summary>
    public List<Vector3> Targets = new List<Vector3>();

    /// <summary>
    /// Speed of the runner
    /// </summary>
    public float Speed;

    /// <summary>
    /// Distance when target counts as reached
    /// </summary>
    private float _checkDistance = 0.3f;

    /// <summary>
    /// Is moving forward
    /// </summary>
    private bool _forward = true;

    /// <summary>
    /// Index of currernt target
    /// </summary>
    private int _currentTargetIndex = 0;

    void Update()
    {
        CheckTargets();
        transform.position = Vector3.MoveTowards(transform.position, Targets[_currentTargetIndex], Time.deltaTime * Speed);
    }

    private void OnDrawGizmos()
    {
        var shiftUp = new Vector3(0, 0.3f, 0);
        var change = _forward ? 1 : -1;
        
        Gizmos.color = _forward ? Color.red : Color.blue;

        Gizmos.DrawLine(transform.position + shiftUp, Targets[_currentTargetIndex] + shiftUp);
        var from = _forward ? _currentTargetIndex + change : 1;
        var to = _forward ? Targets.Count : _currentTargetIndex + 1;
        for (var i = from; i < to; i++)
        {
            Gizmos.DrawLine(Targets[i - 1] + shiftUp, Targets[i] + shiftUp);
        }
        
        foreach (var point in Targets)
        {
            Gizmos.DrawSphere(point + shiftUp, 0.1f);
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
    /// Check target changing conditions
    /// </summary>
    void CheckTargets()
    {
        if (Vector3.Distance(Targets[_currentTargetIndex], transform.position) < _checkDistance)
        {
            if (_forward && _currentTargetIndex == Targets.Count - 1)
            {
                _forward = false;
            } 
            else if (!_forward && _currentTargetIndex == 0)
            {
                _forward = true;
            }
            NextTarget();
        }
    }

    /// <summary>
    /// Change target to the next in queue
    /// </summary>
    void NextTarget()
    {
        _currentTargetIndex += _forward ? 1 : -1;
        transform.LookAt(Targets[_currentTargetIndex]);
    }
}