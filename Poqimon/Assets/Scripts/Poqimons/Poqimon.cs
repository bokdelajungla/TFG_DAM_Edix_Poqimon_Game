using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

[Serializable]
public class Poqimon 
{
    /*
     *      *****************
     *      *   ATRIBUTES   *
     *      *****************
     */ 
    
    /*
     *      SERIALIZED FIELDS
     */
    
    // Poqimon Base characterictis 
    [SerializeField] private PoqimonBase poqimonBase;
   /// <summary>
   /// Poqimon base characteristics' getter
   /// </summary>
    public PoqimonBase PoqimonBase => poqimonBase;

    // Poqimon's level
    [SerializeField] int poqimonLevel;
    // Poqimon's level getter
    public int PoqimonLevel => poqimonLevel;

    /*
     *      STATS & STATS BOOSTS
     */
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }
    
    /*
     *      STATUS & STATUS CHANGES
     */
    
    //
    public Condition Status { get; set; }
    
    //
    public int StatusTime { get; set; }
    
    //
    public Condition VolatileStatus { get; set; }
    
    //
    public int VolatileStatusTime { get; set; }
    
    //
    public Queue<string> StatusChanges { get; private set; } = new Queue<string>();

    /*
     *      HP & EXPERIENCE
     */
    
    //
    public int CurrentHp { get; set; }
    
    //
    public bool HpChanged { get; set; }

    // Poqimon ecperience (with getter & setter)
    public int Exp {get; set;}
    
    /*
     *      MOVEMENTS
     */

    // Poqimon movements's list (with getter and setter)
    public List<Move> Moves {get; set;}
    // Current move used by the poqimon (with getter and setter)
    public Move CurrentMove { get; set; }

    /*
     *  EVENTS
     */
    
    //
    public event Action OnStatusChanged;
    //
    public event Action OnHPChanged;
    
    /*
     *      ******************
     *      *   CONSTUCTOR   *
     *      ******************
     */ 
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="pBase">poqimon's base atributes</param>
    /// <param name="pLevel">poqimon's level</param>
    public Poqimon(PoqimonBase pBase, int pLevel)
    {
        poqimonBase = pBase;
        poqimonLevel = pLevel;

        Init();
    }
    
    /*
     *      ************************
     *      *   PUBLIC FUNCTIONS   *
     *      ************************
     */
    
    /// <summary>
    /// Initializer
    /// </summary>
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
            if(Moves.Count >= PoqimonBase.MaxNumberOfMoves)
            {
                //TODO: Learning logic (only 4 slots)
                break;
            }
        }

        Exp = PoqimonBase.GetExperienceForLvl(PoqimonLevel);
        

        CalcStats();
        CurrentHp = MaxHp;
        
        StatusChanges = new Queue<string>();
        ResetStatBoost();
        Status = null;
        VolatileStatus = null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="saveData"></param>
    public Poqimon(PoqimonSaveData saveData)
    {
        poqimonBase = PoqimonDB.GetObjectByName(saveData.name);
        CurrentHp = saveData.hp;
        poqimonLevel = saveData.level;
        Exp = saveData.exp;

        if (saveData.statusId != null)
            Status = ConditionsDB.Conditions[saveData.statusId.Value];
        else
            Status = null;

        Moves = saveData.moves.Select(m => new Move(m)).ToList();

        CalcStats();
        StatusChanges = new Queue<string>();
        ResetStatBoost();
        VolatileStatus = null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public PoqimonSaveData GetSaveData()
    {
        var saveData = new PoqimonSaveData()
        {
            name = PoqimonBase.PoqimonName,
            hp = CurrentHp,
            level = PoqimonLevel,
            exp = Exp,
            statusId = Status?.Id,
            moves = Moves.Select(m => m.GetSaveData()).ToList()
        };

        return saveData;
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
                StatusChanges.Enqueue($"{PoqimonBase.PoqimonName}'s {stat} rose!");
            }
            else 
            {
                StatusChanges.Enqueue($"{PoqimonBase.PoqimonName}'s {stat} fell!");
            }
        }
    }
    
    /*
     *      STATS GETTERS
     */
    
    //
    public int Attack => GetStat(Stat.Attack);
    //
    public int Defense => GetStat(Stat.Defense);
    //
    public int SpAttack => GetStat(Stat.SpAttack);
    //
    public int SpDefense => GetStat(Stat.SpDefense);
    //
    public int Speed => GetStat(Stat.Speed);
    //
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
        float type = TypeChart.GetEffectiveness(move.MoveBase.MoveType, this.PoqimonBase.PoqimonType1) *
                     TypeChart.GetEffectiveness(move.MoveBase.MoveType, this.PoqimonBase.PoqimonType2);

        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false
        };
        
        //Damage Value
        float d = 0;
        //Random Modifier (plus critical and aeffectiveness)
        float modifiers = UnityEngine.Random.Range(0.85f, 1f) * critical * type;
        //Level dependency
        float a = (2 * attacker.PoqimonLevel + 10)/250f;
        //Damage Calculation
        float atk = (move.MoveBase.MoveCategory == CategoryType.Special) ? attacker.SpAttack : attacker.Attack;
        float def = (move.MoveBase.MoveCategory == CategoryType.Special) ? this.SpDefense : this.Defense;
        d = a * move.MoveBase.MovePower * ((float)atk/ def) + 2;
        int damage = Mathf.FloorToInt(d + modifiers);

        DecreaseHp(damage);
        
        return damageDetails;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="amount"></param>
    public void IncreaseHp(int amount)
    {
        CurrentHp = Mathf.Clamp(CurrentHp + amount, 0, MaxHp);
        OnHPChanged?.Invoke();
    }

    public void DecreaseHp(int damage)
    {
        CurrentHp = Mathf.Clamp(CurrentHp - damage, 0, MaxHp);
        OnHPChanged?.Invoke();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="conditionID"></param>
    public void SetStatus(ConditionID conditionID)
    {
        if (Status != null)
        {
            return;
        }
    
        Status = ConditionsDB.Conditions[conditionID];
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{PoqimonBase.PoqimonName} {Status.StartMsg}");
        OnStatusChanged?.Invoke();
    }

    /// <summary>
    /// 
    /// </summary>
    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="conditionID"></param>
    public void SetVolatileStatus(ConditionID conditionID)
    {
        if (Status != null)
        {
            return;
        }
    
        VolatileStatus = ConditionsDB.Conditions[conditionID];
        VolatileStatus?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{PoqimonBase.PoqimonName} {VolatileStatus.StartMsg}");
    }
    
    /// <summary>
    /// 
    /// </summary>
    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }
    
    /// <summary>
    /// get a random move (IA enemey)
    /// </summary>
    /// <returns>Move the enemy will use</returns>
    public Move GetRndMove()
    {
        // To onlu use moves with more than 0 PP
        var movesWithPP = Moves.Where(m => m.MovePP > 0).ToList();
        
        int r = UnityEngine.Random.Range(0, movesWithPP.Count);
        return movesWithPP[r];
    }

    
    /// <summary>
    /// Reset the Stats and supress the volatile statuses if the battle is over
    /// </summary>
    public void OnBattleOver()
    {
        VolatileStatus = null;
        ResetStatBoost();
    }

    
    /// <summary>
    /// Event before move
    /// Used in case the poqimon has some status don't let the poqimon attack like sleep, paralyzed, ...
    /// Or make attack itself like confusion
    /// </summary>
    /// <returns></returns>
    public bool OnBeforeMove()
    {
        bool canPerformMove = true;
        if (Status?.OnBeforeMove != null)
        {
            if (!Status.OnBeforeMove(this))
            {
                canPerformMove = false;
            }
        }
        if (VolatileStatus?.OnBeforeMove != null)
        {
            if (!VolatileStatus.OnBeforeMove(this))
            {
                canPerformMove = false;
            }
        }
        return canPerformMove;
    }
    
    /// <summary>
    /// Event after the turn
    /// Used in case the poqimon has some status which take life (volatiles or not) like posion, burn, ...
    /// </summary>
    public void OnAfterTurn()
    {
        // (coditional call) We'll only call it if is not null
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public LearnableMove GetLearnableMoveAtCurrentLvl()
    {
        return PoqimonBase.LearnableMoves.Where(x => x.LearnLevel ==  PoqimonLevel).FirstOrDefault(); 
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="learnableMove"></param>
    public void LearnMove(LearnableMove learnableMove)
    {   
        if(Moves.Count <= PoqimonBase
.MaxNumberOfMoves)
        {
            Moves.Add(new Move(learnableMove.MoveBase));
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Evolution CheckForEvolution()
    {
        return PoqimonBase.Evolutions.FirstOrDefault(e => e.LevelRequired <= poqimonLevel);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="evolution"></param>
    public void Evolve (Evolution evolution)
    {   
        //Set Poqimon Base to the Evolution one
        poqimonBase = evolution.EvolvesInto;
        //Recalculate Stat for new poqimon Base
        CalcStats();
        //Set currentHP to new MaxHp (Heal up poqimon)
        CurrentHp = MaxHp;
    }
    
    /*
     *      *************************
     *      *   PRIVATE FUNCTIONS   *
     *      *************************
     */
    
    /// <summary>
    /// Calculate the poqimon stats
    ///  Stats Formulas from https://bulbapedia.bulbagarden.net/wiki/Stat
    /// </summary>
    private void CalcStats()
    {
        //Stats Formulas (from Bulbapedia)
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((poqimonBase.Attack*poqimonLevel) / 100f) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((poqimonBase.Defense*poqimonLevel) / 100f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((poqimonBase.SpAttack*poqimonLevel) / 100f) + 5);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((poqimonBase.SpDefense*poqimonLevel) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((poqimonBase.Speed*poqimonLevel) / 100f) + 5);
        
        MaxHp = Mathf.FloorToInt((poqimonBase.MaxHP * poqimonLevel) / 100f) + 10;
    }
    
    /// <summary>
    /// Reset (to 0) all the stats
    /// </summary>
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
    
    /// <summary>
    /// Fully restore HP & PP
    /// </summary>
    public void Heal() {
        CurrentHp = MaxHp;
        foreach (var move in Moves)
        {
            move.MovePP = move.MoveBase.MovePP;
        }

        CureStatus();
    }
    
    /// <summary>
    /// Get calculate bounded stat (it has especified boosts heights values)
    /// </summary>
    /// <param name="stat"></param>
    /// <returns></returns>
    private int GetStat(Stat stat)
    {
        int statValue = Stats[stat];
        
        int boost = StatBoosts[stat];
        var boostValues = new float[] {1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f};

        statValue = (boost >= 0) ? Mathf.FloorToInt(statValue * boostValues[boost]) : statValue = Mathf.FloorToInt(statValue / boostValues[-boost]);

        return statValue;
    }
}

/// <summary>
/// 
/// </summary>
public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}

/// <summary>
/// 
/// </summary>
[System.Serializable]
public class PoqimonSaveData
{
    public string name;
    public int hp;
    public int level;
    public int exp;
    public ConditionID? statusId;
    public List<MoveSaveData> moves;
}
