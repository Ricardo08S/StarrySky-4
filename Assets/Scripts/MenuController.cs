using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public string SimulationScene;

    public void StartSimulation()
    {
        SceneManager.LoadScene(SimulationScene, LoadSceneMode.Single);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
