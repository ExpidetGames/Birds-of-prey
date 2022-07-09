using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CreateRoomUIManager : MonoBehaviour {

    [SerializeField] private TMP_Dropdown gameModes;
    [SerializeField] private Image mapPreview;

    private int currentlyShownMapIndex = 0;

    private void Start() {
        NetworkedVariables.worldIndex = PrefabOrganizer.Worlds[currentlyShownMapIndex].buildIndex;
        populateDropdown(gameModes);
        gameModes.SetValueWithoutNotify((int)NetworkedVariables.currentGameMode);
        mapPreview.sprite = PrefabOrganizer.Worlds[currentlyShownMapIndex].mapPreview;
    }

    public void quit() {
        NetworkedVariables.scenceToLoad.Add(0);
    }

    public void next() {
        NetworkedVariables.scenceToLoad.Add(7);
    }

    public void changeShownMap(int amount) {
        currentlyShownMapIndex = (currentlyShownMapIndex + Mathf.Abs(PrefabOrganizer.Worlds.Count + amount)) % PrefabOrganizer.Worlds.Count;
        NetworkedVariables.worldIndex = PrefabOrganizer.Worlds[currentlyShownMapIndex].buildIndex;
        mapPreview.sprite = PrefabOrganizer.Worlds[currentlyShownMapIndex].mapPreview;
    }

    public void onGameModeChanged() {
        NetworkedVariables.currentGameMode = (GameModeTypes)gameModes.value;
        Debug.Log($"The selected game Mode is: {NetworkedVariables.currentGameMode}");
    }

    public void populateDropdown(TMP_Dropdown dropdown) {
        List<TMP_Dropdown.OptionData> newOptions = new List<TMP_Dropdown.OptionData>();
        dropdown.options.Clear();
        for(int i = 0; i < Enum.GetNames(typeof(GameModeTypes)).Length; i++) {
            newOptions.Add(new TMP_Dropdown.OptionData(text: Enum.GetName(typeof(GameModeTypes), i)));
        }
        dropdown.AddOptions(newOptions);
    }
}
