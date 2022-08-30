using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class HttpManager : MonoBehaviour
{

    [SerializeField]
    private string URL;

    private string Token;
    private string Username;
    private int maxScore;

    [SerializeField] Text name;
    [SerializeField] Text scoreT;

    void Awake()
    {
        GameObject[] httpManager = GameObject.FindGameObjectsWithTag("httpManager");

        if (httpManager.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        
        Token = PlayerPrefs.GetString("token");
        Username = PlayerPrefs.GetString("username");
        maxScore = PlayerPrefs.GetInt("maxScore");

        Debug.Log("TOKEN:" + Token);

        StartCoroutine(ObtenerPerfil());

        Debug.Log(Username);
    }

    public static void Salir()
    {
        PlayerPrefs.SetString("token", null);
        PlayerPrefs.SetString("username", null);
        PlayerPrefs.SetInt("maxScore", 0);
    }

    public void ClickGetScores()
    {
        StartCoroutine(GetScores());
    }

    private static string GetInputData()
    {
        AuthData data = new AuthData();

        data.username = GameObject.Find("Usuario").GetComponent<InputField>().text;
        data.password = GameObject.Find("Contraseña").GetComponent<InputField>().text;

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

    public void Record()
    {
        int scoreActual = Score.score;

        if (scoreActual > maxScore)
        {
            UserData data = new UserData();

            data.username = Username;
            data.score = scoreActual;

            maxScore = scoreActual;
            PlayerPrefs.SetInt("maxScore", scoreActual);

            string postData = JsonUtility.ToJson(data);
            StartCoroutine(SetScore(postData));
        }
        
        
    }

    IEnumerator GetScores()
    {
        string url = URL + "/api/usuarios?limit=5&sort=true";
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("content-type", "application/json");
        Debug.Log(Token);
        www.SetRequestHeader("x-token", Token);

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200) {
            //Debug.Log(www.downloadHandler.text);
            Scores resData = JsonConvert.DeserializeObject<Scores>(www.downloadHandler.text);

            Debug.Log(www.downloadHandler.text);

            Text TablaNombre = GameObject.Find("Name").GetComponent<Text>();
            Text TablaPuntos = GameObject.Find("Score").GetComponent<Text>();

            foreach (ScoreData score in resData.usuarios)
            {
                Debug.Log(score.username + " | " + score.score);
                TablaNombre.text += score.username + "\n";
                TablaPuntos.text += score.score + "\n";
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
        UnityWebRequest www = UnityWebRequest.Put(url, postData); //mandando los datos a la nube del profesor
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

            Debug.Log("Registrado" + resData.usuario.username + ", id:" + resData.usuario._id);
            
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
            Debug.Log(www.downloadHandler.text);
            AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);

           Debug.Log("Autenticado" + resData.usuario.username + ", id:" + resData.usuario._id);
           Debug.Log("TOKEN: " + resData.token);

           PlayerPrefs.SetString("token", resData.token);
           PlayerPrefs.SetString("username", resData.usuario.username);

           Token = resData.token;
           Username = resData.usuario.username;

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
        string url = URL + "/api/usuarios/" + Username;
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

            Debug.Log("Token valido" + resData.usuario.username + ", id:" + resData.usuario._id + " y su score es: " + resData.usuario.score);
            SceneManager.LoadScene("SampleScene");
            

        }
        else
        {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.text);
        }
    }

    IEnumerator SetScore(string postData)
    {
        Debug.Log("Patch score:");

        string url = URL + "/api/usuarios";

        UnityWebRequest www = UnityWebRequest.Put(url, postData);
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
    public string _id { get; set; }
    public string username { get; set; }
    public string password { get; set; }
    public bool estado { get; set; }
    public int score { get; set; }
}


[System.Serializable]
public class Scores
{
    public List<ScoreData> usuarios;
    
}

[System.Serializable]
public class AuthData
{
    public string username;
    public string password;
    public UserData usuario;
    public string token;
}

[System.Serializable]
public class UserData
{
    public string _id;
    public string username;
    public bool estado;
    public int score;
}