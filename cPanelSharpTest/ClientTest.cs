using cPanelSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace cPanelSharpTest
{
    [TestClass]
    public class ClientTest
    {
        public static string _hostname;
        public static string _username;
        public static string _password;

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            _hostname = Environment.GetEnvironmentVariable("CPANELSHARP:TEST:HOSTNAME");
            _username = Environment.GetEnvironmentVariable("CPANELSHARP:TEST:USERNAME");
            _password = Environment.GetEnvironmentVariable("CPANELSHARP:TEST:PASSWORD");
        }

        [TestMethod]
        [ExpectedException(typeof(MissingCredentialsException))]
        public void PwAndAccessHashEmptyThrowsException()
        {
            var client = new cPanelClient(_username, _hostname);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCredentialsException))]
        public void BothPwAndAccessHashNotEmptyThrowsException()
        {
            var client = new cPanelClient(_username, _hostname, password: _password, accessHash: "foo");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCredentialsException))]
        public void CpanelAndAccessHashThrowsException()
        {
            var client = new cPanelClient(_username, _hostname, accessHash: "foo", cpanel: true);
        }

        [TestMethod]
        public void ValidCredsCreatesClient()
        {
            var client = new cPanelClient(_username, _hostname, password: _password, cpanel: true);
            Assert.IsNotNull(client);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidParametersException))]
        public void WhmAndNoUserThrowsException()
        {
            var client = new cPanelClient(_username, _hostname, accessHash: "foo");
            client.Api2("fooModule", "fooFunc");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidParametersException))]
        public void NoModuleThrowsException()
        {
            var client = new cPanelClient(_username, _hostname, password: _password);
            client.Api2("", "fooFunc");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidParametersException))]
        public void NoFunctionThrowsException()
        {
            var client = new cPanelClient(_username, _hostname, password: _password, cpanel: true);
            client.Api2("fooModule", "");
        }

        [TestMethod]
        public void GetMonthlyBandwidthReturnsBandwidth()
        {
            var client = new cPanelClient(_username, _hostname, password: _password, cpanel: true);
            var bandwidthResponse = client.Api2("Stats", "getmonthlybandwidth");

            Assert.IsTrue(bandwidthResponse.Length > 0);
        }

        [TestMethod]
        public void GetEmailsByRegexReturnsEmails()
        {
            var client = new cPanelClient(_username, _hostname, password: _password, cpanel: true);
            var emailListResponse = client.Api2("Email", "listpops", param: new { regex = "pr" });

            Assert.IsTrue(emailListResponse.Length > 0);
        }

    }
}
