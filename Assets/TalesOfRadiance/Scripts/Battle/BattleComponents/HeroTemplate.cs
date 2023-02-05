namespace TalesOfRadiance.Scripts.Battle.BattleComponents
{
    public class HeroTemplate
    {
        public int Id;
        public string Name;
        public string ModelKey;

        // Attributes
        // Basic
        public int Attack;
        public int MaxHealth;
        public int Defence;
        public int Speed;
        
        //Special
        public float CriticalRate;
        public float CriticalDamage;
        public float ControlRate;
        public float AntiControl;
        public float AntiCritical;
        public float Accuracy;
        public float DamageAvoid;
        public float DodgeRate;
        public float HealRate;
        public float GetHealRate;
        public float DamageIncrease;
        public float PhysicalDamageIncrease;
        public float SpecialDamageIncrease;
        public float PhysicalDamageAvoid;
        public float SpecialDamageAvoid;
        public float SustainDamageIncrease;
        public float SustainDamageAvoid;

        public int[] SkillIndices;

        public HeroTemplate(int id, string name, string modelKey)
        {
            Id = id;
            Name = name;
            ModelKey = modelKey;
        }
    }
}