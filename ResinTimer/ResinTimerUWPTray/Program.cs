namespace ResinTimerUWPTray
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

            string mutexName = "ResinTimerUWPTrayMutex";

            if (!Mutex.TryOpenExisting(mutexName, out _))
            {
                Mutex mutex = new(false, mutexName);
                ApplicationConfiguration.Initialize();
                Application.Run(new ResinTimerUWPTrayContext());

                mutex.Close();
            }
        }
    }
}