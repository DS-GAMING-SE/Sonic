using System.Linq;
using RoR2;
using RoR2.Audio;
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

        private CharacterBody characterBody;
        private InputBankTest inputBank;
        private TeamComponent teamComponent;

        private HurtBox trackingTarget;

        private float trackerUpdateStopwatch;
        private float trackerUpdateFrequency = 6;

        private float trackerMaxDistance;

        private readonly BullseyeSearch search = new BullseyeSearch();
        private readonly SphereSearch sphereSearch = new SphereSearch();

        private void Awake()
        {
            this.indicator = new Indicator(base.gameObject, LegacyResourcesAPI.Load<GameObject>("Prefabs/HuntressTrackingIndicator"));
        }
        private void Start()
        {
            this.characterBody = base.GetComponent<CharacterBody>();
            this.inputBank = base.GetComponent<InputBankTest>();
            this.teamComponent = base.GetComponent<TeamComponent>();
        }

        public HurtBox GetTrackingTarget()
        {
            return this.trackingTarget;
        }

        private void OnEnable()
        {
            this.indicator.active = true;
        }
        private void OnDisable()
        {
            this.indicator.active = false;
        }

        private void FixedUpdate()
        {
            this.trackerUpdateStopwatch += Time.fixedDeltaTime;
            if (this.trackerUpdateStopwatch >= 1f / this.trackerUpdateFrequency)
            {
                this.trackerUpdateStopwatch -= 1f / this.trackerUpdateFrequency;
                HurtBox hurtBox = this.trackingTarget;
                Ray aimRay;
                aimRay = new Ray(this.inputBank.aimOrigin, this.inputBank.aimDirection);
                this.SearchForTarget(aimRay);
                this.indicator.targetTransform = (this.trackingTarget ? this.trackingTarget.transform : null);
            }
        }

        private void SearchForTarget(Ray aimRay)
        {
            TeamMask mask = TeamMask.GetEnemyTeams(teamComponent.teamIndex);
            this.sphereSearch.origin = characterBody.transform.position + this.inputBank.aimDirection;
            this.sphereSearch.radius = 3;
            this.sphereSearch.mask = LayerIndex.entityPrecise.mask;
            this.sphereSearch.RefreshCandidates();
            this.sphereSearch.FilterCandidatesByHurtBoxTeam(mask);

            if (sphereSearch.GetHurtBoxes().Any())
            {
                trackingTarget = null;
                return;
            }

            this.trackerMaxDistance = 15f + (characterBody.moveSpeed * characterBody.sprintingSpeedMultiplier) * 2f;
            this.search.teamMaskFilter = TeamMask.GetUnprotectedTeams(this.teamComponent.teamIndex);
            this.search.filterByLoS = true;
            this.search.searchOrigin = aimRay.origin;
            this.search.searchDirection = aimRay.direction;
            this.search.sortMode = BullseyeSearch.SortMode.Angle;
            this.search.maxDistanceFilter = this.trackerMaxDistance;
            this.search.minDistanceFilter = 8;
            this.search.maxAngleFilter = 10;
            this.search.RefreshCandidates();
            this.search.FilterOutGameObject(base.gameObject);
            this.trackingTarget = this.search.GetResults().FirstOrDefault<HurtBox>();
        }

        public bool CanHomingAttack()
        {
            return characterBody.moveSpeed > 0 && characterBody.sprintingSpeedMultiplier > 0 && trackingTarget != null;
        }
    }
}