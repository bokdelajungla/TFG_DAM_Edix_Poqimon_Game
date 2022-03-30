using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Poqimon", menuName = "Poqimon/new test Poqimon")]
public class TestPokemonBaseAdri : ScriptableObject
{
    [SerializeField] private string name;
    [TextArea] [SerializeField] private string description;
    [SerializeField] private Sprite frontSprite;
    [SerializeField] private Sprite backSprite;
    [SerializeField] private PokemonType type1;
    [SerializeField] private PokemonType type2;

    [SerializeField] private int maxHp;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private int specialAttack;
    [SerializeField] private int specialDefense;
    [SerializeField] private int speed;
    
    /*
     * ESTO HAY QUE AÑADIRLO A LA CLASE PoqimonBase.cs QUE HAGA JORGE!!!!
     */

    [SerializeField] private List<LearnableMove> learnableMoves;

    public List<LearnableMove> LearnableMoves => learnableMoves;
    public class LearnableMove
    {
        [SerializeField] private MoveBase moveBase;
        [SerializeField] private int level;

        public MoveBase Base => moveBase;
        public int Level => level;
    }

    // AÑADIR HASTA AQUI
    
    public int MaxHp => maxHp;
    public int Attack => attack;
    public int Defense => defense;
    public int SpecialAttack => specialAttack;
    public int SpecialDefense => specialDefense;
    public int Speed => speed;
    
    public enum PokemonType
    {
        None, 
        Normal,
        Fire,
        Water
    }
}
