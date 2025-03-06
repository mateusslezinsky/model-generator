using Main.Internal;
using Main.Internal.Domain;
using Main.Utils;

namespace Main
{
    class Program
    {
        public static void Main()
        {
            var directory = Paths.TryGetSolutionDirectoryInfo();
            if (directory != null)
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Movie", Folder = "movie" },
                };
                ModelTranspiler.GenerateTypeScriptInterfaces($"{directory}/Main/Out", $"{directory}/Main/Models", categories);
            }
        } 
    } 
}
