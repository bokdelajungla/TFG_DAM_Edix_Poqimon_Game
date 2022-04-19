using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Poqimon", menuName = "Poqimon/New Poqimon")]

public class PoqimonBaseObject : ScriptableObject
{
    [SerializeField] private int poqimonId;
    public int PoqimonId => poqimonId;
    [SerializeField] private string poqimonName;
    public string PoqimonName => poqimonName;
    [TextArea][SerializeField] private string poqimonDescription;
    public string PoqimonDescription => poqimonDescription; 
    
    [SerializeField] private Sprite poqimonFrontSprite;
    public Sprite PoqimonFrontSprite => poqimonFrontSprite;
    [SerializeField] private Sprite poqimonBackSprite;
    public Sprite PoqimonBackSprite => poqimonBackSprite;

    [SerializeField] private PoqimonType poqimonType1;
    public PoqimonType PoqimonType1 => poqimonType1;
    [SerializeField] private PoqimonType poqimonType2;
    public PoqimonType PoqimonType2 => poqimonType2;

    //Stats
    [SerializeField]private int maxHP;
    [SerializeField]private int attack;
    [SerializeField]private int defense;
    [SerializeField]private int spAttack;
    [SerializeField]private int spDefense;
    [SerializeField]private int speed;
    public int MaxHP => maxHP;
    public int Attack => attack;
    public int Defense => defense;
    public int SpAttack => spAttack;
    public int SpDefense => spDefense;
    public int Speed => speed;

    //Movesets
    [SerializeField] private List<LearnableMove> learnableMoves;
    public List<LearnableMove> LearnableMoves => learnableMoves;
}

public enum PoqimonType
{
    Normal,
    Fight,
    Flying,
    Poison,
    Ground,
    Rock,
    Bug,
    Ghost,
    Steel,
    Fire,
    Water,
    Grass,
    Electr,
    Psychc,
    Ice,
    Dragon,
    Dark,
    Fairy
}

[Serializable]
public class LearnableMove
{
    [SerializeField] private MoveBaseObject moveBase;
    public MoveBaseObject MoveBase => moveBase;
    [SerializeField] private int learnLevel;
    public int LearnLevel => learnLevel;

}

