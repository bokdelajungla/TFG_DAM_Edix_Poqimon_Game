using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    public static void PoissonEffect(Poqimon poq)
    {
        
    }
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } =
        new Dictionary<ConditionID, Condition>()
        {
            {
                ConditionID.psn,
                // Poison take 1/8 of the MaxHP each turn
                // data from https://pokemondb.net/move/poison-powder
                new Condition()
                {
                    Name = "Poison",
                    StartMsg = "has been poisoned",
                    OnAfterTurn = (Poqimon poq) =>
                    {
                        poq.UpdateHP(poq.MaxHp / 8);
                        poq.StatusChanges.Enqueue($"{poq.PoqimonBase.PoqimonName} hurt itself due to poison");
                    }
                }
            }
        };
}

public enum ConditionID
{
    none, psn, brn, slp, par, frz
}