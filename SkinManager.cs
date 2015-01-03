using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;

public class SkinManager
{
    private List<string> Skins = new List<string>();
    private int SelectedSkin;
    private bool Initialize = true;
    private MenuWrapper.BoolLink enabled;
    private MenuWrapper.StringListLink skinList;

    public void AddToMenu(MenuWrapper.SubMenu menu)
    {
        if (Skins.Count == 0)
            return;
        
        MenuWrapper.SubMenu skinMenu = menu.AddSubMenu("Skin Changer");
        
        enabled = skinMenu.AddLinkedBool("Enable skin changer");
        skinList = skinMenu.AddLinkedStringList("Skins", Skins.ToArray());

        SelectedSkin = skinList.Value.SelectedIndex;
    }

    public void Add(string skin)
    {
        Skins.Add(skin);
    }

    public void Update()
    {
        if (!enabled.Value)
            return;
    
        int skin = skinList.Value.SelectedIndex;
        if (Initialize || skin != SelectedSkin)
        {
            Packet.S2C.UpdateModel.Encoded(new Packet.S2C.UpdateModel.Struct(ObjectManager.Player.NetworkId, skin, ObjectManager.Player.ChampionName)).Process();
            SelectedSkin = skin;
            Initialize = false;
        }
    }
}