﻿// Copyright 2012-2013 Peter Henell

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
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using MSTest = Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLDataProducer.Entities.DatabaseEntities;
using SQLDataProducer.Entities.DatabaseEntities.Factories;
using SQLDataProducer.Entities.ExecutionEntities;
using Generators = SQLDataProducer.Entities.Generators;
using SQLDataProducer.Entities;


namespace SQLDataProducer.Tests.EntitiesTests
{
    [TestFixture]
    [MSTest.TestClass]
    public class ExecutionNodeTest : TestBase
    {
        public ExecutionNodeTest()
        {
        }



        [Test]
        [MSTest.TestMethod]
        public void shouldCreateExecutionNodeBySettingRepeatCount()
        {
            int numberOfRepeats = 10;
            ExecutionNode node = ExecutionNode.CreateLevelOneNode(numberOfRepeats);
            Assert.NotNull(node);
            Assert.That(node.RepeatCount, Is.EqualTo(numberOfRepeats));
        }

        [Test]
        [MSTest.TestMethod]
        public void shouldIncreaseTheLevelOfEachChild()
        {
            ExecutionNode node = ExecutionNode.CreateLevelOneNode(1);
            var node2 = node.AddChild(100);
            var node3 = node2.AddChild(24);

            var node4 = node.AddChild(1);

            Assert.That(node2.Level, Is.GreaterThan(node.Level));
            Assert.That(node2.Level, Is.EqualTo(node.Level + 1));

            Assert.That(node3.Level, Is.GreaterThan(node2.Level));
            Assert.That(node3.Level, Is.EqualTo(node2.Level + 1));

            Assert.That(node4.Level, Is.GreaterThan(node.Level));
            Assert.That(node4.Level, Is.EqualTo(node.Level + 1));
        }


        [Test]
        [MSTest.TestMethod]
        public void shouldAddChildNodesToNode()
        {
            ExecutionNode node = ExecutionNode.CreateLevelOneNode(1);
            node.AddChild(100);
            Assert.AreEqual(1, node.Children.Count, "Number of children");

            node.AddChild(1);
            node.AddChild(1);
            Assert.AreEqual(3, node.Children.Count, "Number of children");
            foreach (var item in node.Children)
            {
                Assert.That(item.Level, Is.GreaterThan(1));
                Console.WriteLine(item.Level);

                for (int i = 0; i < 3; i++)
                    item.AddChild(1);

                foreach (var child in item.Children)
                {
                    Assert.That(item.Level, Is.LessThan(child.Level));
                }
            }
        }

        [Test]
        [MSTest.TestMethod]
        public void shouldIterateOverAllNodesInTree()
        {
            ExecutionNode node = ExecutionNode.CreateLevelOneNode(1);
            node.AddChild(1).AddChild(1).AddChild(1).AddChild(1).AddChild(1).AddChild(1);

            int counter = 0;
            NodeIterator it = new NodeIterator(node);
            Assert.That(node.Children.Count, Is.GreaterThan(0));



            foreach (var item in it.GetNodesRecursive())
            {
                counter++;
                Console.WriteLine(item.NodeId);
            }

            Assert.That(counter, Is.EqualTo(7));

        }

        [Test]
        [MSTest.TestMethod]
        public void shouldProduceUniqueIdentitiesForEachNodeAdded()
        {
            ExecutionNode node = ExecutionNode.CreateLevelOneNode(1);
            var node2 = node.AddChild(1);
            var node3 = node2.AddChild(1);
            var node4 = node2.AddChild(1);
            var node5 = node.AddChild(1);

            Assert.That(node, Is.Not.EqualTo(node2));
            Assert.That(node3, Is.Not.EqualTo(node2));
            Assert.That(node4, Is.Not.EqualTo(node2));
            Assert.That(node5, Is.Not.EqualTo(node));

            Assert.That(node, Is.EqualTo(node));
            Assert.That(node2, Is.EqualTo(node2));
        }

        [Test]
        [MSTest.TestMethod]
        public void shouldGetEachExecutionNodeOnlyOnceFromTheIterator()
        {
            ExecutionNode node = ExecutionNode.CreateLevelOneNode(1);
            node.AddChild(1).AddChild(1).AddChild(1).AddChild(1).AddChild(1).AddChild(1);

            int counter = 0;
            NodeIterator it = new NodeIterator(node);
            Assert.That(node.Children.Count, Is.GreaterThan(0));

            HashSet<ExecutionNode> nodes = new HashSet<ExecutionNode>();

            foreach (var item in it.GetNodesRecursive())
            {
                nodes.Add(item);
                counter++;
                Console.WriteLine(item.NodeId);
            }

            Assert.That(counter, Is.EqualTo(7));
            Assert.That(nodes.Count, Is.EqualTo(7));
        }

        [Test]
        [MSTest.TestMethod]
        public void shouldHaveWantedOrderInTheRecursiveness()
        {
            string[] requestedOrder = { "Player", "Deposit", "GameRound", "Transactions", "Tracking", "Withdraw" };

            ExecutionNode player = ExecutionNode.CreateLevelOneNode(50, "Player");

            var deposit = player.AddChild(1, "Deposit");
            var gameRound = player.AddChild(100, "GameRound");
            var withdraw = player.AddChild(1, "Withdraw");
            var transactions = gameRound.AddChild(2, "Transactions");
            var tracking = gameRound.AddChild(1, "Tracking");

            NodeIterator it = new NodeIterator(player);

            List<ExecutionNode> actual = new List<ExecutionNode>(it.GetNodesRecursive());
            
            Assert.That(actual.Count, Is.EqualTo(6));

            for (int i = 0; i < requestedOrder.Length; i++)
            {
                Assert.That(requestedOrder[i], Is.EqualTo(actual[i].NodeName));
            }

            
        }

        [Test]
        [MSTest.TestMethod]
        public void shouldBeEqual()
        {
            ExecutionNode node1 = ExecutionNode.CreateLevelOneNode(1);
            ExecutionNode node2 = ExecutionNode.CreateLevelOneNode(1);

            ExecutionNode node3 = node1;

            Assert.That(node1, Is.EqualTo(node1));
            Assert.That(node1, Is.EqualTo(node3));
            Assert.That(node3, Is.EqualTo(node1));

            Assert.That(node2, Is.EqualTo(node2));
        }

        [Test]
        [MSTest.TestMethod]
        public void shouldNotBeEqual()
        {
            ExecutionNode node1 = ExecutionNode.CreateLevelOneNode(1);
            ExecutionNode node2 = ExecutionNode.CreateLevelOneNode(1);

            ExecutionNode node3 = node1;
            ExecutionNode node4 = null;
            ExecutionNode node5 = node2;

            Assert.That(node1, Is.Not.EqualTo(node2));
            Assert.That(node2, Is.Not.EqualTo(node1));
            Assert.That(node4, Is.Not.EqualTo(node1));

            Assert.That(node4, Is.Null);
            Assert.That(node1, Is.Not.EqualTo(node4));

            Console.WriteLine(node2);
        }

        [Test]
        [MSTest.TestMethod]
        public void shouldReturnOnlyTheNodeIfOnlyOne()
        {
            ExecutionNode node = ExecutionNode.CreateLevelOneNode(1);

            int counter = 0;
            NodeIterator it = new NodeIterator(node);
            Assert.That(node.Children.Count, Is.EqualTo(0));

            HashSet<ExecutionNode> nodes = new HashSet<ExecutionNode>();

            foreach (var item in it.GetNodesRecursive())
            {
                nodes.Add(item);
                counter++;
            }

            Assert.That(counter, Is.EqualTo(1));
            Assert.That(nodes.Count, Is.EqualTo(1));
        }

        [Test]
        [MSTest.TestMethod]
        public void shouldNotCrashOnNoNodesInIterator()
        {
            ExecutionNode node = null;

            int counter = 0;
            NodeIterator it = new NodeIterator(node);

            HashSet<ExecutionNode> nodes = new HashSet<ExecutionNode>();

            foreach (var item in it.GetNodesRecursive())
            {
                nodes.Add(item);
                counter++;
            }

            Assert.That(counter, Is.EqualTo(0));
            Assert.That(nodes.Count, Is.EqualTo(0));
        }
    }
}