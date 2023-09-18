using System.Collections.Generic;

namespace EasyMobile
{
    public interface IAndroidPermissionRequired
    {
        List<AndroidPermission> GetAndroidPermissions();
    }
}
