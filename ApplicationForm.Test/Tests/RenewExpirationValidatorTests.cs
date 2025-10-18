using Xunit;
using System;
using Application_Form.Application.Feature.ApplicatioForm.Command.RenewApplicationExpirationDate;

namespace ApplicationForm.Test.Tests
{
    public class RenewExpirationValidatorTests
    {
        [Fact]
        public void Validator_Fails_When_ApplicationId_Is_Empty()
        {
            var cmd = new RenewApplicationExpirationDateCommand(Guid.Empty, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)));
            var validator = new RenewApplicationExpirationDateValidator();

            var result = validator.Validate(cmd);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "ApplicationId");
        }

        [Fact]
        public void Validator_Fails_When_NewExpirationDate_Is_Not_Future()
        {
            var cmd = new RenewApplicationExpirationDateCommand(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)));
            var validator = new RenewApplicationExpirationDateValidator();

            var result = validator.Validate(cmd);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "NewExpirationDate");
        }

        [Fact]
        public void Validator_Succeeds_For_Valid_Command()
        {
            var cmd = new RenewApplicationExpirationDateCommand(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)));
            var validator = new RenewApplicationExpirationDateValidator();

            var result = validator.Validate(cmd);

            Assert.True(result.IsValid);
        }
    }
}
