using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;


namespace RPG.Abilities.Targeting
{
    [CreateAssetMenu(fileName = "Delayed Click Targeting", menuName = "Abilities/Targeting/Delayed Click", order = 0)]
    public class DelayedClickTargeting : TargetingStrategy
    {
        [SerializeField] private Texture2D cursorTexture;
        [SerializeField] private Vector2 cursorHotspot;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float areaOfEffectRadius;
        [SerializeField] private Transform targetingPrefab;
        private Transform targetingPrefabInstance = null;

        public override void StartTargeting(AbilityData abilityData, Action finished)
        {
            PlayerController playerController = abilityData.User.GetComponent<PlayerController>();
            playerController.StartCoroutine(Targeting(abilityData, playerController, finished));
        }

        private IEnumerator Targeting(AbilityData abilityData, PlayerController playerController, Action finished)
        {
            playerController.enabled = false;

            if (targetingPrefabInstance == null)
                targetingPrefabInstance = Instantiate(targetingPrefab);
            else
                targetingPrefabInstance.gameObject.SetActive(true);

            targetingPrefabInstance.localScale = new Vector3(areaOfEffectRadius * 2, 1, areaOfEffectRadius * 2);

            while (!abilityData.IsCancelled)
            {
                Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);

                RaycastHit raycastHit;
                if (Physics.Raycast(PlayerController.GetMouseRay(), out raycastHit, 1000, layerMask))
                {
                    targetingPrefabInstance.position = raycastHit.point;

                    if (Input.GetMouseButtonDown(0))
                    {
                        yield return new WaitWhile(() => Input.GetMouseButton(0));

                        abilityData.TargetedPoint = raycastHit.point;
                        abilityData.Targets = GetGameObjectsInRadius(abilityData.TargetedPoint);
                        break;
                    }
                    if (Input.GetMouseButtonDown(1))
                    {
                        playerController.enabled = true;
                        targetingPrefabInstance.gameObject.SetActive(false);
                        yield break;
                    }
                }

                yield return null;
            }

            playerController.enabled = true;
            targetingPrefabInstance.gameObject.SetActive(false);
            finished();
        }

        private IEnumerable<GameObject> GetGameObjectsInRadius(Vector3 point)
        {

            var hits = Physics.SphereCastAll(point, areaOfEffectRadius, Vector3.up, 0);

            foreach (var hit in hits)
            {
                yield return hit.collider.gameObject;
            }

        }
    }
}
