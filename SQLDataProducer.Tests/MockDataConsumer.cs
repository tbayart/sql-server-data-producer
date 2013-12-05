﻿using SQLDataProducer.Entities.DataConsumers;
using SQLDataProducer.Entities.DataEntities;
using SQLDataProducer.Entities.ExecutionEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLDataProducer.Tests
{
    class MockDataConsumer : IDataConsumer
    {

        private int rowCounter = 0;
        private bool initialized = false;
        private int identityCounter = 0;

        public Action<object> ValueValidator { get; set; }
        public Action<DataRowEntity> RowValidator { get; set; }

        public static Action<object> NonNullValueValidator { get { return new Action<object>(value => { if (value == null) throw new ArgumentNullException(); }); } }
        public static Action<DataRowEntity> NonNullRowValidator { get { return new Action<DataRowEntity>(value => { if (value == null) throw new ArgumentNullException(); }); } }
        
        public MockDataConsumer()
        {
            ValueValidator = new Action<object>(value => { return; });
            RowValidator = new Action<DataRowEntity>(value => { return; });
        }

        public bool Init(string connectionString, Dictionary<string, string> options = null)
        {
            rowCounter = 0;
            return initialized = true;
        }

        public ExecutionResult Consume(IEnumerable<DataRowEntity> rows, ValueStore valueStore)
        {
            ValidateInitialized();
            foreach (var row in rows)
            {
                rowCounter++;
                RowValidator(row);
                foreach (var field in row.Fields)
                {
                    if (field.ProducesValue)
                    {
                        valueStore.Put(field.KeyValue, identityCounter++);
                    }
                    var value = valueStore.GetByKey(field.KeyValue);
                    ValueValidator(value);
                    Console.WriteLine(value);
                }
            }

            return null;
        }

        private void ValidateInitialized()
        {
            if (!initialized)
            {
                throw new NotSupportedException("consumer is not initialized. Make sure to run Init(...).");
            }
        }

        public void Dispose()
        {

        }

        public int TotalRows
        {
            get
            {
                return rowCounter;
            }
        }

       
    }
}
