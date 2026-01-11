// Author MikeNspired.
// Modified to fix event listener issues, support State grab mode,
// and make magazine insertion more forgiving and easier.

using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace MikeNspired.XRIStarterKit
{
    public class MagazineAttachPointModified : MonoBehaviour
    {
        [SerializeField] private Transform start, end;
        [SerializeField] private float alignAnimationLength = 0.05f;
        [SerializeField] private float insertAnimationLength = 0.1f;
        [SerializeField] private AudioSource loadAudio, unloadAudio;
        [SerializeField] private GunType gunType = null;
        [SerializeField] private Magazine startingMagazine = null;
        [SerializeField] private new Collider collider = null;
        [SerializeField] private bool removeByGrabbing = true;

        private XRGrabInteractable xrGrabInteractable;
        private XRInteractionManager interactionManager;
        private Magazine magazine;
        private bool ammoIsAttached;

        public Magazine Magazine => magazine;
        public GunType GunType => gunType;

        private void Awake()
        {
            OnValidate();

            xrGrabInteractable.selectEntered.AddListener(_ => UpdateMagazineState());
            xrGrabInteractable.selectExited.AddListener(_ => UpdateMagazineState());

            collider.gameObject.SetActive(false);

            if (startingMagazine)
                CreateStartingMagazine();
        }

        private void UpdateMagazineState()
        {
            CancelInvoke();
            Invoke(nameof(ApplyMagazineState), Time.deltaTime);
            Invoke(nameof(UpdateAttachCollider), Time.deltaTime);
        }

        private void UpdateAttachCollider()
        {
            bool gunHeld = xrGrabInteractable.interactorsSelecting.Count > 0;
            collider.gameObject.SetActive(gunHeld);
        }

        private void ApplyMagazineState()
        {
            if (!magazine) return;

            bool gunHeld = xrGrabInteractable.interactorsSelecting.Count > 0;

            if (removeByGrabbing && gunHeld)
                magazine.EnableCollider();
            else
                magazine.DisableCollider();
        }

        private void OnValidate()
        {
            if (!xrGrabInteractable)
                xrGrabInteractable = GetComponentInParent<XRGrabInteractable>();

            if (!interactionManager)
                interactionManager = FindFirstObjectByType<XRInteractionManager>();
        }

        private void CreateStartingMagazine()
        {
            if (magazine) return;

            SetupNewMagazine(Instantiate(startingMagazine, end.position, end.rotation, transform));
            magazine.DisableCollider();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (ammoIsAttached) return;

            var collidedMagazine = other.attachedRigidbody?.GetComponent<Magazine>();
            if (!collidedMagazine) return;
            if (collidedMagazine.GunType != gunType) return;
            if (collidedMagazine.CurrentAmmo <= 0) return;

            // Only requirement: the gun must be held.
            bool gunHeld = xrGrabInteractable.interactorsSelecting.Count > 0;
            if (!gunHeld) return;

            // Release magazine from hand if still held
            ReleaseFromHand(collidedMagazine);

            // Attach magazine
            SetupNewMagazine(collidedMagazine);

            // Start animation
            StartCoroutine(StartAnimation(other.attachedRigidbody.transform));
        }

        private void SetupNewMagazine(Magazine mag)
        {
            magazine = mag;

            var interactable = magazine.GetComponent<XRGrabInteractable>();
            interactable.selectEntered.AddListener(OnMagazineGrabbed);

            magazine.SetupForGunAttachment();
            magazine.transform.parent = transform;

            ammoIsAttached = true;
        }

        private void OnMagazineGrabbed(SelectEnterEventArgs args)
        {
            AmmoRemoved();
        }

        private void ReleaseFromHand(Magazine collidedMagazine)
        {
            var interactable = collidedMagazine.GetComponent<XRGrabInteractable>();
            var interactor = interactable.firstInteractorSelecting;

            if (interactor != null)
                interactionManager.SelectExit(interactor, interactable);
        }

        private void AmmoRemoved()
        {
            StopAllCoroutines();
            CancelInvoke();

            if (magazine != null)
            {
                var interactable = magazine.GetComponent<XRGrabInteractable>();
                interactable.selectEntered.RemoveListener(OnMagazineGrabbed);
            }

            magazine = null;
            unloadAudio.Play();

            Invoke(nameof(ResetAttachState), 0.15f);
        }

        private void ResetAttachState()
        {
            ammoIsAttached = false;
        }

        private IEnumerator StartAnimation(Transform ammo)
        {
            yield return AnimateTransform(ammo, start.localPosition, start.localRotation, alignAnimationLength);
            loadAudio.Play();
            yield return AnimateTransform(ammo, end.localPosition, end.localRotation, insertAnimationLength);
        }

        public void EjectMagazine()
        {
            if (magazine == null) return;

            StopAllCoroutines();
            StartCoroutine(EjectMagazineAnimation(magazine.transform));
        }

        private IEnumerator EjectMagazineAnimation(Transform ammo)
        {
            unloadAudio.Play();
            yield return AnimateTransform(ammo, start.localPosition, start.localRotation, insertAnimationLength);

            if (magazine != null)
            {
                var interactable = magazine.GetComponent<XRGrabInteractable>();
                interactable.selectEntered.RemoveListener(OnMagazineGrabbed);
                magazine.ResetToGrabbableObject();
                magazine = null;
            }

            ammoIsAttached = false;
            collider.gameObject.SetActive(true);
        }

        private IEnumerator AnimateTransform(Transform target, Vector3 targetPosition, Quaternion targetRotation, float duration)
        {
            float timer = 0;
            var startPosition = target.localPosition;
            var startRotation = target.localRotation;

            while (timer < duration)
            {
                float progress = timer / duration;
                target.localPosition = Vector3.Lerp(startPosition, targetPosition, progress);
                target.localRotation = Quaternion.Lerp(startRotation, targetRotation, progress);
                timer += Time.deltaTime;
                yield return null;
            }

            target.localPosition = targetPosition;
            target.localRotation = targetRotation;
        }

        private void OnDestroy()
        {
            if (magazine != null && magazine.TryGetComponent(out XRGrabInteractable interactable))
                interactable.selectEntered.RemoveListener(OnMagazineGrabbed);
        }
    }
}
