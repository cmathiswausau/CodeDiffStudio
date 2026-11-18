namespace CodeDiffStudio.Core
{
    /// <summary>
    /// Specifies whether an item is unchanged, added, removed, or modified.
    /// </summary>
    public enum DifferenceType
    {
        /// <summary>
        /// The compared items are unchanged.
        /// </summary>
        Unchanged,

        /// <summary>
        /// The compared items are added.
        /// </summary>
        Added,

        /// <summary>
        /// The compared items are removed.
        /// </summary>
        Removed,

        /// <summary>
        /// The compared items are modified.
        /// </summary>
        Modified,
    }
}
