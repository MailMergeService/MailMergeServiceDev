namespace XServices.Common.Models
{
    public class EmailMergeVar
    {
        /// <summary>
        /// To name
        /// </summary>
        public string ToName { set; get; }

        /// <summary>
        /// Email Address
        /// </summary>
        public string Email { set; get; }

        /// <summary>
        /// Model Content
        /// </summary>
        public dynamic Model { set; get; }
    }
}