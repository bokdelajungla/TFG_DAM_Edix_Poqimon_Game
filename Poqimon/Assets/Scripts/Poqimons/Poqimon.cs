using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[Serializable]
public class Poqimon 
{
    [SerializeField] PoqimonBaseObject poqimonBase;
    [SerializeField] int poqimonLevel;
    
    public PoqimonBaseObject PoqimonBase
    {
        get
        {
            return poqimonBase;
        }
    }
    public int PoqimonLevel
    {
        get
        {
            return poqimonLevel;
        }
    }
    
    public List<Move> moves;
    public List<Move> Moves => moves;
    
    private int currentHp;
    public int CurrentHp 
    {
        get => currentHp;
        set => currentHp = value;
    }
    
    //Stats Formulas (from Bulbapedia)
    public int Attack => Mathf.FloorToInt((poqimonBase.Attack*poqimonLevel) / 100f) + 5;
    public int Defense => Mathf.FloorToInt((poqimonBase.Defense*poqimonLevel) / 100f) + 5;
    public int SpAttack => Mathf.FloorToInt((poqimonBase.SpAttack*poqimonLevel) / 100f) + 5;
    public int SpDefense => Mathf.FloorToInt((poqimonBase.SpDefense*poqimonLevel) / 100f) + 5;
    public int Speed => Mathf.FloorToInt((poqimonBase.Speed*poqimonLevel) / 100f) + 5;
    
    public int MaxHP => Mathf.FloorToInt((poqimonBase.MaxHP*poqimonLevel) / 100f) + 10;

    
    //Initializer
    public void Init(){
        CurrentHp = MaxHP;

        this.moves = new List<Move>();
        foreach (var learnableMove in poqimonBase.LearnableMoves)
        {
            if(learnableMove.LearnLevel <= poqimonLevel)
            {
                moves.Add(new Move(learnableMove.MoveBase));
            }
            if(moves.Count >= 4)
            {
                //TODO: Learning logic (only 4 slots)
                break;
            }
        }
    }


    


    ///<summary>
    ///Take Damage function (formula from Bulbapedia)
    ///</summary>
    ///<param name="move"> The attacking Poqimon move </param>
    ///<param name="attacker"> The attacking Poqimon object </param>
    ///<returns> true if Poqimon fainted because of the attack, false otherwise </returns> 
    public DamageDetails TakeDamage(Move move, Poqimon attacker)
    {
        //Critical Hit
        float critical = 1f;
        if (UnityEngine.Random.Range(0f, 1f) * 100f <= 6.25f)
        {
            critical = 2f;
        }
        //Move Effectiveness
        /* TODO CORREGIR
        float type = TypeChart.GetEffectiveness(move.MoveBase.MoveType, this.PoqimonBase.PoqimonType1) *
                     TypeChart.GetEffectiveness(move.MoveBase.MoveType, this.PoqimonBase.PoqimonType2);
        */
        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = 0f, // TODO CORREGIR = type,
            Critical = critical,
            Fainted = false
        };
        
        //Damage Value
        float d = 0;
        //Random Modifier
        float modifiers = UnityEngine.Random.Range(0.85f, 1f); // TODO CORREGIR * type * critical;
        //Level dependency
        float a = (2 * attacker.PoqimonLevel + 10)/250f;
        //Damage Calculation
        //Physical
        if(move.MoveBase.MoveCategory == CategoryType.Physical)
        {
            d = a * move.MoveBase.MovePower * ((float)attacker.Attack/ this.Defense) + 2;
        }
        //Special
        else if(move.MoveBase.MoveCategory == CategoryType.Special)
        {
            d = a * move.MoveBase.MovePower * ((float)attacker.SpAttack/this.SpDefense) + 2;
        }
        //Status
        else{
            //TODO: Status attack
        }
       
        int damage = Mathf.FloorToInt(d + modifiers);

        CurrentHp -= damage;
        if (CurrentHp <= 0)
        {
            CurrentHp = 0;
            damageDetails.Fainted = true;
        }
        return damageDetails;
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}
