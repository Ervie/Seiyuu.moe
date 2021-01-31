namespace SeiyuuMoe.VndbBackgroundJobs.Application.Helpers
{
	internal static class VndbParserHelper
	{
		private const string VndbFileServerBaseUrl = "https://s2.vndb.org/";

		public static string GenerateVndbVisualNovelImageUrlFromImageId(string imageId)
			=> !string.IsNullOrWhiteSpace(imageId) ? 
			$"{VndbFileServerBaseUrl}/cv/{GetImageSubPathFromImageId(imageId)}/{GetImageIdWithoutCategoryFromImageId(imageId)}.jpg" :
			null;

		public static string GenerateVndbCharacterImageUrlFromImageId(string imageId)
			=> !string.IsNullOrWhiteSpace(imageId) ?
			$"{VndbFileServerBaseUrl}/ch/{GetImageSubPathFromImageId(imageId)}/{GetImageIdWithoutCategoryFromImageId(imageId)}.jpg" :
			null;

		private static string GetImageSubPathFromImageId(string imageId)
			=> imageId[^2..];

		private static string GetImageIdWithoutCategoryFromImageId(string imageId)
			=> imageId[2..];
	}
}