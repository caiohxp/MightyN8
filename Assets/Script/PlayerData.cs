using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData instance;
    public int totalPoints;
    public int health = 10;

    public enum MathOperation { Sum, Sub, Multi, Div }

    [Header("Ammo & Multiplier")]
    public int universalAmmo = 15; 
    public int currentMultiplier = 1; 

    [Header("Unlocked Operations")]
    public bool unlockedSum = true; 
    public bool unlockedSub = false;
    public bool unlockedMulti = false;
    public bool unlockedDiv = false;

    [Header("Current Active Mode")]
    public MathOperation currentMode = MathOperation.Sum;

    void Awake()
    {
        instance = this;
    }

    void Update() 
    { 
        SwitchWeaponWithKeyboard();
    }

    private void SwitchWeaponWithKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Q))
        {
            if (currentMode == MathOperation.Sum && unlockedSub) currentMode = MathOperation.Sub;
            else if (currentMode == MathOperation.Sub && unlockedMulti) currentMode = MathOperation.Multi;
            else if (currentMode == MathOperation.Multi && unlockedDiv) currentMode = MathOperation.Div;
            else currentMode = MathOperation.Sum;
        }
    }

    // Funções para os botões OnClick() na Unity
    public void SelectSum() { if (unlockedSum) currentMode = MathOperation.Sum; }
    public void SelectSub() { if (unlockedSub) currentMode = MathOperation.Sub; }
    public void SelectMulti() { if (unlockedMulti) currentMode = MathOperation.Multi; }
    public void SelectDiv() { if (unlockedDiv) currentMode = MathOperation.Div; }
}