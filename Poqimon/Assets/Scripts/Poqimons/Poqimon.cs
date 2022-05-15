using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    
    public int CurrentHp {get; set;}
    public int Exp {get; set;}

    public List<Move> Moves {get; set;}
    
    public Dictionary<Stat, int> Stats {get; private set;}

    int GetStat (Stat stat)
    {
        int statValue = Stats[stat];

        //TODO: Introduce Stat Boosting Logic

        return statValue;
    }
    
    public int Attack {
        get {return GetStat(Stat.Attack);}
    }
    public int Defense {
        get {return GetStat(Stat.Defense);}
    }
    public int SpAttack {
        get {return GetStat(Stat.SpAttack);}
    }
    public int SpDefense {
        get {return GetStat(Stat.SpDefense);}
    }
    public int Speed {
        get {return GetStat(Stat.Speed);}
    }
    
    public int MaxHP {get; private set;}

    
    //Initializer
    public void Init(){

        //Generate Moves
        Moves = new List<Move>();
        foreach (var learnableMove in poqimonBase.LearnableMoves)
        {
            if(learnableMove.LearnLevel <= poqimonLevel)
            {
                Moves.Add(new Move(learnableMove.MoveBase));
            }
            if(Moves.Count >= 4)
            {
                //TODO: Learning logic (only 4 slots)
                break;
            }

           
        }

        CalculateStats();
        
        CurrentHp = MaxHP;
    }


    void CalculateStats ()
    {
        //Stats Formulas (from Bulbapedia)
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt(poqimonBase.Attack * poqimonLevel / 100f) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((poqimonBase.Defense * poqimonLevel) / 100f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((poqimonBase.SpAttack * poqimonLevel) / 100f) + 5);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((poqimonBase.SpDefense * poqimonLevel) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((poqimonBase.Speed * poqimonLevel) / 100f) + 5);
    
        MaxHP = Mathf.FloorToInt((poqimonBase.MaxHP * poqimonLevel) / 100f) + 10;
    }

    ///<summary>
    ///Take Damage function (formula from Bulbapedia)
    ///</summary>
    ///<param name="move"> The attacking Poqimon move </param>
    ///<param name="attacker"> The attacking Poqimon object </param>
    ///<returns> true if Poqimon fainted because of the attack, false otherwise </returns> 
    public bool TakeDamage(Move move, Poqimon attacker)
    {
        //Damage Value
        float d = 0;
        //Random Modifier
        float modifiers = UnityEngine.Random.Range(0.85f, 1f);
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
            return true;
        }
        return false;
    }

    public bool CheckForLevelUp()
    {
        if (Exp > PoqimonBase.GetExperienceForLvl(poqimonLevel + 1))
        {
            ++poqimonLevel;
            CalculateStats();
            return true;
        }
        return false;
    }


    public Evolution CheckForEvolution()
    {
        return PoqimonBase.Evolutions.FirstOrDefault(e => e.LevelRequired <= poqimonLevel);
    }

    //Evolution Function
    public void Evolve (Evolution evolution)
    {   
        //Set Poqimon Base to the Evolution one
        poqimonBase = evolution.EvolvesInto;
        //Recalculate Stat for new poqimon Base
        CalculateStats();
        //Set currentHP to new MaxHp (Heal up poqimon)
        CurrentHp = MaxHP;
    }
}
