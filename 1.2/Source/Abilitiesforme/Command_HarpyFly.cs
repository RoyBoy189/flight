using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace Abilitiesforme
{
    [StaticConstructorOnStartup]
    public class Command_HarpyFly : Command, ITargetingSource
    {
        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            HarpyComp harpyComp = this.pawn.TryGetComp<HarpyComp>();
            SoundDefOf.Tick_Tiny.PlayOneShotOnCamera(null);
            Find.Targeter.BeginTargeting(this, null);
        }

        public TargetingParameters targetParams
        {
            get
            {
                TargetingParameters targetingParameters = new TargetingParameters();
                targetingParameters.canTargetLocations = true;
                targetingParameters.canTargetSelf = false;
                targetingParameters.canTargetPawns = false;
                targetingParameters.canTargetFires = false;
                targetingParameters.canTargetBuildings = false;
                targetingParameters.canTargetItems = false;
                targetingParameters.validator = ((TargetInfo x) => DropCellFinder.IsGoodDropSpot(x.Cell, x.Map, true, false, false));
                return targetingParameters;
            }
        }

        public override void GizmoUpdateOnMouseover()
        {
            bool flag = Find.CurrentMap != null;
            if (flag)
            {
                bool flag2 = this.abilityRange > 0f;
                if (flag2)
                {
                    GenDraw.DrawRadiusRing(this.pawn.Position, this.abilityRange);
                }
            }
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth)
        {
            this.disabled = false;
            HarpyComp harpyComp = this.pawn.TryGetComp<HarpyComp>();
            bool flag = harpyComp.cooldownTicks > 180f;
            if (flag)
            {
                this.DisableWithReason("Fly_Cooldown".Translate());
            }
            bool flag2 = this.pawn == null;
            if (flag2)
            {
                this.DisableWithReason("Fly_NoPawn".Translate());
            }
            bool flag3 = this.pawn.IsBurning();
            if (flag3)
            {
                this.DisableWithReason("Fly_OnFire".Translate());
            }
            bool dead = this.pawn.Dead;
            if (dead)
            {
                this.DisableWithReason("Fly_Dead".Translate());
            }
            bool inMentalState = this.pawn.InMentalState;
            if (inMentalState)
            {
                this.DisableWithReason("Fly_MentalState".Translate());
            }
            bool flag4 = this.pawn.Downed || this.pawn.stances.stunner.Stunned || !this.pawn.Awake();
            if (flag4)
            {
                this.DisableWithReason("Fly_Downed".Translate());
            }
            bool flag5 = !HarpyUtility.FlightCapabable(this.pawn);
            if (flag5)
            {
                this.DisableWithReason("Mech_Wings".Translate());
            }
            GizmoResult result = base.GizmoOnGUI(topLeft, maxWidth);
            bool flag6 = harpyComp != null && harpyComp.cooldownTicks > 0f;
            if (flag6)
            {
                CompProperties_HarpyComp compProperties_HarpyComp = harpyComp.props as CompProperties_HarpyComp;
                float value = Mathf.InverseLerp(0f, compProperties_HarpyComp.cooldown * 60f, harpyComp.cooldownTicks);
                Rect rect = new Rect(topLeft.x, topLeft.y, this.GetWidth(maxWidth), 75f);
                Widgets.FillableBar(rect, Mathf.Clamp01(value), Command_HarpyFly.cooldownBarTex, null, false);
                Text.Font = GameFont.Tiny;
                Text.Anchor = TextAnchor.UpperCenter;
                Widgets.Label(rect, (harpyComp.cooldownTicks / 60f).ToStringByStyle(ToStringStyle.Integer, ToStringNumberSense.Absolute));
                Text.Anchor = TextAnchor.UpperLeft;
            }
            return result;
        }

        protected void DisableWithReason(string reason)
        {
            this.disabledReason = reason;
            this.disabled = true;
        }

        public bool MultiSelect
        {
            get
            {
                return false;
            }
        }

        public Thing Caster
        {
            get
            {
                return this.pawn;
            }
        }

        public Pawn CasterPawn
        {
            get
            {
                return this.pawn;
            }
        }

        public Verb GetVerb
        {
            get
            {
                return null;
            }
        }

        public bool CasterIsPawn
        {
            get
            {
                return true;
            }
        }

        public bool IsMeleeAttack
        {
            get
            {
                return false;
            }
        }

        public bool Targetable
        {
            get
            {
                return true;
            }
        }

        public Texture2D UIIcon
        {
            get
            {
                return this.icon;
            }
        }

        public ITargetingSource DestinationSelector
        {
            get
            {
                return null;
            }
        }

        public bool CanHitTarget(LocalTargetInfo target)
        {
            return this.pawn.Map != null && target.Cell.DistanceTo(this.pawn.Position) <= this.abilityRange && target.Cell.Standable(this.pawn.Map) && target.Cell.InBounds(this.pawn.Map) && !target.Cell.Roofed(this.pawn.Map);
        }

        public bool ValidateTarget(LocalTargetInfo target)
        {
            bool flag = !target.Cell.Standable(this.pawn.Map) || !target.Cell.InBounds(this.pawn.Map);
            bool result;
            if (flag)
            {
                Messages.Message("Fly_InvalidCell".Translate(), MessageTypeDefOf.RejectInput, false);
                result = false;
            }
            else
            {
                bool flag2 = target.Cell.DistanceTo(this.pawn.Position) > this.abilityRange;
                if (flag2)
                {
                    Messages.Message("Fly_Distance".Translate(), MessageTypeDefOf.RejectInput, false);
                    result = false;
                }
                else
                {
                    bool flag3 = target.Cell.Roofed(this.pawn.Map);
                    if (flag3)
                    {
                        Messages.Message("Fly_RoofTarget".Translate(), MessageTypeDefOf.RejectInput, false);
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        public void DrawHighlight(LocalTargetInfo target)
        {
            GenDraw.DrawRadiusRing(this.pawn.Position, this.abilityRange);
            bool flag = target.IsValid && !target.Cell.Roofed(this.pawn.Map);
            if (flag)
            {
                GenDraw.DrawTargetHighlight(target);
            }
        }

        public void OnGUI(LocalTargetInfo target)
        {
            bool isValid = target.IsValid;
            if (isValid)
            {
                GenUI.DrawMouseAttachment(this.UIIcon);
            }
            else
            {
                GenUI.DrawMouseAttachment(TexCommand.CannotShoot);
            }
        }

        public void OrderForceTarget(LocalTargetInfo target)
        {
            this.SetTarget(target);
            this.QueueCastingJob(target);
        }

        public virtual void QueueCastingJob(LocalTargetInfo target)
        {
            HarpyComp harpyComp = this.pawn.TryGetComp<HarpyComp>();
            bool flag = harpyComp != null && harpyComp.cooldownTicks > 0f;
            if (!flag)
            {
                Job job = JobMaker.MakeJob(GitThingDefOf.HarpyJob_Flyability);
                job.targetA = target;
                this.pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }
        }

        public void SetTarget(LocalTargetInfo target)
        {
            this.targetCell = target.Cell;
        }

        public virtual void SelectDestination()
        {
            Find.Targeter.BeginTargeting(this, null);
        }

        public IntVec3 targetCell;

        public Pawn pawn;

        public float abilityRange;

        private static readonly Texture2D cooldownBarTex = SolidColorMaterials.NewSolidColorTexture(new Color32(192, 192, 192, 64));

        [NoTranslate]
        public static string IconPath = "Things/Special/FlyIcon";
    }
}