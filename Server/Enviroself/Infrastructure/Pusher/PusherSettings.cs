using PusherServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Enviroself.Infrastructure.Pusher
{
    public class PusherSettings
    {
        public string AppId { get; set; }
        public string AppKey { get; set; }
        public string AppSecret { get; set; }
        public PusherOptions Options { get; set; } = new PusherOptions();
    }
}