using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using MiniForm.Application.Interfaces;
using MiniForm.Dtos.Forms;
using MiniForm.Infrastructure.Services;
using MiniForm.Models;
using Xunit;

namespace MiniForm.Tests
{
    public class FormServiceTests
    {
        [Fact]
        public async Task CreateFormAsync_AddsFormAndSaves()
        {
            var formRepo = new Mock<IFormRepository>();
            formRepo.Setup(r => r.AddAsync(It.IsAny<Form>(), It.IsAny<CancellationToken>())).ReturnsAsync((Form f, CancellationToken _) => f).Verifiable();

            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1).Verifiable();
            uow.Setup(u => u.CommitTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var submissionRepo = new Mock<ISubmissionRepository>();

            var service = new FormService(formRepo.Object, uow.Object, submissionRepo.Object);

            var request = new CreateFormRequest
            {
                Title = "T",
                Description = "D",
                Questions = new List<CreateQuestionRequest> { new CreateQuestionRequest { Title = "Q1", IsRequired = true } }
            };

            var res = await service.CreateFormAsync(request, Guid.NewGuid(), CancellationToken.None);

            res.Title.Should().Be("T");
            formRepo.Verify(r => r.AddAsync(It.IsAny<Form>(), It.IsAny<CancellationToken>()), Times.Once);
            uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SubmitFormAsync_Throws_WhenRequiredMissing()
        {
            var formId = Guid.NewGuid();
            var form = new Form
            {
                Id = formId,
                Questions = new List<Question> { new Question { Id = Guid.NewGuid(), IsRequired = true } }
            };

            var formRepo = new Mock<IFormRepository>();
            formRepo.Setup(r => r.GetByIdAsync(formId, It.IsAny<CancellationToken>())).ReturnsAsync(form);

            var uow = new Mock<IUnitOfWork>();
            var submissionRepo = new Mock<ISubmissionRepository>();

            var service = new FormService(formRepo.Object, uow.Object, submissionRepo.Object);

            var request = new SubmitFormRequest { Answers = new List<QuestionAnswer>() };

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.SubmitFormAsync(formId, request, CancellationToken.None));
        }

        [Fact]
        public async Task SubmitFormAsync_SavesSubmission_WhenValid()
        {
            var qid = Guid.NewGuid();
            var formId = Guid.NewGuid();
            var form = new Form
            {
                Id = formId,
                Questions = new List<Question> { new Question { Id = qid, IsRequired = true } }
            };

            var formRepo = new Mock<IFormRepository>();
            formRepo.Setup(r => r.GetByIdAsync(formId, It.IsAny<CancellationToken>())).ReturnsAsync(form);

            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1).Verifiable();
            uow.Setup(u => u.CommitTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var submissionRepo = new Mock<ISubmissionRepository>();
            submissionRepo.Setup(r => r.AddAsync(It.IsAny<Submission>(), It.IsAny<CancellationToken>())).ReturnsAsync((Submission s, CancellationToken _) => s).Verifiable();

            var service = new FormService(formRepo.Object, uow.Object, submissionRepo.Object);

            var request = new SubmitFormRequest { Answers = new List<QuestionAnswer> { new QuestionAnswer { QuestionId = qid, AnswerText = "ok" } } };

            await service.SubmitFormAsync(formId, request, CancellationToken.None);

            submissionRepo.Verify(r => r.AddAsync(It.IsAny<Submission>(), It.IsAny<CancellationToken>()), Times.Once);
            uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
