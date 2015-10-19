using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QRBa.Tests.Helper;
using System.Net.Http;

namespace QRBa.Tests
{
    /// <summary>
    /// Summary description for ControllerTests
    /// </summary>
    [TestClass]
    public class ControllerTests
    {
        private const string baseUrl = "http://qrba.cc/";
        //private const string baseUrl = "http://localhost:40997/";
        public static readonly HttpClient HttpClient = new HttpClient
        {
            BaseAddress = new Uri(baseUrl),
        };

        [TestInitialize]
        public void Setup()
        {
            DataAccessor.Testability.Cleanup();
        }

        [TestMethod]
        public void EventTest()
        {

        }
    }
}
