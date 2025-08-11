using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

interface ISaveLoad
{
    object GetData();
}
[System.Serializable]
public class EntityData
{
    public string ResourcesPath;
    public virtual void OnLoad(GameObject iObject) { }
}

public static class UserData
{
    readonly static string fileName = "loopscores.dat";
    static string filePath = "";
    public static UserHighScores userHighScores;
    public static ArrayList datas = new ArrayList();

    #region SAVELOAD
    private static void updateFilePath(ref string iFileName)
    {
        string path = Application.persistentDataPath;
        filePath = Path.Combine(path, fileName);
    }

    public static bool SaveHighScores()
    {
        datas.Clear();
        foreach (LoopHighScore lhs in userHighScores.highScores)
        {
            datas.Add(lhs);
        }

        updateFilePath(ref filePath);
        if (File.Exists(filePath))
        { File.Delete(filePath); }

        FileStream fs = new FileStream(filePath, FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();
        ArrayList save_datas = new ArrayList();
        foreach (object o in datas)
        {
            ISaveLoad saveable_o = (ISaveLoad)o;
            save_datas.Add(saveable_o.GetData());
        }
        try
        {
            formatter.Serialize(fs, save_datas);
        }
        catch (System.Runtime.Serialization.SerializationException e)
        {
            Debug.LogError("Failed to serialize : " + e.Message);
            return false;
        }
        finally
        {
            fs.Close();
        }
        save_datas.Clear();
        save_datas = null;
        formatter = null;
        fs = null;
        return true;
    }

    public static bool LoadHighScores()
    {
        updateFilePath(ref filePath);
        if (!File.Exists(filePath))
        { return false; }

        FileStream fs = new FileStream(filePath, FileMode.Open);
        ArrayList load_datas;
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            load_datas = (ArrayList)formatter.Deserialize(fs);
            formatter = null;
        }
        catch (System.Runtime.Serialization.SerializationException e)
        {
            Debug.LogError("Failed to deserialize profile : " + e.Message);
            return false;
        }
        finally
        {
            fs.Close();
        }
        datas.Clear();
        foreach (object o in load_datas)
        {
            LoopHighScore lhs = (LoopHighScore)o;
            if (lhs != null)
            {
                userHighScores.AddHighScore(lhs);
                continue;
            }

            EntityData ed = (EntityData)o;
            if (ed != null)
            {
                ed.OnLoad(UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>(ed.ResourcesPath)));        
            }
        }
        load_datas.Clear();
        load_datas = null;
        fs = null;
        return true;
    }

    public static bool DeleteHighScoresData()
    {
        updateFilePath(ref filePath);
        if (!File.Exists(filePath))
        { return false; }

        File.Delete(filePath);
        return !File.Exists(filePath);
    }
    #endregion

    #region DATAUTILS
    public static void Init()
    {
        userHighScores = new UserHighScores();
    }
    public static bool IsNewHighScore(LoopHighScore iNewLHS)
    {
        if (userHighScores.highScores == null)
            return true; // ??

        bool isSameLoop = false;
        foreach (LoopHighScore lhs in userHighScores.highScores)
        {
            if (lhs.gameMode != iNewLHS.gameMode)
                continue;
            if (lhs.ids.Length != iNewLHS.ids.Length)
                continue;
            // try match ids
            List<byte> A = new List<byte>(lhs.ids);
            List<byte> B = new List<byte>(iNewLHS.ids);
            A = A.OrderBy(e => e).ToList();
            B = B.OrderBy(e => e).ToList();
            for (int i = 0; i < lhs.ids.Length; i++)
            {
                if (A[i] != B[i])
                {
                    isSameLoop = false;
                    break;
                }
                isSameLoop = true;
            }
            if (isSameLoop)
            {
                return iNewLHS.score > lhs.score;
            }
        }
        // its a first high score for those game ids
        return true;
    }

    public static void AddHighScore(LoopHighScore iLHS)
    {
        userHighScores.AddHighScore(iLHS);
    }
    #endregion
}
