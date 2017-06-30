using UnityEngine;
using System.Collections;

public static class StartingVariables
{
    public const int shipHP = 30;
    public const int shipShieldHP = 2;
    public const float shipShieldReactivationTime = 5.0f;

    public const int shipDamage = 30;

    public const float shipSpeed = 2.0f;
    public const float shipTurnSpeed = 1.75f;
    public const float shipDetectionRadius = 1.0f;

    public const float projectileSpeed = 3.5f;
    public const float projectileTravelTime = 1.5f;     //seconds.
    public const float weaponFireRate = 1.0f;
    public const float weaponFireArc = 60.0f;          //degrees from forward

    public const float aiUpdateTicks = 0.25f;
}
