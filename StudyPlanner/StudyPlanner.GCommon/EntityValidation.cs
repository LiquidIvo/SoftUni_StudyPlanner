namespace StudyPlanner.GCommon
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

        // Identity
        public const int FullNameMaxLength = 50;


        //Resource
        public const int ResourceTitleMaxLength = 100;
        public const int ResourceUrlMaxLength = 500;
        public const int ResourceDescriptionMaxLength = 300;
        public const int ResourceTitleMinLength = 1;

    }
}
