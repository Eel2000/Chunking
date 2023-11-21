// See https://aka.ms/new-console-template for more information

using System.IO;

Console.WriteLine("Hello, World!");
Console.WriteLine("Do you want to download fake or Real File???. [F] / [R]");
var res = Console.ReadLine();
if (res == ConsoleKey.R.ToString())
{
    var file = @"C:\Users\CTRL TECH\Videos\126BuddiesThickerThanWater1962\001   Puss Gets the Boot [1940].mkv";
    if (File.Exists(file))
    {
        using var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
        using var binaryReader = new BinaryReader(fileStream);
        var bytesFile = binaryReader.ReadBytes((int)fileStream.Length);

        float mb = (bytesFile.Length / 1024f) / 1024f;

        var info = new FileInfo(file);
        var download = PrepareDownloadv2(bytesFile, info);
        Console.WriteLine($"Your file {info.Name} {mb} MB download is ready. do you want to confirm to start downloading. [Y] / [N]");
        var response = Console.ReadLine();

        //Simulate the download process on the client machine.
        await Download(download, response);


        //re-organize fragments 
        var data = download.OrderBy(x => x.Key).Select(x => x.Value).ToArray();

        var downloaded = new List<byte>();
        foreach (var item in data)
        {
            downloaded.AddRange(item.Chunck);
        }

        //Create the file
        var binaryWriter = new BinaryWriter(File.OpenWrite(@"C:\Users\CTRL TECH\Videos\" + data.First().Name));
        binaryWriter.Write(downloaded.ToArray());
    }
}
else
{
    Console.WriteLine("Fake download by generating by array of 10_000_000");

    byte[] buffer = new byte[1024];

    var data = Enumerable.Repeat<byte>(1, 10_000_000).ToArray();

    Dictionary<int, Film> download = PrepareDownload(data, 1024);

    Console.WriteLine("Your download is ready. do you want to confirm to start downloading. [Y] / [N]");
    var response = Console.ReadLine();

    await Download(download, response);
}


static Dictionary<int, Film> PrepareDownload(byte[] data, int buffer = 512)
{
    Dictionary<int, Film> download = new();

    var chuncks = data.Chunk(buffer).ToArray();

    int i = 0;

    foreach (var chunk in chuncks)
    {
        download.Add(Array.IndexOf(chuncks, chunk), new Film
        {
            Id = i,
            Chunck = chunk,
            Downloaded = DateTime.Now,
            DownloadedBytes = chunk.Length,
            Name = "nom du fichier",
            DownloadedTotalBytes = data.Length
        });

        i++;
    }

    return download;
}
static Dictionary<int, Film> PrepareDownloadv2(byte[] data, FileInfo fileInfo, int buffer = 512)
{
    Dictionary<int, Film> download = new();

    var chuncks = data.Chunk(buffer).ToArray();

    int i = 0;

    foreach (var chunk in chuncks)
    {
        download.Add(Array.IndexOf(chuncks, chunk), new Film
        {
            Id = i,
            Chunck = chunk,
            Downloaded = DateTime.Now,
            DownloadedBytes = chunk.Length,
            Name = fileInfo.Name,
            DownloadedTotalBytes = data.Length,
        });

        i++;
    }

    return download;
}

static async Task Download(Dictionary<int, Film> download, string response)
{
    if (response == ConsoleKey.Y.ToString())
    {
        int sum = 0;
        foreach (var item in download)
        {
            sum += item.Value.DownloadedBytes;

            float percentage = sum * 100 / item.Value.DownloadedTotalBytes;

            //if(percentage <= 0)
            //    Console.Write(percentage.ToString());

            float mb = (sum / 1024f) / 1024f;
            float totalSize = (item.Value.DownloadedTotalBytes / 1024f) / 1024f;

            //Console.Write($"Downloading {mb} MB / {totalSize} MB\n");
            Console.Write($"\r{percentage}% / 100%");

            await Task.Delay(TimeSpan.FromMicroseconds(1));
        }
    }
    else
    {
        Console.WriteLine("donwload canceled");
    }
}

class Film
{
    public int Id { get; set; }
    public byte[] Chunck { get; set; }
    public string Name { get; set; }
    public DateTime Downloaded { get; set; }
    public int DownloadedBytes { get; set; }
    public int DownloadedTotalBytes { get; set; }
}