using UnityEngine;
using VTuber.Core.Foundation;
public class Enemy : MonoBehaviour
{
    void TakeDamage(int amount)
    {
        VDebug.Log("Enemy took damage: " + amount);
    }
}