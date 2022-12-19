using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMobile
{
    public enum ConsentStatus
    {
        /// <summary>
        /// Consent status is not set, subject to request from user.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Consent is granted by the user.
        /// </summary>
        Granted,

        /// <summary>
        /// Consent is revoked by the user.
        /// </summary>
        Revoked,
    }
}
