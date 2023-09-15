namespace LogLib
{
    public class Logger
    {
        /// <summary>
        /// 日志文件名。
        /// Name of log file.
        /// </summary>
        private string LogName = string.Empty;
        /// <summary>
        /// 日志存放文件夹路径。
        /// The folder path of the log store.
        /// </summary>
        private string LogPath = string.Empty;
        /// <summary>
        /// 单个日志文件最大大小（单位：kb）。
        /// Single log file max size in kb.
        /// </summary>
        private int LogSize = 1024;
        /// <summary>
        /// 单个日志的最大存储份数。
        /// Single log type max copies.
        /// </summary>
        private int LogMaxCopies = 5;
        /// <summary>
        /// 最低输出日志级别。
        /// Lowest level of log printing in console.
        /// </summary>
        private LogLevel LogPrintLevel = LogLevel.DEBUG;
        /// <summary>
        /// 最低日志写入级别。
        /// Lowest level of log writing in file.
        /// </summary>
        private LogLevel LogWriteLevel = LogLevel.DEBUG;
        /// <summary>
        /// 文件锁。
        /// Locker.
        /// </summary>
        private object LogLock = new object();
        /// <summary>
        /// 上次进行切片的时间。
        /// Time of last slicing up file.
        /// </summary>
        private DateTime LastSliceTime = DateTime.MinValue;

        private Logger(string logName, string logPath = "Log")
        {
            LogName = logName;
            LogPath = logPath;
        }

        /// <summary>
        /// 基于指定的日志名创建Logger。
        /// Create logger with log name.
        /// </summary>
        /// <param name="logName">日志名 / log name</param>
        /// <returns>Logger</returns>
        public static Logger GetLogger(string logName, string logPath = "Log")
        {
            return new Logger(logName, logPath);
        }
        /// <summary>
        /// 基于自动解析的类名创建Logger。
        /// Create logger with log name what has been parsed from class.
        /// </summary>
        /// <param name="logOwner">带解析类名的对象 / Owner of logger</param>
        /// <returns>Logger</returns>
        public static Logger GetLogger<T>(T logOwner, string logPath = "Log")
        {
            return new Logger(typeof(T).Name, logPath);
        }
        /// <summary>
        /// 设置Logger。
        /// Configure logger.
        /// </summary>
        /// <param name="size">单个日志文件大小限制 / single file max size</param>
        /// <param name="maxCopies">日志最大拷贝数 / max file copies</param>
        /// <param name="printLevel">最低控制台输出等级 / lowest level of log will print in console</param>
        /// <param name="writeLevel">最低写入文件等级 / lowest level of log will write in file</param>
        public void SetLogger(int size = 1024, int maxCopies = 5,
            LogLevel printLevel = LogLevel.DEBUG, LogLevel writeLevel = LogLevel.DEBUG)
        {
            LogSize = size;
            LogMaxCopies = maxCopies;
            LogPrintLevel = printLevel;
            LogWriteLevel = writeLevel;
        }
        /// <summary>
        /// 自动对日志文件进行切片，如果存档数超出设置会删除最早的拿份。
        /// Auto slice up log file, delete the earliest if copies bigger than configuration.
        /// </summary>
        private void AutoSlice(DateTime time, string fileName, string filePath)
        {
            if (time - LastSliceTime <= new TimeSpan(0, 1, 0))
                return;

            LastSliceTime = time;

            lock (LogLock)
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        long size = new FileInfo(filePath).Length / 1024;
                        if (size > LogSize)
                        {
                            File.Move(filePath, Path.Combine(LogPath,
                                $"{fileName}.{DateTime.Now:yyMMddHHmmssfff}"));
                        }
                    }

                    string[] fileNames = Directory.GetFiles(LogPath, $"{fileName}.*");
                    if (fileNames != null && fileNames.Length > LogMaxCopies)
                    {
                        Array.Sort(fileNames);
                        File.Delete(fileNames[1]);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
        /// <summary>
        /// 内部函数，写日志。
        /// Inner func, write log file.
        /// </summary>
        /// <param name="level">日志级别 / log level</param>
        /// <param name="funcName">函数名 / func name</param>
        /// <param name="info">日志内容 / log content</param>
        private void Log(LogLevel level, string funcName, string info)
        {
            try
            {
                if (!Directory.Exists(LogPath))
                    Directory.CreateDirectory(LogPath);

                DateTime now = DateTime.Now;
                string fileName = $"{LogName}.log";
                string filePath = Path.Combine(LogPath, fileName);
                string content = $"{now:yyyy-MM-dd HH:mm:ss.fff} | " +
                    $"{level} | {funcName} | {info}";

                if (level >= LogPrintLevel)
                    Console.WriteLine(content);

                if (level >= LogWriteLevel)
                {
                    AutoSlice(now, fileName, filePath);
                    lock (LogLock)
                        File.AppendAllText(filePath, content + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        /// <summary>
        /// 写Debug日志。
        /// Write log for debug level.
        /// </summary>
        /// <param name="funcName">函数名 / func name</param>
        /// <param name="info">日志内容 / log content</param>
        public void Debug(string funcName, string info)
        {
            Log(LogLevel.DEBUG, funcName, info);
        }
        /// <summary>
        /// 写Info日志。
        /// Write log for info level.
        /// </summary>
        /// <param name="funcName">函数名 / func name</param>
        /// <param name="info">日志内容 / log content</param>
        public void Info(string funcName, string info)
        {
            Log(LogLevel.INFO, funcName, info);
        }
        /// <summary>
        /// 写Warn日志。
        /// Write log for warn level.
        /// </summary>
        /// <param name="funcName">函数名 / func name</param>
        /// <param name="info">日志内容 / log content</param>
        public void Warn(string funcName, string info)
        {
            Log(LogLevel.WARN, funcName, info);
        }
        /// <summary>
        /// 写Error日志。
        /// Write log for error level.
        /// </summary>
        /// <param name="funcName">函数名 / func name</param>
        /// <param name="info">日志内容 / log content</param>
        public void Error(string funcName, string info)
        {
            Log(LogLevel.ERROR, funcName, info);
        }
    }
}