namespace Dimworld.Core;


/// <summary>
/// This class contains all the input actions used in the game.
/// It is used to easily access the input actions and to avoid hardcoding strings.
/// The input actions are defined in the Input Map in the Godot project settings.
/// </summary>
public static class InputActions
{

    // DEBUG

    public static string TOGGLE_TIMESCALE = "toggle_timescale";

    public static string TOGGLE_DEVELOPER_MENU = "toggle_developer_menu";


    // INTERACTION AND INVENTORY

    public static string LEFT_MOUSE = "lmb";

    public static string RIGHT_MOUSE = "rmb";

    public static string SHIFT_LMB = "shift_lmb";

    public static string TOGGLE_INVENTORY = "toggle_inventory";

    public static string INTERACT = "interact";

    public static string UI_CANCEL = "ui_cancel";

    public static string UI_UP = "ui_up";

    public static string HOTBAR_SLOT_NEXT = "hotbar_slot_next";

    public static string HOTBAR_SLOT_PREV = "hotbar_slot_prev";

    public static string HOTBAR_SLOT_0 = "hotbar_slot_0";

    public static string HOTBAR_SLOT_1 = "hotbar_slot_1";

    public static string HOTBAR_SLOT_2 = "hotbar_slot_2";

    public static string HOTBAR_SLOT_3 = "hotbar_slot_3";

    public static string HOTBAR_SLOT_4 = "hotbar_slot_4";

    public static string ACTION_RELOAD = "action_reload";


    // MOVEMENT

    public static string MOVE_UP = "move_up";

    public static string MOVE_DOWN = "move_down";

    public static string MOVE_LEFT = "move_left";

    public static string MOVE_RIGHT = "move_right";

    public static string ACTION_SPRINT = "action_sprint";
}