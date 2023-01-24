using FileRepoServiceApi.Models;
using FileRepoServiceApi.Services;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace FileRepoApiTestProject
{
    public class RepoDbTests : SqliteTestBase
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void Test1()
        {
            
            Assert.Pass();
        }

        [Test]
        public async Task DatabaseIsAvailableAndCanBeConnectedTo()
        {
            Assert.True(await DbContext.Database.CanConnectAsync());
        }

        [Test]
        public async Task FileItemIsAddedToDbIsRetrievable()
        {
            string fullFileName = "C:\\Users\\jschiller\\source\\FileRepoApiTestProject\\TestData\\testfile1.txt";
            FileItem fileItem = CreateFileItem(fullFileName, fullFileName);

            FileRepository fileRepository = new FileRepository(DbContext);

            await fileRepository.AddAsync(fileItem);

            await DbContext.SaveChangesAsync();     

            var addedFileItem = await fileRepository.GetFileItem(fullFileName);

            Assert.True(addedFileItem.Data == fileItem);
        }
        
        [Test]
        public async Task NewFileItemIsAssignedFirstVersionNumber()
        {
            string fullFileName = "C:\\Users\\jschiller\\source\\FileRepoApiTestProject\\TestData\\testfile1.txt";
            FileItem fileItem = CreateFileItem(fullFileName, fullFileName);

            FileRepository fileRepository = new FileRepository(DbContext);

            await fileRepository.AddAsync(fileItem);

            await DbContext.SaveChangesAsync();     

            var addedFileItem = await fileRepository.GetFileItem(fullFileName);

            Assert.True(addedFileItem.Data != null && addedFileItem.Data.Version == 1.0);
        }

        [Test]
        public async Task RevisionFileItemIsAssignedIncrementedVersionNumber()
        {
            string fullFileName1 = "C:\\Users\\jschiller\\source\\FileRepoApiTestProject\\TestData\\testfile1.txt";
            string fullFileName2 = "C:\\Users\\jschiller\\source\\FileRepoApiTestProject\\TestData\\testfile2.txt";

            FileItem fileItem = CreateFileItem(fullFileName1, fullFileName1);
            FileItem fileItem2 = CreateFileItem(fullFileName1, fullFileName2);

            FileRepository fileRepository = new FileRepository(DbContext);

            await fileRepository.AddAsync(fileItem);
            await fileRepository.AddAsync(fileItem2);

            await DbContext.SaveChangesAsync();     

            var addedFileItem1 = await fileRepository.GetFileItem(fullFileName1);
            var addedFileItem2 = await fileRepository.GetFileItem(fullFileName1, 1.0F);


            Assert.True(addedFileItem1.Data != null && addedFileItem1.Data.Version == 1.1F);
            Assert.True(addedFileItem2.Data != null && addedFileItem2.Data.Version == 1.0F);
        }

        private static FileItem CreateFileItem(string fullFileName, string contentFile)
        {
            return new FileItem
            {
                FileName = fullFileName,
                Contents = File.ReadAllBytes(contentFile),
                Size = 43,
                FileType = "TEXT",
                Checksum = "DFHJKYT456"
            };
        }
    }
}