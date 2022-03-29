using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_script : MonoBehaviour
{
    public Slider sliderRef;
    public Toggle spawnPopupUI;
    public Toggle toggleAllUI;
    public Toggle allowSuperGen;
    public Image allPanel;
    public TextMeshProUGUI GenerationCount;
    public TextMeshProUGUI BestFitness;
    public TextMeshProUGUI BestStats;
    public Dropdown combType;
    
   // bool spawnPopupUI = true;
    public void changeSimulationSpeed()
    {
        Time.timeScale = sliderRef.value;
    }


    public void toggleDisplayUI()
    {
        allPanel.gameObject.SetActive(toggleAllUI.isOn);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
