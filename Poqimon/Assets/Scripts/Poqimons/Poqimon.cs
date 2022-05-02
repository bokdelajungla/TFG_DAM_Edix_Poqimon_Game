using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poqimon {
    
    private PoqimonBaseObject poqimonBase;
    // Cambiado por Adrian para ser accesible desde BattleHUD
    public PoqimonBaseObject PoqimonBase => poqimonBase;
    
    private int poqimonLevel;
    public int PoqimonLevel => poqimonLevel;
    
    public List<Move> moves;
    public List<Move> Moves => moves;
    
    private int currentHp;
    public int CurrentHp 
    {
        get => currentHp;
        set => currentHp = value;
    }
    
    //Builder
    public Poqimon (PoqimonBaseObject poqimonBase, int poqimonLevel){
        this.poqimonBase = poqimonBase;
        this.poqimonLevel = poqimonLevel;
        this.currentHp = MaxHP;
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


    //Stats Formulas (from Bulbapedia)
    public int Attack => Mathf.FloorToInt((poqimonBase.Attack*poqimonLevel) / 100f) + 5;
    public int Defense => Mathf.FloorToInt((poqimonBase.Defense*poqimonLevel) / 100f) + 5;
    public int SpAttack => Mathf.FloorToInt((poqimonBase.SpAttack*poqimonLevel) / 100f) + 5;
    public int SpDefense => Mathf.FloorToInt((poqimonBase.SpDefense*poqimonLevel) / 100f) + 5;
    public int Speed => Mathf.FloorToInt((poqimonBase.Speed*poqimonLevel) / 100f) + 5;
    
    public int MaxHP => Mathf.FloorToInt((poqimonBase.MaxHP*poqimonLevel) / 100f) + 10;


    ///<summary>
    ///Take Damage function (formula from Bulbapedia)
    ///</summary>
    ///<param name="move"> The attacking Poqimon move </param>
    ///<param name="attacker"> The attacking Poqimon object </param>
    ///<returns> true if Poqimon fainted because of the attack, false otherwise </returns> 
    public bool TakeDamage(Move move, Poqimon attacker)
    {
        float modifiers = Random.Range(0.85f, 1f);
        float a = (2 * attacker.PoqimonLevel + 10)/250f;
        float d = a * move.MoveBase.MovePower * ((float)attacker.Attack/ this.Defense) + 2;
        int damage = Mathf.FloorToInt(d + modifiers);

        CurrentHp -= damage;
        if (CurrentHp <= 0)
        {
            CurrentHp = 0;
            return true;
        }
        return false;
    }
}
