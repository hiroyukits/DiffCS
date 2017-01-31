namespace DiffCS
{
    public class Definition
    {
        /// <summary>
        /// for Result Data
        /// </summary>
        public class ResultTreeNode
        {
            public DIFF_TYPE DiffType { get; set; }

            public object DiffObjcet { get; set; }

            public int OldDataIndex { get; set; }
            public int NewDataIndex { get; set; }
        }

        /// <summary>
        /// for EditGraph Trace
        /// </summary>
        public class TreeNode
        {
            public int PropK { get; set; }

            public int PosY { get; set; }

            public TreeNode PrevNode { get; set; }

        }

        public enum DIFF_TYPE
        {
            DIFF_SAME = 0,
            DIFF_ADD = 1,
            DIFF_DEL = 2
        }

    }
}
