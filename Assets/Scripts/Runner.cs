using System;
using System.Collections.Generic;
using UnityEngine;

public class Runner: MonoBehaviour
{
    /// <summary>
    /// Control point where runner is standing
    /// </summary>
    public ControlPoint HomeControlPoint;

    /// <summary>
    /// Speed of a runner
    /// </summary>
    public float Speed = 1;

    private Queue<ControlPoint> _targetsQueue = new();
    
    /// <summary>
    /// Target to run to
    /// </summary>
    private ControlPoint _currentTarget;

    /// <summary>
    /// Position of self
    /// </summary>
    private Vector3 SelfPosition => transform.position;

    /// <summary>
    /// Object that runner is carrying
    /// </summary>
    private GameObject _carriedObject;

    /// <summary>
    /// Runner at the furthest point who will take the item
    /// </summary>
    private Runner _nextRunner;

    /// <summary>
    /// Timeout after item was delivered to a final destination
    /// </summary>
    private float _timeout = 2f;

    /// <summary>
    /// Time to wait before movement will start again
    /// </summary>
    private float _waitUntil = 0f;

    /// <summary>
    /// Link to a game services
    /// </summary>
    [Header("Links")] public Services Services;

    /// <summary>
    /// Arm to carry items
    /// </summary>
    [Header("Local links")] 
    [SerializeField] private GameObject RightArm;

    /// <summary>
    /// Link to a hand to hold items
    /// </summary>
    [SerializeField] private GameObject RightHand;

    /// <summary>
    /// Link to a body renderer to set another color for it
    /// </summary>
    [SerializeField] private MeshRenderer CorpseRenderer;
    
    void Start()
    {
        var homePosition = HomeControlPoint.transform.position;
        transform.position = new Vector3(homePosition.x, 0f, homePosition.z);
    }
    
    void Update()
    {
        if (_carriedObject || Vector3.Distance(SelfPosition, HomeControlPoint.transform.position) > Single.Epsilon)
        {
            if (_carriedObject && _waitUntil > Time.time) return;
            CheckTargets();
            transform.position = Vector3.MoveTowards(SelfPosition, _currentTarget.transform.position, Time.deltaTime * Speed);
        }
    }

    /// <summary>
    /// Set runner's body color
    /// </summary>
    public void SetColor(Color color)
    {
        CorpseRenderer.material.color = color;
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
    /// Give item to another runner
    /// </summary>
    /// <param name="anotherRunner">Runner who will take the item</param>
    public void Give(Runner anotherRunner)
    {
        anotherRunner.Take(_carriedObject);
        _carriedObject = null;
        
        RightArm.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    /// <summary>
    /// Take item in runners hand
    /// </summary>
    /// <param name="item">Item to hold</param>
    public void Take(GameObject item)
    {
        RightArm.transform.localRotation = Quaternion.Euler(0, 0, -90);
        
        _carriedObject = item;
        _carriedObject.transform.SetParent(RightHand.transform);
        _carriedObject.transform.localPosition = Vector3.zero;
        _carriedObject.transform.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// Method to fill the queue and define the person, who will receive item
    /// </summary>
    void GenerateQueue()
    {
        var direction = Services.GameController.MovementDirection;
        var controlPoints = Services.GameController.ControlPoints;
        var homeIndex = controlPoints.FindIndex(t => t == HomeControlPoint);
        if (homeIndex == 0 && direction < 0 || homeIndex == controlPoints.Count - 1 && direction > 0)
        {
            direction *= -1;
            Services.GameController.MovementDirection = direction;
            _waitUntil = Time.time + _timeout;
        }
        var cursor = homeIndex + direction;

        while (!Services.GameController.ControlPoints[cursor].Owner)
        {
            _targetsQueue.Enqueue(Services.GameController.ControlPoints[cursor]);
            Debug.Log(cursor);
            cursor += direction;
        }
        
        _nextRunner = Services.GameController.ControlPoints[cursor].Owner;

        while (Services.GameController.ControlPoints[cursor] != HomeControlPoint)
        {
            _targetsQueue.Enqueue(Services.GameController.ControlPoints[cursor]);
            Debug.Log(cursor);
            cursor -= direction;
        }
        
        _targetsQueue.Enqueue(HomeControlPoint);
        Debug.Log(cursor);
    }

    /// <summary>
    /// Check target changing conditions
    /// </summary>
    void CheckTargets()
    {
        if (_targetsQueue.Count == 0)
        {
            if (_carriedObject)
            {
                GenerateQueue();
                NextTarget();
            }
        }
        else
        {
            if (_nextRunner && Vector3.Distance(SelfPosition, _nextRunner.transform.position) < 0.5f)
            {
                Give(_nextRunner);
                _nextRunner = null;
                NextTarget();
            }
            else if (Vector3.Distance(SelfPosition, _currentTarget.transform.position) < Single.Epsilon)
            {
                NextTarget();
            }
        }
    }

    /// <summary>
    /// Change target to the next in queue
    /// </summary>
    void NextTarget()
    {
        if (_targetsQueue.Count == 0) return;
        _currentTarget = _targetsQueue.Dequeue();
        LookAt(_currentTarget.transform.position);
    }
}