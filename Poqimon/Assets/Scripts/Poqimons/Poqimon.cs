using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

[Serializable]
public class Poqimon 
{
    // Base characterictis
    [SerializeField] PoqimonBaseObject poqimonBase;
    public PoqimonBaseObject PoqimonBase => poqimonBase;
    
    public int Exp {get; set;}

    // Lvl
    [SerializeField] int poqimonLevel;
    public int PoqimonLevel => poqimonLevel;

    // Movements
    public List<Move> Moves {get; set;}
    
    // Stats & Stat Boosts
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }
    
    // Status Changes
    public Queue<string> StatusChanges { get; private set; } = new Queue<string>();

    // HP
    public int CurrentHp { get; set; }
    
    //Initializer
    public void Init()
    {
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

        CalcStats();
        CurrentHp = MaxHp;
        ResetStatBoost();
    }

    // calculate the stats of the poqimon
    private void CalcStats()
    {
        //Stats Formulas (from Bulbapedia)
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((poqimonBase.Attack*poqimonLevel) / 100f) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((poqimonBase.Defense*poqimonLevel) / 100f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((poqimonBase.SpAttack*poqimonLevel) / 100f) + 5);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((poqimonBase.SpDefense*poqimonLevel) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((poqimonBase.Speed*poqimonLevel) / 100f) + 5);
        
        MaxHp = Mathf.FloorToInt((poqimonBase.MaxHP*poqimonLevel) / 100f) + 10;
    }

    // Reset (to 0) all the stats
    private void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            { Stat.Attack , 0 },
            { Stat.Defense , 0 },
            { Stat.SpAttack , 0 },
            { Stat.SpDefense , 0 },
            { Stat.Speed , 0 }
        };
    }

    // Getter stats
    private int GetStat(Stat stat)
    {
        int statValue = Stats[stat];
        
        int boost = StatBoosts[stat];
        var boostValues = new float[] {1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f};

        statValue = (boost >= 0) ? Mathf.FloorToInt(statValue * boostValues[boost]) : statValue = Mathf.FloorToInt(statValue / boostValues[-boost]);

        return statValue;
    }

    // Apply boosts to the stats
    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in  statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            // at mox +- 6 of increasse or decreasse the stat 
            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);

            if (boost > 0)
            {
                StatusChanges.Enqueue($"{PoqimonBase.name}'s {stat} rose!");
            }
            else 
            {
                StatusChanges.Enqueue($"{PoqimonBase.name}'s {stat} fell!");
            }
        }
    }
    
    //Stats Formulas (from Bulbapedia)
    public int Attack => GetStat(Stat.Attack);
    public int Defense => GetStat(Stat.Defense);
    public int SpAttack => GetStat(Stat.SpAttack);
    public int SpDefense => GetStat(Stat.SpDefense);
    public int Speed => GetStat(Stat.Speed);
    
    public int MaxHp { get; private set; }
    
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
        //Random Modifier (plus critical and aeffectiveness)
        float modifiers = UnityEngine.Random.Range(0.85f, 1f) * critical; // TODO CORREGIR * type;
        //Level dependency
        float a = (2 * attacker.PoqimonLevel + 10)/250f;
        //Damage Calculationç
        float atk = (move.MoveBase.MoveCategory == CategoryType.Special) ? attacker.SpAttack : attacker.Attack;
        float def = (move.MoveBase.MoveCategory == CategoryType.Special) ? this.SpDefense : this.Defense;
        d = a * move.MoveBase.MovePower * ((float)atk/ def) + 2;
        int damage = Mathf.FloorToInt(d + modifiers);

        CurrentHp -= damage;
        if (CurrentHp <= 0)
        {
            CurrentHp = 0;
            damageDetails.Fainted = true;
        }
        return damageDetails;
    }

    public Move GetRndMove()
    {
        int r = UnityEngine.Random.Range(0, Moves.Count);
        return Moves[r];
    }

    public void OnBattleOver()
    {
        ResetStatBoost();
    }

    public bool CheckForLevelUp()
    {
        if (Exp > PoqimonBase.GetExperienceForLvl(poqimonLevel + 1))
        {
            ++poqimonLevel;
            CalcStats();
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
        CalcStats();
        //Set currentHP to new MaxHp (Heal up poqimon)
        CurrentHp = MaxHp;
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}
