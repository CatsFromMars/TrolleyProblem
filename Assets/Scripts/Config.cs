using UnityEngine;
using System.Collections;

[System.Serializable]
public enum RGroup {
    LeverControl, // lever control group
    NonHaptic,    // non-haptic experimental group
    Haptic        // haptic experimental group
};

public class Config : MonoBehaviour {
    public RGroup group = RGroup.LeverControl;
    public bool debug = false;

    public static RGroup Group {
        get {
            return config.group;
        }
    }
    public static bool Debug {
        get {
            return config.debug;
        }
    }

    private static Config config;

    void Start() {
        config = this;
    }
}
