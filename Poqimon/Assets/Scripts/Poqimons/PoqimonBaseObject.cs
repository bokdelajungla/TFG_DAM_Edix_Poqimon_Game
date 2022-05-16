using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Poqimon", menuName = "Poqimon/New Poqimon")]

public class PoqimonBaseObject : ScriptableObject
{
    [SerializeField] private int poqimonNumber;
    public int PoqimonNumber => poqimonNumber;
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
    [SerializeField] private int maxHP;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private int spAttack;
    [SerializeField] private int spDefense;
    [SerializeField] private int speed;
    public int MaxHP => maxHP;
    public int Attack => attack;
    public int Defense => defense;
    public int SpAttack => spAttack;
    public int SpDefense => spDefense;
    public int Speed => speed;

    //Experience Properties
    [SerializeField] int expYield;
    public int ExpYield => expYield;
    [SerializeField] GrowthRate growthRate;
    public GrowthRate GrowthRate => growthRate;

    //Cathcing Properties
    // data from https://bulbapedia.bulbagarden.net/wiki/List_of_Pok%C3%A9mon_by_catch_rate
    [SerializeField] int catchRate;
    public int CatchRate => catchRate;

    //Movesets
    [SerializeField] private List<LearnableMove> learnableMoves;
    public List<LearnableMove> LearnableMoves => learnableMoves;

    //Evolution Properties
    [SerializeField] List<Evolution> evolutions;
    public List<Evolution> Evolutions => evolutions;

    public int MaxNumberOfMoves {get; set;} = 4;

    public int GetExperienceForLvl(int level)
    {
        if (growthRate == GrowthRate.Fast)
            return Mathf.FloorToInt(4 * (level * level * level ) / 5);
        else if (growthRate == GrowthRate.MediumFast)
            return Mathf.FloorToInt(level * level * level);
        else if (growthRate == GrowthRate.MediumSlow)
            return Mathf.FloorToInt((6 * (level * level * level ) / 5) - (15 * (level * level)) + (100 * level) - 140);
        else if (growthRate == GrowthRate.Slow)
            return Mathf.FloorToInt(5 * (level * level * level) / 4);

        return -1;
    }

}

// Type of the Poqimon
public enum PoqimonType
{
    None,
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

//Poqimon Stats 
public enum Stat
{
    Attack,
    Defense,
    SpAttack,
    SpDefense,
    Speed
}

// Effectivenes of the types
public class TypeChart
{
    static float[][] chart =
    {
                             //Defending type       
    //AttkType               Normal,Fight,Flyng,Poisn, Grnd, Rock,  Bug,Ghost,Steel, Fire,Water,Grass,Elctr,Psych,  Ice,Dragn, Dark,Fairy//  
    /*Normal*/   new float[] {    1,    1,    1,    1,    1, 0.5f,    1,    0, 0.5f,    1,    1,    1,    1,    1,    1,    1,    1,    1},
    /*Fight*/    new float[] {    2,    1, 0.5f, 0.5f,    1,    2, 0.5f,    0,    2,    1,    1,    1,    1, 0.5f,    2,    1,    2, 0.5f},
    /*Flying*/   new float[] {    1,    2,    1,    1,    1, 0.5f,    2,    1, 0.5f,    1,    1,    2, 0.5f,    1,    1,    1,    1,    1},
    /*Poison*/   new float[] {    1,    1,    1, 0.5f, 0.5f, 0.5f,    1, 0.5f,    0,    1,    1,    2,    1,    1,    1,    1,    1,    2},
    /*Ground*/   new float[] {    1,    1,    0,    2,    1,    2, 0.5f,    1,    2,    2,    1, 0.5f,    2,    1,    1,    1,    1,    1},
    /*Rock*/     new float[] {    1, 0.5f,    2,    1, 0.5f,    1,    2,    1, 0.5f,    2,    1,    1,    1,    1,    2,    1,    1,    1},
    /*Bug*/      new float[] {    1, 0.5f, 0.5f, 0.5f,    1,    1,    1, 0.5f, 0.5f, 0.5f,    1,    2,    1,    2,    1,    1,    2, 0.5f},
    /*Ghost*/    new float[] {    0,    1,    1,    1,    1,    1,    1,    2,    1,    1,    1,    1,    1,    2,    1,    1, 0.5f,    1},
    /*Steel*/    new float[] {    1,    1,    1,    1,    1,    2,    1,    1, 0.5f, 0.5f, 0.5f,    1, 0.5f,    1,    2,    1,    1,    2},
    /*Fire*/     new float[] {    1,    1,    1,    1,    1, 0.5f,    2,    1,    2, 0.5f, 0.5f,    2,    1,    1,    2, 0.5f,    1,    1},
    /*Water*/    new float[] {    1,    1,    1,    1,    2,    2,    1,    1,    1,    2, 0.5f, 0.5f,    1,    1,    1, 0.5f,    1,    1},
    /*Grass*/    new float[] {    1,    1, 0.5f, 0.5f,    2,    2, 0.5f,    1, 0.5f, 0.5f,    2, 0.5f,    1,    1,    1, 0.5f,    1,    1},
    /*Electr*/   new float[] {    1,    1,    2,    1,    0,    1,    1,    1,    1,    1,    2, 0.5f, 0.5f,    1,    1, 0.5f,    1,    1},
    /*Psychc*/   new float[] {    1,    2,    1,    2,    1,    1,    1,    1, 0.5f,    1,    1,    1,    1, 0.5f,    1,    1,    0,    1},
    /*Ice*/      new float[] {    1,    1,    2,    1,    2,    1,    1,    1, 0.5f, 0.5f, 0.5f,    2,    1,    1, 0.5f,    2,    1,    1},
    /*Dragon*/   new float[] {    1,    1,    1,    1,    1,    1,    1,    1, 0.5f,    1,    1,    1,    1,    1,    1,    2,    1,    0},
    /*Dark*/     new float[] {    1, 0.5f,    1,    1,    1,    1,    1,    2,    1,    1,    1,    1,    1,    2,    1,    1, 0.5f, 0.5f},
    /*Fairy*/    new float[] {    1,    2,    1, 0.5f,    1,    1,    1,    1, 0.5f, 0.5f,    1,    1,    1,    1,    1,    2,    2,    1}
    };

    public static float GetEffectiveness(PoqimonType atkType, PoqimonType defType)
    {
        if (atkType == PoqimonType.None || defType == PoqimonType.None)
        {
            return 1;
        }

        int row = (int) atkType - 1;
        int col = (int) defType - 1;

        return chart[row][col];
    }
    
}

[Serializable]
public class LearnableMove
{
    [SerializeField] private MoveBaseObject moveBase;
    public MoveBaseObject MoveBase => moveBase;
    [SerializeField] private int learnLevel;
    public int LearnLevel => learnLevel;

}

public enum GrowthRate
{
    Fast, MediumFast, MediumSlow, Slow
}

[System.Serializable]
public class Evolution
{
    [SerializeField] PoqimonBaseObject evolvesInto;
    [SerializeField] int levelRequired;

    public PoqimonBaseObject EvolvesInto => evolvesInto;
    public int LevelRequired => levelRequired;
}
