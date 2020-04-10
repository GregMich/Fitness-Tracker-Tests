using Fitness_Tracker.Infrastructure.PasswordSecurity;
using Xunit;

namespace Fitness_Tracker_Tests.SecurityTests
{
    public class PasswordSecurityTests
    {
        [Theory]
        [InlineData("12345678")]
        [InlineData("123456789")]
        [InlineData("1234567899")]
        public void CorrectPasswordLengthsReturnTrue(string plainTextPassword) =>
            Assert.True(PasswordSecurity.ConfirmLengthRequirements(plainTextPassword));

        [Theory]
        [InlineData("123")]
        [InlineData("12345678912345678912345678912345678912345678912345678901234567890")]
        public void IncorrectPasswordLengthsReturnFalse(string plainTextPassword) =>
            Assert.False(PasswordSecurity.ConfirmLengthRequirements(plainTextPassword));

        [Theory]
        [InlineData("pass&")]
        [InlineData("pass/")]
        [InlineData("pass#")]
        [InlineData("pass\"")]
        public void SpecialCharacterCountCorrectReturnsTrue(string plaintextPassword) =>
            Assert.True(
                PasswordSecurity.ConfirmSpecialCharacterRequirements(plaintextPassword));

        [Fact]
        public void SpecialCharacterIncorrectCountReturnsFalse() =>
            Assert.False(
                PasswordSecurity.ConfirmSpecialCharacterRequirements("password12345"));

        [Theory]
        [InlineData("pass1")]
        [InlineData("pass234")]
        public void NumericCharacterCorrectReturnsTrue(string plaintextPassword) =>
            Assert.True(PasswordSecurity.ConfirmNumericRequirements(plaintextPassword));

        [Fact]
        public void NumericCharacterIncorrectReturnsFalse() =>
            Assert.False(PasswordSecurity.ConfirmNumericRequirements("asdasd$$\\"));

        [Fact]
        public void NoWhiteSpacePasswordReturnsTrue() =>
            Assert.True(PasswordSecurity.ConfirmWhitespaceRequirements("password"));

        [Fact]
        public void WhitespacePasswordReturnsFalse() =>
            Assert.False(PasswordSecurity.ConfirmWhitespaceRequirements(" password"));

        [Theory]
        [InlineData("passWORd")]
        [InlineData("password")]
        public void LowerCaseCorrectReturnsTrue(string plaintextPassword) =>
            Assert.True(
                PasswordSecurity.ConfirmLowerCaseCharacterRequirements(plaintextPassword));

        [Fact]
        public void LowerCaseIncorrectReturnsFalse() =>
            Assert.False(PasswordSecurity.ConfirmLowerCaseCharacterRequirements("PASSWORD"));

        [Theory]
        [InlineData("passWORd")]
        [InlineData("PASSWORD")]
        public void UpperCaseCorrectReturnsTrue(string plaintextPassword) =>
            Assert.True(
                PasswordSecurity.ConfirmUpperCaseCharacterRequirements(plaintextPassword));

        [Fact]
        public void UpperCaseIncorrectReturnsFalse() =>
            Assert.False(PasswordSecurity.ConfirmUpperCaseCharacterRequirements("password"));

        [Theory]
        [InlineData("gwt213Y78#!!")]
        [InlineData("jD313AHhd$\\\"")]
        [InlineData("kOO((788899")]
        public void CorrectCheckPasswordPolicyReturnsTrue(string plaintextPassword) =>
            Assert.True(PasswordSecurity.CheckPasswordPolicies(plaintextPassword));

        [Theory]
        [InlineData("GWTY7889!!!!")]  // no lowercase
        [InlineData("gwty7899$%#")]  // no lowercase
        [InlineData("gwTY7898974")]  // no special character
        [InlineData("gwTY$%^&#")]  // no number
        [InlineData("gW7$")]  // too short
        [InlineData("gwt213Y78#!!gwt213Y78#!!gwt213Y78#!!" +
            "gwt213Y78#!!gwt213Y78#!!gwt213Y78#!!")]  // too long
        [InlineData(" das873D##")]  // whitespace in password
        public void IncorrectCheckPasswordPolicyReturnsFalse(string plaintextPassword) =>
            Assert.False(PasswordSecurity.CheckPasswordPolicies(plaintextPassword));

    }
}
