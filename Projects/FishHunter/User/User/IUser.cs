﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter
{
    public interface IUser : Regulus.Utility.IUpdatable
    {
        Regulus.Remoting.User Remoting { get; }
    }
}
