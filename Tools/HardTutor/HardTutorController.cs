using System.Collections.Generic;
using UnityEngine;

namespace HardTutor
{
    public sealed class HardTutorController
    {
        private readonly IHardTutorRaycasterController hardTutorRaycasterController;
        private readonly IInstanceIdProvider instanceIdProvider;

        public bool IsActive { get; private set; }

        public HardTutorController(
            IHardTutorRaycasterController hardTutorRaycasterController,
            IInstanceIdProvider instanceIdProvider)
        {
            this.hardTutorRaycasterController = hardTutorRaycasterController;
            this.instanceIdProvider = instanceIdProvider;
        }

        public void StartHardTutor(List<string> ids)
        {
            if (IsActive)
            {
                Debug.LogError("We are trying to start Hard Tutor, but it is already started!");
                return;
            }

            var instancesId = instanceIdProvider.GetInstance(ids);

            IsActive = true;
            hardTutorRaycasterController.StartHardTutor(instancesId);
            Debug.Log("Hard Tutor start");
        }

        public void StopHardTutor()
        {
            if (!IsActive)
            {
                Debug.Log("We are trying to stop Hard Tutor, but it is not started!");
                return;
            }

            IsActive = false;
            hardTutorRaycasterController.StopHardTutor();
            Debug.Log("Hard Tutor stop");
        }

        public void Dispose()
        {
            if (IsActive)
            {
                StopHardTutor();
            }
        }
    }
}