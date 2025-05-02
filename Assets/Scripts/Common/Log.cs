using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Log : MonoBehaviour
{
    [SerializeField]    private Text[] logTexts = new Text[10];
    [SerializeField]    private Queue<string> logQueue = new Queue<string>();

    /// <summary>
    /// ログを追加
    /// FindObjectOfType<TextLogger>().AddLog("~~~")
    /// </summary>
    /// <param name="message">表示するテキスト</param>
    public void AddLog(string message)
    {
        logQueue.Enqueue(message);

        // ログが上限を超えていたら古いのを削除
        if (logQueue.Count > 10)
        {
            logQueue.Dequeue();
        }

        // 表示を更新
        string[] logs = logQueue.ToArray();
        for (int i = 0; i < logTexts.Length; i++)
        {
            if (i < logs.Length)
            {
                logTexts[i].text = logs[i];
            }
            else
            {
                logTexts[i].text = "";
            }
        }
    }
}
