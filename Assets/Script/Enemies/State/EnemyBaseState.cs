using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBaseState
{
    public abstract void EnterState(EnemyAI ai);
    public abstract void UpdateState(EnemyAI ai);
    public abstract void ExitState(EnemyAI ai);
}
