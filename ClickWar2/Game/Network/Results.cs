using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickWar2.Game.Network
{
    public enum LoginResults : int
    {
        Success,
        Fail_NotUser,
        Fail_WrongPassword,
        Fail_AlreadyLogin,
        Fail_DifferentVersion,
        Fail_ServerNotReady,
    }

    public enum RegisterResults : int
    {
        Success,
        Fail_AlreadyExist,
        Fail_InvalidName,
        Fail_InvalidPassword,
    }

    public enum RegisterCompanyResults : int
    {
        Success,
        Fail_Unauthorized,
        Fail_NotEnoughResource,
        Fail_AlreadyExist,
        Fail_InvalidName,
    }

    public enum DevelopTechResults : int
    {
        Success,
        Fail_Unauthorized,
        Fail_NotEnoughResource,
        Fail_AlreadyExist,
        Fail_NotEnoughClearance,
        Fail_InvalidName,
        Fail_CompanyNotExist,
        Fail,
    }
}
