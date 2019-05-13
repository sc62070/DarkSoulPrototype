using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    [Header("Sections")]
    public GameObject main;
    public GameObject mainMenu;
    public GameObject ui;
   
    [Header("Player")]
    public Bar healthBar;
    public Bar staminaBar;
    public Bar poiseBar;

    [Header("Boss")]
    public RectTransform bossHealthBar;
    public RectTransform bossHealthBarFill;

    [Header("Configuration")]
    public UIComponentReferences componentReferences;

    public GameObject interactionPanel;

    public Character character;
    public Character boss;

    void Awake() {
        main.SetActive(true);
        mainMenu.SetActive(true);
        ui.SetActive(true);
    }

    void LateUpdate() {

        ui.SetActive(Unimotion.Player.main != null && Dialog.conversationTarget == null);

        if (character != null) {
            healthBar.SetValue(character.health / character.MaxHealth);
            staminaBar.SetValue(character.stamina / character.MaxStamina);
            poiseBar.SetValue(character.poise / character.MaxPoise);
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

    public static void ShowDialog(string text) {
        UIManager manager = FindObjectOfType<UIManager>();
        GameObject o = Instantiate(manager.componentReferences.informationDialog, manager.mainMenu.transform, false);
        o.GetComponent<Dialog>().SetText(text);
    }

    public static void ShowConversationDialog(string text) {
        UIManager manager = FindObjectOfType<UIManager>();
        GameObject o = Instantiate(manager.componentReferences.conversationDialog, manager.mainMenu.transform, false);
        o.GetComponent<Dialog>().SetText(text);
    }
}

[System.Serializable]
public class UIComponentReferences {
    public GameObject informationDialog;
    public GameObject conversationDialog;
    public GameObject registerDialog;
    public GameObject loginDialog;
}
