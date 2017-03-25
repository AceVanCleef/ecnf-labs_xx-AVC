using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fhnw.Ecnf.RoutePlanner.RoutePlannerLib;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Reflection.Emit;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Fhnw.Ecnf.RoutePlanner.RoutePlannerTest
{
    [TestClass]
    [DeploymentItem("data/citiesTestDataLab3.txt")]
    [DeploymentItem("data/linksTestDataLab3.txt")]
    public class Lab5Test
    {
        [TestMethod]
        public void TestFindCitiesByTransportMode()
        {
            Cities cities = new Cities();
            cities.ReadCities(@"citiesTestDataLab3.txt");
            var routes = new Links(cities);
            routes.ReadLinks(@"linksTestDataLab3.txt");

            City[] citiesByMode = routes.FindCities(TransportMode.Rail);
            Assert.AreEqual(11, citiesByMode.Length);

            City[] emptyCitiesByMode = routes.FindCities(TransportMode.Bus);
            Assert.AreEqual(0, emptyCitiesByMode.Length);
        }

        [TestMethod]
        public void TestFindCitiesByTransportModeIsASingleLinqStatement()
        {
            Func<TransportMode,City[]> method = new Links(null).FindCities;
            MethodInfo methodInfo = method.Method;

            CheckForLinqUsage(methodInfo);
        }

        [TestMethod]
        public void TestFindNeighborIsASingleLinqStatement()
        {
            Func<WayPoint, double, IEnumerable<City>> method = new Cities().FindNeighbours;
            MethodInfo methodInfo = method.Method;

            CheckForLinqUsage(methodInfo);
        }

        private static void CheckForLinqUsage(MethodInfo methodInfo)
        {
            Assert.IsTrue(methodInfo.GetMethodBody().LocalVariables.Count <= 2,
                "Implement the method FindCities as a single-line LINQ statement in the form \"return [LINQ];\".");

            // some more not very sophisticated tests to ensure LINQ has been used
            MethodBody mb = methodInfo.GetMethodBody();
            // the method should be smaller than 100 IL byte instructions
            Assert.IsTrue(mb.GetILAsByteArray().Length < 100);
            // and it should contain two "call" ops (to LINQ)
            Assert.IsTrue(mb.GetILAsByteArray().ToList().Contains(40));
            //TODO: verify that a call to LINQ method is done
        }
    }
}