using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData instance;
    public int totalPoints;
    public int health = 10;

    // A ordem do enum é a ordem em que a tecla E/Q percorre os modos
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

    public bool IsUnlocked(MathOperation operation)
    {
        switch (operation)
        {
            case MathOperation.Sum: return unlockedSum;
            case MathOperation.Sub: return unlockedSub;
            case MathOperation.Multi: return unlockedMulti;
            case MathOperation.Div: return unlockedDiv;
        }
        return false;
    }

    // Vai para a próxima operação desbloqueada, pulando as bloqueadas
    // e voltando ao início quando chega no fim
    private void SwitchWeaponWithKeyboard()
    {
        if (!Input.GetKeyDown(KeyCode.E) && !Input.GetKeyDown(KeyCode.Q)) return;

        int total = System.Enum.GetValues(typeof(MathOperation)).Length;

        for (int i = 1; i <= total; i++)
        {
            MathOperation candidato = (MathOperation)(((int)currentMode + i) % total);
            if (IsUnlocked(candidato))
            {
                currentMode = candidato;
                return;
            }
        }
    }

    // Funções para os botões OnClick() na Unity
    public void SelectSum() { if (unlockedSum) currentMode = MathOperation.Sum; }
    public void SelectSub() { if (unlockedSub) currentMode = MathOperation.Sub; }
    public void SelectMulti() { if (unlockedMulti) currentMode = MathOperation.Multi; }
    public void SelectDiv() { if (unlockedDiv) currentMode = MathOperation.Div; }
}