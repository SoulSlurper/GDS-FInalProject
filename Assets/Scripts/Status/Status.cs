using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum StatusType { None, Player, Object, Enemy, Boss }

public class Status : MonoBehaviour
{
    [SerializeField] private StatusAmount _health;
    
    public StatusAmount health
    { 
        get { return _health; }
        private set { }
    }

    public bool noHealth
    { 
        get { return health.amount <= 0f; }
        private set { }
    }

    public virtual StatusType GetStatusType() { return StatusType.None; }
}
