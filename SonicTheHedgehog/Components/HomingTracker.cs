﻿using System.Linq;
using EntityStates;
using RoR2;
using RoR2.Audio;
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

        private CharacterBody characterBody;
        private InputBankTest inputBank;
        private TeamComponent teamComponent;
        private EntityStateMachine bodyState;

        private HurtBox trackingTarget;
        private HurtBox nearbyTarget;
        public bool isTrackingTargetClose;
        public bool enemiesNearby;

        private float trackerUpdateStopwatch;
        private float trackerUpdateFrequency = 10f;

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
            this.bodyState = EntityStateMachine.FindByCustomName(base.gameObject, "Body");
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
                this.trackerUpdateStopwatch = 0;
                System.Type stateType = this.bodyState.state.GetType();
                bool notTargetingState = typeof(Boost).IsAssignableFrom(stateType) || stateType == typeof(Death) || typeof(Parry).IsAssignableFrom(stateType) || typeof(GrandSlamSpin).IsAssignableFrom(stateType) || typeof(GrandSlamFinal).IsAssignableFrom(stateType) || typeof(TransformationBase).IsAssignableFrom(stateType);
                if (notTargetingState)
                {
                    this.indicator.targetTransform = null;
                }
                else
                {
                    EnemiesNearby();
                    this.SearchForTarget(inputBank.GetAimRay());
                    this.indicator.targetTransform = CanHomingAttack() ? this.trackingTarget.transform : null;
                }
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
            this.trackingTarget = this.search.GetResults().FirstOrDefault<HurtBox>();
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
            this.sphereSearch.radius = 3;
            this.sphereSearch.mask = LayerIndex.entityPrecise.mask;
            this.sphereSearch.RefreshCandidates();
            this.sphereSearch.FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(teamComponent.teamIndex));
            this.nearbyTarget = sphereSearch.GetHurtBoxes().FirstOrDefault<HurtBox>();
            this.enemiesNearby = nearbyTarget;
            if (nearbyTarget && Vector3.Dot((this.nearbyTarget.transform.position - characterBody.transform.position).normalized, inputBank.GetAimRay().direction) < 0.2f)
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