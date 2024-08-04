using System.Collections.Generic;

namespace HardTutor
{
    internal class HardTutorRaycasterController : IHardTutorRaycasterController
    {
        public void StartHardTutor(IReadOnlyList<int> instanceIds)
        {
            HardTutorRaycaster.StartHardTutor(instanceIds);
        }

        public void StopHardTutor()
        {
            HardTutorRaycaster.StopHardTutor();
        }
    }
}