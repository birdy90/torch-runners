using System.Collections.Generic;
using UnityEngine;

public class RelayRunner: MonoBehaviour
{
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

    /// <summary>
    /// Object that runner is carrying
    /// </summary>
    private GameObject _carriedObject;
    
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
        CorpseRenderer.material.color = Colors[Random.Range(0, Colors.Count)];
    }
    
    /// <summary>
    /// Give item to another runner
    /// </summary>
    /// <param name="anotherRunner">Runner who will take the item</param>
    public void Give(RelayRunner anotherRunner)
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
}