using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using MiniForm.Application.Interfaces;
using MiniForm.Dtos.Auth;
using MiniForm.Infrastructure.Services;
using MiniForm.Models;
using Xunit;

namespace MiniForm.Tests
{
    public class UserServiceTests
    {
        [Fact]
        public async Task RegisterAsync_AddsUserAndSaves()
        {
            var userRepo = new Mock<IUserRepository>();
            userRepo.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
            userRepo.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask).Verifiable();

            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1).Verifiable();
            uow.Setup(u => u.CommitTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var service = new UserService(userRepo.Object, uow.Object);

            var request = new RegisterRequest { Email = "test@example.com", Password = "Secret123" };

            var user = await service.RegisterAsync(request, CancellationToken.None);

            user.Email.Should().Be("test@example.com");
            user.PasswordHash.Should().NotBeNullOrWhiteSpace();
            userRepo.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
            uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ValidateCredentialsAsync_ReturnsUser_WhenPasswordMatches()
        {
            var email = "valid@example.com";
            var password = "Password!123";

            var user = new User { Id = Guid.NewGuid(), Email = email };
            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
            user.PasswordHash = hasher.HashPassword(user, password);

            var userRepo = new Mock<IUserRepository>();
            userRepo.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>())).ReturnsAsync(user);

            var uow = new Mock<IUnitOfWork>();
            var service = new UserService(userRepo.Object, uow.Object);

            var result = await service.ValidateCredentialsAsync(new LoginRequest { Email = email, Password = password }, CancellationToken.None);

            result.Should().NotBeNull();
            result!.Email.Should().Be(email);
        }
    }
}
