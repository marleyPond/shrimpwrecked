using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public static class ColorHelper {

    //Button Aliases
    static public string DPAD_X_AXIS = "Horizontal_D";
    static public string DPAD_Y_AXIS = "Vertical_D";        //NOT IMPLEMENTED FOR THIS PROJECT
    static public string LEFT_STICK_X = "Horizontal_L";
    static public string LEFT_STICK_Y = "Vertical_L";
    static public string RIGHT_STICK_X = "Horizontal_R";
    static public string RIGHT_STICK_Y = "Vertical_R";
    static public string A = "Button_A";
    static public string B = "Button_B";
    static public string X = "Button_X";
    static public string Y = "Button_Y";
    static public string START = "Button_Start";
    static public string BACK = "Button_Back";
    static public string LEFT_BUMPER = "Bumper_L";
    static public string RIGHT_BUMPER = "Bumper_R";
    static public string LEFT_TRIGGER = "Trigger_L";
    static public string RIGHT_TRIGGER = "Trigger_R";
    static public string STICK_L = "Button_Stick_L";
    static public string STICK_R = "Button_Stick_R";

    //data
    static string[] sweetSuffix = new string[]{ "Illness", "Fortune", "Bludgeoning", "Nonchalance", "Millieu", "Studiousness", "Calamity",
                                "Jealousy", "Ferventness", "Vehemence", "Cowardice", "Order", "Camaraderie", "Maleficence",
                                "Indulgence", "Capriciousness", "Vulgarity", "Pondering", "Madness", "Garrishness",
                                "Fealty", "Flummoxing", "Beguiling", "Perusing", "Preponderance", "Frost", "Lamenting",
                                "Incredulity", "Delusion", "Slaying", "Doom", "Daisies", "Dread", "Stumps", "Stoicism",
                                "Vagueness", "Parades", "Puppies", "Bees", "Exclamation", "Climax", "Crawling", "Justice",
                                "Obscurity", "Ravaging", "Visions", "Blood", "Pillows", "Fists", "Halley", "Dawn",
                                "Failing", "Pensiveness", "Meloncholy", "Bemoaning", "Foresight", "Rambunctiousness",
                                "Tranquility", "Infinity", "Knowing", "Levity", "Silence", "Frustration", "Malice",
                                "Remarkability", "Irony", "Ambience", "Gears", "Lurking", "Bogs", "Meat", "Phantasmagoria",
                                "Witchcraft", "Bonding", "Scrutiny", "Screams", "Instrumentation",
                                "Legend", "Primordia", "Whispers", "Possession", "Sadness", "Ears", "Force", "Unease",
                                "Fervor" , "Gile", "Vomit", "Beginnings", "Teeth", "Seething", "Gnawing",
                                "Yes", "Tangerines", "Guts", "Taste", "Trembling", "Owning", "Fate", "Gravity",
                                "Evisceration", "Shame", "Turning", "Tainting", "Ambiguity", "Mustaches",
                                "Viscera", "Garbage", "Bellowing", "Shame"};

    static string[] sweetPrefix = new string[]{ "Fell", "Uncomfortable", "Awkward", "Blistering", "Dreadful", "Living", "Horrific",
                                "Uncanny", "Idle", "Sharing", "Killing", "Hissing", "Fishy", "Forgotten",
                                "Sentient", "Balmy", "Vibrating", "Clandestine", "Jovial", "Temporary",
                                "Catabolic", "Legendary", "Impossible", "Intense", "Pestilent", "Cowardly",
                                "Comsuming", "Vile", "Careful", "Fighting", "Kittens", "Peaches", "Reverance",
                                "Curmudgeonly", "Betraying", "Frightful", "Kaleidoscopic", "Inching",
                                "Contemptuous", "Famous", "Beard-Worthy", "Smitten","Shirking", "Dark",
                                "Shiny", "Luminous", "Excellent", "Telling", "Growling", "Clever", "Bog",
                                "Lofty", "Darling", "Acceptable", "Beguiling", "Rotting", "Fermented",
                                "Rainbow", "Super", "Imaginary", "Caring", "Subterfuge", "Halcyon",
                                "Karmic", "Callious", "Intimidating", "Fresh", "Caring", "Fidgeting", "Ephemeral",
                                "Debonair", "Rickety", "Hated", "Bone", "Begrudging", "Chipper",
                                "Tense", "Crusty", "Dramatic", "Dancing" , "Shy", "Frothing", "Menacing",
                                "Karaoke", "Fierce", "Ravaging", "Unstable", "Fleeting", "Quantum",
                                "Psychedelic", "Blasphemous", "Neverending", "Feverish", "Lucifugous"  };

    
    //COLOR SET
    static Dictionary<string, Color> colors = new Dictionary<string, Color>
    {
        { "WHITE",new Color (1.0f, 1.0f, 1.0f) },           { "BLACK", new Color (0.0f, 0.0f, 0.0f) },
        { "GRAY_DARK", new Color (0.2f, 0.2f, 0.2f)},       { "GRAY_MEDIUM", new Color (0.5f, 0.5f, 0.5f)},
        { "GRAY_LIGHT", new Color (0.7f, 0.7f, 0.7f)},      { "BLUE_BROODING",new Color (0.0f, 0.25f, 0.5f)},
        { "BLUE", new Color(0.0f, 0.0f, 0.5f )},            { "AQUA", new Color(0.0f, 0.6f, 0.7f )},
        { "BLUE_SEA", new Color(0.3f, 0.5f, 0.8f)},         { "GREEN_BROODING", new Color (0.0f, 0.5f, 0.2f)},
        { "GREEN_LEAF", new Color (0.0f, 0.6f, 0.4f)},      { "BROWN", new Color(0.45f, 0.35f, 0.0f)},
        { "LIGHT_BROWN", new Color(0.35f, 0.25f, 0.0f)},    { "GREEN_BRIGHT", new Color(0.0f, 0.5f, 0.0f)},
        { "GREEN_SLURM", new Color(0.0f, 0.7f, 0.0f)},      { "GREEN_RADIOACTIVE", new Color (0.0f, 1.0f, 0.0f)},
        { "GREEN_UNDERGROWTH", new Color (0.3f, 0.3f, 0.2f)},   { "GREEN_GROWTH", new Color(0.3f, 0.4f, 0.3f )},
        { "ORANGE_LIGHT", new Color (0.7f, 0.6f, 0.3f)},    { "ORANGE_UBER", new Color (0.8f, 0.5f, 0.2f)},
        { "PEACH_GLOOMY", new Color (0.4f, 0.3f, 0.3f)},    { "PINK_DARK", new Color(0.3f, 0.2f, 0.3f )},
        { "PINK_HOT", new Color(0.8f, 0.3f, 0.7f)},         { "PINK_DULL", new Color (0.8f, 0.6f, 0.6f)},
        { "PINK_FLUFF", new Color (0.8f, 0.6f, 0.7f)},      { "PURPLE_GRAPE",new Color (0.2f, 0.2f, 0.3f)},
        { "PURPLE_PINK", new Color( 0.4f, 0.3f, 0.4f)},     { "PURPLE_LIGHT", new Color(0.3f, 0.3f, 0.4f )},
        { "RED_DRIED_BLOOD", new Color (0.3f, 0.0f, 0.0f)}, { "RED_DULL", new Color (0.5f, 0.0f, 0.0f)},
        { "RED_CHERRY", new Color(0.8f, 0.0f, 0.0f)},       { "RED_OBNOXIOUS", new Color(1.0f, 0.0f, 0.0f)},
        { "RED_JAM", new Color(0.3f, 0.2f, 0.2f )},         { "YELLOW", new Color (0.7f, 0.7f, 0.1f)},
        { "DARK_BROWN", new Color( 0.2f, 0.15f, 0.0f)},     { "PEACH_DULL", new Color(0.8f, 0.5f, 0.55f)},
        { "RED_PLEASANT", new Color ( 0.45f, 0.25f, 0.25f) }
    };//eof Colors

    public static string[] hull_colors = new string[]
    {
           "AQUA", "BLUE_SEA", "GREEN_LEAF", "GREEN_GROWTH", "ORANGE_LIGHT", "PURPLE_PINK", "RED_PLEASANT"
    };

    public static List<string> color_keys_all = new List<string>(){
        "WHITE","BLACK","GRAY_DARK", "GRAY_MEDIUM",
        "GRAY_LIGHT","BLUE_BROODING","BLUE", "AQUA", "BLUE_SEA", "GREEN_BROODING", "GREEN_LEAF",
        "BROWN", "LIGHT_BROWN", "GREEN_BRIGHT", "GREEN_SLURM", "GREEN_RADIOACTIVE", "GREEN_UNDERGROWTH",
        "GREEN_GROWTH", "ORANGE_LIGHT", "ORANGE_UBER", "PEACH_GLOOMY", "PINK_DARK", "PINK_HOT", "PINK_DULL",
        "PINK_FLUFF", "PURPLE_GRAPE", "PURPLE_PINK", "PURPLE_LIGHT", "RED_DRIED_BLOOD", "RED_DULL", "RED_CHERRY",
        "RED_OBNOXIOUS", "RED_JAM", "YELLOW", "DARK_BROWN", "PEACH_DULL"
    };

    public static Color GetColorRandom() {
        return colors[color_keys_all[Random.Range(0, color_keys_all.Count)]];
    }

    public static Color GetColorRandomNoShades()
    {
        return colors[color_keys_all[Random.Range(5, color_keys_all.Count)]];
    }

    public static Color GetColorWhite() {
        return colors["WHITE"];
    }

    public static Color GetColorBlack() {
        return colors["Black"];
    }

    public static Color GetColorGrayDark() {
        return colors["GRAY_DARK"];
    }

    public static Color GetColorGrayMeduim() {
        return colors["GRAY_MEDUIM"];
    }

    public static Color GetColorGrayLight() {
        return colors["GRAY_LIGHT"];
    }

    public static Color GetColorBlueBrooding() {
        return colors["BLUE_BROODING"];
    }

    public static Color GetColorBlue() {
        return colors["BLUE"];
    }

    public static Color GetColorAqua() {
        return colors["AQUA"];
    }

    public static Color GetColorBlueSea() {
        return colors["BLUE_SEA"];
    }

    public static Color GetColorBrown() {
        return colors["BROWN"];
    }

    public static Color GetColorLightBrown() {
        return colors["LIGHT_BROWN"];
    }

    public static Color GetColorDarkBrown() {
        return colors["DARK_BROWN"];
    }

    public static Color GetColorGreenBrooding() {
        return colors["GREEN_BROODING"];
    }

    public static Color GetColorGreenLeaf() {
        return colors["GREEN_LEAF"];
    }

    public static Color GetColorGreenBright() {
        return colors["GREEN_BRIGHT"];
    }

    public static Color GetColorGreenSlurm() {
        return colors["GREEN_SLURM"];
    }

    public static Color GetColorGreenRadioactive() {
        return colors["GREEN_RADIOACTIVE"];
    }

    public static Color GetColorGreenUndergrowth() {
        return colors["GREEN_UNDERGROWTH"];
    }

    public static Color GetColorGreenGrowth() {
        return colors["GREEN_GROWTH"];
    }

    public static Color GetColorOrangeLight() {
        return colors["ORANGE_LIGHT"];
    }

    public static Color GetColorOrangeUber() {
        return colors["ORANGE_UBER"];
    }

    public static Color GetColorPeachGloomy() {
        return colors["PEACH_GLOOMY"];
    }

    public static Color GetColorPeachDull() {
        return colors["PEACH_DULL"];
    }

    public static Color GetColorPinkDark() {
        return colors["PINK_DARK"];
    }

    public static Color GetColorPinkHot() {
        return colors["PINK_HOT"];
    }

    public static Color GetColorPinkDull() {
        return colors["PINK_DULL"];
    }

    public static Color GetColorPinkFluff() {
        return colors["PINK_FLUFF"];
    }

    public static Color GetColorPurpleGrape() {
        return colors["PURPLE_GRAPE"];
    }

    public static Color GetColorPurplePink() {
        return colors["PURPLE_PINK"];
    }

    public static Color GetColorPurpleLight() {
        return colors["PURPLE_LIGHT"];
    }

    public static Color GetColorRedDriedBlood() {
        return colors["RED_DRIED_BLOOD"];
    }

    public static Color GetColorRedDull() {
        return colors["RED_DRIED_BLOOD"];
    }

    public static Color GetColorRedCherry() {
        return colors["RED_CHERRY"];
    }

    public static Color GetColorRedObnoxious() {
        return colors["RED_OBNOXIOUS"];
    }

    public static Color GetColorRedJam() {
        return colors["RED_JAM"];
    }

    public static Color GetColorYellow() {
        return colors["YELLOW"];
    }

}
