
using SolarFalcon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.Playables;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] bool forceSave;

    [SerializeField]
    private GameData CurrentSave = new GameData();
    private IDataService DataService = new JSONDataService();
    public PlayerStatus CurrentPlayer;
    public CMF.CameraCombinedInput CameraCombinedInput;
    public CMF.TurnTowardTransformDirection TTTD;
    public ParticleSystem PlayerReticle;
    public GameObject DeathMenu;

    public List<EntityStatus> ActiveEntityList;

    public float playerParrySlowTime;

    [Range(0f, 1f)]
    public float playerParrySlowAmount = 0.845f;

    public ParticleSystem[] AmbientParticles;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

    }

    private void Start()
    {
        if (!DeserializeJson()) // There is no save file, create one
        {
            forceSave = true;
        }

        SolarFalcon.PlayerStatus.PDeath += OnPlayerDeath;
        //PlayerStatus.Instance.GetComponent<HypermodeController>().OnHypermodeActive += OnHypermode;
        //PlayerStatus.Instance.GetComponent<HypermodeController>().OnHypermodeOver += OnHypermodeOver;
        PlayerBlock.Instance.OnParry += OnPlayerParry;

        
    }
    public GameData GetCurrentSave()
    {
        GameData copyOfSave = CurrentSave;
        return copyOfSave;
    }

    public void SerializeJson()
    {
        if (DataService.SaveData("/playerSave.json", CurrentSave, false))
        {
            Debug.Log("Progress saved at " + Application.persistentDataPath + "/playerSave.json");
        }

        else
        {
            Debug.LogError("Could not save file! Show someting on the UI about this!");
            //InputField.text = "<color = #ff0000> Error saving data </color>";
        }
    }

    public bool DeserializeJson()
    {
        try
        {
            CurrentSave = DataService.LoadData<GameData>("/playerSave.json", false);
        }

        catch
        {
            Debug.LogError($"Could not load save file at " + Application.persistentDataPath + "/playerSave.json");
            return false;
        }

        return true;
    }

    private void Update()
    {
        if (forceSave)
        {
            SerializeJson();
            forceSave = false;
        }
    }

    private void OnDestroy()
    {
        SolarFalcon.PlayerStatus.PDeath -= OnPlayerDeath;
        PlayerBlock.Instance.OnParry -= OnPlayerParry;
    }

    void OnHypermode()
    {
        StopAllEntities();
        Invoke("ResumeAllEntities", 10f);
    }

    void OnHypermodeOver()
    {
        StopAllEntities();
        Invoke("ResumeAllEntities", 10f);
    }

    void StopAllEntities()
    {
        for (int i = 0; i < ActiveEntityList.Count; i++)
        {
            ActiveEntityList[i].SendMessage("HaltMovement");
        }

        for(int i = 0;i < AmbientParticles.Length; i++)
        {
            var main = AmbientParticles[i].main;
            main.simulationSpeed = 0;
        }
    }

    void ResumeAllEntities()
    {
        for (int i = 0; i < ActiveEntityList.Count; i++)
        {
            ActiveEntityList[i].SendMessage("ResumeMovement");
        }

        for (int i = 0; i < AmbientParticles.Length; i++)
        {
            var main = AmbientParticles[i].main;
            main.simulationSpeed = 1;
        }
    }

    public bool PlayerHasDagger() { return CurrentSave.DaggerUnlocked; }
    public bool PlayerHasScythe() { return CurrentSave.ScytheUnlocked; }
    public bool PlayerHasScepter() { return CurrentSave.ScepterUnlocked; }
    public bool PlayerHasWhip() { return CurrentSave.WhipUnlocked; }

    void OnPlayerParry()
    {
        for (int i = 0; i < ActiveEntityList.Count; i++)
        {
            ActiveEntityList[i].SendMessage("OverrideTimeScale", playerParrySlowAmount);
        }

        for (int i = 0; i < AmbientParticles.Length; i++)
        {
            var main = AmbientParticles[i].main;
            main.simulationSpeed = playerParrySlowAmount;
        }

        Invoke("ResumeAllEntities", playerParrySlowTime);
    }

    void OnPlayerDeath()
    {
        CameraCombinedInput.playerDead = true;
        TTTD.enabled = false;
        PlayerReticle.Stop();
        Invoke("ShowDeathMenu", 5f);
    }

    void ShowDeathMenu()
    {
        DeathMenu.SetActive(true);
    }

    public void RestartScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void HitStun(float time)
    {
        Time.timeScale = 0.1f;
        StartCoroutine("HitStunOver", time);
    }

    IEnumerator HitStunOver(float time)
    {
        yield return new WaitForSeconds(time);
        Time.timeScale = 1f;
    }
}
