namespace ricaun.RevitTest.TestAdapter.Services
{
    public class RevitTestProcessStart : ProcessStart
    {
        public RevitTestProcessStart(string processPath) : base(processPath)
        {
        }

        private RevitTestProcessStart SetRevitArgument(string name, object value = null)
        {
            SetArgument(name, value);
            return this;
        }
        public RevitTestProcessStart SetFile(string file) => SetRevitArgument("file", file);
        public RevitTestProcessStart SetRead(bool read = true)
        {
            if (!read) return this;
            return SetRevitArgument("read");
        }
        public RevitTestProcessStart SetRevitVersion(int revitVersion) => SetRevitArgument("version", revitVersion);
        public RevitTestProcessStart SetOutput(string output) => SetRevitArgument("output", output);
        public RevitTestProcessStart SetOutputConsole() => SetOutput("console");
        public RevitTestProcessStart SetOpen(bool open = true)
        {
            if (!open) return this;
            return SetRevitArgument("open");
        }
        public RevitTestProcessStart SetClose(bool close = true)
        {
            if (!close) return this;
            return SetRevitArgument("close");
        }
        public RevitTestProcessStart SetTestFilter(string[] testFilters)
        {
            return SetTestFilter(string.Join(",", testFilters));
        }
        public RevitTestProcessStart SetTestFilter(string testFilter)
        {
            if (string.IsNullOrEmpty(testFilter))
                return this;
            return SetRevitArgument("test", testFilter);
        }
    }

}