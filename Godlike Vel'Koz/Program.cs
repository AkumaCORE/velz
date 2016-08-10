using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using Color = System.Drawing.Color;
using System.Linq;
using System.Media;
using EloBuddy.SDK.Rendering;
using SharpDX;
using Godlike_Vel_Koz.Properties;
using EloBuddy.SDK.Enumerations;
namespace Godlike_Vel_Koz

{
    class Program
    {
        #region Sounds
        private static readonly SoundPlayer ohDarn = new SoundPlayer(Resources.oh_darn);
        private static bool playingOhDarn;
        #endregion Sounds

        public static AIHeroClient Champion { get { return Player.Instance; } }
        private static List<Vector2> Points = new List<Vector2>(); 
        static int playerKills = 0;
        
        private static MissileClient QMissile;
        private static MissileClient Handle;
        public static float QTime = 0;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        public static void Loading_OnLoadingComplete(EventArgs args)
        {
            #region Sounds
            ohDarn.LoadAsync();
            #endregion
            playerKills = Player.Instance.ChampionsKilled;

            if (Champion.ChampionName != "Velkoz") return;

            Manager.Initialize();
            Spells.Initialize();

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Game.OnTick += Game_OnTick;
            Interrupter.OnInterruptableSpell += Modes.InterruptMode;
            Gapcloser.OnGapcloser += Modes.GapCloserMode;
           // Game.OnUpdate += QSplitter;
            //Game.OnUpdate += QSplitter2;
        }

        public static void Game_OnUpdate(EventArgs args)
        {
            if (Manager.skinsEnable)
                Player.SetSkinId(Manager.skinsNumber);

            if (Manager.ohdarnEnable && playerKills != Player.Instance.ChampionsKilled)
            {
                if(!playingOhDarn)
                {
                    ohDarn.Play();
                    playingOhDarn = true;
                    Core.DelayAction(() => playingOhDarn = false, 1000);
                    playerKills++;
                }
            }
        }
        


                
        public static void Drawing_OnDraw(EventArgs args)
        {
            if (Game.Time < 10) return;
            if (Champion.IsDead) return;

            Color color = Color.FromArgb(168, 27, 168);

            if (Manager.drawingsQW && (Spells.Q.IsLearned || Spells.W.IsLearned))
                Drawing.DrawCircle(Champion.Position, Spells.Q.Range, color);
            if (Manager.drawingsE && Spells.E.IsLearned)
                Drawing.DrawCircle(Champion.Position, Spells.E.Range, color);
            if (Manager.drawingsR && Spells.R.IsLearned)
                Drawing.DrawCircle(Champion.Position, Spells.R.Range, color);
        }

        public static void Game_OnTick(EventArgs args)
        {
            if (Champion.IsDead) return;

            string currentModes = Orbwalker.ActiveModesFlags.ToString();

            if (Manager.killstealEnable)
                Modes.KillStealMode();
            if (currentModes.Contains(Orbwalker.ActiveModes.Combo.ToString()))
                Modes.ComboMode();
            if (currentModes.Contains(Orbwalker.ActiveModes.Harass.ToString()))
                Modes.HarassMode();
            if (currentModes.Contains(Orbwalker.ActiveModes.LaneClear.ToString()))
                Modes.LaneClearMode();
            if (currentModes.Contains(Orbwalker.ActiveModes.JungleClear.ToString()))
                Modes.JungleMode();
        }
    }
}
