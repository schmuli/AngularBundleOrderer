using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web.Optimization;

namespace Optimization.Orderers
{
    public class AngularOrderer : IBundleOrderer
    {
        private readonly string _baseAppPath;

        public AngularOrderer(string baseAppPath)
        {
            _baseAppPath = baseAppPath;
            if (_baseAppPath[0] == '~')
            {
                _baseAppPath = _baseAppPath.Substring(1);
            }
        }

        public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
        {
            var stats = files.Select(Stats).ToList();

            stats.Sort(SortFiles);

            return stats.Select(a => a.File);
        }

        private int SortFiles(FileStats a, FileStats b)
        {
            var dirSort = string.CompareOrdinal(a.Directory, b.Directory);
            if (dirSort != 0)
            {
                return dirSort;
            }

            var typeSort = a.Type.CompareTo(b.Type);
            if (typeSort != 0)
            {
                return typeSort;
            }

            var nameSort = a.Name.CompareTo(b.Name);
            return nameSort;
        }

        private FileStats Stats(BundleFile file)
        {
            var path = file.VirtualFile.VirtualPath;
            var name = Path.GetFileNameWithoutExtension(path);

            var directory = path.Remove(path.IndexOf(name) - 1);
            if (directory == _baseAppPath)
            {
                directory = char.ConvertFromUtf32(0xFFF);
            }

            return new FileStats
            {
                File = file,
                Directory = directory,
                Name = name,
                Type = GetFileType(name)
            };
        }

        private static FileType GetFileType(string name)
        {
            if (name == "module" || name == "app")
            {
                return FileType.Module;
            }

            var type = Path.GetExtension(name).Substring(1);
            FileType parsedType;
            if (Enum.TryParse(type, true, out parsedType))
            {
                return parsedType;
            }
            return FileType.Other;
        }

        private enum FileType
        {
            Service,
            Controller,
            Directive,
            Module,
            Other
        }

        private class FileStats
        {
            public BundleFile File { get; set; }
            public string Directory { get; set; }
            public string Name { get; set; }
            public FileType Type { get; set; }
        }
    }

    public static class BundleCollectionExtensions
    {
        public static void AddAngularApp(this BundleCollection bundles, string bundleVirtualPath, string baseAppPath)
        {
            var bundle = new ScriptBundle(bundleVirtualPath);
            bundle.Orderer = new AngularOrderer(baseAppPath);
            bundle.IncludeDirectory(baseAppPath, "*.js", true);
            bundles.Add(bundle);
        }
    }
}
