namespace StudyPlanner.Common
{
    public static class EntityValidation
    {
        // StudyTask
        public const int StudyTaskTitleMinLength = 1;
        public const int StudyTaskTitleMaxLength = 100;
        public const int StudyTaskDescriptionMaxLength = 500;

        // Category 
        public const int CategoryNameMinLength = 1;
        public const int CategoryNameMaxLength = 50;
       
        public const int CategoryColorLength = 7;

        //  Subject 
        public const int SubjectNameMinLength = 1;
        public const int SubjectNameMaxLength = 50;

        //StudySession 
        public const int StudySessionNotesMaxLength = 300;
    }
}
