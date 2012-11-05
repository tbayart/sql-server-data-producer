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
using NUnit.Framework;
using SQLDataProducer.Entities.DatabaseEntities;
using SQLDataProducer.Entities.ExecutionEntities;
using SQLDataProducer.DataAccess;
using SQLDataProducer.ContinuousInsertion.Builders;
using SQLDataProducer.Entities.DatabaseEntities.Collections;
using SQLDataProducer.TaskExecuter;
using SQLDataProducer.Entities;
using SQLDataProducer.Entities.OptionEntities;

namespace SQLDataProducer.RandomTestsnStuff
{
    public class RandomTests
    {
        [Test]
        public void ShouldGenerateValuesAndInsertStatementsForAllTables()
        {
            TableEntityDataAccess tda = new TableEntityDataAccess(Connection());

            TableEntityCollection tables = tda.GetAllTablesAndColumns();

            foreach (TableEntity table in tables)
            {
                ExecutionItem ie = new ExecutionItem(table);
                ie.RepeatCount = 3;
                TableEntityInsertStatementBuilder builder = new TableEntityInsertStatementBuilder(ie);
                int i = 1;
                builder.GenerateValues(() => i++);

                Console.WriteLine();
                Console.WriteLine("We dont really care about the errors, we just want to generate data for all the tables and columns.");
                Console.WriteLine(builder.GenerateFullStatement());
                Console.WriteLine("GO");
            }
        }

        [Test]
        public void ShouldExecuteWithNewNForEachExecution()
        {
            WorkflowManager wfm;
            ExecutionTaskOptions options;
            ExecutionItemCollection items;
            Setup(out wfm, out options, out items);

            {
                // new N for each Execution
                options.NumberGeneratorMethod = NumberGeneratorMethods.NewNForEachExecution;
                var res = wfm.RunWorkFlow(options, Connection(), items);

                Console.WriteLine(res.ToString());
                Assert.AreEqual(13, res.InsertCount, "InsertCount should be 13");
                Assert.AreEqual(0, res.ErrorList.Count, "InsertCount should be 0");
                Assert.AreEqual(1, res.ExecutedItemCount, "ExecutedItemCount should be 1");
                Assert.Greater(res.Duration, TimeSpan.Zero, "Duration should > 0");
            }
        }

        [Test]
        public void ShouldExecuteWithNewNForEachRow()
        {
            WorkflowManager wfm;
            ExecutionTaskOptions options;
            ExecutionItemCollection items;
            Setup(out wfm, out options, out items);

            {
                // new N for each row
                options.NumberGeneratorMethod = NumberGeneratorMethods.NewNForEachRow;
                var res = wfm.RunWorkFlow(options, Connection(), items);

                Console.WriteLine(res.ToString());
                Assert.AreEqual(13, res.InsertCount, "InsertCount should be 13");
                Assert.AreEqual(0, res.ErrorList.Count, "InsertCount should be 0");
                Assert.AreEqual(1, res.ExecutedItemCount, "ExecutedItemCount should be 1");
                Assert.Greater(res.Duration, TimeSpan.Zero, "Duration should > 0");
            }
        }

        [Test]
        public void ShouldExecuteWithConstantN()
        {
            WorkflowManager wfm;
            ExecutionTaskOptions options;
            ExecutionItemCollection items;
            Setup(out wfm, out options, out items);
           
            {
                // ConstantN
                options.NumberGeneratorMethod = NumberGeneratorMethods.ConstantN;
                var res = wfm.RunWorkFlow(options, Connection(), items);

                Console.WriteLine(res.ToString());
                Assert.AreEqual(13, res.InsertCount, "InsertCount should be 13");
                Assert.AreEqual(0, res.ErrorList.Count, "InsertCount should be 0");
                Assert.AreEqual(1, res.ExecutedItemCount, "ExecutedItemCount should be 1");
                Assert.Greater(res.Duration, TimeSpan.Zero, "Duration should > 0");
            }
        }

        [Test]
        public void ShouldExecuteOnlyOnCondition_EQUALTO()
        {
            var wfm = new WorkflowManager();
            var tda = new TableEntityDataAccess(Connection());
            var adressTable = tda.GetTableAndColumns("Person", "NewPerson");
            var i1 = new ExecutionItem(adressTable);
            i1.ExecutionCondition = ExecutionConditions.EqualTo;
            i1.ExecutionConditionValue = 2;
            i1.RepeatCount = 10;

            var options = new ExecutionTaskOptions();
            options.ExecutionType = ExecutionTypes.ExecutionCountBased;
            options.FixedExecutions = 3;
            options.NumberGeneratorMethod = NumberGeneratorMethods.NewNForEachRow;
            ExecutionItemCollection items = new ExecutionItemCollection();
            items.Add(i1);

            {
                // new N for each row
                var res = wfm.RunWorkFlow(options, Connection(), items);

                Console.WriteLine(res.ToString());
                Assert.AreEqual(10, res.InsertCount, "InsertCount should be 10");
                Assert.AreEqual(0, res.ErrorList.Count, "InsertCount should be 0");
                Assert.AreEqual(3, res.ExecutedItemCount, "ExecutedItemCount should be 1");
                Assert.Greater(res.Duration, TimeSpan.Zero, "Duration should > 0");
            }
        }

        [Test]
        public void ShouldExecuteOnlyOnCondition_GREATERTHAN()
        {
            var wfm = new WorkflowManager();
            var tda = new TableEntityDataAccess(Connection());
            var adressTable = tda.GetTableAndColumns("Person", "NewPerson");
            var i1 = new ExecutionItem(adressTable);
            i1.ExecutionCondition = ExecutionConditions.GreaterThan;
            i1.ExecutionConditionValue = 2;
            i1.RepeatCount = 10;

            var options = new ExecutionTaskOptions();
            options.ExecutionType = ExecutionTypes.ExecutionCountBased;
            options.FixedExecutions = 3;
            options.NumberGeneratorMethod = NumberGeneratorMethods.NewNForEachRow;
            ExecutionItemCollection items = new ExecutionItemCollection();
            items.Add(i1);

            {
                // new N for each row
                var res = wfm.RunWorkFlow(options, Connection(), items);

                Console.WriteLine(res.ToString());
                Assert.AreEqual(10, res.InsertCount, "InsertCount should be 10");
                Assert.AreEqual(0, res.ErrorList.Count, "InsertCount should be 0");
                Assert.AreEqual(3, res.ExecutedItemCount, "ExecutedItemCount should be 1");
                Assert.Greater(res.Duration, TimeSpan.Zero, "Duration should > 0");
            }
        }

        [Test]
        public void ShouldExecuteOnlyOnCondition_EqualOrGreaterThan()
        {
            var wfm = new WorkflowManager();
            var tda = new TableEntityDataAccess(Connection());
            var adressTable = tda.GetTableAndColumns("Person", "NewPerson");
            var i1 = new ExecutionItem(adressTable);
            i1.ExecutionCondition = ExecutionConditions.EqualOrGreaterThan;
            i1.ExecutionConditionValue = 2;
            i1.RepeatCount = 10;

            var options = new ExecutionTaskOptions();
            options.ExecutionType = ExecutionTypes.ExecutionCountBased;
            options.FixedExecutions = 3;
            options.NumberGeneratorMethod = NumberGeneratorMethods.NewNForEachRow;
            ExecutionItemCollection items = new ExecutionItemCollection();
            items.Add(i1);

            {
                // new N for each row
                var res = wfm.RunWorkFlow(options, Connection(), items);

                Console.WriteLine(res.ToString());
                Assert.AreEqual(20, res.InsertCount, "InsertCount should be 10");
                Assert.AreEqual(0, res.ErrorList.Count, "InsertCount should be 0");
                Assert.AreEqual(3, res.ExecutedItemCount, "ExecutedItemCount should be 1");
                Assert.Greater(res.Duration, TimeSpan.Zero, "Duration should > 0");
            }
        }

        [Test]
        public void ShouldExecuteOnlyOnCondition_LessThan()
        {
            var wfm = new WorkflowManager();
            var tda = new TableEntityDataAccess(Connection());
            var adressTable = tda.GetTableAndColumns("Person", "NewPerson");
            var i1 = new ExecutionItem(adressTable);
            i1.ExecutionCondition = ExecutionConditions.LessThan;
            i1.ExecutionConditionValue = 2;
            i1.RepeatCount = 10;

            var options = new ExecutionTaskOptions();
            options.ExecutionType = ExecutionTypes.ExecutionCountBased;
            options.FixedExecutions = 3;
            options.NumberGeneratorMethod = NumberGeneratorMethods.NewNForEachRow;
            ExecutionItemCollection items = new ExecutionItemCollection();
            items.Add(i1);

            {
                // new N for each row
                var res = wfm.RunWorkFlow(options, Connection(), items);

                Console.WriteLine(res.ToString());
                Assert.AreEqual(10, res.InsertCount, "InsertCount should be 10");
                Assert.AreEqual(0, res.ErrorList.Count, "InsertCount should be 0");
                Assert.AreEqual(3, res.ExecutedItemCount, "ExecutedItemCount should be 1");
                Assert.Greater(res.Duration, TimeSpan.Zero, "Duration should > 0");
            }
        }

        [Test]
        public void ShouldExecuteOnlyOnCondition_EveryOtherX()
        {
            var wfm = new WorkflowManager();
            var tda = new TableEntityDataAccess(Connection());
            var adressTable = tda.GetTableAndColumns("Person", "NewPerson");
            var i1 = new ExecutionItem(adressTable);
            i1.ExecutionCondition = ExecutionConditions.EveryOtherX;
            i1.ExecutionConditionValue = 2;
            i1.RepeatCount = 10;

            var options = new ExecutionTaskOptions();
            options.ExecutionType = ExecutionTypes.ExecutionCountBased;
            options.FixedExecutions = 3;
            options.NumberGeneratorMethod = NumberGeneratorMethods.NewNForEachRow;
            ExecutionItemCollection items = new ExecutionItemCollection();
            items.Add(i1);

            {
                // new N for each row
                var res = wfm.RunWorkFlow(options, Connection(), items);

                Console.WriteLine(res.ToString());
                Assert.AreEqual(20, res.InsertCount, "InsertCount should be 10");
                Assert.AreEqual(0, res.ErrorList.Count, "InsertCount should be 0");
                Assert.AreEqual(3, res.ExecutedItemCount, "ExecutedItemCount should be 1");
                Assert.Greater(res.Duration, TimeSpan.Zero, "Duration should > 0");
            }
        }

        


        private static void Setup(out WorkflowManager wfm, out ExecutionTaskOptions options, out ExecutionItemCollection items)
        {
            TableEntityDataAccess tda = new TableEntityDataAccess(Connection());
            TableEntity adressTable = tda.GetTableAndColumns("Person", "NewPerson");
            TableEntity departmentTable = tda.GetTableAndColumns("Person", "AnotherTable");


            wfm = new WorkflowManager();
            options = new ExecutionTaskOptions();
            options.ExecutionType = ExecutionTypes.ExecutionCountBased;
            options.FixedExecutions = 1;

            items = new ExecutionItemCollection();

            var ei = new ExecutionItem(adressTable);
            ei.RepeatCount = 3;
            items.Add(ei);

            var eiDepartment = new ExecutionItem(departmentTable);
            eiDepartment.RepeatCount = 10;
            items.Add(eiDepartment);
        }

        [Test]
        public void SetCounterShouldIncrementAndAdd()
        {
            // TODO: Test thread safety of set counter methods
            SetCounter sc = new SetCounter();

            Assert.AreEqual(0, sc.Peek());

            sc.Increment();
            Assert.AreEqual(1, sc.Peek());

            sc.Add(10);
            Assert.AreEqual(11, sc.Peek());

        }

        private static string Connection()
        {
            return "Data Source=localhost;Initial Catalog=AdventureWorks;Integrated Security=True";
        }

        public RandomTests()
        {
            string sql = @"

if exists (select 1 from INFORMATION_SCHEMA.tables where TABLE_NAME = 'AnotherTable' and TABLE_SCHEMA = 'Person')
	drop table Person.AnotherTable;

if exists (select 1 from INFORMATION_SCHEMA.tables where TABLE_NAME = 'NewPerson' and TABLE_SCHEMA = 'Person')
	drop table Person.NewPerson;
	
	
create table Person.NewPerson(
	NewPersonId int identity(1, 1) primary key,
	Name varchar(500) not null,
	BitColumn bit, 
	DecimalColumn decimal(10, 4),
	BigintColumn bigint, 
	VarcharMaxColumn varchar(max) null,
	FloatColumn float,
	DateTime2Column datetime2,
	DateTimeColumn datetime,
	NCharFiveColumn nchar(5),
	DateColumn date, 
	TimeColumn time,
	SmallIntColumn smallint,
	SmallDateTimeColumn smalldatetime,
	SmallMoneyColumn smallmoney 
);

create table Person.AnotherTable(
	NewPersonId int foreign key references Person.NewPerson(NewPersonId),
	AnotherColumn char(1),
    ThirdColumn char(1) not null
);";
            AdhocDataAccess adhd = new AdhocDataAccess(Connection());
            adhd.ExecuteNonQuery(sql);

        }
    }
}