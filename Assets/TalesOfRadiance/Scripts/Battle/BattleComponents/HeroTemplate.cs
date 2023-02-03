namespace TalesOfRadiance.Scripts.Battle.CoreClass
{
    public class HeroTemplate
    {
        public int Id;
        public string Name;
        public string ModelKey;

        public HeroTemplate(int id, string name, string modelKey)
        {
            Id = id;
            Name = name;
            ModelKey = modelKey;
        }
    }
}