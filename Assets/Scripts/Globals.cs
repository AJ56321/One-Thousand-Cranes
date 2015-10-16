using UnityEngine;
using System.Collections;

public static class Globals
{
    public static float FIXED_SEC_PER_FRAME = 0.01667f;
    public static int NUMBER_OF_RECORDED_FRAMES = 18;
    public static float DISTANCE_FOR_SWIPE = 10f;
    public static Vector3 CAMERA_OFFSET = new Vector3(1f, 2f, -15f);
    public static float CAMERA_SMOOTH_TIME = 0.15f;
    public static float PANDA_MAX_SPD = 10f;
    public static float BIRD_MAX_SPD = 15f;
    //Here
    public static float PANDA_DELTA_SPD = 2f;
    public static float BIRD_NORMAL_SPD = 7.5f;
    public static float BIRD_MIN_SPD = 1f;
    public static float BIRD_DELTA_SPD = 1.5f;
    //To here will not be used, likely
    public static float TAP_TIME_MAX = 0.2f;
    public static float MOVE_DELAY = 0.01f;
    public static float TONGUE_DIST = 7f;
    public static float TONGUE_PULL_SMOOTH_TIME = 0.3f;
    public static float TONGUE_SPRING_STRENGTH = 25f;
    public static float TONGUE_SPRING_DAMPER = 4f;
    public static float PANDA_FORCE_CONSTANT = 20f;
    public static float BIRD_FORCE_CONSTANT = 20f;
    public static float BIRD_DRAG_CONSTANT = 0.2f;
}
