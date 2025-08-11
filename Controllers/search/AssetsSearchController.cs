// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using MongoDB.Driver;
// using MyAssetsManagerBackend.Entities;
//
// namespace MyAssetsManagerBackend.Controllers.search;
//
//  [ApiController]
//     [Route("api/[controller]")]
//     public class AssetsSearchController : AbstractSearchController
//     {
//         private readonly IMongoCollection<SourceFile> _assetsCollection;
//         private readonly ThumbnailService _thumbnailService;
//         private readonly CloudFrontClient _cloudFrontClient;
//
//         private const long ACL_ASSET_CLASS = 4L;
//
//         public AssetsSearchController(
//             IMongoDatabase database,
//             ThumbnailService thumbnailService,
//             CloudFrontClient cloudFrontClient
//         ) : base(null, database) // IMongoClient optional here
//         {
//             _assetsCollection = database.GetCollection<SourceFile>("SourceFiles");
//             _thumbnailService = thumbnailService;
//             _cloudFrontClient = cloudFrontClient;
//         }
//
//         [HttpGet("assets")]
//         [Authorize]
//         public async Task<IActionResult> Search(
//             [FromQuery] string search,
//             [FromQuery] bool applyArbitraryOrder = false,
//             [FromQuery] string projection = "allDetails",
//             [FromQuery] bool includeSystem = true,
//             [FromQuery] int page = 0,
//             [FromQuery] int size = 20
//         )
//         {
//             var user = new AuthenticatedUser { Name = User.Identity!.Name ?? "" };
//             bool isAdmin = User.IsInRole("ADMIN");
//
//             // Build search filter
//             var searchFilter = GetBuilder<SourceFile>(search);
//
//             // Build ACL filter
//             var aclFilter = GetAclConditionForClass(
//                 "com.guroos.data.domain.SourceFile",
//                 user,
//                 isAdmin,
//                 new List<string>() // group authority list if any
//             );
//
//             // Combine filters
//             var builder = Builders<SourceFile>.Filter;
//             var filter = builder.And(searchFilter, aclFilter);
//
//             if (!includeSystem)
//             {
//                 filter = builder.And(filter,
//                     builder.Ne("Metadata.Subcategory", "SYSTEM"));
//             }
//
//             // Sorting
//             var sort = Builders<SourceFile>.Sort;
//             var sortDefinition = applyArbitraryOrder
//                 ? sort.Ascending("ArbitraryOrder")
//                 : sort.Descending("_id"); // default
//
//             // Pagination
//             var totalCount = await _assetsCollection.CountDocumentsAsync(filter);
//             var results = await _assetsCollection
//                 .Find(filter)
//                 .Sort(sortDefinition)
//                 .Skip(page * size)
//                 .Limit(size)
//                 .ToListAsync();
//
//             // Sign thumbnails & URLs
//             var start = DateTime.UtcNow;
//             foreach (var sf in results)
//             {
//                 if (sf.Thumbnail?.ThumbnailUrls != null &&
//                     (!string.IsNullOrEmpty(sf.Thumbnail.ThumbnailUrls.Small) ||
//                      !string.IsNullOrEmpty(sf.Thumbnail.ThumbnailUrls.Medium)))
//                 {
//                     sf.Thumbnail = _thumbnailService.SignThumbnails(sf.Thumbnail);
//                 }
//                 else
//                 {
//                     sf.Thumbnail = null;
//                 }
//
//                 if (!string.IsNullOrEmpty(sf.Url))
//                 {
//                     try
//                     {
//                         sf.UrlSigned = _cloudFrontClient.SignCloudFrontURL(sf.Url);
//                     }
//                     catch (Exception ex)
//                     {
//                         Console.Error.WriteLine($"Unable to sign SF URL: {ex}");
//                     }
//                 }
//             }
//             var elapsed = DateTime.UtcNow - start;
//             Console.WriteLine($"URL signing took {elapsed.TotalMilliseconds} ms");
//
//             // Apply projections
//             object finalResult;
//             switch (projection)
//             {
//                 case "smallThumbnail":
//                     finalResult = results.Select(p => new SourceFileSmallThumbnail(p));
//                     break;
//                 case "mediumThumbnail":
//                     finalResult = results.Select(p => new SourceFileMediumThumbnail(p));
//                     break;
//                 case "noDetails":
//                     finalResult = results.Select(p => new SourceFileNoDetails(p));
//                     break;
//                 default:
//                     finalResult = results.Select(p => new SourceFileAllDetails(p));
//                     break;
//             }
//
//             var pagedResult = new
//             {
//                 total = totalCount,
//                 page,
//                 size,
//                 data = finalResult
//             };
//
//             return Ok(pagedResult);
//         }
//     }