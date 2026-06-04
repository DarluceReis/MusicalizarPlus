using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;

namespace MusicalizarPlus.Services;

public sealed class StorageOptions
{
    public string Provider { get; set; } = "Local";
    public string LocalPath { get; set; } = "App_Data/uploads";
    public S3StorageOptions S3 { get; set; } = new();
}

public sealed class S3StorageOptions
{
    public string BucketName { get; set; } = "";
    public string Region { get; set; } = "us-east-1";
    public string Prefix { get; set; } = "musicalizarplus";
}

public sealed record StoredFileContent(Stream Content, string ContentType, string FileName);

public interface IFileStorage
{
    Task<UploadedAsset?> SaveAsync(IBrowserFile? file, string folder, CancellationToken cancellationToken = default);
    Task<UploadedAsset?> SaveAsync(IFormFile? file, string folder, CancellationToken cancellationToken = default);
    Task<StoredFileContent?> OpenReadAsync(UploadedAsset? asset, CancellationToken cancellationToken = default);
}

public sealed class LocalFileStorage(
    IWebHostEnvironment environment,
    IOptions<StorageOptions> options) : IFileStorage
{
    private readonly string rootPath = Path.GetFullPath(Path.Combine(environment.ContentRootPath, options.Value.LocalPath));

    public async Task<UploadedAsset?> SaveAsync(IBrowserFile? file, string folder, CancellationToken cancellationToken = default)
    {
        if (file is null)
        {
            return null;
        }

        var contentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType;
        var key = CreateKey(folder, file.Name);
        var fullPath = Path.GetFullPath(Path.Combine(rootPath, key.Replace('/', Path.DirectorySeparatorChar)));
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await using var source = file.OpenReadStream(500 * 1024 * 1024, cancellationToken);
        await using var target = File.Create(fullPath);
        await source.CopyToAsync(target, cancellationToken);

        return new UploadedAsset
        {
            Key = key,
            FileName = file.Name,
            ContentType = contentType
        };
    }

    public async Task<UploadedAsset?> SaveAsync(IFormFile? file, string folder, CancellationToken cancellationToken = default)
    {
        if (file is null || file.Length == 0)
        {
            return null;
        }

        var contentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType;
        var key = CreateKey(folder, file.FileName);
        var fullPath = Path.GetFullPath(Path.Combine(rootPath, key.Replace('/', Path.DirectorySeparatorChar)));
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await using var source = file.OpenReadStream();
        await using var target = File.Create(fullPath);
        await source.CopyToAsync(target, cancellationToken);

        return new UploadedAsset
        {
            Key = key,
            FileName = file.FileName,
            ContentType = contentType
        };
    }

    public Task<StoredFileContent?> OpenReadAsync(UploadedAsset? asset, CancellationToken cancellationToken = default)
    {
        if (asset is null)
        {
            return Task.FromResult<StoredFileContent?>(null);
        }

        var fullPath = Path.GetFullPath(Path.Combine(rootPath, asset.Key.Replace('/', Path.DirectorySeparatorChar)));
        if (!File.Exists(fullPath))
        {
            return Task.FromResult<StoredFileContent?>(null);
        }

        Stream stream = File.OpenRead(fullPath);
        return Task.FromResult<StoredFileContent?>(new StoredFileContent(stream, asset.ContentType, asset.FileName));
    }

    private static string CreateKey(string folder, string fileName)
    {
        var safeName = string.Join("_", Path.GetFileName(fileName).Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
        return $"{folder.Trim('/')}/{Guid.NewGuid():N}-{safeName}";
    }
}

public sealed class S3FileStorage : IFileStorage
{
    private readonly AmazonS3Client client;
    private readonly S3StorageOptions options;

    public S3FileStorage(IOptions<StorageOptions> storageOptions)
    {
        options = storageOptions.Value.S3;
        client = new AmazonS3Client(RegionEndpoint.GetBySystemName(options.Region));
    }

    public async Task<UploadedAsset?> SaveAsync(IBrowserFile? file, string folder, CancellationToken cancellationToken = default)
    {
        if (file is null)
        {
            return null;
        }

        var contentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType;
        var key = CreateKey(folder, file.Name);
        await using var stream = file.OpenReadStream(500 * 1024 * 1024, cancellationToken);

        await client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = options.BucketName,
            Key = key,
            InputStream = stream,
            ContentType = contentType
        }, cancellationToken);

        return new UploadedAsset
        {
            Key = key,
            FileName = file.Name,
            ContentType = contentType
        };
    }

    public async Task<UploadedAsset?> SaveAsync(IFormFile? file, string folder, CancellationToken cancellationToken = default)
    {
        if (file is null || file.Length == 0)
        {
            return null;
        }

        var contentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType;
        var key = CreateKey(folder, file.FileName);
        await using var stream = file.OpenReadStream();

        await client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = options.BucketName,
            Key = key,
            InputStream = stream,
            ContentType = contentType
        }, cancellationToken);

        return new UploadedAsset
        {
            Key = key,
            FileName = file.FileName,
            ContentType = contentType
        };
    }

    public async Task<StoredFileContent?> OpenReadAsync(UploadedAsset? asset, CancellationToken cancellationToken = default)
    {
        if (asset is null)
        {
            return null;
        }

        var response = await client.GetObjectAsync(options.BucketName, asset.Key, cancellationToken);
        var memory = new MemoryStream();
        await response.ResponseStream.CopyToAsync(memory, cancellationToken);
        memory.Position = 0;
        response.Dispose();
        return new StoredFileContent(memory, asset.ContentType, asset.FileName);
    }

    private string CreateKey(string folder, string fileName)
    {
        var safeName = string.Join("_", Path.GetFileName(fileName).Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
        return $"{options.Prefix.Trim('/')}/{folder.Trim('/')}/{Guid.NewGuid():N}-{safeName}";
    }
}
