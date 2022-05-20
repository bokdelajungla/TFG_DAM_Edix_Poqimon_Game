using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public MoveBase MoveBase {get; set;}
    public int MovePP {get; set;}

    public Move(MoveBase moveBase)
    {
        MoveBase = moveBase;
        MovePP = moveBase.MovePP;
    }

    public Move(MoveSaveData saveData)
    {
        MoveBase =  MoveDB.GetObjectByName(saveData.name);
        MovePP = saveData.pp;
    }

    public MoveSaveData GetSaveData()
    {
        var saveData = new MoveSaveData()
        {
            name = MoveBase.MoveName,
            pp = MovePP
        };
        return saveData;
    }

}

[Serializable]
public class MoveSaveData
{
    public string name;
    public int pp;
}