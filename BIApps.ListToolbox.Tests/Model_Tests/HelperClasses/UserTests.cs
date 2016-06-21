using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BIApps.ListToolbox.Model.Helpers;
using BIApps.ListToolbox.Model.Users;
using NUnit.Framework;

namespace BIApps.ListToolbox.Tests.Model_Tests.HelperClasses {

    [TestFixture]
    public class UserTests {

        [Test]
        public void CheckActiveDirectoryGroups() {
            var expected = Environment.UserName;
            var user = new User();

            Assert.AreEqual(expected, user.Username);
        }

        [Test]
        public void CheckBusinessChannelsAreAvailable() {
            var expected = new List<string> {
                "Bingo",
                "Casino",
                "eGaming",
                "Games",
                "Live Casino",
                "Poker",
                "Roller",
                "Sportsbook",
                "Vegas"
            };
            var channels = new BusinessChannelValues();

            foreach (var value in channels.Channels) {
                Assert.Contains(value, expected);
            }
        }
    }
}
