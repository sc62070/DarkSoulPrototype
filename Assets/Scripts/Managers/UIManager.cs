using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    [Header("Components")]
    public GameObject mainMenu;
    public GameObject ui;
   
    [Header("Player")]
    public Bar healthBar;
    public Bar staminaBar;
    public Bar poiseBar;

    [Header("Boss")]
    public RectTransform bossHealthBar;
    public RectTransform bossHealthBarFill;

    public GameObject interactionPanel;

    public Character character;
    public Character boss;

    void Awake() {
        mainMenu.SetActive(true);
        ui.SetActive(true);
    }

    void LateUpdate() {

        if(character != null) {
            healthBar.SetValue(character.health / character.MaxHealth);
            staminaBar.SetValue(character.stamina / character.maxStamina);
            poiseBar.SetValue(character.poise / 100f);
        }

        bossHealthBar.gameObject.SetActive(boss != null && boss.health > 0f && boss.target != null && boss.target.gameObject == Unimotion.Player.main.gameObject);
        if (boss != null) {
            bossHealthBarFill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (boss.health / boss.MaxHealth) * bossHealthBar.rect.width);
            bossHealthBarFill.anchoredPosition = Vector2.zero;
        }

        interactionPanel.SetActive(Interactable.active != null);
        if(Interactable.active != null) {
            interactionPanel.GetComponentInChildren<Text>().text = Interactable.active.Description;
        }
        
    }
}
