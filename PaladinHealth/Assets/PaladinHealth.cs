using UnityEngine;

public class PaladinHealth : MonoBehaviour
{
    // Поля - состояние здоровья
    public string PlayerName = "Паладин";
    public int CurrentHealth = 50;
    public int MaxHealth = 100;

    // Метод получения текущего состояния
    public string GetHealthStatus()
    {
        return PlayerName + " имеет " + CurrentHealth + "/" + MaxHealth + " HP ";
    }
    // Метод восстановления здоровья
    public void Heal(int amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth)
        {
             CurrentHealth = MaxHealth;
        }
        print("Здоровье восстановлено! Текущее: " + CurrentHealth);
    }

    // Метод получения урона
    public void TakeDamage(int amount)
    {
        if (CurrentHealth > 0)
        {
            CurrentHealth -= amount;
            print(PlayerName + " получил урон! Осталось: " + CurrentHealth);
        }
    }
    
    void Start()
    {
        print(GetHealthStatus());
        TakeDamage(20);
    }

}
