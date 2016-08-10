using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using System.Linq;

namespace Godlike_Vel_Koz
{
    class Modes
    {
        public static AIHeroClient Champion = Program.Champion;

        public static void ComboMode()
        {
            if (Manager.comboQ)
            {
                var target = Target.GetChampionTarget(Spells.Q.Range, DamageType.Magical, false, true);
                if (target != null && Player.Instance.Spellbook.GetSpell(SpellSlot.Q).ToggleState == 1 )
                    Spells.CastQ(target);
            }
            if (Manager.comboE)
            {
                var target = Target.GetChampionTarget(Spells.E.Range, DamageType.Magical);
                if (target != null && ((Manager.combolimitE && target.HasBuffOfType(BuffType.Slow)) || !Manager.combolimitE))
                    Spells.CastE(target);
            }
            if (Manager.comboW)
            {
                var target = Target.GetChampionTarget(Spells.W.Range, DamageType.Magical);
                if (target != null && ((Manager.combolimitW && target.HasBuffOfType(BuffType.Knockback)) || !Manager.combolimitW))
                    Spells.CastW(target);
            }
            if (Manager.comboR && Champion.CountEnemiesInRange(Spells.R.Range) >= Manager.combolimitR1 && !Champion.IsUnderTurret())
            {
                var target = Target.GetChampionTarget(Spells.R.Range, DamageType.Magical);
                if (target != null && ((Manager.combolimitR && !Spells.Q.IsReady() && !Spells.W.IsReady() && !Spells.E.IsReady()) || !Manager.combolimitR))
                    Spells.CastR(target);
            }
        }

        public static void HarassMode()
        {
            if (Champion.ManaPercent <= Manager.harrasM) return;
            if (Manager.harrasQ)
            {
                var target = Target.GetChampionTarget(Spells.Q.Range, DamageType.Magical, false, true);
                if (target != null && Player.Instance.Spellbook.GetSpell(SpellSlot.Q).ToggleState == 1)
                    Spells.CastQ(target);
            }
            if (Manager.harrasW)
            {
                var target = Target.GetChampionTarget(Spells.W.Range, DamageType.Magical, false, true);
                if (target != null && ((Manager.harraslimitW && target.HasBuffOfType(BuffType.Knockback)) || !Manager.harraslimitW))
                    Spells.CastW(target);
            }
            if (Manager.harrasE)
            {
                var target = Target.GetChampionTarget(Spells.E.Range, DamageType.Magical);
                if (target != null && ((Manager.harraslimitE && target.HasBuffOfType(BuffType.Slow)) || !Manager.harraslimitE))
                    Spells.CastE(target);
            }
        }

        public static void JungleMode()
        {        
            if (Champion.ManaPercent < Manager.jungleclearM) return;
            if (Manager.jungleclearQ)
            {
                var target = Target.GetMinionTarget(Spells.Q.Range, DamageType.Magical, false, true, true);
                if (target != null)
                    Spells.CastQ(target);
            }
            if (Manager.jungleclearW)
            {
                monsterExecuter("W", Manager.jungleclearlimitW);
            }
            if (Manager.jungleclearE)
            {
                monsterExecuter("E", Manager.jungleclearlimitE);
            }
        }
        public static void monsterExecuter(string spell, int minMinions)
        {
            int skillRange = 0, skillDelay = 0, skillWidth = 0, skillSpeed = 0;
            if (spell == "W")
            {
                skillRange = int.Parse(Spells.W.Range.ToString());
                skillDelay = Spells.W.CastDelay;
                skillWidth = Spells.W.Width;
                skillSpeed = Spells.W.Speed;
                var target = EntityManager.MinionsAndMonsters.GetLineFarmLocation(EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, skillRange), 150, (int)skillRange);
                if (target.HitNumber >= minMinions)
                {
                    Spells.CastSharpW(target.CastPosition);
                }
            }
            else if (spell == "E")
            {
                skillRange = int.Parse(Spells.E.Range.ToString());
                skillDelay = Spells.E.CastDelay;
                skillWidth = Spells.E.Width;
                var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters().Where(m => m.IsValidTarget(skillRange)).ToArray();
                if (minions.Length == 0) return;
                var farmLocation = Prediction.Position.PredictCircularMissileAoe(minions, skillRange, skillWidth, skillDelay, skillSpeed).OrderByDescending(r => r.GetCollisionObjects<Obj_AI_Minion>().Length).FirstOrDefault();
                if (farmLocation != null)
                {
                    var predictedMinion = farmLocation.GetCollisionObjects<Obj_AI_Minion>();
                    if (predictedMinion.Length >= minMinions)
                    {
                        Spells.CastSharpE(farmLocation.CastPosition);
                    }
                }
            }
        }
        public static void LaneClearMode()
        {
            if (Champion.ManaPercent < Manager.laneclearM) return;
            if (Manager.laneclearQ)
            {
                var target = Target.GetMinionTarget(Spells.Q.Range, DamageType.Magical, false, false, true);
                if (target != null)
                    Spells.CastQ(target);
            }
            if (Manager.laneclearW)
            {
                minionExecuter("W", Manager.laneclearlimitW);
            }
            if (Manager.laneclearE)
            {
                minionExecuter("E", Manager.laneclearlimitE);
            }
        }

        public static void minionExecuter(string spell, int minMinions, bool LastHit = false)
        {
            int skillRange = 0, skillDelay = 0, skillWidth = 0, skillSpeed = 0;
            if (spell == "W")
            {
                skillRange = int.Parse(Spells.W.Range.ToString());
                skillDelay = Spells.W.CastDelay;
                skillWidth = Spells.W.Width;
                skillSpeed = Spells.W.Speed;
                var target = EntityManager.MinionsAndMonsters.GetLineFarmLocation(EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.Position, skillRange), 150, (int)skillRange);
                if (target.HitNumber >= minMinions)
                {
                    Spells.CastSharpW(target.CastPosition);
                }
            }
            else if (spell == "E")
            {
                skillRange = int.Parse(Spells.E.Range.ToString());
                skillDelay = Spells.E.CastDelay;
                skillWidth = Spells.E.Width;
                var minions = EntityManager.MinionsAndMonsters.GetLaneMinions().Where(m => m.IsValidTarget(skillRange)).ToArray();
                if (minions.Length == 0) return;
                var farmLocation = Prediction.Position.PredictCircularMissileAoe(minions, skillRange, skillWidth, skillDelay, skillSpeed).OrderByDescending(r => r.GetCollisionObjects<Obj_AI_Minion>().Length).FirstOrDefault();
                if (farmLocation != null)
                {
                    var predictedMinion = farmLocation.GetCollisionObjects<Obj_AI_Minion>();
                    if (predictedMinion.Length >= minMinions)
                    {
                        Spells.CastSharpE(farmLocation.CastPosition);
                    }
                }
            }
        }

        public static void KillStealMode()
        {
            if (Manager.killstealQ)
            {
                var target = Target.GetChampionTarget(Spells.Q.Range, DamageType.Magical, false, true, Spells.QDamage());  
                if (target != null)
                    Spells.CastQ(target);
            }
            if (Manager.killstealW)
            {
                var target = Target.GetChampionTarget(Spells.W.Range, DamageType.Magical, false, false, Spells.WDamage());
                if (target != null)
                    Spells.CastW(target);
            }
            if (Manager.killstealE)
            {
                var target = Target.GetChampionTarget(Spells.W.Range, DamageType.Magical, false, false, Spells.WDamage());
                if (target != null)
                    Spells.CastE(target);
            }
            if (Manager.killstealR && Champion.CountEnemiesInRange(Spells.R.Range) >= Manager.killsteallimitR && !Champion.IsUnderTurret())
            {
                var target = Target.GetChampionTarget(Spells.R.Range, DamageType.Magical, false, false, Spells.RDamage());
                if (target != null)
                    Spells.CastR(target);
           }
        }

        public static void InterruptMode(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            if (!Manager.interrupterEnable) return;
            if (sender != null)
            {
                var target = Target.GetChampionTarget(Spells.E.Range, DamageType.Magical);
                if (target != null)
                    Spells.CastE(target);
            }
        }

        public static void GapCloserMode(Obj_AI_Base sender, Gapcloser.GapcloserEventArgs args)
        {
            if (!Manager.gapEnable) return;
            if (sender != null)
            {
                var target = Target.GetChampionTarget(Spells.E.Range, DamageType.Magical);
                if (target != null)
                    Spells.CastE(target);
            }
        }
    }
}
