using MateralTools.MEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KotenBu.Model
{
    /// <summary>
    /// 日志类型枚举
    /// </summary>
    public enum ApplicationLogTypeEnum : byte
    {
        [EnumShowName("操作日志")]
        Options = 0,
        [EnumShowName("调试日志")]
        Debug = 1,
        [EnumShowName("异常日志")]
        Exception = byte.MaxValue,
    }
}
