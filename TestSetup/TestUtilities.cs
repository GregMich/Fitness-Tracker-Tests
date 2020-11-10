using Castle.Core.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Fitness_Tracker.Infrastructure.Security;
using Microsoft.Extensions.Logging;

namespace Fitness_Tracker_Tests.TestSetup
{
    public static class TestUtilities
    {

        public static Mock<ILogger<T>> GenericSetUpControllerStubLogger<T>() =>
            new Mock<ILogger<T>>();

        public static JsonSerializerOptions getTestJsonSerializerOptions() =>
            new JsonSerializerOptions { WriteIndented = true };

        public static ClaimsPrincipal setUpSampleHttpContextUser() =>
            new ClaimsPrincipal(
                new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.Name, "example name"),
                        new Claim(ClaimTypes.NameIdentifier, "1"),
                        new Claim("custom-claim", "example claim value"),
                    },
                "mock"));

        public static Mock<IClaimsManager> SetUpMockClaimsManager(int getUserIdClaimReturnValue)
        {
            var mock = new Mock<IClaimsManager>();
            mock.Setup(m => m.GetUserIdClaim()).Returns(getUserIdClaimReturnValue);
            mock.Setup(m => m.VerifyUserId(getUserIdClaimReturnValue)).Returns(true);
            return mock;
        }
    }
}
