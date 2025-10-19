using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/BulletList")]
public class BulletList:ScriptableObject
{
    public TextAsset csvFile;
    public List<BulletInfo> bullets=new List<BulletInfo>();

    public void InitBulletList()
    {
        string[] data = csvFile.text.Split(new char[] { '\n' });
        for (int i = 1; i < data.Length - 1; i++)
        {

            string[] row = data[i].Split(new char[] { ',' });
            BulletInfo bi = new BulletInfo();
            // 处理每一行数据
            for (int j = 0; j < row.Length; j++)
            {
                switch (j)
                {
                    case 0:
                        bi.bulletContent = row[j];
                        break;
                    case 1:
                        bi.senderName = row[j];
                        break;
                    // case 2:
                    //     bi.senderIconId = int.Parse(row[j]);
                    //     break;
                }
            }

            bullets.Add(bi);
        }
    }
}
