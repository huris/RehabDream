using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Utils
{

    public void ChangePage(string pageTopage)
    {

        string[] pages = pageTopage.Split(new char[] { ',' });
        string fromPage = pages[0];
        string toPage = pages[1];
        GameObject root = GameObject.Find("Canvas");
        if (fromPage.CompareTo(toPage) != 0)
        {
            try
            {
                root.transform.Find(toPage).gameObject.active = true;
            }
            catch (Exception e)
            {
                Debug.LogWarning("toPage:" + e);
            }
            try
            {
                root.transform.Find(fromPage).gameObject.active = false;
            }
            catch (Exception e)
            {
                Debug.LogWarning("fromPage:" + e);
            }
        }
        //Debug.Log("change page from " + fromPage + " to " + toPage);


    }
    public static void SetImageSprite(Image image, string path)
    {
        double startTime = (double)Time.time;
        //创建文件读取流
        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        fileStream.Seek(0, SeekOrigin.Begin);
        //创建文件长度缓冲区
        byte[] bytes = new byte[fileStream.Length];
        //读取文件
        fileStream.Read(bytes, 0, (int)fileStream.Length);
        //释放文件读取流
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;

        //创建Texture
        int width = 64;
        int height = 64;
        Texture2D texture = new Texture2D(width, height);
        texture.LoadImage(bytes);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        image.sprite = sprite;

    }
    public IEnumerator Load(Image image, string path)
    {
        yield return new WaitForSeconds(0f);
        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        fileStream.Seek(0, SeekOrigin.Begin);
        //创建文件长度缓冲区
        byte[] bytes = new byte[fileStream.Length];
        //读取文件
        fileStream.Read(bytes, 0, (int)fileStream.Length);
        //释放文件读取流
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;
        //创建Texture
        int width = 200;
        int height = 200;
        Texture2D texture = new Texture2D(width, height);
        texture.LoadImage(bytes);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        image.sprite = sprite;
        image.color = new Color(1, 1, 1, 1);

    }
    public static List<string> ReadFile(string path)
    {
        List<string> lines = new List<string>();
        StreamReader sr = new StreamReader(path, Encoding.Default);
        String line;
        while ((line = sr.ReadLine()) != null)
        {
            lines.Add(line);
        }
        return lines;
    }
    public static bool checkFormat(string type, string input)
    {
        switch (type)
        {
            case "name":
                {
                    char[] chars = input.ToCharArray();
                    if (chars.Length == 0)
                    {
                        return false;
                    }
                    for (int i = 0; i < chars.Length; i++)
                    {
                        if (((int)chars[i] >= 0x4e00 && (int)chars[i] <= 0x9fbb)
                            || ((int)chars[i] >= 0 && (int)chars[i] <= 127) || chars[i] == ',' || chars[i] == '。'
                            || chars[i] == '，' || chars[i] == '.' || chars[i] == '、' || chars[i] == '\\'
                            || chars[i] == '/' || chars[i] == '；' || chars[i] == ';' || chars[i] == ':'
                            || chars[i] == '：')
                        {
                        }
                        else
                        {
                            return false;
                        }
                    }
                    break;
                }
            case "age":
                {
                    char[] chars = input.ToCharArray();
                    if (chars.Length == 0)
                    {
                        return false;
                    }
                    for (int i = 0; i < chars.Length; i++)
                    {
                        if ((chars[i] >= '0' && chars[i] <= '9'))
                        {
                        }
                        else
                        {
                            return false;
                        }
                    }
                    break;
                }
            case "weight":
                {
                    char[] chars = input.ToCharArray();
                    if (chars.Length == 0)
                    {
                        return false;
                    }
                    for (int i = 0; i < chars.Length; i++)
                    {
                        if ((chars[i] >= '0' && chars[i] <= '9'))
                        {
                        }
                        else
                        {
                            return false;
                        }
                    }
                    break;
                }
            case "pwd":
                {
                    char[] chars = input.ToCharArray();
                    if (chars.Length == 0)
                    {
                        return false;
                    }
                    for (int i = 0; i < chars.Length; i++)
                    {
                        if ((chars[i] >= '0' && chars[i] <= '9')
                            || (chars[i] >= 'a' && chars[i] <= 'z')
                            || (chars[i] >= 'A' && chars[i] <= 'Z'))
                        {
                        }
                        else
                        {
                            return false;
                        }
                    }
                    break;
                }
        }
        return true;
    }
    public static bool checkRepeatAction(string input, List<Action> list)
    {

        for (int i = 0; i < list.Count; i++)
        {
            if (input.CompareTo(list[i].name) == 0)
            {
                return true;
            }
        }
        return false;
    }

}
