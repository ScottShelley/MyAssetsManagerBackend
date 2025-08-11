using Microsoft.AspNetCore.Mvc;

namespace MyAssetsManagerBackend.Controllers;

[ApiController]
    [Route("sourceFiles")]
    public class SourceFileController : ControllerBase
    {
        private readonly string _s3CloudfrontBase;
        private readonly string _pkPath;
        private readonly string _dropBoxBucketName;

        private readonly ISourceFileRepository _sfr;
        private readonly IS3Client _s3Client;
        private readonly IUserDao _userDao;
        private readonly IFileUploadService _fuService;
        private readonly IThumbnailService _tbService;

        private readonly string[] _systemIC = { "PAUSE", "JUMP", "LOOP" };

        private readonly ILogger<SourceFileController> _logger;

        public SourceFileController(
            IConfiguration config,
            ISourceFileRepository sfr,
            IS3Client s3Client,
            IUserDao userDao,
            IFileUploadService fuService,
            IThumbnailService tbService,
            ILogger<SourceFileController> logger)
        {
            _s3CloudfrontBase = config["aws:s3:cloudfront:base"];
            _pkPath = config["aws:s3:cloudfront:pk:path"];
            _dropBoxBucketName = config["aws:s3:dropbox"];

            _sfr = sfr;
            _s3Client = s3Client;
            _userDao = userDao;
            _fuService = fuService;
            _tbService = tbService;

            _logger = logger;
        }

        [HttpPost("processS3Upload")]
        public async Task<IActionResult> ProcessS3Upload(
            [FromBody] Dictionary<string, string> parameters,
            [FromQuery] bool async = true,
            [FromQuery] bool makePublic = false,
            [FromQuery] bool createAsset = true,
            [FromQuery] string subcategory = null,
            [FromQuery] bool replaceAsset = false,
            [FromQuery] long? replaceAssetId = null)
        {
            var uuid = Guid.NewGuid().ToString();

            parameters.TryGetValue("Key", out var key);
            parameters.TryGetValue("Bucket", out var bucket);

            if (string.IsNullOrEmpty(bucket) || bucket != _dropBoxBucketName)
            {
                _logger.LogError("Bucket names don't match: {Bucket} and {ExpectedBucket}", bucket, _dropBoxBucketName);
                return NotFound(Result.ResponseEntity(uuid, "invalid bucket name"));
            }

            _logger.LogInformation("Processing dropbox file {Key}, async={Async}", key, async);

            if (replaceAsset)
            {
                if (createAsset)
                {
                    return BadRequest(Result.ResponseEntity(uuid, "Replace & create cannot be enabled at the same time"));
                }

                if (!replaceAssetId.HasValue)
                {
                    return BadRequest(Result.ResponseEntity(uuid, "replaceAssetId cannot be empty"));
                }

                if (!await _sfr.ExistsByIdAsync(replaceAssetId.Value))
                {
                    return NotFound(Result.ResponseEntity(uuid, "Asset not found"));
                }

                var mfp = await _s3Client.GetMediaFilePointerAsync(bucket, key);
                var contentType = _fuService.GetContentType(mfp, key);
                var existing = await _sfr.FindByIdAsync(replaceAssetId.Value);

                switch (existing.SourceFileType)
                {
                    case SourceFileType.VIDEOPLACEHOLDER:
                        if (!contentType.Type.Equals("video", StringComparison.OrdinalIgnoreCase))
                            return BadRequest(Result.ResponseEntity(uuid, "content types don't match"));
                        break;
                    case SourceFileType.AUDIOPLACEHOLDER:
                        if (!contentType.Type.Equals("audio", StringComparison.OrdinalIgnoreCase))
                            return BadRequest(Result.ResponseEntity(uuid, "content types don't match"));
                        break;
                    case SourceFileType.IMAGEPLACEHOLDER:
                        if (!contentType.Type.Equals("image", StringComparison.OrdinalIgnoreCase))
                            return BadRequest(Result.ResponseEntity(uuid, "content types don't match"));
                        break;
                    default:
                        _logger.LogInformation("Replacement validation passed for {Uuid} and assetId={AssetId}", uuid, replaceAssetId);
                        break;
                }
            }

            if (async)
            {
                _fuService.ProcessDropboxFileAsync(bucket, key, makePublic, subcategory, createAsset, replaceAsset, replaceAssetId);
                return Ok(Result.ResponseEntity(uuid, null));
            }
            else
            {
                try
                {
                    var sf = await _fuService.ProcessDropboxFile(bucket, key, makePublic, subcategory, createAsset, replaceAsset, replaceAssetId);

                    if (sf?.Thumbnail != null)
                    {
                        var signedThumbnail = _tbService.SignThumbnails(sf.Thumbnail);
                        sf.Thumbnail = signedThumbnail;
                    }

                    return Ok(Result.ResponseEntity(uuid, sf));
                }
                catch (BusinessLogicException ex)
                {
                    return StatusCode(500, Result.ResponseEntity(uuid, ex.Message));
                }
            }
        }
    }

    // Result helper class
    public static class Result
    {
        public static object ResponseEntity(string uuid, object body, string errorMessage = null)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return new { Id = uuid, Error = errorMessage };
            }
            return new { Id = uuid, Data = body };
        }
    }

    // Enums and interfaces (to be implemented in your project)
    public enum SourceFileType
    {
        VIDEOPLACEHOLDER,
        AUDIOPLACEHOLDER,
        IMAGEPLACEHOLDER,
        // add other types as needed
    }

    public class SourceFile
    {
        public SourceFileType SourceFileType { get; set; }
        public Thumbnail Thumbnail { get; set; }
    }

    public class Thumbnail
    {
        // thumbnail properties here
    }

    public interface ISourceFileRepository
    {
        Task<bool> ExistsByIdAsync(long id);
        Task<SourceFile> FindByIdAsync(long id);
    }

    public interface IS3Client
    {
        Task<MediaFilePointer> GetMediaFilePointerAsync(string bucket, string key);
    }

    public class MediaFilePointer
    {
        // properties as needed
    }

    public interface IUserDao { /* your methods */ }

    public interface IFileUploadService
    {
        void ProcessDropboxFileAsync(string bucket, string key, bool makePublic, string subcategory, bool createAsset, bool replaceAsset, long? replaceAssetId);
        Task<SourceFile> ProcessDropboxFile(string bucket, string key, bool makePublic, string subcategory, bool createAsset, bool replaceAsset, long? replaceAssetId);
        MimeType GetContentType(MediaFilePointer mfp, string key);
    }

    public class MimeType
    {
        public string Type { get; set; }
    }

    public interface IThumbnailService
    {
        Thumbnail SignThumbnails(Thumbnail thumbnail);
    }

    public class BusinessLogicException : Exception
    {
        public BusinessLogicException(string message) : base(message) { }
    }
