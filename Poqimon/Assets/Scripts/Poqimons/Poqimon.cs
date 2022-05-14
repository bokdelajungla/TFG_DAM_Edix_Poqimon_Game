using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Poqimon 
{
    [SerializeField] PoqimonBaseObject poqimonBase;
    [SerializeField] int poqimonLevel;

    public PoqimonBaseObject PoqimonBase => poqimonBase;

    public int PoqimonLevel => poqimonLevel;
    
    public List<Move> moves;
    public List<Move> Moves => moves;
    
    public Dictionary<Stat, int> Stats { get; private set; }
    
    public Dictionary<Stat, int> StatBoosts { get; private set; }
    
    private int _currentHp;
    public int CurrentHp 
    {
        get => _currentHp;
        set => _currentHp = value;
    }
    
    //Initializer
    public void Init(){
        
        // Generate Moves
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
        
        CalcStats();
        CurrentHp = MaxHp;

        StatBoosts = new Dictionary<Stat, int>()
        {
            { Stat.Atk , 0 },
            { Stat.Def , 0 },
            { Stat.SpAtk , 0 },
            { Stat.SpDef , 0 },
            { Stat.Speed , 0 }
        };
    }

    // calculate the stats of the poqimon
    void CalcStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Atk, Mathf.FloorToInt((poqimonBase.Attack*poqimonLevel) / 100f) + 5);
        Stats.Add(Stat.Def, Mathf.FloorToInt((poqimonBase.Defense*poqimonLevel) / 100f) + 5);
        Stats.Add(Stat.SpAtk, Mathf.FloorToInt((poqimonBase.SpAttack*poqimonLevel) / 100f) + 5);
        Stats.Add(Stat.SpDef, Mathf.FloorToInt((poqimonBase.SpDefense*poqimonLevel) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((poqimonBase.Speed*poqimonLevel) / 100f) + 5);
        
        MaxHp = Mathf.FloorToInt((poqimonBase.MaxHP*poqimonLevel) / 100f) + 10;
    }

    int GetStat(Stat stat)
    {
        int statValue = Stats[stat];
        
        // Apply boost to the stat
        int boost = StatBoosts[stat];
        var boostValues = new float[] {1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f};

        statValue = (boost >= 0) ? Mathf.FloorToInt(statValue * boostValues[boost]) : statValue = Mathf.FloorToInt(statValue / boostValues[-boost]);

        return statValue;
    }

    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in  statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);
            
            Debug.Log($"{stat} has been boosted to {StatBoosts[stat]}");
        }
    }
    
    //Stats Formulas (from Bulbapedia)
    public int Attack => GetStat(Stat.Atk);
    public int Defense => GetStat(Stat.Def);
    public int SpAttack => GetStat(Stat.SpAtk);
    public int SpDefense => GetStat(Stat.SpDef);
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
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}
