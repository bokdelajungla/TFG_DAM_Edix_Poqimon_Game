using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

[CreateAssetMenu(fileName = "Move", menuName = "Poqimon/New Move")]
public class MoveBase : ScriptableObject
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

    public MoveTarget Target => target;
    [SerializeField] private MoveTarget target;
    public MoveEffects Effects => effects;
    [SerializeField] private MoveEffects effects;
    public int MovePower => movePower;
    [SerializeField] private int moveAccuracy;
    public int MoveAccuracy => moveAccuracy;
    [SerializeField] private int movePP;
    public int MovePP => movePP;
    [SerializeField] private int priority;
    public int Priority => priority;

    [SerializeField] private AudioClip moveAudio;
    public AudioClip MoveAudio => moveAudio;
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] private List<StatBoost> boosts;
    public List<StatBoost> Boosts => boosts;
    
    [SerializeField] private ConditionID status;
    public ConditionID Status => status;
    
    [SerializeField] private ConditionID volatileStatus;
    public ConditionID VolatileStatus => volatileStatus;
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}

public enum CategoryType
{
    Physical,
    Special,
    Status
}

public enum MoveTarget
{
    Enemy, Player
}
