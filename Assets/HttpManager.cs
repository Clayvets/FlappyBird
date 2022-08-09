using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HttpManager : MonoBehaviour
{

    [SerializeField]
    private string URL;
    [SerializeField]  Text name;
    [SerializeField]  Text scoreT;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ClickGetScores()
    {
        StartCoroutine(GetScores());
    }

    IEnumerator GetScores()
    {
        string url = URL + "/leaders";
        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if(www.responseCode == 200){
            //Debug.Log(www.downloadHandler.text);
            Scores resData = JsonUtility.FromJson<Scores>(www.downloadHandler.text);

            foreach (ScoreData score in resData.scores)
            {
                Debug.Log(score.userId +" | "+score.value);
            }

            for (int i = 0; i < resData.scores.Length; i++)
            {
                Score(resData.scores[i]);
            }
            
        }
        else
        {
            Debug.Log(www.error);
        }


    }

    public void Score(ScoreData scoreData)
    {

        name.text += scoreData.name + "\n";
        scoreT.text += scoreData.value + "\n";
    }
   
}


[System.Serializable]
public class ScoreData
{
    public int userId;
    public int value;
    public string name;
    

}

[System.Serializable]
public class Scores
{
    public ScoreData[] scores;
}
