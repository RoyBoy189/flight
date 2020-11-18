using Verse;

namespace Abilitiesforme
{
    public class CompProperties_HarpyComp : CompProperties
    {
        public CompProperties_HarpyComp()
        {
            this.compClass = typeof(HarpyComp);
        }

        public float cooldown = 5f;

        public float range = 40f;
    }
}