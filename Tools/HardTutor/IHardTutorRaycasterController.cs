using System.Collections.Generic;

namespace HardTutor
{
    public interface IHardTutorRaycasterController
    {
        void StartHardTutor(IReadOnlyList<int> instanceIds);
        void StopHardTutor();
    }
}