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
            },
            {
                ConditionID.brn,
                // Burn take 1/16 of the MaxHP each turn
                new Condition()
                {
                    Name = "Burn",
                    StartMsg = "has been burned",
                    OnAfterTurn = (Poqimon poq) =>
                    {
                        poq.UpdateHP(poq.MaxHp / 16);
                        poq.StatusChanges.Enqueue($"{poq.PoqimonBase.PoqimonName} hurt itself due to burn");
                    }
                }
            },
            {
                ConditionID.par,
                // Paralyzed make 1/5 times the poqimon won't perform the move
                new Condition()
                {
                    Name = "Paralyzed",
                    StartMsg = "has been paralyzed",
                    OnBeforeMove = (Poqimon poq) =>
                    {
                        // won't perform the move
                        if (UnityEngine.Random.Range(1, 5) == 1)
                        {
                            poq.StatusChanges.Enqueue($"{poq.PoqimonBase.PoqimonName}'s paralyzed, can't move");
                            return false;
                        }
                        // will perform the move
                        return true;
                    }
                }
            },
            {
                ConditionID.frz,
                // Freeze during 1/5 turns the poqimon won't perform the move
                new Condition()
                {
                    Name = "Freeze",
                    StartMsg = "has been frozen",
                    OnBeforeMove = (Poqimon poq) =>
                    {
                        // will perform the move after 1/5 turns
                        if (UnityEngine.Random.Range(1, 5) == 1)
                        {
                            poq.CureStatus();
                            poq.StatusChanges.Enqueue($"{poq.PoqimonBase.PoqimonName}'s is not frozen anymore");
                            return true;
                        }
                        // won't perform the move while it's frozen
                        return false;
                    }
                }
            },
            {
                ConditionID.slp,
                // Sleep: during 1/3 turns the poqimon won't perform the move
                new Condition()
                {
                    Name = "Sleep",
                    StartMsg = "has fallen asleep",
                    OnStart = (Poqimon poq) =>
                    {
                        // will perform the move after 1/3 turns
                        poq.StatusTime = UnityEngine.Random.Range(1, 4);
                        
                    },
                    OnBeforeMove = (Poqimon poq) =>
                    {
                        // the poqimon wakes up
                        if (poq.StatusTime <= 0)
                        {
                            poq.CureStatus();
                            poq.StatusChanges.Enqueue($"{poq.PoqimonBase.PoqimonName} woke up");
                            return true;
                        }
                        // the poqimon is sleeping
                        --poq.StatusTime;
                        poq.StatusChanges.Enqueue($"{poq.PoqimonBase.PoqimonName} is sleeping");
                        return false;
                    }
                }
            }
        };
}

public enum ConditionID
{
    none, psn, brn, slp, par, frz
}