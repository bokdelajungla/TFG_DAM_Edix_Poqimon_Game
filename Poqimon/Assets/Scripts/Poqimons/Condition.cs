using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition
{
    public ConditionID Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string StartMsg { get; set; }
    
    public Action<Poqimon> OnStart { get; set; }
    
    // Same as action but can return a value (at this case a boolean)
    public Func<Poqimon, bool> OnBeforeMove { get; set; }

    // Action called after an event
    public Action<Poqimon> OnAfterTurn { get; set; }
}

