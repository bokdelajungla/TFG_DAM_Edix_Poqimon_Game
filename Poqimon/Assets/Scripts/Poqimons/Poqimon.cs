using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poqimon {

    private PoqimonBaseObject poqimonBase;
    private int poqimonLevel;
    public int PoqimonLevel => poqimonLevel;
    public List<Move> moves;
    public List<Move> Moves => moves;
    private int currentHp;
    public int CurrentHp => currentHp;

    //Builder
    public Poqimon (PoqimonBaseObject poqimonBase, int poqimonLevel){
        this.poqimonBase = poqimonBase;
        this.poqimonLevel = poqimonLevel;
        this.currentHp = poqimonBase.MaxHP;
        this.moves = new List<Move>();

        foreach (var learnableMove in poqimonBase.LearnableMoves)
        {
            if(learnableMove.LearnLevel <= poqimonLevel)
            {
                moves.Add(new Move(learnableMove.MoveBase));
            }
            if(moves.Count >= 4)
            {
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

    

}
