using System;
using System.Collections.Generic;
using System.Text;

using Configuration;
using Scripting;
using Notification;
using Objects;

namespace Maps
{
    public interface IMapService
    {
        public bool Initialize();

        public KSize MapCount { get; set; }
    }
}
