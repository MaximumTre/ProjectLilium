using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon Info")]
public class WeaponInfo : ScriptableObject
{
    public GameObject WeaponPrefab;

    [Tooltip("Time till second attack can be used.")]
    public float First_to_Second_Time = 0.3f;

    [Tooltip("Time till first attack ends and attacks reset.")]
    public float First_End_Time = 1f;
    
    [Tooltip("Time till final attack can be used.")]
    public float Second_To_Final = 0.4f;
    
    [Tooltip("Time till second attack ends and attacks reset.")]
    public float Second_End_Time = 1;

    [Tooltip("Time till final attack ends and attacks reset.")]
    public float Final_End_Time = 1.3f;
}
