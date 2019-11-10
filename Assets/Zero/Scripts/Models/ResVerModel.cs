using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 文件版本号数据模型(默认只读）
    /// </summary>
    public class ResVerModel
    {
        protected ResVerVO _vo;

        public ResVerVO VO
        {
            get
            {
                return _vo;
            }
        }

        public ResVerModel()
        {

        }

        public ResVerModel(ResVerVO vo)
        {
            _vo = vo;
            if(_vo.items == null)
            {
                _vo.items = new ResVerVO.Item[0];
            }
        }

        /// <summary>
        /// 是否有对应资源的版本信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains(string name)
        {
            foreach (var item in _vo.items)
            {
                if (item.name == name)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 得到资源文件项
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ResVerVO.Item Get(string name)
        {
            foreach (var item in _vo.items)
            {
                if (item.name == name)
                {
                    return item;
                }
            }
            return default(ResVerVO.Item);
        }

        /// <summary>
        /// 得到文件的依赖文件列表
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        //public string[] GetDepends(string name)
        //{
        //    foreach (var item in _vo.items)
        //    {
        //        if (item.name == name)
        //        {
        //            return item.depends;
        //        }
        //    }
        //    return new string[0];
        //}

        /// <summary>
        /// 得到文件的版本号
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetVer(string name)
        {
            foreach (var item in _vo.items)
            {
                if (item.name == name)
                {
                    return item.version;
                }
            }

            return null;
        }

        /// <summary>
        /// 设置文件版本号
        /// </summary>
        /// <returns>The ver.</returns>
        /// <param name="name">Name.</param>
        public void SetVer(string name, string version)
        {
            //如果存在则更新
            for (int i = 0; i < _vo.items.Length; i++)
            {
                if (_vo.items[i].name == name)
                {
                    _vo.items[i].version = version;
                    return; //添加并返回函数
                }
            }

            //如果不存在则添加
            ResVerVO.Item[] items = new ResVerVO.Item[_vo.items.Length + 1];
            Array.Copy(_vo.items, items, _vo.items.Length);
            ResVerVO.Item newItem = new ResVerVO.Item();
            newItem.name = name;
            newItem.version = version;
            items[_vo.items.Length] = newItem;
            _vo.items = items;
        }

        /// <summary>
        /// 移除一个文件的版本号
        /// </summary>
        /// <param name="name"></param>
        public void RemoveVer(string name)
        {
            for (int i = 0; i < _vo.items.Length; i++)
            {
                if (_vo.items[i].name == name)
                {
                    ResVerVO.Item[] items = new ResVerVO.Item[_vo.items.Length - 1];
                    if (i == 0)
                    {
                        Array.Copy(_vo.items, 1, items, 0, items.Length);
                    }
                    else if(i == items.Length)
                    {
                        Array.Copy(_vo.items, 0, items, 0, items.Length);
                    }
                    else
                    {
                        //拷贝前部分
                        Array.Copy(_vo.items, 0, items, 0, i);
                        //拷贝后部分
                        Array.Copy(_vo.items, i + 1, items, i, items.Length - i);
                    }
                    _vo.items = items;
                    break;
                }
            }
        }

        /// <summary>
        /// 清除所有的版本信息
        /// </summary>
        public void ClearVer()
        {
            _vo.items = new ResVerVO.Item[0];
        }


        /// <summary>
        /// 查找资源
        /// <para>查找以name字符串为开头的资源，格式可以为 "res/h." 或 "res/ab/" </para>
        /// <para>如果没有以"."或"/"结尾，则会自动查超所有符合[name加上"."或"/"结尾]的文件</para>
        /// <para>输入 "" 或者 "/" 则会返回所有的资源</para>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<ResVerVO.Item> FindGroup(string name)
        {
            if(null == name)
            {
                return null;
            }

            if (name == "" || name == "/")
            {                               
                var totalList = new List<ResVerVO.Item>(_vo.items);
                return totalList;
            }

            bool isFuzzy = true;
            string fuzzyName0 = null;
            string fuzzyName1 = null;
            if(name.EndsWith("/") || name.Contains("."))
            {
                isFuzzy = false;
            }

            if(isFuzzy)
            {
                fuzzyName0 = name + "/";
                fuzzyName1 = name + ".";
            }

            List<ResVerVO.Item> list = new List<ResVerVO.Item>();
            for (int i = 0; i < _vo.items.Length; i++)
            {
                if (isFuzzy)
                {
                    if (_vo.items[i].name.StartsWith(fuzzyName0) || _vo.items[i].name.StartsWith(fuzzyName1) || _vo.items[i].name == name)
                    {
                        list.Add(_vo.items[i]);
                    }
                }
                else
                {
                    if (_vo.items[i].name.StartsWith(name) || _vo.items[i].name == name)
                    {
                        list.Add(_vo.items[i]);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 和目标比较资源是否版本相同
        /// </summary>
        /// <param name="name"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool IsSameVer(string name, ResVerModel target)
        {
            if (GetVer(name) == target.GetVer(name))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 找到目标资源中和我方资源不同版本的内容
        /// <para>已方有而对方没有的资源被忽略</para>
        /// <para>对方有而已方没有的资源被包括</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public List<ResVerVO.Item> FindGroupDifferent(string name, ResVerModel target)
        {            
            var selfGroup = FindGroup(name);
            var targetGroup = target.FindGroup(name);

            var targetIdx = targetGroup.Count;
            while(--targetIdx > -1)
            {
                var targetItem = targetGroup[targetIdx];

                var selfIdx = selfGroup.Count;
                while(--selfIdx > -1)
                {
                    var selfItem = selfGroup[selfIdx];

                    if(selfItem.Equals(targetItem))
                    {
                        //两边都有的对象，从两边的数组移除，减少之后的运算开销
                        targetGroup.RemoveAt(targetIdx);
                        selfGroup.RemoveAt(selfIdx);
                        break;
                    }
                }
            }
            return targetGroup;
        }

        /// <summary>
        /// 是否含有资源文件
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(ResVerVO.Item item)
        {
            foreach (var selfItem in _vo.items)
            {
                if (selfItem.Equals(item))
                {
                    return true;
                }
            }
            return false;
        }
    }
}