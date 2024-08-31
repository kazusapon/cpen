// See https://aka.ms/new-console-template for more information
using Microsoft.VisualBasic;
using System.Diagnostics;

var path_args = args[0];
var keyword_args = args.Skip(1).Select(x => x.ToLower()).ToArray();

var fileOpener = new FileOpener(path_args, keyword_args);
var IsKeywordContainsFiles = await fileOpener.FileContentSearch();

var idx = 0;
foreach(var file in IsKeywordContainsFiles)
{
    Console.WriteLine($"{idx}: {file}");
    idx++;
}

Console.Write("開くファイルを指定してください: ");
string? fileIndex = Console.ReadLine();

var vscExePath = Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA") ?? "", "Programs", "Microsoft VS Code", "Code.exe");
if (File.Exists(vscExePath))
{
    var application = new ProcessStartInfo();
    application.FileName = vscExePath;
    application.Arguments = IsKeywordContainsFiles.ToArray().ElementAt(int.Parse(fileIndex ?? "0"));

    Process.Start(application);
}
else
{
    Console.WriteLine("Visual Stadio Codeの実行ファイルが見つかりませんでした");
}

