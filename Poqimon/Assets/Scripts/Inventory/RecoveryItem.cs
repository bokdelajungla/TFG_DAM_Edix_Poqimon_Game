using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new recovery item")]
public class RecoveryItem : ItemBase
{

    [Header("HP")]
   [SerializeField] int hpAmout;
   [SerializeField] bool restoreMaxHP;

    [Header("PP")]
   [SerializeField] int pAmout;
   [SerializeField] bool restoreMaxPP;

    [Header("Status condition")]
   /* [SerializeField] ConditionID status; */
   [SerializeField] bool recoverAllStatus;

    [Header("Revive")]
   [SerializeField] bool revive;
   [SerializeField] bool maxRevive;

}
