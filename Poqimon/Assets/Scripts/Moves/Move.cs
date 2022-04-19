using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    private MoveBaseObject moveBase;
    public MoveBaseObject MoveBase => moveBase;
    private int movePP;
    public int MovePP => movePP;

    public Move(MoveBaseObject moveBase)
    {
        this.moveBase = moveBase;
        this.movePP = moveBase.MovePP;
    }

}
