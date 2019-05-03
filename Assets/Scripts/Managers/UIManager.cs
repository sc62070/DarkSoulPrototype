using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    [Header("Player")]
    public RectTransform healthBar;
    public RectTransform healthBarFill;

    public RectTransform staminaBar;
    public RectTransform staminaBarFill;

    [Header("Boss")]
    public RectTransform bossHealthBar;
    public RectTransform bossHealthBarFill;

    public GameObject interactionPanel;

    public Character character;
    public Character boss;

    void Start() {
        
    }

    void LateUpdate() {

        if(character != null) {
            healthBarFill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (character.health / character.MaxHealth) * healthBar.rect.width);
            healthBarFill.anchoredPosition = Vector2.zero;

            staminaBarFill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (character.stamina / character.maxStamina) * staminaBar.rect.width);
            staminaBarFill.anchoredPosition = Vector2.zero;
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
