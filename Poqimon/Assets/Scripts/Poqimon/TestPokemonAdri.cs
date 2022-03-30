using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class TestPokemonAdri
{
    
    TestPokemonBaseAdri _base;
    private int level;
    
    /*
     * ESTO HAY QUE AÑADIRLO A LA CLASE Poqimon.cs QUE HAGA JORGE!!!!
     */
    
    public int HP { get; set; }
    
    public List<Move> Moves { get; set; }
    
    /// <summary>
    /// Constructor de la clase Poqimon
    /// </summary>
    /// <param name="pBase">caracteristicas base del poqimon</param>
    /// <param name="pLevel">nivel de poqimon</param>
    public TestPokemonAdri(TestPokemonBaseAdri pBase, int pLevel)
    {
        _base = pBase;
        level = pLevel;
        HP = _base.MaxHp;
        
        Moves = new List<Move>();
        // Añade desde 1 hasta los 4 primeros movimientos que puede aprender un poqimon
        foreach (var move in _base.LearnableMoves)
        {
            if (move.Level <= level)
            {
                Moves.Add(new Move(move.Base));
            }
            // Si ya ha aprendido cuatro movimientos, no añade más
            if (Moves.Count >= 4)
                break;
        }
    }
    
    // AÑADIR HASTA AQUI
    
    
    public int MaxHp => Mathf.FloorToInt(((_base.MaxHp*level)/100f)+10);
    public int Attack => Mathf.FloorToInt(((_base.Attack*level)/100f)+5);
    public int Defense => Mathf.FloorToInt(((_base.Defense*level)/100f)+5);
    public int SpecialAttack => Mathf.FloorToInt(((_base.SpecialAttack*level)/100f)+5);
    public int SpecialDefense => Mathf.FloorToInt(((_base.SpecialDefense*level)/100f)+5);
    public int Speed => Mathf.FloorToInt(((_base.Speed*level)/100f)+5);

    

    
}
