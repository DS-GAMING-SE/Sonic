using System.Linq;
using EntityStates;
using HedgehogUtils.Miscellaneous;
using RoR2;
using RoR2.Audio;
using SonicTheHedgehog.Modules.Survivors;
using SonicTheHedgehog.SkillStates;
using UnityEngine;
using UnityEngine.UI;

namespace SonicTheHedgehog.Components
{
    [RequireComponent(typeof(InputBankTest))]
    [RequireComponent(typeof(CharacterBody))]
    [RequireComponent(typeof(TeamComponent))]
    public class HomingTracker : MonoBehaviour
    {
        private Indicator indicator;
        private LockOnIndicator lockOnIndicator;
        private bool lockOnIndicatorFound;
        
        public bool visible;
        public bool locked
        {
            get { return _locked; }
            set
            {
                _locked = value;
                if (visible)
                {
                    IndicatorRetarget();
                }
            }
        }
        private bool _locked;

        private CharacterBody characterBody;
        private InputBankTest inputBank;
        private TeamComponent teamComponent;

        private Transform previousTarget;
        private HurtBox trackingTarget;
        private HurtBox trackingTargetLaunched;
        private HurtBox nearbyTarget;
        public bool isTrackingTargetClose;
        public bool enemiesNearby;

        private float trackerUpdateStopwatch;
        private float trackerUpdateFrequency = 10f;

        private BullseyeSearch search;
        private SphereSearch sphereSearch;

        private void Awake()
        {
            this.indicator = new Indicator(base.gameObject, HedgehogUtils.Assets.lockOnIndicator); //LegacyResourcesAPI.Load<GameObject>("Prefabs/HuntressTrackingIndicator")
            this.characterBody = base.GetComponent<CharacterBody>();
            this.inputBank = base.GetComponent<InputBankTest>();
            this.teamComponent = base.GetComponent<TeamComponent>();
        }
        private void Start()
        {
            search = new BullseyeSearch();
            sphereSearch = new SphereSearch();
        }

        public HurtBox GetTrackingTarget()
        {
            return this.trackingTarget;
        }

        public HurtBox GetTrackingTarget(bool allowTargetingLaunched)
        {
            if (allowTargetingLaunched && trackingTargetLaunched)
            {
                return this.trackingTargetLaunched;
            }
            return this.trackingTarget;
        }

        private void OnEnable()
        {
            this.indicator.active = true;
            if (this.indicator.hasVisualizer)
            {
                InitializeIndicator();
            }
        }
        private void InitializeIndicator()
        {
            this.lockOnIndicator = this.indicator.visualizerInstance.GetComponent<LockOnIndicator>();
            if (lockOnIndicator)
            {
                lockOnIndicatorFound = true;
                lockOnIndicator.SetColors(SonicTheHedgehogCharacter.sonicColor2, SonicTheHedgehogCharacter.sonicColor);
            }
        }
        public void SetColors(Color color, Color color2)
        {
            if (lockOnIndicator)
            {
                lockOnIndicator.SetColors(color, color2);
            }
        }
        private void OnDisable()
        {
            this.indicator.active = false;
        }

        private void FixedUpdate()
        {
            if (!Util.HasEffectiveAuthority(base.gameObject)) return;
            if (!lockOnIndicatorFound && this.indicator.hasVisualizer) { InitializeIndicator(); }
            this.trackerUpdateStopwatch += Time.fixedDeltaTime;
            if (this.trackerUpdateStopwatch >= 1f / this.trackerUpdateFrequency)
            {
                this.trackerUpdateStopwatch = 0;
                EnemiesNearby();
                this.SearchForTarget(inputBank.GetAimRay());
                if (visible)
                {
                    if (!locked)
                    {
                        IndicatorRetarget();
                    }
                }
                else
                {
                    this.indicator.targetTransform = null;
                }
                previousTarget = this.indicator.targetTransform;
            }
        }
        private void IndicatorRetarget()
        {
            this.indicator.targetTransform = CanHomingAttack() ? this.trackingTarget.transform : null;
            if (this.lockOnIndicator && this.indicator.targetTransform && (!previousTarget || this.indicator.targetTransform != previousTarget))
            {
                this.lockOnIndicator.UpdateTarget();
                Util.PlaySound("Play_hedgehogutils_lockon", base.gameObject);
            }
        }

        private void SearchForTarget(Ray aimRay)
        {
            this.search.teamMaskFilter = TeamMask.GetUnprotectedTeams(this.teamComponent.teamIndex);
            this.search.filterByLoS = true;
            this.search.searchOrigin = aimRay.origin;
            this.search.searchDirection = aimRay.direction;
            this.search.sortMode = BullseyeSearch.SortMode.Angle;
            this.search.maxDistanceFilter = MaxRange();
            this.search.minDistanceFilter = 0;
            this.search.maxAngleFilter = 12;
            this.search.RefreshCandidates();
            this.search.FilterOutGameObject(base.gameObject);
            this.trackingTargetLaunched = this.search.GetResults().FirstOrDefault(hurt => hurt && hurt.healthComponent && hurt.healthComponent.body && hurt.healthComponent.alive && hurt.healthComponent.body.HasBuff(HedgehogUtils.Buffs.launchedBuff));
            this.trackingTarget = this.search.GetResults().FirstOrDefault(hurt => hurt && hurt.healthComponent && hurt.healthComponent.body && hurt.healthComponent.alive);
            /*this.search.GetResults().ToList().ForEach(hurt => 
            {
                if (hurt && hurt.healthComponent && hurt.healthComponent.body && hurt.healthComponent.alive)
                {
                    if (hurt.healthComponent.body.HasBuff(HedgehogUtils.Buffs.launchedBuff) && !trackingTargetLaunched)
                    {
                        trackingTargetLaunched = hurt;
                    }
                    else if (!trackingTarget)
                    {
                        trackingTarget = hurt;
                    }
                }
                if (trackingTarget && trackingTargetLaunched)
                {
                    return;
                }
            });*/
            if (this.trackingTarget != null)
            {
                this.isTrackingTargetClose = 8 >= Mathf.Abs(Vector3.Magnitude(characterBody.transform.position - this.trackingTarget.transform.position));
            }
            else if (nearbyTarget != null)
            {
                this.trackingTarget = nearbyTarget;
                this.isTrackingTargetClose = true;
            }
        }

        public void EnemiesNearby()
        {
            this.sphereSearch.origin = characterBody.transform.position + this.inputBank.aimDirection;
            this.sphereSearch.radius = 5;
            this.sphereSearch.mask = LayerIndex.entityPrecise.mask;
            this.sphereSearch.RefreshCandidates();
            this.sphereSearch.FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(teamComponent.teamIndex));
            this.nearbyTarget = sphereSearch.GetHurtBoxes().FirstOrDefault<HurtBox>();
            this.enemiesNearby = nearbyTarget;
            if (nearbyTarget && Vector3.Dot((this.nearbyTarget.transform.position - characterBody.transform.position).normalized, inputBank.GetAimRay().direction) < -0.2f)
            {
                this.nearbyTarget = null;
            }
        }

        public bool CanHoming()
        {
            return characterBody.moveSpeed > 0 && trackingTarget != null;
        }

        public bool CanHomingAttack()
        {
            return CanHoming() && !this.enemiesNearby && !this.isTrackingTargetClose;
        }

        public float MaxRange()
        {
            return 15f + characterBody.moveSpeed * 2.5f * (characterBody.isSprinting ? 1 : characterBody.sprintingSpeedMultiplier);
        }

        public float Speed()
        {
            return characterBody.moveSpeed * 5f * (characterBody.isSprinting ? 1 : characterBody.sprintingSpeedMultiplier);
        }
    }
}