﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLDataProducer.Entities.Generators;
using System.Collections.ObjectModel;
using SQLDataProducer.Entities;
using SQLDataProducer.Entities.Generators.Collections;
using System.Xml.Serialization;


namespace SQLDataProducer.DatabaseEntities.Entities
{
    public partial class ColumnEntity : SQLDataProducer.Entities.EntityBase, IXmlSerializable
    {
        string _columnDataType;
        [System.ComponentModel.ReadOnly(true)]
        public string ColumnDataType
        {
            get
            {
                return _columnDataType;
            }
            private set
            {
                if (_columnDataType != value)
                {
                    _columnDataType = value;
                    OnPropertyChanged("ColumnDataType");
                }
            }
        }

        bool _isIdentity;
        [System.ComponentModel.ReadOnly(true)]
        public bool IsIdentity
        {
            get
            {
                return _isIdentity;
            }
            private set
            {
                if (_isIdentity != value)
                {
                    _isIdentity = value;
                    OnPropertyChanged("IsIdentity");
                    OnPropertyChanged("IsNotIdentity");
                }
            }
        }

        [System.ComponentModel.ReadOnly(true)]
        public bool IsNotIdentity
        {
            get
            {
                return !_isIdentity;
            }
        }

        string _columnName;
        [System.ComponentModel.ReadOnly(true)]
        public string ColumnName
        {
            get
            {
                return _columnName;
            }
            private set
            {
                if (_columnName != value)
                {
                    _columnName = value;
                    OnPropertyChanged("ColumnName");
                }
            }
        }


        int _ordinalPosition;
        [System.ComponentModel.ReadOnly(true)]
        public int OrdinalPosition
        {
            get
            {
                return _ordinalPosition;
            }
            private set
            {
                if (_ordinalPosition != value)
                {
                    _ordinalPosition = value;
                    OnPropertyChanged("OrdinalPosition");
                }
            }
        }

        GeneratorBase _generator;
        public GeneratorBase Generator
        {
            get
            {
                return _generator;
            }
            set
            {
                if (_generator != value)
                {
                    _generator = value;
                    OnPropertyChanged("Generator");
                }
            }
        }

        ObservableCollection<GeneratorBase> _valueGenerators;
        public ObservableCollection<GeneratorBase> PossibleGenerators
        {
            get
            {
                return _valueGenerators;
            }
            private set
            {
                if (_valueGenerators != value)
                {
                    _valueGenerators = value;
                    OnPropertyChanged("PossibleGenerators");
                }
            }
        }

        private bool _isForeignKey;
        [System.ComponentModel.ReadOnly(true)]
        public bool IsForeignKey
        {
            get
            {
                return _isForeignKey;
            }
            private set
            {
                if (_isForeignKey != value)
                {
                    _isForeignKey = value;
                    OnPropertyChanged("IsForeignKey");
                }
            }
        }

        private ForeignKeyEntity _foreignKey;
        [System.ComponentModel.ReadOnly(true)]
        public ForeignKeyEntity ForeignKey
        {
            get
            {
                return _foreignKey;
            }
            private set
            {
                if (_foreignKey != value)
                {
                    _foreignKey = value;
                    OnPropertyChanged("ForeignKey");
                }
            }
        }
        
       

        /// <summary>
        /// Constructor of the ColumnEntity
        /// </summary>
        /// <param name="columnName">name of the column</param>
        /// <param name="columnDatatype">string name of the SQL datatype of the column</param>
        /// <param name="isIdentity">true if the column is identity, otherwise false</param>
        /// <param name="ordinalPosition">the ordinal position of the column</param>
        /// <param name="isForeignKey">true if this table is referencing another table using foreign key</param>
        /// <param name="generator">the default generator for this column</param>
        /// <param name="possibleGenerators">the possible generators for this column</param>
        public ColumnEntity(string columnName, string columnDatatype, bool isIdentity, int ordinalPosition, bool isForeignKey, ForeignKeyEntity foreignKeyEntity, ObservableCollection<GeneratorBase> possibleGenerators, GeneratorBase generator)
        {
            this.ColumnName = columnName;
            this.ColumnDataType = columnDatatype;
            this.OrdinalPosition = ordinalPosition;
            this.IsIdentity = isIdentity;

            this.IsForeignKey = isForeignKey;
            this.ForeignKey = foreignKeyEntity;

            this.Generator = generator ?? possibleGenerators.First();
            this.PossibleGenerators = possibleGenerators;

        }
        public ColumnEntity(string columnName, string columnDatatype, bool isIdentity, int ordinalPosition, bool isForeignKey, ForeignKeyEntity foreignKeyEntity, ObservableCollection<GeneratorBase> generators, string generatorName)
        {
            this.ColumnName = columnName;
            this.ColumnDataType = columnDatatype;
            this.OrdinalPosition = ordinalPosition;
            this.IsIdentity = isIdentity;

            this.IsForeignKey = isForeignKey;
            this.ForeignKey = foreignKeyEntity;

            this.Generator = generators.Where(g => g.GeneratorName == generatorName).First();
            this.PossibleGenerators = generators;
        }
        public ColumnEntity()
        {

        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            reader.MoveToContent();
            this.ColumnName = reader.GetAttribute("ColumnName");
            
            GeneratorBase g = new GeneratorBase();
            g.ReadXml(reader);
            this.Generator = g;
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("Column");
            writer.WriteAttributeString("ColumnName", this.ColumnName);
           
            this.Generator.WriteXml(writer);

            writer.WriteEndElement();
        }
    }
}
