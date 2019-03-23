using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{

    /// <summary>
    /// 返回数据体
    /// </summary>
    public class ResultDtoBase
    { 

        public ResultDtoBase() { }
        /// <summary>
        /// 0.业务错误，1.成功，500.系统服务器错误 ， 其他失败，自定义
        /// </summary>
        public int Status { get; set; }

       /// <summary>
       /// 错误消息
       /// </summary>
        public string Message { get; set; }
    }


    /// <summary>
    /// 返回附带单个参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultDto<T> : ResultDtoBase
    {
        /// <summary>
        /// 返回数据体 1
        /// </summary>
        public T FirstParam { get; set; }
    }

    /// <summary>
    /// 返回附带两个参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class ResultDto<T, T2> : ResultDtoBase
    {

        /// <summary>
        /// 返回数据体 1
        /// </summary>
        public T FirstParam { get; set; }

        /// <summary>
        /// 返回数据体 2
        /// </summary>
        public T2 SecondParam { get; set; }
    }

    /// <summary>
    /// 返回附带3个参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    public class ResultDto<T, T2, T3> : ResultDtoBase
    {
        /// <summary>
        /// 返回数据体 1
        /// </summary>
        public T FirstParam { get; set; }

        /// <summary>
        /// 返回数据体 2
        /// </summary>
        public T2 SecondParam { get; set; }

        /// <summary>
        /// 返回数据体 3
        /// </summary>

        public T3 ThirdParam { get; set; }
    }


    /// <summary>
    /// 返回附带4个参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    public class ResultDto<T, T2, T3,T4> : ResultDtoBase
    {
        /// <summary>
        /// 返回数据体 1
        /// </summary>
        public T FirstParam { get; set; }

        /// <summary>
        /// 返回数据体 2
        /// </summary>
        public T2 SecondParam { get; set; }

        /// <summary>
        /// 返回数据体 3
        /// </summary>

        public T3 ThirdParam { get; set; }

        /// <summary>
        /// 返回数据体 4
        /// </summary>
        public T4 FourthParam { get; set; }
    }

}