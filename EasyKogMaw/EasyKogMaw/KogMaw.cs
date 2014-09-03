using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyKogMaw
{
    class KogMaw : Champion
    {
        public KogMaw() : base("KogMaw")
        {
            
        }

        protected override void CreateSpells()
        {
            
        }

        protected override void CreateMenu()
        {
            Menu.AddSubMenu(new Menu("Combo", "Combo"));
            Menu.SubMenu("Combo").AddItem(new MenuItem("Combo_q", "Use Q").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("Combo_w", "Use W").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("Combo_e", "Use E").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("Combo_r", "Use R").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("Combo_maxrstacks", "Max R stacks").SetValue(new Slider(5, 0, 10)));

            Menu.AddSubMenu(new Menu("Harass", "Harass"));
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_q", "Use Q").SetValue(true));
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_w", "Use W").SetValue(true));
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_e", "Use E").SetValue(true));
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_r", "Use R").SetValue(true));
            Menu.SubMenu("Harass").AddItem(new MenuItem("Harass_maxrstacks", "Max R stacks").SetValue(new Slider(1, 0, 10)));

            Menu.AddSubMenu(new Menu("Auto", "Auto"));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_q", "Use Q").SetValue(true));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_w", "Use W").SetValue(true));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_e", "Use E").SetValue(true));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_r", "Use R").SetValue(true));
            Menu.SubMenu("Auto").AddItem(new MenuItem("Auto_maxrstacks", "Max R stacks").SetValue(new Slider(1, 0, 10)));

            Menu.AddSubMenu(new Menu("Drawing", "Drawing"));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_q", "Use Q").SetValue(true));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_w", "Use W").SetValue(true));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_e", "Use E").SetValue(true));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing_r", "Use R").SetValue(true));
        }

        protected override void Combo()
        {
           
        }
        protected override void Harass()
        {
            
        }
        protected override void Auto()
        {
            
        }
        public override void Drawing()
        {
           
        }
    }
}
