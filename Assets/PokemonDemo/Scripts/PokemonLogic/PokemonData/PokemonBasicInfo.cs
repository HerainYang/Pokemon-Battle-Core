using PokemonDemo.Scripts.Enum;

namespace PokemonDemo.Scripts.PokemonLogic.PokemonData
{
    public class PokemonBasicInfo
    {
        public int PokemonID { get; }
        public int HpMax;
        public string Name { get; }

        public int Attack;
        public int Defence;

        public int SpecialAttack;
        public int SpecialDefence;
        public int Speed;

        public PokemonType Type;

        public readonly int[] SkillList;
        protected readonly int[] Abilities;

        public readonly string ImageKey;

        public PokemonBasicInfo(int id, string name, int hpMax, int attack, int defence, int specialAttack, int specialDefence, int speed, PokemonType type, int[] skillList, int[] abilities, string imageKey)
        {
            PokemonID = id;
            HpMax = hpMax;
            Name = name;

            Attack = attack;
            Defence = defence;
            Speed = speed;

            SpecialAttack = specialAttack;
            SpecialDefence = specialDefence;

            Type = type;

            SkillList = skillList;
            Abilities = abilities;

            ImageKey = imageKey;
        }

        protected PokemonBasicInfo(PokemonBasicInfo template) : this(template.PokemonID, template.Name, template.HpMax, template.Attack, template.Defence,
        template.SpecialAttack, template.SpecialDefence, template.Speed, template.Type, (int[])template.SkillList.Clone(), (int[])template.Abilities.Clone(), template.ImageKey) { }

        public int GetHpMax()
        {
            return HpMax;
        }

        public string GetName()
        {
            return Name;
        }

        public int GetSpeed()
        {
            return Speed;
        }
    }
}