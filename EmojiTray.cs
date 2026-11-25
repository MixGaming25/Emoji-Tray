using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Microsoft.Win32;

[ComVisible(true)]
public class EmojiBridge
{
    public Action<string> OnEmojiPicked;
    public Action OnEscapePressed;

    public void Pick(string emoji)
    {
        if (OnEmojiPicked != null) OnEmojiPicked(emoji);
    }

    public void Close()
    {
        if (OnEscapePressed != null) OnEscapePressed();
    }
}

public class EmojiItem
{
    public string c { get; set; }
    public string k { get; set; }
}

static class Program
{
    static string AppDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TrayEmojiPicker");
    static string RecentFile = Path.Combine(AppDir, "recent.json");
    const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    const string RunValueName = "TrayEmojiPicker";

    static readonly string[] DefaultRecent = new string[]
    {
        Cp(0x1F602),        // 😂
        Cp(0x2764, 0xFE0F), // ❤️
        Cp(0x1F62D),        // 😭
        Cp(0x1F480),        // 💀
        Cp(0x1F525)         // 🔥
    };

    static string Cp(int cp, int cp2 = 0)
    {
        string s = char.ConvertFromUtf32(cp);
        if (cp2 != 0) s += char.ConvertFromUtf32(cp2);
        return s;
    }

    static EmojiItem E(int cp, string keywords, int cp2 = 0)
    {
        return new EmojiItem { c = Cp(cp, cp2), k = keywords.ToLowerInvariant() };
    }

    static List<string> GetRecentEmojis()
    {
        var result = new List<string>();
        var serializer = new JavaScriptSerializer();
        if (File.Exists(RecentFile))
        {
            try
            {
                string raw = File.ReadAllText(RecentFile, Encoding.UTF8);
                var loaded = serializer.Deserialize<List<string>>(raw);
                if (loaded != null)
                {
                    foreach (string val in loaded)
                    {
                        if (!string.IsNullOrWhiteSpace(val) && !result.Contains(val) && result.Count < 5)
                            result.Add(val);
                    }
                }
            }
            catch { }
        }

        foreach (string def in DefaultRecent)
        {
            if (result.Count >= 5) break;
            if (!result.Contains(def)) result.Add(def);
        }
        return result;
    }

    static void SaveRecentEmoji(string emoji)
    {
        if (string.IsNullOrWhiteSpace(emoji)) return;
        if (!Directory.Exists(AppDir)) Directory.CreateDirectory(AppDir);

        var current = new List<string> { emoji };
        foreach (string item in GetRecentEmojis())
        {
            if (item != emoji && current.Count < 5)
                current.Add(item);
        }
        var serializer = new JavaScriptSerializer();
        File.WriteAllText(RecentFile, serializer.Serialize(current), Encoding.UTF8);
    }

    static bool IsStartupEnabled()
    {
        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKeyPath, false))
        {
            return key != null && key.GetValue(RunValueName) != null;
        }
    }

    static void SetStartupEnabled(bool enable)
    {
        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKeyPath, true))
        {
            if (key == null) return;
            if (enable)
                key.SetValue(RunValueName, "\"" + Application.ExecutablePath + "\"");
            else
                key.DeleteValue(RunValueName, false);
        }
    }

    static Icon CreateSmileyIcon()
    {
        using (var bmp = new Bitmap(32, 32))
        using (var g = Graphics.FromImage(bmp))
        using (var borderPen = new Pen(Color.Goldenrod, 2f))
        using (var mouthPen = new Pen(Color.Black, 2.2f))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);
            g.FillEllipse(Brushes.Gold, 2, 2, 28, 28);
            g.DrawEllipse(borderPen, 2, 2, 28, 28);
            g.FillEllipse(Brushes.Black, 10, 10, 4, 5);
            g.FillEllipse(Brushes.Black, 18, 10, 4, 5);
            g.DrawArc(mouthPen, 9, 11, 14, 12, 20, 140);
            return Icon.FromHandle(bmp.GetHicon());
        }
    }

    static List<EmojiItem> BuildCatalog()
    {
        return new List<EmojiItem>
        {
            // Smileys & Emotion
            E(0x1F600, "grinning happy smile joy"),
            E(0x1F603, "grinning big eyes happy smile"),
            E(0x1F604, "grinning smiling eyes happy laugh"),
            E(0x1F601, "beaming grin teeth happy"),
            E(0x1F606, "laughing squint xd haha"),
            E(0x1F605, "sweat smile nervous relief"),
            E(0x1F923, "rofl rolling floor laughing lmao"),
            E(0x1F602, "joy tears laughing crying lol"),
            E(0x1F642, "slightly smiling polite"),
            E(0x1F643, "upside down sarcasm silly"),
            E(0x1F609, "wink flirt joke"),
            E(0x1F60A, "blush warm happy cute"),
            E(0x1F607, "innocent halo angel"),
            E(0x1F970, "smiling hearts love adore"),
            E(0x1F60D, "heart eyes love crush"),
            E(0x1F929, "star struck excited wow"),
            E(0x1F618, "kiss blowing heart love"),
            E(0x1F617, "kissing"),
            E(0x1F61A, "kissing closed eyes"),
            E(0x1F60B, "yum delicious tasty food"),
            E(0x1F61B, "tongue playful"),
            E(0x1F61C, "wink tongue crazy playful"),
            E(0x1F92A, "zany goofy crazy"),
            E(0x1F61D, "squinting tongue"),
            E(0x1F911, "money mouth rich cash"),
            E(0x1F917, "hugging hands warm"),
            E(0x1F92D, "hand over mouth oops giggle"),
            E(0x1F92B, "shushing quiet secret shh"),
            E(0x1F914, "thinking hmm wonder"),
            E(0x1F910, "zipper mouth secret quiet"),
            E(0x1F928, "raised eyebrow skeptic sus"),
            E(0x1F610, "neutral straight face meh"),
            E(0x1F611, "expressionless blank"),
            E(0x1F636, "no mouth silent"),
            E(0x1F60F, "smirk smug"),
            E(0x1F612, "unamused annoyed side eye"),
            E(0x1F644, "rolling eyes whatever"),
            E(0x1F62C, "grimacing awkward yikes"),
            E(0x1F925, "lying pinocchio"),
            E(0x1F60C, "relieved calm peace"),
            E(0x1F614, "pensive sad sorry"),
            E(0x1F62A, "sleepy tired"),
            E(0x1F924, "drooling want hungry"),
            E(0x1F634, "sleeping zzz tired"),
            E(0x1F637, "mask sick covid"),
            E(0x1F912, "thermometer fever sick"),
            E(0x1F915, "head bandage hurt"),
            E(0x1F922, "nauseated gross sick"),
            E(0x1F92E, "vomiting puke gross"),
            E(0x1F927, "sneezing bless you"),
            E(0x1F975, "hot sweating heat"),
            E(0x1F976, "cold freezing ice"),
            E(0x1F974, "woozy drunk dizzy"),
            E(0x1F635, "knocked out dizzy dead"),
            E(0x1F92F, "exploding head mind blown"),
            E(0x1F920, "cowboy hat yeehaw"),
            E(0x1F973, "partying celebration birthday"),
            E(0x1F978, "disguised glasses mustache"),
            E(0x1F60E, "sunglasses cool"),
            E(0x1F913, "nerd glasses geek smart"),
            E(0x1F9D0, "monocle inspect classy"),
            E(0x1F615, "confused unsure"),
            E(0x1F61F, "worried concerned"),
            E(0x1F641, "slightly frowning sad"),
            E(0x2639,  "frowning sad", 0xFE0F),
            E(0x1F62E, "open mouth surprised oh"),
            E(0x1F62F, "hushed surprised"),
            E(0x1F632, "astonished shocked"),
            E(0x1F633, "flushed embarrassed blushing"),
            E(0x1F97A, "pleading puppy eyes please"),
            E(0x1F626, "frowning open mouth"),
            E(0x1F627, "anguished"),
            E(0x1F628, "fearful scared"),
            E(0x1F630, "anxious sweat nervous"),
            E(0x1F625, "sad relieved sweat"),
            E(0x1F622, "crying tear sad"),
            E(0x1F62D, "loudly crying sobbing tears"),
            E(0x1F631, "screaming fear omg"),
            E(0x1F616, "confounded frustrated"),
            E(0x1F623, "persevering struggle"),
            E(0x1F61E, "disappointed sad"),
            E(0x1F613, "downcast sweat stressed"),
            E(0x1F629, "weary tired"),
            E(0x1F62B, "tired exhausted"),
            E(0x1F971, "yawning bored sleepy"),
            E(0x1F624, "triumph steam angry"),
            E(0x1F621, "pouting angry mad rage"),
            E(0x1F620, "angry mad"),
            E(0x1F92C, "cursing symbols swearing mad"),
            E(0x1F608, "smiling horns devil"),
            E(0x1F47F, "angry devil imp"),
            E(0x1F480, "skull dead skeleton rip"),
            E(0x2620,  "skull crossbones pirate danger", 0xFE0F),
            E(0x1F4A9, "poop crap"),
            E(0x1F921, "clown circus"),
            E(0x1F479, "ogre monster"),
            E(0x1F47B, "ghost boo halloween"),
            E(0x1F47D, "alien ufo space"),
            E(0x1F916, "robot bot ai"),
            E(0x1F63A, "grinning cat"),
            E(0x1F638, "grinning cat smiling eyes"),
            E(0x1F639, "joy cat tears"),
            E(0x1F63B, "heart eyes cat"),
            E(0x1F63C, "smirk cat"),
            E(0x1F63D, "kissing cat"),
            E(0x1F640, "weary cat shocked"),
            E(0x1F63F, "crying cat"),
            E(0x1F63E, "pouting cat"),

            // Gestures & People
            E(0x1F44B, "wave hello bye hand"),
            E(0x1F91A, "raised back hand"),
            E(0x1F590, "hand fingers splayed", 0xFE0F),
            E(0x270B,  "raised hand stop"),
            E(0x1F596, "vulcan salute spock"),
            E(0x1F44C, "ok hand perfect"),
            E(0x1F90C, "pinched fingers italian chef"),
            E(0x1F90F, "pinching hand small tiny"),
            E(0x270C,  "victory peace two", 0xFE0F),
            E(0x1F91E, "crossed fingers luck"),
            E(0x1F91F, "love you gesture"),
            E(0x1F918, "sign of the horns rock"),
            E(0x1F919, "call me shaka"),
            E(0x1F448, "point left"),
            E(0x1F449, "point right"),
            E(0x1F446, "point up"),
            E(0x1F447, "point down"),
            E(0x261D,  "index pointing up idea", 0xFE0F),
            E(0x1F44D, "thumbs up yes good approve like"),
            E(0x1F44E, "thumbs down no bad dislike"),
            E(0x270A,  "raised fist power"),
            E(0x1F44A, "oncoming fist punch bro"),
            E(0x1F44F, "clapping hands applause bravo"),
            E(0x1F64C, "raising hands hooray celebrate"),
            E(0x1F450, "open hands"),
            E(0x1F932, "palms up together"),
            E(0x1F91D, "handshake deal agree"),
            E(0x1F64F, "folded hands pray please thank you"),
            E(0x270D,  "writing hand", 0xFE0F),
            E(0x1F485, "nail polish sass"),
            E(0x1F4AA, "flexed biceps strong muscle gym"),
            E(0x1F9E0, "brain smart mind"),
            E(0x1F440, "eyes look watching"),
            E(0x1F441, "eye look", 0xFE0F),
            E(0x1F937, "shrug idk dunno"),
            E(0x1F926, "facepalm smh"),

            // Hearts & Symbols
            E(0x2764,  "red heart love", 0xFE0F),
            E(0x1F9E1, "orange heart"),
            E(0x1F49B, "yellow heart"),
            E(0x1F49A, "green heart"),
            E(0x1F499, "blue heart"),
            E(0x1F49C, "purple heart"),
            E(0x1F5A4, "black heart"),
            E(0x1F90D, "white heart"),
            E(0x1F90E, "brown heart"),
            E(0x1F494, "broken heart heartbreak"),
            E(0x2763,  "heart exclamation", 0xFE0F),
            E(0x1F495, "two hearts love"),
            E(0x1F49E, "revolving hearts"),
            E(0x1F493, "beating heart"),
            E(0x1F497, "growing heart"),
            E(0x1F496, "sparkling heart"),
            E(0x1F498, "heart arrow cupid"),
            E(0x1F49D, "heart ribbon gift"),
            E(0x1F4AF, "hundred 100 score perfect"),
            E(0x1F4A2, "anger symbol"),
            E(0x1F4A5, "collision boom explosion"),
            E(0x1F4AB, "dizzy star"),
            E(0x1F4A6, "sweat droplets water"),
            E(0x1F4A8, "dashing away wind fast"),
            E(0x1F4AC, "speech balloon chat message"),
            E(0x1F4AD, "thought balloon think"),
            E(0x1F4A4, "zzz sleep"),

            // Nature, Animals & Weather
            E(0x1F436, "dog puppy pet"),
            E(0x1F431, "cat kitty pet"),
            E(0x1F42D, "mouse"),
            E(0x1F439, "hamster"),
            E(0x1F430, "rabbit bunny"),
            E(0x1F98A, "fox"),
            E(0x1F43B, "bear"),
            E(0x1F43C, "panda"),
            E(0x1F428, "koala"),
            E(0x1F42F, "tiger"),
            E(0x1F981, "lion"),
            E(0x1F437, "pig"),
            E(0x1F438, "frog"),
            E(0x1F435, "monkey"),
            E(0x1F648, "see no evil monkey"),
            E(0x1F649, "hear no evil monkey"),
            E(0x1F64A, "speak no evil monkey"),
            E(0x1F414, "chicken"),
            E(0x1F427, "penguin"),
            E(0x1F985, "eagle bird"),
            E(0x1F986, "duck"),
            E(0x1F989, "owl"),
            E(0x1F43A, "wolf"),
            E(0x1F40D, "snake"),
            E(0x1F422, "turtle"),
            E(0x1F419, "octopus"),
            E(0x1F42C, "dolphin"),
            E(0x1F433, "whale"),
            E(0x1F988, "shark"),
            E(0x1F98B, "butterfly"),
            E(0x1F41D, "honeybee bee"),
            E(0x1F338, "cherry blossom flower"),
            E(0x1F339, "rose flower love"),
            E(0x1F33B, "sunflower"),
            E(0x1F332, "evergreen tree"),
            E(0x1F335, "cactus"),
            E(0x1F340, "four leaf clover luck"),
            E(0x1F341, "maple leaf autumn fall"),
            E(0x1F525, "fire flame hot lit"),
            E(0x2728,  "sparkles stars magic clean"),
            E(0x2B50,  "star favorite"),
            E(0x1F31F, "glowing star"),
            E(0x2600,  "sun sunny bright", 0xFE0F),
            E(0x1F319, "crescent moon night"),
            E(0x2601,  "cloud", 0xFE0F),
            E(0x26A1,  "high voltage lightning zap fast"),
            E(0x2744,  "snowflake cold winter", 0xFE0F),
            E(0x1F308, "rainbow pride"),
            E(0x1F30A, "water wave ocean"),

            // Food & Drink
            E(0x1F34E, "red apple fruit"),
            E(0x1F34C, "banana"),
            E(0x1F349, "watermelon"),
            E(0x1F353, "strawberry"),
            E(0x1F351, "peach"),
            E(0x1F951, "avocado"),
            E(0x1F355, "pizza"),
            E(0x1F354, "hamburger burger"),
            E(0x1F35F, "french fries"),
            E(0x1F32E, "taco"),
            E(0x1F363, "sushi"),
            E(0x1F35C, "steaming bowl ramen noodles"),
            E(0x1F369, "doughnut donut"),
            E(0x1F36A, "cookie"),
            E(0x1F382, "birthday cake"),
            E(0x1F370, "shortcake dessert"),
            E(0x1F36B, "chocolate bar"),
            E(0x1F37F, "popcorn movie"),
            E(0x2615,  "hot beverage coffee tea"),
            E(0x1F37A, "beer mug drink"),
            E(0x1F37B, "clinking beer mugs cheers"),
            E(0x1F377, "wine glass"),
            E(0x1F942, "clinking glasses champagne toast"),

            // Objects, Tech & Checks
            E(0x1F389, "party popper tada congrats celebrate"),
            E(0x1F38A, "confetti ball"),
            E(0x1F381, "wrapped gift present"),
            E(0x1F3C6, "trophy winner award"),
            E(0x1F947, "first place gold medal"),
            E(0x26BD,  "soccer ball football"),
            E(0x1F3C0, "basketball"),
            E(0x1F3AE, "video game controller gaming"),
            E(0x1F3B5, "musical note music"),
            E(0x1F3A7, "headphones music audio"),
            E(0x1F4F1, "mobile phone smartphone"),
            E(0x1F4BB, "laptop computer pc"),
            E(0x2328,  "keyboard typing", 0xFE0F),
            E(0x1F4F7, "camera photo"),
            E(0x1F4A1, "light bulb idea"),
            E(0x1F4B0, "money bag cash dollar"),
            E(0x1F4B8, "money with wings"),
            E(0x1F4CC, "pushpin pin"),
            E(0x1F4CE, "paperclip attach"),
            E(0x1F4DD, "memo note write"),
            E(0x1F4C5, "calendar date"),
            E(0x1F512, "locked lock security"),
            E(0x1F513, "unlocked open"),
            E(0x1F511, "key"),
            E(0x1F528, "hammer tool build"),
            E(0x2699,  "gear settings config", 0xFE0F),
            E(0x1F680, "rocket launch ship"),
            E(0x2708,  "airplane flight travel", 0xFE0F),
            E(0x1F697, "car automobile"),
            E(0x2705,  "check mark button yes done green"),
            E(0x2714,  "check mark tick", 0xFE0F),
            E(0x274C,  "cross mark no wrong red x"),
            E(0x26A0,  "warning alert caution", 0xFE0F),
            E(0x2757,  "exclamation mark important"),
            E(0x2753,  "question mark what"),
            E(0x2B06,  "up arrow", 0xFE0F),
            E(0x2B07,  "down arrow", 0xFE0F),
            E(0x27A1,  "right arrow", 0xFE0F),
            E(0x2B05,  "left arrow", 0xFE0F)
        };
    }

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var serializer = new JavaScriptSerializer();
        string catalogJson = serializer.Serialize(BuildCatalog());
        string recentJson = serializer.Serialize(GetRecentEmojis());

        var form = new Form
        {
            FormBorderStyle = FormBorderStyle.None,
            StartPosition = FormStartPosition.Manual,
            Size = new Size(316, 366),
            BackColor = Color.FromArgb(31, 31, 35),
            TopMost = true,
            ShowInTaskbar = false
        };

        form.Resize += (s, e) =>
        {
            int r = 12;
            var path = new GraphicsPath();
            path.AddArc(0, 0, r, r, 180, 90);
            path.AddArc(form.Width - r, 0, r, r, 270, 90);
            path.AddArc(form.Width - r, form.Height - r, r, r, 0, 90);
            path.AddArc(0, form.Height - r, r, r, 90, 90);
            path.CloseAllFigures();
            form.Region = new Region(path);
        };

        var browser = new WebBrowser
        {
            Dock = DockStyle.Fill,
            AllowWebBrowserDrop = false,
            IsWebBrowserContextMenuEnabled = false,
            WebBrowserShortcutsEnabled = false,
            ScriptErrorsSuppressed = true
        };

        var bridge = new EmojiBridge
        {
            OnEmojiPicked = (emoji) =>
            {
                if (!string.IsNullOrWhiteSpace(emoji))
                {
                    Clipboard.SetText(emoji);
                    SaveRecentEmoji(emoji);
                }
                form.Hide();
            },
            OnEscapePressed = () => form.Hide()
        };

        browser.ObjectForScripting = bridge;
        form.Controls.Add(browser);

        string html = @"<!DOCTYPE html>
<html>
<head>
<meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" />
<meta charset=""utf-8"" />
<style>
  * { box-sizing: border-box; margin: 0; padding: 0; user-select: none; -ms-user-select: none; }
  html, body { width: 100%; height: 100%; background: #1f1f23; color: #f4f4f6; font-family: ""Segoe UI"", system-ui, sans-serif; overflow: hidden; }
  .frame { width: 100%; height: 100%; border: 1px solid #3a3a42; border-radius: 12px; padding: 11px 11px 8px 11px; display: -ms-flexbox; display: flex; -ms-flex-direction: column; flex-direction: column; background: #1f1f23; }
  .label { font-size: 11px; font-weight: 600; color: #9e9ea9; margin-bottom: 5px; padding-left: 2px; }
  .recent-row { display: -ms-grid; display: grid; -ms-grid-columns: 1fr 1fr 1fr 1fr 1fr; grid-template-columns: repeat(5, 1fr); height: 44px; min-height: 44px; max-height: 44px; overflow: hidden; margin-bottom: 6px; }
  .divider { height: 1px; min-height: 1px; background: #35353d; margin: 2px 0 9px 0; }
  .search-wrap { margin-bottom: 8px; }
  #search { width: 100%; height: 32px; background: #2a2a30; border: 1px solid #42424c; border-radius: 6px; padding: 0 10px; color: #ffffff; font-family: ""Segoe UI"", sans-serif; font-size: 13px; outline: none; }
  #search:focus { border-color: #60cdff; background: #2e2e35; }
  #search::-ms-clear { display: none; }
  .scroll-wrap { position: relative; -ms-flex: 1; flex: 1; overflow: hidden; }
  .grid-scroll { width: 100%; height: 100%; overflow-y: scroll; overflow-x: hidden; padding-right: 8px; -ms-overflow-style: none; scrollbar-width: none; }
  .grid-scroll::-webkit-scrollbar { display: none; }
  .scroll-track { position: absolute; top: 2px; right: 0; bottom: 2px; width: 4px; border-radius: 4px; background: transparent; pointer-events: none; }
  .scroll-thumb { position: absolute; top: 0; left: 0; width: 4px; height: 36px; border-radius: 4px; background: #4e4e5a; opacity: 0; transition: opacity 0.15s ease; }
  .scroll-wrap:hover .scroll-thumb, .scroll-thumb.active { opacity: 1; }
  .emoji-grid { display: -ms-grid; display: grid; -ms-grid-columns: 1fr 1fr 1fr 1fr 1fr 1fr; grid-template-columns: repeat(6, 1fr); grid-gap: 2px; }
  .em-btn { height: 42px; display: -ms-flexbox; display: flex; -ms-flex-align: center; align-items: center; -ms-flex-pack: center; justify-content: center; font-family: ""Segoe UI Emoji"", ""Segoe UI Symbol"", sans-serif; font-size: 22px; line-height: 1; border-radius: 7px; cursor: pointer; background: transparent; }
  .em-btn:hover { background: #33333c; }
  .em-btn:active { background: #3f3f4a; }
  .empty-msg { color: #7a7a86; font-size: 12px; text-align: center; padding-top: 28px; }
</style>
</head>
<body>
  <div class=""frame"">
    <div class=""label"">Recently used</div>
    <div id=""recentRow"" class=""recent-row""></div>
    <div class=""divider""></div>
    <div class=""search-wrap"">
      <input id=""search"" type=""text"" placeholder=""Search emojis..."" autocomplete=""off"" spellcheck=""false"" />
    </div>
    <div class=""scroll-wrap"">
      <div id=""gridScroll"" class=""grid-scroll"">
        <div id=""allGrid"" class=""emoji-grid""></div>
        <div id=""emptyMsg"" class=""empty-msg"" style=""display:none;"">No matching emojis</div>
      </div>
      <div class=""scroll-track"">
        <div id=""scrollThumb"" class=""scroll-thumb""></div>
      </div>
    </div>
  </div>
<script>
  var CATALOG = " + catalogJson + @";
  var RECENT = " + recentJson + @";
  var scrollBox = document.getElementById('gridScroll');
  var scrollThumb = document.getElementById('scrollThumb');
  var targetScroll = 0, currentScroll = 0, animRunning = false, hideThumbTimer = null;

  function updateThumb() {
    var maxScroll = scrollBox.scrollHeight - scrollBox.clientHeight;
    if (maxScroll <= 0) { scrollThumb.style.display = 'none'; return; }
    scrollThumb.style.display = 'block';
    var trackH = scrollBox.clientHeight - 4;
    var thumbH = Math.max(26, Math.round((scrollBox.clientHeight / scrollBox.scrollHeight) * trackH));
    var maxTop = trackH - thumbH;
    scrollThumb.style.height = thumbH + 'px';
    scrollThumb.style.top = Math.round((currentScroll / maxScroll) * maxTop) + 'px';
  }

  function showThumbBriefly() {
    scrollThumb.className = 'scroll-thumb active';
    if (hideThumbTimer) clearTimeout(hideThumbTimer);
    hideThumbTimer = setTimeout(function() { scrollThumb.className = 'scroll-thumb'; }, 650);
  }

  function stepScroll() {
    var diff = targetScroll - currentScroll;
    if (Math.abs(diff) < 0.4) {
      currentScroll = targetScroll;
      scrollBox.scrollTop = Math.round(currentScroll);
      updateThumb();
      animRunning = false;
      return;
    }
    currentScroll += diff * 0.34;
    scrollBox.scrollTop = Math.round(currentScroll);
    updateThumb();
    window.requestAnimationFrame(stepScroll);
  }

  function handleWheel(e) {
    e = e || window.event;
    if (e.preventDefault) e.preventDefault();
    e.returnValue = false;
    var maxScroll = scrollBox.scrollHeight - scrollBox.clientHeight;
    if (maxScroll <= 0) return;
    var delta = 0;
    if (typeof e.deltaY === 'number' && e.deltaY !== 0) delta = e.deltaY > 0 ? 110 : -110;
    else if (typeof e.wheelDelta === 'number' && e.wheelDelta !== 0) delta = e.wheelDelta < 0 ? 110 : -110;
    targetScroll = Math.max(0, Math.min(maxScroll, targetScroll + delta));
    showThumbBriefly();
    if (!animRunning) { animRunning = true; window.requestAnimationFrame(stepScroll); }
  }

  if (scrollBox.addEventListener) {
    scrollBox.addEventListener('wheel', handleWheel, false);
    scrollBox.addEventListener('mousewheel', handleWheel, false);
  } else { scrollBox.onmousewheel = handleWheel; }

  function makeBtn(ch, colIndex) {
    var d = document.createElement('div');
    d.className = 'em-btn';
    if (typeof colIndex === 'number') d.style.msGridColumn = String(colIndex);
    d.innerText = ch;
    d.onclick = function() { window.external.Pick(ch); };
    return d;
  }

  function renderRecent(list) {
    if (list) RECENT = list;
    var row = document.getElementById('recentRow');
    row.innerHTML = '';
    for (var i = 0; i < Math.min(RECENT.length, 5); i++) row.appendChild(makeBtn(RECENT[i], i + 1));
  }

  function renderGrid(query) {
    var grid = document.getElementById('allGrid');
    var empty = document.getElementById('emptyMsg');
    grid.innerHTML = '';
    var q = (query || '').replace(/^\s+|\s+$/g, '').toLowerCase();
    var shown = 0;
    for (var i = 0; i < CATALOG.length; i++) {
      var item = CATALOG[i];
      if (!q || item.k.indexOf(q) !== -1) {
        var btn = makeBtn(item.c);
        btn.style.msGridColumn = String((shown % 6) + 1);
        btn.style.msGridRow = String(Math.floor(shown / 6) + 1);
        grid.appendChild(btn);
        shown++;
      }
    }
    empty.style.display = shown === 0 ? 'block' : 'none';
    targetScroll = 0; currentScroll = 0; scrollBox.scrollTop = 0;
    setTimeout(updateThumb, 10);
  }

  function resetView(recentList) {
    renderRecent(recentList);
    var s = document.getElementById('search');
    s.value = '';
    renderGrid('');
    setTimeout(function() { s.focus(); }, 20);
  }

  var searchInput = document.getElementById('search');
  searchInput.onkeyup = function(e) {
    e = e || window.event;
    if (e.keyCode === 27) { window.external.Close(); return; }
    renderGrid(searchInput.value);
  };
  document.onkeydown = function(e) {
    e = e || window.event;
    if (e.keyCode === 27) window.external.Close();
  };

  renderRecent(RECENT);
  renderGrid('');
</script>
</body>
</html>";

        browser.DocumentText = html;
        form.Deactivate += (s, e) => form.Hide();

        var notifyIcon = new NotifyIcon
        {
            Icon = CreateSmileyIcon(),
            Text = "Emoji Picker",
            Visible = true
        };

        Action showPicker = () =>
        {
            if (form.Visible) { form.Hide(); return; }
            Rectangle workArea = Screen.PrimaryScreen.WorkingArea;
            form.Location = new Point(workArea.Right - form.Width - 12, workArea.Bottom - form.Height - 12);
            form.Show();
            form.Activate();
            browser.Focus();
            if (browser.Document != null)
            {
                string latestJson = serializer.Serialize(GetRecentEmojis());
                browser.Document.InvokeScript("eval", new object[] { "resetView(" + latestJson + ");" });
            }
        };

        notifyIcon.MouseClick += (s, e) =>
        {
            if (e.Button == MouseButtons.Left) showPicker();
        };

        var menu = new ContextMenuStrip();
        menu.Items.Add("Open Emoji Picker", null, (s, e) => showPicker());

        var startupItem = new ToolStripMenuItem("Start with Windows") { Checked = IsStartupEnabled() };
        startupItem.Click += (s, e) =>
        {
            bool next = !IsStartupEnabled();
            SetStartupEnabled(next);
            startupItem.Checked = next;
        };
        menu.Items.Add(startupItem);
        menu.Items.Add("-");
        menu.Items.Add("Exit", null, (s, e) =>
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            form.Close();
            Application.Exit();
        });

        notifyIcon.ContextMenuStrip = menu;
        Application.Run();
    }
}