namespace UnityEngine;

public static class AssetLookup
{
	public static class Tags
	{
		public const string UNTAGGED = "Untagged";

		public const string RESPAWN = "Respawn";

		public const string FINISH = "Finish";

		public const string EDITOR_ONLY = "EditorOnly";

		public const string MAIN_CAMERA = "MainCamera";

		public const string PLAYER = "Player";

		public const string GAME_CONTROLLER = "GameController";

		public const string UI_VISUAL_CONTROLS = "UIVisualControls";

		public const string UI_CAMERA = "UICamera";

		public const string IGNORE_DO_F = "IgnoreDoF";

		public const string NPC = "NPC";

		public const string METAL = "Metal";

		public const string LISTENER = "Listener";

		public const string ENEMY = "Enemy";
	}

	public static class Layers
	{
		public const string DEFAULT = "Default";

		public const string TRANSPARENT_FX = "TransparentFX";

		public const string IGNORE_RAYCAST = "Ignore Raycast";

		public const string WATER = "Water";

		public const string UI = "UI";

		public const string PLAYER = "Player";

		public const string NAV_MESH = "NavMesh";

		public const string EVENT_TRIGGER = "EventTrigger";

		public const string INVISIBLE_COLLIDER = "InvisibleCollider";

		public const string AI = "AI";

		public const string AUDIO = "Audio";

		public const string FIRST_PERSON = "FirstPerson";

		public const string RAGDOLL = "Ragdoll";

		public const string SECTION_ZONE = "SectionZone";

		public const string BACKSTAGE = "Backstage";

		public const string NO_NAV_MESH = "NoNavMesh";

		public const string IGNORE_DOF = "IgnoreDOF";

		public const string IGNORE_INVISIBLE_COLLIDER = "IgnoreInvisibleCollider";

		public const string INTERACT_INVISIBLE_COLLIDER = "InteractInvisibleCollider";

		public const string IGNORE_PLAYER = "IgnorePlayer";

		public const string INTERACT_DEFAULT = "InteractDefault";

		public const string IGNORE_AI = "IgnoreAI";

		public const string SKYBOX = "Skybox";

		public const string UI_MODEL = "UIModel";

		public const string MIRROR = "Mirror";
	}

	public static class SortingLayers
	{
		public const string DEFAULT = "Default";
	}

	public static class InputAxes
	{
		public const string JOYSTICK_1ANALOG_0 = "joystick 1 analog 0";

		public const string JOYSTICK_1ANALOG_1 = "joystick 1 analog 1";

		public const string JOYSTICK_1ANALOG_2 = "joystick 1 analog 2";

		public const string JOYSTICK_1ANALOG_3 = "joystick 1 analog 3";

		public const string JOYSTICK_1ANALOG_4 = "joystick 1 analog 4";

		public const string JOYSTICK_1ANALOG_5 = "joystick 1 analog 5";

		public const string JOYSTICK_1ANALOG_6 = "joystick 1 analog 6";

		public const string JOYSTICK_1ANALOG_7 = "joystick 1 analog 7";

		public const string JOYSTICK_1ANALOG_8 = "joystick 1 analog 8";

		public const string JOYSTICK_1ANALOG_9 = "joystick 1 analog 9";

		public const string JOYSTICK_1ANALOG_10 = "joystick 1 analog 10";

		public const string JOYSTICK_1ANALOG_11 = "joystick 1 analog 11";

		public const string JOYSTICK_1ANALOG_12 = "joystick 1 analog 12";

		public const string JOYSTICK_1ANALOG_13 = "joystick 1 analog 13";

		public const string JOYSTICK_1ANALOG_14 = "joystick 1 analog 14";

		public const string JOYSTICK_1ANALOG_15 = "joystick 1 analog 15";

		public const string JOYSTICK_1ANALOG_16 = "joystick 1 analog 16";

		public const string JOYSTICK_1ANALOG_17 = "joystick 1 analog 17";

		public const string JOYSTICK_1ANALOG_18 = "joystick 1 analog 18";

		public const string JOYSTICK_1ANALOG_19 = "joystick 1 analog 19";

		public const string JOYSTICK_2ANALOG_0 = "joystick 2 analog 0";

		public const string JOYSTICK_2ANALOG_1 = "joystick 2 analog 1";

		public const string JOYSTICK_2ANALOG_2 = "joystick 2 analog 2";

		public const string JOYSTICK_2ANALOG_3 = "joystick 2 analog 3";

		public const string JOYSTICK_2ANALOG_4 = "joystick 2 analog 4";

		public const string JOYSTICK_2ANALOG_5 = "joystick 2 analog 5";

		public const string JOYSTICK_2ANALOG_6 = "joystick 2 analog 6";

		public const string JOYSTICK_2ANALOG_7 = "joystick 2 analog 7";

		public const string JOYSTICK_2ANALOG_8 = "joystick 2 analog 8";

		public const string JOYSTICK_2ANALOG_9 = "joystick 2 analog 9";

		public const string JOYSTICK_2ANALOG_10 = "joystick 2 analog 10";

		public const string JOYSTICK_2ANALOG_11 = "joystick 2 analog 11";

		public const string JOYSTICK_2ANALOG_12 = "joystick 2 analog 12";

		public const string JOYSTICK_2ANALOG_13 = "joystick 2 analog 13";

		public const string JOYSTICK_2ANALOG_14 = "joystick 2 analog 14";

		public const string JOYSTICK_2ANALOG_15 = "joystick 2 analog 15";

		public const string JOYSTICK_2ANALOG_16 = "joystick 2 analog 16";

		public const string JOYSTICK_2ANALOG_17 = "joystick 2 analog 17";

		public const string JOYSTICK_2ANALOG_18 = "joystick 2 analog 18";

		public const string JOYSTICK_2ANALOG_19 = "joystick 2 analog 19";

		public const string JOYSTICK_3ANALOG_0 = "joystick 3 analog 0";

		public const string JOYSTICK_3ANALOG_1 = "joystick 3 analog 1";

		public const string JOYSTICK_3ANALOG_2 = "joystick 3 analog 2";

		public const string JOYSTICK_3ANALOG_3 = "joystick 3 analog 3";

		public const string JOYSTICK_3ANALOG_4 = "joystick 3 analog 4";

		public const string JOYSTICK_3ANALOG_5 = "joystick 3 analog 5";

		public const string JOYSTICK_3ANALOG_6 = "joystick 3 analog 6";

		public const string JOYSTICK_3ANALOG_7 = "joystick 3 analog 7";

		public const string JOYSTICK_3ANALOG_8 = "joystick 3 analog 8";

		public const string JOYSTICK_3ANALOG_9 = "joystick 3 analog 9";

		public const string JOYSTICK_3ANALOG_10 = "joystick 3 analog 10";

		public const string JOYSTICK_3ANALOG_11 = "joystick 3 analog 11";

		public const string JOYSTICK_3ANALOG_12 = "joystick 3 analog 12";

		public const string JOYSTICK_3ANALOG_13 = "joystick 3 analog 13";

		public const string JOYSTICK_3ANALOG_14 = "joystick 3 analog 14";

		public const string JOYSTICK_3ANALOG_15 = "joystick 3 analog 15";

		public const string JOYSTICK_3ANALOG_16 = "joystick 3 analog 16";

		public const string JOYSTICK_3ANALOG_17 = "joystick 3 analog 17";

		public const string JOYSTICK_3ANALOG_18 = "joystick 3 analog 18";

		public const string JOYSTICK_3ANALOG_19 = "joystick 3 analog 19";

		public const string JOYSTICK_4ANALOG_0 = "joystick 4 analog 0";

		public const string JOYSTICK_4ANALOG_1 = "joystick 4 analog 1";

		public const string JOYSTICK_4ANALOG_2 = "joystick 4 analog 2";

		public const string JOYSTICK_4ANALOG_3 = "joystick 4 analog 3";

		public const string JOYSTICK_4ANALOG_4 = "joystick 4 analog 4";

		public const string JOYSTICK_4ANALOG_5 = "joystick 4 analog 5";

		public const string JOYSTICK_4ANALOG_6 = "joystick 4 analog 6";

		public const string JOYSTICK_4ANALOG_7 = "joystick 4 analog 7";

		public const string JOYSTICK_4ANALOG_8 = "joystick 4 analog 8";

		public const string JOYSTICK_4ANALOG_9 = "joystick 4 analog 9";

		public const string JOYSTICK_4ANALOG_10 = "joystick 4 analog 10";

		public const string JOYSTICK_4ANALOG_11 = "joystick 4 analog 11";

		public const string JOYSTICK_4ANALOG_12 = "joystick 4 analog 12";

		public const string JOYSTICK_4ANALOG_13 = "joystick 4 analog 13";

		public const string JOYSTICK_4ANALOG_14 = "joystick 4 analog 14";

		public const string JOYSTICK_4ANALOG_15 = "joystick 4 analog 15";

		public const string JOYSTICK_4ANALOG_16 = "joystick 4 analog 16";

		public const string JOYSTICK_4ANALOG_17 = "joystick 4 analog 17";

		public const string JOYSTICK_4ANALOG_18 = "joystick 4 analog 18";

		public const string JOYSTICK_4ANALOG_19 = "joystick 4 analog 19";

		public const string JOYSTICK_5ANALOG_0 = "joystick 5 analog 0";

		public const string JOYSTICK_5ANALOG_1 = "joystick 5 analog 1";

		public const string JOYSTICK_5ANALOG_2 = "joystick 5 analog 2";

		public const string JOYSTICK_5ANALOG_3 = "joystick 5 analog 3";

		public const string JOYSTICK_5ANALOG_4 = "joystick 5 analog 4";

		public const string JOYSTICK_5ANALOG_5 = "joystick 5 analog 5";

		public const string JOYSTICK_5ANALOG_6 = "joystick 5 analog 6";

		public const string JOYSTICK_5ANALOG_7 = "joystick 5 analog 7";

		public const string JOYSTICK_5ANALOG_8 = "joystick 5 analog 8";

		public const string JOYSTICK_5ANALOG_9 = "joystick 5 analog 9";

		public const string JOYSTICK_5ANALOG_10 = "joystick 5 analog 10";

		public const string JOYSTICK_5ANALOG_11 = "joystick 5 analog 11";

		public const string JOYSTICK_5ANALOG_12 = "joystick 5 analog 12";

		public const string JOYSTICK_5ANALOG_13 = "joystick 5 analog 13";

		public const string JOYSTICK_5ANALOG_14 = "joystick 5 analog 14";

		public const string JOYSTICK_5ANALOG_15 = "joystick 5 analog 15";

		public const string JOYSTICK_5ANALOG_16 = "joystick 5 analog 16";

		public const string JOYSTICK_5ANALOG_17 = "joystick 5 analog 17";

		public const string JOYSTICK_5ANALOG_18 = "joystick 5 analog 18";

		public const string JOYSTICK_5ANALOG_19 = "joystick 5 analog 19";

		public const string JOYSTICK_6ANALOG_0 = "joystick 6 analog 0";

		public const string JOYSTICK_6ANALOG_1 = "joystick 6 analog 1";

		public const string JOYSTICK_6ANALOG_2 = "joystick 6 analog 2";

		public const string JOYSTICK_6ANALOG_3 = "joystick 6 analog 3";

		public const string JOYSTICK_6ANALOG_4 = "joystick 6 analog 4";

		public const string JOYSTICK_6ANALOG_5 = "joystick 6 analog 5";

		public const string JOYSTICK_6ANALOG_6 = "joystick 6 analog 6";

		public const string JOYSTICK_6ANALOG_7 = "joystick 6 analog 7";

		public const string JOYSTICK_6ANALOG_8 = "joystick 6 analog 8";

		public const string JOYSTICK_6ANALOG_9 = "joystick 6 analog 9";

		public const string JOYSTICK_6ANALOG_10 = "joystick 6 analog 10";

		public const string JOYSTICK_6ANALOG_11 = "joystick 6 analog 11";

		public const string JOYSTICK_6ANALOG_12 = "joystick 6 analog 12";

		public const string JOYSTICK_6ANALOG_13 = "joystick 6 analog 13";

		public const string JOYSTICK_6ANALOG_14 = "joystick 6 analog 14";

		public const string JOYSTICK_6ANALOG_15 = "joystick 6 analog 15";

		public const string JOYSTICK_6ANALOG_16 = "joystick 6 analog 16";

		public const string JOYSTICK_6ANALOG_17 = "joystick 6 analog 17";

		public const string JOYSTICK_6ANALOG_18 = "joystick 6 analog 18";

		public const string JOYSTICK_6ANALOG_19 = "joystick 6 analog 19";

		public const string JOYSTICK_7ANALOG_0 = "joystick 7 analog 0";

		public const string JOYSTICK_7ANALOG_1 = "joystick 7 analog 1";

		public const string JOYSTICK_7ANALOG_2 = "joystick 7 analog 2";

		public const string JOYSTICK_7ANALOG_3 = "joystick 7 analog 3";

		public const string JOYSTICK_7ANALOG_4 = "joystick 7 analog 4";

		public const string JOYSTICK_7ANALOG_5 = "joystick 7 analog 5";

		public const string JOYSTICK_7ANALOG_6 = "joystick 7 analog 6";

		public const string JOYSTICK_7ANALOG_7 = "joystick 7 analog 7";

		public const string JOYSTICK_7ANALOG_8 = "joystick 7 analog 8";

		public const string JOYSTICK_7ANALOG_9 = "joystick 7 analog 9";

		public const string JOYSTICK_7ANALOG_10 = "joystick 7 analog 10";

		public const string JOYSTICK_7ANALOG_11 = "joystick 7 analog 11";

		public const string JOYSTICK_7ANALOG_12 = "joystick 7 analog 12";

		public const string JOYSTICK_7ANALOG_13 = "joystick 7 analog 13";

		public const string JOYSTICK_7ANALOG_14 = "joystick 7 analog 14";

		public const string JOYSTICK_7ANALOG_15 = "joystick 7 analog 15";

		public const string JOYSTICK_7ANALOG_16 = "joystick 7 analog 16";

		public const string JOYSTICK_7ANALOG_17 = "joystick 7 analog 17";

		public const string JOYSTICK_7ANALOG_18 = "joystick 7 analog 18";

		public const string JOYSTICK_7ANALOG_19 = "joystick 7 analog 19";

		public const string JOYSTICK_8ANALOG_0 = "joystick 8 analog 0";

		public const string JOYSTICK_8ANALOG_1 = "joystick 8 analog 1";

		public const string JOYSTICK_8ANALOG_2 = "joystick 8 analog 2";

		public const string JOYSTICK_8ANALOG_3 = "joystick 8 analog 3";

		public const string JOYSTICK_8ANALOG_4 = "joystick 8 analog 4";

		public const string JOYSTICK_8ANALOG_5 = "joystick 8 analog 5";

		public const string JOYSTICK_8ANALOG_6 = "joystick 8 analog 6";

		public const string JOYSTICK_8ANALOG_7 = "joystick 8 analog 7";

		public const string JOYSTICK_8ANALOG_8 = "joystick 8 analog 8";

		public const string JOYSTICK_8ANALOG_9 = "joystick 8 analog 9";

		public const string JOYSTICK_8ANALOG_10 = "joystick 8 analog 10";

		public const string JOYSTICK_8ANALOG_11 = "joystick 8 analog 11";

		public const string JOYSTICK_8ANALOG_12 = "joystick 8 analog 12";

		public const string JOYSTICK_8ANALOG_13 = "joystick 8 analog 13";

		public const string JOYSTICK_8ANALOG_14 = "joystick 8 analog 14";

		public const string JOYSTICK_8ANALOG_15 = "joystick 8 analog 15";

		public const string JOYSTICK_8ANALOG_16 = "joystick 8 analog 16";

		public const string JOYSTICK_8ANALOG_17 = "joystick 8 analog 17";

		public const string JOYSTICK_8ANALOG_18 = "joystick 8 analog 18";

		public const string JOYSTICK_8ANALOG_19 = "joystick 8 analog 19";

		public const string JOYSTICK_9ANALOG_0 = "joystick 9 analog 0";

		public const string JOYSTICK_9ANALOG_1 = "joystick 9 analog 1";

		public const string JOYSTICK_9ANALOG_2 = "joystick 9 analog 2";

		public const string JOYSTICK_9ANALOG_3 = "joystick 9 analog 3";

		public const string JOYSTICK_9ANALOG_4 = "joystick 9 analog 4";

		public const string JOYSTICK_9ANALOG_5 = "joystick 9 analog 5";

		public const string JOYSTICK_9ANALOG_6 = "joystick 9 analog 6";

		public const string JOYSTICK_9ANALOG_7 = "joystick 9 analog 7";

		public const string JOYSTICK_9ANALOG_8 = "joystick 9 analog 8";

		public const string JOYSTICK_9ANALOG_9 = "joystick 9 analog 9";

		public const string JOYSTICK_9ANALOG_10 = "joystick 9 analog 10";

		public const string JOYSTICK_9ANALOG_11 = "joystick 9 analog 11";

		public const string JOYSTICK_9ANALOG_12 = "joystick 9 analog 12";

		public const string JOYSTICK_9ANALOG_13 = "joystick 9 analog 13";

		public const string JOYSTICK_9ANALOG_14 = "joystick 9 analog 14";

		public const string JOYSTICK_9ANALOG_15 = "joystick 9 analog 15";

		public const string JOYSTICK_9ANALOG_16 = "joystick 9 analog 16";

		public const string JOYSTICK_9ANALOG_17 = "joystick 9 analog 17";

		public const string JOYSTICK_9ANALOG_18 = "joystick 9 analog 18";

		public const string JOYSTICK_9ANALOG_19 = "joystick 9 analog 19";

		public const string JOYSTICK_10ANALOG_0 = "joystick 10 analog 0";

		public const string JOYSTICK_10ANALOG_1 = "joystick 10 analog 1";

		public const string JOYSTICK_10ANALOG_2 = "joystick 10 analog 2";

		public const string JOYSTICK_10ANALOG_3 = "joystick 10 analog 3";

		public const string JOYSTICK_10ANALOG_4 = "joystick 10 analog 4";

		public const string JOYSTICK_10ANALOG_5 = "joystick 10 analog 5";

		public const string JOYSTICK_10ANALOG_6 = "joystick 10 analog 6";

		public const string JOYSTICK_10ANALOG_7 = "joystick 10 analog 7";

		public const string JOYSTICK_10ANALOG_8 = "joystick 10 analog 8";

		public const string JOYSTICK_10ANALOG_9 = "joystick 10 analog 9";

		public const string JOYSTICK_10ANALOG_10 = "joystick 10 analog 10";

		public const string JOYSTICK_10ANALOG_11 = "joystick 10 analog 11";

		public const string JOYSTICK_10ANALOG_12 = "joystick 10 analog 12";

		public const string JOYSTICK_10ANALOG_13 = "joystick 10 analog 13";

		public const string JOYSTICK_10ANALOG_14 = "joystick 10 analog 14";

		public const string JOYSTICK_10ANALOG_15 = "joystick 10 analog 15";

		public const string JOYSTICK_10ANALOG_16 = "joystick 10 analog 16";

		public const string JOYSTICK_10ANALOG_17 = "joystick 10 analog 17";

		public const string JOYSTICK_10ANALOG_18 = "joystick 10 analog 18";

		public const string JOYSTICK_10ANALOG_19 = "joystick 10 analog 19";

		public const string MOUSEX = "mouse x";

		public const string MOUSEY = "mouse y";

		public const string MOUSEZ = "mouse z";

		public const string HORIZONTAL = "Horizontal";

		public const string VERTICAL = "Vertical";

		public const string JUMP = "Jump";

		public const string MOUSE_X = "Mouse X";

		public const string MOUSE_Y = "Mouse Y";

		public const string SUBMIT = "Submit";

		public const string CANCEL = "Cancel";

		public const string MOUSE_SCROLL_WHEEL = "Mouse ScrollWheel";

		public const string XRI_LEFT_PRIMARY_2_D_AXIS_VERTICAL = "XRI_Left_Primary2DAxis_Vertical";

		public const string XRI_LEFT_PRIMARY_2_D_AXIS_HORIZONTAL = "XRI_Left_Primary2DAxis_Horizontal";

		public const string XRI_LEFT_SECONDARY_2_D_AXIS_VERTICAL = "XRI_Left_Secondary2DAxis_Vertical";

		public const string XRI_LEFT_SECONDARY_2_D_AXIS_HORIZONTAL = "XRI_Left_Secondary2DAxis_Horizontal";

		public const string XRI_LEFT_TRIGGER = "XRI_Left_Trigger";

		public const string XRI_LEFT_GRIP = "XRI_Left_Grip";

		public const string XRI_LEFT_INDEX_TOUCH = "XRI_Left_IndexTouch";

		public const string XRI_LEFT_THUMB_TOUCH = "XRI_Left_ThumbTouch";

		public const string XRI_LEFT_PRIMARY_BUTTON = "XRI_Left_PrimaryButton";

		public const string XRI_LEFT_SECONDARY_BUTTON = "XRI_Left_SecondaryButton";

		public const string XRI_LEFT_PRIMARY_TOUCH = "XRI_Left_PrimaryTouch";

		public const string XRI_LEFT_SECONDARY_TOUCH = "XRI_Left_SecondaryTouch";

		public const string XRI_LEFT_GRIP_BUTTON = "XRI_Left_GripButton";

		public const string XRI_LEFT_TRIGGER_BUTTON = "XRI_Left_TriggerButton";

		public const string XRI_LEFT_MENU_BUTTON = "XRI_Left_MenuButton";

		public const string XRI_LEFT_PRIMARY_2_D_AXIS_CLICK = "XRI_Left_Primary2DAxisClick";

		public const string XRI_LEFT_PRIMARY_2_D_AXIS_TOUCH = "XRI_Left_Primary2DAxisTouch";

		public const string XRI_LEFT_THUMBREST = "XRI_Left_Thumbrest";

		public const string XRI_RIGHT_PRIMARY_2_D_AXIS_VERTICAL = "XRI_Right_Primary2DAxis_Vertical";

		public const string XRI_RIGHT_PRIMARY_2_D_AXIS_HORIZONTAL = "XRI_Right_Primary2DAxis_Horizontal";

		public const string XRI_RIGHT_SECONDARY_2_D_AXIS_VERTICAL = "XRI_Right_Secondary2DAxis_Vertical";

		public const string XRI_RIGHT_SECONDARY_2_D_AXIS_HORIZONTAL = "XRI_Right_Secondary2DAxis_Horizontal";

		public const string XRI_RIGHT_TRIGGER = "XRI_Right_Trigger";

		public const string XRI_RIGHT_GRIP = "XRI_Right_Grip";

		public const string XRI_RIGHT_INDEX_TOUCH = "XRI_Right_IndexTouch";

		public const string XRI_RIGHT_THUMB_TOUCH = "XRI_Right_ThumbTouch";

		public const string XRI_RIGHT_PRIMARY_BUTTON = "XRI_Right_PrimaryButton";

		public const string XRI_RIGHT_SECONDARY_BUTTON = "XRI_Right_SecondaryButton";

		public const string XRI_RIGHT_PRIMARY_TOUCH = "XRI_Right_PrimaryTouch";

		public const string XRI_RIGHT_SECONDARY_TOUCH = "XRI_Right_SecondaryTouch";

		public const string XRI_RIGHT_GRIP_BUTTON = "XRI_Right_GripButton";

		public const string XRI_RIGHT_TRIGGER_BUTTON = "XRI_Right_TriggerButton";

		public const string XRI_RIGHT_MENU_BUTTON = "XRI_Right_MenuButton";

		public const string XRI_RIGHT_PRIMARY_2_D_AXIS_CLICK = "XRI_Right_Primary2DAxisClick";

		public const string XRI_RIGHT_PRIMARY_2_D_AXIS_TOUCH = "XRI_Right_Primary2DAxisTouch";

		public const string XRI_RIGHT_THUMBREST = "XRI_Right_Thumbrest";

		public const string XRI_COMBINED_TRIGGER = "XRI_Combined_Trigger";

		public const string XRID_PAD_VERTICAL = "XRI_DPad_Vertical";

		public const string XRID_PAD_HORIZONTAL = "XRI_DPad_Horizontal";

		public const string ENABLE_DEBUG_BUTTON_1 = "Enable Debug Button 1";

		public const string ENABLE_DEBUG_BUTTON_2 = "Enable Debug Button 2";

		public const string DEBUG_RESET = "Debug Reset";

		public const string DEBUG_NEXT = "Debug Next";

		public const string DEBUG_PREVIOUS = "Debug Previous";

		public const string DEBUG_VALIDATE = "Debug Validate";

		public const string DEBUG_PERSISTENT = "Debug Persistent";

		public const string DEBUG_MULTIPLIER = "Debug Multiplier";

		public const string DEBUG_HORIZONTAL = "Debug Horizontal";

		public const string DEBUG_VERTICAL = "Debug Vertical";
	}

	public static class Prefabs
	{
		public const string GAME_MENU_AUDIO_CONTROLLER_BEAST = "GameMenuAudioController_Beast";

		public const string GAME_MENU_AUDIO_CONTROLLER_INK = "GameMenuAudioController_Ink";

		public const string GAME_MENU_AUDIO_CONTROLLER_REAL = "GameMenuAudioController_Real";

		public const string S13_AUDIO_MANAGER = "S13AudioManager";

		public const string COLLECTABLE_BETTY_BOX = "Collectables/Collectable_BettyBox";

		public const string COLLECTABLE_PHOTO_MODE = "Collectables/Collectable_PhotoMode";

		public const string COLLECTABLE_RUBBER_BALL = "Collectables/Collectable_RubberBall";

		public const string GAME_CONTROLLER = "Controllers/GameController";

		public const string INK_DEMON_MANAGER = "Controllers/InkDemonManager";

		public const string EFFECTS_AREA_INK = "Effects/Effects_Area_Ink";

		public const string EFFECTS_BARREL_HIT = "Effects/Effects_Barrel_Hit";

		public const string EFFECTS_BENDY_AREA_FX = "Effects/Effects_Bendy_AreaFX";

		public const string EFFECTS_DUMMY_DEATH = "Effects/Effects_Dummy_Death";

		public const string EFFECTS_HIT_INK = "Effects/Effects_Hit_Ink";

		public const string EFFECTS_HIT_INK_COLOR = "Effects/Effects_Hit_Ink_Color";

		public const string EFFECTS_HIT_SMOKE = "Effects/Effects_Hit_Smoke";

		public const string EFFECTS_INK_EXPLOSION = "Effects/Effects_InkExplosion";

		public const string EFFECTS_INK_MESH_DRIP = "Effects/Effects_InkMeshDrip";

		public const string EFFECTS_LOOTED_ENEMY = "Effects/Effects_Looted_Enemy";

		public const string EFFECTS_LOOTED_ENEMY_COLOR = "Effects/Effects_Looted_Enemy_Color";

		public const string FOOD_BACON_SOUP = "Foods/Food_BaconSoup";

		public const string FOOD_BENDY_BAR = "Foods/Food_BendyBar";

		public const string FOOD_CHIPS = "Foods/Food_Chips";

		public const string FOOD_CRACKERS = "Foods/Food_Crackers";

		public const string FOOD_CRACKERS_OPENED = "Foods/Food_Crackers_Opened";

		public const string FOOD_DONUT = "Foods/Food_Donut";

		public const string FOOD_MEAT = "Foods/Food_Meat";

		public const string FOOD_NUTS = "Foods/Food_Nuts";

		public const string FOOD_SANDWHICH = "Foods/Food_Sandwhich";

		public const string GENT_BATTERY_DEFAULT = "GentBatteries/GentBattery_Default";

		public const string GENT_BATTERY_CASING_DEFAULT = "GentBatteryCasing/GentBatteryCasing_Default";

		public const string GENT_CARD_DEFAULT = "GentCard/GentCard_Default";

		public const string GENT_PARTS_DEFAULT = "GentParts/GentParts_Default";

		public const string GENT_PARTS_OPEN = "GentParts/GentParts_Open";

		public const string GENT_TOOLKIT_DEFAULT = "GentToolkit/GentToolkit_Default";

		public const string DECAL_GROUND_CRACK_01 = "GroundCrack/Decal_Ground_Crack_01";

		public const string DECAL_GROUND_CRACK_02 = "GroundCrack/Decal_Ground_Crack_02";

		public const string IMPACT_BRICK = "Impacts/Impact_Brick";

		public const string IMPACT_BULLET = "Impacts/Impact_Bullet";

		public const string IMPACT_DIRT = "Impacts/Impact_Dirt";

		public const string IMPACT_ELECTRIC = "Impacts/Impact_Electric";

		public const string IMPACT_FABRIC = "Impacts/Impact_Fabric";

		public const string IMPACT_GENERIC = "Impacts/Impact_Generic";

		public const string IMPACT_INK = "Impacts/Impact_Ink";

		public const string IMPACT_INK_COLOR = "Impacts/Impact_Ink_Color";

		public const string IMPACT_METAL = "Impacts/Impact_Metal";

		public const string IMPACT_PAPER = "Impacts/Impact_Paper";

		public const string IMPACT_SPARKS = "Impacts/Impact_Sparks";

		public const string IMPACT_SQUEAK = "Impacts/Impact_Squeak";

		public const string IMPACT_STONE = "Impacts/Impact_Stone";

		public const string IMPACT_TILE = "Impacts/Impact_Tile";

		public const string IMPACT_WOOD = "Impacts/Impact_Wood";

		public const string INITIALIZE_GAME = "Initialize/InitializeGame";

		public const string PARTICLES_BANISH = "Particles/Particles_Banish";

		public const string PARTICLES_BENDY_SPLASH = "Particles/Particles_BendySplash";

		public const string PARTICLES_CLOUDS = "Particles/Particles_Clouds";

		public const string PARTICLES_EDISON_PIPE_FX = "Particles/Particles_EdisonPipe_FX";

		public const string PARTICLES_ELECTRICITY = "Particles/Particles_Electricity";

		public const string PARTICLES_FALLING_DIRT = "Particles/Particles_Falling_Dirt";

		public const string PARTICLES_FIREFLIES = "Particles/Particles_Fireflies";

		public const string PARTICLES_FLAME = "Particles/Particles_Flame";

		public const string PARTICLES_FLIES = "Particles/Particles_Flies";

		public const string PARTICLES_GUTS = "Particles/Particles_Guts";

		public const string PARTICLES_HIT_ENEMY_INK_FX = "Particles/Particles_HitEnemyInkFX";

		public const string PARTICLES_INK_DEMON_PARTICLE_DRIP = "Particles/Particles_InkDemonParticleDrip";

		public const string PARTICLES_INK_DEMON_FOOTPRINT = "Particles/Particles_InkDemon_Footprint";

		public const string PARTICLES_INK_DRIP_FAST = "Particles/Particles_InkDrip_Fast";

		public const string PARTICLES_INK_DRIP_MEDIUM = "Particles/Particles_InkDrip_Medium";

		public const string PARTICLES_INK_DRIP_SLOW = "Particles/Particles_InkDrip_Slow";

		public const string PARTICLES_INK_DRIP_SLOW_NO_COLLISION = "Particles/Particles_InkDrip_Slow_NoCollision";

		public const string PARTICLES_INK_FX_SMALL = "Particles/Particles_InkFX_Small";

		public const string PARTICLES_INK_HIT = "Particles/Particles_InkHit";

		public const string PARTICLES_INTERACTABLE_SPARKLE = "Particles/Particles_Interactable_Sparkle";

		public const string PARTICLES_PAPER = "Particles/Particles_Paper";

		public const string PARTICLES_SIGNAL_TOWER = "Particles/Particles_SignalTower";

		public const string PARTICLES_SMOKE_AREA_HEAVY = "Particles/Particles_Smoke_Area_Heavy";

		public const string PARTICLES_SMOKE_AREA_LIGHT = "Particles/Particles_Smoke_Area_Light";

		public const string PARTICLES_SMOKE_AREA_MEDIUM = "Particles/Particles_Smoke_Area_Medium";

		public const string PARTICLES_SMOKE_STREAM_HEAVY = "Particles/Particles_Smoke_Stream_Heavy";

		public const string PARTICLES_SPARKS_SMALL = "Particles/Particles_Sparks_Small";

		public const string PARTICLES_SPARKS_STREAM = "Particles/Particles_Sparks_Stream";

		public const string PARTICLES_SPARKS_TRIGGER_ONLY = "Particles/Particles_Sparks_TriggerOnly";

		public const string PARTICLES_SPLASH = "Particles/Particles_Splash";

		public const string PARTICLES_TELEPORT_TRAIL = "Particles/Particles_TeleportTrail";

		public const string PARTICLES_WILSON_DEATH = "Particles/Particles_WilsonDeath";

		public const string PROJECTILE_BULLET_LINE = "Projectiles/Projectile_Bullet_Line";

		public const string PROJECTILE_INK = "Projectiles/Projectile_Ink";

		public const string THROWABLE_ANCHOR = "Projectiles/Throwable_Anchor";

		public const string THROWABLE_GEAR = "Projectiles/Throwable_Gear";

		public const string SLUG_MANY = "Slugs/Slug_Many";

		public const string SLUGX_1 = "Slugs/Slug_x1";

		public const string SLUGX_2 = "Slugs/Slug_x2";

		public const string SLUGX_3 = "Slugs/Slug_x3";

		public const string SLUGX_4 = "Slugs/Slug_x4";

		public const string SLUGX_5 = "Slugs/Slug_x5";

		public const string WEAPON_GENT_PIPE = "Weapons/Weapon_GentPipe";

		public const string WEAPON_GENT_PIPE_01 = "Weapons/Weapon_GentPipe_01";

		public const string WEAPON_GENT_PIPE_02 = "Weapons/Weapon_GentPipe_02";

		public const string WEAPON_GENT_PIPE_03 = "Weapons/Weapon_GentPipe_03";

		public const string WEAPON_GENT_PIPE_04 = "Weapons/Weapon_GentPipe_04";

		public const string WEAPON_GENT_PIPE_POWER_INDICATOR = "Weapons/Weapon_GentPipe_PowerIndicator";

		public const string S101_JOEY_DREW_APARTMENT_AUDIO = "Audio/S101_JoeyDrewApartment_Audio";

		public const string S102_ARCH_GATE_OFFICES_AUDIO = "Audio/S102_ArchGateOffices_Audio";

		public const string S103_JOEY_DREW_EXHIBIT_AUDIO = "Audio/S103_JoeyDrewExhibit_Audio";

		public const string S104_BACK_OFFICE_AUDIO = "Audio/S104_BackOffice_Audio";

		public const string S105_WELCOME_HOME_AUDIO = "Audio/S105_WelcomeHome_Audio";

		public const string S106_HEAVENLY_TOYS_AUDIO = "Audio/S106_HeavenlyToys_Audio";

		public const string S107_FACTORY_ACCESS_AUDIO = "Audio/S107_FactoryAccess_Audio";

		public const string S108_ARTIST_ATRIUM_AUDIO = "Audio/S108_ArtistAtrium_Audio";

		public const string S109_ANIMATION_DEPARTMENT_AUDIO = "Audio/S109_AnimationDepartment_Audio";

		public const string S110_ANIMATION_ALLEY_AUDIO = "Audio/S110_AnimationAlley_Audio";

		public const string S111_PIPEWAYS_AUDIO = "Audio/S111_Pipeways_Audio";

		public const string S112_LOCKER_ROOM_AUDIO = "Audio/S112_LockerRoom_Audio";

		public const string S113_ARTISTS_REST_AUDIO = "Audio/S113_ArtistsRest_Audio";

		public const string S114_ELEVATORS_AUDIO = "Audio/S114_Elevators_Audio";

		public const string S115_WIDOW_CHAMBER_AUDIO = "Audio/S115_WidowChamber_Audio";

		public const string S116_SEWER_ENTRANCE_AUDIO = "Audio/S116_SewerEntrance_Audio";

		public const string S117_SEWERS_AUDIO = "Audio/S117_Sewers_Audio";

		public const string S118_CITY_ENTRANCE_AUDIO = "Audio/S118_CityEntrance_Audio";

		public const string S119_CITY_AUDIO = "Audio/S119_City_Audio";

		public const string S120_OLD_STUDIO_AUDIO = "Audio/S120_OldStudio_Audio";

		public const string S121_GENT_WORKSHOP_ENTRANCE_AUDIO = "Audio/S121_GentWorkshopEntrance_Audio";

		public const string S122_GENT_WORKSHOP_AUDIO = "Audio/S122_GentWorkshop_Audio";

		public const string S123_SUBWAY_AUDIO = "Audio/S123_Subway_Audio";

		public const string S124_RETREAT_AUDIO = "Audio/S124_Retreat_Audio";

		public const string S125_ALICE_ANGEL_AUDIO = "Audio/S125_AliceAngel_Audio";

		public const string S126_NORTH_WING_ENTRANCE_AUDIO = "Audio/S126_NorthWingEntrance_Audio";

		public const string S127_SOUTH_WING_ENTRANCE_AUDIO = "Audio/S127_SouthWingEntrance_Audio";

		public const string S128_LABORATORY_AUDIO = "Audio/S128_Laboratory_Audio";

		public const string S129_SHIP_AHOY_AUDIO = "Audio/S129_ShipAhoy_Audio";

		public const string S130_BEAST_BENDY_AUDIO = "Audio/S130_BeastBendy_Audio";

		public const string S132_JOEY_DREW_APARTMENT_AUDIO = "Audio/S132_JoeyDrewApartment_Audio";

		public const string ENEMY_INK_WIDOW_01 = "Enemy_InkWidow_01";

		public const string ENEMY_INK_WIDOW_02 = "Enemy_InkWidow_02";

		public const string INK_WIDOW_EGG = "InkWidowEgg";

		public const string ENEMY_SHIP_AHOY = "Enemy_ShipAhoy";

		public const string COMPANION_BENDY = "Companion_Bendy";

		public const string DUMMY_TOM_LURKER = "Dummy_TomLurker";

		public const string PLAYER_AUDREY = "Characters/Player/Player_Audrey";

		public const string PLAYER_AUDREY_EMPTY = "Characters/Player/Player_Audrey_Empty";

		public const string PLAYER_AUDREY_INSANE = "Characters/Player/Player_Audrey_Insane";

		public const string PLAYER_AUDREY_REAL_WORLD = "Characters/Player/Player_Audrey_RealWorld";

		public const string PLAYER_BEAST_BENDY = "Characters/Player/Player_BeastBendy";

		public const string PLAYER_SPAWN_QUAD = "Characters/Player/Player_SpawnQuad";

		public const string PRESENT_SEASONAL = "Seasonal/Presents/Present_Seasonal";

		public const string UI_BLOCKER = "UI/Blocker/UIBlocker";

		public const string UI_HIT_BORDER = "UI/Borders/UIHitBorder";

		public const string UI_VISUAL_CONTROLS = "UI/Core/UIVisualControls";

		public const string UI_CROSSHAIR = "UI/Crosshairs/UICrosshair";

		public const string UI_CROSSHAIR_REAL = "UI/Crosshairs/UICrosshairReal";

		public const string MENU_ELEMENT_BUTTON = "UI/Elements/MenuElementButton";

		public const string MENU_ELEMENT_SPACE = "UI/Elements/MenuElementSpace";

		public const string MENU_ELEMENT_TITLE = "UI/Elements/MenuElementTitle";

		public const string UI_ELEMENT_AUDIO_LOG = "UI/Elements/UIElementAudioLog";

		public const string UI_ELEMENT_AUDIO_LOG_BUTTON_LABEL = "UI/Elements/UIElementAudioLogButtonLabel";

		public const string UI_ELEMENT_GAME_MENU_MENU_BUTTON = "UI/Elements/UIElementGameMenuMenuButton";

		public const string UI_ELEMENT_GAME_MENU_SETTINGS_BUTTON = "UI/Elements/UIElementGameMenuSettingsButton";

		public const string UI_ELEMENT_GENT_EXCHANGE_COMPONENT = "UI/Elements/UIElementGentExchangeComponent";

		public const string UI_ELEMENT_GENT_UPGRADE_COMPONENT = "UI/Elements/UIElementGentUpgradeComponent";

		public const string UI_ELEMENT_HEALTH_STATUS = "UI/Elements/UIElementHealthStatus";

		public const string UI_ELEMENT_LABEL = "UI/Elements/UIElementLabel";

		public const string UI_ELEMENT_MEMO = "UI/Elements/UIElementMemo";

		public const string UI_ELEMENT_MEMO_BUTTON_LABEL = "UI/Elements/UIElementMemoButtonLabel";

		public const string UI_ELEMENT_MEMORY = "UI/Elements/UIElementMemory";

		public const string UI_ELEMENT_MEMORY_BUTTON_LABEL = "UI/Elements/UIElementMemoryButtonLabel";

		public const string UI_ELEMENT_MEMORY_IMAGE = "UI/Elements/UIElementMemoryImage";

		public const string UI_ELEMENT_MENU_FOOTER = "UI/Elements/UIElementMenuFooter";

		public const string UI_ELEMENT_MENU_FOOTER_BUTTON = "UI/Elements/UIElementMenuFooterButton";

		public const string UI_ELEMENT_MENU_TITLE_PAUSE = "UI/Elements/UIElementMenuTitlePause";

		public const string UI_ELEMENT_NOTIFICATION_BOX_LABEL = "UI/Elements/UIElementNotificationBoxLabel";

		public const string UI_ELEMENT_PHOTOSENSITIVE_WARNING = "UI/Elements/UIElementPhotosensitiveWarning";

		public const string UI_ELEMENT_RESOURCE = "UI/Elements/UIElementResource";

		public const string UI_ELEMENT_TITLE_MENU_BUTTON_CONTROLLER_LABEL = "UI/Elements/UIElementTitleMenuButtonControllerLabel";

		public const string UI_ELEMENT_TITLE_MENU_BUTTON_LABEL = "UI/Elements/UIElementTitleMenuButtonLabel";

		public const string UI_ELEMENT_TITLE_MENU_LABEL = "UI/Elements/UIElementTitleMenuLabel";

		public const string UI_ELEMENT_TITLE_MENU_SAVE = "UI/Elements/UIElementTitleMenuSave";

		public const string UI_ELEMENT_TITLE_MENU_TAB = "UI/Elements/UIElementTitleMenuTab";

		public const string UI_ELEMENT_UPGRADE = "UI/Elements/UIElementUpgrade";

		public const string UI_ELEMENT_UPGRADE_LEVEL = "UI/Elements/UIElementUpgradeLevel";

		public const string UI_ELEMENT_UPGRADES_STATUS = "UI/Elements/UIElementUpgradesStatus";

		public const string UI_NAV_INPUT = "UI/Elements/UINavInput";

		public const string MENU_ELEMENT_BUTTON_CONTROLLER_LABEL = "UI/GameMenu/MenuElementButtonControllerLabel";

		public const string MENU_ELEMENT_BUTTON_LABEL = "UI/GameMenu/MenuElementButtonLabel";

		public const string UI_ELEMENT_GAME_MENU_BUTTON = "UI/GameMenu/UIElementGameMenuButton";

		public const string UI_ELEMENT_GAME_MENU_BUTTON_LABEL = "UI/GameMenu/UIElementGameMenuButtonLabel";

		public const string UI_ELEMENT_GAME_MENU_OBJECTIVE = "UI/GameMenu/UIElementGameMenuObjective";

		public const string UI_GAME_MENU = "UI/GameMenu/UIGameMenu";

		public const string UI_PROMPT = "UI/GameMenuPrompt/UIPrompt";

		public const string UI_TITLE_PROMPT = "UI/GameMenuPrompt/UITitlePrompt";

		public const string UI_ELEMENT_GAME_OVER_BUTTON = "UI/GameOver/UIElementGameOverButton";

		public const string UI_GAME_OVER = "UI/GameOver/UIGameOver";

		public const string ASYNC_LOADER = "UI/Loaders/AsyncLoader";

		public const string UI_MESSAGE = "UI/Messages/UIMessage";

		public const string UI_AUDIO_LOG = "UI/Modals/UIAudioLog";

		public const string UI_CHAPTER_TITLE = "UI/Modals/UIChapterTitle";

		public const string UI_CUTSCENE_BARS = "UI/Modals/UICutsceneBars";

		public const string UI_GENT_EXCHANGE = "UI/Modals/UIGentExchange";

		public const string UI_GENT_UPGRADES = "UI/Modals/UIGentUpgrades";

		public const string UI_INFO_POPUP = "UI/Modals/UIInfoPopup";

		public const string UI_NOTIFICATION_BOX = "UI/Modals/UINotificationBox";

		public const string UI_NOTIFICATION_TEXT = "UI/Modals/UINotificationText";

		public const string UI_UPGRADES = "UI/Modals/UIUpgrades";

		public const string UI_NAVIGATION = "UI/Navigation/UINavigation";

		public const string UI_NAVIGATION_REAL = "UI/Navigation/UINavigationReal";

		public const string UI_OBJECTIVE = "UI/Objectives/UIObjective";

		public const string UI_OBJECTIVE_REAL = "UI/Objectives/UIObjectiveReal";

		public const string UI_BOOK = "UI/Prompts/UIBook";

		public const string UI_INSPECT = "UI/Prompts/UIInspect";

		public const string UI_INTERACTION = "UI/Prompts/UIInteraction";

		public const string UI_INTERACTION_INVALID = "UI/Prompts/UIInteractionInvalid";

		public const string UI_INTERACTION_REAL = "UI/Prompts/UIInteractionReal";

		public const string UI_MEMO = "UI/Prompts/UIMemo";

		public const string UI_SUBTITLES = "UI/Subtitles/UISubtitles";

		public const string UI_TELEPORT = "UI/Teleport/UITeleport";

		public const string UI_CONSOLE = "UI/Videos/UIConsole";

		public const string UIJDS = "UI/Videos/UIJDS";

		public const string UI_MAIN_CREDITS = "UI/Videos/UIMainCredits";

		public const string UI_MAIN_TITLE = "UI/Videos/UIMainTitle";

		public const string UI_POST_CREDITS = "UI/Videos/UIPostCredits";

		public const string UI_ARCH_GATE_INTRO = "UI/Views/UIArchGateIntro";

		public const string UI_ARCH_GATE_JDS_PRESENTS = "UI/Views/UIArchGateJDSPresents";

		public const string UI_CREDITS = "UI/Views/UICredits";

		public const string UIHUD = "UI/Views/UIHUD";

		public const string UI_LOADING_VIEW = "UI/Views/UILoadingView";

		public const string UI_PHOTOSENSITIVE_WARNING = "UI/Views/UIPhotosensitiveWarning";

		public const string UI_TITLE_VIEW = "UI/Views/UITitleView";

		public const string PREFAB_SEASONAL_HALLOWEEN_HAT_01_AUDREY = "Hats/Prefab_Seasonal_Halloween_Hat_01_Audrey";

		public const string PREFAB_SEASONAL_HALLOWEEN_HAT_01_UI_MODEL = "Hats/Prefab_Seasonal_Halloween_Hat_01_UIModel";

		public const string PREFAB_SEASONAL_WINTER_HAT_01 = "Hats/Prefab_Seasonal_Winter_Hat_01";

		public const string PREFAB_SEASONAL_WINTER_HAT_01_AUDREY = "Hats/Prefab_Seasonal_Winter_Hat_01_Audrey";

		public const string PREFAB_SEASONAL_WINTER_HAT_01_UI_MODEL = "Hats/Prefab_Seasonal_Winter_Hat_01_UIModel";

		public const string PREFAB_CHARACTER_FISHER = "Characters/Prefab_Character_Fisher";

		public const string ENEMY_BUTCHER_GANG_FISHER = "Enemies/Enemy_ButcherGang_Fisher";

		public const string PREFAB_CHARACTER_KEEPER = "Characters/Prefab_Character_Keeper";

		public const string PREFAB_CHARACTER_KEEPER_HOLDING_LOST_ONE = "Characters/Prefab_Character_Keeper_HoldingLostOne";

		public const string PREFAB_CHARACTER_KEEPER_HOVER = "Characters/Prefab_Character_Keeper_Hover";

		public const string ENEMY_KEEPER = "Enemies/Enemy_Keeper";

		public const string PREFAB_CHARACTER_KING_WIDOW = "Characters/Prefab_Character_KingWidow";

		public const string ENEMY_KING_WIDOW = "Enemies/Enemy_KingWidow";

		public const string PREFAB_CHARACTER_LOST_ONE_FEMALE = "Characters/Prefab_Character_LostOneFemale";

		public const string PREFAB_CHARACTER_LOST_ONE_FEMALE_AMOK_FOLLOWER = "Characters/Prefab_Character_LostOneFemale_Amok_Follower";

		public const string PREFAB_CHARACTER_LOST_ONE_FEMALE_AMOK_FOLLOWER_VICIOUS = "Characters/Prefab_Character_LostOneFemale_Amok_Follower_Vicious";

		public const string PREFAB_CHARACTER_LOST_ONE_FEMALE_BAG_HEAD = "Characters/Prefab_Character_LostOneFemale_BagHead";

		public const string PREFAB_CHARACTER_LOST_ONE_FEMALE_BAG_HEAD_VICIOUS = "Characters/Prefab_Character_LostOneFemale_BagHead_Vicious";

		public const string PREFAB_CHARACTER_LOST_ONE_FEMALE_COLOR = "Characters/Prefab_Character_LostOneFemale_Color";

		public const string PREFAB_CHARACTER_LOST_ONE_FEMALE_COLOR_VICIOUS = "Characters/Prefab_Character_LostOneFemale_Color_Vicious";

		public const string PREFAB_CHARACTER_LOST_ONE_FEMALE_HAMBUSH = "Characters/Prefab_Character_LostOneFemale_Hambush";

		public const string PREFAB_CHARACTER_LOST_ONE_FEMALE_HAMBUSH_VICIOUS = "Characters/Prefab_Character_LostOneFemale_Hambush_Vicious";

		public const string PREFAB_CHARACTER_LOST_ONE_FEMALE_MELANIE = "Characters/Prefab_Character_LostOneFemale_Melanie";

		public const string PREFAB_CHARACTER_LOST_ONE_FEMALE_MELANIE_VICIOUS = "Characters/Prefab_Character_LostOneFemale_Melanie_Vicious";

		public const string PREFAB_CHARACTER_LOST_ONE_FEMALE_VICIOUS = "Characters/Prefab_Character_LostOneFemale_Vicious";

		public const string PREFAB_CHARACTER_LOST_ONE_MALE = "Characters/Prefab_Character_LostOneMale";

		public const string PREFAB_CHARACTER_LOST_ONE_MALE_AMOK = "Characters/Prefab_Character_LostOneMale_Amok";

		public const string PREFAB_CHARACTER_LOST_ONE_MALE_AMOK_FOLLOWER = "Characters/Prefab_Character_LostOneMale_Amok_Follower";

		public const string PREFAB_CHARACTER_LOST_ONE_MALE_AMOK_FOLLOWER_VICIOUS = "Characters/Prefab_Character_LostOneMale_Amok_Follower_Vicious";

		public const string PREFAB_CHARACTER_LOST_ONE_MALE_BAG_HEAD = "Characters/Prefab_Character_LostOneMale_BagHead";

		public const string PREFAB_CHARACTER_LOST_ONE_MALE_BAG_HEAD_VICIOUS = "Characters/Prefab_Character_LostOneMale_BagHead_Vicious";

		public const string PREFAB_CHARACTER_LOST_ONE_MALE_COLOR = "Characters/Prefab_Character_LostOneMale_Color";

		public const string PREFAB_CHARACTER_LOST_ONE_MALE_COLOR_VICIOUS = "Characters/Prefab_Character_LostOneMale_Color_Vicious";

		public const string PREFAB_CHARACTER_LOST_ONE_MALE_HAL = "Characters/Prefab_Character_LostOneMale_Hal";

		public const string PREFAB_CHARACTER_LOST_ONE_MALE_HAL_VICIOUS = "Characters/Prefab_Character_LostOneMale_Hal_Vicious";

		public const string PREFAB_CHARACTER_LOST_ONE_MALE_HAMBUSH = "Characters/Prefab_Character_LostOneMale_Hambush";

		public const string PREFAB_CHARACTER_LOST_ONE_MALE_HAMBUSH_VICIOUS = "Characters/Prefab_Character_LostOneMale_Hambush_Vicious";

		public const string PREFAB_CHARACTER_LOST_ONE_MALE_MATT = "Characters/Prefab_Character_LostOneMale_Matt";

		public const string PREFAB_CHARACTER_LOST_ONE_MALE_SCOTT = "Characters/Prefab_Character_LostOneMale_Scott";

		public const string PREFAB_CHARACTER_LOST_ONE_MALE_VICIOUS = "Characters/Prefab_Character_LostOneMale_Vicious";

		public const string COMPANION_LOST_ONE_F_AMOK_FOLLOWER = "Companions/Companion_LostOneF_Amok_Follower";

		public const string COMPANION_LOST_ONE_AMOK_FOLLOWER = "Companions/Companion_LostOne_Amok_Follower";

		public const string DUMMY_LOST_ONE_F_DEFAULT = "Dummies/Dummy_LostOneF_Default";

		public const string DUMMY_LOST_ONE_F_DEFAULT_VICIOUS = "Dummies/Dummy_LostOneF_Default_Vicious";

		public const string DUMMY_LOST_ONE_DEFAULT = "Dummies/Dummy_LostOne_Default";

		public const string DUMMY_LOST_ONE_DEFAULT_VICIOUS = "Dummies/Dummy_LostOne_Default_Vicious";

		public const string ENEMY_LOST_ONE = "Enemies/Enemy_LostOne";

		public const string ENEMY_LOST_ONE_F_AMOK_FOLLOWER = "Enemies/Enemy_LostOneF_Amok_Follower";

		public const string ENEMY_LOST_ONE_F_COLOR = "Enemies/Enemy_LostOneF_Color";

		public const string ENEMY_LOST_ONE_F_COLOR_VICIOUS = "Enemies/Enemy_LostOneF_Color_Vicious";

		public const string ENEMY_LOST_ONE_F_DEFAULT = "Enemies/Enemy_LostOneF_Default";

		public const string ENEMY_LOST_ONE_AMOK = "Enemies/Enemy_LostOne_Amok";

		public const string ENEMY_LOST_ONE_AMOK_FOLLOWER = "Enemies/Enemy_LostOne_Amok_Follower";

		public const string ENEMY_LOST_ONE_COLOR = "Enemies/Enemy_LostOne_Color";

		public const string ENEMY_LOST_ONE_COLOR_VICIOUS = "Enemies/Enemy_LostOne_Color_Vicious";

		public const string ENEMY_LOST_ONE_DEFAULT = "Enemies/Enemy_LostOne_Default";

		public const string ENEMY_LOST_ONE_HAL = "Enemies/Enemy_LostOne_Hal";

		public const string ENEMY_LOST_ONE_HAL_VICIOUS = "Enemies/Enemy_LostOne_Hal_Vicious";

		public const string ENEMY_LOST_ONE_SCOTT = "Enemies/Enemy_LostOne_Scott";

		public const string ENEMY_LOST_ONE_VICIOUS = "Enemies/Enemy_LostOne_Vicious";

		public const string ENEMY_LOST_ONE_SEASONAL_HALLOWEEN = "Enemies/Enemy_LostOne_Seasonal_Halloween";

		public const string ENEMY_LOST_ONE_SEASONAL_HALLOWEEN_PUMPKIN_HEAD = "Enemies/Enemy_LostOne_Seasonal_Halloween_PumpkinHead";

		public const string ENEMY_LOST_ONE_SEASONAL_WINTER = "Enemies/Enemy_LostOne_Seasonal_Winter";

		public const string PREFAB_CHARACTER_PIPER = "Characters/Prefab_Character_Piper";

		public const string ENEMY_BUTCHER_GANG_PIPER = "Enemies/Enemy_ButcherGang_Piper";

		public const string PREFAB_CHARACTER_SEARCHER = "Characters/Prefab_Character_Searcher";

		public const string PREFAB_CHARACTER_SEARCHER_02 = "Characters/Prefab_Character_Searcher_02";

		public const string DUMMY_SEARCHER = "Dummies/Dummy_Searcher";

		public const string ENEMY_SEARCHER_01 = "Enemies/Enemy_Searcher_01";

		public const string ENEMY_SEARCHER_02 = "Enemies/Enemy_Searcher_02";

		public const string SEARCHER_SPAWNER = "Spawners/SearcherSpawner";

		public const string SEARCHER_SPAWNER_LOCKER = "Spawners/SearcherSpawner_Locker";

		public const string SEARCHER_SPAWNER_LOCKER_ACTIVE = "Spawners/SearcherSpawner_Locker_Active";

		public const string SEARCHER_SPAWNER_LOCKER_ACTIVE_SINGLE = "Spawners/SearcherSpawner_Locker_Active_Single";

		public const string PREFAB_CHARACTER_STRIKER = "Characters/Prefab_Character_Striker";

		public const string ENEMY_BUTCHER_GANG_STRIKER = "Enemies/Enemy_ButcherGang_Striker";

		public const string UI_ELEMENT_GAME_MENU_ABILITY = "UI/Elements/Abilities/UIElementGameMenuAbility";

		public const string UI_ELEMENT_GAME_MENU_ABILITY_EMPTY = "UI/Elements/Abilities/UIElementGameMenuAbilityEmpty";

		public const string UI_ELEMENT_GAME_MENU_ABILITY_HEADER = "UI/Elements/Abilities/UIElementGameMenuAbilityHeader";

		public const string UI_ELEMENT_TITLE_MENU_SELECTOR = "UI/Elements/Options/UIElementTitleMenuSelector";

		public const string UI_ELEMENT_TITLE_MENU_SLIDER = "UI/Elements/Options/UIElementTitleMenuSlider";

		public const string UI_ELEMENT_TITLE_MENU_TOGGLE = "UI/Elements/Options/UIElementTitleMenuToggle";

		public const string UI_ELEMENT_GAME_MENU_WEAPON = "UI/Elements/Weapon/UIElementGameMenuWeapon";

		public const string UI_ELEMENT_GAME_MENU_WEAPON_EMPTY = "UI/Elements/Weapon/UIElementGameMenuWeaponEmpty";

		public const string UI_ELEMENT_GAME_MENU_WEAPON_HEADER = "UI/Elements/Weapon/UIElementGameMenuWeaponHeader";

		public const string UI_ELEMENT_GAME_MENU_ICON_ABILITIES = "UI/GameMenu/Icons/UIElementGameMenuIconAbilities";

		public const string UI_ELEMENT_GAME_MENU_ICON_AUDIO_LOGS = "UI/GameMenu/Icons/UIElementGameMenuIconAudioLogs";

		public const string UI_ELEMENT_GAME_MENU_ICON_MEMORIES = "UI/GameMenu/Icons/UIElementGameMenuIconMemories";

		public const string UI_ELEMENT_GAME_MENU_ICON_MEMOS = "UI/GameMenu/Icons/UIElementGameMenuIconMemos";

		public const string UI_ELEMENT_GAME_MENU_ICON_MENU = "UI/GameMenu/Icons/UIElementGameMenuIconMenu";

		public const string UI_ELEMENT_GAME_MENU_ICON_SETTINGS = "UI/GameMenu/Icons/UIElementGameMenuIconSettings";

		public const string UI_ELEMENT_GAME_MENU_ICON_STATUS_BEAST = "UI/GameMenu/Icons/UIElementGameMenuIconStatusBeast";

		public const string UI_ELEMENT_GAME_MENU_ICON_STATUS_INK = "UI/GameMenu/Icons/UIElementGameMenuIconStatusInk";

		public const string UI_ELEMENT_GAME_MENU_ICON_STATUS_REAL = "UI/GameMenu/Icons/UIElementGameMenuIconStatusReal";

		public const string UI_ELEMENT_GAME_MENU_ICON_WEAPON = "UI/GameMenu/Icons/UIElementGameMenuIconWeapon";

		public const string MENU_ELEMENT_SAVE = "UI/GameMenu/Saves/MenuElementSave";

		public const string MENU_ELEMENT_SAVE_SLOT = "UI/GameMenu/Saves/MenuElementSaveSlot";

		public const string MENU_ELEMENT_SAVE_SLOT_EMPTY = "UI/GameMenu/Saves/MenuElementSaveSlotEmpty";

		public const string UI_ELEMENT_GAME_MENU_SELECTOR = "UI/GameMenu/Settings/UIElementGameMenuSelector";

		public const string UI_ELEMENT_GAME_MENU_SLIDER = "UI/GameMenu/Settings/UIElementGameMenuSlider";

		public const string UI_ELEMENT_GAME_MENU_TOGGLE = "UI/GameMenu/Settings/UIElementGameMenuToggle";
	}

	public static class Scenes
	{
		public const string INITIALIZE_GAME = "InitializeGame";

		public const string EMPTY = "Empty";

		public const string GAME = "Game";

		public const string RESET = "Reset";

		public const string SECTION_S101_JOEY_DREW_APARTMENT = "Section_S101_JoeyDrewApartment";

		public const string SECTION_S102_ARCH_GATE_OFFICES = "Section_S102_ArchGateOffices";

		public const string SECTION_S103_JOEY_DREW_EXHIBIT = "Section_S103_JoeyDrewExhibit";

		public const string SECTION_S104_BACK_OFFICE = "Section_S104_BackOffice";

		public const string SECTION_S105_WELCOME_HOME = "Section_S105_WelcomeHome";

		public const string SECTION_S106_HEAVENLY_TOYS = "Section_S106_HeavenlyToys";

		public const string SECTION_S107_FACTORY_ACCESS = "Section_S107_FactoryAccess";

		public const string SECTION_S108_ARTIST_ATRIUM = "Section_S108_ArtistAtrium";

		public const string SECTION_S109_ANIMATION_DEPARTMENT = "Section_S109_AnimationDepartment";

		public const string SECTION_S110_ANIMATION_ALLEY = "Section_S110_AnimationAlley";

		public const string SECTION_S111_PIPEWAYS = "Section_S111_Pipeways";

		public const string SECTION_S112_LOCKER_ROOM = "Section_S112_LockerRoom";

		public const string SECTION_S113_ARTISTS_REST = "Section_S113_ArtistsRest";

		public const string SECTION_S114_ELEVATORS = "Section_S114_Elevators";

		public const string SECTION_S115_WIDOW_CHAMBER = "Section_S115_WidowChamber";

		public const string SECTION_S116_SEWER_ENTRANCE = "Section_S116_SewerEntrance";

		public const string SECTION_S117_SEWERS = "Section_S117_Sewers";

		public const string SECTION_S118_CITY_ENTRANCE = "Section_S118_CityEntrance";

		public const string SECTION_S119_CITY = "Section_S119_City";

		public const string SECTION_S120_OLD_STUDIO = "Section_S120_OldStudio";

		public const string SECTION_S121_GENT_WORKSHOP_ENTRANCE = "Section_S121_GentWorkshopEntrance";

		public const string SECTION_S122_GENT_WORKSHOP = "Section_S122_GentWorkshop";

		public const string SECTION_S123_SUBWAY = "Section_S123_Subway";

		public const string SECTION_S124_RETREAT = "Section_S124_Retreat";

		public const string SECTION_S125_ALICE_ANGEL = "Section_S125_AliceAngel";

		public const string SECTION_S126_NORTH_WING_ENTRANCE = "Section_S126_NorthWingEntrance";

		public const string SECTION_S127_SOUTH_WING_ENTRANCE = "Section_S127_SouthWingEntrance";

		public const string SECTION_S128_LABORATORY = "Section_S128_Laboratory";

		public const string SECTION_S129_SHIP_AHOY = "Section_S129_ShipAhoy";

		public const string SECTION_S130_BEAST_BENDY = "Section_S130_BeastBendy";

		public const string SECTION_S131_INSANE = "Section_S131_Insane";

		public const string SECTION_S132_JOEY_DREW_APARTMENT = "Section_S132_JoeyDrewApartment";

		public const string SECTION_S133_ARCHIVES = "Section_S133_Archives";
	}

	public static class Materials
	{
		public const string TRANSPARENT_PURPLE = "transparentPurple";

		public const string TRANSPARENT_YELLOW = "transparentYellow";

		public const string INK_ARCH_GATE = "Ink_ArchGate";

		public const string INK_SPOUT_ARCH_GATE = "Ink_Spout_ArchGate";

		public const string DARKNESS_BOX_MATERIAL = "Darkness_Box_Material";

		public const string DARKNESS_TUNNEL_MATERIAL = "Darkness_Tunnel_Material";

		public const string DARKNESS_VOLUME_MATERIAL = "Darkness_Volume_Material";

		public const string DECAL_MATERIAL = "DecalMaterial";

		public const string DECAL_MATERIAL_YELLOW = "DecalMaterial_Yellow";

		public const string BATDRDECALTORNDRAWING = "batdr_decal_torndrawing";

		public const string BENDY_POSE_STATIC_MATERIAL_01 = "BendyPoseStaticMaterial_01";

		public const string DECALATRIUMMURAL = "decal_atrium_mural";

		public const string DECAL_MIRROR_MATERIAL = "Decal_Mirror_Material";

		public const string INKSPLATDECAL = "ink_splat_decal";

		public const string MEATLYMATERIAL_01 = "meatly_material_01";

		public const string MEATLYMATERIAL_02 = "meatly_material_02";

		public const string MEATLYMATERIAL_03 = "meatly_material_03";

		public const string MEATLYMATERIAL_04 = "meatly_material_04";

		public const string MEATLYMATERIAL_05 = "meatly_material_05";

		public const string MEATLYMATERIAL_06 = "meatly_material_06";

		public const string MEATLYMATERIAL_07 = "meatly_material_07";

		public const string MEATLYMATERIAL_08 = "meatly_material_08";

		public const string MEATLYMATERIAL_09 = "meatly_material_09";

		public const string MEATLYMATERIAL_10 = "meatly_material_10";

		public const string TESTINKDEMONSPLAT = "test_inkdemon_splat";

		public const string BULLET_LINE = "Bullet_Line";

		public const string BULLET_LINE_FAKE = "Bullet_Line_Fake";

		public const string CARTOON_PROJECTION = "CartoonProjection";

		public const string DEFAULT_HIGHLIGHT = "DefaultHighlight";

		public const string EFFECT_BENDY_FOOTSTEP_MATERIAL = "Effect_BendyFootstep_Material";

		public const string EFFECT_BENDY_INK_EFFECT_MATERIAL = "Effect_BendyInkEffect_Material";

		public const string EFFECT_JOEY_MELT_MATERIAL = "Effect_Joey_Melt_Material";

		public const string HIGHLIGHT_MATERIAL = "HighlightMaterial";

		public const string HIGHLIGHT_OUTLINE_MATERIAL = "HighlightOutlineMaterial";

		public const string INVISIBLE_HIGHLIGHT_MATERIAL = "InvisibleHighlightMaterial";

		public const string SHEET = "Sheet";

		public const string SHEET_TOON = "SheetToon";

		public const string SWING_BLUR = "SwingBlur";

		public const string WATER_SPLASH = "WaterSplash";

		public const string WATER_SPLASH_R_IPPLE = "WaterSplash_RIpple";

		public const string GLASS = "Glass";

		public const string GLASS_DEFAULT = "Glass_Default";

		public const string GLASS_DEFAULT_02 = "Glass_Default_02";

		public const string GLASS_DEFAULT_BROKEN = "Glass_Default_Broken";

		public const string GLASS_DEFAULT_NO_REFLECTION = "Glass_Default_NoReflection";

		public const string GLASS_GENERIC = "Glass_Generic";

		public const string GLASS_GENERIC_SOFT_1 = "Glass_Generic_Soft 1";

		public const string GLASS_GENERIC_SOFT = "Glass_Generic_Soft";

		public const string GLASS_TOON_GENERIC = "Glass_Toon_Generic";

		public const string GLASS_TOON_TEST = "Glass_Toon_Test";

		public const string GLASS_TUBE = "Glass_Tube";

		public const string LIGHT_DEFAULT = "Light_Default";

		public const string LIGHT_DIM = "Light_Dim";

		public const string LIGHT_MEDIUM = "Light_Medium";

		public const string LIGHT_RW_WHITE = "Light_RW_White";

		public const string CITY_SKYLINE = "City_Skyline";

		public const string PARTCILE_SMOKE_MATERIAL = "Partcile_Smoke_Material";

		public const string PARTICLE_BANISH_BODY_DUST = "Particle_Banish_Body_Dust";

		public const string PARTICLE_BRICK_STONE_MATERIAL = "Particle_BrickStone_Material";

		public const string PARTICLE_CANDLE_FLAME_MATERIAL = "Particle_CandleFlame_Material";

		public const string PARTICLE_CITY_CLOUDS_MATERIAL = "Particle_City_Clouds_Material";

		public const string PARTICLE_CRACKED_MATERIAL = "Particle_Cracked_Material";

		public const string PARTICLE_ELECTRICITY = "Particle_Electricity";

		public const string PARTICLE_ELECTRICITY_LOOP = "Particle_Electricity_Loop";

		public const string PARTICLE_FALLING_DUST_MATERIAL = "Particle_Falling_Dust_Material";

		public const string PARTICLE_FIREFLY_MATERIAL = "Particle_Firefly_Material";

		public const string PARTICLE_FLY_MATERIAL = "Particle_Fly_Material";

		public const string PARTICLE_IMPACT_MATERIAL = "Particle_Impact_Material";

		public const string PARTICLE_LIGHTNING_CIRCLE_MATERIAL = "Particle_LightningCircle_Material";

		public const string PARTICLE_LIGHTNING_ZAP_MATERIAL = "Particle_LightningZap_Material";

		public const string PARTICLE_PAPER_MATERIAL = "Particle_Paper_Material";

		public const string PARTICLE_SNOW_MATERIAL = "Particle_Snow_Material";

		public const string PARTICLE_SPARKLE_DOTS = "Particle_Sparkle_Dots";

		public const string PARTICLE_SPARKLE_MATERIAL = "Particle_Sparkle_Material";

		public const string PARTICLE_SPARK_MATERIAL = "Particle_Spark_Material";

		public const string PARTICLE_TAKEDOWN_MATERIAL = "Particle_Takedown_Material";

		public const string PARTICLE_TELEPORT_MATERIAL = "Particle_Teleport_Material";

		public const string PARTICLE_TILE_MATERIAL = "Particle_Tile_Material";

		public const string PARTICLE_WOOD_MATERIAL = "Particle_Wood_Material";

		public const string MONITOR_RENDER_MATERIAL = "MonitorRenderMaterial";

		public const string UI_RENDER_MATERIAL = "UIRenderMaterial";

		public const string DISSOLVE_MATERIAL = "DissolveMaterial";

		public const string BASIC_BLUR = "BasicBlur";

		public const string BASIC_BLUR_LOW = "BasicBlurLow";

		public const string UI_BASE = "UIBase";

		public const string UI_BLOOM = "UIBloom";

		public const string UI_DISSOLVE = "UIDissolve";

		public const string UI_RAIN = "UIRain";

		public const string BLACK = "Black";

		public const string BLACK_UNLIT = "Black_Unlit";

		public const string EDITOR_BENDY_MATERIAL = "EditorBendy_Material";

		public const string EDITOR_CUTSCENE_MATERIAL = "EditorCutscene_Material";

		public const string EDITOR_SCENE_PLAY_MATERIAL = "EditorScenePlay_Material";

		public const string UI_TITLE_VIDEO_MATERIAL = "UITitleVideo_Material";

		public const string BKDEFAULT = "bk_default";

		public const string LANTERN_DEFAULT = "Lantern_Default";

		public const string LANTERN_DEFAULT_OUTLINE = "Lantern_Default_Outline";

		public const string INK_MATERIAL_SEARCHER_FACES = "Ink_Material_SearcherFaces";

		public const string PORTER_BANISH_MATERIAL = "Porter_Banish_Material";

		public const string PORTER_BANISH_MATERIAL_OUTLINE = "Porter_Banish_Material_Outline";

		public const string MASTER_ANIMATION_ALLEY_MATERIAL_01 = "Master_AnimationAlley_Material_01";

		public const string MASTER_ANIMATION_ALLEY_MATERIAL_01_OUTLINE = "Master_AnimationAlley_Material_01_Outline";

		public const string MASTER_ANIMATION_ALLEY_MATERIAL_02 = "Master_AnimationAlley_Material_02";

		public const string MASTER_ANIMATION_ALLEY_MATERIAL_02_OUTLINE = "Master_AnimationAlley_Material_02_Outline";

		public const string MASTER_ANIMATION_ALLEY_MATERIAL_03 = "Master_AnimationAlley_Material_03";

		public const string MASTER_ANIMATION_ALLEY_MATERIAL_03_OUTLINE = "Master_AnimationAlley_Material_03_Outline";

		public const string MASTER_ANIMATION_ALLEY_MATERIAL_04 = "Master_AnimationAlley_Material_04";

		public const string MASTER_ANIMATION_ALLEY_MATERIAL_04_OUTLINE = "Master_AnimationAlley_Material_04_Outline";

		public const string MASTER_ANIMATION_ALLEY_MATERIAL_05 = "Master_AnimationAlley_Material_05";

		public const string MASTER_ANIMATION_ALLEY_MATERIAL_06 = "Master_AnimationAlley_Material_06";

		public const string MASTER_ANIMATION_ALLEY_MATERIAL_07 = "Master_AnimationAlley_Material_07";

		public const string MASTER_ANIMATION_ALLEY_MATERIAL_08 = "Master_AnimationAlley_Material_08";

		public const string MASTER_ANIMATION_ALLEY_MATERIAL_08_OUTLINE = "Master_AnimationAlley_Material_08_Outline";

		public const string ARCH_GATE_LIGHT_MATERIAL = "ArchGate_Light_Material";

		public const string MASTER_ARCH_GATE_MATERIAL_01 = "Master_ArchGate_Material_01";

		public const string MASTER_ARCH_GATE_MATERIAL_01_NO_SHINE = "Master_ArchGate_Material_01_NoShine";

		public const string MASTER_ARCH_GATE_MATERIAL_01_OUTLINE = "Master_ArchGate_Material_01_Outline";

		public const string MASTER_ARCH_GATE_MATERIAL_02 = "Master_ArchGate_Material_02";

		public const string MASTER_ARCH_GATE_MATERIAL_02_NO_SHINE = "Master_ArchGate_Material_02_NoShine";

		public const string MASTER_ARCH_GATE_MATERIAL_02_OUTLINE = "Master_ArchGate_Material_02_Outline";

		public const string MASTER_ARCH_GATE_MATERIAL_03 = "Master_ArchGate_Material_03";

		public const string MASTER_ARCH_GATE_MATERIAL_03_NO_SHINE = "Master_ArchGate_Material_03_NoShine";

		public const string MASTER_ARCH_GATE_MATERIAL_03_OUTLINE = "Master_ArchGate_Material_03_Outline";

		public const string MASTER_ARCH_GATE_MATERIAL_04 = "Master_ArchGate_Material_04";

		public const string MASTER_ARCH_GATE_MATERIAL_04_OUTLINE = "Master_ArchGate_Material_04_Outline";

		public const string MASTER_ARCH_GATE_MATERIAL_05 = "Master_ArchGate_Material_05";

		public const string MASTER_ARCH_GATE_MATERIAL_05_OUTLINE = "Master_ArchGate_Material_05_Outline";

		public const string MASTER_ARCH_GATE_MATERIAL_06 = "Master_ArchGate_Material_06";

		public const string MASTER_ARCH_GATE_MATERIAL_06_NO_SHINE = "Master_ArchGate_Material_06_NoShine";

		public const string MASTER_ARCH_GATE_MATERIAL_BRONZE = "Master_ArchGate_Material_Bronze";

		public const string MASTER_ARCH_GATE_MATERIAL_BRONZE_OUTLINE = "Master_ArchGate_Material_Bronze_Outline";

		public const string MASTER_ARCH_GATE_MATERIAL_CITY_SCAPE = "Master_ArchGate_Material_CityScape";

		public const string MASTER_ARCH_GATE_MATERIAL_GLASS_01 = "Master_ArchGate_Material_Glass_01";

		public const string MASTER_ARCH_GATE_MATERIAL_GLASS_02 = "Master_ArchGate_Material_Glass_02";

		public const string MASTER_ARCH_GATE_MATERIAL_GLASS_03 = "Master_ArchGate_Material_Glass_03";

		public const string MASTER_ARCH_GATE_MATERIAL_PLUSH_BENDY = "Master_ArchGate_Material_Plush_Bendy";

		public const string MASTER_ARCH_GATE_MATERIAL_PLUSH_BENDY_OUTLINE = "Master_ArchGate_Material_Plush_Bendy_Outline";

		public const string MASTER_ARCH_GATE_MATERIAL_RAIN = "Master_ArchGate_Material_Rain";

		public const string MASTER_ARCH_GATE_MATERIAL_RAIN_SHADOW = "Master_ArchGate_Material_Rain_Shadow";

		public const string CARPET_TEST_3 = "CarpetTest 3";

		public const string MASTER_BANNERS_MATERIAL_01 = "Master_Banners_Material_01";

		public const string MASTER_BANNERS_MATERIAL_01_FLIP = "Master_Banners_Material_01_Flip";

		public const string CITY_1 = "City1";

		public const string CITY_2 = "City2";

		public const string MASTER_CITY_MATERIAL_01 = "Master_City_Material_01";

		public const string MASTER_CITY_MATERIAL_01_OUTLINE = "Master_City_Material_01_Outline";

		public const string MASTER_CITY_MATERIAL_02 = "Master_City_Material_02";

		public const string MASTER_CITY_MATERIAL_03 = "Master_City_Material_03";

		public const string MASTER_CITY_MATERIAL_03_OUTLINE = "Master_City_Material_03_Outline";

		public const string MASTER_CITY_MATERIAL_04 = "Master_City_Material_04";

		public const string MASTER_CITY_MATERIAL_04_OUTLINE = "Master_City_Material_04_Outline";

		public const string MASTER_CITY_MATERIAL_05 = "Master_City_Material_05";

		public const string MASTER_CITY_MATERIAL_05_EMISSION = "Master_City_Material_05_Emission";

		public const string MASTER_CITY_MATERIAL_06 = "Master_City_Material_06";

		public const string MASTER_CITY_MATERIAL_07 = "Master_City_Material_07";

		public const string MASTER_CITY_MATERIAL_07_OUTLINE = "Master_City_Material_07_Outline";

		public const string MASTER_CITY_MATERIAL_CITYSCAPE = "Master_City_Material_Cityscape";

		public const string MASTER_CITY_MATERIAL_MOON = "Master_City_Material_Moon";

		public const string MASTER_CITY_MATERIAL_MOON_EASTER_EGG = "Master_City_Material_Moon_EasterEgg";

		public const string MASTER_CITY_MATERIAL_SIGNS = "Master_City_Material_Signs";

		public const string MASTER_CITY_MATERIAL_SIGNS_EMISSION = "Master_City_Material_Signs_Emission";

		public const string MASTER_CITY_MATERIAL_SIGNS_EMISSION_OFF = "Master_City_Material_Signs_Emission_Off";

		public const string MASTER_CURTAIN_MATERIAL_01 = "Master_Curtain_Material_01";

		public const string MASTER_CUTOUT_INK_BENDY_MATERIAL = "Master_Cutout_InkBendy_Material";

		public const string MASTER_CUTOUT_MATERIAL_01 = "Master_Cutout_Material_01";

		public const string MASTER_CUTOUT_MATERIAL_02 = "Master_Cutout_Material_02";

		public const string MASTER_GENERAL_MATERIAL_01 = "Master_General_Material_01";

		public const string MASTER_GENERAL_MATERIAL_01_OUTLINE = "Master_General_Material_01_Outline";

		public const string MASTER_GENERAL_MATERIAL_02 = "Master_General_Material_02";

		public const string MASTER_GENERAL_MATERIAL_02_OUTLINE = "Master_General_Material_02_Outline";

		public const string MASTER_GENERAL_MATERIAL_03 = "Master_General_Material_03";

		public const string MASTER_GENERAL_MATERIAL_03_OUTLINE = "Master_General_Material_03_Outline";

		public const string MASTER_GENERAL_MATERIAL_04 = "Master_General_Material_04";

		public const string MASTER_GENERAL_MATERIAL_04_OUTLINE = "Master_General_Material_04_Outline";

		public const string MASTER_GENERAL_MATERIAL_05 = "Master_General_Material_05";

		public const string MASTER_GENERAL_MATERIAL_06 = "Master_General_Material_06";

		public const string MASTER_GENERAL_MATERIAL_06_OUTLINE = "Master_General_Material_06_Outline";

		public const string MASTER_GENERAL_MATERIAL_07 = "Master_General_Material_07";

		public const string MASTER_GENERAL_MATERIAL_07_OUTLINE = "Master_General_Material_07_Outline";

		public const string MASTER_GENT_MATERIAL_01 = "Master_Gent_Material_01";

		public const string MASTER_GENT_MATERIAL_01_OUTLINE = "Master_Gent_Material_01_Outline";

		public const string MASTER_GENT_MATERIAL_02 = "Master_Gent_Material_02";

		public const string MASTER_GENT_MATERIAL_02_OUTLINE = "Master_Gent_Material_02_Outline";

		public const string MASTER_GENT_MATERIAL_03 = "Master_Gent_Material_03";

		public const string MASTER_GENT_MATERIAL_03_OUTLINE = "Master_Gent_Material_03_Outline";

		public const string MASTER_GENT_MATERIAL_04 = "Master_Gent_Material_04";

		public const string MASTER_GENT_MATERIAL_05 = "Master_Gent_Material_05";

		public const string MASTER_GENT_MATERIAL_06 = "Master_Gent_Material_06";

		public const string MASTER_GENT_MATERIAL_07 = "Master_Gent_Material_07";

		public const string MASTER_GENT_MATERIAL_07_OUTLINE = "Master_Gent_Material_07_Outline";

		public const string MASTER_HEAVENLY_TOYS_MATERIAL_01 = "Master_HeavenlyToys_Material_01";

		public const string MASTER_HEAVENLY_TOYS_MATERIAL_01_OUTLINE = "Master_HeavenlyToys_Material_01_Outline";

		public const string MASTER_HEAVENLY_TOYS_MATERIAL_02 = "Master_HeavenlyToys_Material_02";

		public const string MASTER_INK_MATERIAL_BLACK_01 = "Master_Ink_Material_Black_01";

		public const string MASTER_INK_MATERIAL_COLOR = "Master_Ink_Material_Color";

		public const string MASTER_INK_MATERIAL_COLOR_ANIMATED = "Master_Ink_Material_Color_Animated";

		public const string MASTER_INK_MATERIAL_FOUNTAIN_FALL = "Master_Ink_Material_Fountain_Fall";

		public const string MASTER_INK_MATERIAL_FOUNTAIN_POOL = "Master_Ink_Material_Fountain_Pool";

		public const string MASTER_INK_MATERIAL_GENERIC_01 = "Master_Ink_Material_Generic_01";

		public const string MASTER_INK_MATERIAL_GENERIC_02 = "Master_Ink_Material_Generic_02";

		public const string MASTER_INK_MATERIAL_WATER_01 = "Master_Ink_Material_Water_01";

		public const string MASTER_INK_MATERIAL_WATER_02 = "Master_Ink_Material_Water_02";

		public const string MASTER_INK_MATERIAL_WATER_03 = "Master_Ink_Material_Water_03";

		public const string MASTER_INK_MATERIAL_WATER_TAP = "Master_Ink_Material_Water_Tap";

		public const string MASTER_INSANE_MATERIAL_01 = "Master_Insane_Material_01";

		public const string MASTER_INTRO_MATERIAL_01 = "Master_Intro_Material_01";

		public const string MASTER_INTRO_MATERIAL_01_OUTLINE = "Master_Intro_Material_01_Outline";

		public const string MASTER_INTRO_MATERIAL_02 = "Master_Intro_Material_02";

		public const string MASTER_INTRO_MATERIAL_03 = "Master_Intro_Material_03";

		public const string MASTER_INTRO_MATERIAL_03_OUTLINE = "Master_Intro_Material_03_Outline";

		public const string MASTER_INTRO_MATERIAL_LIGHT_RAYS = "Master_Intro_Material_LightRays";

		public const string MATERIAL_GUN_MUZZLE = "Material_Gun_Muzzle";

		public const string MASTER_JD_APARTMENT_DECAL_MATERIAL_01 = "Master_JDApartment_Decal_Material_01";

		public const string MASTER_JD_APARTMENT_ENV_MATERIAL_01 = "Master_JDApartment_Env_Material_01";

		public const string MASTER_JD_APARTMENT_ENV_MATERIAL_02 = "Master_JDApartment_Env_Material_02";

		public const string MASTER_JD_APARTMENT_ENV_MATERIAL_03 = "Master_JDApartment_Env_Material_03";

		public const string MASTER_JD_APARTMENT_MATERIAL_LIGHT_RAYS = "Master_JDApartment_Material_LightRays";

		public const string MASTER_JD_APARTMENT_PROPS_MATERIAL_01 = "Master_JDApartment_Props_Material_01";

		public const string MASTER_JD_APARTMENT_PROPS_MATERIAL_02 = "Master_JDApartment_Props_Material_02";

		public const string MASTER_JD_APARTMENT_PROPS_MATERIAL_03 = "Master_JDApartment_Props_Material_03";

		public const string MASTER_JD_APARTMENT_PROPS_MATERIAL_03_EMISSION = "Master_JDApartment_Props_Material_03_Emission";

		public const string MASTER_JD_APARTMENT_PROPS_MATERIAL_04 = "Master_JDApartment_Props_Material_04";

		public const string MASTER_JD_APARTMENT_PROPS_MATERIAL_04_TRANSPARENT = "Master_JDApartment_Props_Material_04_Transparent";

		public const string MASTER_JD_APARTMENT_PROPS_MATERIAL_05 = "Master_JDApartment_Props_Material_05";

		public const string MASTER_JD_APARTMENT_PROPS_MATERIAL_06 = "Master_JDApartment_Props_Material_06";

		public const string MASTER_JD_APARTMENT_PROPS_MATERIAL_06_GLASS = "Master_JDApartment_Props_Material_06_Glass";

		public const string MASTER_JD_APARTMENT_PROPS_MATERIAL_07 = "Master_JDApartment_Props_Material_07";

		public const string PARTICLE_JD_APARTMENT_MATERIAL_CLOUDS = "Particle_JDApartment_Material_Clouds";

		public const string PARTICLE_JD_APARTMENT_MATERIAL_FLIES = "Particle_JDApartment_Material_Flies";

		public const string PARTICLE_JD_APARTMENT_MATERIAL_WATER_DROPLETS = "Particle_JDApartment_Material_Water_Droplets";

		public const string MASTER_LABRATORY_MATERIAL_01 = "Master_Labratory_Material_01";

		public const string MASTER_LABRATORY_MATERIAL_01_COLOR = "Master_Labratory_Material_01_Color";

		public const string MASTER_LABRATORY_MATERIAL_01_OUTLINE = "Master_Labratory_Material_01_Outline";

		public const string MASTER_LABRATORY_MATERIAL_02 = "Master_Labratory_Material_02";

		public const string MASTER_LABRATORY_MATERIAL_03 = "Master_Labratory_Material_03";

		public const string MASTER_LABRATORY_MATERIAL_03_OUTLINE = "Master_Labratory_Material_03_Outline";

		public const string MASTER_LABRATORY_MATERIAL_04 = "Master_Labratory_Material_04";

		public const string MASTER_LABRATORY_MATERIAL_05 = "Master_Labratory_Material_05";

		public const string MASTER_LIGHTFIXTURE_MATERIAL_01 = "Master_Lightfixture_Material_01";

		public const string MASTER_LIGHTFIXTURE_MATERIAL_01_OFF = "Master_Lightfixture_Material_01_Off";

		public const string MASTER_LIGHTFIXTURE_MATERIAL_01_OFF_OUTLINE = "Master_Lightfixture_Material_01_Off_Outline";

		public const string MASTER_LIGHTFIXTURE_MATERIAL_01_OUTLINE = "Master_Lightfixture_Material_01_Outline";

		public const string MASTER_MANOR_MATERIAL_01 = "Master_Manor_Material_01";

		public const string MASTER_MANOR_MATERIAL_02 = "Master_Manor_Material_02";

		public const string MASTER_MANOR_MATERIAL_02_OUTLINE = "Master_Manor_Material_02_Outline";

		public const string MASTER_MECHANISMS_MATERIAL_01 = "Master_Mechanisms_Material_01";

		public const string MASTER_PLUSH_MATERIAL_BENDY = "Master_Plush_Material_Bendy";

		public const string MASTER_PLUSH_MATERIAL_BORIS = "Master_Plush_Material_Boris";

		public const string MASTER_PLUSH_MATERIAL_BORIS_OUTLINE = "Master_Plush_Material_Boris_Outline";

		public const string MASTER_POSTERS_MATERIAL_01 = "Master_Posters_Material_01";

		public const string MASTER_POSTERS_MATERIAL_02 = "Master_Posters_Material_02";

		public const string MASTER_POSTERS_MATERIAL_03 = "Master_Posters_Material_03";

		public const string MASTER_POSTERS_MATERIAL_04 = "Master_Posters_Material_04";

		public const string MASTER_POSTERS_MATERIAL_05 = "Master_Posters_Material_05";

		public const string MASTER_POSTERS_MATERIAL_06 = "Master_Posters_Material_06";

		public const string MASTER_POSTERS_MATERIAL_07 = "Master_Posters_Material_07";

		public const string MASTER_PROPS_MATERIAL_01 = "Master_Props_Material_01";

		public const string MASTER_PROPS_MATERIAL_01_OUTLINE = "Master_Props_Material_01_Outline";

		public const string MASTER_PROPS_MATERIAL_02 = "Master_Props_Material_02";

		public const string MASTER_PROPS_MATERIAL_02_CHARACTER = "Master_Props_Material_02_Character";

		public const string MASTER_PROPS_MATERIAL_02_OUTLINE = "Master_Props_Material_02_Outline";

		public const string MASTER_PROPS_MATERIAL_03 = "Master_Props_Material_03";

		public const string MASTER_PROPS_MATERIAL_03_OUTLINE = "Master_Props_Material_03_Outline";

		public const string MASTER_PROPS_MATERIAL_03_PLANTS = "Master_Props_Material_03_Plants";

		public const string MASTER_PROPS_MATERIAL_04 = "Master_Props_Material_04";

		public const string MASTER_PROPS_MATERIAL_04_OUTLINE = "Master_Props_Material_04_Outline";

		public const string MASTER_RETREAT_MATERIAL_01 = "Master_Retreat_Material_01";

		public const string MASTER_RETREAT_MATERIAL_01_OUTLINE = "Master_Retreat_Material_01_Outline";

		public const string MASTER_RETREAT_MATERIAL_02 = "Master_Retreat_Material_02";

		public const string MASTER_RETREAT_MATERIAL_03 = "Master_Retreat_Material_03";

		public const string MASTER_RETREAT_MATERIAL_03_OUTLINE = "Master_Retreat_Material_03_Outline";

		public const string MASTER_RETREAT_MATERIAL_04 = "Master_Retreat_Material_04";

		public const string MASTER_RETREAT_MATERIAL_04_OUTLINE = "Master_Retreat_Material_04_Outline";

		public const string MASTER_RETREAT_MATERIAL_05 = "Master_Retreat_Material_05";

		public const string MASTER_RETREAT_MATERIAL_05_OUTLINE = "Master_Retreat_Material_05_Outline";

		public const string MASTER_SEWER_MATERIAL_01 = "Master_Sewer_Material_01";

		public const string MASTER_SEWER_MATERIAL_02 = "Master_Sewer_Material_02";

		public const string MASTER_SEWER_MATERIAL_02_OUTLINE = "Master_Sewer_Material_02_Outline";

		public const string MASTER_SEWER_MATERIAL_03 = "Master_Sewer_Material_03";

		public const string MASTER_SEWER_MATERIAL_TRANSPARENT_01 = "Master_Sewer_Material_Transparent_01";

		public const string MASTER_GENT_PIPE_MATERIAL_01 = "Master_GentPipe_Material_01";

		public const string MASTER_GENT_PIPE_MATERIAL_01_OFF = "Master_GentPipe_Material_01_Off";

		public const string MASTER_GENT_PIPE_MATERIAL_01_OUTLINE = "Master_GentPipe_Material_01_Outline";

		public const string MASTER_WIDOW_CHAMBER_MATERIAL_01 = "Master_WidowChamber_Material_01";

		public const string MASTER_WIDOW_CHAMBER_MATERIAL_01_HITTABLE = "Master_WidowChamber_Material_01_Hittable";

		public const string MASTER_EMISSION_MATERIAL_01 = "Master_Emission_Material_01";

		public const string MASTER_MATERIAL_01 = "Master_Material_01";

		public const string MASTER_MATERIAL_01_OUTLINE = "Master_Material_01_Outline";

		public const string MASTER_MATERIAL_02 = "Master_Material_02";

		public const string MASTER_MATERIAL_03 = "Master_Material_03";

		public const string MASTER_MATERIAL_03_OUTLINE = "Master_Material_03_Outline";

		public const string MASTER_MATERIAL_04 = "Master_Material_04";

		public const string MASTER_MATERIAL_05 = "Master_Material_05";

		public const string MASTER_MATERIAL_05_OUTLINE = "Master_Material_05_Outline";

		public const string MASTER_MATERIAL_06 = "Master_Material_06";

		public const string MASTER_MATERIAL_06_OUTLINE = "Master_Material_06_Outline";

		public const string MASTER_MATERIAL_07 = "Master_Material_07";

		public const string MASTER_MATERIAL_07_EMISSION = "Master_Material_07_Emission";

		public const string MASTER_MATERIAL_07_EMISSION_OFF = "Master_Material_07_Emission_Off";

		public const string MASTER_MATERIAL_07_OUTLINE = "Master_Material_07_Outline";

		public const string MASTER_MATERIAL_08 = "Master_Material_08";

		public const string MASTER_MATERIAL_09 = "Master_Material_09";

		public const string MASTER_MATERIAL_09_OUTLINE = "Master_Material_09_Outline";

		public const string MASTER_MATERIAL_10 = "Master_Material_10";

		public const string MASTER_MATERIAL_11 = "Master_Material_11";

		public const string MASTER_MATERIAL_12 = "Master_Material_12";

		public const string MASTER_MATERIAL_13 = "Master_Material_13";

		public const string MASTER_MATERIAL_13_EMISSION = "Master_Material_13_Emission";

		public const string MASTER_MATERIAL_14 = "Master_Material_14";

		public const string MASTER_MATERIAL_14_EMISSION = "Master_Material_14_Emission";

		public const string MASTER_MATERIAL_14_EMISSION_OFF = "Master_Material_14_Emission_Off";

		public const string MASTER_MATERIAL_14_OUTLINE = "Master_Material_14_Outline";

		public const string MASTER_MATERIAL_15 = "Master_Material_15";

		public const string MASTER_MATERIAL_15_OUTLINE = "Master_Material_15_Outline";

		public const string MASTER_MATERIAL_16 = "Master_Material_16";

		public const string MASTER_MATERIAL_16_OUTLINE = "Master_Material_16_Outline";

		public const string MASTER_MATERIAL_17 = "Master_Material_17";

		public const string MASTER_MATERIAL_17_OUTLINE = "Master_Material_17_Outline";

		public const string MASTER_MATERIAL_18 = "Master_Material_18";

		public const string MASTER_MATERIAL_18_OUTLINE = "Master_Material_18_Outline";

		public const string MASTER_MATERIAL_19 = "Master_Material_19";

		public const string MASTER_MATERIAL_19_OUTLINE = "Master_Material_19_Outline";

		public const string MASTER_MATERIAL_20 = "Master_Material_20";

		public const string MASTER_MATERIAL_20_OUTLINE = "Master_Material_20_Outline";

		public const string MASTER_MATERIAL_21 = "Master_Material_21";

		public const string MASTER_MATERIAL_21_OUTLINE = "Master_Material_21_Outline";

		public const string MASTER_MATERIAL_22 = "Master_Material_22";

		public const string MASTER_MATERIAL_23 = "Master_Material_23";

		public const string MASTER_MATERIAL_23_OUTLINE = "Master_Material_23_Outline";

		public const string MASTER_SEASONAL_MATERIAL_01 = "Master_Seasonal_Material_01";

		public const string MASTER_SEASONAL_MATERIAL_01_CHARACTER = "Master_Seasonal_Material_01_Character";

		public const string MASTER_SEASONAL_MATERIAL_01_EMISSION = "Master_Seasonal_Material_01_Emission";

		public const string MASTER_SEASONAL_MATERIAL_01_OUTLINE = "Master_Seasonal_Material_01_Outline";

		public const string PARTCILE_INK_MATERIAL_BLOOD_SPLATTER_01 = "Partcile_Ink_Material_BloodSplatter_01";

		public const string PARTCILE_INK_MATERIAL_BLOOD_SPLATTER_02 = "Partcile_Ink_Material_BloodSplatter_02";

		public const string PARTICLE_INK_MATERIAL_ATTACK_SWIPE = "Particle_Ink_Material_Attack_Swipe";

		public const string PARTICLE_INK_MATERIAL_FULL_DISOLVE = "Particle_Ink_Material_FullDisolve";

		public const string PARTICLE_INK_MATERIAL_SPLASH = "Particle_Ink_Material_Splash";

		public const string PARTICLE_INK_MATERIAL_SPLASH_COLOR = "Particle_Ink_Material_Splash_Color";

		public const string PARTICLE_INK_MATERIAL_SPLASH_INVERTED = "Particle_Ink_Material_Splash_Inverted";

		public const string PARTICLE_INK_MATERIAL_SPLASH_SPECKS = "Particle_Ink_Material_Splash_Specks";

		public const string PARTICLE_INK_MATERIAL_SPLASH_THICK = "Particle_Ink_Material_Splash_Thick";

		public const string PARTICLE_INK_MATERIAL_SPLASH_VIOLENT = "Particle_Ink_Material_Splash_Violent";

		public const string PARTICLE_INK_MATERIAL_SPLASH_VIOLENT_COLOR = "Particle_Ink_Material_Splash_Violent_Color";

		public const string PARTICLE_INK_MATERIAL_TRAIL = "Particle_Ink_Material_Trail";

		public const string PARTICLE_INK_MATERIAL_TRAIL_BLOB = "Particle_Ink_Material_Trail_Blob";

		public const string PARTICLE_INK_MATERIAL_TRAIL_BLOB_COLOR = "Particle_Ink_Material_Trail_Blob_Color";

		public const string PARTICLE_INK_MATERIAL_TRAIL_COLOR = "Particle_Ink_Material_Trail_Color";

		public const string PARTICLE_INK_MTAERIAL_ATTACK_SPLAT = "Particle_Ink_Mtaerial_Attack_Splat";

		public const string PARTICLE_INK_MTAERIAL_ATTACK_SPLAT_COLOR = "Particle_Ink_Mtaerial_Attack_Splat_Color";

		public const string FISHER_DEFAULT = "Fisher_Default";

		public const string INK_BULB_DEFAULT = "InkBulb_Default";

		public const string INK_DEMON_DEFAULT = "InkDemon_Default";

		public const string INK_WIDOW_DEFAULT = "InkWidow_Default";

		public const string INK_WIDOW_TEETH = "InkWidow_Teeth";

		public const string KEEPER_DEFAULT = "Keeper_Default";

		public const string KEEPER_ELITE = "Keeper_Elite";

		public const string KING_WIDOW_DEFAULT = "KingWidow_Default";

		public const string KING_WIDOW_TEETH = "KingWidow_Teeth";

		public const string LOST_ONES_F_DEFAULT = "LostOnesF_Default";

		public const string LOST_ONES_F_DEFAULT_OUTLINE = "LostOnesF_Default_Outline";

		public const string LOST_ONES_F_NO_EYES = "LostOnesF_NoEyes";

		public const string LOST_ONES_F_NO_EYES_OUTLINE = "LostOnesF_NoEyes_Outline";

		public const string LOST_ONES_DEFAULT = "LostOnes_Default";

		public const string LOST_ONES_DEFAULT_OUTLINE = "LostOnes_Default_Outline";

		public const string LOST_ONES_GUITAR = "LostOnes_Guitar";

		public const string LOST_ONES_NO_EYES = "LostOnes_NoEyes";

		public const string LOST_ONES_NO_EYES_OUTLINE = "LostOnes_NoEyes_Outline";

		public const string LOST_ONE_SEASONAL_DEFAULT = "LostOne_Seasonal_Default";

		public const string LURKER_DEFAULT = "Lurker_Default";

		public const string LURKER_DEFAULT_OUTLINE = "Lurker_Default_Outline";

		public const string PIPER_DEFAULT = "Piper_Default";

		public const string PIPER_WRENCH = "Piper_Wrench";

		public const string SEARCHER_DEFAULT = "Searcher_Default";

		public const string SEARCHER_DEFAULT_OUTLINE = "Searcher_Default_Outline";

		public const string SEARCHER_INKTRAIL = "Searcher_Inktrail";

		public const string SHIP_AHOY_DEFAULT = "ShipAhoy_Default";

		public const string SHIP_AHOY_GUTS_DEFAULT = "ShipAhoy_Guts_Default";

		public const string SLICER_DEFAULT = "Slicer_Default";

		public const string SLICER_DEFAULT_EMISSION = "Slicer_Default_Emission";

		public const string STRIKER_DEFAULT = "Striker_Default";

		public const string ALICE_ANGEL_DEFAULT = "AliceAngel_Default";

		public const string ALICE_ANGEL_DEFAULT_OUTLINE = "AliceAngel_Default_Outline";

		public const string ALLISON_DEFAULT = "Allison_Default";

		public const string ALLISON_SWORD_DEFAULT = "Allison_Sword_Default";

		public const string BENDY_FACE_DEFAULT = "BendyFace_Default";

		public const string BENDY_FACE_SAD = "BendyFace_Sad";

		public const string BENDY_FACE_WIAS = "BendyFace_WIAS";

		public const string BENDY_HEAD_DEFAULT = "BendyHead_Default";

		public const string BENDY_HEAD_DEFAULT_OUTLINE = "BendyHead_Default_Outline";

		public const string BENDY_DEFAULT = "Bendy_Default";

		public const string BENDY_DEFAULT_OUTLINE = "Bendy_Default_Outline";

		public const string BERTRUM_DEFAULT = "Bertrum_Default";

		public const string BETTY_DEFAULT = "Betty_Default";

		public const string BETTY_DEFAULT_OUTLINE = "Betty_Default_Outline";

		public const string BORIS_DEFAULT = "Boris_Default";

		public const string HEIDI_DEFAULT = "Heidi_Default";

		public const string HEIDI_DEFAULT_OUTLINE = "Heidi_Default_Outline";

		public const string HENRY_DEFAULT = "Henry_Default";

		public const string JOEY_DREW_DEFAULT = "JoeyDrew_Default";

		public const string JOEY_DREW_DEFAULT_OUTLINE = "JoeyDrew_Default_Outline";

		public const string PORTER_DEFAULT = "Porter_Default";

		public const string PORTER_DEFAULT_OUTLINE = "Porter_Default_Outline";

		public const string SAMMY_DEFAULT = "Sammy_Default";

		public const string TOM_DEFAULT = "Tom_Default";

		public const string TOM_DEFAULT_OUTLINE = "Tom_Default_Outline";

		public const string WILSON_DEFAULT = "Wilson_Default";

		public const string WILSON_DEFAULT_OUTLINE = "Wilson_Default_Outline";

		public const string WILSON_HUMAN_DEFAULT = "Wilson_Human_Default";

		public const string WILSON_HUMAN_DEFAULT_OUTLINE = "Wilson_Human_Default_Outline";

		public const string AUDREY_HUMAN = "AudreyHuman";

		public const string AUDREY_INK = "AudreyInk";

		public const string AUDREY_INK_HAND_INTRO = "AudreyInkHand_Intro";

		public const string AUDREY_INK_FIRST_PERSON = "AudreyInk_FirstPerson";

		public const string AUDREY_INK_UI = "AudreyInk_UI";

		public const string AUDREY_INSANE = "AudreyInsane";

		public const string BEAST_BENDY_DEFAULT = "BeastBendy_Default";

		public const string INK_CROW_DEFAULT = "InkCrow_Default";

		public const string FISH_DEFAULT = "Fish_Default";

		public const string INK_MACHINE_MAT = "InkMachine_Mat";

		public const string LURKER_HEART_BASIC = "LurkerHeart_Basic";

		public const string LURKER_HEART_BASIC_OUTLINE = "LurkerHeart_Basic_Outline";

		public const string LURKER_HEART_DEFAUL = "LurkerHeart_Defaul";

		public const string MASTER_ARCH_GATE_PEN_MATERIAL_01 = "Master_ArchGate_Pen_Material_01";

		public const string ARCH_GATE_BENDY_MATERIAL = "ArchGate_Bendy_Material";

		public const string ARCH_GATE_DIRT_MATERIAL = "ArchGate_Dirt_Material";

		public const string ARCHGATE_LOGO_MATERIAL = "Archgate_Logo_Material";

		public const string ARCH_GATE_OPENING_CELL_MATERIAL = "ArchGate_OpeningCell_Material";

		public const string ARCH_GATE_SCREAM_MATERIAL = "ArchGate_Scream_Material";

		public const string BENDY_FACE_REAL_WORLD_DEFAULT = "BendyFace_RealWorld_Default";

		public const string BENDY_HEAD_REAL_WORLD_DEFAULT = "BendyHead_RealWorld_Default";

		public const string BENDY_REAL_WORLD_DEFAULT = "Bendy_RealWorld_Default";

		public const string AUDREY_INK_HAND = "AudreyInkHand";

		public const string AUDREY_INK_HAND_UI = "AudreyInkHand_UI";
	}

	public static class Shaders
	{
		public const string JDS_ADDITIVE = "JDS/Additive";

		public const string JDS_ALPHA_MASKED = "JDS/Alpha Masked";

		public const string HIDDEN_OLD_GLASS = "Hidden/Old/Glass";

		public const string JDSFX_BENDY_INK_EFFECT = "JDS/FX/Bendy Ink Effect";

		public const string JDS_TOON_CHARACTER_SHADER = "JDS/Toon Character Shader";

		public const string JDSFX_CLOUDS = "JDS/FX/Clouds";

		public const string JDS_CUSTOM_MOTION_VECTOR_PASS = "JDS/CustomMotionVectorPass";

		public const string JDS_DARKNESS_VOLUME = "JDS/Darkness Volume";

		public const string JDS_DIFFUSE = "JDS/Diffuse";

		public const string JDS_DIFFUSE_DISSOLVE = "JDS/Diffuse Dissolve";

		public const string JDS_DIFFUSE_HIGHLIGHT = "JDS/DiffuseHighlight";

		public const string JDS_DIFFUSE_HIGHLIGHT_INVISIBLE = "JDS/DiffuseHighlightInvisible";

		public const string JDS_DIFFUSE_HITTABLE = "JDS/DiffuseHittable";

		public const string JDS_DIFFUSE_OUTLINE = "JDS/DiffuseOutline";

		public const string JDS_PARTICLES_DUST = "JDS/Particles/Dust";

		public const string JDS_PARTICLES_ELECTRICITY = "JDS/Particles/Electricity";

		public const string JDS_EMISSIVE = "JDS/Emissive";

		public const string JDS_EMISSIVE_HIGHLIGHT = "JDS/EmissiveHighlight";

		public const string JDS_EMISSIVE_TRANSPARENT = "JDS/EmissiveTransparent";

		public const string JDS_FABRIC_WAVE = "JDS/Fabric Wave";

		public const string JDS_FAKE_REFLECTION = "JDS/Fake Reflection";

		public const string JDS_PARTICLES_FPS_STANDARD_UNLIT = "JDS/Particles/FPS Standard Unlit";

		public const string JDSFPS_TOON_CHARACTER_SHADER = "JDS/FPS Toon Character Shader";

		public const string JDS_GLASS = "JDS/Glass";

		public const string JDS_GLASS_CARTOON = "JDS/GlassCartoon";

		public const string JDS_GLASS_SHADOW_CASTER = "JDS/Glass Shadow Caster";

		public const string JDS_GLASS_TOON = "JDS/GlassToon";

		public const string JDS_HIGHLIGHT = "JDS/Highlight";

		public const string JDSFX_SPECIAL_INK_DEMON_HOLE_TUNNEL = "JDS/FX/Special/Ink Demon Hole Tunnel";

		public const string JDSFX_SPECIAL_INK_DEMON_HOLE_CUT = "JDS/FX/Special/Ink Demon Hole Cut";

		public const string JDS_INK_COLOR = "JDS/InkColor";

		public const string JDSFX_SPECIAL_INK_DEMON_FOOTPRINT = "JDS/FX/Special/Ink Demon Footprint";

		public const string JDS_INK_INK_FLOW = "JDS/Ink/Ink Flow";

		public const string JDS_INK_INK_SPLASH_SHADER = "JDS/Ink/Ink Splash Shader";

		public const string JDS_INK_INK_SPLASH_SHADER_COLOR = "JDS/Ink/Ink Splash Shader Color";

		public const string JDS_INK_SPECIFIC_INK_BLOOD_SPATTER_SHADER = "JDS/Ink/Specific/Ink Blood Spatter Shader";

		public const string JDSFX_SPECIAL_INVISIBLE_HIGHLIGHT = "JDS/FX/Special/InvisibleHighlight";

		public const string JDSFX_SPECIAL_SHEET = "JDS/FX/Special/Sheet";

		public const string SKYBOX_SKYBOX_COLOR = "Skybox/Skybox Color";

		public const string JDSFX_SMOKE = "JDS/FX/Smoke";

		public const string JDS_STANDARD_HIGHLIGHT = "JDS/Standard Highlight";

		public const string JDS_STANDARD_SPECULAR_HIGHLIGHT = "JDS/Standard Specular Highlight";

		public const string JDS_STANDARD_SPECULAR = "JDS/Standard Specular";

		public const string JDS_STANDARD = "JDS/Standard";

		public const string JDS_PARTICLES_TAKEDOWN_DISSOLVE_PARTICLES = "JDS/Particles/Takedown Dissolve Particles";

		public const string JDS_TRANSPARENT = "JDS/Transparent";

		public const string JDS_TRANSPARENT_MESH = "JDS/TransparentMesh";

		public const string JDS_TRANSPARENT_MESH_NON_SPEC = "JDS/TransparentMeshNonSpec";

		public const string JDS_TRANSPARENT_MESH_NORMAL = "JDS/TransparentMeshNormal";

		public const string JDS_TRANSPARENT_MESH_NORMAL_ONE_SIDE = "JDS/TransparentMeshNormalOneSide";

		public const string JDS_TRANSPARENT_ONE_SIDE = "JDS/TransparentOneSide";

		public const string JDSUI_DISOLVE = "JDS/UI/Disolve";

		public const string EDITOR_UNLIT_TRANSPARENT = "Editor/UnlitTransparent";

		public const string JDS_WATER = "JDS/Water";

		public const string JDSFX_WATER_SPLASH = "JDS/FX/WaterSplash";

		public const string JDSUI_BASE = "JDS/UI/Base";

		public const string JDSUI_BLOOM = "JDS/UI/Bloom";

		public const string JDSUI_BLUR = "JDS/UI/Blur";

		public const string JDS_POST_PROCESS_DAMAGE_EFFECT = "JDS/Post Process/Damage Effect";

		public const string JDS_POST_PROCESS_PLAYER_DEATH = "JDS/Post Process/Player Death";

		public const string JDS_POST_PROCESS_GAIN_POWER_EFFECT = "JDS/Post Process/Gain Power Effect";

		public const string JDS_POST_PROCESS_GROUND_FOG = "JDS/Post Process/Ground Fog";

		public const string JDS_POST_PROCESS_INK_DEMON_EFFECT = "JDS/Post Process/Ink Demon Effect";

		public const string JDS_POST_PROCESS_TAKEDOWN = "JDS/Post Process/Takedown";

		public const string JDS_POST_PROCESS_TELEPORT = "JDS/Post Process/Teleport";

		public const string JDS_POST_PROCESS_VISION_EFFECT = "JDS/Post Process/Vision Effect";
	}

	public static class Textures
	{
		public const string CURSORICON_01 = "cursor_icon_01";

		public const string CURSORICON_02 = "cursor_icon_02";

		public const string AUDREY_SIGIL_BANISH = "Audrey_Sigil_Banish";

		public const string AUDREY_SIGIL_FAST_TRAVEL = "Audrey_Sigil_FastTravel";

		public const string AUDREY_SIGIL_FLOW = "Audrey_Sigil_Flow";
	}

	public static class Sprites
	{
		public class Lookup
		{
		}

		public class Lists
		{
		}
	}

	public static class ScriptableObjects
	{
		public const string ABILITY_DATA = "Abilities/AbilityData";

		public const string AUDIO_LOG_CLIP_GROUP = "AudioLogs/AudioLogClipGroup";

		public const string AUTOSAVE_SPRITE_DATA = "Autosave/AutosaveSpriteData";

		public const string UI_INFO_POPUP_BANISH = "InfoPopup/UIInfoPopup_Banish";

		public const string UI_INFO_POPUP_FAST_TRAVEL = "InfoPopup/UIInfoPopup_FastTravel";

		public const string UI_INFO_POPUP_FLOW = "InfoPopup/UIInfoPopup_Flow";

		public const string UI_INFO_POPUP_FOOD = "InfoPopup/UIInfoPopup_Food";

		public const string UI_INFO_POPUP_GENT_LOCK = "InfoPopup/UIInfoPopup_GentLock";

		public const string UI_INFO_POPUP_GENT_PIPE = "InfoPopup/UIInfoPopup_GentPipe";

		public const string UI_INFO_POPUP_INK_DEMON = "InfoPopup/UIInfoPopup_InkDemon";

		public const string UI_INFO_POPUP_SHOCK_PIPE = "InfoPopup/UIInfoPopup_ShockPipe";

		public const string UI_INFO_POPUP_STEALTH = "InfoPopup/UIInfoPopup_Stealth";

		public const string UI_INFO_POPUP_STUN_PIPE = "InfoPopup/UIInfoPopup_StunPipe";

		public const string UI_INFO_POPUP_SUBWAY = "InfoPopup/UIInfoPopup_Subway";

		public const string I_2_LOCALIZATION_GROUP = "Localization/I2LocalizationGroup";

		public const string CONTROLLER_INPUT_MAPPING_GRAPHICS = "MappingGraphics/ControllerInputMappingGraphics";

		public const string UPGRADES_DATA = "UpgradesData/UpgradesData";

		public const string WEAPON_UPGRADE_DATA = "Weapons/WeaponUpgradeData";

		public const string UI_ICON_MEMORY_BALL = "Icon/Memories/UIIcon_Memory_Ball";

		public const string UI_ICON_MEMORY_CLOCK = "Icon/Memories/UIIcon_Memory_Clock";

		public const string UI_ICON_MEMORY_CRAYONS = "Icon/Memories/UIIcon_Memory_Crayons";

		public const string UI_ICON_MEMORY_CUP = "Icon/Memories/UIIcon_Memory_Cup";

		public const string UI_ICON_MEMORY_DUCK = "Icon/Memories/UIIcon_Memory_Duck";

		public const string UI_ICON_MEMORY_HAT = "Icon/Memories/UIIcon_Memory_Hat";

		public const string UI_ICON_MEMORY_HOT_DOG = "Icon/Memories/UIIcon_Memory_HotDog";

		public const string UI_ICON_MEMORY_MILK = "Icon/Memories/UIIcon_Memory_Milk";

		public const string UI_ICON_MEMORY_OIL = "Icon/Memories/UIIcon_Memory_Oil";

		public const string UI_ICON_MEMORY_PLANE = "Icon/Memories/UIIcon_Memory_Plane";

		public const string LOST_ONE_AMOK_AUDIO = "Audio/LostOne_Amok_Audio";

		public const string LOST_ONE_AMOK_FOLLOWER_FEMALE_AUDIO = "Audio/LostOne_Amok_Follower_Female_Audio";

		public const string LOST_ONE_AMOK_FOLLOWER_MALE_AUDIO = "Audio/LostOne_Amok_Follower_Male_Audio";

		public const string LOST_ONE_FEMALE_AUDIO = "Audio/LostOne_Female_Audio";

		public const string LOST_ONE_FEMALE_MELANIE_AUDIO = "Audio/LostOne_Female_Melanie_Audio";

		public const string LOST_ONE_FEMALE_SILENT_AUDIO = "Audio/LostOne_Female_Silent_Audio";

		public const string LOST_ONE_MALE_AUDIO = "Audio/LostOne_Male_Audio";

		public const string LOST_ONE_MALE_HAL_AUDIO = "Audio/LostOne_Male_Hal_Audio";

		public const string LOST_ONE_MALE_MATT_AUDIO = "Audio/LostOne_Male_Matt_Audio";

		public const string LOST_ONE_MALE_SCOTT_AUDIO = "Audio/LostOne_Male_Scott_Audio";

		public const string LOST_ONE_MALE_SILENT_AUDIO = "Audio/LostOne_Male_Silent_Audio";

		public const string LOST_ONE_ALL_ENEMY_SELECTOR = "EnemySelectors/LostOne_All_EnemySelector";

		public const string LOST_ONE_AMOK_FOLLOWER_ENEMY_SELECTOR = "EnemySelectors/LostOne_Amok_Follower_EnemySelector";

		public const string LOST_ONE_BAG_HEAD_ENEMY_SELECTOR = "EnemySelectors/LostOne_BagHead_EnemySelector";

		public const string LOST_ONE_COLOR_ENEMY_SELECTOR = "EnemySelectors/LostOne_Color_EnemySelector";

		public const string LOST_ONE_ENEMY_SELECTOR = "EnemySelectors/LostOne_EnemySelector";

		public const string LOST_ONE_FEMALE_ENEMY_SELECTOR = "EnemySelectors/LostOne_Female_EnemySelector";

		public const string LOST_ONE_FEMALE_MELANIE = "EnemySelectors/LostOne_Female_Melanie";

		public const string LOST_ONE_HAMBUSH = "EnemySelectors/LostOne_Hambush";

		public const string LOST_ONE_MALE_ENEMY_SELECTOR = "EnemySelectors/LostOne_Male_EnemySelector";

		public const string LOST_ONE_MALE_HAL_VICIOUS = "EnemySelectors/LostOne_Male_Hal_Vicious";

		public const string LOST_ONE_MALE_MATT = "EnemySelectors/LostOne_Male_Matt";

		public const string LOST_ONE_MALE_SCOTT = "EnemySelectors/LostOne_Male_Scott";

		public const string UI_ICON_BATTERY_CASING_LARGE = "Icon/Collectables/Large/UIIcon_BatteryCasing_Large";

		public const string UI_ICON_BATTERY_LARGE = "Icon/Collectables/Large/UIIcon_Battery_Large";

		public const string UI_ICON_CARD_LARGE = "Icon/Collectables/Large/UIIcon_Card_Large";

		public const string UI_ICON_FOOD_LARGE = "Icon/Collectables/Large/UIIcon_Food_Large";

		public const string UI_ICON_KEY_LARGE = "Icon/Collectables/Large/UIIcon_Key_Large";

		public const string UI_ICON_PARTS_LARGE = "Icon/Collectables/Large/UIIcon_Parts_Large";

		public const string UI_ICON_SLUG_LARGE = "Icon/Collectables/Large/UIIcon_Slug_Large";

		public const string UI_ICON_TOOLKIT_LARGE = "Icon/Collectables/Large/UIIcon_Toolkit_Large";

		public const string UI_ICON_BATTERY = "Icon/Collectables/Small/UIIcon_Battery";

		public const string UI_ICON_BATTERY_CASING = "Icon/Collectables/Small/UIIcon_BatteryCasing";

		public const string UI_ICON_CARD = "Icon/Collectables/Small/UIIcon_Card";

		public const string UI_ICON_FOOD = "Icon/Collectables/Small/UIIcon_Food";

		public const string UI_ICON_KEY = "Icon/Collectables/Small/UIIcon_Key";

		public const string UI_ICON_PARTS = "Icon/Collectables/Small/UIIcon_Parts";

		public const string UI_ICON_SLUG = "Icon/Collectables/Small/UIIcon_Slug";

		public const string UI_ICON_TOOLKIT = "Icon/Collectables/Small/UIIcon_Toolkit";
	}
}
