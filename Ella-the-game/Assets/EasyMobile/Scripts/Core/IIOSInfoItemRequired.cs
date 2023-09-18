using System.Collections.Generic;

namespace EasyMobile
{
    public interface IIOSInfoItemRequired
    {
        List<iOSInfoPlistItem> GetIOSInfoPlistKeys();
    }
}
