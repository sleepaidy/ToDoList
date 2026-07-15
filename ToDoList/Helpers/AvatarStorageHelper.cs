namespace ToDoList.Helpers
{
    public static class AvatarStorageHelper
    {
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".webp", ".gif"
        };

        public static string? TryGetSafeExtension(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
            {
                return null;
            }

            return extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase)
                ? ".jpg"
                : extension.ToLowerInvariant();
        }

        public static string SaveAvatar(IWebHostEnvironment env, int userId, IFormFile file, string extension)
        {
            var avatarsDir = Path.Combine(env.WebRootPath, "images", "avatars");
            Directory.CreateDirectory(avatarsDir);

            foreach (var oldFile in Directory.GetFiles(avatarsDir, $"avatar-{userId}.*"))
            {
                File.Delete(oldFile);
            }

            var fileName = $"avatar-{userId}{extension}";
            var path = Path.Combine(avatarsDir, fileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return $"/images/avatars/{fileName}";
        }
    }
}
