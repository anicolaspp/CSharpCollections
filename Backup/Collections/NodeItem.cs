namespace Nikos.Collections.Advance.HD_Engine
{
    /// <summary>
    /// Represent a Generic parameter of B-Tree
    /// The derived clases needen a default constructor for good functionality whith the B-Tree
    /// </summary>
    public abstract class NodeItem
    {
        /// <summary>
        /// Get bytes of representation of class.
        /// </summary>
        /// <returns></returns>
        public abstract byte[] ToByteArray();

        /// <summary>
        /// Load to data-object data passed as parameters
        /// </summary>
        /// <param name="data">Data for load internal objects</param>
        public abstract void LoadFromByteArray(byte[] data);
    }
}
