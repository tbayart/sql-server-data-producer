﻿// Copyright 2012 Peter Henell

//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;

namespace SQLDataProducer.Entities.DatabaseEntities
{
    public class ColumnDataTypeDefinition : EntityBase
    {

        public ColumnDataTypeDefinition(string rawDataType, bool nullable)
        {
            Raw = rawDataType;
            DBType = StringToDBDataType(rawDataType);
            IsNullable = nullable;

            if (DBType == DbType.String)
            {
                int length = GetLengthOfStringDataType(rawDataType);
                MaxLength = length;
            }
        }

        private static int GetLengthOfStringDataType(string rawDataType)
        {
            // find the LENGTH part of the string datatype(LENGTH)
            Regex r = new Regex(@"\((?<length>[0-9]*|max)\)", RegexOptions.Compiled);
            int length;

            if (r.Matches(rawDataType).Count > 0)
            {
                // a length was found in the string between the paranteses
                var m = r.Match(rawDataType).Result("${length}");

                if (m.ToLower() == "max")
                {
                    length = int.MaxValue;
                }
                else if (!int.TryParse(m, out length))
                {
                    length = 0;
                }
            }
            else
            {
                // no length in the string, this is probably a sysname column
                length = 50;
            }
            return length;
        }

        DbType _dbType;
        public DbType DBType
        {
            get
            {
                return _dbType;
            }
            set
            {
                if (_dbType != value)
                {
                    _dbType = value;
                    OnPropertyChanged("DBType");
                }
            }
        }

        //        DBDataType _dbType;
        //public DBDataType DBType
        //{
        //    get
        //    {
        //        return _dbType;
        //    }
        //    set
        //    {
        //        if (_dbType != value)
        //        {
        //            _dbType = value;
        //            OnPropertyChanged("DBType");
        //        }
        //    }
        


        bool _isNullable;
        public bool IsNullable
        {
            get
            {
                return _isNullable;
            }
            set
            {
                if (_isNullable != value)
                {
                    _isNullable = value;
                    OnPropertyChanged("IsNullable");
                }
            }
        }


        int _maxLength;
        public int MaxLength
        {
            get
            {
                return _maxLength;
            }
            set
            {
                if (_maxLength != value)
                {
                    _maxLength = value;
                    OnPropertyChanged("MaxLength");
                }
            }
        }


        string _raw;
        public string Raw
        {
            get
            {
                return _raw;
            }
            set
            {
                if (_raw != value)
                {
                    _raw = value;
                    OnPropertyChanged("Raw");
                }
            }
        }


        public override string ToString()
        {
            return DBType.ToString();
        }

        
        private DbType StringToDBDataType(string dataType)
        {
            dataType = dataType.ToLower();

            if (dataType.StartsWith("int"))
            {
                return DbType.Int32;// DBDataType.INT;
            }
            else if (dataType.StartsWith("tinyint"))
            {
                return DbType.Byte;// DBDataType.TINYINT;
            }
            else if (dataType.StartsWith("smallint"))
            {
                return DbType.Int16;// DBDataType.SMALLINT;
            }
            else if (dataType.StartsWith("bigint"))
            {
                return DbType.Int64;// DBDataType.BIGINT;
            }
            else if (dataType.StartsWith("bit")
                || dataType.StartsWith("flag"))
            {
                return DbType.Boolean;// DBDataType.BIT;
            }
            else if (dataType.StartsWith("varchar")
                || dataType.StartsWith("nvarchar")
                || dataType.StartsWith("char")
                || dataType.StartsWith("nchar")
                || dataType.StartsWith("sysname"))
            {

                return DbType.String;// DBDataType.VARCHAR;
            }
            else if (dataType.StartsWith("decimal")
                || dataType.StartsWith("float")
                || dataType.StartsWith("money")
                || dataType.StartsWith("smallmoney"))
            {
                return DbType.Decimal;// DBDataType.DECIMAL;
            }
            else if (dataType.StartsWith("datetime")
                || dataType.StartsWith("date")
                || dataType.StartsWith("time")
                || dataType.StartsWith("smalldatetime"))
            {
                return DbType.DateTime;// DBDataType.DATETIME;
            }
            else if (dataType.StartsWith("uniqueidentifier"))
            {
                return DbType.Guid;// DBDataType.UNIQUEIDENTIFIER;
            }
            else if (dataType.StartsWith("xml"))
                return DbType.Xml;

            else
            {
                return DbType.Object;// DBDataType.UNKNOWN;
            }
        }
    

        //int _precision;
        //public int Precision
        //{
        //    get
        //    {
        //        return _precision;
        //    }
        //    set
        //    {
        //        if (_precision != value)
        //        {
        //            _precision = value;
        //            OnPropertyChanged("Precision");
        //        }
        //    }
        //}
    }

   
}
