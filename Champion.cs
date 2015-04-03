using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

public abstract class Champion
{
	protected Obj_AI_Hero Player;
	protected MenuWrapper Menu;
    protected SkinManager Skins;

	protected Spell Q;
	protected Spell W;
	protected Spell E;
	protected Spell R;
	
	protected Dictionary<string, MenuWrapper.BoolLink> BoolLinks = new Dictionary<string, MenuWrapper.BoolLink>();
    protected Dictionary<string, MenuWrapper.CircleLink> CircleLinks = new Dictionary<string, MenuWrapper.CircleLink>();
    protected Dictionary<string, MenuWrapper.KeyBindLink> KeyLinks = new Dictionary<string, MenuWrapper.KeyBindLink>();
    protected Dictionary<string, MenuWrapper.SliderLink> SliderLinks = new Dictionary<string, MenuWrapper.SliderLink>();
	
    protected List<Obj_AI_Hero> Enemies;
    
    private const int tick = 1000 / 20;
    private int lastTick = Environment.TickCount;
    private string targetChampion;
        
	protected Champion(string championName)
	{
		targetChampion = championName;
		CustomEvents.Game.OnGameLoad += OnGameLoad;
	}

	~Champion()
	{
		CustomEvents.Game.OnGameLoad -= OnGameLoad;
		Game.OnUpdate -= OnUpdate;
     	Drawing.OnDraw -= OnDraw;
        Drawing.OnEndScene -= OnEndScene;
        AntiGapcloser.OnEnemyGapcloser -= OnEnemyGapcloser;
        Interrupter.OnPossibleToInterrupt -= OnPossibleToInterrupt;
	}
	
	private void OnGameLoad(EventArgs args)
	{	
		if(ObjectManager.Player.ChampionName.ToLower() != targetChampion.ToLower())
			return;
		
 		Player = ObjectManager.Player;
        Skins = new SkinManager();

        OnInitSpells();
        OnInit();
        OnInitSkins();

     	Menu = new MenuWrapper("Easy" + Player.ChampionName);
        Skins.AddToMenu(Menu.MainMenu);

        OnInitMenu();

        BoolLinks.Add("packets", Menu.MainMenu.AddLinkedBool("Use packet cast", true));

        Game.OnUpdate += OnUpdate;
     	Drawing.OnDraw += OnDraw;
        Drawing.OnEndScene += OnEndScene;
        AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
        Interrupter.OnPossibleToInterrupt += OnPossibleToInterrupt;
        Spellbook.OnCastSpell += OnCastSpell;
        Obj_AI_Hero.OnProcessSpellCast += OnProcessSpellCast;
        
        Game.PrintChat("Easy" + Player.ChampionName + " is loaded! Skin Changer does not work!");
	}

	protected bool IsPacketCastEnabled()
    {
        return BoolLinks["packets"].Value;
    }
	
	private void OnUpdate(EventArgs args)
	{
		if (Environment.TickCount < lastTick + tick) return;
        lastTick = Environment.TickCount;
        
        Spells.PacketCast = IsPacketCastEnabled();
        Enemies = ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy && x.IsValidTarget()).ToList();

        OnUpdate();
		
        if (Player.IsWindingUp || Player.IsDashing()) return;
        
        bool minionBlock = MinionManager.GetMinions(Player.Position, Player.AttackRange, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.None).Count(x => HealthPrediction.GetHealthPrediction(x, 1500) <= Player.GetAutoAttackDamage(x)) > 0;

        switch (Menu.Orbwalker.ActiveMode)
        {
            case Orbwalking.OrbwalkingMode.Combo:
                OnCombo();
                break;
            case Orbwalking.OrbwalkingMode.Mixed:
                if (!minionBlock) OnHarass();
                break;
            case Orbwalking.OrbwalkingMode.LaneClear:
            case Orbwalking.OrbwalkingMode.LastHit:
            default:
                if (!minionBlock) OnAuto();
                break;
        }
	}
	
	private void OnDraw(EventArgs args)
	{
		OnDraw();
	}
	
	private void OnEndScene(EventArgs args)
	{
		OnEndScene();
	}
	
	protected SpellDataInst GetSpellData(SpellSlot spell)
    {
        return Player.Spellbook.GetSpell(spell);
    }
	
	protected virtual void OnInit() { }
	protected virtual void OnInitSpells() { }
	protected virtual void OnInitSkins() { }
	protected virtual void OnInitMenu() { }
    protected virtual void OnUpdate() { }
    protected virtual void OnDraw() { }
    protected virtual void OnEndScene() { }
    protected virtual void OnCombo() { }
    protected virtual void OnHarass() { }
    protected virtual void OnAuto() { }
	protected virtual void OnEnemyGapcloser(ActiveGapcloser gapcloser) { }
	protected virtual void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell) { }
    protected virtual void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args) { }
    protected virtual void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args) { }
}