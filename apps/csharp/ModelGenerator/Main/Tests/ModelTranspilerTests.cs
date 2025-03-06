using Main.Internal;
using Main.Internal.Domain;
using Main.Utils;
using Xunit;

namespace Main.Tests
{
    public class ModelTranspilerTests
    {
        private string? GetBasePath()
        {
            var directory = Paths.TryGetSolutionDirectoryInfo();
            return directory != null ? $"{directory}/Main/Out" : null;
        }

        private string? GetModelsPath()
        {
            var directory = Paths.TryGetSolutionDirectoryInfo();
            return directory != null ? $"{directory}/Main/Models" : null;
        }

        [Fact]
        public void GenerateTypeScriptInterfaces_ShouldCreateOutputDirectory()
        {
            string? basePath = GetBasePath();
            string? modelsPath = GetModelsPath();
            var categories = new List<Category>
            {
                new Category { Name = "Movie", Folder = "movie" }
            };

            if (modelsPath != null && basePath != null)
            {
                ModelTranspiler.GenerateTypeScriptInterfaces(basePath, modelsPath, categories);
                Assert.True(Directory.Exists(basePath));
            }
            else
            {
                Assert.Fail();
            }
        }

        [Fact]
        public void GenerateTypeScriptInterfaces_ShouldDeleteExistingOutputDirectory()
        {
            string? basePath = GetBasePath();
            string? modelsPath = GetModelsPath();
            var categories = new List<Category>
            {
                new Category { Name = "Movie", Folder = "movie" }
            };

            Directory.CreateDirectory(basePath);
            File.WriteAllText(Path.Combine(basePath, "test.txt"), "test");

            if (modelsPath != null && basePath != null)
            {
                ModelTranspiler.GenerateTypeScriptInterfaces(basePath, modelsPath, categories);
                Assert.False(File.Exists(Path.Combine(basePath, "test.txt")));
            }
            else
            {
                Assert.Fail();
            }
        }

        [Fact]
        public void GenerateTypeScriptInterfaces_ShouldGenerateTypeScriptFiles()
        {
            string? basePath = GetBasePath();
            string? modelsPath = GetModelsPath();
            var categories = new List<Category>
            {
                new Category { Name = "Movie", Folder = "movie" }
            };
            if (modelsPath != null && basePath != null)
            {
                ModelTranspiler.GenerateTypeScriptInterfaces(basePath, modelsPath, categories);

                var tsFiles = Directory.GetFiles(basePath, "*.ts", SearchOption.AllDirectories);
                Assert.NotEmpty(tsFiles);
            }
            else
            {
                Assert.Fail();
            }
        }

        [Fact]
        public void GenerateTypeScriptInterfaces_ShouldHandleEmptyModelsPath()
        {
            string? basePath = GetBasePath();
            string? modelsPath = GetModelsPath();
            var categories = new List<Category>
            {
                new Category { Name = "Movie", Folder = "movie" }
            };

            if (modelsPath != null && basePath != null)
            {
                ModelTranspiler.GenerateTypeScriptInterfaces(basePath, modelsPath, categories);

                var tsFiles = Directory.GetFiles(basePath, "*.ts", SearchOption.AllDirectories);
                Assert.Empty(tsFiles);
            }
            else
            {
                Assert.Fail();
            }
        }

        [Fact]
        public void GenerateTypeScriptInterfaces_ShouldCategorizeFilesCorrectly()
        {
            string? basePath = GetBasePath();
            string? modelsPath = GetModelsPath();
            var categories = new List<Category>
            {
                new Category { Name = "Movie", Folder = "movie" },
                new Category { Name = "User", Folder = "user" }
            };
            if (modelsPath != null && basePath != null)
            {
                ModelTranspiler.GenerateTypeScriptInterfaces(basePath, modelsPath, categories);

                var movieFiles =
                    Directory.GetFiles(Path.Combine(basePath, "movie"), "*.ts", SearchOption.AllDirectories);
                var userFiles = Directory.GetFiles(Path.Combine(basePath, "user"), "*.ts", SearchOption.AllDirectories);
                Assert.NotEmpty(movieFiles);
                Assert.NotEmpty(userFiles);
            }
            else
            {
                Assert.Fail();
            }
        }

        [Fact]
        public void GenerateTypeScriptInterfaces_ShouldHandleNestedTypes()
        {
            string? basePath = GetBasePath();
            string? modelsPath = GetModelsPath();
            var categories = new List<Category>
            {
                new Category { Name = "Movie", Folder = "movie" }
            };
            if (modelsPath != null && basePath != null)
            {
                ModelTranspiler.GenerateTypeScriptInterfaces(basePath, modelsPath, categories);

                var tsFiles = Directory.GetFiles(basePath, "*.ts", SearchOption.AllDirectories);
                Assert.NotEmpty(tsFiles);
            }
            else
            {
                Assert.Fail();
            }
        }
    }
}