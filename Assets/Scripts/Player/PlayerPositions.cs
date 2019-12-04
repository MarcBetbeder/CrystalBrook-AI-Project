using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Positions")]
public class PlayerPositions : ScriptableObject
{
    [SerializeField] string[] positions;

    public string[] GetPositions()
    {
        return positions;
    }

    public int GetNumPositions()
    {
        return positions.Length;
    }
}
