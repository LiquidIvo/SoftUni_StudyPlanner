namespace StudyPlanner.GCommon
{
    public static class ApplicationConstants
    {
        public const string DateFormatForStudyTask = "yyyy-MM-dd";
        public const string DateFormatForStudySessionForCalculations = "yyyy-MM-ddTHH:mm";
        public const string DateFormatForStudySession = "dd MMM yyyy HH:mm";


        public const string AdminRoleName = "Admin";
        public const string AdminAreaName = "Admin";    
        public const string UserRoleName = "User";
        public const string AdminOrUser = AdminRoleName + "," + UserRoleName;
    }
}
