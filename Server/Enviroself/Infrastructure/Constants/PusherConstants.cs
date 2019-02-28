using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Enviroself.Infrastructure.Constants
{
    public class PusherConstants
    {
        public static string CHANNEL_NAME { get; } = "channer";
        public static string LIKE_EVENT_NAME { get; } = "new-like";
        public static string USER_STATUS_EVENT_NAME { get; } = "user-status";
    }
}
