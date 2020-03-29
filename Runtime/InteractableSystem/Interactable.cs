using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
    public static bool logOnInteract = true;
    public static bool logOnAttack = true;

    [Header("Interactable")]


    private Collider2D trigger;


    #region Callback Methodes
    /*
     *
     *  Callback Methodes
     * 
     */

    void Awake()
    {
        trigger = GetComponent<Collider2D>();
    }

    void Start()
    {
        
    }

    protected virtual void OnTriggerEnter2D(Collider2D _other)
    {
        // Call OnInteract
        OnInteract(_other.transform);
    }

    void Update()
    {
        
    }

    #endregion

    #region Gameplay Methodes
    /*
     *
     * 
     *  Gameplay Methodes
     *
     *  
     */

    /// <summary>
    /// OnAttack Callback for Interactables
    /// </summary>
    /// <param name="_interactor"></param>
    /// <returns>True if interaction was successful</returns>
    public virtual bool OnAttack(Transform _interactor)
    {
        if (logOnAttack)
            Debug.Log(_interactor.gameObject.name + " attacked \"" + this.GetType().Name + "\" [" + gameObject.name + "] at position: " + transform.position);

        return true;
    }

    /// <summary>
    /// OnInteract Callback for Interactables
    /// </summary>
    /// <param name="_interactor"></param>
    /// <returns>True if interaction was successful</returns>
    public virtual bool OnInteract(Transform _interactor)
    {
        if (logOnInteract)
            Debug.Log(_interactor.gameObject.name + " interacted with \"" + this.GetType().Name + "\" [" + gameObject.name + "] at position: " + transform.position);

        return true;
    }

    #endregion

    #region Helper Methodes
    /*
     *
     *  Helper Methodes
     * 
     */

    #endregion
}
