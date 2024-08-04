using System.Collections.Generic;

namespace HardTutor
{
    //B
    public interface IInstanceIdProvider
    {
        List<int> GetInstance(List<string> ids);
    }
}