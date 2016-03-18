using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickWar2.Game.Network
{
    public enum MessageTypes : int
    {
        Req_Login = 10,
        Rsp_Login,
        
        Req_Logout,
        Rsp_Logout,

        Req_Register,
        Rsp_Register,

        Req_Notice,
        Ntf_Notice,

        Req_CountryLocation,
        Rsp_CountryLocation,

        Req_NewTerritory,
        Rsp_NewTerritory,

        Req_AddTilePower,
        Ntf_SetTilePower,

        Req_Chunk,
        Rsp_Chunk,

        Req_BuildCountry,
        Ntf_CountryBuilt,

        Req_AttackTerritory,
        Ntf_AttackTerritory,

        Req_Vision,
        Rsp_Vision,

        Req_AllTerritory,
        Rsp_AllTerritory,

        Req_AllVision,
        Rsp_AllVision,

        Req_SendPower,
        Ntf_SendPower,

        Req_EditTileSign,
        Ntf_EditTileSign,

        Req_UserColor,
        Rsp_UserColor,

        Ntf_UserLogin,
        Ntf_UserLogout,

        Req_AllUserInfo,
        Rsp_AllUserInfo,

        Ntf_UserEnd,

        Req_BuildFactory,
        Ntf_BuildFactory,

        Req_ConvertAllResource,
        Ntf_ConvertAllResource,

        Req_DestroyFactory,
        Ntf_DestroyFactory,

        Req_SendMail,

        Req_Mailbox,
        Ntf_ReceiveMail,

        Req_ReadMail,

        Req_RegisterCompany,
        Rsp_RegisterCompany,

        Req_BuildCompany,
        Ntf_BuildCompany,

        Req_MyAllCompanyName,
        Rsp_MyAllCompanyName,

        Req_DestroyCompany,
        Ntf_DestroyCompany,

        Req_MyAllCompanySiteCount,
        Rsp_MyAllCompanySiteCount,

        Req_DevelopTech,
        Rsp_DevelopTech,

        Req_MyAllCompanyTechList,
        Rsp_MyAllCompanyTechList,

        Req_DiscardTech,
        Rsp_DiscardTech,

        Req_ProduceProduct,
        Rsp_ProduceProduct,

        Req_MyAllCompanyProductList,
        Rsp_MyAllCompanyProductList,

        Req_DiscardProduct,
        Rsp_DiscardProduct,

        Req_BuildChip,
        Ntf_BuildChip,

        Req_DestroyChip,
        Ntf_DestroyChip,

        Req_TechProgram,
        Rsp_TechProgram,

        Req_SellTech,
        Ntf_SellTech,

        Req_AllSellingTech,

        Req_BuyTech,
        Ntf_BuyTech,

        Ntf_CheckUser,

        Req_SetScreen,
        Rsp_SetScreen,

        Req_SellProduct,
        Ntf_SellProduct,

        Req_AllSellingProduct,

        Req_BuyProduct,
        Ntf_BuyProduct,


    }
}
