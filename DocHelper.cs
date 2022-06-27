using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DataClean.Common
{
    public class DocHelper
    {
        private static int upsetCutLength = 5;
        /// <summary>
        /// 打乱文档
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static string UpsetDoc(string doc)
        {
            //int pageSize = Convert.ToInt32(_configuration.GetSection("upsetCutLength").Value);

            int pageSize = upsetCutLength;

            //1. 每5个字符分割，拿到字符串数组
            var array = Regex.Matches(doc, @"(\r|\n|\s|\t|.){1," + pageSize + "}").Cast<Match>().Select(m => m.Groups[0].Value.Length < pageSize ? m.Groups[0].Value.PadRight(pageSize, '∞') : m.Groups[0].Value).ToList();
            //var i = 0;
            //var array = new List<string>();
            //while (doc.Length > i)
            //{
            //    var str = doc.Substring(i, Math.Min(pageSize, doc.Length - i));
            //    array.Add(str);
            //    i = i + pageSize;
            //}


            //数组长度是否为偶数
            bool isEven = array.Count % 2 == 0;
            //打乱数组中元素顺序
            int half, len1, len2;
            if (isEven)
            {
                half = array.Count / 2;
                len1 = len2 = half;
            }
            else
            {
                half = array.Count / 2 + 1;
                len1 = half;
                len2 = half - 1;
            }

            //2. 对位交换
            for (int i = 0; i < half; i++)
            {
                string t;
                int pointI = half + i;
                //奇数的和对位的交换，偶数的不换
                if (pointI < array.Count && i % 2 == 0)
                {
                    t = array[i];
                    array[i] = array[pointI];
                    array[pointI] = t;
                }
            }

            //3. 先将列表对半切开，并调换顺序：假设数组有31项：0-30，变成：16-30，0-15
            List<string> temp = new List<string>();
            temp.AddRange(array.GetRange(half, len2));
            temp.AddRange(array.GetRange(0, len1));

            //6个一组，反转数组
            int idx = 0;
            List<string> result = new List<string>();
            while (idx < temp.Count)
            {
                var list = temp.GetRange(idx, Math.Min(6, temp.Count - idx));
                list.Reverse();
                result.AddRange(list);
                idx = idx + 6;
            }

            string joinStr = string.Join("", result);
            return joinStr;
        }

        /// <summary>
        /// 还原打乱的文档
        /// </summary>
        /// <param name="upsetedDoc"></param>
        /// <returns></returns>
        public static string ReductDoc(string upsetedDoc)
        {
            int pageSize = upsetCutLength;
            //固定长度
            //int pageSize = Convert.ToInt32(_configuration.GetSection("upsetCutLength").Value);
            //1. 将打乱的文档按照固定长度分割为字符串数组
            var array = Regex.Matches(upsetedDoc, @"(\r|\n|\s|\t|.){" + pageSize + "}").Cast<Match>().Select(m => m.Groups[0].Value).ToList();

            int idx = 0;
            List<string> result = new List<string>();
            while (idx < array.Count)
            {
                //2.1. 每6项分为一组，最后一组可能小于6
                var list = array.GetRange(idx, Math.Min(6, array.Count - idx));
                //2.2. 将组内6个元素的排列顺序反转
                list.Reverse();
                result.AddRange(list);
                idx = idx + 6;
            }

            int half, len1, len2;
            if (result.Count % 2 == 0)
            {
                half = result.Count / 2;
                len1 = len2 = half;
            }
            else
            {
                half = result.Count / 2 + 1;
                len1 = half - 1;
                len2 = half;
            }
            List<string> temp = new List<string>();
            //3. 先将列表对半切开，并调换顺序：假设数组有31项：1-31，变成：16-31，1-15
            temp.AddRange(result.GetRange((result.Count % 2 == 0 ? half : half - 1), len2));
            temp.AddRange(result.GetRange(0, len1));

            //4. 对位交换
            for (int i = 0; i < half; i++)
            {
                string t;
                int pointI = half + i;
                //奇数的和对位的交换，偶数的不换
                if (pointI < temp.Count && i % 2 == 0)
                {
                    t = temp[i];
                    temp[i] = temp[pointI];
                    temp[pointI] = t;
                }
            }

            //5. 拼接为字符串
            string joinStr = string.Join("", temp);
            //6. 删除补位符号
            joinStr = joinStr.Replace("∞", "");
            return joinStr;
        }
    }
}
