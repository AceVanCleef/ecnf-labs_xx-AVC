using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fhnw.Ecnf.RoutePlanner.RoutePlannerLib;
using System.Collections.Generic;
using System.Threading;
using System.Globalization;
using System.IO;

namespace Fhnw.Ecnf.RoutePlanner.RoutePlannerTest
{
    [TestClass]
    [DeploymentItem("data/citiesTestDataLab3.txt")]
    [DeploymentItem("data/linksTestDataLab3.txt")]
    public class Lab3Test
    {
        private const string CitiesTestFile = "citiesTestDataLab3.txt";
        private const string LinksTestFile = "linksTestDataLab3.txt";

        [TestMethod]
        public void TestLinkTransportMode()
        {
            var mumbai = new City("Mumbai", "India", 12383146, 18.96, 72.82);
            var buenosAires = new City("Buenos Aires", "Argentina", 12116379, -34.61, -58.37);

            var link = new Link(mumbai, buenosAires, 10);

            // default transport should be Car
            Assert.AreEqual(TransportMode.Car, link.TransportMode);

            link = new Link(mumbai, buenosAires, 10, TransportMode.Ship);
            Assert.AreEqual(TransportMode.Ship, link.TransportMode);
        }

        [TestMethod]
        public void TestTask1FindCityInCities()
        {
            var cities = new Cities();
            cities.ReadCities(CitiesTestFile);

            try
            {
                var x = cities["noCity"];
                Assert.Fail(
                    "Indexer Cities[string] should throw a KeyNotFoundException when the supplied City cannot be found.");
            }
            catch (KeyNotFoundException)
            {
            }

            Assert.AreEqual("Zürich", cities["Zürich"].Name);

            // should be case insensitive
            Assert.AreEqual("Zürich", cities["züRicH"].Name);

            // should be case insensitive, even in "weird" cultures,
            // see http://www.moserware.com/2008/02/does-your-code-pass-turkey-test.html
            var previousCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            Assert.AreEqual("Zürich", cities["züRicH"].Name);
            Assert.AreEqual("Zürich", cities["züRIcH"].Name);
            Thread.CurrentThread.CurrentCulture = previousCulture;

            // should be picky about spaces
            try
            {
                var x = cities["züRicH "];
                Assert.Fail("Indexer Cities[string] should be picky about leading/trailing spaces.");
            }
            catch (KeyNotFoundException)
            {
            }
        }

        [TestMethod]
        public void TestReadLinks()
        {
            var cities = new Cities();
            cities.ReadCities(CitiesTestFile);

            Assert.AreEqual(7, new Links(cities).ReadLinks(LinksTestFile));
        }

        [TestMethod]
        public void TestTask2FiredEvents()
        {
            var cities = new Cities();
            cities.ReadCities(CitiesTestFile);

            var links = new Links(cities);

            // test available cities
            links.RouteRequested += (sender, e) =>
            {
                Assert.AreEqual("Bern", e.FromCity.Name);
                Assert.AreEqual("Zürich", e.ToCity.Name);
            };
            links.FindShortestRouteBetween("Bern", "Zürich", TransportMode.Rail);

            // test case insensitivity
            links.FindShortestRouteBetween("BeRN", "ZüRiCh", TransportMode.Rail);

            // test not existing cities
            links = new Links(cities);
            links.RouteRequested += (sender, e) =>
            {
                Assert.Fail("Listeners should only be informed about valid requests.");
            };
            try
            {
                links.FindShortestRouteBetween("doesNotExist", "either", TransportMode.Rail);
                Assert.Fail("Should throw KeyNotFoundException when called with invalid city names.");
            }
            catch (KeyNotFoundException)
            {
            }
        }

        [TestMethod]
        public void TestTask2EventWithNoObserver()
        {
            var cities = new Cities();
            cities.ReadCities(CitiesTestFile);

            var links = new Links(cities);

            // should run without exception
            links.FindShortestRouteBetween("Bern", "Zürich", TransportMode.Rail);
        }

        [TestMethod]
        public void TestRequestWatcher()
        {
            var reqWatch = new RouteRequestWatcher();

            var cities = new Cities();
            cities.ReadCities(CitiesTestFile);

            var links = new Links(cities);

            // capture console output in memory stream to verify correct console output
            MemoryStream ms = new MemoryStream();
            using (StreamWriter sw = new StreamWriter(ms))
            {
                
                TextWriter tmp = Console.Out;
                Console.SetOut(sw);

                links.RouteRequested += reqWatch.LogRouteRequests;

                links.FindShortestRouteBetween("Bern", "Zürich", TransportMode.Rail);
                links.FindShortestRouteBetween("Bern", "Zürich", TransportMode.Rail);
                links.FindShortestRouteBetween("Basel", "Bern", TransportMode.Rail);

                // make sure content is written to output
                sw.Flush();


                Assert.AreEqual(reqWatch.GetCityRequests(cities["Zürich"]), 2);
                Assert.AreEqual(reqWatch.GetCityRequests(cities["Bern"]), 1);
                Assert.AreEqual(reqWatch.GetCityRequests(cities["Basel"]), 0);
                Assert.AreEqual(reqWatch.GetCityRequests(cities["Lausanne"]), 0);

                // now check 
                VerifyConsoleOut(ms);
            } // IMPORTANT: close stream after test, so that memorystream is kept open for the test

    }

        // a very simple test to verify console output
        // just checks one line
        private void VerifyConsoleOut(MemoryStream ms)
        {
            ms.Position = 0;
            using (StreamReader sw = new StreamReader(ms))
            {
                // just check if the following line is in the file
                string line;
                while ((line = sw.ReadLine()) != null)
                {
                    if (line.Contains("ToCity: Bern has been requested 1 times"))
                    {
                        Assert.IsTrue(true);
                        return;
                    }
                }

                Assert.Fail();

            }
        }
    }
}
