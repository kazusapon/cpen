using System.ComponentModel;
using System.Reflection.PortableExecutable;
using System.Text;
using Ude;

public class FileOpener
{
    private readonly string _path;

    private readonly string[] _keywords;

    private readonly Encoding _utf8 = Encoding.UTF8;

    private readonly Encoding _sjis;

    public FileOpener(string path, string[] keywords)
    {
        _path = path;
        _keywords = keywords;

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // SJIS使う必殺技らしい
        _sjis = Encoding.GetEncoding("shift-jis");
    }

    public async Task<IList<string>> FileContentSearch()
    {
        var files = new List<string>();
        foreach(string file in GetFiles())
        {
            using(var streamReader = new StreamReader(file))
            {
                // 文字コード判別
                var detector = new CharsetDetector();
                detector.Feed(streamReader.BaseStream);
                detector.DataEnd();
                var charSetName = detector.Charset;

                streamReader.BaseStream.Seek(0, SeekOrigin.Begin);

                var content = await streamReader.ReadToEndAsync();
                var convertedUtf8String = ConvertShiftJisToUtf8(content, charSetName);
                if (_keywords.All(keyword => convertedUtf8String.ToLower().Contains(keyword)))
                {
                    files.Add(file);
                }
            }    
        }

        return files;
    }

    private IList<string> GetFiles()
    {
        return Directory.EnumerateFiles(_path, "*", SearchOption.AllDirectories)
                .Where(f => File.Exists(f))
                .Select(f => f.ToString())
                .ToList();
    }

    private string ConvertShiftJisToUtf8(string content, string charSetName)
    {
        if (charSetName == "UTF-8")
            return content;

        var bytes = Encoding.UTF8.GetBytes(content);
        var utf8Bytes = Encoding.Convert(_sjis, _utf8, bytes);

        return _utf8.GetString(utf8Bytes);
    }
}
