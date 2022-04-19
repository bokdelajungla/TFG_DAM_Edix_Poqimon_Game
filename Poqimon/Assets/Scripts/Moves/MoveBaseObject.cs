using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

[CreateAssetMenu(fileName = "Move", menuName = "Poqimon/New Move")]
public class MoveBaseObject : ScriptableObject
{
    [SerializeField] private string moveName;
    public string MoveName => moveName;
    [TextArea][SerializeField] private string moveDescription;
    public string MoveDescription => moveDescription;
    [SerializeField] private PoqimonType moveType;
    public PoqimonType MoveType => moveType;
    [SerializeField] private CategoryType moveCategory;
    public CategoryType MoveCategory => moveCategory;
    [SerializeField] private int movePower;
    public int MovePower => movePower;
    [SerializeField] private int moveAccuracy;
    public int MoveAccuracy => moveAccuracy;
    [SerializeField] private int movePP;
    public int MovePP => movePP;
}

public enum CategoryType
{
    Physical,
    Special,
    Status
}
