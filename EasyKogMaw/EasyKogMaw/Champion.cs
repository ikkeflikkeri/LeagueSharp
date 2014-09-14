using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

abstract class Champion
{
	public Obj_AI_Hero Player;
	public Menu Menu;
	public Orbwalking.Orbwalker Orbwalker;
	public Dictionary<string, Spell> Spells = new Dictionary<string, Spell>();

	private int tick = 1000 / 20;
	private int lastTick = Environment.TickCount;
	private string ChampionName;
    private bool isDebugging;

    private SkinManager SkinManager;

	public Champion(string name, bool debug = false)
	{
		ChampionName = name;
        isDebugging = debug;
       
		CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
	}

	void Game_OnGameLoad(EventArgs args)
	{
		Player = ObjectManager.Player;

		if (ChampionName.ToLower() != Player.ChampionName.ToLower())
			return;

        SkinManager = new SkinManager();

		InitializeSpells();
		InitializeSkins(ref SkinManager);

		Menu = new Menu("Easy" + ChampionName, "Easy" + ChampionName, true);

        SkinManager.AddToMenu(ref Menu);

		Menu.AddSubMenu(new Menu("Target Selector", "Target Selector"));
		SimpleTs.AddToMenu(Menu.SubMenu("Target Selector"));

		Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
		Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("Orbwalker"));

		CreateMenu();

		Menu.AddItem(new MenuItem("Recall_block", "Block skills while recalling").SetValue(true));

		Menu.AddToMainMenu();

		Game.OnGameUpdate += Game_OnGameUpdate;
		Game.OnGameEnd += Game_OnGameEnd;
		LeagueSharp.Drawing.OnDraw += Drawing_OnDraw;

		using (WebClient wc = new WebClient())
		{
			wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
			string amount = wc.UploadString("http://niels-wouters.be/LeagueSharp/playcount.php", "assembly=" + ChampionName);
			Game.PrintChat("Easy" + ChampionName + " is loaded! This assembly has been played in " + amount + " games.");
		}
	}

	void Game_OnGameEnd(GameEndEventArgs args)
	{
		using (WebClient wc = new WebClient())
		{
			wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
			wc.UploadString("http://niels-wouters.be/LeagueSharp/stats.php", "assembly=" + ChampionName);
		}
	}

	void Drawing_OnDraw(EventArgs args)
	{
		Drawing();

        if (isDebugging) DrawBuffs();
	}

	void Game_OnGameUpdate(EventArgs args)
	{
		if (Environment.TickCount < lastTick + tick) return;
		lastTick = Environment.TickCount;

        SkinManager.Update();

		Update();

		if ((Menu.Item("Recall_block").GetValue<bool>() && Player.HasBuff("Recall")) || Player.IsWindingUp)
			return;

		if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo) Combo();

		if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed) Harass();

		Auto();

	}

    protected void Cast(string spell, SimpleTs.DamageType damageType, bool packet = true, bool aoe = false)
    {
        if (Spells[spell].IsSkillshot) CastSkillshot(spell, damageType, packet, aoe);
        if (Spells[spell].IsChargedSpell) CastChargedSpell(spell, damageType, packet, aoe);
        CastOnUnit(spell, damageType, packet);
    }

    protected void CastSelf(string spell, SimpleTs.DamageType damageType, float extraRange = 0)
    {
        if (!Spells[spell].IsReady()) return;

        Obj_AI_Hero target = SimpleTs.GetTarget(Spells[spell].Range + extraRange, damageType);
        if (target == null) return;

        Spells[spell].Cast();
    }

    private void CastChargedSpell(string spell, SimpleTs.DamageType damageType, bool packet, bool aoe)
    {
        if (!Spells[spell].IsReady())
            return;

        Obj_AI_Hero target = SimpleTs.GetTarget(Spells[spell].ChargedMaxRange, damageType);
        if (target == null || !target.IsValidTarget(Spells[spell].ChargedMaxRange))
            return;

        if(!Spells[spell].IsCharging)
            Spells[spell].StartCharging();
        else
        {
            if (Spells[spell].GetPrediction(target).Hitchance >= HitChance.High)
                Spells[spell].Cast(target, packet, aoe);
        }
    }
    private void CastSkillshot(string spell, SimpleTs.DamageType damageType, bool packet, bool aoe)
    {
        if (!Spells[spell].IsReady()) return;

        Obj_AI_Hero target = SimpleTs.GetTarget(Spells[spell].Range, damageType);
        if (target == null) return;

        if (target.IsValidTarget(Spells[spell].Range) && Spells[spell].GetPrediction(target).Hitchance >= HitChance.High)
            Spells[spell].Cast(target, packet, aoe);
    }
    private void CastOnUnit(string spell, SimpleTs.DamageType damageType, bool packet)
    {
        if (!Spells[spell].IsReady()) return;

        Obj_AI_Hero target = SimpleTs.GetTarget(Spells[spell].Range, damageType);
        if (target == null) return;

        Spells[spell].CastOnUnit(target, packet);
    }

	protected virtual void InitializeSkins(ref SkinManager Skins)
	{

	}
    protected virtual void InitializeSpells()
	{

	}
	protected virtual void CreateMenu()
	{

	}
	protected virtual void Combo()
	{

	}
	protected virtual void Harass()
	{

	}
	protected virtual void Auto()
	{

	}
	protected virtual void Drawing()
	{

	}
	protected virtual void Update()
	{

	}

    protected void DrawBuffs()
    {
        float y = 0;
        foreach (var t in ObjectManager.Player.Buffs.Select(b => b.DisplayName + " - " + b.IsActive + " - " + (b.EndTime > Game.Time) + " - " + b.IsPositive))
        {
            LeagueSharp.Drawing.DrawText(0, y, System.Drawing.Color.Wheat, t);
            y += 16;
        }
    }
}

class SkinManager
{
    private List<string> Skins = new List<string>();
    private Menu Menu;
    private int SelectedSkin;
    private bool Initialize = true;

    public SkinManager()
    {

    }

    public void AddToMenu(ref Menu menu)
    {
        Menu = menu;

        if (Skins.Count > 0)
        {
            Menu.AddSubMenu(new Menu("Skin Changer", "Skin Changer"));
            Menu.SubMenu("Skin Changer").AddItem(new MenuItem("Skin_enabled", "Enable skin changer").SetValue(false));
            Menu.SubMenu("Skin Changer").AddItem(new MenuItem("Skin_select", "Skins").SetValue(new StringList(Skins.ToArray())));
            SelectedSkin = Menu.Item("Skin_select").GetValue<StringList>().SelectedIndex;
        }
    }

    public void Add(string skin)
    {
        Skins.Add(skin);
    }

    public void Update()
    {
        if (Menu.Item("Skin_enabled").GetValue<bool>())
        {
            int skin = Menu.Item("Skin_select").GetValue<StringList>().SelectedIndex;
            if (Initialize || skin != SelectedSkin)
            {
                GenerateSkinPacket(skin);
                SelectedSkin = skin;
                Initialize = false;
            }
        }
    }

    private static void GenerateSkinPacket(int skinNumber)
    {
        int netID = ObjectManager.Player.NetworkId;
        GamePacket model = Packet.S2C.UpdateModel.Encoded(new Packet.S2C.UpdateModel.Struct(ObjectManager.Player.NetworkId, skinNumber, ObjectManager.Player.ChampionName));
        model.Process(PacketChannel.S2C);
    }
}