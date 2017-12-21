using MateralTools.MEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KotenBu.Model
{
    /// <summary>
    /// 权限修改模型
    /// </summary>
    public sealed class EditPermissionsInModel : T_Permissions, IVerificationLoginModel
    {
        /// <summary>
        /// 登录用户唯一标识
        /// </summary>
        public Guid LoginUserID { get; set; }
        /// <summary>
        /// 登录用户Token
        /// </summary>
        public string LoginUserToken { get; set; }
    }
    /// <summary>
    /// 权限类型枚举
    /// </summary>
    public enum PermissionsTypesEnum : byte
    {
        /// <summary>
        /// 后台菜单
        /// </summary>
        [EnumShowName("后台菜单")]
        Menu = 0,
        /// <summary>
        /// 功能
        /// </summary>
        [EnumShowName("功能")]
        Function = 1,
        ///// <summary>
        ///// 模块
        ///// </summary>
        //[EnumShowName("模块")]
        //Mode = 2
    }
    /// <summary>
    /// 权限Code特性
    /// </summary>
    public sealed class PermissionsCodeAttribute : Attribute
    {
        /// <summary>
        /// 权限代码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="code"></param>
        public PermissionsCodeAttribute(string code)
        {
            Code = code;
        }
    }
    /// <summary>
    /// 权限模型枚举
    /// </summary>
    public enum PermissionsModelModeEnum
    {
        /// <summary>
        /// 所有
        /// </summary>
        All,
        /// <summary>
        /// 只有拥有的
        /// </summary>
        Has
    }
    /// <summary>
    /// 权限模型
    /// </summary>
    public sealed class PermissionsModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Ico { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 子级
        /// </summary>
        public List<PermissionsModel> Items { get; set; }
        /// <summary>
        /// 是否拥有
        /// </summary>
        public bool IsHas { get; set; }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="item">权限对象</param>
        /// <param name="ifEnable">启用标识</param>
        /// <param name="hasID">拥有的唯一标识</param>
        /// <param name="mode">模式</param>
        public PermissionsModel(T_Permissions item, bool? ifEnable, Guid[] hasID, PermissionsModelModeEnum mode)
        {
            ID = item.ID;
            Name = item.Name;
            Ico = item.Ico;
            Code = item.Code;
            if (hasID != null && hasID.Contains(item.ID))
            {
                IsHas = true;
            }
            else
            {
                IsHas = false;
            }
            if (item.T_Permissions1.Count > 0)
            {
                Items = new List<PermissionsModel>();
                List<T_Permissions> listM;
                if (ifEnable == null)
                {
                    listM = item.T_Permissions1.OrderByDescending(m => m.Ranks).ToList();
                }
                else
                {
                    listM = item.T_Permissions1.Where(m => m.IfEnable = ifEnable.Value).OrderByDescending(m => m.Ranks).ToList();
                }
                foreach (T_Permissions perM in listM)
                {
                    if (!perM.IfDelete)
                    {
                        switch (mode)
                        {
                            case PermissionsModelModeEnum.All:
                                Items.Add(new PermissionsModel(perM, ifEnable, hasID, mode));
                                break;
                            case PermissionsModelModeEnum.Has:
                                if (hasID.Contains(perM.ID))
                                {
                                    Items.Add(new PermissionsModel(perM, ifEnable, hasID, mode));
                                }
                                break;
                        }
                    }
                }
            }
            else
            {
                Items = null;
            }
        }
        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="listM">权限集</param>
        /// <param name="ifEnable">启用标识</param>
        /// <param name="hasID">拥有的唯一标识</param>
        /// <param name="mode">模式</param>
        /// <returns>权限</returns>
        public static List<PermissionsModel> GetList(List<T_Permissions> listM, bool? ifEnable, Guid[] hasID = null, PermissionsModelModeEnum mode = PermissionsModelModeEnum.All)
        {
            List<PermissionsModel> resM = new List<PermissionsModel>();
            if (ifEnable == null)
            {
                listM = listM.OrderByDescending(m => m.Ranks).ToList();
            }
            else
            {
                listM = listM.Where(m => m.IfEnable = ifEnable.Value).OrderByDescending(m => m.Ranks).ToList();
            }
            foreach (T_Permissions item in listM)
            {
                if (!item.IfDelete)
                {
                    switch (mode)
                    {
                        case PermissionsModelModeEnum.All:
                            resM.Add(new PermissionsModel(item, ifEnable, hasID, mode));
                            break;
                        case PermissionsModelModeEnum.Has:
                            if (hasID.Contains(item.ID))
                            {
                                resM.Add(new PermissionsModel(item, ifEnable, hasID, mode));
                            }
                            break;
                    }
                }
            }
            return resM;
        }
    }
    /// <summary>
    /// 权限组模型
    /// </summary>
    public sealed class PermissionsGroupModel
    {
        /// <summary>
        /// 类型
        /// </summary>
        public PermissionsTypesEnum Type { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string TypeStr
        {
            get
            {
                return EnumManager.GetShowName(Type);
            }
        }
        /// <summary>
        /// 权限集
        /// </summary>
        public List<PermissionsModel> Items { get; set; }
    }
}
