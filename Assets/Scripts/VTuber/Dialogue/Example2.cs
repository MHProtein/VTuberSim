using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject enemy; // Drag your Enemy GameObject into this field in the Inspector

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 向 enemy 发送消息，调用名为 "TakeDamage" 的方法，传递一个参数
            enemy.SendMessage("TakeDamage", 10);
        }
    }
}