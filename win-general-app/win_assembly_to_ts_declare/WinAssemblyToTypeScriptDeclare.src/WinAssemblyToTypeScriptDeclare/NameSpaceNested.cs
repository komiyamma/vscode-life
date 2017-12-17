/*
 * Copyright (c) 2017 Akitsugu Komiyama
 * under the MIT License
 */

using System;

namespace WinAssemblyToTypeScriptDeclare
{
    // 指定の名前空間+クラス名で、整形用
    class NameSpaceNested : IComparable
    {
        public int NestLevel;
        public NameSpaceNested ParentNameSpace;
        public string NameSpace;
        public string FullNameSpace
        {
            get
            {
                return ParentNameSpace.FullNameSpace + "." + NameSpace;
            }
        }

        // 並べ替え用途
        public int CompareTo(object obj)
        {
            NameSpaceNested other = obj as NameSpaceNested;
            if (this.NestLevel < other.NestLevel)
            {
                return -1;
            }
            if (this.NestLevel == other.NestLevel)
            {
                return this.FullNameSpace.CompareTo(other.FullNameSpace);
            }
            if (this.NestLevel > other.NestLevel)
            {
                return +1;
            }
            return 0;
        }

        //objと自分自身が等価のときはtrueを返す
        public override bool Equals(object obj)
        {
            //objがnullか、型が違うときは、等価でない
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            //FullNameSpaceで比較する
            NameSpaceNested c = (NameSpaceNested)obj;
            return (this.FullNameSpace == c.FullNameSpace);
        }

        //Equalsがtrueを返すときに同じ値を返す
        public override int GetHashCode()
        {
            return this.FullNameSpace.GetHashCode();
        }

    }

}
