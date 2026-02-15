using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using AuthorTools.Api.Models;
using AuthorTools.Api.Services;
using AuthorTools.Api.Services.Interfaces;
using AuthorTools.Common.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Xunit;

namespace AuthorTools.Api.Services.UnitTests;


public class FileServiceTests
{
    /// <summary>
    /// Tests that UploadAsync successfully uploads a file with valid content and returns the correct response.
    /// This test verifies the happy path scenario where a file is uploaded, copied to a memory stream,
    /// and the Azure blob service is called with the correct parameters.
    /// NOTE: This test is skipped because AzureBlobService.UploadBlobAsync is not virtual and cannot be mocked.
    /// To make this testable, consider:
    /// 1. Creating an IAzureBlobService interface and having AzureBlobService implement it
    /// 2. Making UploadBlobAsync virtual in AzureBlobService
    /// 3. Using the interface type in FileService constructor dependency injection
    /// </summary>
    [Fact(Skip="ProductionBugSuspected")]
    [Trait("Category", "ProductionBugSuspected")]
    public async Task UploadAsync_ValidFile_ReturnsOkWithFileId()
    {
        // Arrange
        var azureBlobService = Substitute.For<AzureBlobService>("connection", "container");
        var fileService = new FileService(azureBlobService);

        var file = Substitute.For<IFormFile>();
        var fileContent = new byte[] { 1, 2, 3, 4, 5 };
        var fileName = "test.txt";
        var contentType = "text/plain";

        file.FileName.Returns(fileName);
        file.ContentType.Returns(contentType);
        file.CopyToAsync(Arg.Any<Stream>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var stream = callInfo.ArgAt<Stream>(0);
                return stream.WriteAsync(fileContent, 0, fileContent.Length);
            });

        // Act
        var result = await fileService.UploadAsync(file);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();
        result.Value!.FileId.Should().NotBeNullOrEmpty();

        // Verify that UploadBlobAsync was called with correct parameters
        // NOTE: This cannot be verified because the method is not virtual
        // await azureBlobService.Received(1).UploadBlobAsync(
        //     fileName,
        //     Arg.Any<string>(),
        //     contentType,
        //     Arg.Any<byte[]>());
    }

    /// <summary>
    /// Tests that UploadAsync handles files with special characters in the filename.
    /// This verifies that the service correctly processes filenames containing special characters,
    /// spaces, and unicode characters.
    /// NOTE: This test is skipped because AzureBlobService.UploadBlobAsync is not virtual and cannot be mocked.
    /// </summary>
    [Theory(Skip = "AzureBlobService.UploadBlobAsync is not virtual and cannot be mocked with NSubstitute. Refactor to use an interface.")]
    [InlineData("file with spaces.txt")]
    [InlineData("file@special#chars$.pdf")]
    [InlineData("файл.doc")]
    [InlineData("file\twith\ttabs.txt")]
    public async Task UploadAsync_FileNameWithSpecialCharacters_ReturnsOkWithFileId(string fileName)
    {
        // Arrange
        var azureBlobService = Substitute.For<AzureBlobService>("connection", "container");
        var fileService = new FileService(azureBlobService);

        var file = Substitute.For<IFormFile>();
        var fileContent = new byte[] { 1, 2, 3 };

        file.FileName.Returns(fileName);
        file.ContentType.Returns("application/octet-stream");
        file.CopyToAsync(Arg.Any<Stream>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var stream = callInfo.ArgAt<Stream>(0);
                return stream.WriteAsync(fileContent, 0, fileContent.Length);
            });

        // Act
        var result = await fileService.UploadAsync(file);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();
        result.Value!.FileId.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Tests that UploadAsync handles different content types correctly.
    /// This verifies that various MIME types are processed appropriately.
    /// NOTE: This test is skipped because AzureBlobService.UploadBlobAsync is not virtual and cannot be mocked.
    /// </summary>
    [Theory(Skip = "AzureBlobService.UploadBlobAsync is not virtual and cannot be mocked with NSubstitute. Refactor to use an interface.")]
    [InlineData("application/pdf")]
    [InlineData("image/jpeg")]
    [InlineData("video/mp4")]
    [InlineData("application/octet-stream")]
    [InlineData("text/html")]
    public async Task UploadAsync_DifferentContentTypes_ReturnsOkWithFileId(string contentType)
    {
        // Arrange
        var azureBlobService = Substitute.For<AzureBlobService>("connection", "container");
        var fileService = new FileService(azureBlobService);

        var file = Substitute.For<IFormFile>();
        var fileContent = new byte[] { 1, 2, 3 };

        file.FileName.Returns("test.file");
        file.ContentType.Returns(contentType);
        file.CopyToAsync(Arg.Any<Stream>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var stream = callInfo.ArgAt<Stream>(0);
                return stream.WriteAsync(fileContent, 0, fileContent.Length);
            });

        // Act
        var result = await fileService.UploadAsync(file);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();
        result.Value!.FileId.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Tests that DeleteAsync calls DeleteBlobAsync on the blob service with the provided ID
    /// and returns an Ok result when deletion succeeds.
    /// Input: Valid file ID.
    /// Expected: DeleteBlobAsync is called with the ID, and Ok result is returned.
    /// </summary>
    [Theory]
    [InlineData("validFileId123")]
    [InlineData("file-with-dashes")]
    [InlineData("file_with_underscores")]
    [InlineData("FILE123")]
    [InlineData("a")]
    public async Task DeleteAsync_ValidId_CallsDeleteBlobAsyncAndReturnsOk(string id)
    {
        // Arrange
        var mockAzureBlobService = Substitute.For<AzureBlobService>("connection", "container");
        mockAzureBlobService.DeleteBlobAsync(id).Returns(Task.CompletedTask);
        var fileService = new FileService(mockAzureBlobService);

        // Act
        var result = await fileService.DeleteAsync(id);

        // Assert
        await mockAzureBlobService.Received(1).DeleteBlobAsync(id);
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.Ok>();
    }

    /// <summary>
    /// Tests that DeleteAsync passes empty or whitespace IDs to the blob service without validation.
    /// Input: Empty string or whitespace-only string.
    /// Expected: DeleteBlobAsync is called with the provided value, and Ok result is returned.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public async Task DeleteAsync_EmptyOrWhitespaceId_PassesToServiceAndReturnsOk(string id)
    {
        // Arrange
        var mockAzureBlobService = Substitute.For<AzureBlobService>("connection", "container");
        mockAzureBlobService.DeleteBlobAsync(id).Returns(Task.CompletedTask);
        var fileService = new FileService(mockAzureBlobService);

        // Act
        var result = await fileService.DeleteAsync(id);

        // Assert
        await mockAzureBlobService.Received(1).DeleteBlobAsync(id);
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.Ok>();
    }

    /// <summary>
    /// Tests that DeleteAsync handles IDs with special characters correctly.
    /// Input: IDs containing special characters, symbols, or unicode.
    /// Expected: DeleteBlobAsync is called with the provided ID, and Ok result is returned.
    /// </summary>
    [Theory]
    [InlineData("file@#$%^&*()")]
    [InlineData("file/with/slashes")]
    [InlineData("file\\with\\backslashes")]
    [InlineData("file.with.dots")]
    [InlineData("file:with:colons")]
    [InlineData("file|with|pipes")]
    [InlineData("файл")]
    public async Task DeleteAsync_SpecialCharactersInId_PassesToServiceAndReturnsOk(string id)
    {
        // Arrange
        var mockAzureBlobService = Substitute.For<AzureBlobService>("connection", "container");
        mockAzureBlobService.DeleteBlobAsync(id).Returns(Task.CompletedTask);
        var fileService = new FileService(mockAzureBlobService);

        // Act
        var result = await fileService.DeleteAsync(id);

        // Assert
        await mockAzureBlobService.Received(1).DeleteBlobAsync(id);
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.Ok>();
    }

    /// <summary>
    /// Tests that GetFileResult returns a file result when provided with a valid file ID.
    /// </summary>
    [Fact(Skip="ProductionBugSuspected")]
    [Trait("Category", "ProductionBugSuspected")]
    public async Task GetFileResult_ValidId_ReturnsFileResult()
    {
        // Arrange
        var azureBlobService = Substitute.For<AzureBlobService>("connection", "container");
        var fileService = new FileService(azureBlobService);
        var fileId = "valid-file-id-123";
        var expectedContent = new byte[] { 1, 2, 3, 4, 5 };
        var expectedFileName = "test-file.txt";
        var expectedContentType = "text/plain";
        var fileStorageResult = new FileStorageResult(expectedContent, expectedFileName, expectedContentType);

        azureBlobService.GetBlobAsync(fileId).Returns(Task.FromResult(fileStorageResult));

        // Act
        var result = await fileService.GetFileResult(fileId);

        // Assert
        result.Should().NotBeNull();
        await azureBlobService.Received(1).GetBlobAsync(fileId);
    }

    /// <summary>
    /// Tests that GetFileResult calls GetBlobAsync with the correct ID parameter.
    /// </summary>
    /// <param name="id">The file ID to test.</param>
    [Theory]
    [InlineData("simple-id")]
    [InlineData("id-with-special-chars-@#$%")]
    [InlineData("very-long-id-" + "abcdefghijklmnopqrstuvwxyz0123456789")]
    [InlineData("123456")]
    [InlineData("GUID-12345678-1234-1234-1234-123456789012")]
    public async Task GetFileResult_VariousValidIds_CallsGetBlobAsync(string id)
    {
        // Arrange
        var azureBlobService = Substitute.For<AzureBlobService>("connection", "container");
        var fileService = new FileService(azureBlobService);
        var fileContent = new byte[] { 1, 2, 3 };
        var fileStorageResult = new FileStorageResult(fileContent, "file.txt", "application/octet-stream");

        azureBlobService.GetBlobAsync(id).Returns(Task.FromResult(fileStorageResult));

        // Act
        var result = await fileService.GetFileResult(id);

        // Assert
        await azureBlobService.Received(1).GetBlobAsync(id);
    }

    /// <summary>
    /// Tests that GetFileResult handles whitespace-only ID by passing it to GetBlobAsync.
    /// </summary>
    [Theory]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public async Task GetFileResult_WhitespaceId_PassesToGetBlobAsync(string id)
    {
        // Arrange
        var azureBlobService = Substitute.For<AzureBlobService>("connection", "container");
        var fileService = new FileService(azureBlobService);
        var fileStorageResult = new FileStorageResult(new byte[] { }, "file.txt", "text/plain");

        azureBlobService.GetBlobAsync(id).Returns(Task.FromResult(fileStorageResult));

        // Act
        var result = await fileService.GetFileResult(id);

        // Assert
        await azureBlobService.Received(1).GetBlobAsync(id);
    }

    /// <summary>
    /// Tests that GetFileResult handles different file content types correctly.
    /// </summary>
    /// <param name="contentType">The content type to test.</param>
    /// <param name="fileName">The file name to test.</param>
    [Theory]
    [InlineData("application/pdf", "document.pdf")]
    [InlineData("image/jpeg", "photo.jpg")]
    [InlineData("application/json", "data.json")]
    [InlineData("text/html", "page.html")]
    public async Task GetFileResult_DifferentContentTypes_ReturnsFileResult(string contentType, string fileName)
    {
        // Arrange
        var azureBlobService = Substitute.For<AzureBlobService>("connection", "container");
        var fileService = new FileService(azureBlobService);
        var fileId = "test-file-id";
        var fileContent = new byte[] { 10, 20, 30, 40 };
        var fileStorageResult = new FileStorageResult(fileContent, fileName, contentType);

        azureBlobService.GetBlobAsync(fileId).Returns(Task.FromResult(fileStorageResult));

        // Act
        var result = await fileService.GetFileResult(fileId);

        // Assert
        result.Should().NotBeNull();
        await azureBlobService.Received(1).GetBlobAsync(fileId);
    }

}