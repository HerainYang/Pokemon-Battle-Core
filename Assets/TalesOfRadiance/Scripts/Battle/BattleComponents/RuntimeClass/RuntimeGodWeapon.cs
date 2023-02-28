using TalesOfRadiance.Scripts.Battle.Managers;
using TalesOfRadiance.Scripts.Character;

namespace TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass
{
    public class RuntimeGodWeapon : AtorBattleEntity
    {
        public CharacterAnchor Anchor;
        public HeroTemplate Template;
        
        public RuntimeGodWeapon(HeroTemplate template, CharacterTeam team, CharacterAnchor anchor) : base(team)
        {
            Template = template;
            Anchor = anchor;
        }

        public override void LoadBattleMoveBp()
        {
            BattleMgr.Instance.AddBattleEntityMove(this);
        }

        public override SkillTemplate MakeBattleDecision()
        {
            throw new System.NotImplementedException();
        }
    }
}