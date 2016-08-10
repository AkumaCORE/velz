using System;
using System.Collections.Generic;
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
        public static List<Vector2> Perpendiculars { get; set; }
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
            Game.OnUpdate += QSplitter;
            GameObject.OnCreate += Obj_AI_Base_OnCreate;
            Game.OnUpdate += QSplitter2;
            Drawing.OnDraw += OnDraw3;
        }
        
        private static void OnDraw3(EventArgs args)
        {

             //var CurrentTarget = TargetSelector.GetTarget(1500, DamageType.Magical);
	    // var enemydirection = CurrentTarget.ServerPosition;
	     var startPos = Handle.Position.To2D();
         //    var intertwoD = intersection.To2D();


                   
		  //   Circle.Draw(SharpDX.Color.White, 10, 50, intertwoD.To3D());
	            // Circle.Draw(SharpDX.Color.Blue, 100, 100, enemydirection);
	            Circle.Draw(SharpDX.Color.Red, 10, 50, startPos.To3D());
	             foreach (var perpendicular in Perpendiculars)
	             {
	                Chat.Print("Q detected");
	                var endPos = Handle.Position.To2D() + 1000 * perpendicular;
	                Circle.Draw(SharpDX.Color.Yellow, 10, 60, endPos.To3D());
	                 
        	     }
               
             
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
        
        public static void Obj_AI_Base_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.IsAlly && sender != null && sender.Type != GameObjectType.obj_AI_Minion)
            {
                var missile = (MissileClient)sender;
                if (missile.SData.Name != null && missile.SData.Name == "VelkozQMissile")
                    {
                    QMissile = missile;
                    Handle = missile;
                    QTime = Core.GameTickCount;
                    }
            }
        }
        
        private static void QSplitter(EventArgs args)
        {
            // Check if the missile is active
            if (Handle != null && Core.GameTickCount - QTime <= 1500)

            {
                 // Chat.Print("Q detected");
                  var Direction = (Handle.EndPosition.To2D() - Handle.StartPosition.To2D()).Normalized();
                  Perpendiculars.Add(Direction.Perpendicular());
                  Perpendiculars.Add(Direction.Perpendicular2());

            }
            else
                Handle = null;
        }
        
        private static void QSplitter2(EventArgs args)
        {
		
		if (Handle != null)
		{
	                foreach (var perpendicular in Perpendiculars)
	                {
			    Chat.Print("enem1y");
	                    if (Handle != null)
	                    {
	                        
	                        var startPos = Handle.Position.To2D();
	                        var endPos = Handle.Position.To2D() + 1100 * perpendicular;
	
	                        var collisionObjects = EntityManager.Heroes.Enemies.Where(it => it.IsValidTarget(1500));
	                        if (collisionObjects.Count() >= 1)
	                        {
	                            Chat.Print("enemy");
	                            
	                        }
	                        
	                        foreach (var hero in collisionObjects)
	                        {
		                if (Prediction.Position.Collision.LinearMissileCollision(hero, startPos, endPos, 2100, 100, 0))
	                    	{
	
	
	                            Spells.Q.Cast(Champion);
	
	                           
	                        }
	                        }
	                       
	                    }
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
