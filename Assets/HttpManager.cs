using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HttpManager : MonoBehaviour
{

    [SerializeField]
    private string URL;

    private string Token;
    private string Username;

    [SerializeField]  Text name;
    [SerializeField]  Text scoreT;

    // Start is called before the first frame update
    void Start()
    {
        Token = PlayerPrefs.GetString("token");
        Username = PlayerPrefs.GetString("username");
        Debug.Log("TOKEN:" + Token);

        StartCoroutine(ObtenerPerfil());
        

    }

    public void ClickGetScores()
    {
        StartCoroutine(GetScores());
    }

    private static string GetInputData()
    {
        AuthData data = new AuthData();

        data.usuario = GameObject.Find("Usuario").GetComponent<InputField>().text;
        data.contraseña = GameObject.Find("Contraseña").GetComponent<InputField>().text;

        string postData = JsonUtility.ToJson(data);
        return postData;
    }
    public void ClickRegistro()
    {
        string postData = GetInputData();
        StartCoroutine(Registro(postData));

    }
    public void ClickIngreso()
    {
        string postData = GetInputData();
        StartCoroutine(Ingreso(postData));
    }

    IEnumerator GetScores()
    {
        string url = URL + "/api/usuarios" + "?limit=5&sort=true";
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("x-token", Token);

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

    IEnumerator Registro(string postData)
    {
        Debug.Log("Registro: " + postData);

        string url = URL + "/api/usuarios";
        UnityWebRequest www = UnityWebRequest.Put(url, postData);
        www.method = "POST";
        www.SetRequestHeader("content-type", "application/json");


        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            //Debug.Log(www.downloadHandler.text);
            AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);

            Debug.Log("Registrado" + resData.usuarioPostman.username + ", id:" + resData.usuarioPostman._id);
            
            StartCoroutine(Ingreso(postData));
            

        }
        else
        {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.text);
        }


    }

    IEnumerator Ingreso(string postData)
    {
        Debug.Log("Ingreso: " + postData);

        string url = URL + "/api/auth/login";
        UnityWebRequest www = UnityWebRequest.Put(url, postData);
        www.method = "POST";
        www.SetRequestHeader("content-type", "application/json");

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            //Debug.Log(www.downloadHandler.text);
            AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);

            Debug.Log("Autenticado" + resData.usuarioPostman.username + ", id:" + resData.usuarioPostman._id);
            Debug.Log("TOKEN: " + resData.token);

            PlayerPrefs.SetString("token", resData.token);
            PlayerPrefs.SetString("username", resData.usuarioPostman.username);
            SceneManager.LoadScene("SampleScene");

        }
        else
        {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.text);
        }


    }

    IEnumerator ObtenerPerfil()
    {
        string url = URL + "/api/usuarios" + Username;
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("x-token", Token);

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            //Debug.Log(www.downloadHandler.text);
            AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);

            Debug.Log("Token valido" + resData.usuarioPostman.username + ", id:" + resData.usuarioPostman._id + " y su score es: " + resData.usuarioPostman.score);
            SceneManager.LoadScene("SampleScene");
        }
        else
        {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.text);
        }
    }

    IEnumerator SetScore()
    {
        Debug.Log("Patch score:");


        string url = URL + "/api/usuarios" + "?limit=5&sort=true";
        UnityWebRequest www = UnityWebRequest.Put(url,Username);
        www.method = "PATCH";
        www.SetRequestHeader("content-type", "application/json");
        www.SetRequestHeader("x-token", Token);


        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            //Debug.Log(www.downloadHandler.text);
            AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);

            
            


        }
        else
        {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.text);
        }


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

[System.Serializable]
public class AuthData
{
    public string usuario;
    public string contraseña;
    public UserData usuarioPostman;
    public string token;
}

public class UserData
{
    public string _id;
    public string username;
    public bool estado;
    public int score;

}