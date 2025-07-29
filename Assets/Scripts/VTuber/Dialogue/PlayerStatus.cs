using UnityEngine;
using VTuber.Core.Foundation;
public class PlayerStatus : VMonoBehaviour
{
    public int Stamina = 100;
    public int Experience = 0;

    public void AddExperience(int amount)
    {
        Experience += amount;
        Debug.Log($"[PlayerStatus] Added {amount} EXP. Total now: {Experience}");
    }
}