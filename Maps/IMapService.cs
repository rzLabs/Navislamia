using System;
using System.Collections.Generic;
using System.Text;

using Configuration;
using Scripting;
using Navislamia.Notification;
using Objects;

namespace Maps
{
    public interface IMapService
    {
        public bool Initialize(string directory);

        public KSize MapCount { get; set; }
    }
}
