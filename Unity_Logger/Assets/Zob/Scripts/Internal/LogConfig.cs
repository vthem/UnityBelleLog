using System.Collections.Generic;

namespace Zob.Internal
{
    public struct LogConfigEntry
    {
        public LogLevel minLogLevel;
        public bool isEnable;
    }

    [System.Serializable]
    public class LogConfigDomainFilter
    {
        public string domainMatch;
        public List<string> restrictWriter;
    }

    [System.Serializable]
    public class LogConfigModel
    {
        public bool useThread;
        public List<LogConfigDomainFilter> filters;
    }

    //public class LogConfig : LogSingleton<LogConfig>
    //{
    //    private LogConfigModel _model;

    //    public LogConfig()
    //    {
    //    }

    //    public LogConfigEntry GetEntry(string domain)
    //    {
    //        LogConfigEntry entry;
    //        entry.isEnable = false;
    //        entry.minLogLevel = LogLevel.Fatal;
    //        return entry;
    //    }

    //    public static LogConfigModel DefaultModel
    //    {
    //        get
    //        {
    //            var model = new LogConfigModel();
    //            model.filters = new List<LogConfigDomainFilter>();

    //            var filter = new LogConfigDomainFilter();
    //            filter.domainMatch = "*";
    //            model.filters.Add(filter);
    //            return model;
    //        }
    //    }
    //}
}