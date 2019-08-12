using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotbarView : MonoSingleton<HotbarView> {
    public List<Hotkey> hotkeys;

    void Awake() {
        hotkeys = new List<Hotkey>(GetComponentsInChildren<Hotkey>());
    }


}
