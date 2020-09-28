using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfinityScript;

namespace Switch
{
    public class Switch : BaseScript
    {
        public Switch()
        {
            GSCFunctions.MakeDvarServerInfo("didyouknow", "^2Switch script by LastDemon99");
            GSCFunctions.MakeDvarServerInfo("g_motd", "^2Switch script by LastDemon99");
            GSCFunctions.MakeDvarServerInfo("motd", "^2Switch script by LastDemon99");

            InfiniteStock();
            PlayerConnected += new Action<Entity>((player) =>
            {
                ServerWelcomeTittle(player, "Switch", new float[] { 0, 0, 1 });
                player.SetClientDvar("ui_mapname", "Switch");
                player.SetClientDvar("ui_gametype", "Switch");

                StingerFire(player);
                OnSpawned(player);
            });
        }

        private void OnSpawned(Entity player)
        {
            DisableSelectClass(player);

            player.SpawnedPlayer += new Action(() =>
            {
                GSCFunctions.DisableWeaponPickup(player);
                player.TakeAllWeapons();
                AfterDelay(350, () => { GiveWeapon(player); GiveAllPerks(player); });
            });
        }
        private void GiveWeapon(Entity player)
        {
            int index = GSCFunctions.RandomIntRange(0, DefaultWeapons.Length);
            string getwep = DefaultWeapons[index];
            getwep = RandomCamo(player, getwep);

            player.TakeAllWeapons();
            player.GiveWeapon(getwep);
            player.SwitchToWeaponImmediate(getwep);
        }
        private void InfiniteStock()
        {
            OnInterval(50, () =>
            {
                foreach (Entity player in BaseScript.Players)
                    GSCFunctions.SetWeaponAmmoStock(player, player.CurrentWeapon, 45);
                return true;
            });
        }
        private void StingerFire(Entity player)
        {
            player.NotifyOnPlayerCommand("attack", "+attack");
            player.OnNotify("attack", self =>
            {
                if (player.CurrentWeapon == "stinger_mp")
                {
                    if (GSCFunctions.PlayerAds(player) >= 1f)
                    {
                        if (GSCFunctions.GetWeaponAmmoClip(player, player.CurrentWeapon) != 0)
                        {
                            Vector3 vector = GSCFunctions.AnglesToForward(GSCFunctions.GetPlayerAngles(player));
                            Vector3 dsa = new Vector3(vector.X * 1000000f, vector.Y * 1000000f, vector.Z * 1000000f);
                            GSCFunctions.MagicBullet("stinger_mp", GSCFunctions.GetTagOrigin(player, "tag_weapon_left"), dsa, self);
                            GSCFunctions.SetWeaponAmmoClip(player, player.CurrentWeapon, 0);
                        }
                    }
                }
            });
        }

        private void GiveAllPerks(Entity player)
        {
            player.SetPerk("specialty_longersprint", true, false);
            player.SetPerk("specialty_fastreload", true, false);
            player.SetPerk("specialty_scavenger", true, false);
            player.SetPerk("specialty_blindeye", true, false);
            player.SetPerk("specialty_paint", true, false);
            player.SetPerk("specialty_hardline", true, false);
            player.SetPerk("specialty_coldblooded", true, false);
            player.SetPerk("specialty_quickdraw", true, false);
            player.SetPerk("specialty_twoprimaries", true, false);
            player.SetPerk("specialty_assists", true, false);
            player.SetPerk("_specialty_blastshield", true, false);
            player.SetPerk("specialty_detectexplosive", true, false);
            player.SetPerk("specialty_autospot", true, false);
            player.SetPerk("specialty_bulletaccuracy", true, false);
            player.SetPerk("specialty_quieter", true, false);
            player.SetPerk("specialty_stalker", true, false);
        }
        private void DisableSelectClass(Entity player)
        {
            GSCFunctions.ClosePopUpMenu(player, "");
            GSCFunctions.CloseInGameMenu(player);
            player.Notify("menuresponse", "team_marinesopfor", "axis");
            player.OnNotify("joined_team", ent =>
            {
                AfterDelay(500, () => { ent.Notify("menuresponse", "changeclass", "class1"); });
            });
            player.OnNotify("menuresponse", (player2, menu, response) =>
            {
                if (menu.ToString().Equals("class") && response.ToString().Equals("changeclass_marines"))
                {
                    AfterDelay(100, () => { player.Notify("menuresponse", "changeclass", "back"); });
                }
            });
        }
        private string RandomCamo(Entity player, string weapon)
        {
            string newWep = weapon;
            if (!NoCamo.Contains(newWep))
            {
                int cnum = GSCFunctions.RandomIntRange(1, 13);
                if (cnum < 10)
                    newWep = weapon + "_camo0" + cnum.ToString();
                else
                    newWep = weapon + "_camo" + cnum.ToString();
            }
            return newWep;
        }
        public static void ServerWelcomeTittle(Entity player, string tittle, float[] rgb)
        {
            player.SetField("welcome", 0);
            player.SpawnedPlayer += new Action(() =>
            {
                if (player.GetField<int>("welcome") == 0)
                {
                    HudElem serverWelcome = HudElem.CreateFontString(player, HudElem.Fonts.HudBig, 1f);
                    serverWelcome.SetPoint("TOPCENTER", "TOPCENTER", 0, 165);
                    serverWelcome.SetText(tittle);
                    serverWelcome.GlowColor = (new Vector3(rgb[0], rgb[1], rgb[2]));
                    serverWelcome.GlowAlpha = 1f;
                    serverWelcome.SetPulseFX(150, 4700, 700);
                    player.SetField("welcome", 1);

                    AfterDelay(5000, () => { serverWelcome.Destroy(); });
                }
            });
        }

        public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
            if (damage >= player.Health && player != attacker)
                if (mod != "MOD_MELEE" || weapon == "riotshield_mp")
                    GiveWeapon(attacker); 
        }

        private string[] DefaultWeapons = {  "iw5_m4_mp",
                                           "iw5_ak47_mp",
                                           "iw5_m16_mp",
                                           "iw5_fad_mp",
                                           "iw5_acr_mp",
                                           "iw5_type95_mp",
                                           "iw5_mk14_mp",
                                           "iw5_scar_mp",
                                           "iw5_g36c_mp",
                                           "iw5_cm901_mp",
                                           "iw5_mp5_mp",
                                           "iw5_mp7_mp",
                                           "iw5_m9_mp",
                                           "iw5_p90_mp",
                                           "iw5_pp90m1_mp",
                                           "iw5_ump45_mp",
                                           "iw5_barrett_mp_barrettscopevz",
                                           "iw5_rsass_mp_rsassscopevz",
                                           "iw5_dragunov_mp_dragunovscopevz",
                                           "iw5_msr_mp_msrscopevz",
                                           "iw5_l96a1_mp_l96a1scopevz",
                                           "iw5_as50_mp_as50scopevz",
                                           "iw5_ksg_mp",
                                           "iw5_1887_mp",
                                           "iw5_striker_mp",
                                           "iw5_aa12_mp",
                                           "iw5_usas12_mp",
                                           "iw5_spas12_mp",
                                           "iw5_m60_mp",
                                           "iw5_mk46_mp",
                                           "iw5_pecheneg_mp",
                                           "iw5_sa80_mp",
                                           "iw5_mg36_mp",
                                           "iw5_44magnum_mp",
                                           "iw5_usp45_mp",
                                           "iw5_deserteagle_mp",
                                           "iw5_mp412_mp",
                                           "iw5_p99_mp",
                                           "iw5_fnfiveseven_mp",
                                           "iw5_g18_mp",
                                           "iw5_fmg9_mp",
                                           "iw5_mp9_mp",
                                           "iw5_skorpion_mp",
                                           "m320_mp",
                                           "rpg_mp",
                                           "iw5_smaw_mp",
                                           "stinger_mp",
                                           "xm25_mp",
                                           "riotshield_mp",
                                           "javelin_mp"};


        private string[] NoCamo = {  "iw5_44magnum_mp",
                                           "iw5_usp45_mp",
                                           "iw5_deserteagle_mp",
                                           "iw5_mp412_mp",
                                           "iw5_p99_mp",
                                           "iw5_fnfiveseven_mp",
                                           "iw5_g18_mp",
                                           "iw5_fmg9_mp",
                                           "iw5_mp9_mp",
                                           "iw5_skorpion_mp",
                                           "m320_mp",
                                           "rpg_mp",
                                           "iw5_smaw_mp",
                                           "stinger_mp",
                                           "xm25_mp",
                                           "javelin_mp",
                                           "iw5_m60jugg_mp",
                                           "iw5_riotshieldjugg_mp",
                                           "riotshield_mp"};
    }
}
