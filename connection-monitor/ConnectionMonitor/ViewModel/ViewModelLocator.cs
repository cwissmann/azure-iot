using Windows.System.Profile;

namespace ConnectionMonitor
{
    public class ViewModelLocator
    {
        private StatusViewModel statusViewModel = null;

        private FezHatHandler fezHatHandler = null;

        public ViewModelLocator()
        {
            if (AnalyticsInfo.VersionInfo.DeviceFamily.Equals("Windows.IoT"))
            {
                this.fezHatHandler = new FezHatHandler();
            }
        }

        #region Tokens

        public static object StatusUpdateToken = new object();

        #endregion

        public StatusViewModel Status
        {
            get
            {
                if (this.statusViewModel == null)
                {
                    this.statusViewModel = new StatusViewModel();
                }

                return this.statusViewModel;
            }
        }
    }
}
