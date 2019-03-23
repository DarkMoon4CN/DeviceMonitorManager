using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Linq.Expressions;

namespace ChuanYe.Utils
{
    class Program
    {
        static void Main(string[] args)
        {
            string numStr = "20";

            //DataExtension 使用方式
            int num = numStr.ToInt();

            //ParameterReplacer 使用方式
            List<int> temp = new List<int>();
            var where = PredicateExtensionses.True<int>();
            for (int i = 0; i < 30; i++)
            {
                temp.Add(i);
            }
            where = where.And(p=>temp.Contains(p));
            


            //log   web.config 或者 app.config 没有配置
            LoggerHelper.Info("test num :" + temp.SerializeJSON());


            

            Console.ReadKey();

        }

    }

}
