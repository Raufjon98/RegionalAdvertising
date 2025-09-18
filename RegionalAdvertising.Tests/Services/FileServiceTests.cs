using Castle.DynamicProxy;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Services
{
    public class FileServiceTests
    {
        private readonly FileService _service;
        private readonly IMemoryCache _cache; 

        public FileServiceTests()
        {
            _cache = A.Fake<IMemoryCache>();
            _service = new FileService(_cache);
        }
        
        [Fact]
        public async Task ReadFileReturnsErrorWhenPathEmpty()
        {
            // Act
            var result = await _service.ReadFile("");
            
            //Assert
            var badRequest = Assert.IsType<BadRequest<string>>(result);
            badRequest.Value.Should().Be("File path is required");
        }

        [Fact]
        public async Task ReadFileReturnsErrorWhenPathNotExist()
        { 
            //Act
            var result = await _service.ReadFile("nonexistent.txt");
            
            //Assert
            var badRequest = Assert.IsType<BadRequest<string>>(result);
            badRequest.Value.Should().Be("File not found");
        }
        
        [Fact]
        public async Task ReadFileLoadsDataWhenFileExists()
        {
            //Arrange
            var path = "testFile.txt";
            await File.WriteAllLinesAsync(path, new[]
            {
                "\"/ru\": \"Яндекс.Директ\""
            });
            //Act
            var result = await _service.ReadFile(path);
            //Assert
            var okResult = result.Should().BeOfType<Ok<Dictionary<string, List<string>>>>().Subject;
            okResult.Value?.Values.SelectMany(r => r).Should().Contain("Яндекс.Директ");
            File.Delete(path);
        }
        
        [Fact]
        public async Task GetAdvertisingsByLocationShuldReturnErrorWhenlocationNotExist()
        {
            //Act
            var result = _service.GetAdvertisingsByLocation("");
            //Assert
            var badRequest = Assert.IsType<BadRequest<string>>(result);
            badRequest.Value.Should().Be("Location is required");
        }

        [Fact]
        public async Task GetAdvertisingsByLocation_ShouldReturnMatchingAds()
        {
            // Arrange
            var cache = new MemoryCache(new MemoryCacheOptions());
            var service = new Services.FileService(cache);

            var path =  "test.txt";
            await File.WriteAllLinesAsync(path, new[]
            {
                "\"/ru\": \"Яндекс.Директ\""
            });

            // Act
            await service.ReadFile(path);
            var result = service.GetAdvertisingsByLocation("/ru");

            // Assert
            var okResult = result.Should().BeOfType<Ok<List<string>>>().Subject;
            okResult.Value.Should().Contain("Яндекс.Директ");
            
            File.Delete(path);
        }
    }
}


